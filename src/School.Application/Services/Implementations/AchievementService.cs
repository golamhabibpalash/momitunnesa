using AutoMapper;
using School.Application.DTOs.Achievement;
using School.Application.DTOs.Common;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class AchievementService : IAchievementService
{
    private readonly IRepository<Achievement> _repository;
    private readonly IMapper _mapper;

    public AchievementService(IRepository<Achievement> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<AchievementDto>>> GetAllAsync()
    {
        try
        {
            var achievements = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<AchievementDto>>(achievements.OrderByDescending(a => a.Date));
            return ResponseDto<IEnumerable<AchievementDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<AchievementDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<AchievementDto>> GetByIdAsync(int id)
    {
        try
        {
            var achievement = await _repository.GetByIdAsync(id);
            if (achievement is null)
                return ResponseDto<AchievementDto>.Fail("Achievement not found.");

            var dto = _mapper.Map<AchievementDto>(achievement);
            return ResponseDto<AchievementDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<AchievementDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<AchievementDto>> CreateAsync(AchievementDto dto)
    {
        try
        {
            var entity = _mapper.Map<Achievement>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<AchievementDto>(entity);
            return ResponseDto<AchievementDto>.Ok(resultDto, "Achievement created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<AchievementDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<AchievementDto>> UpdateAsync(AchievementDto dto)
    {
        try
        {
            var achievement = await _repository.GetByIdAsync(dto.Id);
            if (achievement is null)
                return ResponseDto<AchievementDto>.Fail("Achievement not found.");

            _mapper.Map(dto, achievement);
            _repository.Update(achievement);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<AchievementDto>(achievement);
            return ResponseDto<AchievementDto>.Ok(resultDto, "Achievement updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<AchievementDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var achievement = await _repository.GetByIdAsync(id);
            if (achievement is null)
                return ResponseDto<bool>.Fail("Achievement not found.");

            _repository.Delete(achievement);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Achievement deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }
}
