# Nielsen heuristic evaluation

High-fidelity Blazor prototype — evaluator review 2026-10-02. Severity: 0 none, 1 cosmetic, 2 minor, 3 major, 4 critical.

| # | Heuristic | Example (screen) | Evidence | Improvement | Sev |
|---|-----------|------------------|----------|-------------|-----|
| 1 | Visibility of system status | Wizard stepper shows current step | `/paciente/citas/nueva` | Authenticated Lighthouse optional | 0 |
| 2 | Match system ↔ real world | “Cita”, “Especialidad”, “Médico” | All patient UI | — | 0 |
| 3 | User control & freedom | Back on wizard; cancel dialog | Wizard, `/paciente/citas` | — | 0 |
| 4 | Consistency | Shared `.btn`, `.card`, layout | Patient/doctor/admin | — | 0 |
| 5 | Error prevention | Confirm before cancel/deactivate | `ConfirmDialog` | — | 0 |
| 6 | Recognition over recall | Review step summary | Wizard step 4 | — | 0 |
| 7 | Flexibility | Dashboard shortcuts | `/paciente` | — | 0 |
| 8 | Aesthetic & minimalist | No KPI charts on patient home | `/paciente` | — | 0 |
| 9 | Recover from errors | Spanish `ApiErrorTranslator` | Login, forms | — | 0 |
| 10 | Help & documentation | Empty states with next action | Lists | Optional FAQ page | 1 |

No critical (4) or major (3) heuristic violations identified in static review.

Cross-reference: `docs/design-traceability.md`, `docs/frontend.md`.
