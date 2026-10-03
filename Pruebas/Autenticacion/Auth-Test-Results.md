# Authentication and authorization tests

| ID | Scenario | Expected | Actual | Status | Evidence |
|----|----------|----------|--------|--------|----------|
| A-01 | Protected route without login | 401 API | HTTP 401 on `GET /api/patients/me` | Passed | IT `AnonymousProtectedEndpoint_ReturnsUnauthorized`; dev API log |
| A-02 | Valid patient token | 200 on own resources | 200 on `GET /api/appointments/my` with Bearer | Passed | dev API log |
| A-03 | Invalid token | 401 | HTTP 401 with malformed JWT | Passed | dev API log |
| A-04 | Expired session (UI) | Clear session, redirect login | Implemented: `SessionExpiredHandler` → `/login?expired=1` | Not Executed (timed expiry) |
| A-05 | Patient → admin API | 403 | HTTP 403 on `POST /api/specialties` as patient | Passed | IT `PatientCannotCreateSpecialty` |
| A-06 | Patient → doctor action | 403 | IT `DoctorCannotConfirmAnotherDoctorsAppointment` (patient/doctor boundaries) | Passed (API) |
| A-07 | Doctor → admin page | Redirect/forbidden UI | `[Authorize(Roles=Administrator)]` on `/admin` | Not Executed (UI) |
| A-08 | Doctor ≠ other doctor appointment | 403 | IT `DoctorCannotConfirmAnotherDoctorsAppointment` | Passed |
| A-09 | Patient ≠ other patient appointment | 403 | IT `PatientCannotCancelAnotherPatientsAppointment` | Passed |
| A-10 | Admin role | Admin API OK | IT `AdministratorCanCreateSpecialty` with fixture admin | Passed (API) |

## Frontend behavior (documented, partial execution)

- Unauthenticated access to protected Blazor routes → `RedirectToLogin` (code review).
- Login failure → generic Spanish message (`ApiErrorTranslator.LoginFailureMessage`).
- Role dashboards after login → `/paciente`, `/medico`, `/admin` (code review).

## Evidence files

- `../Evidencias/integration-tests-output.txt`
- `../Evidencias/functional-api-dev-run.txt`
