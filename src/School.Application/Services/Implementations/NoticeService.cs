using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Notice;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Enums;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class NoticeService : INoticeService
{
    private readonly IRepository<Notice> _repository;
    private readonly IMapper _mapper;

    public NoticeService(IRepository<Notice> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<NoticeDto>>> GetAllAsync()
    {
        try
        {
            var notices = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<NoticeDto>>(notices.OrderByDescending(n => n.PublishDate));
            return ResponseDto<IEnumerable<NoticeDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<NoticeDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<NoticeDto>> GetByIdAsync(int id)
    {
        try
        {
            var notice = await _repository.GetByIdAsync(id);
            if (notice is null)
                return ResponseDto<NoticeDto>.Fail("Notice not found.");

            var dto = _mapper.Map<NoticeDto>(notice);
            return ResponseDto<NoticeDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoticeDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<NoticeDto>> CreateAsync(NoticeCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Notice>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<NoticeDto>(entity);
            return ResponseDto<NoticeDto>.Ok(resultDto, "Notice created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<NoticeDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<NoticeDto>> UpdateAsync(NoticeUpdateDto dto)
    {
        try
        {
            var notice = await _repository.GetByIdAsync(dto.Id);
            if (notice is null)
                return ResponseDto<NoticeDto>.Fail("Notice not found.");

            _mapper.Map(dto, notice);
            _repository.Update(notice);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<NoticeDto>(notice);
            return ResponseDto<NoticeDto>.Ok(resultDto, "Notice updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<NoticeDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var notice = await _repository.GetByIdAsync(id);
            if (notice is null)
                return ResponseDto<bool>.Fail("Notice not found.");

            _repository.Delete(notice);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Notice deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<NoticeDto>>> GetActiveNoticesAsync()
    {
        try
        {
            var notices = await _repository.FindAsync(n => n.IsActive && (n.ExpiryDate == null || n.ExpiryDate >= DateTime.UtcNow));
            var dtos = _mapper.Map<IEnumerable<NoticeDto>>(notices.OrderByDescending(n => n.PublishDate));
            return ResponseDto<IEnumerable<NoticeDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<NoticeDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<NoticeDto>>> GetFeaturedNoticesAsync()
    {
        try
        {
            var notices = await _repository.FindAsync(n => n.IsFeatured && n.IsActive);
            var dtos = _mapper.Map<IEnumerable<NoticeDto>>(notices.OrderByDescending(n => n.PublishDate));
            return ResponseDto<IEnumerable<NoticeDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<NoticeDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<NoticeDto>>> GetNoticesByCategoryAsync(NoticeCategory category)
    {
        try
        {
            var notices = await _repository.FindAsync(n => n.NoticeCategory == category && n.IsActive);
            var dtos = _mapper.Map<IEnumerable<NoticeDto>>(notices.OrderByDescending(n => n.PublishDate));
            return ResponseDto<IEnumerable<NoticeDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<NoticeDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> IncrementViewCountAsync(int id)
    {
        try
        {
            var notice = await _repository.GetByIdAsync(id);
            if (notice is null)
                return ResponseDto<bool>.Fail("Notice not found.");

            notice.ViewCount++;
            _repository.Update(notice);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }
}
