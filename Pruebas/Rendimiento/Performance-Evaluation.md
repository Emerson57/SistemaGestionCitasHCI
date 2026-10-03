# Frontend performance evaluation

## Scope

Review of **MedicalAppointments.Web** for obvious inefficiencies. No production profiling.

## Findings

| Area | Assessment |
|------|------------|
| Large assets | No heavy image libraries; CSS limited to `design-system.css` + `app.css` |
| JavaScript | Blazor framework only; no extra SPA bundles |
| API calls | Typed services; no duplicate polling observed in page code |
| HttpClient | Single configured pipeline per service (handlers registered once) |
| Re-renders | Interactive Server; wizard loads data per step (acceptable) |
| Lighthouse | Home desktop Performance **100**, mobile **96** (dev build) |

## Development-only overhead

- Interactive Server SignalR circuit
- Unminified CSS/JS in Debug
- No response compression in default dev profile

## Actions taken

None required for academic prototype beyond documentation.

## Optional future (post-deployment)

- Enable compression in hosting
- Publish trim/AOT only if measured benefit
