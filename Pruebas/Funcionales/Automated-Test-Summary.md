# Automated test summary

**Execution date:** 2026-10-02 (local development machine)  
**Command:** `dotnet test SistemaGestionCitasHCI.slnx`

## Results

| Suite | Project | Passed | Failed | Skipped | Total |
|-------|---------|--------|--------|---------|-------|
| Unit | MedicalAppointments.UnitTests | 30 | 0 | 0 | 30 |
| Integration | MedicalAppointments.IntegrationTests | 22 | 0 | 0 | 22 |
| **Total** | | **52** | **0** | **0** | **52** |

## Unit test areas

- **Domain:** appointment rules, specialty, doctor availability
- **Application:** appointment service (schedule, conflict, cancel, reschedule), doctor service, specialty service

## Integration test areas

- Auth register/login, duplicate email, invalid password
- Authorization (401 anonymous, 403 patient/admin boundaries)
- Appointments lifecycle, conflicts, cancel, reschedule, availability
- Doctor agenda and confirm; cross-doctor isolation
- Admin specialty creation
- Persistence of status history (dedicated persistence tests)

## Evidence

- `../Evidencias/dotnet-test-full-output.txt`
- `../Evidencias/integration-tests-output.txt`

Build after Phase 6 dialog fix: **0 errors, 0 warnings** (see completion report).
