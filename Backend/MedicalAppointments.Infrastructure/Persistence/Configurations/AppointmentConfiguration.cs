using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.PatientId)
            .IsRequired();

        builder.Property(a => a.DoctorId)
            .IsRequired();

        builder.Property(a => a.AppointmentDateTime)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.Reason)
            .HasMaxLength(500);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasIndex(a => a.PatientId);
        builder.HasIndex(a => a.DoctorId);
        builder.HasIndex(a => a.AppointmentDateTime);

        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDateTime })
            .IsUnique()
            .HasFilter($"[{nameof(Appointment.Status)}] IN ({(int)AppointmentStatus.Scheduled}, {(int)AppointmentStatus.Confirmed})");

        builder.HasMany(a => a.StatusHistory)
            .WithOne(h => h.Appointment)
            .HasForeignKey(h => h.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(a => a.StatusHistory)
            .HasField("_statusHistory")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .AutoInclude(false);
    }
}
