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

The mapper's job is to turn the raw `Data` JSON into a dictionary of placeholder values.
It is called **at fetch time**, not at creation time.

### 2a. Create the mapper class

**File:** `Pdr.Api/Services/Notifications/Mappers/DocumentUploadedMapper.cs`

```csharp
using System.Text.Json;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentUploadedMapper(
    IDocumentRepository documentRepository,
    IEngagementRepository engagementRepository,
    IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => "DocumentUploaded";

    public record DocumentUploadedPayload(int DocumentId, int EngagementId, int UploaderUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentUploadedPayload>(data)
            ?? throw new InvalidOperationException("DocumentUploaded notification data could not be parsed.");

        var document = await documentRepository.GetByIdAsync(payload.DocumentId);
        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var uploader = await userRepository.GetByIdAsync(payload.UploaderUserId);

        return new Dictionary<string, object?>
        {
            ["DocumentName"]   = document?.Name,
            ["EngagementName"] = engagement?.Name,
            ["UploaderName"]   = uploader?.FullName,
            ["EngagementId"]   = payload.EngagementId
        };
    }
}
```

- The `TemplateKey` must exactly match the `Key` from the template seed.
- Use a **primary constructor** — all existing mappers use this style.
- Inject repositories via the primary constructor — all mappers are scoped services.
- Place the payload record type **inside** the class, and make it **`public`** (not `private`) — the service that creates this notification (step 3) must construct this exact type instead of a hand-written anonymous object, so the compiler catches a property rename/typo/count mismatch between the two sides instead of it failing silently at deserialize time.
- The `data` parameter is the same JSON you pass to `CreateFromTemplateAsync` later. Keep it to **IDs only**, no full objects.
- If the template defines a `LinkPattern` that references a field not already in your dictionary (commonly `EngagementId`), add it to the dictionary too — the link is rendered with the same values dictionary as `Title`/`Body`.
- Throw `InvalidOperationException` when the payload cannot be parsed — do **not** return an empty dictionary (that would leave unreplaced `{Placeholder}` tokens in the rendered notification).
- Import `using Pdr.Api.Repositories.Interfaces;` for repository interfaces and `using System.Text.Json;` for `JsonSerializer`.

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
- Priority is `priorityOverride ?? template.DefaultPriority` — omit `priorityOverride` (or pass `null`) to use the template's seeded `DefaultPriority`. Pass it when the same template needs different urgency depending on the calling context (e.g. the same reminder template fired at different day-thresholds before a deadline, where the closer threshold should be `High` even though the template's own default is `Low`).

**Method signature:**

```csharp
Task CreateFromTemplateAsync(
    string templateKey,
    int engagementId,
    RoleEnum receiverRole,
    string data,
    DateTime scheduledFor,
    NotificationPriority? priorityOverride = null,
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

**Method signature:**

```csharp
Task CreateFromTemplateAsync(
    string templateKey,
    int receiverUserId,
    string data,
    DateTime scheduledFor,
    NotificationPriority? priorityOverride = null,
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

**Method signature:**

```csharp
Task CreateFromTemplateAsync(
    string templateKey,
    IEnumerable<int> receiverUserIds,
    string data,
    DateTime scheduledFor,
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

## That's all

No changes are needed in controllers, DTOs, or the fetch pipeline.
Every time a notification is fetched, the service automatically:

1. Finds the matching mapper via the template's `Key`.
2. Calls `MapAsync(data)` to get a variable dictionary.
3. Renders `Title`, `Body`, and (if set) `LinkPattern` with `TemplateRenderer` — unless `Notification.Link` was explicitly set as an override, in which case that value wins for the link.

Adding a new event is always **four steps**: seed template → mapper → register → fire.
The ad-hoc path in section 4 is the rare exception to this — not an alternative default.
