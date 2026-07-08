using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;

namespace School.Infrastructure.Data.Configurations;

public class SchoolInfoConfiguration : IEntityTypeConfiguration<SchoolInfo>
{
    public void Configure(EntityTypeBuilder<SchoolInfo> builder)
    {
        builder.ToTable("SchoolInfos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.SchoolNameEn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.SchoolNameBn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.SloganEn)
            .HasMaxLength(500);

        builder.Property(e => e.SloganBn)
            .HasMaxLength(500);

        builder.Property(e => e.AddressEn)
            .HasMaxLength(1000);

        builder.Property(e => e.AddressBn)
            .HasMaxLength(1000);

        builder.Property(e => e.Phone)
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .HasMaxLength(100);

        builder.Property(e => e.Website)
            .HasMaxLength(200);

        builder.Property(e => e.LogoUrl)
            .HasMaxLength(500);

        builder.Property(e => e.FaviconUrl)
            .HasMaxLength(500);

        builder.Property(e => e.GoogleMapEmbedUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.FooterTextEn)
            .HasMaxLength(1000);

        builder.Property(e => e.FooterTextBn)
            .HasMaxLength(1000);
    }
}
