# Error state validation

| Scenario | Expected UI/API | Executed | Evidence |
|----------|-----------------|----------|----------|
| Invalid login | Generic Spanish message | API IT + code | `Login_FailsWithIncorrectPassword` |
| Duplicate email | 409 / understandable message | API dev + IT | `RegisterPatient_DuplicateEmailFails`, dev log |
| Empty required fields | Validation summary | Code review | DataAnnotations on login/register |
| Unavailable slot | 409 Spanish message | IT | `OccupiedSlotProducesConflict` |
| API unavailable | ErrorState / message | Not simulated (server stop) | Not Executed |
| Unauthorized action | 403 / forbidden page | IT + `[Authorize]` | Multiple integration tests |
| No specialties | EmptyState | Code review | Wizard step 1 |
| No doctors | EmptyState + back | Code review | Wizard step 2 |
| No availability | EmptyState message | Code review | Wizard step 3 |
| No appointments | EmptyState + CTA | Code review | `/paciente/citas` |

Messages must not expose stack traces or SQL — confirmed in `GlobalExceptionHandler` + `ApiErrorTranslator` (code review).
