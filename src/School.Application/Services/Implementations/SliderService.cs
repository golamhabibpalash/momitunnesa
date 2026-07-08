using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Slider;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class SliderService : ISliderService
{
    private readonly IRepository<Slider> _repository;
    private readonly IMapper _mapper;

    public SliderService(IRepository<Slider> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<SliderDto>>> GetAllAsync()
    {
        try
        {
            var sliders = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<SliderDto>>(sliders.OrderBy(s => s.SortOrder));
            return ResponseDto<IEnumerable<SliderDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<SliderDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<SliderDto>> GetByIdAsync(int id)
    {
        try
        {
            var slider = await _repository.GetByIdAsync(id);
            if (slider is null)
                return ResponseDto<SliderDto>.Fail("Slider not found.");

            var dto = _mapper.Map<SliderDto>(slider);
            return ResponseDto<SliderDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<SliderDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<SliderDto>> CreateAsync(SliderCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Slider>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<SliderDto>(entity);
            return ResponseDto<SliderDto>.Ok(resultDto, "Slider created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<SliderDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<SliderDto>> UpdateAsync(SliderUpdateDto dto)
    {
        try
        {
            var slider = await _repository.GetByIdAsync(dto.Id);
            if (slider is null)
                return ResponseDto<SliderDto>.Fail("Slider not found.");

            _mapper.Map(dto, slider);
            _repository.Update(slider);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<SliderDto>(slider);
            return ResponseDto<SliderDto>.Ok(resultDto, "Slider updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<SliderDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var slider = await _repository.GetByIdAsync(id);
            if (slider is null)
                return ResponseDto<bool>.Fail("Slider not found.");

            _repository.Delete(slider);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Slider deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<SliderDto>>> GetActiveSlidersAsync()
    {
        try
        {
            var sliders = await _repository.GetActiveAsync();
            var dtos = _mapper.Map<IEnumerable<SliderDto>>(sliders.OrderBy(s => s.SortOrder));
            return ResponseDto<IEnumerable<SliderDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<SliderDto>>.Fail(ex);
        }
    }
}
