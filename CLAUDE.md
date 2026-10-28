# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A bilingual (English/Bangla) school website with a public site and an admin CMS, built with ASP.NET Core MVC on .NET 10, EF Core 10 (SQL Server), and ASP.NET Identity. The solution is `SchoolManagement.sln`. There is no README, test project, CI, or linter config, so you verify changes with `dotnet build` and by running the app.

Commands, change points, and known data-model and config traps are in `AGENTS.md`, imported below. Keep the two files consistent: put new verified facts in `AGENTS.md`, and keep only the orientation material here.

@AGENTS.md

## Layer responsibilities

References point inward: `Web → Infrastructure → Application → Domain`.

- **School.Domain**: entities, enums, and the `IRepository<T>` / `IUnitOfWork` interfaces. Most entities derive from `BaseEntity` (Id, audit fields, soft-delete fields). Only `Page` uses `BaseTranslatableEntity`.
- **School.Application**: services (`Services/Interfaces` + `Services/Implementations`) that take `IRepository<TEntity>` + `IMapper`. Also holds DTOs, the unregistered FluentValidation validators, and a single AutoMapper `MappingProfile`.
- **School.Infrastructure**: `ApplicationDbContext` (Identity plus all entities, with configurations under `Data/Configurations`), the generic `Repository<T>`, `UnitOfWork`, migrations, and `ApplicationDataService`. Identity and cookie config are in `DependencyInjection.cs`.
- **School.Web**: MVC app. Public controllers are in `Controllers/` and the admin CMS is in `Areas/Admin`. Shared layout pieces (main/footer menus, footer info, quick links, latest news/notices, language switcher) are ViewComponents in `ViewComponents/`. `Middleware/VisitorCounterMiddleware` records visits.

Site-wide configuration is a key/value `SiteSetting` table accessed through `ISettingsService`. School identity and branding (names, slogan, footer, logos) live in the single `SchoolInfo` row.
