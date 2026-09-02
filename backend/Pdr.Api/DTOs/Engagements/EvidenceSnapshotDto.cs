using System;
using System.Collections.Generic;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.DTOs.Engagements;

public record EvidenceSnapshotDto(
    DateTime SignedOffAt,
    string SignedOffByUserName,
    DateOnly BillingStartDate,
    List<EvidenceSnapshotItemDto> Items);

public record EvidenceSnapshotItemDto(
    string Title,
    string CategoryName,
    EvidenceSnapshotItemStatus StatusAtSignOff,
    int? EvidenceDocumentId,
    string? EvidenceDocumentFileName,
    DateTime? EvidenceSubmittedAt,
    string? EvidenceSubmittedByName);
