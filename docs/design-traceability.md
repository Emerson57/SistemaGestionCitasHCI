# Design traceability — Wireframe / mockup → high-fidelity Blazor

This document maps the **academic patient flow and screens** described in `PROJECT_CONTEXT.md` (evolution from low-fidelity wireframe and medium-fidelity mockup) to the current **MedicalAppointments.Web** implementation after Phase 5.1 visual refinement.

The repository’s `Fase-Diseño/` folder is reserved for design artifacts; traceability here is based on **documented flows, RF requirements, and the implemented UI**, not on invented deliverables.

Legend:

- **Preserved** — Same purpose, structure, or sequence as the prior design phase.
- **Improved** — Visual/UX polish without changing information architecture.

---

## 1. Home

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | Entry point; explain system; CTAs to schedule and login (PROJECT_CONTEXT §2). |
| **Route** | `/` |
| **Preserved** | System name, short description, primary CTA “Agendar una cita”, secondary “Iniciar sesión”, four benefit blocks (specialties, doctor, easy scheduling, my appointments). |
| **Improved** | Healthcare-oriented cards with numbered benefit icons; consistent typography (`.page-lead`); calm spacing; public header unchanged in role. |
| **HCI** | Recognition over recall (benefits list); minimalist hero (no marketing clutter). |

---

## 2. Login

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-03 authentication; gateway to role dashboards. |
| **Route** | `/login` |
| **Preserved** | Email, password, submit, link to registration; generic credential error. |
| **Improved** | Narrow focused panel (`.auth-panel`); password show/hide as text button; session-expired alert; registration link in footer. |
| **HCI** | Error recovery; visibility of status (loading label on submit). |

---

## 3. Registration

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-01, RF-02 patient capture. |
| **Route** | `/registro` |
| **Preserved** | All RF-02 fields; policy checkbox; no clinical data. |
| **Improved** | Sections: Datos personales, Datos de contacto, Información complementaria, Credenciales; two-column grid on tablet+; helper text on password/disability. |
| **HCI** | Error prevention (birth date, password match); logical grouping reduces cognitive load. |

---

## 4. Patient dashboard

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | Hub after login; next steps toward scheduling (PROJECT_CONTEXT flow). |
| **Route** | `/paciente` |
| **Preserved** | Greeting, next appointment highlight, actions: schedule, my appointments, history, profile. |
| **Improved** | Task-first layout (no KPI charts); empty state with CTA when no upcoming appointment; status badge on next appointment. |
| **HCI** | Visibility of status; flexibility (shortcuts). |

---

## 5. Specialty selection (wizard step 1)

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-06; first step of scheduling. |
| **Route** | `/paciente/citas/nueva` (step 1) |
| **Preserved** | List/cards of active specialties; name + description; forward only after selection. |
| **Improved** | Stepper with step numbers; “Seleccionada” text + border on card; loading/empty/error states. |
| **HCI** | Recognition rather than recall; status not by color alone. |

---

## 6. Doctor selection (wizard step 2)

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-07 doctors by specialty. |
| **Route** | `/paciente/citas/nueva` (step 2) |
| **Preserved** | Filter by chosen specialty; name, specialty, professional license for trust. |
| **Improved** | Same card selection pattern as specialties; back to step 1; empty state with action. |
| **HCI** | User control (back); no fictional ratings/photos. |

---

## 7. Date / time selection (wizard step 3)

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-08 availability. |
| **Route** | `/paciente/citas/nueva` (step 3) |
| **Preserved** | Date list from API; time slots; optional reason; no fake slots. |
| **Improved** | Touch-friendly time grid; selected slot shows check suffix; disabled past slots; clearer empty copy. |
| **HCI** | Error prevention; adequate touch targets. |

---

## 8. Appointment review (wizard step 4)

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | Explicit review before commit (HCI §18, RF-09). |
| **Route** | `/paciente/citas/nueva` (step 4) |
| **Preserved** | Summary fields; Volver + Confirmar; no POST before confirm. |
| **Improved** | Definition list (`.summary-list`); primary confirm visually dominant; reschedule shows old date. |
| **HCI** | Error prevention; consistency of actions. |

---

## 9. Appointment confirmation (wizard step 5)

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-10 visible confirmation. |
| **Route** | `/paciente/citas/nueva` (after success) |
| **Preserved** | Persistent success screen; summary; Ver mis citas / inicio. |
| **Improved** | Success icon panel (`.success-panel`); structured summary; not toast-only. |
| **HCI** | Visibility of system status. |

---

## 10. My appointments

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | Upcoming appointments; reschedule/cancel. |
| **Route** | `/paciente/citas` |
| **Preserved** | Doctor, specialty, date/time, status, Reprogramar, Cancelar. |
| **Improved** | `AppointmentStatusBadge`; list cards; confirm dialog on cancel; empty copy per spec. |
| **HCI** | User control; destructive confirmation. |

---

## 11. Appointment history

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-12; not clinical history. |
| **Route** | `/paciente/historial` |
| **Preserved** | Completed/cancelled/past; no diagnoses. |
| **Improved** | Clear lead text; badges; empty state “No tienes citas anteriores”. |
| **HCI** | Match system ↔ real world (appointment vs medical record). |

---

## 12. Patient profile

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | View/update permitted patient fields. |
| **Route** | `/paciente/perfil` |
| **Preserved** | Editable profile fields only; no UserId/role in form. |
| **Improved** | Narrow form width; persistent success alert; two-column layout. |
| **HCI** | Visibility of save result. |

---

## 13. Doctor dashboard

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | Doctor entry; agenda and availability. |
| **Route** | `/medico` |
| **Preserved** | Today/upcoming counts; links to agenda and availability. |
| **Improved** | Summary cards instead of raw text lines; same design tokens as patient UI. |
| **HCI** | Consistency across roles. |

---

## 14. Doctor agenda

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-13; own appointments only. |
| **Route** | `/medico/agenda` |
| **Preserved** | Patient name, date/time, status, confirm/complete when allowed. |
| **Improved** | Ordered list cards; badges; optional reason. |
| **HCI** | Minimal patient data exposure. |

---

## 15. Doctor availability

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | Doctor manages own slots. |
| **Route** | `/medico/disponibilidad` |
| **Preserved** | Date, start, end; client-side range checks. |
| **Improved** | Labeled fields, grid layout, explanatory lead. |
| **HCI** | Error prevention on time range. |

---

## 16. Admin dashboard

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | Shortcuts to admin tasks (no heavy analytics). |
| **Route** | `/admin` |
| **Preserved** | Especialidades, Médicos, Pacientes links. |
| **Improved** | Card shortcuts with hover; lead text; same shell as other roles. |
| **HCI** | Minimalist admin entry. |

---

## 17. Admin specialties

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | Specialty CRUD; soft deactivate. |
| **Route** | `/admin/especialidades` |
| **Preserved** | Create/edit/deactivate (not permanent delete language). |
| **Improved** | `ConfirmDialog` on desactivar; admin list layout; empty state. |
| **HCI** | Error prevention on destructive ops. |

---

## 18. Admin doctors

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | RF-04/05 doctor registration. |
| **Route** | `/admin/medicos` |
| **Preserved** | Filter by specialty; create with initial password; deactivate. |
| **Improved** | Confirm on deactivate; form grid; password field not echoed after create. |
| **HCI** | Security (no password display); consistency. |

---

## 19. Admin patients

| Aspect | Detail |
|--------|--------|
| **Previous purpose** | List/view/deactivate; progressive disclosure. |
| **Route** | `/admin/pacientes` |
| **Preserved** | Summary list; expand for address/birth; desactivar. |
| **Improved** | Confirm dialog; reduced table noise (card list). |
| **HCI** | Progressive disclosure of PII. |

---

## Global shell (all authenticated roles)

| Preserved | Side navigation by role (RF-14), header with app name, logout. |
| Improved | Role badge (subtle), active `NavLink` styling, mobile drawer, `.page-container` max width, shared tokens. |

---

## Design evolution summary

```
Wireframe (IA + flow)
    → Mockup (visual hierarchy + Spanish copy)
        → High-fidelity Blazor (functional + refined tokens, a11y, responsive)
```

No new business capabilities were added in Phase 5.1; changes are **visual and documentary** alignment with the academic prototype narrative in `PROJECT_CONTEXT.md`.
