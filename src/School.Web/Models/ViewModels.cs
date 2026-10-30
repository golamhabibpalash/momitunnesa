namespace School.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public class HeadTeacherMessageViewModel
{
    public string? Name { get; set; }
    public string? NameBn { get; set; }
    public string? Message { get; set; }
    public string? MessageBn { get; set; }
    public string? ImageUrl { get; set; }
}

public class ChairmanMessageViewModel
{
    public string? Name { get; set; }
    public string? NameBn { get; set; }
    public string? Message { get; set; }
    public string? MessageBn { get; set; }
    public string? ImageUrl { get; set; }
}

public class StatisticsViewModel
{
    public int Teachers { get; set; }
    public int Students { get; set; }
    public int Staff { get; set; }
    public int Achievements { get; set; }
}

public class LinkItem
{
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Url { get; set; }
    public string? Icon { get; set; }
}

public class PageViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Slug { get; set; }
    public string? Content { get; set; }
    public string? ContentBn { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? FeaturedImage { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SchoolProfileViewModel
{
    public string? Eiin { get; set; }
    public string? SchoolCode { get; set; }
    public string? EstablishedYear { get; set; }
    public string? Board { get; set; }
    public int TeacherCount { get; set; }
    public int StudentCount { get; set; }
    public string? Division { get; set; }
    public string? District { get; set; }
    public string? Upazila { get; set; }
    public string? PostOffice { get; set; }
    public string? PostCode { get; set; }
}

public class NoticeListViewModel
{
    public List<NoticeItem>? Notices { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? Category { get; set; }
}

public class NoticeItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? CategoryBn { get; set; }
    public DateTime? PublishDate { get; set; }
    public bool IsFeatured { get; set; }
    public List<AttachmentItem>? Attachments { get; set; }
}

public class NoticeViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Content { get; set; }
    public string? ContentBn { get; set; }
    public string? Category { get; set; }
    public string? CategoryBn { get; set; }
    public DateTime? PublishDate { get; set; }
    public bool IsFeatured { get; set; }
    public List<AttachmentItem>? Attachments { get; set; }
}

public class AttachmentItem
{
    public string? Name { get; set; }
    public string? NameBn { get; set; }
    public string? Url { get; set; }
    public string? Size { get; set; }
}

public class NewsListViewModel
{
    public List<NewsItem>? NewsItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? Category { get; set; }
    public string? SearchQuery { get; set; }
}

public class NewsItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public DateTime? PublishDate { get; set; }
}

public class NewsViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Content { get; set; }
    public string? ContentBn { get; set; }
    public string? ImageUrl { get; set; }
    public string? Author { get; set; }
    public string? Category { get; set; }
    public string? CategoryBn { get; set; }
    public DateTime? PublishDate { get; set; }
    public List<string>? Tags { get; set; }
}

public class EventListViewModel
{
    public List<EventItem>? Events { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? Filter { get; set; }
}

public class EventItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Description { get; set; }
    public DateTime? EventDate { get; set; }
    public string? Location { get; set; }
    public string? LocationBn { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
}

public class TeacherListViewModel
{
    public List<TeacherItem>? Teachers { get; set; }
    public string? Designation { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class TeacherItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? NameBn { get; set; }
    public string? Designation { get; set; }
    public string? DesignationBn { get; set; }
    public string? Qualification { get; set; }
    public string? ImageUrl { get; set; }
    public string? Subject { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsHeadTeacher { get; set; }
}

public class TeacherViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? NameBn { get; set; }
    public string? Designation { get; set; }
    public string? DesignationBn { get; set; }
    public string? Qualification { get; set; }
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string? SubjectBn { get; set; }
    public string? Biography { get; set; }
    public string? BiographyBn { get; set; }
    public string? Experience { get; set; }
    public DateTime? JoiningDate { get; set; }
}

public class StaffListViewModel
{
    public List<StaffItem>? Staff { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class StaffItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? NameBn { get; set; }
    public string? Designation { get; set; }
    public string? DesignationBn { get; set; }
    public string? Department { get; set; }
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class GalleryIndexViewModel
{
    public List<GalleryAlbum>? Albums { get; set; }
    public List<GalleryVideoItem> Videos { get; set; } = new();
}

public class GalleryVideoItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? ThumbnailUrl { get; set; }
}

public class GalleryAlbum
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? CoverImageUrl { get; set; }
    public int PhotoCount { get; set; }
}

public class GalleryAlbumViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Description { get; set; }
    public string? DescriptionBn { get; set; }
    public List<string>? Images { get; set; }
}

public class DownloadListViewModel
{
    public List<DownloadItem>? Downloads { get; set; }
    public string? Category { get; set; }
    public string? SearchQuery { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class DownloadItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Description { get; set; }
    public string? FileUrl { get; set; }
    public string? FileType { get; set; }
    public string? FileSize { get; set; }
    public string? Category { get; set; }
    public string? CategoryBn { get; set; }
    public DateTime? UploadDate { get; set; }
}

public class AcademicCalendarViewModel
{
    public int Year { get; set; }
    public List<CalendarEvent>? Events { get; set; }
}

public class CalendarEvent
{
    public DateTime Date { get; set; }
    public string? Title { get; set; }
    public string? TitleBn { get; set; }
    public string? Description { get; set; }
    public bool IsHoliday { get; set; }
}

public class ContactViewModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}

public class SearchViewModel
{
    public string? Query { get; set; }
    public List<SearchResult>? Results { get; set; }
    public int TotalResults { get; set; }
    public int CurrentPage { get; set; }
}

public class SearchResult
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Type { get; set; }
    public string? TypeBn { get; set; }
    public string? Date { get; set; }
}

public class PersonCardModel
{
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? Meta { get; set; }
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? DetailsUrl { get; set; }
}
