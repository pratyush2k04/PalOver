# Backend Authorization Attributes — Reference & Usage Guide

This guide explains how to make full use of the declarative authorization attributes in `Pdr.Api.Attributes`.

The authorization system provides a layered, declarative security model covering:
1. **Authentication:** Ensuring the caller has a valid JWT (`[Authorize]`).
2. **User Roles:** Enforcing user roles on endpoints (`[AuthorizeRoles]`, `[AuthorizeRolesWithAdminBypass]`).
3. **Engagement Access & Engagement Roles:** Verifying assignment or specific engagement roles on the target engagement (`[RequireEngagementAccess]`, `[RequireEngagementRole]`).
4. **Item Engagement Access & Engagement Roles:** Verifying engagement assignment or engagement roles on the engagement that *owns* a specific sub-entity (`[RequireItemEngagementAccess]`, `[RequireItemEngagementRole]`).
5. **Admin Bypass:** Allowing administrators to bypass user role or engagement role restrictions (`[AllowAdmin]`, `[AuthorizeRolesWithAdminBypass]`).

---

## 1. Summary of Attributes

| Attribute | Scope | Target | Pipeline | Description |
| :--- | :--- | :--- | :--- | :--- |
| `[AuthorizeRoles(params UserRoleEnum[])]` | User Role | Class / Method | ASP.NET Middleware | Requires caller to have at least one of the specified user roles (`PreSales`, `Leadership`, `Delivery`). Returns **401** if unauthenticated, **403** if user role is missing. |
| `[AuthorizeRolesWithAdminBypass(params UserRoleEnum[])]` | User Role + Admin | Class / Method | Middleware + Action Filter | Requires caller to have one of the specified user roles **OR** have the admin claim (`isAdmin = true`). |
| `[AllowAdmin]` | Admin Marker | Class / Method | Action Filter Metadata | Marker attribute. When placed on a controller or action, any subsequent engagement access, engagement role, or item check is bypassed if `currentUserService.IsAdmin() == true`. |
| `[RequireEngagementAccess(routeParam?)]` | Engagement Access | Class / Method | Action Filter | Enforces that the caller is assigned to the engagement identified by the route parameter (default: `"engagementId"`). Returns **403** if unassigned. |
| `[RequireEngagementRole(params EngagementRoleEnum[])]` | Engagement Role | Class / Method | Action Filter | Enforces that the caller holds at least one of the specified engagement roles on the target engagement. Returns **403** if caller lacks the engagement role. |
| `[RequireItemEngagementAccess(ItemEntityType, routeParam?)]` | Item Engagement Access | Method | Action Filter | Resolves the owning engagement of the specified item entity via `IEngagementAccessService` and verifies that the caller is assigned to that engagement. Returns **404** if item missing/deleted, **403** if unassigned. |
| `[RequireItemEngagementRole(ItemEntityType, params EngagementRoleEnum[])]` | Item Engagement Role | Method | Action Filter | Resolves the owning engagement of the specified item entity via `IEngagementAccessService` and verifies that the caller holds at least one of the required engagement roles on the owning engagement. Returns **404** if item missing/deleted, **403** if engagement role is missing. |

---

## 2. Choosing the Right Attribute Pattern

### Pattern A: Entire Controller Scoped to an Engagement
When all endpoints in a controller operate within an engagement route (e.g. `/api/engagements/{engagementId:int}/checklist`):

```csharp
[ApiController]
[Authorize]
[RequireEngagementAccess] // Enforces engagement assignment for ALL actions in this controller
[Route("api/engagements/{engagementId:int}/checklist")]
public class EngagementChecklistController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetChecklist(int engagementId) => ...;

    // Elevate permission for sensitive operations requiring specific engagement roles:
    [HttpPost("sign-off")]
    [RequireEngagementRole(EngagementRoleEnum.TechLead, EngagementRoleEnum.ProjectManager)]
    public async Task<IActionResult> SignOff(int engagementId) => ...;
}
```

### Pattern B: Allowing Administrators to Bypass Engagement Checks
Engagement managers and team leads might only be allowed on engagements they are assigned to, but administrators often need access to troubleshoot or configure items:

```csharp
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/kickoff")]
[RequireEngagementAccess]
[AllowAdmin] // Administrators can access even if not assigned to this engagement
public class KickoffController : ControllerBase
{
    [HttpPut("status")]
    [RequireEngagementRole(EngagementRoleEnum.ProjectManager)]
    [AllowAdmin] // Administrator can update status even without the ProjectManager engagement role
    public async Task<IActionResult> UpdateStatus(int engagementId) => ...;
}
```

### Pattern C: Standalone Item Endpoints (Resolving Engagement by Item ID)
When an endpoint operates on a sub-entity where the client only passes `{id}` rather than `{engagementId}` (e.g. `DELETE /api/documents/{id}`):

```csharp
[ApiController]
[Authorize]
[Route("api/documents")]
public class DocumentController : ControllerBase
{
    // Any assigned engagement member can view:
    [HttpGet("{id:int}")]
    [RequireItemEngagementAccess(ItemEntityType.Document)]
    public async Task<IActionResult> GetDocument(int id) => ...;

    // Only Tech Leads or PMs on the parent engagement can remove documents:
    [HttpDelete("{id:int}")]
    [RequireItemEngagementRole(ItemEntityType.Document, EngagementRoleEnum.TechLead, EngagementRoleEnum.ProjectManager)]
    [AllowAdmin] // Administrators can delete any document
    public async Task<IActionResult> DeleteDocument(int id) => ...;
}
```

### Pattern D: User Roles with Admin Bypass
When a feature requires specific user roles (e.g. Leadership, PreSales), but administrators can also access it:

```csharp
[ApiController]
[AuthorizeRolesWithAdminBypass(UserRoleEnum.Leadership)]
[Route("api/reports/kpi")]
public class KpiReportController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetReport() => ...;
}
```

---

## 3. Supported Entity Types in Item Authorization

The `ItemEntityType` enum maps to the corresponding repository lookups:

| `ItemEntityType` | Default Route Param | Underneath Database Resolution | Soft-Delete Filtered |
| :--- | :--- | :--- | :--- |
| `AccessItem` | `"id"` | `db.AccessItems` | `!x.IsDeleted` |
| `EnvironmentTooling` | `"id"` | `db.EnvironmentToolingItems` | `!x.IsDeleted` |
| `ChangeRequest` | `"id"` | `db.ChangeRequests` | `!x.IsDeleted` |
| `BrownfieldFinding` | `"findingId"` | `db.BrownfieldFindings` | `!x.IsDeleted` |
| `DiscoveryFeature` | `"featureId"` | `db.DiscoveryFeatureItems` $\rightarrow$ `ArchitectureValidation` | `!x.IsDeleted && !x.ArchitectureValidation.IsDeleted` |
| `Document` | `"id"` | `db.Documents` | `!x.IsDeleted` |
| `HandoverReadinessItem` | `"id"` | `db.HandoverReadinessItems` | `!x.IsDeleted` |

> [!TIP]
> If your action route uses a non-standard parameter name (e.g. `{documentId}` instead of `{id}`), pass the parameter name explicitly:
> ```csharp
> [HttpDelete("{documentId:int}")]
> [RequireItemEngagementAccess(ItemEntityType.Document, routeParameterName: "documentId")]
> ```

---

## 4. How to Add a New Item Entity Type

If a new entity is created that belongs to an engagement (e.g., `Risk`, `KickoffActionItem`):

1. **Add enum value to `Pdr.Api.Models.ItemEntityType`:**
   ```csharp
   public enum ItemEntityType
   {
       ...
       Risk
   }
   ```

2. **Add lookup in `Pdr.Api.Repositories.ItemEngagementRepository`:**
   ```csharp
   ItemEntityType.Risk => await db.Risks
       .AsNoTracking()
       .Where(x => x.Id == itemId && !x.IsDeleted)
       .Select(x => (int?)x.EngagementId)
       .FirstOrDefaultAsync(),
   ```

3. **Add 404 mapping in `Pdr.Api.Services.EngagementAccessService.ThrowNotFound`:**
   ```csharp
   ItemEntityType.Risk => new RiskNotFoundException(id),
   ```

4. **Use on your controller action:**
   ```csharp
   [HttpPut("api/risks/{id:int}")]
   [RequireItemEngagementRole(ItemEntityType.Risk, EngagementRoleEnum.TechLead, EngagementRoleEnum.ProjectManager)]
   public async Task<IActionResult> UpdateRisk(int id, ...) => ...;
   ```

---

## 5. Best Practices & Rules

1. **Always pair with `[Authorize]`:**
   Engagement access and engagement role filters assume the user identity is already authenticated. Always place `[Authorize]` at the controller level.
2. **Do not confuse user roles with engagement roles:**
   - `UserRoleEnum` represents **user roles** (`Leadership`, `PreSales`, `Delivery`), enforced via `[AuthorizeRoles]` or `[AuthorizeRolesWithAdminBypass]`.
   - `EngagementRoleEnum` represents **engagement roles** (`ProjectManager`, `TechLead`, `FullStackLead`, `DataArchitect`, etc.), enforced on specific engagements via `[RequireEngagementRole]` or `[RequireItemEngagementRole]`.
3. **Prefer Controller-level `[RequireEngagementAccess]`:**
   Placing `[RequireEngagementAccess]` on the controller class establishes a safe default so new action methods require engagement assignment automatically.
4. **`AllowMultiple = false`:**
   Do not stack multiple instances of `[AuthorizeRoles]` or `[RequireEngagementRole]` on the same endpoint. Combine the required roles into the `params` argument:
   ```csharp
   // GOOD (OR logic):
   [RequireEngagementRole(EngagementRoleEnum.TechLead, EngagementRoleEnum.ProjectManager)]

   // BAD (will error due to AllowMultiple = false):
   [RequireEngagementRole(EngagementRoleEnum.TechLead)]
   [RequireEngagementRole(EngagementRoleEnum.ProjectManager)]
   ```
