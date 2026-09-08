using Pdr.Api.Constants;
using Pdr.Api.DTOs.ChangeRequests;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.ChangeRequest;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class ChangeRequestService(
    IChangeRequestRepository changeRequestRepository,
    IEngagementRepository engagementRepository,
    ICurrentUserService currentUserService,
    ILogger<ChangeRequestService> logger) : IChangeRequestService
{
    public async Task<List<ChangeRequestDto>> GetChangeRequestsAsync(int engagementId, string? status = null)
    {
        var engagementExists = await changeRequestRepository.EngagementExistsAsync(engagementId);
        if (!engagementExists)
        {
            throw new EngagementNotFoundException(engagementId);
        }

        ChangeRequestStatus? statusFilter = ParseStatus(status);
        var changeRequests = await changeRequestRepository.GetByEngagementIdAsync(engagementId, statusFilter);

        return changeRequests.Select(MapToDto).ToList();
    }

    public async Task<ChangeRequestDto> CreateChangeRequestAsync(int engagementId, CreateChangeRequestDto dto)
    {
        var engagementExists = await changeRequestRepository.EngagementExistsAsync(engagementId);
        if (!engagementExists)
        {
            throw new EngagementNotFoundException(engagementId);
        }

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var currentUserId = currentUserService.GetUserId();

        var entity = new ChangeRequest
        {
            EngagementId = engagementId,
            Description = dto.Description.Trim(),
            Impact = dto.Impact.Trim(),
            Status = ChangeRequestStatus.UnderReview,
            CreatedBy = currentUserId.ToString()
        };

        var created = await changeRequestRepository.AddAsync(entity);
        logger.LogInformation("Change request {ChangeRequestId} logged in Under Review for engagement {EngagementId} by user {UserId}", created.Id, engagementId, currentUserId);

        return MapToDto(created);
    }

    public async Task<ChangeRequestDto> ApproveChangeRequestAsync(int id)
    {
        var changeRequest = await changeRequestRepository.GetByIdAsync(id)
            ?? throw new ChangeRequestNotFoundException(id);

        await engagementRepository.EnsureNotSignedOffAsync(changeRequest.EngagementId);

        if (changeRequest.Status != ChangeRequestStatus.UnderReview)
        {
            throw new ChangeRequestInvalidStateTransitionException($"Cannot approve change request in status {FormatStatus(changeRequest.Status)}.");
        }

        var currentUserId = currentUserService.GetUserId();

        changeRequest.Status = ChangeRequestStatus.Approved;
        changeRequest.ReviewedById = currentUserId;
        changeRequest.ReviewedAt = DateTime.UtcNow;

        await changeRequestRepository.SaveChangesAsync();
        logger.LogInformation("Change request {ChangeRequestId} approved by user {UserId}", id, currentUserId);

        return MapToDto(changeRequest);
    }

    public async Task<ChangeRequestDto> RejectChangeRequestAsync(int id, RejectChangeRequestDto dto)
    {
        var changeRequest = await changeRequestRepository.GetByIdAsync(id)
            ?? throw new ChangeRequestNotFoundException(id);

        await engagementRepository.EnsureNotSignedOffAsync(changeRequest.EngagementId);

        if (string.IsNullOrWhiteSpace(dto.RejectionReason))
        {
            throw new RejectionReasonRequiredException();
        }

        if (changeRequest.Status != ChangeRequestStatus.UnderReview)
        {
            throw new ChangeRequestInvalidStateTransitionException($"Cannot reject change request in status {FormatStatus(changeRequest.Status)}.");
        }

        var currentUserId = currentUserService.GetUserId();

        changeRequest.Status = ChangeRequestStatus.Rejected;
        changeRequest.RejectionReason = dto.RejectionReason.Trim();
        changeRequest.ReviewedById = currentUserId;
        changeRequest.ReviewedAt = DateTime.UtcNow;

        await changeRequestRepository.SaveChangesAsync();
        logger.LogInformation("Change request {ChangeRequestId} rejected by user {UserId}", id, currentUserId);

        return MapToDto(changeRequest);
    }

    private static ChangeRequestStatus? ParseStatus(string? statusStr)
    {
        if (string.IsNullOrWhiteSpace(statusStr)) return null;

        var normalized = statusStr.Trim().Replace(" ", "").Replace("_", "").Replace("-", "");

        if (Enum.TryParse<ChangeRequestStatus>(normalized, true, out var statusEnum))
        {
            return statusEnum;
        }

        return null;
    }

    private static string FormatStatus(ChangeRequestStatus status)
    {
        return status switch
        {
            ChangeRequestStatus.UnderReview => ChangeRequestConstants.StatusDisplayNames.UnderReview,
            ChangeRequestStatus.Approved => ChangeRequestConstants.StatusDisplayNames.Approved,
            ChangeRequestStatus.Rejected => ChangeRequestConstants.StatusDisplayNames.Rejected,
            _ => status.ToString()
        };
    }

    private static ChangeRequestDto MapToDto(ChangeRequest entity)
    {
        return new ChangeRequestDto
        {
            Id = entity.Id,
            Description = entity.Description,
            Impact = entity.Impact,
            Status = FormatStatus(entity.Status),
            RejectionReason = entity.RejectionReason
        };
    }
}
