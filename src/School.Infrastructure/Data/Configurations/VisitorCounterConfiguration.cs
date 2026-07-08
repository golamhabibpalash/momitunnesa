using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;

namespace School.Infrastructure.Data.Configurations;

public class VisitorCounterConfiguration : IEntityTypeConfiguration<VisitorCounter>
{
    public void Configure(EntityTypeBuilder<VisitorCounter> builder)
    {
        builder.ToTable("VisitorCounters");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.IPAddress)
            .HasMaxLength(50);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.PageUrl)
            .HasMaxLength(500);

        builder.HasIndex(e => e.Date)
            .HasDatabaseName("IX_VisitorCounters_Date");
    }
}
