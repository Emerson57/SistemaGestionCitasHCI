# Development seed (DevSeed)

**Development only.** `DevSeed:Enabled` defaults to `false`. Production and integration tests never run this seeder.

## Enable

In `Backend/MedicalAppointments.Api` User Secrets (do not commit passwords):

```bash
dotnet user-secrets set "DevSeed:Enabled" "true"
dotnet user-secrets set "DevSeed:Admin:Email" "admin@dev.local"
dotnet user-secrets set "DevSeed:Admin:Password" "<strong-password>"
dotnet user-secrets set "DevSeed:Admin:FullName" "Admin Demo"

dotnet user-secrets set "DevSeed:Doctors:DefaultPassword" "<strong-password>"

dotnet user-secrets set "DevSeed:Patient:Email" "patient@dev.local"
dotnet user-secrets set "DevSeed:Patient:Password" "<strong-password>"
dotnet user-secrets set "DevSeed:Patient:FullName" "Paciente Demo"
```

Legacy keys `DevAdmin:Email`, `DevAdmin:Password`, `DevAdmin:FullName` still work for the **admin** account if `DevSeed:Admin:*` is omitted.

Also required: `Jwt:SigningKey` (see root README).

## Reference data

When enabled, idempotent seed creates specialties: Cardiología, Dermatología, Pediatría, Medicina Interna, Neurología.

## Development doctors (one per specialty)

All development doctors share the password configured in **`DevSeed:Doctors:DefaultPassword`** (User Secrets only — never commit or document the actual value).

On each API start with DevSeed enabled, passwords for these accounts are reset via ASP.NET Core Identity (`GeneratePasswordResetToken` + `ResetPasswordAsync`).

| Specialty | Email | Display name | License |
|-----------|--------|--------------|---------|
| Cardiología | cardiologia@dev.local | Dr. Carlos Mendoza | DEV-CARD-001 |
| Dermatología | dermatologia@dev.local | Dra. Laura Martínez | DEV-DERM-001 |
| Pediatría | pediatria@dev.local | Dra. Andrea Gómez | DEV-PED-001 |
| Medicina Interna | medicinainterna@dev.local | Dr. Daniel Rodríguez | DEV-MI-001 |
| Neurología | neurologia@dev.local | Dr. Felipe Torres | DEV-NEURO-001 |

Each doctor receives **five upcoming weekdays** of availability with four 30-minute slots per day (09:00, 10:00, 14:00, 15:00) when missing.

Legacy single-doctor seed (`DevSeed:Doctor:Email`, e.g. `doctor@dev.local`) is **deactivated** when it is not one of the emails above, so the wizard does not show duplicate doctors per specialty.

If `DevSeed:Doctors:DefaultPassword` is missing, new doctor accounts are not created and password reset is skipped (safe log message only).

## Reset development database (safe local reset)

```bash
dotnet ef database drop --force \
  --project Backend/MedicalAppointments.Infrastructure/MedicalAppointments.Infrastructure.csproj \
  --startup-project Backend/MedicalAppointments.Api/MedicalAppointments.Api.csproj

dotnet ef database update \
  --project Backend/MedicalAppointments.Infrastructure/MedicalAppointments.Infrastructure.csproj \
  --startup-project Backend/MedicalAppointments.Api/MedicalAppointments.Api.csproj
```

Start the API once with `DevSeed:Enabled=true` to recreate users and reference data.

There is **no** production reset endpoint.
