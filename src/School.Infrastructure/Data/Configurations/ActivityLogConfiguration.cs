using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;

namespace School.Infrastructure.Data.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId)
            .HasMaxLength(100);

        builder.Property(e => e.UserName)
            .HasMaxLength(200);

        builder.Property(e => e.Action)
            .HasMaxLength(200);

        builder.Property(e => e.Controller)
            .HasMaxLength(200);

        builder.Property(e => e.IPAddress)
            .HasMaxLength(50);

        builder.Property(e => e.BrowserInfo)
            .HasMaxLength(500);

        builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_ActivityLogs_Timestamp");

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Action);
    }
}
