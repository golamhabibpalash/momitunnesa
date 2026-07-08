using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using School.Domain.Common;
using School.Domain.Entities;

namespace School.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Menu> Menus { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Notice> Notices { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<GalleryAlbum> GalleryAlbums { get; set; }
    public DbSet<GalleryImage> GalleryImages { get; set; }
    public DbSet<GalleryVideo> GalleryVideos { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<ManagingCommittee> ManagingCommittees { get; set; }
    public DbSet<Download> Downloads { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<AcademicCalendar> AcademicCalendars { get; set; }
    public DbSet<Routine> Routines { get; set; }
    public DbSet<StudentInfo> StudentInfos { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Syllabus> Syllabuses { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<SiteSetting> SiteSettings { get; set; }
    public DbSet<SocialLink> SocialLinks { get; set; }
    public DbSet<VisitorCounter> VisitorCounters { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<SchoolInfo> SchoolInfos { get; set; }
    public DbSet<Publication> Publications { get; set; }
    public DbSet<ImportantLink> ImportantLinks { get; set; }
    public DbSet<Admission> Admissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ApplyGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder });
            }
        }
    }

    private static void ApplyGlobalQueryFilter<T>(ModelBuilder modelBuilder)
        where T : BaseEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
