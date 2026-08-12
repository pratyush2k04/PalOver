---
trigger: always_on
---

# Project Rules

## Project Stack

* Frontend: Angular 21, TypeScript, SCSS.
* Backend: ASP.NET Core .NET 10 Web API.
* Database: PostgreSQL.
* Backend architecture: Controller → Service → Repository → EF Core → PostgreSQL.
* Use Dependency Injection throughout.
* Follow `agents/angular-architecture` for all Angular architecture and Angular-specific implementation rules.

---

# General

* Follow the existing project structure and coding patterns.
* Before creating anything, check whether a similar component, service, model, DTO, utility, constant, or implementation already exists.
* Reuse or extend existing code before creating new code.
* Write simple, readable, production-ready code.
* Follow SOLID, DRY, KISS, and Separation of Concerns.
* Prefer composition over inheritance unless inheritance is clearly beneficial.
* Prefer readability over cleverness.
* Keep responsibilities focused and follow the Single Responsibility Principle.
* Do not over-engineer.
* Avoid premature optimization.
* Do not add unnecessary abstractions, files, packages, or architectural layers.
* Do not introduce new architectural patterns unless explicitly requested.
* Do not rename, move, refactor, or modify unrelated code unless required.
* Make the smallest change necessary to complete the task.
* Do not break existing APIs, routes, DTOs, component contracts, or shared models without checking all usages first.
* Maintain consistency with the existing codebase.
* Follow existing naming and formatting conventions.

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

* Keep controllers thin.
* Controllers coordinate requests and responses only.
* Business logic belongs in services.
* Never access `DbContext` directly from controllers.
* Return appropriate HTTP status codes.

## Services

* Business logic belongs in services.
* Keep services focused.
* Avoid duplicate business logic.
* Keep one business responsibility per service method.

## Repositories

* Repositories contain data-access logic only.
* Repositories must not contain business rules.
* Do not introduce unnecessary generic repositories.
* Use Unit of Work only if the project already uses it or a transaction requires it.

---

# DTOs

* Always use DTOs at API boundaries.
* Never expose EF entities directly.
* Use separate Create, Update, and Response DTOs when their contracts differ.
* Reuse existing DTOs when appropriate.

---

# Entity Framework Core

* Prefer LINQ over raw SQL.
* Avoid raw SQL unless required.
* Use Fluent API for entity configuration.
* Use `AsNoTracking()` for appropriate read-only queries.
* Prefer `Select()` projections when full entities are unnecessary.
* Avoid unnecessary `Include()` calls.
* Avoid N+1 query problems.
* Minimize unnecessary database calls.
* Apply filtering, sorting, searching, and pagination in the database where practical.
* Do not load data into memory before filtering unless necessary.
* Use EF Core migrations for schema changes.
* Keep migrations focused.
* Do not manually edit generated migrations unless absolutely necessary.
* Configure relationships properly.
* Prefer database-enforced integrity where appropriate.
* Never run concurrent operations against the same EF Core `DbContext`.

---

# Async

* Use `async`/`await`.
* Never use `.Result`, `.Wait()`, or other blocking patterns.
* Use `CancellationToken` where appropriate.
* Use `Task.WhenAll()` only for independent operations.
* Never run concurrent operations against the same EF Core `DbContext`.

---

# Validation

* Use FluentValidation where available.
* Validate request DTOs outside controllers.
* Input validation belongs in validators.
* Business-rule validation belongs in services.
* Keep validation reusable.
* Do not duplicate validation unnecessarily.

---

# Exception Handling

Use global exception handling.

* Do not add repetitive `try/catch` blocks when global handling already covers the exception.
* Do not catch exceptions only to rethrow them.
* Never expose internal exception details to clients.

Custom exceptions belong in:

```text
backend/Pdr.Api/Exceptions
```

All custom exceptions must inherit from:

```text
AppException
```

* Do not throw raw `Exception` or `ApplicationException` for expected application failures.
* Name custom exceptions descriptively with an `Exception` suffix.
* Each custom exception must provide a clear message and appropriate status code through the `AppException` base constructor.

---

# API Standards

Every endpoint should:

* Validate requests.
* Use appropriate HTTP methods and status codes.
* Return meaningful error messages.
* Return the project's existing standard API response format.
* Use DTOs at API boundaries.
* Return standardized error responses / ProblemDetails.
* Add pagination, filtering, sorting, and searching only where they are actually useful.
* Do not introduce a second API response format.

## Swagger / OpenAPI

Every public API endpoint must include Swagger/OpenAPI documentation.

Document:

* Endpoint summary.
* Endpoint description.
* Request body.
* Path parameters.
* Query parameters.
* Response types and status codes.
* Possible error responses.

Keep Swagger documentation up to date whenever an endpoint changes.

---

# Logging

Use `ILogger<T>` and structured logging.

Prefer:

```csharp
_logger.LogInformation(
    "Created engagement {EngagementId}",
    engagementId);
```

Avoid string interpolation for structured logging:

```csharp
_logger.LogInformation(
    $"Created engagement {engagementId}");
```

Never log:

* Passwords.
* Tokens.
* Secrets.
* Connection strings.
* Sensitive information.

Log useful diagnostic information and unexpected exceptions.

---

# PostgreSQL

* Follow consistent naming conventions for tables, columns, constraints, and indexes.
* Use proper foreign keys and constraints.
* Use appropriate nullability.
* Add indexes only when justified by actual query patterns.
* Prefer database-enforced integrity where appropriate.
* Use EF Core migrations for schema changes.

---

# Security & Configuration

* Never hardcode secrets, API keys, connection strings, passwords, or environment-specific URLs.
* Use `IConfiguration`, `IOptions<T>`, and environment configuration.
- Keep environment-specific settings separate.
* Validate external input.
- Validate authorization/permissions before sensitive operations.
* Frontend role checks are for UI only and are not a security boundary.
* Do not trust roles or permissions sent by the frontend when they can be derived from authenticated identity.
* Never expose internal exception details to clients.

---

# .NET Constants

Avoid hardcoded values that are reused across the application.

Store reusable .NET constants in dedicated constants classes grouped by responsibility.

Examples:

```text
Constants/
├── ClaimConstants.cs
├── RoleConstants.cs
├── PolicyConstants.cs
└── AppConstants.cs
```

Avoid repeating the same string literal in multiple locations.

- Put reusable strings, keys, statuses, routes, messages, and other repeated values in constants.
- Before adding a string literal, check whether an equivalent constant already exists.
- When modifying or creating services, validators, exceptions, or controllers, check for repeated/hardcoded strings and extract reusable values into the appropriate constants class.
- Do not duplicate the same string literal across multiple files.

---
### Folder Consistency

- Do not create subfolders inside established folders such as `Exceptions`, `Validators`, `Controllers`, `Services`, or `Repositories` unless explicitly required.
- Follow the existing flat folder structure and naming conventions.
---
# Program.cs / Extensions

- Keep `Program.cs` focused on application startup and composition.
- Put service-registration and application-configuration extensions in the existing `Extensions/` folder.
- Reuse the existing extension methods instead of adding configuration directly to `Program.cs`.
- Do not allow `Program.cs` to grow with feature-specific service/configuration setup.
---
# Entity Configuration

- Do not use Data Annotations for entity configuration or validation.
- Use Fluent API for EF Core entity configuration.
- Use FluentValidation for request validation.
---
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

Use:

* PascalCase for classes, properties, and public members.
* camelCase for variables and parameters.
* Descriptive names.
* Guard clauses where appropriate.
* Small, focused methods.
* Reusable methods where appropriate.

Avoid:

* Deeply nested conditions.
* Magic numbers.
* Repeated magic strings.
* Duplicate code.
* Unnecessary comments.

Comments should explain **why**, not obvious code behavior.

Document non-obvious business rules.

---

# Git & Change Safety

* Make focused changes.
* Keep pull requests small.
* Do not modify unrelated files.
* Do not reformat unrelated code.
* Do not commit secrets, temporary files, generated artifacts, or debugging files.
* Do not overwrite another developer's work unnecessarily.
* Check usages before changing shared code or contracts.
* Keep commit messages meaningful.
* Preserve existing formatting unless necessary.

---

# AI Workflow

Before implementing a task:

1. Understand the requested behavior.
2. Inspect relevant existing code.
3. Search for similar implementations.
4. Reuse existing code where possible.
5. For Angular, use `agents/angular-architecture` to determine the correct architectural location and dependency boundaries.
6. Make the smallest required change.
7. Verify the result.

Do not silently invent important business rules.

If existing code establishes a pattern, follow it.

If a major requirement is genuinely ambiguous and cannot be inferred from the repository, ask before making a major architectural decision.

---

# Before Completing a Task

Verify:

* Code follows the existing architecture.
* Only necessary files were changed.
* Existing functionality was reused where possible.
* No duplicate implementation was introduced.
* Angular dependency boundaries are respected.
* API calls remain in services.
* Global infrastructure is reused instead of duplicated.
* Routes use the correct layout.
* Backend controllers remain thin.
* Business logic remains in services.
* Repositories contain only data-access logic.
* DTOs are used at API boundaries.
* EF Core queries are reasonable.
* Async code is properly awaited.
* Validation and exception handling follow existing patterns.
* No secrets or sensitive data are exposed.
* Imports and routes resolve.
* Code compiles.
* Existing contracts are preserved.
* Naming conventions are followed.
* No unnecessary files were created.

---

# Final Rule

Prefer:

```text
existing pattern > new pattern
reuse > duplication
simple > clever
focused change > large refactor
existing infrastructure > new infrastructure
```

Consistency with the existing repository is more important than theoretical architectural perfection.
