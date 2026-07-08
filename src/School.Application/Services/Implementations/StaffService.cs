using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Staff;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class StaffService : IStaffService
{
    private readonly IRepository<Staff> _repository;
    private readonly IMapper _mapper;

    public StaffService(IRepository<Staff> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<StaffDto>>> GetAllAsync()
    {
        try
        {
            var staffList = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<StaffDto>>(staffList.OrderBy(s => s.SortOrder));
            return ResponseDto<IEnumerable<StaffDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<StaffDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<StaffDto>> GetByIdAsync(int id)
    {
        try
        {
            var staff = await _repository.GetByIdAsync(id);
            if (staff is null)
                return ResponseDto<StaffDto>.Fail("Staff not found.");

            var dto = _mapper.Map<StaffDto>(staff);
            return ResponseDto<StaffDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<StaffDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<StaffDto>> CreateAsync(StaffCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Staff>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<StaffDto>(entity);
            return ResponseDto<StaffDto>.Ok(resultDto, "Staff created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<StaffDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<StaffDto>> UpdateAsync(StaffUpdateDto dto)
    {
        try
        {
            var staff = await _repository.GetByIdAsync(dto.Id);
            if (staff is null)
                return ResponseDto<StaffDto>.Fail("Staff not found.");

            _mapper.Map(dto, staff);
            _repository.Update(staff);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<StaffDto>(staff);
            return ResponseDto<StaffDto>.Ok(resultDto, "Staff updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<StaffDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var staff = await _repository.GetByIdAsync(id);
            if (staff is null)
                return ResponseDto<bool>.Fail("Staff not found.");

            _repository.Delete(staff);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Staff deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<StaffDto>>> GetActiveStaffAsync()
    {
        try
        {
            var staffList = await _repository.FindAsync(s => s.IsActive);
            var dtos = _mapper.Map<IEnumerable<StaffDto>>(staffList.OrderBy(s => s.SortOrder));
            return ResponseDto<IEnumerable<StaffDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<StaffDto>>.Fail(ex);
        }
    }
}
