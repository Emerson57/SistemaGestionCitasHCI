# Manual validation guide

Step-by-step checklist for **Chrome or Edge** (100% zoom). Full sign-off form: [Pruebas/Funcionales/Manual-Browser-Signoff.md](../Pruebas/Funcionales/Manual-Browser-Signoff.md). Deployment gate: [deployment-gate.md](deployment-gate.md).

**Result codes (when recording):** `PASS` | `FAIL` | `BLOCKED` | `NOT EXECUTED` — only mark `PASS` after you observe the step.

---

## 1. Start API

From repository root:

```bash
dotnet run --project Backend/MedicalAppointments.Api --launch-profile https
```

| Check | Expected |
|-------|----------|
| URL | https://localhost:7211 |
| OpenAPI | https://localhost:7211/openapi/v1.json → **HTTP 200** |
| Console | No startup exception; DevSeed log line if enabled (no passwords printed) |

**Ports in use:** Stop prior `MedicalAppointments.Api` / `dotnet` processes holding **7211** or **5119**, then restart. On Windows: Task Manager → end `MedicalAppointments.Api.exe`, or close the terminal running the API.

---

## 2. Start Web

Second terminal:

```bash
dotnet run --project Frontend/MedicalAppointments.Web --launch-profile https
```

| Check | Expected |
|-------|----------|
| URL | https://localhost:7171 |
| Home | Landing page loads over HTTPS |

**Ports in use:** Stop prior Web process on **7171** / **5171** before restart.

---

## 3. Verify DevSeed

Passwords are **only** in API **User Secrets** (never in repo docs). See [dev-seed.md](dev-seed.md).

1. User Secrets on `Backend/MedicalAppointments.Api`:
   - `DevSeed:Enabled` = `true`
   - `DevSeed:Patient:Email`, `DevSeed:Doctor:Email`, `DevSeed:Admin:Email` (+ matching `*:Password` keys)
2. **Login test (no password in notes):** At https://localhost:7171/login sign in as **patient@dev.local** → should reach `/paciente` after brief redirect via `/auth/complete/…`.
3. **Specialties (after patient login):** Programar cita → step 1 should list seeded names including:
   - Cardiología, Dermatología, Pediatría, Medicina Interna, Neurología
4. **Doctor / admin:** Repeat login with **doctor@dev.local** → `/medico`; **admin@dev.local** → `/admin`.

If login fails: confirm API is running, `DevSeed:Enabled`, and secrets are set (do not paste secrets into chat or screenshots).

---

## 4. Patient flow

Login: **patient@dev.local** (password from User Secrets).

| Step | Action | Expected visible result | Evidence |
|------|--------|-------------------------|----------|
| 1 | Dashboard | “Hola…”, shortcuts, sidebar patient nav | **E04** |
| 2 | **Programar cita** | Wizard step 1 Especialidad | |
| 3 | Select specialty | Card highlighted, “Seleccionada” | **E05** |
| 4 | Continuar → select doctor | Doctor cards, license line | **E06** |
| 5 | Continuar → date/time | Date dropdown + time slots | **E07** |
| 6 | Select slot → **Revisar** | Summary list (specialty, doctor, datetime) | **E08** |
| 7 | **Confirmar cita** | Step 5 success panel persists (not toast-only) | **E09** |
| 8 | **Mis citas** | New appointment in list, status badge | **E10** |
| 9 | **Reprogramar** | Wizard with new slot; list updates | |
| 10 | **Cancelar** → confirm dialog | Appointment cancelled or status updated | |
| 11 | **Historial** | Past/cancelled entries readable | **E11** |
| 12 | **Mi perfil** | Form loads; save allowed field | |
| 13 | **Cerrar sesión** | Home or public; cookie cleared | |
| 14 | Visit `/paciente` | Redirect to login | |

Record each row in Manual-Browser-Signoff **PAT-** table.

---

## 5. Doctor flow

**Preparation (use the app, not raw DB):**

1. Complete patient steps 1–7 (leave one **Scheduled** appointment).
2. Patient logout (step 13).
3. Login **doctor@dev.local**.

| Step | Action | Expected | Evidence |
|------|--------|----------|----------|
| 1 | Dashboard | Doctor home `/medico` | |
| 2 | **Agenda** | Patient appointment visible | **E12** |
| 3 | **Confirmar** | Status → confirmed | |
| 4 | **Completar** | Status → completed | |
| 5 | **Disponibilidad** | Form + existing slots | |
| 6 | **Agregar disponibilidad** | Valid date + HH:mm; success message | |
| 7 | Logout | Session ends | |

Record **DOC-** rows in sign-off form.

---

## 6. Admin flow

Login: **admin@dev.local**. Use disposable names (e.g. `Demo UI …`) for create/edit/deactivate.

| Step | Action | Expected | Evidence |
|------|--------|----------|----------|
| 1 | Dashboard | `/admin` cards | |
| 2 | **Especialidades** | List of active specialties | **E13** |
| 3 | Create specialty | Appears in list | |
| 4 | Edit | Name/description update | |
| 5 | Deactivate (disposable) | Confirm dialog; removed from active list | |
| 6 | **Médicos** | List | |
| 7 | Create / edit / deactivate doctor (demo) | CRUD reflected | |
| 8 | **Pacientes** | List; **Ver detalle** | |
| 9 | Logout | | |

Do **not** deactivate seeded demo patient/admin unless you can re-seed. Record **ADM-** rows.

---

## 7. Dialog keyboard test

For each dialog, open from UI, then test **Tab**, **Shift+Tab**, **Escape**, **Enter** (careful with Enter on confirm).

| Dialog | How to open |
|--------|-------------|
| Cancel appointment | Mis citas → Cancelar |
| Deactivate specialty | Admin especialidades → Desactivar (disposable) |
| Deactivate doctor | Admin médicos → Desactivar (demo) |
| Deactivate patient | Admin pacientes → Desactivar (**disposable patient only**) |

**Expected (if implemented):** focus visible; order Cancel → Confirm; Escape closes; Enter activates focused button; focus returns to control that opened dialog after close.

Record in sign-off **Dialog keyboard** table. Do not mark PASS unless observed.

---

## 8. Responsive test

DevTools → device toolbar. For each width **375, 768, 1280, 1440**, spot-check:

- No horizontal page scroll (except intentional tables)
- Header / **Menú** drawer usable
- Wizard steps readable; buttons not overlapping
- Dialogs fit viewport
- Forms and admin cards readable

**Screens to visit:** Home, Login, Registro, `/paciente`, wizard, Mis citas, Historial, Doctor agenda, Doctor disponibilidad, Admin especialidades, Admin médicos.

Update cells in [Pruebas/Responsive/Responsive-Evaluation.md](../Pruebas/Responsive/Responsive-Evaluation.md) with **Passed** or **Issue Found** (not inferred from CSS).

---

## 9. Screenshot capture

**Folder:** `Pruebas/Evidencias/Screenshots/`

| Context | Size |
|---------|------|
| Desktop E01–E13 | **1440 × 900**, zoom **100%** |
| Mobile E14 | **375 × 812** (home) |

**Files:** E01-home … E14-mobile (see [Evidencias README](../Pruebas/Evidencias/README.md)).

**Do capture:** clean UI, development identities, realistic demo data.  
**Do not capture:** DevTools, terminal, passwords, JWT, User Secrets, real personal data. Hide bookmarks bar if easy.

Public E01, E02, E03, E14 can be refreshed with `Pruebas/scripts/capture-public-screenshots.mjs` (Web must be running). **E04–E13 require authenticated manual capture.**

---

## 10. Final sign-off

1. Complete [Manual-Browser-Signoff.md](../Pruebas/Funcionales/Manual-Browser-Signoff.md) (all PAT/DOC/ADM + dialogs).
2. Update [Test-Matrix.md](../Pruebas/Test-Matrix.md) manual browser totals.
3. Complete [deployment-gate.md](deployment-gate.md) checkboxes.
4. Update [release-readiness.md](release-readiness.md) if status changed.
5. Participant usability: if not run, state **Pending** in release-readiness (do not fabricate).

**Deployment:** Do not deploy until deployment gate required items are checked.
