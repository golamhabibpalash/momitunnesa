using System.ComponentModel.DataAnnotations;
using School.Application.DTOs.Common;
using School.Application.DTOs.Settings;
using School.Domain.Enums;

namespace School.Application.Services.Interfaces;

public class MenuDto
{
    public int Id { get; set; }
    public int? ParentMenuId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleBn { get; set; }
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public string MenuType { get; set; } = string.Empty;
    public string MenuLocation { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsExternal { get; set; }
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public string? Target { get; set; }
    public List<MenuDto> Children { get; set; } = new();
}

public class MenuCreateDto
{
    public int? ParentMenuId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleBn { get; set; }
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public MenuType MenuType { get; set; }
    public MenuLocation MenuLocation { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsExternal { get; set; }
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public string? Target { get; set; }
}

public class MenuUpdateDto : MenuCreateDto
{
    public int Id { get; set; }
}

public interface IMenuService
{
    Task<ResponseDto<IEnumerable<MenuDto>>> GetAllAsync();
    Task<ResponseDto<MenuDto>> GetByIdAsync(int id);
    Task<ResponseDto<MenuDto>> CreateAsync(MenuCreateDto dto);
    Task<ResponseDto<MenuDto>> UpdateAsync(MenuUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<IEnumerable<MenuDto>>> GetMenusByLocationAsync(MenuLocation location);
    Task<ResponseDto<IEnumerable<MenuDto>>> GetNavigationMenuAsync();
    Task<ResponseDto<bool>> UpdateSortOrderAsync(List<MenuSortOrderDto> items);
}

public class MenuSortOrderDto
{
    public int Id { get; set; }
    public int? ParentMenuId { get; set; }
    public int SortOrder { get; set; }
}
