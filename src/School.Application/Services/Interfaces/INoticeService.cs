using School.Application.DTOs.Common;
using School.Application.DTOs.Notice;
using School.Domain.Enums;

namespace School.Application.Services.Interfaces;

public interface INoticeService
{
    Task<ResponseDto<IEnumerable<NoticeDto>>> GetAllAsync();
    Task<ResponseDto<NoticeDto>> GetByIdAsync(int id);
    Task<ResponseDto<NoticeDto>> CreateAsync(NoticeCreateDto dto);
    Task<ResponseDto<NoticeDto>> UpdateAsync(NoticeUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<IEnumerable<NoticeDto>>> GetActiveNoticesAsync();
    Task<ResponseDto<IEnumerable<NoticeDto>>> GetFeaturedNoticesAsync();
    Task<ResponseDto<IEnumerable<NoticeDto>>> GetNoticesByCategoryAsync(NoticeCategory category);
    Task<ResponseDto<bool>> IncrementViewCountAsync(int id);
}
