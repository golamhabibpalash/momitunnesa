using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;
using School.Domain.Enums;

namespace School.Infrastructure.Data.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TitleEn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.TitleBn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(e => e.PublishDate)
            .HasDatabaseName("IX_News_PublishDate");

        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.IsFeatured);
    }
}
