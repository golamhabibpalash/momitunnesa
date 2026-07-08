using School.Application.DTOs.Common;
using School.Application.DTOs.Teacher;

namespace School.Application.Services.Interfaces;

public interface ITeacherService
{
    Task<ResponseDto<IEnumerable<TeacherDto>>> GetAllAsync();
    Task<ResponseDto<TeacherDto>> GetByIdAsync(int id);
    Task<ResponseDto<TeacherDto>> CreateAsync(TeacherCreateDto dto);
    Task<ResponseDto<TeacherDto>> UpdateAsync(TeacherUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<IEnumerable<TeacherDto>>> GetActiveTeachersAsync();
    Task<ResponseDto<TeacherDto?>> GetHeadTeacherAsync();
}
