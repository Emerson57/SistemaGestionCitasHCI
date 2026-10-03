# Frontend — MedicalAppointments.Web

Blazor Web App (.NET 10) with **Interactive Server** rendering. UI copy is Spanish; code identifiers are English. The Web project does not reference Backend Domain, Infrastructure, or Application — it acts as an external API client.

## Architecture

```
MedicalAppointments.Web/
├── Auth/                    # AppRoles constants
├── Components/
│   ├── Appointments/        # Guided scheduling wizard
│   ├── Common/              # Loading, empty/error states, dialog, toast
│   ├── Layout/              # MainLayout, PublicLayout
│   ├── Navigation/          # RoleNav
│   └── Pages/               # Public, Auth, Patient, Doctor, Admin, Errors
├── Extensions/              # DI, API HttpClient registration
├── Helpers/                 # Display labels, slot helpers
├── Models/                  # API request/response mirrors
├── Services/
│   ├── Api/                 # Typed HttpClient services
│   ├── Authentication/      # JWT state, token storage, handlers
│   └── State/               # NotificationService
└── wwwroot/css/             # design-system.css + app.css
```

## Configuration

`appsettings.json`:

```json
"Api": {
  "BaseUrl": "https://localhost:7211"
}
```

Run **API first**, then Web. Default Web URLs: `https://localhost:7171` / `http://localhost:5171`.

## Routes

| Route | Role | Purpose |
|-------|------|---------|
| `/` | Public | Landing |
| `/login`, `/registro` | Public | Auth |
| `/paciente` | Patient | Dashboard |
| `/paciente/citas/nueva` | Patient | 5-step scheduling wizard |
| `/paciente/citas` | Patient | Upcoming appointments |
| `/paciente/citas/{id}/reprogramar` | Patient | Reschedule (reuses wizard) |
| `/paciente/historial` | Patient | Past/cancelled/completed |
| `/paciente/perfil` | Patient | Profile edit |
| `/medico`, `/medico/agenda`, `/medico/disponibilidad` | Doctor | Agenda & availability |
| `/admin`, `/admin/especialidades`, `/admin/medicos`, `/admin/pacientes` | Administrator | CRUD shortcuts |
| 403 / 404 | — | Forbidden / Not found |

## API client

- **Services:** `IAuthApiService`, `IPatientApiService`, `ISpecialtyApiService`, `IDoctorApiService`, `IAppointmentApiService`, `IAvailabilityApiService`
- **Base:** `ApiClientBase` — JSON, success checks, `ApiException` + `ApiProblemDetails`
- **Errors:** `ApiErrorTranslator` maps status codes to Spanish UX messages
- **Handlers:** `BearerTokenHandler` (attach JWT to API origin only), `SessionExpiredHandler` (401 → sign out → `/login?expired=1`)

## Authentication

Blazor **Interactive Server** uses two coordinated mechanisms:

1. **JWT in protected session storage** — API calls via `BearerTokenHandler`.
2. **HttpOnly cookie** — satisfies ASP.NET Core authorization on SSR and full-page navigations to `[Authorize]` routes.

### Login / register flow

```
User submits Login (Interactive) → API POST /api/auth/login
  → JwtAuthenticationStateProvider.SignInAsync (session storage + claims)
  → SignInTicketStore.CreateTicket (AuthenticationResponse in IMemoryCache, 2 min TTL)
  → Browser navigates (forceLoad) to GET /auth/complete/{ticketId}
  → ConsumeTicket (one-time: removed from cache on use)
  → SignInAsync cookie (non-persistent session cookie)
  → Redirect to role home or safe local returnUrl
```

- **Ticket:** Random 32-char hex id; **not logged**; **no password** in ticket payload (post-login user profile + access token reference only in server memory). Replaying `/auth/complete/{id}` after consume → redirect `/login?expired=1`.
- **returnUrl:** Only local paths allowed (`/…`, rejects `//`, `\`, `@`).
- **Logout:** Client clears session storage → `GET /auth/sign-out` clears cookie → `/`.

### Cookie options (Development / HTTPS)

| Option | Value | Rationale |
|--------|-------|-----------|
| Name | `MedicalAppointments.Auth` | App-scoped |
| HttpOnly | true | Not readable from JS |
| SecurePolicy | Always | HTTPS launch profiles |
| SameSite | Lax | Same-site navigations + OAuth-safe default |
| IsPersistent | false on sign-in | Session cookie semantics at login completion |

Sliding expiration uses framework cookie defaults unless overridden in `AddCookie` (not shortened for automation).

### Components

- `SignInTicketStore` — singleton + `IMemoryCache`
- `AuthEndpointExtensions.MapAuthEndpoints` — `/auth/complete/{ticketId}`, `/auth/sign-out`
- `HeaderActions` — interactive logout; `MainLayout` without `@rendermode` (avoids SSR/Body render-mode conflict)

Manual sign-off: [Pruebas/Funcionales/Manual-Browser-Signoff.md](../Pruebas/Funcionales/Manual-Browser-Signoff.md).

- **Roles:** From token claims (`Patient`, `Doctor`, `Administrator`); backend remains authoritative

## Design system

CSS custom properties in `wwwroot/css/design-system.css`: `--primary`, `--primary-hover`, `--primary-soft`, semantic colors, text/background/surface tokens, shadows, radius, spacing. Shared patterns: `.btn` (primary/secondary/danger/ghost), `.card`, `.form-field`, `.status-badge`, wizard stepper, `.page-container`, responsive layout in `app.css`.

See **[design-traceability.md](design-traceability.md)** for screen-by-screen mapping to the wireframe/mockup flow.

## High-Fidelity Visual Refinement (Phase 5.1)

- **Visual direction:** Calm healthcare UI — teal primary, white surfaces, minimal decoration, no admin-template chrome.
- **Typography:** `.page-title`, `.section-title`, `.card-title`, `.page-lead`, `.meta-text`, `.helper-text` with readable sizes and line-height.
- **Layout:** Max-width content (720px forms, 960px lists); public landing up to 1100px; fixed header + sidebar on desktop, drawer on mobile.
- **Appointment wizard:** Numbered stepper (1–4) + persistent step 5 confirmation; selection labels (“Seleccionada/o”); review via definition list.
- **Components:** `AppointmentStatusBadge`, shared `ConfirmDialog` for cancel/deactivate, compact `LoadingIndicator`, consistent `EmptyState`/`ErrorState`.
- **Responsive:** Breakpoints at ~900px (nav) and 768px (grids); wizard steps wrap on narrow viewports.
- **Accessibility:** Skip link, focus rings, `aria-current` on wizard step, badges with text + dot (not color-only), dialog roles.
- **Traceability:** Documented per screen in `design-traceability.md`; IA and patient flow unchanged from PROJECT_CONTEXT §2.

## HCI / Nielsen examples

1. **Visibility of system status** — Wizard step indicator; loading/empty/error components; login “Iniciando sesión…”
2. **Match system ↔ real world** — Spanish clinical-adjacent labels (cita, especialidad, médico); status labels via `DisplayLabels`
3. **User control & freedom** — Back buttons on wizard steps; cancel appointment confirmation dialog
4. **Consistency** — Shared layout, buttons, forms across Patient/Doctor/Admin
5. **Error prevention** — Client validation on register/login; unavailable slots not selectable; confirm before cancel/deactivate
6. **Recognition over recall** — Review step shows full summary before POST; appointment list shows doctor/specialty/date
7. **Flexibility** — Dashboard shortcuts to common tasks
8. **Minimalist design** — No metric-heavy dashboards; focused cards and lists
9. **Recover from errors** — `ErrorState` with retry; ProblemDetails translated to plain Spanish
10. **Help** — Helper text on password/register where useful; empty states explain next action

## Accessibility

- Semantic landmarks (`header`, `main`, `nav`)
- Skip link to main content
- Visible labels (not placeholder-only)
- Focus styles via `--focus-ring`
- `role="alert"` / `role="status"` on errors and confirmations
- Confirmation dialog with titled actions
- Selection states use text/border patterns, not color alone (wizard steps, slot buttons)

## Appointment wizard (HCI core)

Steps: Especialidad → Médico → Fecha y horario → Revisión → Confirmación (persistent success screen, not toast-only). Reschedule reuses the same component with prior appointment context and explicit old/new comparison on review.

## Manual smoke checklist

With API + Web running: home, register, login, patient dashboard, schedule/cancel/reschedule, doctor pages (doctor account), admin pages (requires `DevAdmin:*` user secrets), logout.

## Screenshots

_(Placeholder — add captures of home, wizard, and dashboards for documentation.)_
