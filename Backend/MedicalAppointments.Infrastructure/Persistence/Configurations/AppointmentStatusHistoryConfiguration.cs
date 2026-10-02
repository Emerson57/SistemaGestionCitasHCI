using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Persistence.Configurations;

public class AppointmentStatusHistoryConfiguration : IEntityTypeConfiguration<AppointmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<AppointmentStatusHistory> builder)
    {
        builder.ToTable("AppointmentStatusHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .ValueGeneratedNever();

        builder.Property(h => h.AppointmentId)
            .IsRequired();

        builder.Property(h => h.Action)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(h => h.PreviousStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(h => h.NewStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(h => h.ChangedAt)
            .IsRequired();

        builder.Property(h => h.ChangedBy)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(h => h.AppointmentId);
    }
}
