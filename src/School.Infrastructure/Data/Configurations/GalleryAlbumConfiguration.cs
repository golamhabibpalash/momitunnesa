using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;

namespace School.Infrastructure.Data.Configurations;

public class GalleryAlbumConfiguration : IEntityTypeConfiguration<GalleryAlbum>
{
    public void Configure(EntityTypeBuilder<GalleryAlbum> builder)
    {
        builder.ToTable("GalleryAlbums");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TitleEn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.TitleBn)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.CoverImageUrl)
            .HasMaxLength(500);

        builder.HasMany(e => e.GalleryImages)
            .WithOne(e => e.GalleryAlbum)
            .HasForeignKey(e => e.GalleryAlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.IsActive);
    }
}
