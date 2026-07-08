using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using School.Application.DTOs.Account;
using School.Application.DTOs.Common;
using School.Application.Services.Interfaces;

namespace School.Application.Services.Implementations;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<RoleDto>>> GetAllAsync()
    {
        try
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return ResponseDto<IEnumerable<RoleDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<RoleDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<RoleDto>> GetByIdAsync(string id)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return ResponseDto<RoleDto>.Fail("Role not found.");

            var dto = _mapper.Map<RoleDto>(role);
            return ResponseDto<RoleDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<RoleDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<RoleDto>> CreateAsync(RoleCreateDto dto)
    {
        try
        {
            var existing = await _roleManager.RoleExistsAsync(dto.Name);
            if (existing)
                return ResponseDto<RoleDto>.Fail("Role already exists.");

            var role = new IdentityRole(dto.Name);
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return ResponseDto<RoleDto>.Fail("Failed to create role.",
                    result.Errors.Select(e => e.Description).ToList());

            var roleDto = _mapper.Map<RoleDto>(role);
            return ResponseDto<RoleDto>.Ok(roleDto, "Role created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<RoleDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(string id)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return ResponseDto<bool>.Fail("Role not found.");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return ResponseDto<bool>.Fail("Failed to delete role.",
                    result.Errors.Select(e => e.Description).ToList());

            return ResponseDto<bool>.Ok(true, "Role deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }
}
