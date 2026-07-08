using AutoMapper;
using Microsoft.AspNetCore.Identity;
using School.Application.DTOs.Account;
using School.Application.DTOs.Achievement;
using School.Application.DTOs.Contact;
using School.Application.DTOs.Download;
using School.Application.DTOs.Event;
using School.Application.DTOs.Gallery;
using School.Application.DTOs.News;
using School.Application.DTOs.Notice;
using School.Application.DTOs.Page;
using School.Application.DTOs.Settings;
using School.Application.DTOs.Slider;
using School.Application.DTOs.Staff;
using School.Application.DTOs.Teacher;
using School.Application.Services.Interfaces;
using School.Domain.Entities;

namespace School.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Slider, SliderDto>().ReverseMap();
        CreateMap<Slider, SliderCreateDto>().ReverseMap();
        CreateMap<Slider, SliderUpdateDto>().ReverseMap();

        CreateMap<Page, PageDto>()
            .ForMember(d => d.MetaTitle, o => o.MapFrom(s => s.MetaTitle))
            .ReverseMap();
        CreateMap<Page, PageCreateDto>().ReverseMap();
        CreateMap<Page, PageUpdateDto>().ReverseMap();

        CreateMap<Notice, NoticeDto>()
            .ForMember(d => d.NoticeCategory, o => o.MapFrom(s => s.NoticeCategory.ToString()));
        CreateMap<Notice, NoticeCreateDto>().ReverseMap();
        CreateMap<Notice, NoticeUpdateDto>().ReverseMap();

        CreateMap<News, NewsDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()));
        CreateMap<News, NewsCreateDto>().ReverseMap();
        CreateMap<News, NewsUpdateDto>().ReverseMap();

        CreateMap<Event, EventDto>().ReverseMap();
        CreateMap<Event, EventCreateDto>().ReverseMap();
        CreateMap<Event, EventUpdateDto>().ReverseMap();

        CreateMap<Teacher, TeacherDto>()
            .ForMember(d => d.Designation, o => o.MapFrom(s => s.Designation.ToString()));
        CreateMap<Teacher, TeacherCreateDto>().ReverseMap();
        CreateMap<Teacher, TeacherUpdateDto>().ReverseMap();

        CreateMap<Staff, StaffDto>()
            .ForMember(d => d.Designation, o => o.MapFrom(s => s.Designation.ToString()));
        CreateMap<Staff, StaffCreateDto>().ReverseMap();
        CreateMap<Staff, StaffUpdateDto>().ReverseMap();

        CreateMap<GalleryAlbum, GalleryAlbumDto>()
            .ForMember(d => d.ImageCount, o => o.Ignore())
            .ForMember(d => d.GalleryImages, o => o.MapFrom(s => s.GalleryImages));
        CreateMap<GalleryAlbum, GalleryAlbumCreateDto>().ReverseMap();
        CreateMap<GalleryAlbum, GalleryAlbumUpdateDto>().ReverseMap();

        CreateMap<GalleryImage, GalleryImageDto>().ReverseMap();
        CreateMap<GalleryImage, GalleryImageCreateDto>().ReverseMap();

        CreateMap<GalleryVideo, GalleryVideoDto>().ReverseMap();
        CreateMap<GalleryVideo, GalleryVideoCreateDto>().ReverseMap();
        CreateMap<GalleryVideo, GalleryVideoUpdateDto>().ReverseMap();

        CreateMap<Download, DownloadDto>()
            .ForMember(d => d.DownloadCategory, o => o.MapFrom(s => s.DownloadCategory.ToString()));
        CreateMap<Download, DownloadCreateDto>().ReverseMap();
        CreateMap<Download, DownloadUpdateDto>().ReverseMap();

        CreateMap<ContactMessage, ContactMessageDto>().ReverseMap();
        CreateMap<ContactMessage, ContactCreateDto>().ReverseMap();

        CreateMap<Achievement, AchievementDto>()
            .ForMember(d => d.AchievementCategory, o => o.MapFrom(s => s.AchievementCategory.ToString()));
        CreateMap<Achievement, AchievementDto>().ReverseMap();

        CreateMap<SiteSetting, SiteSettingDto>().ReverseMap();
        CreateMap<SiteSetting, SiteSettingUpdateDto>().ReverseMap();

        CreateMap<SocialLink, SocialLinkDto>().ReverseMap();
        CreateMap<SocialLink, SocialLinkCreateDto>().ReverseMap();

        CreateMap<SchoolInfo, SchoolInfoDto>().ReverseMap();
        CreateMap<SchoolInfo, SchoolInfoUpdateDto>().ReverseMap();

        CreateMap<Menu, MenuDto>()
            .ForMember(d => d.MenuType, o => o.MapFrom(s => s.MenuType.ToString()))
            .ForMember(d => d.MenuLocation, o => o.MapFrom(s => s.MenuLocation.ToString()))
            .ForMember(d => d.Children, o => o.Ignore());
        CreateMap<Menu, MenuCreateDto>().ReverseMap();
        CreateMap<Menu, MenuUpdateDto>().ReverseMap();

        CreateMap<ApplicationUser, UserDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.PhoneNumber))
            .ForMember(d => d.Roles, o => o.Ignore());

        CreateMap<IdentityRole, RoleDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
    }
}
