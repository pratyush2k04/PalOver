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
    private readonly IHandoverQueryQuestionRepository _questionRepository;
    private readonly IHandoverQueryResponseRepository _responseRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly INotificationService _notificationService;

    public HandoverQueriesService(
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        IHandoverQueryRepository queryRepository,
        IHandoverQueryQuestionRepository questionRepository,
        IHandoverQueryResponseRepository responseRepository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _queryRepository = queryRepository;
        _questionRepository = questionRepository;
        _responseRepository = responseRepository;
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

        return await MapToDtosAsync(queries);
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

        var query = new HandoverQuery
        {
            HandoverId = handover.HandoverId,
            FieldSectionId = (int)fieldSection,
            ReferenceId = request.ReferenceId,
            RaisedByUserId = int.Parse(userIdStr),
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };
        await _queryRepository.AddAsync(query);
        await _queryRepository.SaveChangesAsync();

        var question = new HandoverQueryQuestion
        {
            QueryId = query.Id,
            QuestionText = request.Question,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };
        await _questionRepository.AddAsync(question);
        await _questionRepository.SaveChangesAsync();

        var actorUserId = int.Parse(userIdStr);
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

    public async Task<HandoverQueryDto> RespondToQueryAsync(int engagementId, int queryId, RespondHandoverQueryRequest request, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);

        var query = await _queryRepository.GetByIdAsync(queryId, handover.HandoverId);
        if (query == null)
            throw new HandoverQueryNotFoundException(queryId);

        var existingResponse = await _responseRepository.GetByQueryIdAsync(queryId);
        if (existingResponse != null)
            throw new HandoverQueryAlreadyRespondedException();

        var response = new HandoverQueryResponse
        {
            QueryId = queryId,
            ResponseText = request.ResponseText,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };
        await _responseRepository.AddAsync(response);
        await _responseRepository.SaveChangesAsync();

        query.RespondedByUserId = int.Parse(userIdStr);
        query.UpdatedBy = userIdStr;
        await _queryRepository.SaveChangesAsync();

        var actorUserId = int.Parse(userIdStr);
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

        return await GetSingleDtoOrThrowAsync(engagementId, queryId);
    }

    public async Task<HandoverQueryDto> ToggleResolutionAsync(int engagementId, int queryId, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);

        var query = await _queryRepository.GetByIdAsync(queryId, handover.HandoverId);
        if (query == null)
            throw new HandoverQueryNotFoundException(queryId);

        var newIsResolved = !query.IsResolved;

        if (newIsResolved && query.Response == null)
            throw new HandoverQueryNotYetRespondedException();

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

    private async Task<List<HandoverQueryDto>> MapToDtosAsync(List<HandoverQuery> queries)
    {
        var queryIds = queries.Select(q => q.Id).ToList();
        var questionsByQueryId = await _questionRepository.GetByQueryIdsAsync(queryIds);
        var responsesByQueryId = await _responseRepository.GetByQueryIdsAsync(queryIds);

        return queries.Select(q =>
        {
            questionsByQueryId.TryGetValue(q.Id, out var question);
            responsesByQueryId.TryGetValue(q.Id, out var response);

            return new HandoverQueryDto
            {
                Id = q.Id,
                HandoverId = q.HandoverId,
                FieldSection = q.FieldSection.Name,
                ReferenceId = q.ReferenceId,
                Question = question?.QuestionText ?? string.Empty,
                RaisedByName = q.RaisedByUser.FullName,
                RaisedAt = question?.CreatedAt ?? q.CreatedAt,
                ResponseText = response?.ResponseText,
                RespondedByName = q.RespondedByUser?.FullName,
                RespondedAt = response?.CreatedAt,
                IsResolved = q.IsResolved,
                ResolvedByName = q.ResolvedByUser?.FullName,
                ResolvedByRole = q.ResolvedByUser?.Role?.Name,
                ResolvedAt = q.ResolvedAt
            };
        }).ToList();
    }
}
