# Keyboard-only test results

**Method:** Manual review of markup/components + dialog fix verification (no full browser automation in CI).  
**Date:** 2026-10-02

| Area | Tab order | Focus visible | Enter/Space | Escape | Notes |
|------|-----------|---------------|-------------|--------|-------|
| Skip link | First focusable when Tab from load | Yes | Activates link | — | `.skip-link` |
| Public header | Logo area → buttons/links | Yes | — | — | |
| Login form | Email → password → show → submit | Yes | Submit on Enter in field | — | |
| Register form | Fieldset order top-to-bottom | Yes | — | — | Long form scroll |
| Main nav | Vertical `NavLink` list | Yes | Activate link | — | Mobile menu toggle |
| Appointment wizard | Step buttons → cards → actions | Yes | Card buttons activate | — | Cards are `<button>` |
| Confirm dialog | Cancel (initial focus) → Confirm | Yes | Buttons activate | **Closes** | Phase 6 fix |
| Primary actions | Buttons in `.hero-actions` | Yes | — | — | |

## Issues found and addressed

| Issue | Severity | Action |
|-------|----------|--------|
| Dialog closed on backdrop click only | Minor | Removed backdrop dismiss; use Cancel/Escape |
| Dialog no Escape | Minor | Added `@onkeydown` Escape handler |
| Dialog focus not moved into modal | Minor | `FocusAsync()` on cancel when opened |

## Not fully exercised

- Full wizard run keyboard-only in browser (recommended before final academic demo).
- Mobile drawer trap (focus may move to background when open — monitor in manual test).
