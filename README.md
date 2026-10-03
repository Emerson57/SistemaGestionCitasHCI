# SistemaGestionCitasHCI

High-fidelity academic prototype for **medical appointment management** with a Human-Computer Interaction (HCI) focus.

## Architecture

Layered modular backend:

- **Domain** — entities, enums, domain rules
- **Application** — use cases, DTOs, repository abstractions, `Result` pattern
- **Infrastructure** — EF Core, SQL Server, Identity, JWT token service
- **Api** — REST controllers, JWT authentication, ProblemDetails
- **Web** — Blazor Web App frontend (Interactive Server, Phase 5)
- **Tests** — unit tests (Domain/Application) and API integration tests

## Technologies

- .NET 10, C# 14
- ASP.NET Core Web API
- Entity Framework Core 10 + SQL Server (LocalDB for development)
- ASP.NET Core Identity + JWT Bearer
- xUnit

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (Visual Studio / Build Tools)
- Optional: `dotnet-ef` global tool for migrations

## Configuration

### Connection string

Default development connection (trusted LocalDB) is in `Backend/MedicalAppointments.Api/appsettings.json`.

### User Secrets (required for JWT)

Do **not** commit real signing keys.

From `Backend/MedicalAppointments.Api`:

```bash
dotnet user-secrets set "Jwt:SigningKey" "YourLocalDevSigningKey_AtLeast32Characters"
```

Optional **development seed** (patient, doctor, admin, specialties) — see [docs/dev-seed.md](docs/dev-seed.md). Set `DevSeed:Enabled` to `true` and configure `DevSeed:*` in User Secrets (passwords never committed). Legacy `DevAdmin:*` keys still work for admin only.

### JWT structure (non-secret values in appsettings)

- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpirationMinutes`

## Database migrations

```bash
dotnet ef database update \
  --project Backend/MedicalAppointments.Infrastructure/MedicalAppointments.Infrastructure.csproj \
  --startup-project Backend/MedicalAppointments.Api/MedicalAppointments.Api.csproj
```

## Run API

Apply migrations first (creates/updates `MedicalAppointmentsDb` on LocalDB):

```bash
dotnet ef database update \
  --project Backend/MedicalAppointments.Infrastructure/MedicalAppointments.Infrastructure.csproj \
  --startup-project Backend/MedicalAppointments.Api/MedicalAppointments.Api.csproj
```

```bash
dotnet run --project Backend/MedicalAppointments.Api/MedicalAppointments.Api.csproj
```

Default HTTPS URL (see `launchSettings.json`): `https://localhost:7211`

Development OpenAPI document: `https://localhost:7211/openapi/v1.json`

**Backend REST API (Phase 4) is complete.** **Blazor frontend (Phase 5)** lives in `Frontend/MedicalAppointments.Web`.

## Run Web (frontend)

Set `Api:BaseUrl` in `Frontend/MedicalAppointments.Web/appsettings.json` (default `https://localhost:7211`). Start the **API first**.

```bash
dotnet run --project Frontend/MedicalAppointments.Web/MedicalAppointments.Web.csproj
```

Default HTTPS: `https://localhost:7171`.

- JWT in Protected Session Storage; role-based redirects after login (see [docs/frontend.md](docs/frontend.md)).
- Screenshots: _(placeholder)_.

## Run tests

```bash
dotnet test SistemaGestionCitasHCI.slnx
```

Current baseline: 30 unit tests + 22 integration tests (52 total).

Integration tests use an isolated LocalDB database created per test run.

## Manual browser validation

Before demonstration deployment, complete [docs/manual-validation-guide.md](docs/manual-validation-guide.md) and [docs/deployment-gate.md](docs/deployment-gate.md). Sign-off form: [Pruebas/Funcionales/Manual-Browser-Signoff.md](Pruebas/Funcionales/Manual-Browser-Signoff.md).

## Security

Never commit passwords, JWT signing keys, or production connection strings with credentials.
