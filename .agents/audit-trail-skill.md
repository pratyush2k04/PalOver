# Audit Trail – Reference for Recording and Reading Audit Events

This guide explains how to use the central audit trail: seeding a new action type, logging an event from any service, and how reads render the final message.

You only need to touch **one file** to log an existing action from a new place, and **two files + one migration** if you're introducing a brand-new action type. Nothing else in the pipeline (pagination, rendering, the read endpoint) needs to change.

---

## Overview

The audit trail is a central, append-only log of significant actions taken by users (logging in, creating/updating/deleting entities, etc.). It is stored in two tables:

- `audit_actions` — a small, seeded lookup table of action *types* (e.g. `UserCreated`). Rarely changes.
- `audit_logs` — one row per actual *event* (who did it, which action type, when, and a free-form JSON payload). Grows continuously.

Reading the log is already fully built: `GET /api/audit-logs` returns a cursor-paginated, human-readable feed, restricted to `Leadership`/`Admin` users. **You never need to touch the controller, DTOs, or pagination logic.**

Writing to the log is a single call: inject `IAuditLogService` into your own service and call `LogAsync(...)`. That's the entire integration surface for recording a new event of an *existing* action type.

---

## 1. Seed a new `AuditAction` (only if you need a brand-new action type)

If the action type you want to log already exists (check `Models/Audit/AuditActionEnum.cs`), **skip this step entirely** and go to section 2.

Action identity is an `int` enum, and the database row's primary key must match that int exactly — it is never auto-generated (`ValueGeneratedNever()` in the entity config).

**File:** `Pdr.Api/Models/Audit/AuditActionEnum.cs`

```csharp
public enum AuditActionEnum
{
    UserLoggedIn = 1,
    UserCreated = 2,
    UserUpdated = 3,
    UserDeleted = 4,
    // Add your new value with the next unused int, e.g.:
    EngagementArchived = 5
}
```

**File:** `Pdr.Api/Data/Configuration/AuditActionConfiguration.cs`

Add a matching seed row inside the existing `HasData(...)` call:

```csharp
builder.HasData(
    new AuditAction
    {
        AuditActionId = (int)AuditActionEnum.UserLoggedIn,
        Action = "UserLoggedIn",
        Details = "User {userName} logged into the system"
    },
    // ...existing entries...
    new AuditAction
    {
        AuditActionId = (int)AuditActionEnum.EngagementArchived,
        Action = "EngagementArchived",
        Details = "Engagement {engagementName} was archived by {actorName}"
    });
```

**Important — `Details` is a template, not literal text.** The `{userName}`, `{actorName}`, `{engagementName}` placeholders are **not** filled in when the row is seeded or when a log entry is written. They are filled in later, at *read* time, by matching each `{placeholder}` against a key in the JSON payload you pass to `LogAsync` (see section 2). You can invent any placeholder names you like — there is no compile-time or database-level link between a placeholder name and anything else. Pick names that match the keys you intend to put in the payload dictionary.

Once you've added the enum value and the seed row, generate a migration (this is schema-layer work):

```bash
dotnet ef migrations add Add<YourActionName>AuditAction
```

---

## 2. Log an event from your own service

**Signature** (`Pdr.Api/Services/Interfaces/IAuditLogService.cs`):

```csharp
Task LogAsync(AuditActionEnum action, IDictionary<string, object?> payload);
```

**Example** — logging from a hypothetical service, after archiving an engagement:

```csharp
public class EngagementService(
    IEngagementRepository repository,
    ICurrentUserService currentUserService,
    IAuditLogService auditLogService) : IEngagementService
{
    public async Task ArchiveAsync(int engagementId)
    {
        var engagement = await repository.GetByIdAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        engagement.IsArchived = true;
        await repository.SaveChangesAsync();

        await auditLogService.LogAsync(AuditActionEnum.EngagementArchived, new Dictionary<string, object?>
        {
            ["engagementName"] = engagement.Name,
            ["actorName"] = currentUserService.GetFullName()
        });
    }
}
```

No DI registration is needed — `IAuditLogService` is already registered as scoped in `DependencyInjectionExtensions.cs`. Just constructor-inject it like any other service.

### What happens internally, step by step

1. **The acting user is always the current caller.** `LogAsync` resolves the user via `ICurrentUserService.GetUserId()`. There is no parameter to attribute the log entry to a different user, and no supported way to log on behalf of a system/background process — it always requires an authenticated `HttpContext`.
2. **The action row is looked up by the enum's int value.** If no seeded `AuditAction` row exists for that value (e.g. you forgot section 1, or passed a value that was never seeded), it throws a raw `InvalidOperationException`. Note this bypasses this repo's usual `AppException`-based exception convention — that's a known gap in this feature, not a pattern to copy elsewhere.
3. **The payload is serialized to JSON exactly as given, with no schema.** `PayloadJson` accepts any `IDictionary<string, object?>` — there is no fixed shape per action type, no required keys, and no size limit enforced in code. This is the main flexibility of the feature: you are not constrained to a specific DTO per action; any JSON-serializable dictionary works.
4. **The write is its own, separate save.** The repository calls `AddAsync` then its own `SaveChangesAsync` — this is **not** batched with whatever `SaveChangesAsync` your calling code already did (or is about to do). The audit entry commits independently. If your own business save fails *after* you've already called `LogAsync`, the audit entry is **not** automatically rolled back.

---

## 3. How reads and rendering work (informational — you don't need to build this)

`GET /api/audit-logs?cursor={id}&limit={n}` is already fully implemented and restricted to `Leadership`/`Admin` roles (`AuditLogController`, inline `IsInRole`/`IsAdmin` check — no attribute-based policy). It returns a `CursorPagedResult<AuditLogDto>`, newest entries first.

Internally, `AuditLogService.GetAuditLogsAsync`:

1. Fetches `limit + 1` rows ordered by `AuditLogId` descending (cursor = "give me rows with a smaller id than this"); if it got more than `limit` back, there's a next page (`HasMore = true`, `NextCursor` = the last item's id).
2. Maps each `AuditLog` row to an `AuditLogDto` by deserializing `PayloadJson` back into a dictionary and rendering the action's `Details` template against it:

```csharp
Details = templateRenderer.Render(auditLog.AuditAction?.Details ?? string.Empty, payload)
```

`TemplateRenderer.Render` is a simple loop of `template.Replace("{key}", value)` calls. Two things to know about how loose this is:

- If your payload has a key with **no matching placeholder** in the template, that key is silently ignored — no error.
- If the template has a **placeholder with no matching payload key**, it is left in the output as literal text (e.g. `"Engagement {engagementName} was archived by {actorName}"` renders with a literal `{actorName}` if that key was missing) — no error, no exception, just a visibly wrong string.

There is nothing to add here for a new action type — as soon as the `AuditAction.Details` template and your `LogAsync` payload keys line up, rendering works automatically.

---

## Summary table

| I want to... | Then do this |
|---|---|
| Log an event using an **existing** action type | Inject `IAuditLogService`, call `LogAsync(existingEnumValue, payloadDict)`. Nothing else. |
| Introduce a **brand-new** action type | Add an enum value to `AuditActionEnum`, seed a matching row in `AuditActionConfiguration`, generate a migration, then call `LogAsync` as above. |
| Read/display the audit log | Nothing to build — `GET /api/audit-logs` already exists, restricted to `Leadership`/`Admin`. |
| Change who can read the audit log | Edit the `IsInRole`/`IsAdmin` check in `AuditLogController.GetAuditLogs`. |

---

## Constraints and gotchas

- **Never call `LogAsync` with an `AuditActionEnum` value that has no seeded `AuditAction` row** — it throws `InvalidOperationException`, not a handled/logged failure.
- **`PayloadJson` has no schema.** Don't assume any particular shape when reading it back outside of `AuditLogService.MapToDto` — it's whatever dictionary the original caller passed.
- **`LogAsync` always attributes the entry to the current authenticated user.** It cannot log on behalf of another user or an unauthenticated/system/background actor.
- **The audit write commits on its own `SaveChangesAsync`,** independent of the caller's own transaction/save. Don't assume atomicity between your business write and the audit write.
- **Do not add action-specific typed fields (new columns, new DTOs per action).** The whole point of the JSON payload dictionary is to be the single, generic extensibility point for every action type. Adding typed per-action fields would break that contract — express anything action-specific through payload keys and a matching `Details` template instead.
