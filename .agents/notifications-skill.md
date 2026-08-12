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
   - `CreatedAt`, `UpdatedAt`, `CreatedBy`, `IsDeleted` – always set these (see existing entries for the pattern).

```csharp
new NotificationTemplate
{
    Id = 4,                         // next available id (check the file for the current max)
    Key = "DocumentUploaded",
    Title = "Document uploaded — {DocumentName}",
    Body = "{UploaderName} uploaded {DocumentName} to engagement {EngagementName}.",
    DefaultPriority = NotificationPriority.Medium,
    CreatedAt = seedTimestamp,
    UpdatedAt = seedTimestamp,
    CreatedBy = "System",
    IsDeleted = false
}
```

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

    private record DocumentUploadedPayload(int DocumentId, int EngagementId, int UploaderUserId);

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
            ["UploaderName"]   = uploader?.FullName
        };
    }
}
```

- The `TemplateKey` must exactly match the `Key` from the template seed.
- Use a **primary constructor** — all existing mappers use this style.
- Inject repositories via the primary constructor — all mappers are scoped services.
- Place the `private record` payload type **inside** the class.
- The `data` parameter is the same JSON you pass to `CreateFromTemplateAsync` later. Keep it to **IDs only**, no full objects.
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

You have **two ways** to create a notification, depending on whether the recipient is determined by a role on an engagement or is a specific, known user.

### Option A – Notify a role on an engagement (most domain events)

Use this when the event belongs to an engagement and the recipient is whoever currently holds a particular role (e.g., Tech Lead, Project Management). The service resolves the actual user automatically and silently does nothing if the role is currently unassigned.

```csharp
await notificationService.CreateFromTemplateAsync(
    templateKey: "DocumentUploaded",
    engagementId: engagementId,
    receiverRole: RoleEnum.ProjectManagement,   // who should be notified
    data: JsonSerializer.Serialize(new
    {
        DocumentId = document.Id,
        EngagementId = engagementId,
        UploaderUserId = currentUserId
    }),
    scheduledFor: DateTime.UtcNow,
    link: $"/engagements/{engagementId}/documents");   // optional – omit if not needed
```

**What happens internally:**
- Looks up the engagement assignment for the given role.
- If no user is assigned → logs and returns (no exception, no row).
- If the `templateKey` doesn't match any `NotificationTemplate` → logs and returns (no exception, no row). Unlike Option B, a missing template here is **not** an error.
- Otherwise, creates a notification addressed to that user.

**Method signature:**

```csharp
Task CreateFromTemplateAsync(
    string templateKey,
    int engagementId,
    RoleEnum receiverRole,
    string data,
    DateTime scheduledFor,
    string? link = null,
    CancellationToken cancellationToken = default);
```

### Option B – Notify a specific user directly

Use this when the recipient is not (or cannot be) derived from an engagement role – for example, notifying a user that their password was changed, or that they've been mentioned in a comment. You supply the `receiverUserId` directly; no engagement lookup is performed.

```csharp
await notificationService.CreateFromTemplateAsync(
    templateKey: "PasswordChanged",
    receiverUserId: currentUserId,               // known user ID
    data: JsonSerializer.Serialize(new
    {
        ChangedAt = DateTime.UtcNow
    }),
    scheduledFor: DateTime.UtcNow);
```

**What happens internally:**
- Looks up the template by key; throws `NotificationTemplateNotFoundException` if missing.
- Creates a notification addressed to the given user, using the template's default priority.

**Method signature:**

```csharp
Task CreateFromTemplateAsync(
    string templateKey,
    int receiverUserId,
    string data,
    DateTime scheduledFor,
    CancellationToken cancellationToken = default);
```

---

## That's all

No changes are needed in controllers, DTOs, or the fetch pipeline.
Every time a notification is fetched, the service automatically:

1. Finds the matching mapper via the template's `Key`.
2. Calls `MapAsync(data)` to get a variable dictionary.
3. Renders `Title` and `Body` with `TemplateRenderer`.

Adding a new event is always **four steps**: seed template → mapper → register → fire.
