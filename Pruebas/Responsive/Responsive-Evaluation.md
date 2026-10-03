# Responsive evaluation (Phase 7.2)

**Rule:** Do not mark **Passed** from CSS review alone. Each cell requires real browser DevTools at the listed width unless marked **Not Executed**.

**Viewports:** 375 | 768 | 1280 | 1440 (CSS pixels, 100% zoom)

**Status values:** Passed | Issue Found | Not Executed

## Matrix

| Screen | 375 | 768 | 1280 | 1440 | Notes |
|--------|-----|-----|------|------|-------|
| Home | Not Executed | Not Executed | Not Executed | Not Executed | E14 = home @ 375 only (real capture) |
| Login | Not Executed | Not Executed | Not Executed | Not Executed | E02 @ 1440 pending manual confirmation at all widths |
| Registration | Not Executed | Not Executed | Not Executed | Not Executed | E03 @ 1440 pending matrix |
| Patient dashboard | Not Executed | Not Executed | Not Executed | Not Executed | Requires authenticated manual session |
| Appointment wizard | Not Executed | Not Executed | Not Executed | Not Executed | Stepper wrap @ ≤900px — verify in browser |
| My appointments | Not Executed | Not Executed | Not Executed | Not Executed | |
| History | Not Executed | Not Executed | Not Executed | Not Executed | |
| Doctor agenda | Not Executed | Not Executed | Not Executed | Not Executed | |
| Doctor availability | Not Executed | Not Executed | Not Executed | Not Executed | |
| Admin specialties | Not Executed | Not Executed | Not Executed | Not Executed | |
| Admin doctors | Not Executed | Not Executed | Not Executed | Not Executed | |

## Evidence

| Artifact | Viewport | Valid for sign-off? |
|----------|----------|---------------------|
| `E01-home.png` | 1440×900 (automation refresh) | Public home desktop |
| `E14-mobile.png` | 375×812 | Public home mobile |
| E04–E13 | — | **Human capture required** — stale/invalid automation files removed |

## CSS mechanisms (reference only — not Pass evidence)

- `@media (max-width: 900px)`: sidebar drawer
- `@media (min-width: 768px)`: two-column form grids
- `.page-container` max-width 960px
- `.wizard-step` flex wrap

## Procedure

Use [../Funcionales/Manual-Browser-Signoff.md](../Funcionales/Manual-Browser-Signoff.md): for each screen, resize DevTools, note horizontal scroll/clipping, update this matrix.
