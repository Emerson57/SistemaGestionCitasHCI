# Master test matrix

**Project:** SistemaGestionCitasHCI — Phases 6–7  
**Last update:** 2026-10-02 (Phase 7.2 sign-off package; browser checklist not prefilled)

Status values: **Passed** | **Failed** | **Blocked** | **Not Executed**

---

## Functional (F-01 – F-25)

| ID | Area | Scenario | Preconditions | Steps (summary) | Expected | Actual | Status | Evidence | Notes |
|----|------|----------|---------------|-----------------|----------|--------|--------|----------|-------|
| F-01 | Funcional | Register patient | API up | POST register | 201 + token | Token returned | Passed | IT + `functional-api-dev-run.txt` | |
| F-02 | Funcional | Login patient | User exists | POST login | 200 + token | OK | Passed | IT + dev API | |
| F-03 | Funcional | Patient dashboard | DevSeed patient | Open `/paciente` | Dashboard | Login OK; dashboard PNG pending Chrome | **Partial** | `phase71-browser-results.json` | Phase 7.1 auth fix |
| F-04 | Funcional | Load specialties | Auth | GET specialties | 200 list | IT passed | Passed | IT `AuthenticatedUserCanListSpecialties` | |
| F-05 | Funcional | Doctors by specialty | Specialty id | GET doctors | Filtered list | IT scenarios | Passed | IT | |
| F-06 | Funcional | Load availability | Doctor slots | GET availability | Slots | IT cancel/reschedule | Passed | IT | |
| F-07 | Funcional | Schedule appointment | Slot free | POST appointment | 201 | IT passed | Passed | IT | |
| F-08 | Funcional | Confirmation UI | After schedule | View step 5 | Persistent success | UI not captured | Not Executed | — | API OK |
| F-09 | Funcional | My appointments | Patient | GET my | 200 list | 200 dev | Passed | IT + dev API | |
| F-10 | Funcional | Reschedule | Active appt | PUT reschedule | New datetime | IT passed | Passed | IT | |
| F-11 | Funcional | Cancel | Active appt | DELETE cancel | Cancelled | IT passed | Passed | IT | |
| F-12 | Funcional | History | Patient | GET history | Past list | IT persistence | Passed | IT | |
| F-13 | Funcional | Update profile | Patient | PUT me | Updated | No IT/UI run | Not Executed | — | |
| F-14 | Funcional | Doctor agenda | Doctor JWT | GET agenda | Own appts | IT passed | Passed | IT | |
| F-15 | Funcional | Doctor confirm | Scheduled | POST confirm | Confirmed | IT passed | Passed | IT | |
| F-16 | Funcional | Doctor complete | Confirmed | POST complete | Completed | No IT | Not Executed | — | |
| F-17 | Funcional | Doctor availability | Doctor | POST availability | Created | UI not run | Not Executed | — | |
| F-18 | Funcional | Admin create specialty | Admin | POST specialty | Created | IT passed | Passed | IT | |
| F-19 | Funcional | Admin update specialty | Admin | PUT | Updated | Not run | Not Executed | — | DevSeed available |
| F-20 | Funcional | Admin deactivate specialty | Admin | DELETE/deactivate | Inactive | No run | Not Executed | — | |
| F-21 | Funcional | Admin create doctor | Admin | POST doctor | Created | IT helpers | Passed | IT | |
| F-22 | Funcional | Admin update doctor | Admin | PUT | Updated | No run | Not Executed | — | |
| F-23 | Funcional | Admin deactivate doctor | Admin | Deactivate | Inactive | No run | Not Executed | — | |
| F-24 | Funcional | Admin view patients | Admin | GET/list | List | Not run UI | Not Executed | — | AD-UI API login OK |
| F-25 | Funcional | Admin deactivate patient | Admin | Deactivate | Inactive | No run | Not Executed | — | |

---

## Authentication / authorization (A-01 – A-10)

| ID | Area | Scenario | Preconditions | Steps | Expected | Actual | Status | Evidence | Notes |
|----|------|----------|---------------|-------|----------|--------|--------|----------|-------|
| A-01 | Auth | No login | Anonymous | GET protected | 401 | 401 | Passed | IT + dev API | |
| A-02 | Auth | Valid patient token | Bearer | GET my data | 200 | 200 | Passed | dev API | |
| A-03 | Auth | Invalid token | Bad JWT | GET | 401 | 401 | Passed | dev API | |
| A-04 | Auth | Expired session UI | Short JWT | Wait/expired API call | Redirect login | Not timed | Not Executed | — | Handler exists |
| A-05 | Auth | Patient → admin API | Patient JWT | POST specialty | 403 | 403 | Passed | IT | |
| A-06 | Auth | Patient → doctor API | Mixed roles | Forbidden action | 403 | 403 | Passed | IT | |
| A-07 | Auth | Patient → admin/medico UI | Patient cookie | Navigate `/admin`, `/medico` | Forbidden | `/forbidden?ReturnUrl=…` | **Passed** | `phase71-browser-results.json` | Phase 7.1 |
| A-08 | Auth | Doctor A ≠ B appt | Two doctors | Confirm other | 403 | 403 | Passed | IT | |
| A-09 | Auth | Patient A ≠ B appt | Two patients | Cancel other | 403 | 403 | Passed | IT | |
| A-10 | Auth | Admin role | Admin JWT | Admin API | 200/201 | IT passed | Passed | IT | |

---

## Evidence type summary (Phase 7.2)

| Channel | Scope | Status | Evidence |
|---------|-------|--------|----------|
| **Automated** | Unit + integration (52) | **Passed** | `Evidencias/dotnet-test-full-output.txt` |
| **Manual API** | DevSeed lifecycle | **Passed** | `phase7-manual-api-validation.txt` |
| **Manual Browser** | PAT / DOC / ADM checklists | **Not Executed** | `Funcionales/Manual-Browser-Signoff.md` |
| **Hotfix UX-009** | Login redirect after valid credentials | **Browser PASS (patient `/paciente`)** | Login + auth bridge | Doctor/admin redirect: verify with DevSeed User Secrets |
| **Hotfix UX-010** | False session expired on patient pages | **Fix applied — stability PASS pending** | API session cookie + 401 handling | 10× navigate/reload sign-off |
| **Hotfix UX-011** | Registration UI failure with DB persisted | **Fix applied — browser R1–R5 pending** | Register auth bridge + messaging | Do not PASS registration on DB rows alone |
| **Automation smoke** | Public PNG + partial auth (7.1) | **Partial / not sign-off** | `phase71-browser-results.json` |

Do **not** treat API Passed as Browser Passed for the same user story.

## Manual browser checklists (PAT / DOC / ADM)

Executable checklist: **`Funcionales/Manual-Browser-Signoff.md`**. All rows start **blank** — fill only after real Chrome/Edge execution.

| Checklist | Items | Passed | Failed | Blocked | Not Executed |
|-----------|-------|--------|--------|---------|--------------|
| Patient PAT-01–20 | 20 | 0 | 0 | 0 | **20** |
| Doctor DOC-01–12 | 12 | 0 | 0 | 0 | **12** |
| Admin ADM-01–15 | 15 | 0 | 0 | 0 | **15** |
| Dialog keyboard (4 dialogs × checks) | — | — | — | — | **Not Executed** |
| Responsive matrix (11×4 widths) | 44 cells | — | — | — | **Not Executed** |

Historical Phase 7.1 automation notes: `Funcionales/Phase7-Manual-UI-Results.md` (superseded for sign-off by Manual-Browser-Signoff.md).

---

## Cross-cutting (documented elsewhere)

| Topic | Document |
|-------|----------|
| Lighthouse | `Rendimiento/Lighthouse-Summary.md` |
| Accessibility | `Accesibilidad/Accessibility-Evaluation.md` |
| Keyboard | `Accesibilidad/Keyboard-Test-Results.md` |
| Responsive | `Responsive/Responsive-Evaluation.md` |
| API timing | `Rendimiento/Api-Response-Times.md` |
| Usability sessions | `Usabilidad/Usability-Test-Plan.md` (pending) |
| Nielsen | `Usabilidad/Nielsen-Heuristic-Evaluation.md` |
