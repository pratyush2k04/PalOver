---
trigger: always_on
---

# Angular Architecture Rules

## Purpose

Feature-first Angular architecture. This file is the source of truth for Angular architecture and implementation.

Dependency direction:

```text
Features → Shared → Common
```

Rules:

* `features` may import `shared` and `common`.
* `shared` may import `common`.
* `shared` must not import `features`.
* `common` must not import `shared` or `features`.
* `core` contains app-wide infrastructure only.
* `core` must not contain feature-specific business logic.
* `layouts` contain application shells, not feature implementation.

---

# Stack & Structure

* Angular 21.
* Standalone components.
* Zoneless application.

```text
src/app/
├── core/
├── layouts/
├── features/
├── shared/
├── common/
├── app.routes.ts
└── app.config.ts
```

---

# Core

App-wide infrastructure:

* authentication
* route guards
* HTTP/interceptors
* loading
* notifications
* configuration
* app initialization
* tokens
* environment configuration

Initialize Core once.

---

# Layouts

Application shells containing shared navigation and the global router outlet.

Example:

```text
layouts/
└── main-layout/
    ├── sidebar/
    ├── topbar/
    └── main-layout.component
```
- Structure secured top-level routes with `MainLayoutComponent` as the parent and feature routes as its children.

Feature routes render inside `MainLayoutComponent`.

Do not bypass the layout unless the route is intentionally layout-less, e.g. login/error pages.

Layouts must not depend on feature implementation details.

---
# Features

Each top-level sidebar menu is a feature.

Examples:

- engagements
- engagement-allocation
- users
- templates
- config
- portfolio
- reports

A feature owns its pages, components, services, models, and routes.

Use the existing feature structure. Do not introduce additional architectural folders such as `pages/` unless the project already uses them or there is an explicit requirement.

---

# Engagements

`engagements` is a single feature.

Engagement-specific areas such as Overview, Checklist, Documents, Handover, Kickoff, etc. belong under:

```text
features/
└── engagements/
    └── engagement-workspace/
        ├── checklist/
        ├── documents/
        ├── handover/
        ├── kickoff/
        ├── overview/
        └── ...
# Routing

`app.routes.ts` owns top-level navigation.

```text
/engagements
/users
/templates
/config
/portfolio
/reports
/spec-map
```

Use lazy loading for feature routes where appropriate.

Protect secured routes with guards.

Each feature owns its route file.

## Engagement Routes

`engagements.routes.ts` owns:

```text
/engagements
/engagements/:engagementId
/engagements/:engagementId/overview
/engagements/:engagementId/handover
/engagements/:engagementId/documents
/engagements/:engagementId/people
/engagements/:engagementId/checklist
/engagements/:engagementId/kickoff
/engagements/:engagementId/resources
/engagements/:engagementId/technical
/engagements/:engagementId/risks
/engagements/:engagementId/assumptions-decisions
```

`/engagements` → list.

`/engagements/:engagementId` → detail shell.

Child routes render inside the detail shell.

---

# Engagement Detail

`EngagementDetailComponent` is the shell:

```text
EngagementDetail
├── Tabs
└── RouterOutlet
```

Tabs remain visible while only the child outlet changes.

Engagement tabs must use child routes rendered inside `EngagementDetailComponent`.

---

# Component Placement

* One page → keep local to that page.
* Multiple pages in one feature → `features/<feature>/components`.
* Multiple top-level features + business/domain knowledge → `shared`.
* Generic + business-logic-independent → `common`.
* Prefer local feature code before promoting to Shared/Common.
* Do not create premature abstractions.

---

Change it to:

```md
# Shared

Reusable application/domain components, services, models, and constants shared across top-level features.

Examples:

```text
shared/
├── components/
├── services/
├── models/
├── constants/
└── pipes/

---

# Common

Generic, business-logic-independent building blocks; treat it as the application's internal UI kit.

Examples:

```text
common/
├── components/
├── services/
├── directives/
├── pipes/
└── interceptors/
```

Common must not depend on business logic, Shared, or Features.

---

# Common UI Reuse

Before creating generic UI, inspect:

```text
common/components/
common/services/
common/directives/
common/pipes/
```

Do not rely on example lists in this file as the current inventory.

* Reuse an existing Common component when it fits.
* Do not hand-roll equivalent generic UI.
* New Common UI must be business-logic-independent.
* Use standalone components.
* Follow existing Common component patterns.
* Prefer signal-based inputs where supported.
* Compose Common primitives in feature-specific UI where appropriate.
- Use `@common/...` and `@shared/...` aliases for Common/Shared imports.
- Never use long relative imports such as `../../../` for Common or Shared code.

---

# Angular Imports

Use configured aliases:

```text
@core
@features
@shared
@layout
@env
```

Use the project's configured Common alias where applicable.

Prefer aliases over long relative paths.

Relative imports are acceptable within the same feature/folder.

---

# Components

* Use standalone components.
* Keep components focused on presentation, interaction, forms, and view state.
* Business logic belongs in services.
* API calls belong in services.
* Components must not use `HttpClient` directly.
* Keep components small and cohesive.
* Use smart/container components only when necessary.

---

# Templates

Use:

```text
@if
@for
@switch
```

Avoid `*ngIf`, `*ngFor`, and `*ngSwitch` unless explicitly requested.

Every `@for` must have an appropriate `track` expression.

---

# State

* Application is zoneless.
* Prefer Signals.
* Use `signal()` for state.
* Use `computed()` for derived state.
* Use `effect()` only for actual side effects.
* Prefer signal-based inputs/outputs where supported.
* Avoid unnecessary RxJS for simple local state.

---

# Forms

* Prefer Reactive Forms.
* Use typed forms where possible.
* Use FormBuilder where appropriate.
* Avoid Template-driven Forms unless explicitly requested.
* Keep validation reusable.
* Display validation errors consistently.

# Component Placement

- Used by one component/page → keep it local to that component/page.
- Reused within one Engagement area → keep it inside that area.
- Reused across multiple Engagement areas → `engagement-workspace/components`.
- Reused across multiple top-level features and understands business/domain concepts → `shared`.
- Generic and business-logic-independent → `common`.
- Prefer local code before promoting a component upward.
---

# HTTP, Loading & Errors

All API calls go through services.

Use the global:

```text
core/loading/loader.service.ts
```

for HTTP loading.

Do not create local HTTP-loading signals such as:

```text
isLoading
isSaving
isSubmitting
```

Use:

```text
core/http/http-error.interceptor.ts
```

for centralized HTTP errors.

Do not call `toastService.error()` inside normal HTTP `error:` callbacks.

Component-level error toasts are only for business failures where HTTP succeeds but the response indicates failure:

```typescript
if (!res.success) {
  this.toastService.error(res.message);
}
```

Avoid duplicate error messages.

---

# TypeScript

* Use strict typing.
* Avoid `any` and `as any`.
* Prefer interfaces and typed models.
* Prefer `readonly` where appropriate.
* Fix typing problems instead of bypassing TypeScript.
* Prefer enums or typed constant objects over magic strings where appropriate.

---

# Constants

Shared Angular constants belong in:

```text
shared/constants/app.constants.ts
```

Use it for reusable:

* UI strings
* route segments
* API paths
* claim names
* roles
* policies
* status values
* repeated keys
* configuration keys
* regex patterns

Do not create multiple shared constants files unless they represent separate reusable domains.

---

# Styling

* Use SCSS.
* Reuse existing design tokens, CSS variables, mixins, and shared styles.
* Use semantic tokens for colors, spacing, typography, breakpoints, and sizing.
* Use SCSS mixins for repeated patterns.
* Avoid duplicated styles.
* Keep shared tokens/mixins in the project's shared styling/theme location.
* Avoid inline styles.
* Prefer responsive layouts.
* Use `rem` for spacing, sizing, typography, margins, padding, gaps, and border radius.
* Use HEX colors or existing CSS variables/tokens.
* Avoid RGB/RGBA.

---

# Testing

Do not generate `.spec.ts` files or frontend unit tests unless explicitly requested.

---

# Architecture Decision

Use this order when placing Angular code:

```text
App-wide infrastructure → core

Application shell → layouts

One page → page-local

Multiple pages in one feature → feature/components

Multiple top-level features + domain logic → shared

Generic, business-independent UI → common
```

Never violate:

```text
Features → Shared → Common
```

Prefer:

```text
existing pattern > new pattern
local code > premature abstraction
reuse > duplication
```
