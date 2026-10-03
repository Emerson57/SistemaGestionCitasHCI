# API response time samples (development)

**Disclaimer:** Development environment / LocalDB / single machine. **Not** production benchmarks.

**API base:** `https://localhost:7211`  
**Date:** 2026-10-02

Samples measured with `curl.exe` + PowerShell `Stopwatch` (TLS, local Kestrel).

## GET /api/specialties

Requires **Authorization** header (controller is `[Authorize]`).

| Run | Time (ms) |
|-----|-----------|
| 1 | 49 |
| 2–5 | See `api-timing-samples.txt` |

Anonymous call returns **401** (expected).

## GET /api/appointments/my (patient Bearer token)

| Run | Time (ms) |
|-----|-----------|
| 1 | 138 |
| 2–3 | See `api-timing-samples.txt` |

## POST /api/auth/login

| Run | Time (ms) |
|-----|-----------|
| 1–3 | See `api-timing-samples.txt` |

## GET /api/doctors?specialtyId={id}

Executed with patient token when specialties exist in dev DB; empty array if no catalog data.

## Evidence

- `api-timing-samples.txt`
- `../Evidencias/functional-api-dev-run.txt`
