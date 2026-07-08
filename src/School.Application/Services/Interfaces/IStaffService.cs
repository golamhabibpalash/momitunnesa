using School.Application.DTOs.Common;
using School.Application.DTOs.Staff;

namespace School.Application.Services.Interfaces;

public interface IStaffService
{
    Task<ResponseDto<IEnumerable<StaffDto>>> GetAllAsync();
    Task<ResponseDto<StaffDto>> GetByIdAsync(int id);
    Task<ResponseDto<StaffDto>> CreateAsync(StaffCreateDto dto);
    Task<ResponseDto<StaffDto>> UpdateAsync(StaffUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<IEnumerable<StaffDto>>> GetActiveStaffAsync();
}
