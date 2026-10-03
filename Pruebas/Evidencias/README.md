# Evidence storage

Real artifacts only. No synthetic edits. No passwords, JWTs, or User Secrets in files or screenshots.

## Logs

| File | Description |
|------|-------------|
| `dotnet-test-full-output.txt` | Solution test run |
| `integration-tests-output.txt` | Integration tests |
| `functional-api-dev-run.txt` | Dev API checks (tokens redacted) |
| `phase7-manual-api-validation.txt` | DevSeed API flows |
| `phase71-browser-results.json` | Phase 7.1 automation smoke (not sign-off) |

## Screenshot mapping (E01–E14)

Save files under `Screenshots/`. Manual capture guide: [docs/manual-validation-guide.md](../../docs/manual-validation-guide.md).

| Evidence ID | Filename | Route / screen | Related test | Purpose |
|-------------|----------|----------------|--------------|---------|
| E01 | E01-home.png | `/` | Public home | Landing / high-fidelity entry |
| E02 | E02-login.png | `/login` | PAT-01 context | Login form, public layout |
| E03 | E03-registration.png | `/registro` | Registration | Patient signup form |
| E04 | E04-patient-dashboard.png | `/paciente` | PAT-02 | Patient dashboard after auth |
| E05 | E05-specialty-selection.png | `/paciente/citas/nueva` step 1 | PAT-04 | Wizard specialty selection |
| E06 | E06-doctor-selection.png | Wizard step 2 | PAT-05 | Doctor selection |
| E07 | E07-date-time.png | Wizard step 3 | PAT-06 | Date and time slots |
| E08 | E08-review.png | Wizard step 4 | PAT-07 | Review before confirm |
| E09 | E09-confirmation.png | Wizard step 5 | PAT-08, PAT-09 | Persistent success confirmation |
| E10 | E10-my-appointments.png | `/paciente/citas` | PAT-10 | My appointments list |
| E11 | E11-history.png | `/paciente/historial` | PAT-15 | Appointment history |
| E12 | E12-doctor-agenda.png | `/medico/agenda` | DOC-03, DOC-04 | Doctor agenda |
| E13 | E13-admin-specialties.png | `/admin/especialidades` | ADM-03 | Admin specialties management |
| E14 | E14-mobile.png | `/` @ 375×812 | Responsive home | Mobile layout sample |

### Capture status (update after manual session)

| ID | Status |
|----|--------|
| E01, E02, E03, E14 | Available (public script or manual) |
| E04–E13 | **Pending** human capture @ 1440×900 unless file verified visually |

Refresh public PNGs: `Pruebas/scripts/capture-public-screenshots.mjs` (Web running).

## Manual sign-off

[Manual-Browser-Signoff.md](../Funcionales/Manual-Browser-Signoff.md) · [deployment-gate.md](../../docs/deployment-gate.md)

## Lighthouse

`../Rendimiento/lighthouse-*.json`
