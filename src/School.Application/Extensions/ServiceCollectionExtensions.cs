using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using School.Application.Mapping;
using School.Application.Services.Implementations;
using School.Application.Services.Interfaces;

namespace School.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<INoticeService, NoticeService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IMenuService, MenuService>();

        return services;
    }
}
