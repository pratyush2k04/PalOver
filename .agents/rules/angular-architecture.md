---
trigger: always_on
---

# Angular Frontend Architecture Rules

## Purpose

This project follows a feature-first architecture with clear dependency boundaries.

The architecture is divided into:

- features
- shared
- common
- layouts
- core

Dependency direction:

Features
    ↓
Shared
    ↓
Common

Never reverse these dependencies.

- `common` must not import from `shared` or `features`.
- `shared` must not import from `features`.
- `features` may import from both `shared` and `common`.

---

# Folder Structure

src/
└── app/
    ├── core/
    ├── layouts/
    ├── features/
    ├── shared/
    ├── common/
    ├── app.routes.ts
    └── app.config.ts

---

# Core

Contains application-wide infrastructure.

Examples:

- Authentication
- Route Guards
- HTTP configuration
- Global configuration
- App initialization
- Tokens
- Environment configuration

Core should be initialized once and should not contain feature-specific code.

---

# Layouts

Contains application shells.

Example:

layouts/
└── main-layout/
    ├── sidebar/
    ├── topbar/
    └── main-layout.component

The MainLayout contains:

- Sidebar
- Header
- Global Router Outlet

Feature pages render inside this layout.

---

# Features

Each top-level sidebar menu is a Feature.

Examples:

- Engagements
- Users
- Templates
- Config
- Portfolio
- Reports
- Spec Map

Every feature owns:

- pages
- components
- services
- models
- routes

Example:

features/
└── users/
    ├── pages/
    ├── components/
    ├── services/
    ├── models/
    └── users.routes.ts

---

# Engagement Feature

Engagements is a single feature.

Overview, Checklist, Kickoff, Documents, etc. are NOT independent features.

They are pages inside Engagements.

Structure:

features/
└── engagements/
    ├── pages/
    │   ├── engagement-list/
    │   └── engagement-detail/
    │
    ├── overview/
    ├── handover/
    ├── documents/
    ├── people/
    ├── checklist/
    ├── kickoff/
    ├── resources/
    ├── technical/
    ├── risks/
    ├── assumptions-decisions/
    │
    ├── components/
    ├── services/
    ├── models/
    └── engagements.routes.ts

---

# Engagement Navigation

Application Flow

Sidebar

↓

Engagements

↓

Engagement List

↓

Select Engagement

↓

Engagement Detail

↓

Overview
Handover
Documents
People
Checklist
Kickoff
Resources
Technical
Risks
Assumptions & Decisions

---

# Routing

## App Routes

Responsible for top-level navigation.

Example:

/

/engagements

/users

/templates

/config

/portfolio

/reports

/spec-map

Every sidebar item loads its own feature routes.

Example:

app.routes.ts

MainLayout

└── Engagements

└── Users

└── Templates

└── Reports

etc.

---

## Engagement Routes

engagements.routes.ts controls navigation inside one engagement.

Routes:

/engagements

Shows Engagement List.

/engagements/:engagementId

Loads Engagement Detail.

/engagements/:engagementId/overview

Shows Overview page.

/engagements/:engagementId/handover

Shows Handover page.

/engagements/:engagementId/documents

Shows Documents page.

/engagements/:engagementId/people

Shows People page.

/engagements/:engagementId/checklist

Shows Checklist page.

/engagements/:engagementId/kickoff

Shows Kickoff page.

/engagements/:engagementId/resources

Shows Resources page.

/engagements/:engagementId/technical

Shows Technical page.

/engagements/:engagementId/risks

Shows Risks page.

/engagements/:engagementId/assumptions-decisions

Shows Assumptions & Decisions page.

---

# Engagement Detail Layout

EngagementDetailComponent acts as the shell for engagement pages.

Structure:

EngagementDetail

├── Engagement Tabs

│

└── Router Outlet

The tabs remain visible.

Only the Router Outlet changes.

Example:

Overview

↓

Checklist

↓

Kickoff

The shell stays the same.

Only the child page changes.

---

# Feature Components

Feature components belong inside their own feature.

Example:

features/
└── engagements/
    └── overview/
        └── components/
            ├── readiness-banner/
            ├── team-card/
            └── commercial-model-card/

These components should not be moved to Shared unless reused across multiple top-level features.

---

# Shared

Shared contains reusable BUSINESS components.

These understand the application's domain.

Examples:

shared/
├── components/
│   ├── user-display/
│   ├── engagement-status/
│   └── checklist-status/
│
├── services/
│   ├── user.service.ts
│   └── engagement.service.ts
│
├── models/
└── pipes/

Shared may import Common.

Shared must never import Features.

---

# Common

Common contains completely generic building blocks.

These should be reusable in any Angular application.

Examples:

common/
├── components/
│   ├── button/
│   ├── input/
│   ├── modal/
│   ├── spinner/
│   ├── badge/
│   ├── search-input/
│   └── empty-state/
│
├── services/
├── directives/
├── pipes/
└── interceptors/

Common must never depend on business logic.

Common must never import Shared or Features.

---

# Decision Rules

When creating new code:

Question 1

Is it only used by one page?

→ Keep it inside that page.

Example:

overview/components/team-card

---

Question 2

Is it reused by multiple pages inside Engagements?

→ Move it to:

features/engagements/components

---

Question 3

Is it reused by multiple top-level features and understands business concepts?

→ Move it to:

shared/

---

Question 4

Can it be copied into another Angular application without modification?

→ Move it to:

common/

---

# Dependency Rules

Allowed

Features
    ↓
Shared
    ↓
Common

Not Allowed

Common
    ↓
Shared

Common
    ↓
Features

Shared
    ↓
Features

---

# Code Generation Rules

When generating Angular code:

- Follow feature-first architecture.
- Use standalone components.
- Keep routing modular.
- Each feature owns its own routes file.
- Do not place business-specific components inside Common.
- Do not move feature components into Shared unless reused across multiple top-level features.
- Prefer local feature components before promoting them to Shared.
- Keep generic UI components in Common.
- Keep application-wide infrastructure inside Core.
- Keep layouts separate from features.
- Engagement tabs must be child routes rendered inside EngagementDetailComponent.