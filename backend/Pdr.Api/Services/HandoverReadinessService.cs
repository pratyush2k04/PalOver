using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pdr.Api.DTOs.HandoverReadiness;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.HandoverReadiness;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HandoverReadinessService(
    IHandoverReadinessRepository repository,
    IEngagementRepository engagementRepository,
    IRiskService riskService,
    IDocumentRepository documentRepository,
    ICurrentUserService currentUserService) : IHandoverReadinessService
{
    public async Task<List<HandoverReadinessItemDto>> GetItemsAsync(int engagementId)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var items = await repository.GetItemsByEngagementIdAsync(engagementId);

        if (items.Count == 0)
        {
            var definitions = await repository.GetActiveDefinitionsAsync();
            var preSalesOwnerId = await repository.GetPreSalesOwnerIdAsync(engagementId) ?? 1;

            var newItems = definitions.Select(def => new HandoverReadinessItem
            {
                EngagementId = engagementId,
                Area = def.Area,
                Name = def.Name,
                OwnerId = preSalesOwnerId,
                Status = HandoverReadinessStatus.InProgress,
                HandoverReadinessDefinitionId = def.Id
            }).ToList();

            if (newItems.Count > 0)
            {
                await repository.AddItemsAsync(newItems);
                await repository.SaveChangesAsync();
                items = await repository.GetItemsByEngagementIdAsync(engagementId);
            }
        }

        return items.Select(MapToDto).ToList();
    }

    public async Task<HandoverReadinessItemDto> AddCustomItemAsync(int engagementId, AddCustomHandoverReadinessItemRequest request)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new HandoverReadinessItemNameRequiredException();

        var preSalesOwnerId = await repository.GetPreSalesOwnerIdAsync(engagementId) ?? 1;
        var targetArea = string.IsNullOrWhiteSpace(request.Area) ? "HANDOVER" : request.Area.Trim().ToUpperInvariant();

        var item = new HandoverReadinessItem
        {
            EngagementId = engagementId,
            Area = targetArea,
            Name = request.Name.Trim(),
            OwnerId = preSalesOwnerId,
            Status = HandoverReadinessStatus.InProgress,
            HandoverReadinessDefinitionId = null
        };

        await repository.AddItemAsync(item);
        await repository.SaveChangesAsync();

        var created = await repository.GetByIdAsync(item.Id)
            ?? throw new HandoverReadinessItemNotFoundException(item.Id);

        return MapToDto(created);
    }

    public async Task<HandoverReadinessItemDto> UpdateItemStatusAsync(int id, UpdateHandoverReadinessStatusRequest request)
    {
        var item = await repository.GetByIdAsync(id)
            ?? throw new HandoverReadinessItemNotFoundException(id);

        await engagementRepository.EnsureNotSignedOffAsync(item.EngagementId);

        if (request.Status == HandoverReadinessStatus.Blocked && string.IsNullOrWhiteSpace(request.BlockReason))
        {
            throw new HandoverReadinessBlockReasonRequiredException();
        }

        item.Status = request.Status;
        if (request.Status == HandoverReadinessStatus.Blocked)
        {
            item.BlockReason = request.BlockReason?.Trim();
        }
        else
        {
            item.BlockReason = null;
        }

        await repository.SaveChangesAsync();

        await riskService.SyncAutoCreatedRisksAsync(item.EngagementId);

        var updated = await repository.GetByIdAsync(id)
            ?? throw new HandoverReadinessItemNotFoundException(id);

        return MapToDto(updated);
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await repository.GetByIdAsync(id)
            ?? throw new HandoverReadinessItemNotFoundException(id);

        await engagementRepository.EnsureNotSignedOffAsync(item.EngagementId);

        item.IsDeleted = true;
        await repository.SaveChangesAsync();

        await riskService.SyncAutoCreatedRisksAsync(item.EngagementId);
    }

    public async Task<HandoverReadinessItemDto> SaveItemEvidenceAsync(int engagementId, int itemId, SaveHandoverReadinessEvidenceRequestDto dto)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var item = await repository.GetByIdAsync(itemId)
            ?? throw new HandoverReadinessItemNotFoundException(itemId);

        if (item.EngagementId != engagementId)
            throw new HandoverReadinessItemNotFoundException(itemId);

        var evidenceDocument = await documentRepository.GetByIdAsync(dto.DocumentId)
            ?? throw new DocumentNotFoundException(dto.DocumentId);

        if (evidenceDocument.EngagementId != engagementId)
            throw new EvidenceDocumentNotInEngagementException(dto.DocumentId);

        var currentUserId = currentUserService.GetUserId();
        item.EvidenceDocumentId = evidenceDocument.Id;
        item.EvidenceSubmittedAt = DateTime.UtcNow;
        item.EvidenceSubmittedById = currentUserId;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await repository.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(itemId)
            ?? throw new HandoverReadinessItemNotFoundException(itemId);

        return MapToDto(updated);
    }

    private static HandoverReadinessItemDto MapToDto(HandoverReadinessItem item)
    {
        return new HandoverReadinessItemDto
        {
            Id = item.Id,
            Area = item.Area,
            Name = item.Name,
            OwnerFullName = item.Owner?.FullName ?? "Pre-Sales",
            Status = item.Status.ToString(),
            BlockReason = item.BlockReason,
            EvidenceDocumentId = item.EvidenceDocumentId,
            EvidenceDocumentFileName = item.EvidenceDocument?.FileName,
            EvidenceSubmittedAt = item.EvidenceSubmittedAt,
            EvidenceSubmittedById = item.EvidenceSubmittedById,
            EvidenceSubmittedByName = item.EvidenceSubmittedBy?.FullName
        };
    }
}
