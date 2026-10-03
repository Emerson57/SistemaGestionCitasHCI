# UX issue log

| ID | Screen | Finding | Severity | HCI principle | Action | Status |
|----|--------|---------|----------|---------------|--------|--------|
| UX-001 | Confirm dialog | Backdrop click closed dialog without explicit choice | 2 | Error prevention | Remove backdrop dismiss; focus + Escape | **Fixed** Phase 6 |
| UX-002 | Home (Lighthouse) | Missing static `<title>` / meta description in initial HTML | 1 | Robust | Added default title + meta in `App.razor` | **Fixed** Phase 6 |
| UX-003 | Admin UI (dev) | DevSeed required for local admin UI | 2 | — | `DevSeed:Enabled` + User Secrets | **Resolved** Phase 7 |
| UX-005 | Playwright + Blazor | Headless automation unstable for Interactive Server shell after login | 2 | Visibility | Manual sign-off checklist; public-only capture script | **Open** (accepted) |
| UX-007 | Auth returnUrl | Open redirect via `//host` rejected in `/auth/complete` | 2 | Security | `IsSafeLocalReturnUrl` | **Fixed** Phase 7.2 |
| UX-008 | ConfirmDialog | Focus not restored to trigger after close | 2 | A11y | `confirm-dialog-focus.js` | **Fixed** Phase 7.2 (verify manually) |
| UX-009 | Login | Valid credentials did not redirect; submit appeared to do nothing | 4 | Visibility / error recovery / user control | Removed interactive `FormName` (static form POST bypassed `OnValidSubmit`); cookie principal merged in `JwtAuthenticationStateProvider` after `/auth/complete`; safe `returnUrl` normalization; do not swallow `NavigationException` | **Fixed** (verify in browser) |
| UX-010 | Patient dashboard / protected pages | Authenticated UI with false “session expired” after login, refresh, or navigation | 4 | Visibility / consistency / error recovery | HttpOnly API session cookie synced at `/auth/complete`; composite token storage; session-expired only when Bearer was sent; defer protected API loads until token ready | **Fixed** (verify in browser) |
| UX-011 | Patient registration | UI reported registration failure although user/patient were persisted | 4 | Visibility / error prevention / consistency | API session cookie write no longer throws on Blazor circuit; do not swallow `NavigationException`; separate account-created vs registration-failed UX; duplicate-email message | **Fixed** (verify in browser) |
| UX-006 | Protected routes (Web) | `[Authorize]` pages threw Developer Exception (`IAuthenticationService` missing); JWT-only session did not satisfy SSR | 1 | Error prevention | Cookie auth + `/auth/complete/{ticket}`; remove `@rendermode` from `MainLayout` | **Fixed** Phase 7.1 |
| UX-004 | Usability | No participant sessions yet | — | — | Execute `Usability-Test-Plan.md` | Pending |

## Before / after UX-001

- **Before:** Click outside dialog closed it (mouse only).
- **After:** User must choose Cancel, Confirm, or Escape.
