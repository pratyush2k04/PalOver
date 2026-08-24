namespace Pdr.Api.DTOs.Audit;

public record AuditLogLiteDto(int EngagementId, int AuditActionId, string PayloadJson, DateTime CreatedAt);
