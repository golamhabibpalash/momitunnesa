using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Enums;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class MenuService : IMenuService
{
    private readonly IRepository<Menu> _repository;
    private readonly IMapper _mapper;

    public MenuService(IRepository<Menu> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<MenuDto>>> GetAllAsync()
    {
        try
        {
            var menus = await _repository.GetAllAsync();
            var hierarchy = BuildHierarchy(menus.Where(m => m.ParentMenuId == null).OrderBy(m => m.SortOrder));
            return ResponseDto<IEnumerable<MenuDto>>.Ok(hierarchy);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<MenuDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<MenuDto>> GetByIdAsync(int id)
    {
        try
        {
            var menu = await _repository.GetByIdAsync(id);
            if (menu is null)
                return ResponseDto<MenuDto>.Fail("Menu not found.");

            var dto = _mapper.Map<MenuDto>(menu);
            var children = await _repository.FindAsync(m => m.ParentMenuId == id);
            dto.Children = _mapper.Map<List<MenuDto>>(children.OrderBy(m => m.SortOrder));
            return ResponseDto<MenuDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<MenuDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<MenuDto>> CreateAsync(MenuCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Menu>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<MenuDto>(entity);
            return ResponseDto<MenuDto>.Ok(resultDto, "Menu created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<MenuDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<MenuDto>> UpdateAsync(MenuUpdateDto dto)
    {
        try
        {
            var menu = await _repository.GetByIdAsync(dto.Id);
            if (menu is null)
                return ResponseDto<MenuDto>.Fail("Menu not found.");

            _mapper.Map(dto, menu);
            _repository.Update(menu);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<MenuDto>(menu);
            return ResponseDto<MenuDto>.Ok(resultDto, "Menu updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<MenuDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var menu = await _repository.GetByIdAsync(id);
            if (menu is null)
                return ResponseDto<bool>.Fail("Menu not found.");

            var children = await _repository.FindAsync(m => m.ParentMenuId == id);
            foreach (var child in children)
                _repository.Delete(child);

            _repository.Delete(menu);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Menu deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<MenuDto>>> GetMenusByLocationAsync(MenuLocation location)
    {
        try
        {
            var menus = await _repository.FindAsync(m => m.MenuLocation == location && m.IsActive);
            var hierarchy = BuildHierarchy(menus.Where(m => m.ParentMenuId == null).OrderBy(m => m.SortOrder));
            return ResponseDto<IEnumerable<MenuDto>>.Ok(hierarchy);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<MenuDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<MenuDto>>> GetNavigationMenuAsync()
    {
        try
        {
            var menus = await _repository.FindAsync(m => m.IsActive);
            var hierarchy = BuildHierarchy(menus.Where(m => m.ParentMenuId == null).OrderBy(m => m.SortOrder));
            return ResponseDto<IEnumerable<MenuDto>>.Ok(hierarchy);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<MenuDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> UpdateSortOrderAsync(List<MenuSortOrderDto> items)
    {
        try
        {
            foreach (var item in items)
            {
                var menu = await _repository.GetByIdAsync(item.Id);
                if (menu is null) continue;
                menu.SortOrder = item.SortOrder;
                menu.ParentMenuId = item.ParentMenuId;
                _repository.Update(menu);
            }
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Menu order updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    private List<MenuDto> BuildHierarchy(IEnumerable<Menu> parentMenus)
    {
        var dtos = _mapper.Map<List<MenuDto>>(parentMenus);
        foreach (var dto in dtos)
        {
            var children = _repository.FindAsync(m => m.ParentMenuId == dto.Id).Result;
            if (children.Any())
            {
                dto.Children = _mapper.Map<List<MenuDto>>(children.OrderBy(m => m.SortOrder));
                PopulateChildren(dto.Children);
            }
        }
        return dtos;
    }

    private void PopulateChildren(List<MenuDto> menus)
    {
        foreach (var menu in menus)
        {
            var children = _repository.FindAsync(m => m.ParentMenuId == menu.Id).Result;
            if (children.Any())
            {
                menu.Children = _mapper.Map<List<MenuDto>>(children.OrderBy(m => m.SortOrder));
                PopulateChildren(menu.Children);
            }
        }
    }
}
