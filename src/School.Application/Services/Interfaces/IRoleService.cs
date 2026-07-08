using School.Application.DTOs.Account;
using School.Application.DTOs.Common;

namespace School.Application.Services.Interfaces;

public interface IRoleService
{
    Task<ResponseDto<IEnumerable<RoleDto>>> GetAllAsync();
    Task<ResponseDto<RoleDto>> GetByIdAsync(string id);
    Task<ResponseDto<RoleDto>> CreateAsync(RoleCreateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(string id);
}
