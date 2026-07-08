using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Teacher;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class TeacherService : ITeacherService
{
    private readonly IRepository<Teacher> _repository;
    private readonly IMapper _mapper;

    public TeacherService(IRepository<Teacher> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<TeacherDto>>> GetAllAsync()
    {
        try
        {
            var teachers = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<TeacherDto>>(teachers.OrderBy(t => t.SortOrder));
            return ResponseDto<IEnumerable<TeacherDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<TeacherDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<TeacherDto>> GetByIdAsync(int id)
    {
        try
        {
            var teacher = await _repository.GetByIdAsync(id);
            if (teacher is null)
                return ResponseDto<TeacherDto>.Fail("Teacher not found.");

            var dto = _mapper.Map<TeacherDto>(teacher);
            return ResponseDto<TeacherDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<TeacherDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<TeacherDto>> CreateAsync(TeacherCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Teacher>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<TeacherDto>(entity);
            return ResponseDto<TeacherDto>.Ok(resultDto, "Teacher created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<TeacherDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<TeacherDto>> UpdateAsync(TeacherUpdateDto dto)
    {
        try
        {
            var teacher = await _repository.GetByIdAsync(dto.Id);
            if (teacher is null)
                return ResponseDto<TeacherDto>.Fail("Teacher not found.");

            _mapper.Map(dto, teacher);
            _repository.Update(teacher);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<TeacherDto>(teacher);
            return ResponseDto<TeacherDto>.Ok(resultDto, "Teacher updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<TeacherDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var teacher = await _repository.GetByIdAsync(id);
            if (teacher is null)
                return ResponseDto<bool>.Fail("Teacher not found.");

            _repository.Delete(teacher);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Teacher deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<TeacherDto>>> GetActiveTeachersAsync()
    {
        try
        {
            var teachers = await _repository.FindAsync(t => t.IsActive);
            var dtos = _mapper.Map<IEnumerable<TeacherDto>>(teachers.OrderBy(t => t.SortOrder));
            return ResponseDto<IEnumerable<TeacherDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<TeacherDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<TeacherDto?>> GetHeadTeacherAsync()
    {
        try
        {
            var teachers = await _repository.FindAsync(t => t.IsHeadTeacher && t.IsActive);
            var teacher = teachers.FirstOrDefault();
            if (teacher is null)
                return ResponseDto<TeacherDto?>.Ok(null, "No head teacher found.");

            var dto = _mapper.Map<TeacherDto>(teacher);
            return ResponseDto<TeacherDto?>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<TeacherDto?>.Fail(ex);
        }
    }
}
