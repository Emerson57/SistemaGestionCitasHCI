# Accessibility evaluation

**Standards reference:** WCAG 2.2 principles (Perceivable, Operable, Understandable, Robust)  
**Date:** 2026-10-02

## Automated

| Tool | Target | Result |
|------|--------|--------|
| Lighthouse Accessibility | `/` desktop | **94** |
| Lighthouse Accessibility | `/login` desktop | **100** |
| Lighthouse Accessibility | `/` mobile | **94** |

Raw audits: `../Rendimiento/lighthouse-*.json`

## Manual / code review checklist

| Criterion | Finding | Status |
|-----------|---------|--------|
| Labels on forms | Login/register use `<label for>` + `InputText` | OK |
| Heading hierarchy | One `h1` per page, sections `h2` | OK |
| Landmarks | `header`, `main`, `nav`, skip link | OK |
| Focus visibility | `:focus-visible` outline in design system | OK |
| Color alone | Status badges include text; wizard selection text | OK |
| Dialogs | `role="dialog"`, `aria-modal`, labelled title | Improved (Phase 6) |
| Status messages | `role="alert"` / `role="status"` on errors and empty states | OK |
| Link/button names | Nav links text; menu button has `aria-label` | OK |
| Contrast | Lighthouse login 100; home minor dev console noise | Acceptable |

## WCAG mapping (examples)

- **Perceivable:** Spanish UI text; visible labels; badge text not color-only.
- **Operable:** Keyboard tab order on forms; skip link; dialog Escape (see keyboard doc).
- **Understandable:** Spanish error messages via `ApiErrorTranslator`; validation summaries.
- **Robust:** Semantic HTML in Razor; ARIA only where needed (dialog).

## axe

Not run (no axe CLI in project). Lighthouse used instead per phase instructions.

## Fix applied in Phase 6

- `ConfirmDialog`: `aria-describedby`, initial focus on cancel, **Escape** closes, backdrop click no longer dismisses (reduces accidental cancel).

## Remaining gaps

- Authenticated wizard not run through Lighthouse with session.
- Home static shell `<title>` for SEO/Lighthouse (see Lighthouse summary).
