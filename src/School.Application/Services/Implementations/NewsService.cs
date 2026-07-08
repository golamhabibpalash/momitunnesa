using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.News;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class NewsService : INewsService
{
    private readonly IRepository<News> _repository;
    private readonly IMapper _mapper;

    public NewsService(IRepository<News> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<NewsDto>>> GetAllAsync()
    {
        try
        {
            var newsList = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<NewsDto>>(newsList.OrderByDescending(n => n.PublishDate));
            return ResponseDto<IEnumerable<NewsDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<NewsDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<NewsDto>> GetByIdAsync(int id)
    {
        try
        {
            var news = await _repository.GetByIdAsync(id);
            if (news is null)
                return ResponseDto<NewsDto>.Fail("News not found.");

            var dto = _mapper.Map<NewsDto>(news);
            return ResponseDto<NewsDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<NewsDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<NewsDto>> CreateAsync(NewsCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<News>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<NewsDto>(entity);
            return ResponseDto<NewsDto>.Ok(resultDto, "News created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<NewsDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<NewsDto>> UpdateAsync(NewsUpdateDto dto)
    {
        try
        {
            var news = await _repository.GetByIdAsync(dto.Id);
            if (news is null)
                return ResponseDto<NewsDto>.Fail("News not found.");

            _mapper.Map(dto, news);
            _repository.Update(news);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<NewsDto>(news);
            return ResponseDto<NewsDto>.Ok(resultDto, "News updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<NewsDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var news = await _repository.GetByIdAsync(id);
            if (news is null)
                return ResponseDto<bool>.Fail("News not found.");

            _repository.Delete(news);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "News deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<NewsDto>>> GetActiveNewsAsync()
    {
        try
        {
            var newsList = await _repository.FindAsync(n => n.IsActive);
            var dtos = _mapper.Map<IEnumerable<NewsDto>>(newsList.OrderByDescending(n => n.PublishDate));
            return ResponseDto<IEnumerable<NewsDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<NewsDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<NewsDto>>> GetFeaturedNewsAsync()
    {
        try
        {
            var newsList = await _repository.FindAsync(n => n.IsFeatured && n.IsActive);
            var dtos = _mapper.Map<IEnumerable<NewsDto>>(newsList.OrderByDescending(n => n.PublishDate));
            return ResponseDto<IEnumerable<NewsDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<NewsDto>>.Fail(ex);
        }
    }
}
