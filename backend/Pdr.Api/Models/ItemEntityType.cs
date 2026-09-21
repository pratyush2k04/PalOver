global using ItemEntityType = Pdr.Api.Models.ItemEntityType;

namespace Pdr.Api.Models;

/// <summary>
/// Supported entity types for item-level engagement authorization.
/// </summary>
public enum ItemEntityType
{
    AccessItem,
    EnvironmentTooling,
    ChangeRequest,
    BrownfieldFinding,
    DiscoveryFeature,
    Document,
    HandoverReadinessItem
}
