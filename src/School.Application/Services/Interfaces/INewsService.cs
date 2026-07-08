using School.Application.DTOs.Common;
using School.Application.DTOs.News;

namespace School.Application.Services.Interfaces;

public interface INewsService
{
    Task<ResponseDto<IEnumerable<NewsDto>>> GetAllAsync();
    Task<ResponseDto<NewsDto>> GetByIdAsync(int id);
    Task<ResponseDto<NewsDto>> CreateAsync(NewsCreateDto dto);
    Task<ResponseDto<NewsDto>> UpdateAsync(NewsUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<IEnumerable<NewsDto>>> GetActiveNewsAsync();
    Task<ResponseDto<IEnumerable<NewsDto>>> GetFeaturedNewsAsync();
}
