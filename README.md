# PDR — Project Delivery Readiness

Monorepo for the PDR application: a **.NET 10 Web API** (PostgreSQL via EF Core) plus an
**Angular 21** front-end.

- **Production:** the .NET app serves the built Angular app (static files) and the API on a
  single port.
- **Local dev:** run the API and Angular independently; `ng serve` proxies `/api` to the API
  for full hot-reload.

## About the Project

**PDR (Project Delivery Readiness)** is a governance and transition platform built to streamline the handover process from the **Pre-Sales / Deal Team** to the **Project Manager (PM) and Delivery Team** before client **Kickoff**.

In technology delivery and client engagements, project friction, budget overruns, and Day-1 delivery failures frequently stem from:
- Sales promises and commitments that delivery was unaware of or never validated.
- Misaligned client expectations, missing background context, or unvetted commercial terms.
- Delayed access to client repositories, environments, VPNs, and tools on Day 1.
- Incomplete documentation, unmanaged project risks, and unconfirmed staffing allocations.

PDR addresses these challenges by instituting a structured, transparent, and auditable handover-to-kickoff lifecycle before teams meet with the client.

### The Handover to Kickoff Lifecycle

```
[ Pre-Sales Deal ] ──> [ Pre-Sales Handover ] ──> [ Handover Submission ]
                                                         │
                                                         ▼
[ Client Kickoff & Day-1 ] <── [ Kickoff-Ready Gate ] <── [ PM & Delivery Review ]
                               (Readiness Scoring)      (Realistic Estimates & Queries)
```

1. **Pre-Sales Handover & Context Capture**
   - **Client Context**: Captures critical client background, the core business problem being solved, deal origin, and client sentiment (*New*, *Warm*, or *Strained*).
   - **Sales Commitments**: Catalogs promises made during the sales cycle across Software, Services, and Training, including committed timelines.
   - **Commercial & Readiness Checklist**: Pre-sales verifies readiness criteria and attaches initial commercial agreements (e.g. SOW, proposal, RFP).

2. **Delivery & PM Review (Reality Check & Alignment)**
   - **Promised vs. Realistic Estimates**: Project Managers and Tech Leads review commitments against delivery realities. Any timeline or scope variance is automatically calculated and flagged, requiring explicit acknowledgment, mitigation rationale, or escalation to leadership.
   - **In-Line Handover Queries**: Delivery raises threaded queries directly on specific fields or commitments. All queries must be resolved between Pre-Sales and Delivery before the handover can be accepted.
   - **Formal Acceptance**: Once queries are settled and realistic commitments are established, the handover is marked as *Reviewed*.

3. **Technical & Operational Readiness**
   - **Access Tracker**: Tracks accounts, credentials, cloud access, repositories, and VPN permissions needed before Day 1.
   - **Environment & Tooling**: Configures development environments, CI/CD pipelines, and project tools.
   - **Architecture & Discovery**: Documents brownfield findings, technical debt, and architecture validations.

4. **Kickoff Gate & Client Kickoff**
   - **Readiness Scoring**: Continuously computes an objective readiness score across handover fields, commitment acknowledgments, and mandatory checklist gates.
   - **Proximity-Based Health Tiers**: Dynamically evaluates readiness against required thresholds based on days remaining until kickoff (`>14 days`, `8–14 days`, `4–7 days`, `1–3 days`, `Day 0`).
   - **Kickoff-Ready Gate**: Prevents premature kickoffs. Engagements must reach 100% readiness (or obtain an authorized Leadership override) before being certified *Kickoff-Ready*.
   - **Client Kickoff Meeting & Day-1 Sign-Off**: Once cleared, the kickoff meeting with the client is held, recording agreed outcomes, actionable next steps, an audit-proof evidence snapshot, and Day-1 sign-off.

### Core Engagement Capabilities

Beyond the handover review and kickoff gating, PDR equips teams with dedicated modules to manage engagement assets, risk posture, and staffing:

- **Document Management & Evidence Repository**:
  - Upload, categorize, and organize critical engagement documents (*RFP*, *Proposal*, *Signed SOW*, *Commercial Agreements*, *Design or Approach*, and *Email Threads*).
  - Multi-version document history with audit tracking and secure downloads.
  - Role-based visibility controls (*Team-wide*, *PM-only*, or custom role-restricted access).
  - Directly attaches uploaded documents as verifiable evidence for handover checklist items, technical prerequisites, and Day-1 sign-offs.

- **Risk Management & Blocker Escalation**:
  - Log and monitor engagement risks with structured impact and likelihood scoring (*Low*, *Medium*, *High*, *Critical*).
  - Automatically captures blockers originating from the Checklist, Access Tracker, or Environment Tooling as formal risks.
  - In-line collaboration with threaded discussions, mentions, and mitigation action tracking.
  - One-click escalation to Leadership for urgent blockers that jeopardize kickoff readiness.

- **Resource Requests & Team Allocation**:
  - Raise structured staffing requests specifying standard roles (e.g. *Senior Developer*, *Developer*, *QA Engineer*, *Solution Architect*, *Business Analyst*) and specialized skill sets (e.g. *.NET*, *Angular*, *Cloud / DevOps*, *Data Engineering*).
  - Define required headcount, job descriptions, and needed-by dates aligned with billing milestones.
  - Track named resource allocations, fulfillment status, and reallocation history.
  - Automated resourcing gap detection flags unconfirmed resource requests against upcoming billing start dates, alerting leadership to staffing deficits before kickoff.

## Layout

```
pdr-app/
├─ Pdr.slnx               # .NET 10 solution
├─ Dockerfile              # single multi-stage image (node build → dotnet publish → runtime)
├─ docker-compose.yml      # app only — connects to whatever Postgres you point it at via .env
├─ backend/Pdr.Api/        # .NET Web API, EF Core, /api endpoints, serves wwwroot
└─ frontend/               # Angular app (proxy.conf.json → /api)
```

## Prerequisites

- .NET SDK 10
- Node.js 24.12.x (Angular 21) + Angular CLI 21
- Docker (optional, to run the app in a container)
- A Postgres instance (native install, or your own container) — this repo does not provide one

## Local development

There is no Postgres container in this repo — bring your own Postgres (a native install,
or one you run yourself in Docker) and point the app at it. Since `appsettings.Development.json`
no longer carries a real password, set your local secrets once via .NET **User Secrets**
(already wired up via `UserSecretsId`, untracked by git):
```bash

cd backend/Pdr.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=pdrDB;Username=<you>;Password=<pw>"
dotnet user-secrets set "JwtSettings:SecretKey" "<any-local-dev-value>"
```

Then run the two dev servers (separate terminals):

```bash
# Terminal 1 — API on http://localhost:5000 (applies migrations + seeds on startup)
cd backend/Pdr.Api
dotnet run

# Terminal 2 — Angular on http://localhost:4200 (proxies /api → :5000)
cd frontend
ng serve
```

Open <http://localhost:4200>. The sample page lists/creates notes through the proxy.

## Run the app in Docker

The compose file only builds/runs the `app` container — no Postgres container. Copy
`.env.example` to `.env` and set `ConnectionStrings__DefaultConnection` to point at your
Postgres (use `host.docker.internal` instead of `localhost` if Postgres runs on your host
machine) and `JwtSettings__SecretKey` (Compose reads `.env` automatically; it's gitignored
and local-only):

```bash
docker compose up --build
```

Open <http://localhost:8080> — the .NET app serves the Angular app and the API from one origin.

## Database / migrations

The connection string resolves in this order (later wins):

1. `appsettings.Development.json` — default `localhost:5432`, no password (works for native **or** dockerized Postgres).
2. .NET **User Secrets** — per-developer override, untracked.
3. `ConnectionStrings__DefaultConnection` env var — set via your local `.env` when using Docker Compose, or injected directly by the hosting platform in a managed-server deployment.

The API runs `Database.Migrate()` on startup (starter convenience). To manage migrations manually:

```bash
cd backend/Pdr.Api
dotnet ef migrations add <Name>
dotnet ef database update
```

## Managed server deployment

There is no `.env` file on the managed server and no dockerized Postgres in production —
the hosting platform passes secrets straight into the running container as environment
variables (e.g. via `docker run -e ...` or the platform's own env/secret configuration UI).
ASP.NET Core reads process environment variables automatically (`Section__Key` maps to
`Section:Key` in config), so no code changes are needed — just set these on the container:

| Env var | Required | Notes |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Yes | Full connection string for the external/managed Postgres database. |
| `JwtSettings__SecretKey` | Yes | Generate a fresh, strong random secret. **Do not reuse the old dev key** — it was committed to git history and is compromised. |
| `JwtSettings__Issuer` | No | Defaults to `PdrApi` from `appsettings.json` if unset. |
| `JwtSettings__Audience` | No | Defaults to `PdrWeb` from `appsettings.json` if unset. |
| `JwtSettings__ExpiryMinutes` | No | Defaults to `30` from `appsettings.json` if unset. |
