# Functional test results (F-01 – F-25)

Legend:

- **API/IT:** Covered by integration test or dev API script (see evidence).
- **UI:** End-to-end Blazor walkthrough in browser (not fully executed in this phase automation).

| ID | Scenario | Primary evidence | Status |
|----|----------|------------------|--------|
| F-01 | Register patient | IT `RegisterPatient_Succeeds`; dev API `functional-api-dev-run.txt` | Passed |
| F-02 | Login patient | IT `Login_SucceedsWithCorrectPassword`; dev API login | Passed |
| F-03 | Patient dashboard | UI not walked; patient JWT + `GET /api/appointments/my` OK | Not Executed (UI) |
| F-04 | Load specialties | IT `AuthenticatedUserCanListSpecialties` | Passed (API) |
| F-05 | Doctors by specialty | IT scheduling scenarios (doctor by specialty) | Passed (API) |
| F-06 | Load availability | IT cancel/reschedule availability tests | Passed (API) |
| F-07 | Schedule appointment | IT `PatientCanScheduleValidAppointment` | Passed (API) |
| F-08 | Confirmation screen | IT creates appointment; UI step 5 not captured | Not Executed (UI) |
| F-09 | My appointments | IT `PatientCanListOwnAppointments`; dev API 200 | Passed (API) |
| F-10 | Reschedule | IT `ReschedulingReleasesOldSlotAndReservesNewSlot` | Passed (API) |
| F-11 | Cancel | IT `CancellingOwnAppointmentReleasesAvailability` | Passed (API) |
| F-12 | History | IT lists/history via appointment workflows (status history persistence) | Passed (API) |
| F-13 | Update profile | No dedicated IT; UI form not executed | Not Executed |
| F-14 | Doctor agenda | IT `DoctorCanViewOwnAgenda` | Passed (API) |
| F-15 | Doctor confirm | IT `DoctorCanConfirmOwnAppointment` | Passed (API) |
| F-16 | Doctor complete | No integration test yet | Not Executed |
| F-17 | Doctor availability | Seeded in IT; doctor UI create not executed | Not Executed (UI) |
| F-18 | Admin create specialty | IT `AdministratorCanCreateSpecialty` | Passed (API) |
| F-19 | Admin update specialty | No IT; dev admin UI blocked without DevAdmin secrets | Blocked (UI) |
| F-20 | Admin deactivate specialty | No IT | Not Executed |
| F-21 | Admin create doctor | IT via `CreateDoctorAsync` helpers | Passed (API) |
| F-22 | Admin update doctor | No IT | Not Executed |
| F-23 | Admin deactivate doctor | No IT | Not Executed |
| F-24 | Admin view patients | No IT; dev admin UI blocked | Blocked (UI) |
| F-25 | Admin deactivate patient | No IT | Not Executed |

## Dev API script

`../Evidencias/functional-api-dev-run.txt` — register, login, appointments/my, 401/409 checks.

## Recommendation

Before deployment, complete **UI Not Executed** rows with a structured manual session (patient + integration admin or DevAdmin secrets for admin UI).
