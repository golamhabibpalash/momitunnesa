# AGENTS.md

Verified against the current code. There is no README, CI, test project, or linter/formatter config. `CLAUDE.md` imports this file and adds a short layer overview.

## Commands

Run from the repo root.

```bash
dotnet build SchoolManagement.sln
dotnet run --project src/School.Web   # http://localhost:5189 or https://localhost:7030

dotnet ef migrations add <Name> --project src/School.Infrastructure --startup-project src/School.Web
dotnet ef database update --project src/School.Infrastructure --startup-project src/School.Web
```

- The `dotnet ef` commands require a globally installed `dotnet-ef`; there is no local tool manifest in the repo.
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
- Admin URLs are `/Admin/{Controller}/{Action}` via the `areas` conventional route in `Program.cs`; there are no `[Route]` attributes or API endpoints.
- Admin create/edit share one model-bound view: `Edit` actions return `View("Create", dto)` (or `"Form"`/`"Edit"`) with the id in `ViewBag`. File inputs use their own names (`Image`, `Photo`, `File`...) and go through `BaseAdminController.UploadFileAsync`/`DeleteUploadAsync`; delete the old file only after the update succeeds.
- `BaseAdminController.OnActionExecutionAsync` writes an `ActivityLog` row for every successful admin POST (redirect/JSON result with no `ErrorMessage` in TempData). Use `SetErrorNotification` on failure or the failure is logged as success.
- Non-nullable `string` DTO properties are implicitly required by MVC even without `[Required]`. Remove the ModelState key for values the controller fills in (upload URLs) or that are optional on the form.
- `BaseAdminController.NotFound(...)` is a redirect-to-Dashboard helper, not a 404. In admin controllers, both `return NotFound()` and `base.NotFound()` call it, hiding `Controller.NotFound()`; return `new NotFoundResult()` or `StatusCode(404)` for a real 404.
- Content controllers are `[Authorize(Roles = "SuperAdmin,Admin")]`; `Settings`, `User`, and `Role` are `SuperAdmin` only. Login path is `/Admin/Account/Login`.
- Public error helpers on `BaseController` (`NotFoundResponse`, `BadRequestResponse`, `ServerError`) return JSON bodies, not error views.
- The public header nav and footer quick links are hardcoded in `_Layout.cshtml`; admin-created `Main`/`Footer` menus are appended by the `MainMenu`/`FooterMenu` view components.
- Category and designation labels (en/bn) come from `Helpers/CategoryHelper.cs`; admin selects and public filters both iterate the enums, so never hardcode category strings.
- `page/{slug}` works. `notice/{slug}` and `news/{slug}` are registered but their actions take `int id` and links use `/Notice/Details/{id}`; don't build on the slug routes without changing the actions.
- Detail lookups (`PagesController` via `GetBySlugAsync`, News/Notice/Event `GetByIdAsync`) do not check `IsPublished`/`Status`/`IsActive`. Drafts and inactive rows are reachable by URL unless you add those filters.

## Data-model traps

- Only `Page` derives from `BaseTranslatableEntity`. Most entities (`News`, `Notice`, `Event`, `Slider`, `Teacher`, `Staff`, etc.) derive from `BaseEntity` and declare their own `TitleEn`/`TitleBn` (or `NameEn`/`NameBn`) plus `IsActive`; don't assume a uniform `ContentEn`/`Status` shape.
- `SchoolInfo`, `VisitorCounter`, and `ActivityLog` do not derive from `BaseEntity`: no soft-delete filter and no automatic audit stamping. `SchoolInfo` therefore cannot use `IRepository<SchoolInfo>`.
- All `BaseEntity` rows are soft-deleted (`Delete` sets `IsDeleted`) and globally filtered to `!IsDeleted`; use `IgnoreQueryFilters()` only when deleted rows are genuinely needed.
- `ApplicationDbContext.SaveChangesAsync` stamps `CreatedAt`/`UpdatedAt`; domain entities also initialize `CreatedAt` to UTC.
- `SchoolInfo` is the single row read through `IApplicationDataService`. Adding a field means updating the entity, DTOs, Settings view/controller, and the manual copy list in `AddOrUpdateSchoolInfoAsync`, or the value will silently never save.
- Partial settings updates rely on `SchoolInfoUpdateDto` mapping only non-null members. Preserve that condition; empty strings are the way the admin clears a field.
- `PageService` regenerates `Slug` from `TitleEn` (fallback `TitleBn`) on every create and update; custom slugs are overwritten. The admin Create form shows a Slug input, but `PageCreateDto` has no `Slug` property, so that field is ignored.
- SEO defaults and the homepage student count are `SiteSetting` rows (keys in `Helpers/SeoSettingKeys.cs`), written with upsert by the admin SEO page and read in `_Layout.cshtml`/`HomeController`.
- New `SiteSetting` keys are not backfilled into an existing database because startup seeding only runs when the table is empty.

## Localization and front end

- Cultures are `en` (default) and `bn`, stored in the ASP.NET culture cookie set by `HomeController.SwitchLanguage`.
- Pick database content with `LocalizationHelper.GetLocalizedContent(en, bn)` or `BaseController.GetLocalizedContent`; don't branch on culture manually.
- `Resources/SharedResource*.resx` exist but are referenced nowhere. UI strings are hardcoded `isRtl`/bilingual ternaries; the admin UI is English-only. Don't assume resource localization is wired up.
- Front-end libraries (Bootstrap, jQuery, Font Awesome) load from CDNs in both layouts; the admin layout also uses CDN DataTables, Chart.js, and SweetAlert2. The exception is TinyMCE, loaded from `~/lib/tinymce` via the `Areas/Admin/Views/Shared/_RichTextEditor.cshtml` partial; `wwwroot/lib` is committed despite being gitignored. Never switch to `cdn.tiny.cloud` (it shows an "API key required" prompt).
- The admin layout initializes every `.table-datatable` and strips `<td colspan>` placeholder rows first (DataTables rejects them with "Incorrect column count"). Don't add a second page-level `DataTable()` call on those tables. There is no npm/build step; don't add package tooling.

## Uploads

- `MediaService` hardcodes a 10 MB limit (100 MB for .mp4/.webm/.ogg/.mov) and its own extension list (which differs from `SiteSettings:FileUpload` in `appsettings.json`). The `SiteSettings:FileUpload` values are never read.
- Uploads resolve from `Directory.GetCurrentDirectory()/wwwroot/uploads`. With the documented `dotnet run --project src/School.Web`, the Web SDK runs the app with `src/School.Web` as its working directory, so files land in `src/School.Web/wwwroot/uploads`; launching the built DLL from another directory changes that location.
- `MediaService.NormalizeFolder` rejects paths that escape `wwwroot/uploads`; route every user-supplied folder through it. Gallery and Media upload actions raise the request size limit with `[RequestSizeLimit]`; a reverse proxy (IIS `maxAllowedContentLength`, nginx `client_max_body_size`) needs a matching limit.
- Gallery videos store either a YouTube URL or an uploaded `/uploads/...` path in `GalleryVideo.YouTubeUrl`; use `Helpers/VideoHelper.cs` to tell them apart.
- Branding replacement in `SettingsController` only deletes files under `/uploads/` and only after the new path is persisted; don't delete static assets such as `/favicon.ico`.

## Validation

- FluentValidation validators exist under `src/School.Application/Validators`, but nothing registers them (`AddFluentValidationAutoValidation` is absent). MVC currently validates with DataAnnotations only; register them deliberately if you expect them to run.
