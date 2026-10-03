# Lighthouse summary

**Tool:** Lighthouse 12.6.0 (CLI via `npx`), Chrome headless, `--ignore-certificate-errors`  
**Environment:** Development Blazor Web `https://localhost:7171`  
**Date:** 2026-10-02

| Page | Viewport | Performance | Accessibility | Best Practices | SEO | Raw JSON |
|------|----------|-------------|---------------|----------------|-----|----------|
| `/` | Desktop preset | 100 | 94 | 96 | 80 | `lighthouse-home.json` |
| `/login` | Desktop preset | 100 | 100 | 96 | 90 | `lighthouse-login.json` |
| `/` | Mobile emulation | 96 | 94 | — | — | `lighthouse-home-mobile.json` |

## Major findings (not all require fixes for academic prototype)

- **SEO / document title on `/`:** Initial static HTML may lack `<title>` until Blazor `PageTitle` renders; Lighthouse flags `document-title` on home. Login page scores 100 accessibility (labels, contrast).
- **Best practices:** Console errors (Blazor dev / WebSocket) may appear in Lighthouse runs against Interactive Server.
- **Performance:** Dev build, no compression — `uses-text-compression`, `unminified-css` expected in Development.

## Authenticated pages

`/paciente` and `/paciente/citas/nueva` were **not** audited with Lighthouse CLI (JWT session required). Recommend manual Lighthouse in Chrome with logged-in patient before final report.

## Retest

No retest after changes required for scores; optional after adding static `<title>` in `App.razor` if SEO improvement desired.
