using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(d => d.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.ProfessionalLicense)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.SpecialtyId)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.HasIndex(d => d.UserId)
            .IsUnique();

        builder.HasIndex(d => d.ProfessionalLicense)
            .IsUnique();

        builder.HasMany(d => d.Availabilities)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Appointments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(d => d.Availabilities)
            .HasField("_availabilities")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(d => d.Appointments)
            .HasField("_appointments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
