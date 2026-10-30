using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using School.Application.DTOs.Account;
using School.Application.DTOs.Common;
using School.Application.Services.Interfaces;
using School.Domain.Entities;

namespace School.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<UserDto>>> GetAllAsync()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            var dtos = new List<UserDto>();
            foreach (var user in users)
            {
                var dto = _mapper.Map<UserDto>(user);
                dto.Roles = await _userManager.GetRolesAsync(user);
                dtos.Add(dto);
            }
            return ResponseDto<IEnumerable<UserDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<UserDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<UserDto>> GetByIdAsync(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return ResponseDto<UserDto>.Fail("User not found.");

            var dto = _mapper.Map<UserDto>(user);
            dto.Roles = await _userManager.GetRolesAsync(user);
            return ResponseDto<UserDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<UserDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<UserDto>> GetUserWithRolesAsync(string id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<ResponseDto<UserDto>> CreateAsync(UserCreateDto dto)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser is not null)
                return ResponseDto<UserDto>.Fail("A user with this email already exists.");

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.Phone,
                IsActive = dto.IsActive
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return ResponseDto<UserDto>.Fail("Failed to create user.",
                    result.Errors.Select(e => e.Description).ToList());

            if (dto.Roles.Any())
            {
                var roleResult = await _userManager.AddToRolesAsync(user, dto.Roles);
                if (!roleResult.Succeeded)
                {
                    // Don't leave a half-configured account behind.
                    await _userManager.DeleteAsync(user);
                    return ResponseDto<UserDto>.Fail("Failed to assign roles.",
                        roleResult.Errors.Select(e => e.Description).ToList());
                }
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);
            return ResponseDto<UserDto>.Ok(userDto, "User created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<UserDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<UserDto>> UpdateAsync(string id, UserUpdateDto dto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return ResponseDto<UserDto>.Fail("User not found.");

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.Phone;
            user.IsActive = dto.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ResponseDto<UserDto>.Fail("Failed to update user.",
                    updateResult.Errors.Select(e => e.Description).ToList());

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (dto.Roles.Any())
                await _userManager.AddToRolesAsync(user, dto.Roles);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);
            return ResponseDto<UserDto>.Ok(userDto, "User updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<UserDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return ResponseDto<bool>.Fail("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return ResponseDto<bool>.Fail("Failed to delete user.",
                    result.Errors.Select(e => e.Description).ToList());

            return ResponseDto<bool>.Ok(true, "User deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }
}
