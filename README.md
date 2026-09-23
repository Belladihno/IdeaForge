# IdeaForge

**AI Use Case Prioritization Tool** — a backend prototype for understanding
TeKnowledge's [inKnowVerse™](https://www.teknowledge.com/) innovation platform.

Team members submit AI ideas, the system scores them with a weighted formula,
a live leaderboard ranks them by priority, and reviewers govern them through a
pipeline from capture to live delivery (or rejection).

## Prototype disclaimer

This is a **learning prototype**, not production software. It mirrors
inKnowVerse™'s core innovation-pipeline loop (capture → score → rank → govern)
to understand how such a platform works. There is **no authentication** — anyone
with API access can submit ideas *and* move them through the pipeline. Do not
expose this beyond local development without adding access control.

It maps to inKnowVerse™ like this:

| inKnowVerse™ capability | IdeaForge implementation |
|---|---|
| Ideas enter from anywhere | Open submission endpoint, no login |
| Scored against criteria you define | Weighted score: Value, Feasibility, Urgency, Risk |
| Ranked so highest-value work ships first | Leaderboard ordered by priority score |
| Captured → Evaluated → In Build → Live | Status pipeline with transition rules |
| Human in the loop | Reviewers hold every status decision |
| Every idea tracked | Owner, status, reason, timestamps on each idea |
| Tells you where AI is not the answer | Rejection with mandatory reason |

## Workflow

```
Submit → Auto-score → Ranked → Evaluated → In Build → Live
              ↓            ↘ Rejected (from any early stage, reason required)
```

1. **Submit** — a team member posts title, description, department, name, and four
   1–5 self-assessed scores (Value, Feasibility, Urgency, Risk).
2. **Auto-score** — the Domain engine computes a priority score (0–21). High value
   pushes it up; high risk pulls it down (floored at zero).
3. **Rank** — the leaderboard lists everything highest-score-first in real time,
   with optional department / status / title-search filters.
4. **Govern** — a reviewer advances ideas one stage at a time (`Captured →
   Evaluated → In Build → Live`) or rejects with a reason. `Live`/`Rejected`
   are terminal; stages can't be skipped.

## Scoring

```
Priority Score = (Value × 2.0) + (Urgency × 1.5) + (Feasibility × 1.0) − (Risk × 1.5)
```

Floored at 0 → range **0–21**. Weights reflect enterprise judgment: business value
dominates, urgency matters next, feasibility gives a small boost, risk penalizes
heavily. The formula and weights are our own design choice — inKnowVerse™
publishes only the criteria dimensions (value, feasibility, security, compliance),
never a formula.

## Tech stack

- **.NET 8** ASP.NET Core Web API, **Clean Architecture** (Domain → Application → Infrastructure → API)
- **CQRS via MediatR**, validation via **FluentValidation**, mapping via **Mapster**
- **EF Core 9 + PostgreSQL (Npgsql)**, Fluent API configuration, code-first migrations
- **Serilog** logging, **Swagger (Swashbuckle)** API docs, **xUnit + FluentAssertions + NSubstitute** tests

## Prerequisites

- .NET 8 SDK
- PostgreSQL running locally + an `ideaforge` database
- EF Core CLI: `dotnet tool install -g dotnet-ef` (v9 line to match EF Core 9)

## Getting started

```bash
# 1. Connection strings
#    appsettings.json holds a placeholder.
#    Put the real one in IdeaForge.API/appsettings.Development.json (gitignored):
#    "DefaultConnection": "Host=localhost;Port=5432;Database=ideaforge;Username=postgres;Password=<yours>"

# 2. Apply migrations
dotnet ef database update \
  --project IdeaForge.Infrastructure/IdeaForge.Infrastructure.csproj \
  --startup-project IdeaForge.API/IdeaForge.API.csproj

# 3. Run (Development profile)
dotnet run --project IdeaForge.API/IdeaForge.API.csproj
# API: http://localhost:5162 · docs: http://localhost:5162/swagger

# 4. Tests
dotnet test IdeaForge.Tests/IdeaForge.Tests.csproj
```

## API reference

All responses share the envelope `{ "success", "data", "message" }`
(camelCase; enums serialized as strings, e.g. `"InBuild"`).

| Method | Path | Purpose | Success |
|---|---|---|---|
| POST | `/api/ideas` | Submit an idea | 201 + `{ id }` |
| GET | `/api/ideas?department=&status=&search=` | Ranked list (score desc) | 200 + `IdeaResponse[]` |
| GET | `/api/ideas/{id}` | Single idea | 200 / 404 |
| PATCH | `/api/ideas/{id}/status` | Move pipeline stage | 200 |

Errors: validation/illegal transition → 400 (messages joined with `"; "`),
missing idea → 404, unexpected → 500 `"Something went wrong. Please try again."`

## Project structure

```
IdeaForge/
├── IdeaForge.Domain/          # Entities, enums, PriorityScoreEngine (zero dependencies)
├── IdeaForge.Application/     # CQRS commands/queries, validators, DTOs, IIdeaRepository
├── IdeaForge.Infrastructure/  # EF Core DbContext, Fluent config, repository, migrations
├── IdeaForge.API/             # Controllers, middleware, response envelope, Program.cs
└── IdeaForge.Tests/           # Engine, handler (mocked repo), and validator tests
```

## Key decisions and why

- **Clean Architecture** — business logic (scoring, transitions) is isolated from
  frameworks and the database, so each can change independently. Standard in
  enterprise .NET; signals intent to senior readers.
- **Scoring lives in Domain** — it's core business logic, not a controller
  concern. Pure static function: trivially testable, zero dependencies.
- **CQRS + MediatR** — one focused file per operation; controllers stay thin
  (receive → dispatch → wrap → return). Reads and writes evolve separately.
- **FluentValidation in the MediatR pipeline** — invalid data never reaches a
  handler; one global behavior covers every command.
- **Fluent API over Data Annotations** — keeps EF attributes out of Domain
  entities, preserving the dependency rule.
- **GUID primary keys** — no exposed record counts, safe in distributed systems.
- **Enums stored as strings** — human-readable DB (`"Captured"` not `0`),
  immune to enum reordering bugs.
- **PATCH for status** — partial update semantics (PUT would imply full replacement).
- **Response envelope** — predictable `{success, data, message}` contract for any client.
- **Score floor at zero** — a leaderboard showing `-3` is nonsense; clamping keeps
  ordering intact while staying presentable.
- **No auth (prototype)** — matches the original TRD scope; the first production
  step is `[Authorize(Roles="Admin")]` on the status endpoint.

## Tests

8 tests, all green: formula correctness + boundaries + risk effect (Domain),
handler saves a correctly scored `Captured` entity via mocked repository
(Application), validator accepts/rejects (Application). Controllers and
repositories are intentionally untested in prototype scope.
