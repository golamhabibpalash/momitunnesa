using School.Application.DTOs.Account;
using School.Application.DTOs.Common;

namespace School.Application.Services.Interfaces;

public interface IUserService
{
    Task<ResponseDto<IEnumerable<UserDto>>> GetAllAsync();
    Task<ResponseDto<UserDto>> GetByIdAsync(string id);
    Task<ResponseDto<UserDto>> GetUserWithRolesAsync(string id);
    Task<ResponseDto<UserDto>> CreateAsync(UserCreateDto dto);
    Task<ResponseDto<UserDto>> UpdateAsync(string id, UserUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(string id);
}
