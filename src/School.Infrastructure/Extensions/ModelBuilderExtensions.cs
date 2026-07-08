using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;

namespace School.Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static void SeedDefaultData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SchoolInfo>().HasData(new SchoolInfo
        {
            Id = 1,
            SchoolNameEn = "Momitunnesa Secondary School",
            SchoolNameBn = "মমিতুন্নেছা মাধ্যমিক বিদ্যালয়",
            SloganEn = "Education for Enlightenment",
            SloganBn = "শিক্ষাই আলোর পথ",
            FooterTextEn = "© 2024 Momitunnesa Secondary School. All rights reserved.",
            FooterTextBn = "© ২০২৪ মমিতুন্নেছা মাধ্যমিক বিদ্যালয়। সর্বস্বত্ত্ব সংরক্ষিত।"
        });

        modelBuilder.Entity<SiteSetting>().HasData(
            new SiteSetting
            {
                Id = 1,
                Key = "SiteName",
                Value = "Momitunnesa Secondary School",
                Description = "Name of the school website"
            },
            new SiteSetting
            {
                Id = 2,
                Key = "SiteNameBn",
                Value = "মমিতুন্নেছা মাধ্যমিক বিদ্যালয়",
                Description = "School name in Bangla"
            },
            new SiteSetting
            {
                Id = 3,
                Key = "SiteDescription",
                Value = "Official website of Momitunnesa Secondary School",
                Description = "Site description for SEO"
            },
            new SiteSetting
            {
                Id = 4,
                Key = "DefaultLanguage",
                Value = "Bangla",
                Description = "Default display language"
            },
            new SiteSetting
            {
                Id = 5,
                Key = "ItemsPerPage",
                Value = "20",
                Description = "Number of items per page in listings"
            },
            new SiteSetting
            {
                Id = 6,
                Key = "MaintenanceMode",
                Value = "False",
                Description = "Enable/disable maintenance mode"
            },
            new SiteSetting
            {
                Id = 7,
                Key = "EnableRegistration",
                Value = "True",
                Description = "Allow new user registration"
            }
        );
    }
}
