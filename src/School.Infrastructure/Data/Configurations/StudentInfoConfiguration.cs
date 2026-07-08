using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;

namespace School.Infrastructure.Data.Configurations;

public class StudentInfoConfiguration : IEntityTypeConfiguration<StudentInfo>
{
    public void Configure(EntityTypeBuilder<StudentInfo> builder)
    {
        builder.ToTable("StudentInfos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.NameBn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Class)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Roll)
            .HasMaxLength(20);

        builder.Property(e => e.Section)
            .HasMaxLength(50);

        builder.Property(e => e.FatherName)
            .HasMaxLength(200);

        builder.Property(e => e.MotherName)
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(e => e.Class);
        builder.HasIndex(e => e.Roll);
        builder.HasIndex(e => e.IsActive);
    }
}
