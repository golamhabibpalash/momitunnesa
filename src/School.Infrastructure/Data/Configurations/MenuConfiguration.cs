using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;
using School.Domain.Enums;

namespace School.Infrastructure.Data.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Url)
            .HasMaxLength(500);

        builder.Property(e => e.Icon)
            .HasMaxLength(100);

        builder.Property(e => e.Controller)
            .HasMaxLength(100);

        builder.Property(e => e.Action)
            .HasMaxLength(100);

        builder.Property(e => e.Target)
            .HasMaxLength(50);

        builder.Property(e => e.MenuType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.MenuLocation)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(e => e.ParentMenu)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentMenuId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.ParentMenuId);
        builder.HasIndex(e => e.MenuLocation);
        builder.HasIndex(e => e.SortOrder);
    }
}
