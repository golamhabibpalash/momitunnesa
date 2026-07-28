using School.Infrastructure;
using School.Application.Extensions;
using School.Web.Helpers;
using Serilog;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using School.Web.Middleware;
using Microsoft.AspNetCore.Identity;
using School.Domain.Entities;
using School.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddApplicationServices();
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("bn") };
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.SchoolInfos.Any())
    {
        db.SchoolInfos.Add(new SchoolInfo
        {
            SchoolNameEn = "Momitunnesa Secondary School",
            SchoolNameBn = "মমিতুন্নেছা মাধ্যমিক বিদ্যালয়",
            SloganEn = "Education for Enlightenment",
            SloganBn = "শিক্ষাই আলোর পথ",
            FooterTextEn = "&copy; 2024 Momitunnesa Secondary School. All rights reserved.",
            FooterTextBn = "&copy; ২০২৪ মমিতুন্নেছা মাধ্যমিক বিদ্যালয়। সর্বস্বত্ত্ব সংরক্ষিত।"
        });
    }

    if (!db.SiteSettings.Any())
    {
        db.SiteSettings.AddRange(
            new SiteSetting { Key = "SiteName", Value = "Momitunnesa Secondary School" },
            new SiteSetting { Key = "SiteNameBn", Value = "মমিতুন্নেছা মাধ্যমিক বিদ্যালয়" },
            new SiteSetting { Key = "SiteDescription", Value = "Official website of Momitunnesa Secondary School" },
            new SiteSetting { Key = "DefaultLanguage", Value = "Bangla" },
            new SiteSetting { Key = "ItemsPerPage", Value = "20" },
            new SiteSetting { Key = "MaintenanceMode", Value = "False" },
            new SiteSetting { Key = "EnableRegistration", Value = "True" }
        );
    }

    await db.SaveChangesAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("SuperAdmin"))
        await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    if (!await roleManager.RoleExistsAsync("Editor"))
        await roleManager.CreateAsync(new IdentityRole("Editor"));

    if (await userManager.FindByEmailAsync("admin@school.com") is null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@school.com",
            Email = "admin@school.com",
            FullName = "Super Admin",
            IsActive = true,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<VisitorCounterMiddleware>();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "pages",
    pattern: "page/{slug}",
    defaults: new { controller = "Pages", action = "Index" });

app.MapControllerRoute(
    name: "notice",
    pattern: "notice/{slug}",
    defaults: new { controller = "Notice", action = "Details" });

app.MapControllerRoute(
    name: "news",
    pattern: "news/{slug}",
    defaults: new { controller = "News", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();
app.Run();
