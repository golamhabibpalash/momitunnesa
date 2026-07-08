using School.Application.DTOs.Achievement;
using School.Application.DTOs.Common;

namespace School.Application.Services.Interfaces;

public interface IAchievementService
{
    Task<ResponseDto<IEnumerable<AchievementDto>>> GetAllAsync();
    Task<ResponseDto<AchievementDto>> GetByIdAsync(int id);
    Task<ResponseDto<AchievementDto>> CreateAsync(AchievementDto dto);
    Task<ResponseDto<AchievementDto>> UpdateAsync(AchievementDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
}
