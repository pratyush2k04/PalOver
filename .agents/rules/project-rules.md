---
trigger: always_on
---

# Project Rules

## Project Stack

- Frontend: Angular 21, TypeScript, SCSS
- Backend: ASP.NET Core .NET 10 Web API
- Database: PostgreSQL
- Backend architecture: Controller → Service → Repository → EF Core → PostgreSQL
- Use Dependency Injection throughout.

# General

- Follow the existing project structure and coding patterns.
- Before creating anything, check whether a similar component, service, model, DTO, utility, constant, or implementation already exists.
- Reuse or extend existing code before creating new code.
- Write simple, readable, production-ready code.
- Follow SOLID, DRY, KISS, and Separation of Concerns.
- Do not over-engineer.
- Do not add unnecessary abstractions, files, packages, or architectural layers.
- Do not rename, move, refactor, or modify unrelated code unless required.
- Make the smallest change necessary to complete the task.
- Do not break existing APIs, routes, DTOs, component contracts, or shared models without checking all usages first.
- Follow existing naming and formatting conventions.

---

# Angular Architecture

Use:

```text
src/app/
├── core/
├── features/
├── shared/
└── layout/
```

## `core`

App-wide infrastructure only.

Examples:

- authentication
- guards
- HTTP/interceptors
- loading
- notifications
- app initialization

---

# Angular Imports

Use configured aliases for cross-folder imports:

```text
@core
@features
@shared
@layout
@env
```

Prefer:

```typescript
import { LoaderService } from "@core/loading/loader.service";
```

Avoid long relative imports such as:

```typescript
../../../../core/loading/loader.service
```

Relative imports are fine within the same feature/folder.

---

# Angular Components

- Use standalone components.
- Keep components focused on UI, user interaction, forms, and view state.
- Business logic belongs in services.
- API calls belong in services.
- Components must not use `HttpClient` directly.
- Prefer reusable components only when there is genuine reuse.
- Do not create generic components prematurely.

---

# Angular Templates

Use modern Angular control flow:

```text
@if
@for
@switch
```

Avoid:

```text
*ngIf
*ngFor
*ngSwitch
```

Use an appropriate `track` expression with every `@for`.

---

# Angular State

- The application is zoneless.
- Prefer Signals for state.
- Use `signal()` for state.
- Use `computed()` for derived state.
- Use `effect()` only for actual side effects.
- Avoid unnecessary RxJS for simple component state.
- Prefer signal-based inputs/outputs where supported.

---

# Angular Forms

- Prefer Reactive Forms.
- Use typed forms where possible.
- Keep validation reusable.
- Avoid Template-driven Forms unless explicitly requested.

---

# Angular HTTP & Loading

All API calls must go through Angular services.

Use the global:

```text
core/loading/loader.service.ts
```

for HTTP loading state.

Do not create local HTTP-loading signals such as:

```typescript
isLoading;
isSaving;
isSubmitting;
```

HTTP errors are handled centrally by:

```text
core/http/http-error.interceptor.ts
```

Do not call:

```typescript
toastService.error(...)
```

inside normal HTTP `error:` callbacks.

Only show a component-level error toast for business failures where the HTTP request succeeds but the response indicates failure, for example:

```typescript
if (!res.success) {
  this.toastService.error(res.message);
}
```

Avoid duplicate error messages.

---

# Angular and dotnet Constants

Shared strings, keys, statuses, and route segments belong in:

````text
shared/constants/app.constants.ts

---

# Angular TypeScript

- Use strict typing.
- Avoid `any` and `as any`.
- Prefer interfaces and typed models.
- Prefer `readonly` where appropriate.
- Fix typing problems instead of bypassing TypeScript.

---

# Angular Styling

- Use SCSS.
- Do not use Angular Material unless explicitly requested.
- Reuse existing design tokens, CSS variables, and shared styles.
- Avoid inline styles.
- Prefer responsive layouts.
- Use `rem` instead of `px` for spacing, sizing, typography, margins, padding, gaps, and border radius.
- `px` is acceptable for thin borders such as `1px` or when technically required.
- Use HEX colors instead of RGB/RGBA.
- Prefer existing CSS variables/design tokens over hardcoded colors.

Example:

```scss
.card {
  padding: 1.5rem;
  font-size: 1rem;
  color: #1a1a1a;
  border: 1px solid #e5e5e5;
}
```

---

# Angular Testing

Do not generate `.spec.ts` files or frontend unit tests unless explicitly requested.

---

# Backend Architecture

Follow:

```text
Controller
    ↓
DTO
    ↓
Service
    ↓
Repository
    ↓
Unit of Work (if already used/needed)
    ↓
EF Core
    ↓
PostgreSQL
```

## Controllers

- Keep controllers thin.
- Controllers coordinate requests and responses only.
- No business logic.
- Never access `DbContext` directly.

## Services

- Business logic belongs in services.
- Keep service methods focused.
- Avoid duplicate business logic.

## Repositories

- Repositories contain data-access logic only.
- Do not put business rules in repositories.
- Do not introduce unnecessary generic repositories.
- Use Unit of Work only if the project already uses it or a transaction requires it.

---

# DTOs

- Always use DTOs at API boundaries.
- Never expose EF entities directly.
- Use separate Create, Update, and Response DTOs when their contracts differ.
- Reuse existing DTOs when appropriate.

---

# Entity Framework Core

- Prefer LINQ over raw SQL.
- Use Fluent API for entity configuration.
- Use `AsNoTracking()` for appropriate read-only queries.
- Prefer `Select()` projections when full entities are unnecessary.
- Avoid unnecessary `Include()`.
- Avoid N+1 queries.
- Apply filtering, sorting, searching, and pagination in the database where practical.
- Do not load data into memory before filtering unless necessary.
- Use EF Core migrations for schema changes.
- Keep migrations focused.

---

# Async

- Use `async`/`await`.
- Never use `.Result`, `.Wait()`, or other blocking patterns.
- Use `CancellationToken` where appropriate.
- Use `Task.WhenAll()` only for independent operations.
- Never run concurrent operations against the same EF Core `DbContext`.

---

# Validation

- Use FluentValidation where available.
- Validate request DTOs outside controllers.
- Input validation belongs in validators.
- Business-rule validation belongs in services.
- Do not duplicate validation unnecessarily.

---

# Exception Handling

Use global exception handling.

Do not add repetitive `try/catch` blocks when global handling already covers the exception.

Custom exceptions belong in:

```text
backend/Pdr.Api/Exceptions
```

All custom exceptions must inherit from:

```text
AppException
```

Do not throw raw `Exception` or `ApplicationException` for expected application failures.

Use descriptive exception names ending with `Exception`.

Never expose internal exception details to clients.

---

# API Standards

- Use appropriate HTTP methods and status codes.
- Return the project's existing standard API response format.
- Use standardized error responses / ProblemDetails.
- Add pagination, filtering, sorting, and searching only where they are actually useful.
- Do not introduce a second API response format.

---
## Swagger Documentation

Every public API endpoint must include Swagger/OpenAPI documentation.

Document:
- endpoint summary
- endpoint description
- request body
- path parameters
- query parameters
- response types and status codes
- possible error responses

Swagger documentation should remain up to date whenever an endpoint changes.

# Logging

Use `ILogger<T>` and structured logging.

Use:

```csharp
_logger.LogInformation(
    "Created engagement {EngagementId}",
    engagementId);
```

Avoid string interpolation for structured logs.

Never log:

- passwords
- tokens
- secrets
- connection strings
- sensitive information

---

# PostgreSQL

- Use proper foreign keys and constraints.
- Use appropriate nullability.
- Add indexes only when justified by actual query patterns.
- Prefer database-enforced integrity where appropriate.
- Use EF Core migrations for schema changes.

---

# Security & Configuration

- Never hardcode secrets, API keys, connection strings, passwords, or environment-specific URLs.
- Use `IConfiguration`, `IOptions<T>`, and environment configuration.
- Validate external input.
- Enforce authorization on the backend.
- Frontend role checks are for UI only and are not a security boundary.
- Do not trust roles/permissions sent by the frontend when they can be derived from authenticated identity.

# Naming & Code Style

Follow existing project naming conventions.

Typical backend naming:

```text
EngagementController
EngagementService
EngagementRepository

IEngagementService
IEngagementRepository
```

- PascalCase → classes, properties, public members.
- camelCase → variables and parameters.
- Prefer guard clauses.
- Avoid deeply nested conditions.
- Avoid magic numbers and repeated magic strings.
- Use descriptive names.
- Comments should explain **why**, not obvious code behavior.

---
# Constants

Avoid hardcoded values that are reused across the application.

Extract shared constants such as:

- UI strings
- route segments
- API route paths
- endpoint paths
- claim names
- role names
- policy names
- status values
- repeated keys
- configuration keys
- regex patterns
## Naming Consistency

When creating new APIs or features, keep naming consistent across the entire stack.

## Angular

Shared application constants belong in:

```text
shared/constants/app.constants.ts
```

Do not create multiple constants files for shared application values unless they represent a separate reusable domain.

## .NET

Store reusable constants in dedicated constants classes grouped by responsibility.

Examples:

```text
Constants/
├── ClaimConstants.cs
├── RoleConstants.cs
├── PolicyConstants.cs
└── AppConstants.cs
```

Avoid repeating the same string literal in multiple locations.

# Git & Change Safety

- Make focused changes.
- Do not modify unrelated files.
- Do not reformat unrelated code.
- Do not commit secrets, temporary files, or debugging artifacts.
- Do not overwrite another developer's work unnecessarily.
- Check usages before changing shared code or contracts.

# AI Workflow

Before implementing a task:

1. Understand the requested behavior.
2. Inspect relevant existing code.
3. Search for similar implementations.
4. Reuse existing code where possible.
5. Decide the correct architectural location.
6. Make the smallest required change.
7. Verify the result.

For Angular files:

```text
App-wide infrastructure? → core

Owned by one feature? → features/<feature>

Reusable across features? → shared

Application shell? → layout
Do not silently invent important business rules.

If existing code establishes a pattern, follow it.

If a major requirement is genuinely ambiguous and cannot be inferred from the repository, ask before making a major architectural decision.

Before Completing a Task

Verify:

Code follows the existing architecture.
Only necessary files were changed.
Existing functionality was reused where possible.
No duplicate implementation was introduced.
Angular dependency boundaries are respected.
API calls remain in services.
Global loader and HTTP error handling are used.
Routes use the correct layout.
Backend controllers remain thin.
Business logic remains in services.
Repositories contain only data access.
DTOs are used.
EF Core queries are reasonable.
Async code is properly awaited.
Validation and exception handling follow existing patterns.
No secrets or sensitive data are exposed.
Imports and routes resolve.
Code compiles.
Existing contracts are preserved.
Final Rule

Prefer:

existing pattern > new pattern
reuse > duplication
simple > clever
focused change > large refactor
existing infrastructure > new infrastructure

Consistency with the existing repository is more important than theoretical architectural perfection.
````
