using System.Text.RegularExpressions;
using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Page;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class PageService : IPageService
{
    private readonly IRepository<Page> _repository;
    private readonly IMapper _mapper;

    public PageService(IRepository<Page> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<PageDto>>> GetAllAsync()
    {
        try
        {
            var pages = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<PageDto>>(pages.OrderBy(p => p.SortOrder));
            return ResponseDto<IEnumerable<PageDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<PageDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<PageDto>> GetByIdAsync(int id)
    {
        try
        {
            var page = await _repository.GetByIdAsync(id);
            if (page is null)
                return ResponseDto<PageDto>.Fail("Page not found.");

            var dto = _mapper.Map<PageDto>(page);
            return ResponseDto<PageDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<PageDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<PageDto>> CreateAsync(PageCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Page>(dto);
            entity.Slug = GenerateSlug(dto.TitleEn ?? dto.TitleBn ?? "page");
            if (dto.IsPublished)
                entity.PublishedAt = DateTime.UtcNow;

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<PageDto>(entity);
            return ResponseDto<PageDto>.Ok(resultDto, "Page created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<PageDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<PageDto>> UpdateAsync(PageUpdateDto dto)
    {
        try
        {
            var page = await _repository.GetByIdAsync(dto.Id);
            if (page is null)
                return ResponseDto<PageDto>.Fail("Page not found.");

            _mapper.Map(dto, page);
            page.Slug = GenerateSlug(dto.TitleEn ?? dto.TitleBn ?? "page");
            if (dto.IsPublished && !page.PublishedAt.HasValue)
                page.PublishedAt = DateTime.UtcNow;

            _repository.Update(page);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<PageDto>(page);
            return ResponseDto<PageDto>.Ok(resultDto, "Page updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<PageDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var page = await _repository.GetByIdAsync(id);
            if (page is null)
                return ResponseDto<bool>.Fail("Page not found.");

            _repository.Delete(page);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Page deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<PageDto>> GetBySlugAsync(string slug)
    {
        try
        {
            var pages = await _repository.FindAsync(p => p.Slug == slug);
            var page = pages.FirstOrDefault();
            if (page is null)
                return ResponseDto<PageDto>.Fail("Page not found.");

            var dto = _mapper.Map<PageDto>(page);
            return ResponseDto<PageDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<PageDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<PageDto>>> GetPublishedPagesAsync()
    {
        try
        {
            var pages = await _repository.FindAsync(p => p.IsPublished && p.Status);
            var dtos = _mapper.Map<IEnumerable<PageDto>>(pages.OrderBy(p => p.SortOrder));
            return ResponseDto<IEnumerable<PageDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<PageDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<PageDto>> GetHomePageAsync()
    {
        try
        {
            var pages = await _repository.FindAsync(p => p.IsHomePage && p.IsPublished && p.Status);
            var page = pages.FirstOrDefault();
            if (page is null)
                return ResponseDto<PageDto>.Fail("Home page not found.");

            var dto = _mapper.Map<PageDto>(page);
            return ResponseDto<PageDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<PageDto>.Fail(ex);
        }
    }

    private static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "page";

        text = text.ToLowerInvariant().Trim();
        text = Regex.Replace(text, @"[^a-z0-9\u0980-\u09FF\s-]", "");
        text = Regex.Replace(text, @"\s+", "-");
        text = Regex.Replace(text, @"-+", "-");
        text = text.Trim('-');
        return text.Length > 200 ? text[..200] : text;
    }
}
