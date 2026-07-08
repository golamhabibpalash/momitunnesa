using School.Application.DTOs.Common;
using School.Application.DTOs.Settings;

namespace School.Application.Services.Interfaces;

public interface ISettingsService
{
    Task<ResponseDto<IEnumerable<SiteSettingDto>>> GetAllSettingsAsync();
    Task<ResponseDto<SiteSettingDto>> GetSettingByKeyAsync(string key);
    Task<ResponseDto<bool>> UpdateSettingAsync(SiteSettingUpdateDto dto);

    Task<ResponseDto<IEnumerable<SocialLinkDto>>> GetAllSocialLinksAsync();
    Task<ResponseDto<SocialLinkDto>> CreateSocialLinkAsync(SocialLinkCreateDto dto);
    Task<ResponseDto<bool>> DeleteSocialLinkAsync(int id);

    Task<ResponseDto<SchoolInfoDto>> GetSchoolInfoAsync();
    Task<ResponseDto<bool>> UpdateSchoolInfoAsync(SchoolInfoUpdateDto dto);
}
