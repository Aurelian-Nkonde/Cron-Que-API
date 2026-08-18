# cron-que

A small .NET 10 Web API built to demonstrate three things end-to-end:
**JWT authentication with ownership enforcement**, **scheduled/queued
background jobs** (Hangfire, multiple queues), and a request lifecycle
that ties them together — not a CRUD-only demo.

## What it does

Users create `Item`s that carry a lifespan (2 / 5 / 15 minutes). On
creation, two background jobs are scheduled: one enqueues a "your item
was created" notification immediately, the other is scheduled to fire
when the item's lifespan runs out. When that expiry job fires, it flips
the item's status to `EXPIRED` and enqueues notifications to every user
who liked it. Nothing about that flow is driven by polling or a
front-end timer — it's all handled server-side by Hangfire, backed by
Postgres.

```
POST /api/items
  → item saved (Status: ACTIVE)
  → BackgroundJob.Enqueue   → "item created" notification
  → BackgroundJob.Schedule  → fires at ExpiresAt
        ↓ (after N minutes)
      ExpireItemAsync
        → Status: EXPIRED
        → BackgroundJob.Enqueue → notify every user who liked the item
```

## Tech stack

- **.NET 10 / ASP.NET Core** — Web API, minimal hosting model
- **EF Core + Npgsql** — Postgres persistence
- **Hangfire + Hangfire.PostgreSql** — job scheduling/queueing, backed by
  the same Postgres instance, three named queues
  (`general-queue`, `very-fast-queue`, `a-long-running-queue`)
- **JWT Bearer auth** — `Microsoft.AspNetCore.Authentication.JwtBearer`,
  passwords hashed with BCrypt
- **Scalar** — interactive OpenAPI docs in development

## Running it

Requires a Postgres instance reachable at the connection string in
`appsettings.json` (defaults to `localhost:5432`, db `cron-que`).

```bash
# apply migrations
dotnet ef database update

# run
dotnet run
```

- API: `http://localhost:5023`
- Interactive API docs (dev only): `http://localhost:5023/scalar/v1`
- Hangfire dashboard (queues, scheduled/recurring/failed jobs):
  `http://localhost:5023/hangfire`

## Auth flow

```bash
# register
curl -X POST http://localhost:5023/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Ada","email":"ada@example.com","password":"hunter2"}'

# login → returns { token, expiresAt }
curl -X POST http://localhost:5023/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"ada@example.com","password":"hunter2"}'

# authenticated request
curl http://localhost:5023/api/items \
  -H "Authorization: Bearer <token>"
```

All write endpoints except register and login require a valid JWT.
Writes that belong to a specific user (creating an item, liking an item,
updating an item's status, deleting a like) take the acting user's id
from the token, not from the request body — a caller can't act on
another user's behalf just by changing an id in the payload, and
`PATCH /items/{id}/status` / `DELETE /likes/{id}` return `403` if the
caller doesn't own the resource.

## API surface

| Resource | Endpoints |
|---|---|
| Auth | `POST /api/auth/login` |
| Users | `POST /api/users` (register, anonymous) · `GET /api/users` · `GET /api/users/{id}` |
| Items | `POST /api/items` · `GET /api/items` · `GET /api/items/{id}` · `PATCH /api/items/{id}/status` (owner only) |
| Likes | `POST /api/likes` · `GET /api/likes` · `DELETE /api/likes/{id}` (owner only) |
| Notifications | `GET /api/notifications` · `POST /api/notifications` |

## Project structure

```
Controllers/   HTTP endpoints, ownership checks
Service/       business logic + Hangfire job methods
Models/        EF entities
Dtos/          request/response records
Data/          AppDbContext
Migrations/    EF Core migrations
```

## Not in scope

Deliberately kept small for a portfolio project — no caching layer, no
test suite, no CI. See `FRONTEND_PLAN.md` for the (not yet built)
Next.js frontend that will make the auth/job flow visible in a browser
instead of curl.
# Cron-Que-API
