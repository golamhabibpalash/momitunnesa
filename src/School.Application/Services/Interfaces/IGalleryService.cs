using School.Application.DTOs.Common;
using School.Application.DTOs.Gallery;

namespace School.Application.Services.Interfaces;

public interface IGalleryService
{
    Task<ResponseDto<IEnumerable<GalleryAlbumDto>>> GetAllAlbumsAsync();
    Task<ResponseDto<GalleryAlbumDto>> GetAlbumByIdAsync(int id);
    Task<ResponseDto<GalleryAlbumDto>> CreateAlbumAsync(GalleryAlbumCreateDto dto);
    Task<ResponseDto<GalleryAlbumDto>> UpdateAlbumAsync(GalleryAlbumUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAlbumAsync(int id);
    Task<ResponseDto<IEnumerable<GalleryAlbumDto>>> GetAlbumsWithImagesAsync();

    Task<ResponseDto<IEnumerable<GalleryImageDto>>> GetImagesByAlbumIdAsync(int albumId);
    Task<ResponseDto<GalleryImageDto>> CreateImageAsync(GalleryImageCreateDto dto);
    Task<ResponseDto<bool>> DeleteImageAsync(int id);

    Task<ResponseDto<IEnumerable<GalleryVideoDto>>> GetAllVideosAsync();
    Task<ResponseDto<GalleryVideoDto>> GetVideoByIdAsync(int id);
    Task<ResponseDto<GalleryVideoDto>> CreateVideoAsync(GalleryVideoCreateDto dto);
    Task<ResponseDto<GalleryVideoDto>> UpdateVideoAsync(GalleryVideoUpdateDto dto);
    Task<ResponseDto<bool>> DeleteVideoAsync(int id);
}
