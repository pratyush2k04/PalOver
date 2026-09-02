using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class HandoverQueriesService : IHandoverQueriesService
{
    private readonly IHandoverRepository _handoverRepository;
    private readonly ICommitmentRepository _commitmentRepository;
    private readonly IHandoverQueryRepository _queryRepository;
    private readonly IHandoverQueryMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public HandoverQueriesService(
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        IHandoverQueryRepository queryRepository,
        IHandoverQueryMessageRepository messageRepository,
        IUserRepository userRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _queryRepository = queryRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _auditLogService = auditLogService;
        _notificationService = notificationService;
    }

    private async Task<HandoverInfo> GetHandoverOrThrowAsync(int engagementId)
    {
        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);

        if (handover == null)
            throw new HandoverNotFoundException(engagementId);

        return handover;
    }

    public async Task<List<HandoverQueryDto>> GetQueriesAsync(int engagementId)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);
        var queries = await _queryRepository.GetByHandoverIdAsync(handover.HandoverId);

        return MapToDtos(queries);
    }

    public async Task<HandoverQueryDto> RaiseQueryAsync(int engagementId, CreateHandoverQueryRequest request, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);

        var status = await _handoverRepository.GetStatusByIdAsync(handover.StatusId);
        if (status?.StatusName != HandoverStatusNames.Submitted)
            throw new HandoverNotYetSubmittedException();

        var fieldSection = Enum.Parse<HandoverQueryFieldSectionEnum>(request.FieldSection);

        if (fieldSection == HandoverQueryFieldSectionEnum.Commitment)
        {
            var commitment = await _commitmentRepository.GetByIdAsync(request.ReferenceId!.Value, handover.HandoverId);
            if (commitment == null)
                throw new CommitmentNotFoundException(request.ReferenceId.Value);
        }

        var actorUserId = int.Parse(userIdStr);

        var query = new HandoverQuery
        {
            HandoverId = handover.HandoverId,
            FieldSectionId = (int)fieldSection,
            ReferenceId = request.ReferenceId,
            RaisedByUserId = actorUserId,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };
        await _queryRepository.AddAsync(query);
        await _queryRepository.SaveChangesAsync();

        var initialMessage = new HandoverQueryMessage
        {
            QueryId = query.Id,
            SenderUserId = actorUserId,
            MessageType = HandoverQueryMessageType.Question,
            MessageText = request.Question,
            SequenceNumber = 1,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };
        await _messageRepository.AddAsync(initialMessage);
        await _messageRepository.SaveChangesAsync();

        await _auditLogService.LogAsync(
            AuditActionEnum.HandoverQueryRaised,
            new Dictionary<string, object?>
            {
                ["QueryId"] = query.Id,
                ["HandoverId"] = handover.HandoverId,
                ["EngagementId"] = engagementId,
                ["ActorUserId"] = actorUserId
            },
            actorUserId);

        await _notificationService.CreateFromTemplateAsync(
            "HandoverQueryRaised",
            engagementId,
            RoleEnum.PreSales,
            JsonSerializer.Serialize(new HandoverQueryRaisedMapper.HandoverQueryRaisedData(
                QueryId: query.Id, HandoverId: handover.HandoverId, TriggerUserId: actorUserId, EngagementId: engagementId)),
            DateTime.UtcNow);

        return await GetSingleDtoOrThrowAsync(engagementId, query.Id);
    }

    public async Task<HandoverQueryDto> AddMessageAsync(int engagementId, int queryId, AddHandoverQueryMessageRequest request, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);

        var query = await _queryRepository.GetByIdAsync(queryId, handover.HandoverId);
        if (query == null)
            throw new HandoverQueryNotFoundException(queryId);

        if (query.IsResolved)
            throw new HandoverQueryAlreadyResolvedException();

        var actorUserId = int.Parse(userIdStr);
        var actorUser = await _userRepository.GetByIdAsync(actorUserId)
            ?? throw new UserNotFoundException(actorUserId);

        var latestMessage = query.Messages
            .OrderByDescending(m => m.SequenceNumber)
            .FirstOrDefault();

        HandoverQueryMessageType messageType;
        if (latestMessage == null || latestMessage.MessageType == HandoverQueryMessageType.Response)
        {
            // Expected turn is Question (PM)
            if (actorUser.Role.Name != RoleConstants.ProjectManagement)
                throw new HandoverQueryInvalidTurnException(HandoverQueryConstants.InvalidTurnExpectedResponse);

            messageType = HandoverQueryMessageType.Question;
        }
        else
        {
            // Expected turn is Response (Pre Sales)
            if (actorUser.Role.Name != RoleConstants.PreSales)
                throw new HandoverQueryInvalidTurnException(HandoverQueryConstants.InvalidTurnExpectedQuestion);

            messageType = HandoverQueryMessageType.Response;
        }

        var nextSeq = await _messageRepository.GetNextSequenceNumberAsync(queryId);

        var message = new HandoverQueryMessage
        {
            QueryId = queryId,
            SenderUserId = actorUserId,
            MessageType = messageType,
            MessageText = request.MessageText,
            SequenceNumber = nextSeq,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };
        await _messageRepository.AddAsync(message);
        await _messageRepository.SaveChangesAsync();

        query.UpdatedBy = userIdStr;
        await _queryRepository.SaveChangesAsync();

        if (messageType == HandoverQueryMessageType.Response)
        {
            await _auditLogService.LogAsync(
                AuditActionEnum.HandoverQueryResponded,
                new Dictionary<string, object?>
                {
                    ["QueryId"] = queryId,
                    ["HandoverId"] = handover.HandoverId,
                    ["EngagementId"] = engagementId,
                    ["ActorUserId"] = actorUserId
                },
                actorUserId);

            await _notificationService.CreateFromTemplateAsync(
                "HandoverQueryRaised",
                engagementId,
                RoleEnum.ProjectManagement,
                JsonSerializer.Serialize(new HandoverQueryRaisedMapper.HandoverQueryRaisedData(
                    QueryId: queryId, HandoverId: handover.HandoverId, TriggerUserId: actorUserId, EngagementId: engagementId)),
                DateTime.UtcNow);
        }
        else
        {
            await _auditLogService.LogAsync(
                AuditActionEnum.HandoverQueryRaised,
                new Dictionary<string, object?>
                {
                    ["QueryId"] = queryId,
                    ["HandoverId"] = handover.HandoverId,
                    ["EngagementId"] = engagementId,
                    ["ActorUserId"] = actorUserId
                },
                actorUserId);

            await _notificationService.CreateFromTemplateAsync(
                "HandoverQueryRaised",
                engagementId,
                RoleEnum.PreSales,
                JsonSerializer.Serialize(new HandoverQueryRaisedMapper.HandoverQueryRaisedData(
                    QueryId: queryId, HandoverId: handover.HandoverId, TriggerUserId: actorUserId, EngagementId: engagementId)),
                DateTime.UtcNow);
        }

        return await GetSingleDtoOrThrowAsync(engagementId, queryId);
    }

    public async Task<HandoverQueryDto> ToggleResolutionAsync(int engagementId, int queryId, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);

        var query = await _queryRepository.GetByIdAsync(queryId, handover.HandoverId);
        if (query == null)
            throw new HandoverQueryNotFoundException(queryId);

        var newIsResolved = !query.IsResolved;

        if (newIsResolved)
        {
            var orderedMessages = query.Messages.OrderBy(m => m.SequenceNumber).ToList();
            if (!orderedMessages.Any(m => m.MessageType == HandoverQueryMessageType.Response))
                throw new HandoverQueryNoResponsesException();

            if (orderedMessages.Last().MessageType == HandoverQueryMessageType.Question)
                throw new HandoverQueryPendingResponseException();
        }

        query.IsResolved = newIsResolved;
        query.ResolvedByUserId = newIsResolved ? int.Parse(userIdStr) : null;
        query.ResolvedAt = newIsResolved ? DateTime.UtcNow : null;
        query.UpdatedBy = userIdStr;

        await _queryRepository.SaveChangesAsync();

        if (newIsResolved)
        {
            var actorUserId = int.Parse(userIdStr);
            await _auditLogService.LogAsync(
                AuditActionEnum.HandoverQueryResolved,
                new Dictionary<string, object?>
                {
                    ["QueryId"] = queryId,
                    ["HandoverId"] = handover.HandoverId,
                    ["EngagementId"] = engagementId,
                    ["ActorUserId"] = actorUserId
                },
                actorUserId);
        }

        return await GetSingleDtoOrThrowAsync(engagementId, queryId);
    }

    private async Task<HandoverQueryDto> GetSingleDtoOrThrowAsync(int engagementId, int queryId)
    {
        var dtos = await GetQueriesAsync(engagementId);
        return dtos.First(d => d.Id == queryId);
    }

    private static List<HandoverQueryDto> MapToDtos(List<HandoverQuery> queries)
    {
        return queries.Select(q =>
        {
            var messages = q.Messages
                .OrderBy(m => m.SequenceNumber)
                .Select(m => new HandoverQueryMessageDto
                {
                    Id = m.Id,
                    QueryId = m.QueryId,
                    SenderUserId = m.SenderUserId,
                    SenderName = m.SenderUser?.FullName ?? string.Empty,
                    SenderRole = m.SenderUser?.Role?.Name ?? string.Empty,
                    MessageType = m.MessageType.ToString(),
                    MessageText = m.MessageText,
                    SequenceNumber = m.SequenceNumber,
                    CreatedAt = m.CreatedAt
                })
                .ToList();

            var latestMessage = messages.LastOrDefault();

            return new HandoverQueryDto
            {
                Id = q.Id,
                HandoverId = q.HandoverId,
                FieldSection = q.FieldSection.Name,
                ReferenceId = q.ReferenceId,
                IsResolved = q.IsResolved,
                RaisedByName = q.RaisedByUser?.FullName ?? string.Empty,
                RaisedAt = q.CreatedAt,
                ResolvedByName = q.ResolvedByUser?.FullName,
                ResolvedByRole = q.ResolvedByUser?.Role?.Name,
                ResolvedAt = q.ResolvedAt,
                Messages = messages,
                LatestMessageType = latestMessage?.MessageType ?? string.Empty,
                CanAddFollowUp = !q.IsResolved && latestMessage?.MessageType == nameof(HandoverQueryMessageType.Response),
                CanRespond = !q.IsResolved && latestMessage?.MessageType == nameof(HandoverQueryMessageType.Question)
            };
        }).ToList();
    }
}
