# Manual browser sign-off form

**Quick guide:** [docs/manual-validation-guide.md](../../docs/manual-validation-guide.md)  
**Deployment gate:** [docs/deployment-gate.md](../../docs/deployment-gate.md)

Execute in **Chrome or Edge**, zoom **100%**. Desktop screenshots **1440×900** → `Pruebas/Evidencias/Screenshots/`.

## How to fill this form

| Column | Use |
|--------|-----|
| **ID** | Stable test id (PAT / DOC / ADM) |
| **Test** | What you did |
| **Result** | One of: **PASS** · **FAIL** · **BLOCKED** · **NOT EXECUTED** |
| **Observation** | What you saw (dates, errors, cleanup) |
| **Evidence** | Screenshot id (E04…) or — |

**Do not prefill PASS.** Use **NOT EXECUTED** until you run the step; **BLOCKED** if environment/data prevents the step.

---

## Environment (check once)

| Item | Result | Observation |
|------|--------|-------------|
| API https://localhost:7211 / OpenAPI 200 | | |
| Web https://localhost:7171 | | |
| DevSeed active (patient login works) | | |

---

## Patient

| ID | Test | Result | Observation | Evidence |
|----|------|--------|-------------|----------|
| PAT-01 | Login as patient@dev.local | | | |
| PAT-02 | Dashboard visible (`/paciente`) | | | E04 |
| PAT-03 | Open Programar cita | | | |
| PAT-04 | Select specialty | | | E05 |
| PAT-05 | Select doctor | | | E06 |
| PAT-06 | Select date/time slot | | | E07 |
| PAT-07 | Review step | | | E08 |
| PAT-08 | Confirm appointment | | | |
| PAT-09 | Persistent confirmation (step 5) | | | E09 |
| PAT-10 | Mis citas list | | | E10 |
| PAT-11 | Reschedule appointment | | | |
| PAT-12 | Verify updated date/time in list | | | |
| PAT-13 | Cancel appointment | | | |
| PAT-14 | Cancel dialog (mouse + keyboard) | | | |
| PAT-15 | Historial | | | E11 |
| PAT-16 | Mi perfil opens | | | |
| PAT-17 | Update profile field and save | | | |
| PAT-18 | Refresh while authenticated | | | |
| PAT-19 | Logout | | | |
| PAT-20 | `/paciente` after logout → login | | | |

---

## Doctor

**Prepare:** Patient schedules appointment → patient logout → doctor login.

| ID | Test | Result | Observation | Evidence |
|----|------|--------|-------------|----------|
| DOC-01 | Login as doctor@dev.local | | | |
| DOC-02 | Doctor dashboard | | | |
| DOC-03 | Open Agenda | | | E12 |
| DOC-04 | Scheduled appointment visible | | | |
| DOC-05 | Confirm appointment | | | |
| DOC-06 | Confirmed status visible | | | |
| DOC-07 | Complete appointment | | | |
| DOC-08 | Completed status visible | | | |
| DOC-09 | Open Disponibilidad | | | |
| DOC-10 | Create availability | | | |
| DOC-11 | New availability visible | | | |
| DOC-12 | Logout | | | |

---

## Administrator

Use disposable demo names for create/edit/deactivate. Do not deactivate seeded DevSeed users unless you can re-seed.

| ID | Test | Result | Observation | Evidence |
|----|------|--------|-------------|----------|
| ADM-01 | Login as admin@dev.local | | | |
| ADM-02 | Admin dashboard | | | |
| ADM-03 | List specialties | | | E13 |
| ADM-04 | Create specialty | | | |
| ADM-05 | Edit specialty | | | |
| ADM-06 | Deactivate specialty (disposable) | | | |
| ADM-07 | Deactivate confirmation dialog | | | |
| ADM-08 | List doctors | | | |
| ADM-09 | Create doctor | | | |
| ADM-10 | Edit doctor | | | |
| ADM-11 | Deactivate doctor (demo) | | | |
| ADM-12 | List patients | | | |
| ADM-13 | View patient detail | | | |
| ADM-14 | Deactivate disposable patient only | | | |
| ADM-15 | Logout | | | |

---

## Dialog keyboard

Test **Tab**, **Shift+Tab**, **Escape**, **Enter** on each. Mark **Result** only if fully observed.

| ID | Dialog | Initial focus OK | Tab order OK | Escape closes | Enter behavior OK | Focus return OK | Result |
|----|--------|------------------|--------------|---------------|-------------------|-----------------|--------|
| DIA-01 | Cancel appointment | | | | | | |
| DIA-02 | Deactivate specialty | | | | | | |
| DIA-03 | Deactivate doctor | | | | | | |
| DIA-04 | Deactivate patient | | | | | | |

---

## Sign-off

| Tester | Date | PAT critical OK? | DOC critical OK? | ADM critical OK? |
|--------|------|------------------|-------------------|------------------|
| | | | | |

**Critical paths:** PAT-01–10 + E04–E10; DOC-01–05 + E12; ADM-01–04 + E13; E01–E03, E14 public.
