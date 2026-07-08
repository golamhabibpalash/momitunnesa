using School.Application.DTOs.Common;
using School.Application.DTOs.Slider;

namespace School.Application.Services.Interfaces;

public interface ISliderService
{
    Task<ResponseDto<IEnumerable<SliderDto>>> GetAllAsync();
    Task<ResponseDto<SliderDto>> GetByIdAsync(int id);
    Task<ResponseDto<SliderDto>> CreateAsync(SliderCreateDto dto);
    Task<ResponseDto<SliderDto>> UpdateAsync(SliderUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<IEnumerable<SliderDto>>> GetActiveSlidersAsync();
}
