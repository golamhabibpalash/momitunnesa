using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;
using School.Domain.Enums;

namespace School.Infrastructure.Data.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("Teachers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.NameBn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Email)
            .HasMaxLength(100);

        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.QualificationEn)
            .HasMaxLength(500);

        builder.Property(e => e.QualificationBn)
            .HasMaxLength(500);

        builder.Property(e => e.SubjectEn)
            .HasMaxLength(200);

        builder.Property(e => e.SubjectBn)
            .HasMaxLength(200);

        builder.Property(e => e.Designation)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(e => e.Designation);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.IsHeadTeacher);
        builder.HasIndex(e => e.SortOrder);
    }
}
