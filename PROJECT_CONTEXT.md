\# PROJECT CONTEXT

\# SistemaGestionCitasHCI



\## 1. Project overview



This repository contains the high-fidelity implementation of an academic

Human-Computer Interaction (HCI) project:



\*\*Sistema de Gestión de Citas de Especialidades Médicas centrado en HCI\*\*



The project is part of the course:



\*\*Human-Computer Interaction and Digital Citizenship\*\*



The application is the evolution of previously completed low-fidelity

wireframes and medium-fidelity mockups.



The purpose of this phase is to implement a real, functional,

high-fidelity web prototype while preserving the principles of:



\- Human-Computer Interaction (HCI)

\- User-Centered Design (UCD)

\- ISO 9241-210:2019

\- Usability

\- Accessibility

\- UX

\- UI

\- Interaction Design (IxD)

\- Nielsen usability heuristics

\- Responsive Web Design



This is an academic project, but the implementation must follow

professional software engineering practices.



\---



\# 2. Functional purpose



The system manages appointments with medical specialists.



The three principal roles are:



1\. Patient

2\. Medical Specialist

3\. Administrator



The main business flow for a patient is:



Home

→ Login/Register

→ Patient Dashboard

→ Schedule Appointment

→ Select Specialty

→ Select Doctor

→ Select Date and Available Time

→ Review Appointment

→ Confirm Appointment

→ My Appointments

→ Appointment History



The system must remain focused on appointment management.



It is NOT a complete Electronic Health Record system.



\---



\# 3. Functional scope



\## Patient



A patient must be able to:



\- Register.

\- Log in.

\- Log out.

\- View and update their basic profile.

\- View available medical specialties.

\- View doctors by specialty.

\- View doctor availability.

\- Schedule an appointment.

\- Review appointment details before confirmation.

\- Receive clear confirmation after scheduling.

\- View upcoming appointments.

\- Reschedule an appointment.

\- Cancel an appointment.

\- View appointment history.



Patient information includes:



\- Name

\- Date of birth

\- Address

\- Phone number

\- Sex

\- Disability information when applicable

\- Marital status

\- Email



Only information required by the academic case study should be stored.



\---



\## Medical Specialist



A medical specialist must be able to:



\- Log in.

\- View their own agenda.

\- View appointments assigned to them.

\- View only the minimum patient information necessary for the appointment.

\- Manage their availability when applicable.



Medical specialists must NOT have access to unrelated patient records.



\---



\## Administrator



An administrator must be able to:



\- Log in.

\- Manage medical specialties.

\- Manage medical specialists.

\- View/manage basic system users.

\- Activate/deactivate records when appropriate.

\- Manage operational appointment information when required.



The administrator must not have access to clinical information because

clinical records are outside the scope of this prototype.



\---



\# 4. Original Functional Requirements



The system must preserve the original requirements defined during the

low- and medium-fidelity phases.



\## RF-01

Register new patients.



\## RF-02

Capture patient information:

name, date of birth, address, phone, sex, disability,

marital status and email.



\## RF-03

Authenticate system users.



\## RF-04

Register and manage medical specialists.



\## RF-05

Associate doctors with a medical specialty.



\## RF-06

Display available medical specialties.



\## RF-07

Display doctors according to the selected specialty.



\## RF-08

Display available dates and appointment times.



\## RF-09

Allow a patient to schedule an appointment.



\## RF-10

Provide visible confirmation after scheduling.



\## RF-11

Associate appointments with each patient.



\## RF-12

Allow patients to view their appointment history.



\## RF-13

Allow doctors to view their agenda.



\## RF-14

Provide navigation according to user role.



\---



\# 5. Additional CRUD requirements for High-Fidelity Prototype



The second assignment explicitly requires CRUD operations.



Implement:



\## Patients

\- Create

\- Read

\- Update

\- Deactivate



Avoid hard deletion of operational data when possible.



\## Doctors

\- Create

\- Read

\- Update

\- Deactivate



\## Specialties

\- Create

\- Read

\- Update

\- Deactivate



\## Appointments

\- Create

\- Read

\- Reschedule / Update

\- Cancel



Appointment cancellation should normally be represented as a status

change instead of physically deleting historical information.



\---



\# 6. Appointment business rules



An appointment must belong to:



\- One Patient

\- One Doctor



A Doctor belongs to one Specialty.



A Doctor may have multiple availability slots.



A Patient may have multiple appointments.



A Doctor may have multiple appointments.



The system must prevent two active appointments from occupying

the same doctor/date/time slot.



An appointment should have a controlled status such as:



\- Scheduled

\- Confirmed

\- Completed

\- Cancelled

\- Rescheduled



Relevant appointment status changes should be auditable.



A cancelled appointment must remain available in appointment history.



\---



\# 7. Out of scope



DO NOT implement unless explicitly requested later:



\- Electronic Medical Records

\- Clinical histories

\- Diagnoses

\- Prescriptions

\- Medications

\- Laboratory results

\- Medical documents

\- Billing

\- Payments

\- EPS integrations

\- IPS integrations

\- Insurance integrations

\- Telemedicine

\- WhatsApp integration

\- SMS integration

\- Email notification infrastructure

\- AI medical diagnosis

\- Storage of unnecessary sensitive clinical information



Keep the solution focused on appointment management.



\---



\# 8. Technology stack



Always use modern supported Microsoft technologies.



Current baseline:



\- Visual Studio 2026

\- .NET 10 LTS

\- C# 14

\- ASP.NET Core 10

\- ASP.NET Core Web API

\- Blazor Web App

\- Entity Framework Core 10

\- SQL Server

\- ASP.NET Core Identity

\- JWT Bearer Authentication for API access

\- OpenAPI

\- xUnit

\- Git

\- GitHub



Do not downgrade framework versions unless explicitly requested.



Do not introduce Node.js, React, Angular, MongoDB or another stack

without a justified requirement.



\---



\# 9. Architecture



Use a modular layered architecture.



Repository structure:



SistemaGestionCitasHCI/

│

├── SistemaGestionCitasHCI.slnx

│

├── Backend/

│   ├── MedicalAppointments.Api/

│   ├── MedicalAppointments.Application/

│   ├── MedicalAppointments.Domain/

│   └── MedicalAppointments.Infrastructure/

│

├── Frontend/

│   └── MedicalAppointments.Web/

│

├── Tests/

│   ├── MedicalAppointments.UnitTests/

│   └── MedicalAppointments.IntegrationTests/

│

├── Fase-Diseño/

│

├── Pruebas/

│

├── docs/

│

├── Presentacion/

│

├── README.md

├── .gitignore

└── PROJECT\_CONTEXT.md



\---



\# 10. Responsibilities by project



\## MedicalAppointments.Domain



Type:



.NET Class Library



Responsibilities:



\- Domain entities

\- Enums

\- Value objects when justified

\- Domain rules

\- Domain exceptions



It must NOT depend on:



\- EF Core

\- ASP.NET Core

\- SQL Server

\- Blazor

\- Infrastructure



The Domain project must remain framework-independent wherever practical.



\---



\## MedicalAppointments.Application



Type:



.NET Class Library



Responsibilities:



\- Application use cases

\- DTOs

\- Interfaces / abstractions

\- Application services

\- Validation contracts

\- Business orchestration

\- Mapping when required



Project dependency:



Application

→ Domain



It must not depend directly on Infrastructure.



\---



\## MedicalAppointments.Infrastructure



Type:



.NET Class Library



Responsibilities:



\- Entity Framework Core

\- DbContext

\- Entity configurations

\- Repositories when required

\- ASP.NET Core Identity persistence

\- Database migrations

\- SQL Server persistence

\- External infrastructure implementations



Project dependencies:



Infrastructure

→ Application

→ Domain



Infrastructure may also reference Domain directly where required.



\---



\## MedicalAppointments.Api



Type:



ASP.NET Core Web API



Responsibilities:



\- HTTP API

\- REST endpoints/controllers

\- Authentication

\- Authorization

\- JWT

\- OpenAPI

\- Dependency injection composition

\- Middleware

\- Exception handling

\- HTTP response contracts



Project dependencies:



Api

→ Application

Api

→ Infrastructure



The API must NOT contain core business rules that belong in

Application or Domain.



\---



\## MedicalAppointments.Web



Type:



Blazor Web App



Responsibilities:



\- High-fidelity user interface

\- Responsive UI

\- Accessible components

\- Forms

\- Validation presentation

\- Authentication UI

\- Patient dashboard

\- Doctor dashboard

\- Administrator dashboard

\- API consumption



The Web project must communicate with the Backend using HTTP/API

contracts.



DO NOT access SQL Server directly from the Web project.



DO NOT make the Web project depend on Infrastructure.



Preferred architectural boundary:



Blazor Web

→ HTTPS

→ MedicalAppointments.Api

→ Application

→ Domain / Infrastructure

→ SQL Server



\---



\## MedicalAppointments.UnitTests



Type:



xUnit Test Project



Responsibilities:



\- Domain tests

\- Application tests

\- Business-rule tests

\- Validation tests



Primary references:



UnitTests

→ Domain

UnitTests

→ Application



Do not require a real production database.



\---



\## MedicalAppointments.IntegrationTests



Type:



xUnit Test Project



Responsibilities:



\- API endpoint tests

\- Authentication tests

\- Authorization tests

\- Persistence integration tests

\- Complete request/response workflows



Primary reference:



IntegrationTests

→ Api



Use WebApplicationFactory where appropriate.



Use an isolated test database/environment.



\---



\# 11. Project references



Create these references:



MedicalAppointments.Application

→ MedicalAppointments.Domain



MedicalAppointments.Infrastructure

→ MedicalAppointments.Domain



MedicalAppointments.Infrastructure

→ MedicalAppointments.Application



MedicalAppointments.Api

→ MedicalAppointments.Application



MedicalAppointments.Api

→ MedicalAppointments.Infrastructure



MedicalAppointments.UnitTests

→ MedicalAppointments.Domain



MedicalAppointments.UnitTests

→ MedicalAppointments.Application



MedicalAppointments.IntegrationTests

→ MedicalAppointments.Api



Avoid circular dependencies.



MedicalAppointments.Web should preferably communicate through the API

instead of project references to Backend implementation projects.



\---



\# 12. Initial domain model



Prepare the architecture for the following entities:



\## ApplicationUser

Identity user for authentication/authorization.



Do not duplicate password handling inside domain entities.



\## Patient



Suggested properties:



\- PatientId : Guid

\- UserId

\- FullName

\- BirthDate

\- Address

\- PhoneNumber

\- Sex

\- Disability

\- MaritalStatus

\- Email / identity relationship as appropriate

\- IsActive

\- CreatedAt

\- UpdatedAt



\## Doctor



Suggested properties:



\- DoctorId : Guid

\- UserId

\- SpecialtyId

\- FullName

\- ProfessionalLicense

\- IsActive

\- CreatedAt

\- UpdatedAt



\## Specialty



Suggested properties:



\- SpecialtyId

\- Name

\- Description

\- IsActive



\## DoctorAvailability



Suggested properties:



\- DoctorAvailabilityId : Guid

\- DoctorId

\- Date

\- StartTime

\- EndTime

\- Status



\## Appointment



Suggested properties:



\- AppointmentId : Guid

\- PatientId

\- DoctorId

\- AppointmentDateTime

\- Status

\- Reason

\- CreatedAt

\- UpdatedAt



\## AppointmentStatusHistory



Suggested properties:



\- AppointmentStatusHistoryId : Guid

\- AppointmentId

\- PreviousStatus

\- NewStatus

\- ChangedAt

\- ChangedBy



Use proper navigation properties and database constraints.



Do not over-engineer entities before their actual use cases are

implemented.



\---



\# 13. IDs and dates



Prefer:



Guid

for business entities such as:



\- Patient

\- Doctor

\- Appointment

\- Availability

\- History



Simple catalog IDs such as Specialty may use int.



For timestamps prefer UTC internally:



DateTimeOffset / UTC where appropriate.



The UI may convert values to the user's local context.



\---



\# 14. Authentication and authorization



Use:



ASP.NET Core Identity



Roles:



\- Patient

\- Doctor

\- Administrator



Use JWT Bearer authentication for protected API endpoints.



Rules:



\- Never store plain-text passwords.

\- Never create custom password hashing.

\- Never trust role information coming only from the frontend.

\- Authorization must also be enforced by the API.

\- Verify ownership of patient resources.

\- A patient cannot access another patient's appointments.

\- A doctor cannot access another doctor's agenda.

\- Administrator operations require Administrator role.



\---



\# 15. Security requirements



Never commit:



\- Passwords

\- JWT signing secrets

\- Connection strings containing credentials

\- API keys

\- Tokens

\- Production secrets



Use development configuration mechanisms such as User Secrets.



Use environment variables / secure configuration for deployment.



Use HTTPS.



Validate all external input.



Use parameterized database access through EF Core.



Return safe API error responses.



Do not expose stack traces to end users.



Implement global exception handling.



Use authorization policies/roles where appropriate.



\---



\# 16. Data deletion policy



For operational/master data prefer soft deletion:



IsActive = false



This applies especially to:



\- Doctors

\- Specialties

\- Users



For appointments use status transitions such as Cancelled instead

of destroying the record.



Preserve appointment history for auditability.



\---



\# 17. API design principles



Use RESTful conventions.



Examples:



POST   /api/auth/register

POST   /api/auth/login



GET    /api/specialties

GET    /api/specialties/{id}

POST   /api/specialties

PUT    /api/specialties/{id}

DELETE /api/specialties/{id}



GET    /api/doctors

GET    /api/doctors/{id}

GET    /api/doctors?specialtyId={id}

POST   /api/doctors

PUT    /api/doctors/{id}

DELETE /api/doctors/{id}



GET    /api/doctors/{id}/availability



POST   /api/appointments

GET    /api/appointments/my

GET    /api/appointments/{id}

PUT    /api/appointments/{id}/reschedule

DELETE /api/appointments/{id}



GET    /api/appointments/history



GET    /api/doctors/me/agenda



Use appropriate:



\- 200 OK

\- 201 Created

\- 204 No Content

\- 400 Bad Request

\- 401 Unauthorized

\- 403 Forbidden

\- 404 Not Found

\- 409 Conflict



Use ProblemDetails for API errors where appropriate.



\---



\# 18. HCI requirements



The application must not be treated as only a technical CRUD.



Every interface must follow HCI principles.



Important requirements:



\## Visibility of system status



Always provide feedback for:



\- Loading

\- Saving

\- Successful actions

\- Errors

\- Appointment confirmation

\- Cancellation

\- Session state



\## Match between system and real world



Use familiar medical appointment terminology.



Avoid unnecessary technical language in UI.



\## User control and freedom



Allow:



\- Back navigation

\- Cancellation before confirmation

\- Appointment cancellation when business rules allow it

\- Reprogramming



\## Consistency



Buttons, labels, forms and navigation should behave consistently.



\## Error prevention



Prevent:



\- Invalid dates

\- Missing required fields

\- Duplicate appointment slots

\- Invalid email

\- Invalid authentication actions

\- Unauthorized access



\## Recognition rather than recall



Display available:



\- Specialties

\- Doctors

\- Dates

\- Times



Do not require users to remember codes.



\## Minimalist design



Avoid unnecessary elements and information.



\## Error recovery



Error messages must explain:



1\. What happened

2\. Why when useful

3\. What the user can do next



\---



\# 19. Accessibility



Target WCAG 2.2 principles where practical.



Requirements include:



\- Semantic HTML

\- Keyboard navigation

\- Visible focus

\- Proper labels

\- Accessible validation messages

\- Alternative text for meaningful images

\- Sufficient contrast

\- No information conveyed only by color

\- Logical heading hierarchy

\- Responsive layout

\- Adequate touch target sizes



Accessibility must be considered during implementation, not added only

at the end.



\---



\# 20. Responsive design



The frontend must support at least:



\- Desktop

\- Tablet

\- Mobile



Use a mobile-first responsive approach where practical.



Do not create separate applications for desktop and mobile.



\---



\# 21. User interface states



Every data-driven page should consider:



\- Initial

\- Loading

\- Success

\- Empty

\- Validation Error

\- API Error

\- Unauthorized

\- Forbidden



Avoid blank screens.



\---



\# 22. Testing strategy



Use xUnit.



Minimum automated testing areas:



\## Unit tests



\- Appointment scheduling rules

\- Appointment conflict prevention

\- Rescheduling rules

\- Cancellation rules

\- Application validations



\## Integration tests



\- Registration

\- Login

\- Authorization

\- Specialty endpoints

\- Doctor endpoints

\- Appointment endpoints

\- Appointment ownership

\- Doctor agenda

\- Administrator authorization



Manual/UI evaluation will later include:



\- Usability

\- Accessibility

\- Responsive design

\- Navigation

\- Performance



\---



\# 23. Code quality rules



Use:



\- Nullable reference types enabled

\- Implicit usings where appropriate

\- Async/await for I/O

\- CancellationToken in application/API async operations when appropriate

\- Dependency Injection

\- Options pattern for configuration

\- Centralized exception handling

\- Structured logging

\- Strong typing

\- DTOs for API boundaries



Avoid:



\- Business logic inside controllers

\- Giant services/classes

\- Static global state

\- Magic strings

\- Duplicate code

\- Direct DbContext use in UI

\- Returning EF entities directly as public API contracts

\- Premature abstraction

\- Unnecessary design patterns



Use clear English names for source-code identifiers.



UI text may be Spanish because the academic application targets

Spanish-speaking users.



\---



\# 24. Development principles



Implement incrementally.



Do NOT try to implement the whole system in one operation.



Every phase should leave the solution:



\- Compiling

\- Runnable

\- Testable

\- Commit-ready



Before introducing a dependency:



1\. Explain why it is required.

2\. Prefer official Microsoft/.NET packages.

3\. Use versions compatible with .NET 10.

4\. Avoid unnecessary packages.



Do not change architecture without explaining the impact first.



\---



\# 25. Git conventions



Repository uses Git and GitHub.



Main stable branch:



main



Development may use:



develop



Feature branches should follow:



feature/<feature-name>



Examples:



feature/initial-architecture

feature/authentication

feature/patients

feature/doctors

feature/appointments



Commit examples:



chore: initialize solution architecture

feat: add appointment domain model

feat: implement patient registration

feat: implement appointment scheduling

test: add appointment integration tests

docs: add software architecture



Never commit secrets or generated build artifacts.



\---



\# 26. First task for Cursor



DO NOT implement business functionality yet.



The first task is ONLY to initialize the solution architecture.



The repository already contains:



SistemaGestionCitasHCI.slnx



Create these directories and projects:



Backend/

├── MedicalAppointments.Api

├── MedicalAppointments.Application

├── MedicalAppointments.Domain

└── MedicalAppointments.Infrastructure



Frontend/

└── MedicalAppointments.Web



Tests/

├── MedicalAppointments.UnitTests

└── MedicalAppointments.IntegrationTests



Use .NET 10.



Project types:



MedicalAppointments.Domain

= .NET Class Library



MedicalAppointments.Application

= .NET Class Library



MedicalAppointments.Infrastructure

= .NET Class Library



MedicalAppointments.Api

= ASP.NET Core Web API



MedicalAppointments.Web

= Blazor Web App



MedicalAppointments.UnitTests

= xUnit Test Project



MedicalAppointments.IntegrationTests

= xUnit Test Project



Add all projects to:



SistemaGestionCitasHCI.slnx



Configure ONLY the project references defined in this document.



Do not install application libraries yet except packages strictly

required by the selected project templates.



Do not create database entities yet.



Do not create authentication yet.



Do not create CRUD yet.



Do not create migrations yet.



Do not change the repository architecture.



After initialization:



1\. Restore packages.

2\. Build the complete solution.

3\. Fix any compiler errors.

4\. Confirm that build succeeds with zero errors.

5\. Show the final project tree.

6\. Explain all project references created.

7\. Report any warnings.

8\. Do not proceed to another development phase until explicitly instructed.

