# Frontend Plan

A minimal frontend whose only job is to make the three things this project
is actually about *visible*: **auth**, **background jobs**, and
**caching**. Not a product UI — a demo surface.

## Stack

Next.js (App Router) + TypeScript + Tailwind CSS + shadcn/ui, in its own
project directory (already scaffolded). This doc only adds structure on
top of what's there — no scaffolding steps here.

## Pages

| Page | Route | shadcn components | What it proves |
|---|---|---|---|
| Login / Register | `/login` | `Card`, `Input`, `Button`, `Form` | Auth: calls `POST /api/auth/login` and `POST /api/users` (register), stores the JWT client-side, redirects into the app on success. |
| Items | `/items` | `Table` or `Card` grid, `Select` (Times: 2/5/15 min), `Dialog` or inline form | List + create. Scheduling an item here is what lets a viewer watch something actually happen later. |
| Item Detail | `/items/[id]` | `Badge` (ACTIVE/EXPIRED), `Button` (like/unlike) | Live countdown to `ExpiresAt`; refetching after it hits zero flips the badge — **the background-job payoff**. |
| Notifications | `/notifications` | `Card`/list | Polls `GET /api/notifications` every few seconds (`useEffect` + `setInterval` — no websockets, not worth it here). Entries appear on their own as expiry jobs fire — **the other background-job payoff**. |
| Jobs | `/jobs` | — | Not reimplemented; a link/iframe to the existing Hangfire dashboard at `/hangfire`, which already visualizes queues, scheduled jobs, and recurring jobs. |

Five pages, no more. Settings, profile editing, and admin screens are
explicitly out of scope.

## Auth mechanics

- `AuthContext` (React context) holding the JWT, backed by `localStorage`
  so a refresh doesn't force re-login.
- `useAuth()` hook for reading the token / current user / logout.
- A simple client-side guard component that redirects to `/login` when
  there's no token — not Next middleware, not a real security boundary,
  just enough to gate the demo pages. The actual authorization boundary is
  the API, which already rejects unauthenticated/non-owner requests.

## Caching tie-in

Once the caching work lands (Redis, cache-aside on `GetAllItems`/
`GetItemById`), the API should expose a cheap, visible signal — e.g. an
`X-Cache: HIT|MISS` response header — so the `/items` page can show a
small "served from cache" badge on repeat loads. Coordination point with
the caching task, not built here.

## Backend change needed before the frontend can talk to the API

The API has no CORS policy today. The Next dev server (port `3000`)
calling the API (a different port) cross-origin will fail without adding
`builder.Services.AddCors(...)` + `app.UseCors(...)` in `Program.cs`,
scoped to the frontend's dev origin. Small change, needed once the
frontend actually starts calling the API — not done in this pass.

## Explicitly out of scope

- No server-side (RSC) fetching with forwarded auth cookies — keep it
  client-fetched and simple.
- No frontend test suite.
- No pages beyond the five above.

Goal is a demo surface that proves the backend works, not a second
project to maintain.
