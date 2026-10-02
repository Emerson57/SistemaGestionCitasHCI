using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository(ApplicationDbContext context) : IAppointmentRepository
{
    private static readonly AppointmentStatus[] ActiveStatuses =
    [
        AppointmentStatus.Scheduled,
        AppointmentStatus.Confirmed
    ];

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Appointments
            .Include(a => a.StatusHistory)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken) =>
        await context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.Specialty)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetHistoryByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken) =>
        await context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.Specialty)
            .Where(a => a.PatientId == patientId
                && (a.Status == AppointmentStatus.Completed || a.Status == AppointmentStatus.Cancelled))
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken) =>
        await context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.Specialty)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsActiveAppointmentForSlotAsync(
        Guid doctorId,
        DateTimeOffset appointmentDateTime,
        Guid? excludeAppointmentId,
        CancellationToken cancellationToken) =>
        await context.Appointments
            .AsNoTracking()
            .AnyAsync(
                a => a.DoctorId == doctorId
                    && a.AppointmentDateTime == appointmentDateTime
                    && ActiveStatuses.Contains(a.Status)
                    && (excludeAppointmentId == null || a.Id != excludeAppointmentId),
                cancellationToken);

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken) =>
        await context.Appointments.AddAsync(appointment, cancellationToken);

    public void Update(Appointment appointment) =>
        context.Appointments.UpdateIfDetached(appointment);
}
