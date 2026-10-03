# Deployment gate (demonstration / academic)

Use this checklist **before** any controlled demo hosting or production-like deployment. This project remains an **academic prototype**, not clinical production software.

**Prerequisite guide:** [manual-validation-guide.md](manual-validation-guide.md)

---

## Evidence and manual validation

- [ ] **E04–E13** captured in `Pruebas/Evidencias/Screenshots/` (real browser, 1440×900; E14 @ 375×812)
- [ ] **Patient** checklist completed ([Manual-Browser-Signoff.md](../Pruebas/Funcionales/Manual-Browser-Signoff.md) PAT-01–20)
- [ ] **Doctor** checklist completed (DOC-01–12)
- [ ] **Admin** checklist completed (ADM-01–15)
- [ ] **Responsive** matrix completed ([Responsive-Evaluation.md](../Pruebas/Responsive/Responsive-Evaluation.md))
- [ ] **Dialog keyboard** checks completed (sign-off dialog table)
- [ ] **Test Matrix** updated with manual browser results ([Test-Matrix.md](../Pruebas/Test-Matrix.md))
- [ ] **No secrets** in evidence files or screenshots (JWT, passwords, User Secrets)

---

## Automated quality

- [ ] **52** automated tests passing (`dotnet test SistemaGestionCitasHCI.slnx`)
- [ ] **0** build errors (`dotnet build SistemaGestionCitasHCI.slnx`)
- [ ] **0** warnings (release build)

---

## Environment and security

- [ ] **DevSeed disabled** for non-development deployment (`DevSeed:Enabled` false; not Production seeding)
- [ ] Production **JWT**, **connection string**, and **CORS** configured via secure store (not committed)
- [ ] **Participant usability** status documented in [release-readiness.md](release-readiness.md)

---

## Recommendation

**Proceed with demonstration deployment only when** required demonstration items above are complete (especially E04–E13 and PAT/DOC/ADM sign-off).

**Participant usability** may remain **pending** if clearly disclosed in release-readiness and course rules allow — unless your academic instructions explicitly require executed participant sessions.

**Do not** claim production or clinical readiness.
