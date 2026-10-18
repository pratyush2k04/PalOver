# Notification System – Reference for Adding a New Notification Type

This guide explains how to introduce a new notification event into the backend.

You only need to touch four areas; the rendering pipeline, status computation, and API endpoints adapt automatically.

---

## 1. Create the template (seed data)

Templates define the default title, body, and priority for a notification type.
They are stored in the `NotificationTemplates` table and seeded via an EF migration.

**File:** `Pdr.Api/Data/Configuration/NotificationTemplateConfiguration.cs`

1. Open the configuration class.
2. Add a new entry to the `HasData(...)` list:
   - `Key` – a unique string, e.g. `"DocumentUploaded"` (must match the mapper's `TemplateKey`).
   - `Title` & `Body` – template strings with `{Variable}` placeholders.
   - `DefaultPriority` – `Low`, `Medium`, or `High`.
   - `LinkPattern` – optional. A URL template with `{Variable}` placeholders (e.g. `"/engagements/{EngagementId}/workspace/overview"`), rendered the same way as `Title`/`Body`. Omit (leave `null`) if this notification type has no natural destination to link to.
   - `CreatedAt`, `UpdatedAt`, `CreatedBy`, `IsDeleted` – always set these (see existing entries for the pattern).

```csharp
new NotificationTemplate
{
    Id = 4,
    Key = "DocumentUploaded",
    Title = "Document uploaded — {DocumentName}",
    Body = "{UploaderName} uploaded {DocumentName} to engagement {EngagementName}.",
    DefaultPriority = NotificationPriority.Medium,
    LinkPattern = "/engagements/{EngagementId}/workspace/documents",
    CreatedAt = seedTimestamp,
    UpdatedAt = seedTimestamp,
    CreatedBy = "System",
    IsDeleted = false
}
```

Note: `Notification.Link` (per-row) still exists as an explicit override — if a specific notification instance needs a different link than its template's default pattern, set `Link` directly when creating it and it takes precedence over the rendered `LinkPattern`. Most notification types won't need this; the template's `LinkPattern` is the default path.

3. Create and apply a migration:

```bash
dotnet ef migrations add AddDocumentUploadedNotificationTemplate
```

This matches the existing convention (e.g., `RoleConfiguration` is seeded the same way).

---

## 2. Write the mapper & register it

The mapper's job is two-fold:
1. **Phase 1 (ID Collection):** Collect referenced entity IDs without touching the database so the central preloader can fetch them in a single batch query.
2. **Phase 3 (Mapping):** Read the preloaded entities synchronously from `BatchLoadContext` and return a dictionary of placeholder values.

It is called **at fetch time**, not at creation time.

### 2a. Create the mapper class

**File:** `Pdr.Api/Services/Notifications/Mappers/DocumentUploadedMapper.cs`

**CRITICAL ARCHITECTURAL RULE:**
> ⚠️ **Mappers must NEVER inject repositories or services!**
> Mappers are pure, synchronous, in-memory components. All database loading is executed in bulk by the central `IBatchDataPreloader` to prevent N+1 database queries.

```csharp
using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentUploadedMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DocumentUploaded;

    public record DocumentUploadedPayload(int DocumentId, int EngagementId, int UploaderUserId);

    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentUploadedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.UploaderUserId);
        context.DocumentIds.Add(payload.DocumentId);
    }

    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentUploadedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DocumentUploaded));

        var document = context.Documents.GetValueOrDefault(payload.DocumentId);
        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var uploader = context.Users.GetValueOrDefault(payload.UploaderUserId);

        return new Dictionary<string, object?>
        {
            ["DocumentName"]   = document?.FileName,
            ["EngagementName"] = engagement?.EngagementName,
            ["UploaderName"]   = uploader?.FullName,
            ["EngagementId"]   = payload.EngagementId
        };
    }
}
```

- The `TemplateKey` must match the `Key` from the template seed (use `NotificationConstants.TemplateKeys` where available).
- **Mappers must have a parameterless constructor.** Never inject repositories or services.
- Place the payload record type **inside** the class, and make it **`public`** (not `private`) — the service that creates this notification (step 3) must construct this exact type instead of a hand-written anonymous object, so the compiler catches a property rename/typo/count mismatch between the two sides instead of it failing silently at deserialize time.
- Implement `CollectIds`: deserialize payload, register all required IDs into `context.<Entity>Ids`.
- Implement `Map`: look up preloaded entities synchronously from `context.<Entities>.GetValueOrDefault(...)` and return placeholder keys matching the template.
- The `data` parameter is the same JSON you pass to `CreateFromTemplateAsync` later. Keep it to **IDs only**, no full objects.
- If the template defines a `LinkPattern` that references a field not already in your dictionary (commonly `EngagementId`), add it to the dictionary too — the link is rendered with the same values dictionary as `Title`/`Body`.
- Throw `InvalidOperationException` when the payload cannot be parsed — do **not** return an empty dictionary (that would leave unreplaced `{Placeholder}` tokens in the rendered notification).
- Import `using Pdr.Api.Services.Templating;` for `BatchLoadContext` and `using System.Text.Json;` for `JsonSerializer`.

### 2b. Register the mapper

**File:** `Pdr.Api/Services/Notifications/NotificationMapperRegistry.cs`

Add one line to the `AddNotificationMappers` method:

```csharp
services.AddScoped<INotificationTemplateMapper, DocumentUploadedMapper>();
```

The renderer will automatically pick up the mapper when a notification with this template key is fetched.

---

## 3. Fire the notification from a service

You have **three ways** to create a notification: recipient determined by a role on an engagement, a single specific known user, or a batch of specific known users.

**Optional Snapshotting & Static Fallback:**
You can optionally provide fallback data at creation time:
1. `_initialValues` inside the JSON `data` string: A dictionary of pre-computed entity values (e.g. `["_initialValues"] = new Dictionary<string, string?> { ["DocumentName"] = "Design.pdf", ["EngagementName"] = "Falcon" }`). If an entity is deleted from the database in the future, the template renderer uses this snapshot while preserving all other live entities!
2. `staticFallback`: An optional pre-rendered static string saved directly to the `static_fallback` column on `notifications`. If ANY required entity in the template is deleted and lacks an `_initialValues` snapshot, the entire template falls back to this static string. If no static fallback is supplied, missing items render as `"<DELETED>"`.

### Option A – Notify a role on an engagement (most domain events)

Use this when the event belongs to an engagement and the recipient is whoever currently holds a particular role (e.g., Tech Lead, Project Management). The service resolves the actual user automatically and silently does nothing if the role is currently unassigned.

```csharp
await notificationService.CreateFromTemplateAsync(
    templateKey: "DocumentUploaded",
    engagementId: engagementId,
    receiverRole: RoleEnum.ProjectManagement,   // who should be notified
    data: JsonSerializer.Serialize(new DocumentUploadedMapper.DocumentUploadedPayload(
        DocumentId: document.Id,
        EngagementId: engagementId,
        UploaderUserId: currentUserId)),
    scheduledFor: DateTime.UtcNow);
```

**What happens internally:**
- Looks up the engagement assignment for the given role.
- If no user is assigned → logs and returns (no exception, no row).
- If the `templateKey` doesn't match any `NotificationTemplate` → logs and returns (no exception, no row). Unlike Option B, a missing template here is **not** an error.
- Otherwise, creates a notification addressed to that user. Its link is not set here — it's rendered from the template's `LinkPattern` (if any) at fetch time, using the mapper's returned values. There is no `link` parameter on this method; if this specific notification needs a link that differs from its template's default, set `Notification.Link` directly on the entity as an override (rare — most types don't need this).
- Priority is `priorityOverride ?? template.DefaultPriority` — omit `priorityOverride` (or pass `null`) to use the template's seeded `DefaultPriority`. Pass it when the same template needs different urgency depending on the calling context.
- Static fallback string is saved to the `static_fallback` column if provided.

**Method signature:**

```csharp
Task CreateFromTemplateAsync(
    string templateKey,
    int engagementId,
    RoleEnum receiverRole,
    string data,
    DateTime scheduledFor,
    NotificationPriority? priorityOverride = null,
    string? staticFallback = null,
    CancellationToken cancellationToken = default);
```

### Option B – Notify a specific user directly

Use this when the recipient is not (or cannot be) derived from an engagement role – for example, notifying a user that their password was changed, or that they've been mentioned in a comment. You supply the `receiverUserId` directly; no engagement lookup is performed.

```csharp
await notificationService.CreateFromTemplateAsync(
    templateKey: "PasswordChanged",
    receiverUserId: currentUserId,               // known user ID
    data: JsonSerializer.Serialize(new PasswordChangedMapper.PasswordChangedPayload(
        ChangedAt: DateTime.UtcNow)),
    scheduledFor: DateTime.UtcNow);
```

**What happens internally:**
- Looks up the template by key; throws `NotificationTemplateNotFoundException` if missing.
- Creates a notification addressed to the given user, using `priorityOverride ?? template.DefaultPriority` (same override rule as Option A — omit it to use the template's default).
- Static fallback string is saved to the `static_fallback` column if provided.

**Method signature:**

```csharp
Task CreateFromTemplateAsync(
    string templateKey,
    int receiverUserId,
    string data,
    DateTime scheduledFor,
    NotificationPriority? priorityOverride = null,
    string? staticFallback = null,
    CancellationToken cancellationToken = default);
```

### Option C – Notify a batch of specific users directly

Use this when the same event needs to notify several known users at once — e.g. everyone in a leadership group, or all watchers of a record — rather than a single role or a single user. It's the same idea as Option B but for many `receiverUserId`s in one call, without looping `CreateFromTemplateAsync` per user.

```csharp
var leadershipAssignments = await resourceRequestRepository.GetLeadershipAssignmentsAsync(engagementId);

await notificationService.CreateFromTemplateAsync(
    templateKey: "ResourceRequestRaised",
    receiverUserIds: leadershipAssignments.Select(a => a.UserId),
    data: JsonSerializer.Serialize(new ResourceRequestRaisedMapper.ResourceRequestRaisedData(
        resourceRequestId, engagementId)),
    scheduledFor: DateTime.UtcNow);
```

**What happens internally:**
- Looks up the template by key; throws `NotificationTemplateNotFoundException` if missing (same as Option B).
- Creates one `Notification` row per user ID via `AddRangeAsync`.
- Unlike Options A and B, there is **no `priorityOverride` parameter** — every row uses the template's `DefaultPriority`.
- Static fallback string is saved to the `static_fallback` column for all created rows if provided.

**Method signature:**

```csharp
Task CreateFromTemplateAsync(
    string templateKey,
    IEnumerable<int> receiverUserIds,
    string data,
    DateTime scheduledFor,
    string? staticFallback = null,
    CancellationToken cancellationToken = default);
```

---

## 4. (Rare) Ad-hoc notifications — no template, literal title/body/link

> ⚠️ **Avoid this unless absolutely necessary.** Sections 1–3 (template + mapper) are the default for every notification type, including ones that look like a "just this once" case. Only reach for this section when the notification is a genuine one-off that will never recur as an application-driven event — e.g. a manually seeded row, a support/ops-driven message, or migration-time data.
>
> Reasons to avoid it:
> - It bypasses `NotificationTemplates`, the single source of truth for notification copy — wording can no longer be centrally audited, updated, or kept consistent with every other notification type.
> - There's no mapper/placeholder rendering, so every string must be hand-assembled by the calling code at creation time. That bakes stale data into the row instead of resolving it at fetch time the way templated notifications do.
> - Ad-hoc rows are invisible to the `NotificationTemplates` inventory — anyone auditing "what notifications can this app send" won't find them there.
>
> If there's any chance this event fires again from real application code, add a template (sections 1–3) instead.

### How it works today

A `Notification` row does not require a template. `Notification.TemplateId` (and the `Template` navigation) are nullable, and `Title`, `Body`, `Link` are plain nullable strings directly on the entity (`Pdr.Api/Models/Notifications/Notification.cs`).

`NotificationService.ResolveContentAsync` branches on whether a template is attached:

```csharp
if (notification.TemplateId is null || notification.Template is null)
{
    return (notification.Title ?? string.Empty, notification.Body ?? string.Empty, notification.Link);
}
```

When there's no template, the row's literal `Title`/`Body`/`Link` are returned exactly as stored — no mapper, no `TemplateRenderer` involved.

This is an **all-or-nothing** split, not a partial override: once a `Template` **is** attached, `Title`/`Body` always come from the template (rendered or raw), and any literal value sitting in `notification.Title`/`notification.Body` is silently ignored. Only `Link` has a true per-row override that coexists with a template (`notification.Link ?? renderedLinkPattern`) — there is no equivalent override for `Title`/`Body` against an attached template.

### How to actually create one

There is currently **no** public method for this — `INotificationService`/`NotificationService` only expose the two `CreateFromTemplateAsync` overloads (template-key-based), both of which always look up a `NotificationTemplate`. If an ad-hoc notification is genuinely needed, don't repurpose `CreateFromTemplateAsync` or invent a fake template row to work around it — that defeats the point of the template system and produces confusing seed data.

Instead, follow the project's Controller → Service → Repository layering: add a small, explicitly-named method to `INotificationService`/`NotificationService` (e.g. `CreateAdHocAsync(int receiverUserId, string title, string body, string? link, NotificationPriority priority, DateTime scheduledFor, CancellationToken cancellationToken = default)`) that builds a `Notification` with `TemplateId = null` and the literal `Title`/`Body`/`Link`, then persists it via the existing `INotificationRepository.AddAsync`. Do not call the repository directly from a feature service — the service layer stays the only entry point, same as the templated path.

Repeat: this path should stay rare. Default to sections 1–3.

---

## 5. How reads and rendering work (informational — you don't need to build this)

`GET /api/notifications` reads and renders notifications in a 3-phase batch pipeline:

1. **Phase 1 (ID Collection):**
   `NotificationService` instantiates a `BatchLoadContext`. It pre-seeds `context.Users` with recipient users (`notification.ReceiverUserId`). Then for each notification with an attached template, it resolves the mapper and calls `mapper.CollectIds(notification.Data, context)`.
2. **Phase 2 (Bulk Preload):**
   `batchPreloader.PreloadAsync(context)` runs bulk `WHERE id IN (...)` queries sequentially for only the entity types that were actually referenced across all notifications on the page. This drops database queries from dozens down to a handful of batch queries per page!
3. **Phase 3 (Synchronous Mapping & Priority Rendering):**
   Each notification is mapped and rendered synchronously via `templateRenderer.Render(template, values, initialValues, staticFallback)` for `Title`, `Body`, and `LinkPattern` (unless `Notification.Link` was explicitly overridden):

```csharp
var values = mapper.Map(notification.Data, context);
var initialValues = TemplateRenderer.ExtractInitialValues(notification.Data);
var staticFallback = notification.StaticFallback;

var title = templateRenderer.Render(template.Title, values, initialValues, staticFallback);
var body = templateRenderer.Render(template.Body, values, initialValues, staticFallback);
var link = notification.Link ?? (template.LinkPattern is not null
    ? templateRenderer.Render(template.LinkPattern, values, initialValues, staticFallback)
    : null);
```

### Fallback Priority Hierarchy
When rendering template placeholders `{variable}`:
1. **Live Entity:** Values resolved from preloaded database entities take top priority.
2. **`_initialValues` Snapshot:** If an entity has been deleted or is missing, the renderer checks if an `_initialValues` snapshot was stored in the JSON data payload. If found, it uses the snapshot while preserving all other live entity values!
3. **`staticFallback` Column:** If ANY missing entity placeholder lacks an initial value, and a `staticFallback` string is present in the `static_fallback` column, the ENTIRE template is replaced by `staticFallback`.
4. **`"<DELETED>"` Default:** If no static fallback is available, unresolved placeholders render as `"<DELETED>"` (never raw braces or empty strings).

---

## That's all

No changes are needed in controllers, DTOs, or the fetch pipeline.
Every time a notification is fetched, the service automatically:

1. Gathers all referenced entity IDs via `mapper.CollectIds(notification.Data, context)`.
2. Bulk-preloads required entities in single batch queries via `batchPreloader.PreloadAsync(context)`.
3. Calls `mapper.Map(notification.Data, context)` to get a variable dictionary.
4. Renders `Title`, `Body`, and (if set) `LinkPattern` with `TemplateRenderer` respecting the fallback hierarchy — unless `Notification.Link` was explicitly set as an override, in which case that value wins for the link.

Adding a new event is always **four steps**: seed template → mapper → register → fire.
The ad-hoc path in section 4 is the rare exception to this — not an alternative default.
