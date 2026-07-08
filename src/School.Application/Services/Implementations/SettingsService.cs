using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Settings;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class SettingsService : ISettingsService
{
    private readonly IRepository<SiteSetting> _settingRepo;
    private readonly IRepository<SocialLink> _socialLinkRepo;
    private readonly IApplicationDataService _dataService;
    private readonly IMapper _mapper;

    public SettingsService(
        IRepository<SiteSetting> settingRepo,
        IRepository<SocialLink> socialLinkRepo,
        IApplicationDataService dataService,
        IMapper mapper)
    {
        _settingRepo = settingRepo;
        _socialLinkRepo = socialLinkRepo;
        _dataService = dataService;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<SiteSettingDto>>> GetAllSettingsAsync()
    {
        try
        {
            var settings = await _settingRepo.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<SiteSettingDto>>(settings);
            return ResponseDto<IEnumerable<SiteSettingDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<SiteSettingDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<SiteSettingDto>> GetSettingByKeyAsync(string key)
    {
        try
        {
            var settings = await _settingRepo.FindAsync(s => s.Key == key);
            var setting = settings.FirstOrDefault();
            if (setting is null)
                return ResponseDto<SiteSettingDto>.Fail("Setting not found.");

            var dto = _mapper.Map<SiteSettingDto>(setting);
            return ResponseDto<SiteSettingDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<SiteSettingDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> UpdateSettingAsync(SiteSettingUpdateDto dto)
    {
        try
        {
            var settings = await _settingRepo.FindAsync(s => s.Key == dto.Key);
            var setting = settings.FirstOrDefault();
            if (setting is null)
            {
                setting = _mapper.Map<SiteSetting>(dto);
                await _settingRepo.AddAsync(setting);
            }
            else
            {
                _mapper.Map(dto, setting);
                _settingRepo.Update(setting);
            }

            await _settingRepo.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Setting updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<SocialLinkDto>>> GetAllSocialLinksAsync()
    {
        try
        {
            var links = await _socialLinkRepo.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<SocialLinkDto>>(links.OrderBy(l => l.SortOrder));
            return ResponseDto<IEnumerable<SocialLinkDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<SocialLinkDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<SocialLinkDto>> CreateSocialLinkAsync(SocialLinkCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<SocialLink>(dto);
            await _socialLinkRepo.AddAsync(entity);
            await _socialLinkRepo.SaveChangesAsync();

            var resultDto = _mapper.Map<SocialLinkDto>(entity);
            return ResponseDto<SocialLinkDto>.Ok(resultDto, "Social link created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<SocialLinkDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteSocialLinkAsync(int id)
    {
        try
        {
            var link = await _socialLinkRepo.GetByIdAsync(id);
            if (link is null)
                return ResponseDto<bool>.Fail("Social link not found.");

            _socialLinkRepo.Delete(link);
            await _socialLinkRepo.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Social link deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<SchoolInfoDto>> GetSchoolInfoAsync()
    {
        try
        {
            var info = await _dataService.GetSchoolInfoAsync();
            if (info is null)
                return ResponseDto<SchoolInfoDto>.Fail("School info not found.");

            var dto = _mapper.Map<SchoolInfoDto>(info);
            return ResponseDto<SchoolInfoDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<SchoolInfoDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> UpdateSchoolInfoAsync(SchoolInfoUpdateDto dto)
    {
        try
        {
            var info = await _dataService.GetSchoolInfoAsync();
            if (info is null)
            {
                info = _mapper.Map<SchoolInfo>(dto);
                await _dataService.AddOrUpdateSchoolInfoAsync(info);
            }
            else
            {
                _mapper.Map(dto, info);
                await _dataService.AddOrUpdateSchoolInfoAsync(info);
            }

            await _dataService.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "School info updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }
}
