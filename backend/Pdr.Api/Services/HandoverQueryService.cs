using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HandoverQueryService : IHandoverQueryService
{
    private readonly IHandoverRepository _handoverRepository;
    private readonly ICommitmentRepository _commitmentRepository;
    private readonly IHandoverQueryRepository _queryRepository;
    private readonly IHandoverQueryQuestionRepository _questionRepository;
    private readonly IHandoverQueryResponseRepository _responseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogService _auditLogService;

    public HandoverQueryService(
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        IHandoverQueryRepository queryRepository,
        IHandoverQueryQuestionRepository questionRepository,
        IHandoverQueryResponseRepository responseRepository,
        IUserRepository userRepository,
        IAuditLogService auditLogService)
    {
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _queryRepository = queryRepository;
        _questionRepository = questionRepository;
        _responseRepository = responseRepository;
        _userRepository = userRepository;
        _auditLogService = auditLogService;
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
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };
        await _queryRepository.AddAsync(query);
        await _queryRepository.SaveChangesAsync();

        var question = new HandoverQueryQuestion
        {
            QueryId = query.QueryId,
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
                ["QueryId"] = query.QueryId,
                ["HandoverId"] = handover.HandoverId,
                ["EngagementId"] = engagementId,
                ["ActorUserId"] = actorUserId
            },
            actorUserId);

        return await GetSingleDtoOrThrowAsync(engagementId, query.QueryId);
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

    public async Task<HandoverQueryDto> ResolveQueryAsync(int engagementId, int queryId, ResolveHandoverQueryRequest request, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);

        var query = await _queryRepository.GetByIdAsync(queryId, handover.HandoverId);
        if (query == null)
            throw new HandoverQueryNotFoundException(queryId);

        var isNewlyResolved = request.IsResolved && !query.IsResolved;

        query.IsResolved = request.IsResolved;
        query.ResolvedByUserId = request.IsResolved ? int.Parse(userIdStr) : null;
        query.ResolvedAt = request.IsResolved ? DateTime.UtcNow : null;
        query.UpdatedBy = userIdStr;

        await _queryRepository.SaveChangesAsync();

        if (isNewlyResolved)
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
        return dtos.First(d => d.QueryId == queryId);
    }

    private async Task<List<HandoverQueryDto>> MapToDtosAsync(List<HandoverQuery> queries)
    {
        var queryIds = queries.Select(q => q.QueryId).ToList();
        var questionsByQueryId = await _questionRepository.GetByQueryIdsAsync(queryIds);
        var responsesByQueryId = await _responseRepository.GetByQueryIdsAsync(queryIds);

        var actorIds = questionsByQueryId.Values.Select(q => q.CreatedBy)
            .Concat(responsesByQueryId.Values.Select(r => r.CreatedBy))
            .Where(id => id != null)
            .Select(id => int.TryParse(id, out var parsed) ? parsed : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var actors = await _userRepository.GetByIdsAsync(actorIds);
        var actorsById = actors.ToDictionary(u => u.Id);

        (string Name, string? Role) ResolveActor(string? actorIdStr)
        {
            if (actorIdStr != null && int.TryParse(actorIdStr, out var id) && actorsById.TryGetValue(id, out var user))
                return (user.FullName, user.Role?.Name);

            return (actorIdStr ?? string.Empty, null);
        }

        return queries.Select(q =>
        {
            questionsByQueryId.TryGetValue(q.QueryId, out var question);
            responsesByQueryId.TryGetValue(q.QueryId, out var response);

            var raisedBy = ResolveActor(question?.CreatedBy);
            var respondedBy = response != null ? ResolveActor(response.CreatedBy) : ((string, string?)?)null;

            return new HandoverQueryDto
            {
                QueryId = q.QueryId,
                HandoverId = q.HandoverId,
                FieldSection = q.FieldSection.Name,
                ReferenceId = q.ReferenceId,
                Question = question?.QuestionText ?? string.Empty,
                RaisedByName = raisedBy.Name,
                RaisedByRole = raisedBy.Role,
                RaisedAt = question?.CreatedAt ?? q.CreatedAt,
                ResponseText = response?.ResponseText,
                RespondedByName = respondedBy?.Item1,
                RespondedByRole = respondedBy?.Item2,
                RespondedAt = response?.CreatedAt,
                IsResolved = q.IsResolved,
                ResolvedByName = q.ResolvedByUser?.FullName,
                ResolvedByRole = q.ResolvedByUser?.Role?.Name,
                ResolvedAt = q.ResolvedAt
            };
        }).ToList();
    }
}
