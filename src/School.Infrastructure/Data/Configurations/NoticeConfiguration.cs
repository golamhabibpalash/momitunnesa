using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;
using School.Domain.Enums;

namespace School.Infrastructure.Data.Configurations;

public class NoticeConfiguration : IEntityTypeConfiguration<Notice>
{
    public void Configure(EntityTypeBuilder<Notice> builder)
    {
        builder.ToTable("Notices");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TitleEn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.TitleBn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.AttachmentUrl)
            .HasMaxLength(500);

        builder.Property(e => e.NoticeCategory)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(e => e.PublishDate)
            .HasDatabaseName("IX_Notices_PublishDate");

        builder.HasIndex(e => e.ExpiryDate)
            .HasDatabaseName("IX_Notices_ExpiryDate");

        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.IsFeatured);
    }
}
