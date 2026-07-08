using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Download;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Enums;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class DownloadService : IDownloadService
{
    private readonly IRepository<Download> _repository;
    private readonly IMapper _mapper;

    public DownloadService(IRepository<Download> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<DownloadDto>>> GetAllAsync()
    {
        try
        {
            var downloads = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<DownloadDto>>(downloads.OrderByDescending(d => d.CreatedAt));
            return ResponseDto<IEnumerable<DownloadDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<DownloadDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<DownloadDto>> GetByIdAsync(int id)
    {
        try
        {
            var download = await _repository.GetByIdAsync(id);
            if (download is null)
                return ResponseDto<DownloadDto>.Fail("Download not found.");

            var dto = _mapper.Map<DownloadDto>(download);
            return ResponseDto<DownloadDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<DownloadDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<DownloadDto>> CreateAsync(DownloadCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Download>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<DownloadDto>(entity);
            return ResponseDto<DownloadDto>.Ok(resultDto, "Download created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<DownloadDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<DownloadDto>> UpdateAsync(DownloadUpdateDto dto)
    {
        try
        {
            var download = await _repository.GetByIdAsync(dto.Id);
            if (download is null)
                return ResponseDto<DownloadDto>.Fail("Download not found.");

            _mapper.Map(dto, download);
            _repository.Update(download);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<DownloadDto>(download);
            return ResponseDto<DownloadDto>.Ok(resultDto, "Download updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<DownloadDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var download = await _repository.GetByIdAsync(id);
            if (download is null)
                return ResponseDto<bool>.Fail("Download not found.");

            _repository.Delete(download);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Download deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<DownloadDto>>> GetDownloadsByCategoryAsync(DownloadCategory category)
    {
        try
        {
            var downloads = await _repository.FindAsync(d => d.DownloadCategory == category && d.IsActive);
            var dtos = _mapper.Map<IEnumerable<DownloadDto>>(downloads.OrderByDescending(d => d.CreatedAt));
            return ResponseDto<IEnumerable<DownloadDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<DownloadDto>>.Fail(ex);
        }
    }
}
