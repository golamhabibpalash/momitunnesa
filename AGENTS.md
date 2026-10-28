# AGENTS.md

Verified against the current code. There is no README, CI, test project, linter/formatter config, or local dotnet tool manifest. `CLAUDE.md` imports this file and adds a short layer overview.

## Commands

Run from the repo root.

```bash
dotnet build SchoolManagement.sln
dotnet run --project src/School.Web   # http://localhost:5189 or https://localhost:7030

dotnet ef migrations add <Name> --project src/School.Infrastructure --startup-project src/School.Web
dotnet ef database update --project src/School.Infrastructure --startup-project src/School.Web
```

- Startup exits with code 1 if SQL Server at `localhost,1433` is unreachable or the `SchoolManagement` database is missing. Migrations are never applied automatically, so run `database update` first.
- Startup seeding in `Program.cs` seeds `SchoolInfo` and default `SiteSettings` only when those tables are empty, and creates roles `SuperAdmin`/`Admin`/`Editor` plus `admin@school.com` / `Admin@123` when absent. `ModelBuilderExtensions.SeedDefaultData` is never called and the migration has no `HasData`.
- Verification is `dotnet build` plus running the app; there are no tests. The build currently emits NU1902/NU1903 warnings for transitive `System.Security.Cryptography.Pkcs` 9.0.4; these are pre-existing.
- Serilog writes to the console and `logs/log-.txt` relative to the working directory (`src/School.Web` with the documented run command).

## Architecture and change points

- Layering is `Web -> Infrastructure -> Application -> Domain` (net10.0, EF Core 10, SQL Server).
- Application services catch exceptions and return `ResponseDto<T>` (`Ok`/`OkWithCount`/`Fail`); don't add throwing-style services next to them.
- Register every new service in `src/School.Application/Extensions/ServiceCollectionExtensions.cs` (`AddApplicationServices`).
- Application cannot reference EF. EF-specific queries and entities without a repository fit (`SchoolInfo`, visitor stats, activity log) go through `IApplicationDataService`, implemented in `src/School.Infrastructure/Services/ApplicationDataService.cs`.
- DTO folders use `XDto`, `XCreateDto`, `XUpdateDto : XCreateDto`; AutoMapper config is centralized in `Mapping/MappingProfile.cs`.
- Public controllers inherit `BaseController`; admin controllers in `Areas/Admin` inherit `BaseAdminController` and use its `SetSuccessNotification`/`SetErrorNotification` helpers (rendered by `_Notification.cshtml`).
- Content controllers are `[Authorize(Roles = "SuperAdmin,Admin")]`; `Settings`, `User`, and `Role` are `SuperAdmin` only. Login path is `/Admin/Account/Login`.
- Public error helpers on `BaseController` (`NotFoundResponse`, `BadRequestResponse`, `ServerError`) return JSON bodies, not error views.
- `page/{slug}` works. `notice/{slug}` and `news/{slug}` are registered but their actions take `int id` and links use `/Notice/Details/{id}`; don't build on the slug routes without changing the actions.

## Data-model traps

- Only `Page` derives from `BaseTranslatableEntity`. Most entities (`News`, `Notice`, `Event`, `Slider`, `Teacher`, `Staff`, etc.) derive from `BaseEntity` and declare their own `TitleEn`/`TitleBn` (or `NameEn`/`NameBn`) plus `IsActive`; don't assume a uniform `ContentEn`/`Status` shape.
- `SchoolInfo`, `VisitorCounter`, and `ActivityLog` do not derive from `BaseEntity`: no soft-delete filter and no automatic audit stamping. `SchoolInfo` therefore cannot use `IRepository<SchoolInfo>`.
- All `BaseEntity` rows are soft-deleted (`Delete` sets `IsDeleted`) and globally filtered to `!IsDeleted`; use `IgnoreQueryFilters()` only when deleted rows are genuinely needed.
- `ApplicationDbContext.SaveChangesAsync` stamps `CreatedAt`/`UpdatedAt`; domain entities also initialize `CreatedAt` to UTC.
- `SchoolInfo` is the single row read through `IApplicationDataService`. Adding a field means updating the entity, DTOs, Settings view/controller, and the manual copy list in `AddOrUpdateSchoolInfoAsync`, or the value will silently never save.
- Partial settings updates rely on `SchoolInfoUpdateDto` mapping only non-null members. Preserve that condition; empty strings are the way the admin clears a field.
- `PageService` regenerates `Slug` from `TitleEn` (fallback `TitleBn`) on every create and update; custom slugs are overwritten.
- New `SiteSetting` keys are not backfilled into an existing database because startup seeding only runs when the table is empty.

## Localization and front end

- Cultures are `en` (default) and `bn`, stored in the ASP.NET culture cookie set by `HomeController.SwitchLanguage`.
- Pick database content with `LocalizationHelper.GetLocalizedContent(en, bn)` or `BaseController.GetLocalizedContent`; don't branch on culture manually.
- `Resources/SharedResource*.resx` exist but are referenced nowhere. UI strings are hardcoded `isRtl`/bilingual ternaries; the admin UI is English-only. Don't assume resource localization is wired up.
- Front-end libraries (Bootstrap, jQuery, Font Awesome) load from CDNs in both layouts; the admin layout also uses CDN DataTables, Chart.js, and SweetAlert2. `wwwroot/lib` is vendored but not referenced, and there is no npm/build step; don't add package tooling.

## Uploads

- `MediaService` hardcodes the 10 MB limit and extension list. The `SiteSettings:FileUpload` values in `appsettings.json` are never read.
- Uploads resolve from `Directory.GetCurrentDirectory()/wwwroot/uploads`. With the documented `dotnet run --project src/School.Web`, the launch profile makes `src/School.Web` the working directory, so files land in `src/School.Web/wwwroot/uploads`.
- Branding replacement in `SettingsController` only deletes files under `/uploads/` and only after the new path is persisted; don't delete static assets such as `/favicon.ico`.

## Validation

- FluentValidation validators exist under `src/School.Application/Validators`, but nothing registers them (`AddFluentValidationAutoValidation` is absent). MVC currently validates with DataAnnotations only; register them deliberately if you expect them to run.
