# PDR — Project Delivery Readiness

Monorepo for the PDR application: a **.NET 10 Web API** (PostgreSQL via EF Core) plus an
**Angular 21** front-end.

- **Production:** the .NET app serves the built Angular app (static files) and the API on a
  single port.
- **Local dev:** run the API and Angular independently; `ng serve` proxies `/api` to the API
  for full hot-reload.

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
