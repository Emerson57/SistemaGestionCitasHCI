# Release readiness (academic prototype)

**Evidence-based conclusion:** Suitable for **academic demonstration** after **manual browser sign-off** (`Pruebas/Funcionales/Manual-Browser-Signoff.md`). **Not** production or clinical deployment.

**Do not deploy** to production without addressing deployment blockers below.

**Manual validation:** [manual-validation-guide.md](manual-validation-guide.md) · **Gate:** [deployment-gate.md](deployment-gate.md)

---

## Automated validation

| Item | Status |
|------|--------|
| `dotnet build` (solution) | **0 errors, 0 warnings** (with API/Web stopped) |
| `dotnet test` | **52/52 passed** (30 unit + 22 integration) |
| Evidence | `Pruebas/Evidencias/dotnet-test-full-output.txt`, `Pruebas/API/Integration-Test-Summary.md` |

---

## API manual validation

| Item | Status |
|------|--------|
| DevSeed patient/doctor/admin flows | **Passed** |
| Appointment schedule / reschedule / cancel / history | **Passed** |
| Role 403 on cross-role API actions | **Passed** |
| Evidence | `Pruebas/Evidencias/phase7-manual-api-validation.txt` |

---

## Browser UI validation

| Item | Status |
|------|--------|
| Auth bridge (cookie + ticket) | **Implemented & reviewed** — `docs/frontend.md` |
| PAT / DOC / ADM manual checklists | **Not Executed** — use `Manual-Browser-Signoff.md` |
| Phase 7.1 headless automation | **Partial smoke only** — not sign-off |
| Wrong-password message (observed in automation) | Spanish, non-technical |

---

## Responsive validation

| Item | Status |
|------|--------|
| DevTools matrix 375 / 768 / 1280 / 1440 | **Not Executed** (all cells) |
| Public mobile home | **E14-mobile.png** @ 375×812 |
| Reference | `Pruebas/Responsive/Responsive-Evaluation.md` |

---

## Accessibility

| Item | Status |
|------|--------|
| Lighthouse (public login/home) | Documented — `Pruebas/Rendimiento/Lighthouse-Summary.md` |
| ConfirmDialog keyboard | Component improved (focus capture/restore Phase 7.2); **manual dialog matrix pending** |
| Authenticated pages Lighthouse | **Not Executed** |

---

## Lighthouse

Public routes executed; authenticated `/paciente` performance **Not Executed** (session).

---

## Usability participant testing

**Pending execution with participants.** Plan: `Pruebas/Usabilidad/Usability-Test-Plan.md`. No fabricated results.

---

## Evidence package

| Asset | Status |
|-------|--------|
| E01, E02, E03, E14 | **Available** (public, 1440 / 375) |
| E04–E13 | **Pending human capture** in Chrome @ 1440×900 |
| Invalid pre-fix screenshots | **Removed** from `Evidencias/Screenshots/` |
| Index | `Pruebas/Evidencias/README.md` |

---

## Known limitations

- Interactive Server limits reliable unattended authenticated UI capture (architecture unchanged by design).
- DevSeed credentials must never be used in production.
- No EHR/clinical scope.
- Some admin UI paths lack dedicated integration tests (API admin partially covered).

---

## Deployment readiness

| Gate | Status |
|------|--------|
| Production hosting / Azure | **Not started** |
| DevSeed disabled in Production | **Enforced in code** |
| Secrets in User Secrets / env | **Required** |
| Manual browser sign-off complete | **No** — demonstration blocker |
| Participant usability | **Pending** (transparent) |

**Recommendation for demonstration deployment:** Complete `Manual-Browser-Signoff.md`, capture E04–E13, update responsive matrix, then proceed with **controlled** demo hosting (HTTPS, secrets store, DevSeed off). Do not claim production readiness.
