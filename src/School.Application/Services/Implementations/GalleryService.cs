using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Gallery;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class GalleryService : IGalleryService
{
    private readonly IRepository<GalleryAlbum> _albumRepo;
    private readonly IRepository<GalleryImage> _imageRepo;
    private readonly IRepository<GalleryVideo> _videoRepo;
    private readonly IMapper _mapper;

    public GalleryService(
        IRepository<GalleryAlbum> albumRepo,
        IRepository<GalleryImage> imageRepo,
        IRepository<GalleryVideo> videoRepo,
        IMapper mapper)
    {
        _albumRepo = albumRepo;
        _imageRepo = imageRepo;
        _videoRepo = videoRepo;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<GalleryAlbumDto>>> GetAllAlbumsAsync()
    {
        try
        {
            var albums = await _albumRepo.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<GalleryAlbumDto>>(albums.OrderByDescending(a => a.CreatedAt));
            return ResponseDto<IEnumerable<GalleryAlbumDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<GalleryAlbumDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<GalleryAlbumDto>> GetAlbumByIdAsync(int id)
    {
        try
        {
            var album = await _albumRepo.GetByIdAsync(id);
            if (album is null)
                return ResponseDto<GalleryAlbumDto>.Fail("Album not found.");

            var dto = _mapper.Map<GalleryAlbumDto>(album);
            dto.ImageCount = album.GalleryImages?.Count ?? 0;
            dto.GalleryImages = _mapper.Map<List<GalleryImageDto>>(album.GalleryImages?.OrderBy(i => i.SortOrder));
            return ResponseDto<GalleryAlbumDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<GalleryAlbumDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<GalleryAlbumDto>> CreateAlbumAsync(GalleryAlbumCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<GalleryAlbum>(dto);
            await _albumRepo.AddAsync(entity);
            await _albumRepo.SaveChangesAsync();

            var resultDto = _mapper.Map<GalleryAlbumDto>(entity);
            return ResponseDto<GalleryAlbumDto>.Ok(resultDto, "Album created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<GalleryAlbumDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<GalleryAlbumDto>> UpdateAlbumAsync(GalleryAlbumUpdateDto dto)
    {
        try
        {
            var album = await _albumRepo.GetByIdAsync(dto.Id);
            if (album is null)
                return ResponseDto<GalleryAlbumDto>.Fail("Album not found.");

            _mapper.Map(dto, album);
            _albumRepo.Update(album);
            await _albumRepo.SaveChangesAsync();

            var resultDto = _mapper.Map<GalleryAlbumDto>(album);
            return ResponseDto<GalleryAlbumDto>.Ok(resultDto, "Album updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<GalleryAlbumDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAlbumAsync(int id)
    {
        try
        {
            var album = await _albumRepo.GetByIdAsync(id);
            if (album is null)
                return ResponseDto<bool>.Fail("Album not found.");

            _albumRepo.Delete(album);
            await _albumRepo.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Album deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<GalleryAlbumDto>>> GetAlbumsWithImagesAsync()
    {
        try
        {
            var albums = await _albumRepo.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<GalleryAlbumDto>>(albums.OrderByDescending(a => a.CreatedAt));
            foreach (var dto in dtos)
            {
                var images = await _imageRepo.FindAsync(i => i.GalleryAlbumId == dto.Id);
                dto.ImageCount = images.Count();
                dto.GalleryImages = _mapper.Map<List<GalleryImageDto>>(images.OrderBy(i => i.SortOrder));
            }
            return ResponseDto<IEnumerable<GalleryAlbumDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<GalleryAlbumDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<GalleryImageDto>>> GetImagesByAlbumIdAsync(int albumId)
    {
        try
        {
            var images = await _imageRepo.FindAsync(i => i.GalleryAlbumId == albumId);
            var dtos = _mapper.Map<IEnumerable<GalleryImageDto>>(images.OrderBy(i => i.SortOrder));
            return ResponseDto<IEnumerable<GalleryImageDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<GalleryImageDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<GalleryImageDto>> CreateImageAsync(GalleryImageCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<GalleryImage>(dto);
            await _imageRepo.AddAsync(entity);
            await _imageRepo.SaveChangesAsync();

            var resultDto = _mapper.Map<GalleryImageDto>(entity);
            return ResponseDto<GalleryImageDto>.Ok(resultDto, "Image added successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<GalleryImageDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteImageAsync(int id)
    {
        try
        {
            var image = await _imageRepo.GetByIdAsync(id);
            if (image is null)
                return ResponseDto<bool>.Fail("Image not found.");

            _imageRepo.Delete(image);
            await _imageRepo.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Image deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<GalleryVideoDto>>> GetAllVideosAsync()
    {
        try
        {
            var videos = await _videoRepo.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<GalleryVideoDto>>(videos.OrderBy(v => v.SortOrder));
            return ResponseDto<IEnumerable<GalleryVideoDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<GalleryVideoDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<GalleryVideoDto>> GetVideoByIdAsync(int id)
    {
        try
        {
            var video = await _videoRepo.GetByIdAsync(id);
            if (video is null)
                return ResponseDto<GalleryVideoDto>.Fail("Video not found.");

            var dto = _mapper.Map<GalleryVideoDto>(video);
            return ResponseDto<GalleryVideoDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<GalleryVideoDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<GalleryVideoDto>> CreateVideoAsync(GalleryVideoCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<GalleryVideo>(dto);
            await _videoRepo.AddAsync(entity);
            await _videoRepo.SaveChangesAsync();

            var resultDto = _mapper.Map<GalleryVideoDto>(entity);
            return ResponseDto<GalleryVideoDto>.Ok(resultDto, "Video added successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<GalleryVideoDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<GalleryVideoDto>> UpdateVideoAsync(GalleryVideoUpdateDto dto)
    {
        try
        {
            var video = await _videoRepo.GetByIdAsync(dto.Id);
            if (video is null)
                return ResponseDto<GalleryVideoDto>.Fail("Video not found.");

            _mapper.Map(dto, video);
            _videoRepo.Update(video);
            await _videoRepo.SaveChangesAsync();

            var resultDto = _mapper.Map<GalleryVideoDto>(video);
            return ResponseDto<GalleryVideoDto>.Ok(resultDto, "Video updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<GalleryVideoDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteVideoAsync(int id)
    {
        try
        {
            var video = await _videoRepo.GetByIdAsync(id);
            if (video is null)
                return ResponseDto<bool>.Fail("Video not found.");

            _videoRepo.Delete(video);
            await _videoRepo.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Video deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }
}
