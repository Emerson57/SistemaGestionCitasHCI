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

- **State:** `JwtAuthenticationStateProvider` reads JWT claims after login/register
- **Storage:** `ProtectedSessionTokenStorage` via ASP.NET **Protected Session Storage** (encrypted, tab/session scoped). Tradeoff: survives in-session navigation/reconnect; not for cross-device persistence; passwords never stored
- **Roles:** From token claims only (`Patient`, `Doctor`, `Administrator`); `[Authorize(Roles = "...")]` on pages; backend remains authoritative

## Design system

CSS custom properties in `wwwroot/css/design-system.css`: primary/secondary/success/warning/danger, text, surfaces, spacing, radius, focus ring. Shared patterns: `.btn`, `.card`, `.form-field`, wizard steps, responsive layout in `app.css`.

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
