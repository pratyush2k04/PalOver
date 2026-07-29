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
├─ docker-compose.yml      # app + postgres
├─ backend/Pdr.Api/        # .NET Web API, EF Core, /api endpoints, serves wwwroot
└─ frontend/               # Angular app (proxy.conf.json → /api)
```

## Prerequisites

- .NET SDK 10
- Node.js 24.12.x (Angular 21) + Angular CLI 21
- Docker (optional, for the containerized stack / a throwaway Postgres)

## Local development

You need a Postgres database. Pick **one**:

- **No local install:** `docker compose up db` (exposes Postgres on `localhost:5432`).
  If port 5432 is already taken by a native Postgres, run it on another port by setting
  `DB_PORT` and point the API at that port (see below). Set the variable per your shell:
  - PowerShell: `$env:DB_PORT=5433; docker compose up db`
  - cmd.exe: `set DB_PORT=5433&& docker compose up db`
  - bash: `DB_PORT=5433 docker compose up db`
  - shell-agnostic: put `DB_PORT=5433` in a `.env` file next to `docker-compose.yml`, then
    just `docker compose up db`.
- **Native Postgres:** use your own install. If your credentials differ from the default
  (`Host=localhost;Port=5432;Database=pdr;Username=pdr;Password=pdr`), override them once
  without touching tracked files:
  ```bash
  cd backend/Pdr.Api
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=pdr;Username=<you>;Password=<pw>"
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

## Run the whole stack in Docker

```bash
docker compose up --build
```

Open <http://localhost:8080> — the .NET app serves the Angular app and the API from one
origin, backed by the Postgres container. Data persists in the `pdr_pgdata` volume across
restarts (`docker compose down` keeps it; `down -v` wipes it).

## Database / migrations

The connection string resolves in this order (later wins):

1. `appsettings.Development.json` — default `localhost:5432` (works for native **or** dockerized Postgres).
2. .NET **User Secrets** — per-developer override, untracked.
3. `ConnectionStrings__DefaultConnection` env var — used by Docker Compose to target the `db` service.

The API runs `Database.Migrate()` on startup (starter convenience). To manage migrations manually:

```bash
cd backend/Pdr.Api
dotnet ef migrations add <Name>
dotnet ef database update
```
