using School.Application.DTOs.Common;
using School.Application.DTOs.Page;

namespace School.Application.Services.Interfaces;

public interface IPageService
{
    Task<ResponseDto<IEnumerable<PageDto>>> GetAllAsync();
    Task<ResponseDto<PageDto>> GetByIdAsync(int id);
    Task<ResponseDto<PageDto>> CreateAsync(PageCreateDto dto);
    Task<ResponseDto<PageDto>> UpdateAsync(PageUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<PageDto>> GetBySlugAsync(string slug);
    Task<ResponseDto<IEnumerable<PageDto>>> GetPublishedPagesAsync();
    Task<ResponseDto<PageDto>> GetHomePageAsync();
}
