# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A bilingual (English/Bangla) school website with a public site and an admin CMS, built with ASP.NET Core MVC on .NET 10, EF Core (SQL Server), and ASP.NET Identity. The solution is `SchoolManagement.sln`. There is no test project.

## Commands

Run from the repo root.

```bash
dotnet build SchoolManagement.sln
dotnet run --project src/School.Web

# EF Core migrations: the DbContext lives in Infrastructure, the startup project is Web
dotnet ef migrations add <Name> --project src/School.Infrastructure --startup-project src/School.Web
dotnet ef database update --project src/School.Infrastructure --startup-project src/School.Web
```

You need SQL Server at `localhost,1433` (connection string in `src/School.Web/appsettings.json`, database `SchoolManagement`). On startup, `Program.cs` calls `Database.CanConnect()` and exits with an error if the database is unreachable or missing. It does **not** apply migrations automatically, so run `dotnet ef database update` first.

On startup, `Program.cs` also seeds a default `SchoolInfo` row, `SiteSettings` key/value rows, the roles `SuperAdmin`/`Admin`/`Editor`, and the admin user `admin@school.com` / `Admin@123`.

Serilog writes to the console and to `src/School.Web/logs/log-*.txt`.

## Architecture

Clean-architecture layering, with references pointing inward: `Web → Infrastructure → Application → Domain`.

- **School.Domain**: entities, enums, and the `IRepository<T>` / `IUnitOfWork` interfaces. Every entity derives from `BaseEntity` (Id, audit fields, soft-delete fields). Content entities derive from `BaseTranslatableEntity`, which adds paired `TitleEn`/`TitleBn` and `ContentEn`/`ContentBn` fields, `Slug`, SEO meta, `Status`, and `SortOrder`.
- **School.Application**: services (`Services/Interfaces` + `Services/Implementations`), DTOs, FluentValidation validators, and a single AutoMapper `Mapping/MappingProfile.cs`. Services take `IRepository<TEntity>` + `IMapper` and return `ResponseDto<T>` (`Ok` / `OkWithCount` / `Fail`). They catch exceptions inside and return `Fail` rather than throwing. Each DTO folder follows the pattern `XDto`, `XCreateDto`, and `XUpdateDto : XCreateDto`. New services must be registered in `Extensions/ServiceCollectionExtensions.cs` (`AddApplicationServices`).
- **School.Infrastructure**: `ApplicationDbContext` (Identity + all entities, configurations under `Data/Configurations`), the generic `Repository<T>`, `UnitOfWork`, and migrations. Application cannot reference EF, so a query that needs EF or entities with no repository fit (visitor stats, activity log, `SchoolInfo`) goes through `IApplicationDataService`. Its interface is in Application and its implementation is in `Infrastructure/Services/ApplicationDataService.cs`. Identity and cookie config live in `DependencyInjection.cs`, and the login path is `/Admin/Account/Login`.
- **School.Web**: MVC app.
  - Public controllers in `Controllers/` inherit `BaseController`.
  - Admin CMS lives in `Areas/Admin`. Controllers inherit `BaseAdminController` (`[Area("Admin")]`, `[Authorize]`, TempData notification helpers such as `SetSuccessNotification`). Most add `[Authorize(Roles = "SuperAdmin,Admin")]`.
  - Shared layout pieces (menus, footer, latest news/notices, language switcher) are ViewComponents.
  - `VisitorCounterMiddleware` records visits.
  - Custom routes: `page/{slug}`, `notice/{slug}`, `news/{slug}`, plus the area route defaulting to `Dashboard`.

### Data-layer behaviors to know

- **Soft delete everywhere**: `Repository.Delete` sets `IsDeleted` rather than removing the row. A global query filter (`!IsDeleted`) applies to every `BaseEntity`, so deleted rows are invisible unless you use `IgnoreQueryFilters()`.
- `ApplicationDbContext.SaveChangesAsync` stamps `CreatedAt`/`UpdatedAt` automatically.
- Site-wide configuration is a key/value table (`SiteSetting`) accessed through `ISettingsService`. School identity and branding (names, slogan, footer, logos) live in the single `SchoolInfo` row.

### Localization

- Supported cultures are `en` (default) and `bn`, chosen via the ASP.NET culture cookie. `HomeController` has the action that sets the cookie.
- UI strings live in `Resources/SharedResource.resx` / `SharedResource.bn.resx`.
- Database content is bilingual through paired `*En`/`*Bn` fields. Pick between them with `LocalizationHelper.GetLocalizedContent(en, bn)` (or `BaseController.GetLocalizedContent`) instead of checking the culture by hand.

### Static files and uploads

Client libraries (bootstrap, jquery, jquery-validation) are vendored in `wwwroot/lib`. Uploaded media goes under `wwwroot/uploads/` (for example `uploads/branding/`), handled by `MediaService` and `SettingsController`. Allowed extensions and the size limit are set in `appsettings.json` under `SiteSettings:FileUpload`.
