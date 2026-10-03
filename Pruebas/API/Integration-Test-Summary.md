# API integration test summary

## Framework

- **xUnit** + `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`)
- **Collection fixture:** `IntegrationTestFixture` (single factory per collection)

## Database

- **SQL Server LocalDB**, isolated database per test run: `MedicalAppointmentsIntegrationTests_{Guid}`
- Migrations applied on fixture init; database **deleted** on dispose
- Not the developer `MedicalAppointmentsDb` used at runtime

## Lifecycle

1. Set environment variables (connection string, JWT signing key, IntegrationTesting environment)
2. Create `WebApplicationFactory`
3. Migrate + seed Identity roles
4. Seed integration admin user (`admin.integration@test.local`)
5. Run tests against in-memory HTTP client (no external Kestrel required)

## Endpoint groups covered

| Area | Examples |
|------|----------|
| Auth | `POST /api/auth/register`, `POST /api/auth/login` |
| Patients | `GET /api/patients/me`, forbidden cross-patient access |
| Specialties | `GET/POST /api/specialties` (admin create) |
| Doctors | Create via admin helpers, agenda, confirm |
| Appointments | Schedule, list mine, conflict 409, cancel, reschedule |
| Availability | Slot release/reserve on cancel/reschedule |

## Test count

**22** integration tests (19 API workflow + 3 persistence), all **Passed** on 2026-10-02.

## Command

```bash
dotnet test Tests/MedicalAppointments.IntegrationTests/MedicalAppointments.IntegrationTests.csproj
```

## Evidence

`../Evidencias/integration-tests-output.txt`

## Note on development API

Manual calls against `https://localhost:7211` use the **developer** LocalDB database and JWT User Secrets. Integration tests do **not** modify that database.
