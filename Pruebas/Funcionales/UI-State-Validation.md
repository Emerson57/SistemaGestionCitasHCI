# Loading / empty / success state validation

| Screen / data | Loading | Empty | Success | Failure |
|---------------|---------|-------|---------|---------|
| Specialties (wizard) | `LoadingIndicator` | `EmptyState` | Card grid | `validation-summary` |
| Doctors | Same | Same | Same | Same |
| Availability | Same | Same | Time grid | Same |
| My appointments | Same | Empty + CTA | List cards | `ErrorState` + retry |
| History | Same | Empty message | List | `ErrorState` |
| Patient dashboard | Skeleton | Empty next + CTA | Next card | `ErrorState` |
| Doctor agenda | Same | Empty agenda | List | `ErrorState` |
| Admin lists | Same | EmptyState | Cards | Partial (some pages) |
| Profile save | Button disabled | — | `alert-success` | validation summary |
| Appointment confirm | — | — | `success-panel` step 5 | — |

Verified by component/code review; interactive walkthrough **recommended** before final demo.
