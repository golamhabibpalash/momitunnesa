using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;
using School.Domain.Enums;

namespace School.Infrastructure.Data.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Pages");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TitleEn)
            .HasMaxLength(500);

        builder.Property(e => e.TitleBn)
            .HasMaxLength(500);

        builder.Property(e => e.Slug)
            .HasMaxLength(500);

        builder.Property(e => e.MetaTitle)
            .HasMaxLength(200);

        builder.Property(e => e.MetaDescription)
            .HasMaxLength(500);

        builder.Property(e => e.MetaKeywords)
            .HasMaxLength(300);

        builder.Property(e => e.FeaturedImage)
            .HasMaxLength(500);

        builder.Property(e => e.TemplateType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(e => e.Slug)
            .IsUnique()
            .HasFilter("[Slug] IS NOT NULL");

        builder.HasIndex(e => e.IsHomePage);
        builder.HasIndex(e => e.IsPublished);
        builder.HasIndex(e => e.SortOrder);
    }
}
