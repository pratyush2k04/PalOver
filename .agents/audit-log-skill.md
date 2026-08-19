# Audit Log – Reference for Recording and Reading Audit Events

This guide explains how to use the central audit log: seeding a new action type, logging an event from any service, and how reads resolve and render the final message.

You only need to touch **one file** to log an existing action from a new place, and **3 files + one migration** if you're introducing a brand-new action type (an enum value, a seed row, and a mapper class). Nothing else in the pipeline (pagination, rendering, the read endpoint) needs to change.

---

## Overview

The audit log is a central, append-only log of significant actions taken by users (logging in, creating/updating/deleting entities, etc.). It is stored in two tables:

- `audit_actions` — a small, seeded lookup table of action *types* (e.g. `UserCreated`). Rarely changes.
- `audit_logs` — one row per actual *event* (who did it, which action type, when, and a JSON payload of ids). Grows continuously.

Reading the log is already fully built: `GET /api/audit-logs` returns a cursor-paginated, human-readable feed — every authenticated user can call it, but a non-`Leadership`/non-`Admin` caller only sees their own entries (see section 3). **You never need to touch the controller, DTOs, or pagination logic.**

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
        Action = AuditActionEnum.UserLoggedIn.ToString(),
        Details = "User {userName} logged into the system"
    },
    // ...existing entries...
    new AuditAction
    {
        AuditActionId = (int)AuditActionEnum.EngagementArchived,
        Action = AuditActionEnum.EngagementArchived.ToString(),
        Details = "Engagement {engagementName} was archived by {actorName}"
    });
```

**Important — `Details` is a template, not literal text, and its placeholders are filled from the *mapper's* output, not the raw payload.** The `{userName}`, `{actorName}`, `{engagementName}` placeholders are filled in at *read* time by matching each `{placeholder}` against a key in the dictionary your action's `IAuditTemplateMapper` returns — **not** against the raw JSON payload you pass to `LogAsync` (that payload holds ids; the mapper resolves those ids into the display values the template expects). You can invent any placeholder names you like as long as your mapper's returned dictionary uses the same keys.

**You also need a mapper class for a brand-new action type** — see section 2b.

**You also need to declare engagement-scope and role visibility for the new action.** Whether an action is engagement-scoped, and who may see it in an engagement's Audits feed, is not optional metadata — it's easy to forget and easy to get wrong (silently invisible, or over-exposed to roles that shouldn't see it), so it must be declared explicitly alongside the enum value, seed row, and mapper.

**File:** `Pdr.Api/Models/Audit/AuditActionVisibility.cs`

Add a matching entry to `AuditActionVisibilityRegistry.Entries`:

```csharp
[AuditActionEnum.EngagementArchived] = new(VisibleToRoles: ["ProjectManagement", "Leadership"]),
```

If the action is *not* engagement-scoped content (e.g. an account-level action like `UserLoggedIn`), use `VisibleToRoles: null` instead — this is what signals "not engagement content, the role-matrix doesn't apply." For an engagement-scoped action, default to the most restrictive role set that still makes sense and widen deliberately, not by omission. Role strings must match the `USER_ROLES` values already defined on the frontend (`frontend/src/app/shared/constants/app.constants.ts`) so the two sides can't drift. Nothing reads this registry yet — it exists so the decision is captured in one typed place, ready for the engagement Audits tab's visibility matrix to consume later.

Once you've added the enum value, seed row, mapper, and visibility registry entry, generate a migration (this is schema-layer work):

```bash
dotnet ef migrations add Add<YourActionName>AuditAction
```

---

## 2. Log an event from your own service

**Signature** (`Pdr.Api/Services/Interfaces/IAuditLogService.cs`):

```csharp
Task LogAsync(AuditActionEnum action, IDictionary<string, object?> payload, int? actingUserId = null);
```

**The payload holds ids, not resolved display strings.** `LogAsync` doesn't render anything itself — it just serializes whatever dictionary you pass and stores it as-is. Resolution to display values (e.g. an id to a user's full name) happens later, at read time, via the action's registered `IAuditTemplateMapper`.

**Example** — the real `UserLoggedIn` call site, from `AuthService.LoginAsync`, fired only on a successful login:

```csharp
public class AuthService(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IAuditLogService auditLogService) : IAuthService
{
    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
    {
        // ...credential validation...

        var token = jwtTokenGenerator.GenerateToken(user);

        await auditLogService.LogAsync(
            AuditActionEnum.UserLoggedIn,
            new Dictionary<string, object?> { ["UserId"] = user.Id },
            actingUserId: user.Id);

        return new AuthResultDto { IsSuccess = true, Token = token };
    }
}
```

Note the payload key is `UserId` (an id), not `userName` (a resolved string) — the opposite of what you might expect from the `Details` template's `{userName}` placeholder. The `UserLoggedInMapper` (section 2b) is what turns `UserId` into `userName` at read time. Also note `actingUserId: user.Id` is passed explicitly here — login is anonymous, so there's no authenticated caller on the request yet for `ICurrentUserService` to fall back to.

No DI registration is needed for `IAuditLogService` itself — it's already registered as scoped in `DependencyInjectionExtensions.cs`. Just constructor-inject it like any other service.

### What happens internally, step by step

1. **The acting user defaults to the current caller, via an optional override.** `LogAsync(action, payload, actingUserId: null)` resolves the user via `ICurrentUserService.GetUserId()` when `actingUserId` is omitted. Pass `actingUserId` explicitly for the rare case where the acting user isn't (yet) the authenticated caller of the current request — e.g. `AuthService.LoginAsync` passes the just-authenticated `user.Id`, since login itself is an anonymous endpoint with no JWT on the request yet. There's still no supported way to log on behalf of a true system/background process with no user at all.
2. **The action row is looked up by the enum's int value.** If no seeded `AuditAction` row exists for that value (e.g. you forgot section 1, or passed a value that was never seeded), it throws a raw `InvalidOperationException`. Note this bypasses this repo's usual `AppException`-based exception convention — that's a known gap in this feature, not a pattern to copy elsewhere.
3. **The payload is serialized to JSON exactly as given, with no schema enforced by `LogAsync` itself.** `PayloadJson` accepts any `IDictionary<string, object?>` — there is no fixed shape checked at write time. But the keys/values you choose are only useful if they match what your action's registered `IAuditTemplateMapper.MapAsync` expects to deserialize (ids, keyed to that mapper's private payload record) — not the `Details` template's placeholder names.
4. **The write is its own, separate save.** The repository calls `AddAsync` then its own `SaveChangesAsync` — this is **not** batched with whatever `SaveChangesAsync` your calling code already did (or is about to do). The audit entry commits independently. If your own business save fails *after* you've already called `LogAsync`, the audit entry is **not** automatically rolled back.

## 2b. Add a mapper (required for a brand-new action type)

Every seeded `AuditAction` needs a matching `IAuditTemplateMapper` — it's what resolves the ids in your payload into the display values the `Details` template needs. Mirrors `INotificationTemplateMapper`/the notification mappers exactly.

**File:** `Pdr.Api/Services/Audit/Mappers/<YourAction>Mapper.cs`

```csharp
using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserLoggedInMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserLoggedIn);

    private record UserLoggedInData(int UserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserLoggedInData>(data)
            ?? throw new InvalidOperationException("UserLoggedIn audit payload could not be parsed.");

        var user = await userRepository.GetByIdAsync(payload.UserId);

        return new Dictionary<string, object?>
        {
            ["userName"] = user?.FullName
        };
    }
}
```

`TemplateKey` must exactly match the `AuditAction.Action` value, which is always `AuditActionEnum.<YourAction>.ToString()` — using `nameof(AuditActionEnum.<YourAction>)` keeps them in sync automatically.

Then register it in `Pdr.Api/Services/Audit/AuditMapperRegistry.cs`:

```csharp
public static class AuditMapperRegistry
{
    public static IServiceCollection AddAuditMappers(this IServiceCollection services)
    {
        services.AddScoped<IAuditTemplateMapper, UserLoggedInMapper>();
        // ...existing mappers...
        services.AddScoped<IAuditTemplateMapper, YourNewMapper>();

        return services;
    }
}
```

No other DI changes needed — `TemplateMapperResolver<IAuditTemplateMapper>` is already registered and picks up every `IAuditTemplateMapper` in the container via `IEnumerable<IAuditTemplateMapper>`.

---

## 3. How reads and rendering work (informational — you don't need to build this)

`GET /api/audit-logs?cursor={id}&limit={n}` is already fully implemented. Every authenticated user can call it (`AuditLogController`, `[Authorize]`), but the rows returned are scoped: `Leadership`/`Admin` callers (inline `IsInRole`/`IsAdmin` check, computed in `AuditLogService.GetAuditLogsAsync`) see every user's entries; everyone else only sees rows where they were the acting user. It returns a `CursorPagedResult<AuditLogDto>`, newest entries first.

Internally, `AuditLogService.GetAuditLogsAsync`:

1. Computes whether the caller is elevated (`Leadership` or `Admin`) and passes `null` (no filter) or the caller's own user id into `repository.GetPageAsync(cursor, limit, scopedUserId)`.
2. Fetches `limit + 1` rows ordered by `AuditLogId` descending (cursor = "give me rows with a smaller id than this"); if it got more than `limit` back, there's a next page (`HasMore = true`, `NextCursor` = the last item's id).
3. Maps each `AuditLog` row to an `AuditLogDto` by resolving the row's mapper and rendering against its output:

```csharp
var mapper = mapperResolver.Resolve(auditLog.AuditAction.Action)
    ?? throw new InvalidOperationException($"No IAuditTemplateMapper registered for action {actionKey}.");

var values = await mapper.MapAsync(auditLog.PayloadJson);

Details = templateRenderer.Render(auditLog.AuditAction?.Details ?? string.Empty, values);
```

`TemplateRenderer.Render` is a simple loop of `template.Replace("{key}", value)` calls over the mapper's returned dictionary. Two things to know about how loose this is:

- If the mapper's returned dictionary has a key with **no matching placeholder** in the template, that key is silently ignored — no error.
- If the template has a **placeholder with no matching key** in the mapper's output, it is left in the output as literal text (e.g. `"Engagement {engagementName} was archived by {actorName}"` renders with a literal `{actorName}` if that key was missing) — no error, no exception, just a visibly wrong string.

There is nothing to add here for a new action type — as soon as your mapper is registered and its output keys line up with the `AuditAction.Details` template's placeholders, rendering works automatically.

---

## Summary table

| I want to... | Then do this |
|---|---|
| Log an event using an **existing** action type | Inject `IAuditLogService`, call `LogAsync(existingEnumValue, idPayloadDict)`. Nothing else. |
| Introduce a **brand-new** action type | Add an enum value to `AuditActionEnum`, seed a matching row in `AuditActionConfiguration`, add an `IAuditTemplateMapper` implementation and register it in `AuditMapperRegistry`, **declare its engagement-scope and role visibility in `AuditActionVisibilityRegistry`**, generate a migration, then call `LogAsync` as above. **Also add the matching value to the frontend's `AuditAction` type** (see below). |
| Read/display the audit log | Nothing to build — `GET /api/audit-logs` already exists; every authenticated user can call it, scoped to their own entries unless `Leadership`/`Admin`. Pass `engagementId` to scope to one engagement (see engineering note below). |
| Change who can read the full (unscoped) audit log, or which rows non-elevated users see | Edit the `isElevated`/`scopedUserId` logic in `AuditLogService.GetAuditLogsAsync`. |
| Declare who may see an action in an engagement's Audits feed | Add/update its entry in `AuditActionVisibilityRegistry.Entries` (`Pdr.Api/Models/Audit/AuditActionVisibility.cs`). |

**Engineering note — `engagementId` filtering isn't indexed.** `GetAuditLogsAsync`'s optional `engagementId` parameter filters on a value extracted from `PayloadJson` at read time (see `MapToDtoAsync`/`ExtractEngagementId`), not a database column. There's no index to push the filter into SQL, so a scoped request scans batches of rows (bounded by `MaxScanRows`) rather than doing a `WHERE` on an indexed column. This is deliberate — see the engagement Audits tab's plan for why a schema change was ruled out — but means very high audit-log volumes could see a scoped request return `HasMore: true` without finding `limit` matches, requiring the client to keep paging.

### Frontend: keep `AuditAction` in sync

`AuditLogDto.Action` (`Pdr.Api/DTOs/Audit/AuditLogDto.cs`) is serialized as the enum's **string name** (e.g. `"UserLoggedIn"`), not a number — the CLR type is `string`, sourced from the seeded `AuditAction.Action` column (`AuditActionEnum.X.ToString()`), and no `JsonStringEnumConverter` is involved.

The frontend mirrors this as a string literal union in `frontend/src/app/features/audit-log/models/audit-log.model.ts`:

```ts
export type AuditAction = 'UserLoggedIn' | 'UserCreated' | 'UserUpdated' | 'UserDeleted';
```

**Whenever you add a new value to `AuditActionEnum` on the backend, add the matching string literal to `AuditAction` on the frontend in the same change.** Do not model it as a numeric TS `enum` (unlike e.g. `CadenceTypeEnum`) — the wire value is always the string name, never the int.

---

## Constraints and gotchas

- **Never call `LogAsync` with an `AuditActionEnum` value that has no seeded `AuditAction` row** — it throws `InvalidOperationException`, not a handled/logged failure.
- **A missing `IAuditTemplateMapper` for a seeded action throws `InvalidOperationException` at read time** (in `AuditLogService.MapToDtoAsync`) — every seeded `AuditAction` must have a matching registered mapper.
- **`PayloadJson` holds ids, not resolved display values, and has no schema enforced by `LogAsync` itself.** Its keys/values are whatever the original caller passed — but they're only meaningful if they match what that action's `IAuditTemplateMapper` expects to deserialize.
- **`LogAsync` attributes the entry to the current authenticated user by default, or to `actingUserId` if you pass one.** Use the override only when the acting user genuinely isn't yet the request's authenticated caller (e.g. logging in). It still can't log on behalf of a true unauthenticated/system/background actor with no user id at all.
- **The audit write commits on its own `SaveChangesAsync`,** independent of the caller's own transaction/save. Don't assume atomicity between your business write and the audit write.
- **Do not add action-specific typed fields to `AuditLog`/`AuditLogDto` (new columns, new DTOs per action).** The whole point of the JSON payload + per-action mapper is to be the single, generic extensibility point for every action type. Adding typed per-action fields would break that contract — express anything action-specific through payload ids, a matching mapper, and a `Details` template instead.
