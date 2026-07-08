using School.Application.DTOs.Common;
using School.Application.DTOs.Event;

namespace School.Application.Services.Interfaces;

public interface IEventService
{
    Task<ResponseDto<IEnumerable<EventDto>>> GetAllAsync();
    Task<ResponseDto<EventDto>> GetByIdAsync(int id);
    Task<ResponseDto<EventDto>> CreateAsync(EventCreateDto dto);
    Task<ResponseDto<EventDto>> UpdateAsync(EventUpdateDto dto);
    Task<ResponseDto<bool>> DeleteAsync(int id);
    Task<ResponseDto<IEnumerable<EventDto>>> GetUpcomingEventsAsync();
}
