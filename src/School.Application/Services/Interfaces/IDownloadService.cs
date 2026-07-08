using School.Application.DTOs.Common;
using School.Application.DTOs.Download;
using School.Domain.Enums;

namespace School.Application.Services.Interfaces;

public interface IDownloadService
{
    Task<ResponseDto<IEnumerable<DownloadDto>>> GetAllAsync();
    Task<ResponseDto<DownloadDto>> GetByIdAsync(int id);
    Task<ResponseDto<DownloadDto>> CreateAsync(DownloadCreateDto dto);
    Task<ResponseDto<DownloadDto>> UpdateAsync(DownloadUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<IEnumerable<DownloadDto>>> GetDownloadsByCategoryAsync(DownloadCategory category);
}
