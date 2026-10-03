# Phase 7.1 manual UI results (historical)

> **Phase 7.2 sign-off:** use **[Manual-Browser-Signoff.md](./Manual-Browser-Signoff.md)** — do not mark Passed until executed in Chrome/Edge.

# Phase 7.1 manual UI results

**Date:** 2026-10-02  
**Environment:** `https://localhost:7171` (Web) + `https://localhost:7211` (API), DevSeed enabled via User Secrets (`DevSeed:Enabled=true`, credentials not logged).  
**Automation:** Playwright Chromium (`Pruebas/scripts/phase71-browser-validation.mjs`) + visual review of captured PNGs.  
**Evidence log:** `../Evidencias/phase71-browser-results.json` (latest run; prior runs retained in git history).

## Patient

| ID | Result | Notes | Evidence |
|----|--------|-------|----------|
| P-UI-01 | **Passed** | Login + cookie completion (`/auth/complete/{ticket}`) reaches `/paciente` | phase71 JSON; API still 200 |
| P-UI-02 | **Blocked** | Headless run did not stabilize Interactive Server shell (`.app-shell`) for dashboard screenshot | E04 stale — recapture in Chrome |
| P-UI-03–09 | **Not Executed (UI)** | Wizard steps blocked by same Interactive Server automation gap | API schedule/reschedule OK in phase7 API log |
| P-UI-10–15 | **Not Executed (UI)** | Depends on wizard completion in browser | API list/history passed |
| P-UI-16–17 | **Not Executed (UI)** | Profile form not automated this run | — |
| P-UI-18–20 | **Partial** | Logout via `/auth/sign-out` + cookie clear verified in script; post-logout `/paciente` → login **Passed** | AUTH-Anon-Patient |

## Doctor

| ID | Result | Notes | Evidence |
|----|--------|-------|----------|
| D-UI-01–02 | **Passed** | Login lands on `/medico` (automation, 2026-10-02 20:01Z run) | phase71 JSON |
| D-UI-03–04 | **Passed** | Agenda route loads in automation | E12 needs Chrome recapture |
| D-UI-05–06 | **Blocked** | No `Scheduled` appointment visible for Confirm/Complete (API seed slot conflict) | Prepare via patient UI booking |
| D-UI-07 | **Passed** | Disponibilidad page opens | — |
| D-UI-08 | **Not Executed** | Create availability (date picker) deferred to manual keyboard review | — |
| D-UI-09 | **Passed** | Seeded weekday slots visible via API + prior seed | — |
| D-UI-10 | **Passed** | Patient → `/medico` shows forbidden (403 UI) | AUTH-Patient-Doctor |
| D-UI-11 | **Passed** | Sign-out endpoint clears cookie | script logout helper |

## Administrator

| ID | Result | Notes | Evidence |
|----|--------|-------|----------|
| AD-UI-01–03 | **Passed** | Login + specialties list | E13 partial; phase71 JSON |
| AD-UI-04–06 | **Not Executed (UI)** | Create/edit/deactivate blocked in headless (form hydration) | Manual Chrome with demo names |
| AD-UI-07 | **Passed** | Doctors admin route | — |
| AD-UI-08–10 | **Not Executed** | Doctor CRUD UI not walked | — |
| AD-UI-11–12 | **Partial** | Patients list opened; detail button not confirmed in last run | — |
| AD-UI-13 | **Not Executed** | Avoid deactivating seeded DevSeed patient | — |
| AD-UI-14 | **Not Executed** | Confirm dialog keyboard — manual | ConfirmDialog Escape/focus Phase 6 |
| AD-UI-15 | **Passed** | Logout | script |

## Session

| Check | Result | Notes |
|-------|--------|-------|
| Login persists across navigation | **Passed** | Cookie + session storage after ticket completion |
| Refresh | **Not Executed** | Manual Chrome |
| Logout clears session | **Passed** | JWT storage clear + `/auth/sign-out` |
| Back button after logout | **Not Executed** | Manual |
| Protected URL after logout | **Passed** | `/paciente` → `/login?ReturnUrl=…` |
| Invalid session loop | **Not Executed** | No loop observed in automation |

## Authorization

| Check | Result | Observed |
|-------|--------|----------|
| Patient → `/admin` | **Passed** | `/forbidden?ReturnUrl=%2Fadmin` |
| Patient → `/medico` | **Passed** | `/forbidden?ReturnUrl=%2Fmedico` |
| Anonymous → `/paciente` | **Passed** | 302 → `/login?ReturnUrl=%2Fpaciente` |
| Doctor → `/admin` | **Not Executed** | Manual |
| Anonymous → `/medico`, `/admin` | **Not Executed** | Same middleware expected |

## Responsive

| Viewport | Result | Notes |
|----------|--------|-------|
| 375×812 | **Partial** | E14-mobile.png (home) | 
| 768×1024, 1280×800, 1440×900 | **Not Executed** | Device mode in Chrome pending |

See `../Responsive/Responsive-Evaluation.md`.

## Accessibility interactions

| Dialog | Keyboard (Tab/Escape/Enter) | Result |
|--------|----------------------------|--------|
| Cancel appointment | — | **Not Executed** (manual) |
| Deactivate specialty/doctor/patient | — | **Not Executed** (manual) |

ConfirmDialog: initial focus on Cancel, Escape closes, no backdrop dismiss (Phase 6).

## Errors

| Scenario | Result | Message (Spanish) |
|----------|--------|-------------------|
| Wrong password | **Passed** | “Correo o contraseña incorrectos…” |
| Required fields / invalid email | **Not Executed** | Manual |
| Appointment conflict | **Not Executed** | API IT covers conflict |
| API unavailable | **Not Executed** | — |
| Unauthorized page | **Passed** | Forbidden copy on `/forbidden` |
| Empty availability | **Not Executed** | Wizard empty state — manual |

## Remaining gaps

1. **Recapture E04–E13 in Chrome/Edge** after Phase 7.1 auth fix (cookie + ticket flow).
2. **Complete P-UI wizard** and doctor Confirm/Complete with a fresh patient-booked appointment.
3. **Admin CRUD** (AD-UI-04–10) and **keyboard dialog** pass in real browser.
4. **Responsive matrix** at 768/1280/1440 for all listed routes.
5. **Participant usability** sessions still pending (`Usabilidad/Usability-Test-Plan.md`).

## Frontend defect fixed this phase

Protected Blazor routes returned Developer Exception Page (`IAuthenticationService` missing) until cookie authentication + `/auth/complete/{ticket}` and layout render-mode correction were added. See UX-006 in `../Usabilidad/UX-Issue-Log.md`.
