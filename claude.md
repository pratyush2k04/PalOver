# CLAUDE.md

> **Purpose:** This file defines the coding standards, architecture, and AI instructions for this project. Any AI assistant working on this repository should follow these rules unless explicitly instructed otherwise.

---

# Project Overview

## Tech Stack

* **Frontend:** Angular 21
* **Backend:** ASP.NET Core (.NET 10) Web API
* **Database:** PostgreSQL

## Architecture

* Controller-Service-Repository Architecture for backend
* Entity Framework Core using PostgreSQL
* Dependency Injection
* Request flow:

```text
Middleware
    ↓
Controller
    ↓
DTO
    ↓
Service
    ↓
Feature Repository / Repository
    ↓
Unit of Work (if applicable)
    ↓
EF Core
    ↓
PostgreSQL
```
## Dependency Injection

- Use constructor injection.
- Never use the Service Locator pattern.
- Use Scoped lifetime by default unless another lifetime is required.

Business logic must reside in the Service layer.

Repositories should only contain data access logic.

Controllers should only coordinate requests and responses.

---

# General Rules

## Code Quality

* Always generate production-ready code.
* Keep methods small and readable.
* Follow SOLID principles.
* Follow DRY (Don't Repeat Yourself).
* Follow KISS (Keep It Simple).
* Prefer readability over clever code.
* Avoid duplicate logic.
* Keep the project modular.
* Maintain consistency with the existing codebase.
* Prefer composition over inheritance unless inheritance is clearly beneficial.
* Keep responsibilities focused (Single Responsibility Principle).
* Avoid premature optimization.

## Files

* Do not generate unnecessary files.
* Do not rename existing files unless requested.
* Do not modify unrelated code.
* Preserve existing project structure.
* Do not introduce new architectural patterns unless explicitly requested.

---

# Frontend Rules (Angular)

## Angular Version

* Angular 21

## Components

* Use Standalone Components.
* Keep components focused on presentation.
* Move business logic into services.
* Prefer reusable shared components.
* Keep components small and cohesive.
* Prefer smart/container components only when necessary.

## The `common/` UI Kit (`frontend/src/app/common/`)

`common/` holds business-logic-independent, presentation-only building blocks — treat it as an internal UI-kit that could be extracted into its own library. It is separate from `shared/`, which holds app-specific-but-cross-feature chrome (e.g. header, sidebar, workspace-card) that carries real domain coupling (auth, routing, business copy).

**Before writing any new UI in a feature, list the current contents of `common/components/`, `common/services/`, `common/directives/`, and `common/pipes/` and check whether something already covers the need.** Do not rely on memory or on any list of names in this file — `common/` grows over time, so treat any such list (including the examples below) as illustrative and out of date by construction, never as the full inventory. Always verify against the actual folder contents before deciding something doesn't exist yet.

As of when this was written, `common/components/` included things like buttons, badges, alerts, avatars, spinners, empty-states, pagination, tabs, toasts, a confirm-dialog, and a set of form controls (input/textarea/select/checkbox) sharing a CVA base class; `common/services/` included a dialog-opening service and a toast service. Confirm what's actually there — this list is a hint about the *kind* of thing that belongs in `common/`, not a catalog to check against.

**Rules:**

* Do not hand-roll a native `<button>`, spinner, status pill/badge, alert box, empty-state message, or modal shell inside a feature component when an equivalent already exists in `common/` — use the shared component instead (e.g. `<app-button variant="...">`, `<app-status-badge tone="...">`, `<app-loading-spinner>`, `<app-alert variant="...">`, `<app-empty-state message="...">`, `DialogService.open(...)`).
* If a genuinely new, reusable, business-logic-free UI element is needed (and no `common/` component fits), create it under `common/components/<name>/` with selector `app-<name>`, following the existing components' shape (signal `input()`s, a small typed variant/tone union where applicable, `standalone: true`). Do not add business/domain logic to anything in `common/`.
* Only build a one-off custom implementation in a feature folder when the requirement is genuinely feature-specific (e.g. a field needs a text input alongside Cancel/Confirm, which `app-confirm-dialog` doesn't support) — and even then, prefer composing `common/` primitives (e.g. `app-button`, `app-alert`) inside the custom markup rather than re-styling raw HTML.
* Import everything from `common/` (and `shared/`) via the TypeScript path aliases — `@common/components/...`, `@common/services/...`, `@shared/...` — never via relative `../../../` paths.

## Templates

Use modern Angular control flow:

* @if
* @for
* @switch

Avoid:

* *ngIf
* *ngFor
* *ngSwitch

unless explicitly requested.

## Change Detection & State

* Build the app fully zoneless (no Zone.js).
* Use Signals for state management wherever possible.
* Prefer signal-based inputs/outputs over decorators where supported.
* Use computed signals where appropriate.
* Use effects only when side effects are required.
* Avoid unnecessary RxJS usage for local component state.

## Forms

* Prefer Reactive Forms.
* Use FormBuilder where appropriate.
* Avoid Template-driven Forms unless requested.
* Keep validation logic reusable.
* Display validation errors consistently.

## TypeScript

* Use strict typing.
* Avoid `any`.
* Prefer interfaces and typed models.
* Prefer readonly properties where applicable.
* Prefer enums or constant objects over magic strings.

## Routing

* Use lazy loading when appropriate.
* Keep routing modular.
* Every new feature route must render inside `MainLayoutComponent`'s `<router-outlet>` (`components/main-layout/main-layout.html`), so it inherits the shared `<app-sidebar>` and `<app-header>`.
* Do not create standalone top-level routes that bypass the main layout unless the feature is explicitly meant to be layout-less (e.g. login, error pages).
* Structure the route config with `MainLayoutComponent` as a parent route and feature routes as its children.
* Protect secured routes with route guards.

## Styling

* Prefer SCSS.
* Reuse common styles.
* Avoid inline styles.
* Follow responsive design principles.
* Reuse design tokens and variables.

## API Communication

* Use Angular HttpClient.
* Keep API calls inside services.
* Never call APIs directly from components.
* Handle loading, success, and error states consistently.
* Display user-friendly error messages.

## Testing

Do NOT generate:

* `.spec.ts` files
* Frontend unit tests

unless explicitly requested.

---

# Backend Rules (.NET)

## Framework

* ASP.NET Core (.NET 10)

## Controllers

* Keep controllers thin.
* Controllers should only coordinate requests.
* Business logic belongs in services.
* Return appropriate IActionResult responses.
* Never access DbContext directly from controllers.

## Services

* Keep services focused.
* Avoid duplicate business logic.
* Keep methods easy to understand.
* One business responsibility per service method.

## Repository

* Use Repository Pattern where appropriate.
* Keep data access separate from business logic.
* Repositories should never contain business rules.
* Use Unit of Work if the project already follows that pattern.

## DTOs

Always use DTOs.

Never expose Entity Framework entities directly.

Examples:

* Create DTO
* Update DTO
* Response DTO

## Entity Framework Core

* Use LINQ whenever possible.
* Avoid raw SQL unless required.
* Use migrations.
* Configure relationships properly.
* Use Fluent API instead of Data Annotations.
* Prefer `AsNoTracking()` for read-only queries.
* Avoid unnecessary `Include()` calls.
* Prevent N+1 query problems.
* Keep queries optimized.

## Dependency Injection

* Use constructor injection.
* Never use the Service Locator pattern.
* Register services with appropriate lifetimes (Scoped by default unless another lifetime is required).

## Async

* Use async/await throughout.
* Never block asynchronous code.
* Support CancellationToken where appropriate.
* Prefer Task/Task<T> for asynchronous methods.
* Use Task.WhenAll() when independent operations can run concurrently.

---

# API Standards

Every endpoint should:

* Validate requests.
* Return appropriate HTTP status codes.
* Return meaningful error messages.
* Return consistent API responses.
* Use DTOs.
* Support pagination where appropriate.
* Support filtering.
* Support sorting.
* Support searching where appropriate.

## API Response Consistency

* Return consistent success responses.
* Return standardized error responses.
* Use ProblemDetails or the project's standard error response format.

---

# Exception Handling

* Use Global Exception Middleware.
* Avoid repetitive try/catch blocks.
* Return standardized error responses.

## Custom Exceptions

* All custom exceptions must be created inside `backend/Pdr.Api/Exceptions`.
* Every custom exception must inherit from the base `AppException` class.
* Never throw raw `Exception` or `ApplicationException` directly.
* Name exceptions descriptively with an `Exception` suffix.
* Each exception should call the `AppException` base constructor with a clear message and the appropriate `StatusCodes` value.
* Do not catch exceptions only to rethrow them.

---

# Validation

* Use FluentValidation if available.
* Validate all request DTOs.
* Keep validation separate from controllers.
* Keep validation reusable.
* Never duplicate validation logic.

---

# Logging

* Use ILogger.
* Log unexpected exceptions.
* Never log passwords or sensitive information.
* Log useful diagnostic information.
* Use structured logging.

Example:

```csharp
_logger.LogInformation(
    "Created engagement {EngagementId}",
    engagement.Id);
```

Avoid:

```csharp
_logger.LogInformation($"Created engagement {engagement.Id}");
```

---

# Database Rules

## Database

* PostgreSQL

## Tables

* Use consistent naming conventions.
* Define proper foreign keys.
* Add indexes where appropriate.
* Avoid nullable columns unless necessary.
* Use appropriate constraints.
* Prefer database integrity over application-only validation.

## Migrations

* Use EF Core migrations.
* Do not manually edit generated migrations unless absolutely necessary.

---

# Naming Conventions

## Classes

Examples:

* UserService
* UserRepository
* UserController

## Interfaces

* IUserService
* IUserRepository

## Methods

* GetAsync()
* CreateAsync()
* UpdateAsync()
* DeleteAsync()

## Variables

* camelCase

## Properties

* PascalCase

## Constants

* PascalCase or UPPER_CASE (follow existing project convention)

---

# Code Style

* Prefer guard clauses.
* Avoid deeply nested conditions.
* Extract reusable methods.
* Keep methods reasonably small.
* Remove duplicate code.
* Follow existing project formatting.
* Prefer expressive variable names.
* Avoid magic numbers and magic strings.

---

# Comments

* Explain **why**, not **what**.
* Avoid unnecessary comments.
* Prefer self-explanatory code.
* Document non-obvious business rules.

---

# Performance

* Minimize unnecessary database calls.
* Avoid N+1 query problems.
* Optimize LINQ queries.
* Use pagination for large datasets.
* Avoid loading unnecessary data.
* Prefer projections (`Select`) instead of loading entire entities when appropriate.

---

# Security

* Validate all user input.
* Never hardcode secrets.
* Store configuration securely.
* Follow authentication and authorization best practices.
* Never expose internal exception details.
* Validate permissions before sensitive operations.

---

# Configuration

* Never hardcode connection strings.
* Never hardcode API keys.
* Use IConfiguration or IOptions<T>.
* Keep environment-specific settings separate.

---

# Git Guidelines

* Make focused commits.
* Keep pull requests small.
* Do not change unrelated files.
* Preserve existing formatting unless necessary.
* Keep commit messages meaningful.
* Avoid committing temporary or generated files.

#

---

# Before Completing Any Task

Verify that:

* Code compiles.
* No duplicate logic exists.
* (Frontend) No new UI duplicates an existing `common/` component (button, badge, alert, spinner, empty-state, dialog, etc.) — reused where possible, imported via `@common/*`/`@shared/*` aliases, not relative paths.
* Naming conventions are followed.
* DTOs are used where appropriate.
* Validation is implemented.
* Exception handling is implemented.
* Pagination is used where needed.
* Logging is appropriate.
* Code follows project architecture.
* No unnecessary files were created.
* Async methods are properly awaited.
* No sensitive information is exposed.
* Existing project conventions have been preserved.
