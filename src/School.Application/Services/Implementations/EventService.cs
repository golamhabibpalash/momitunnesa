using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Event;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class EventService : IEventService
{
    private readonly IRepository<Event> _repository;
    private readonly IMapper _mapper;

    public EventService(IRepository<Event> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<EventDto>>> GetAllAsync()
    {
        try
        {
            var events = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<EventDto>>(events.OrderBy(e => e.EventDate));
            return ResponseDto<IEnumerable<EventDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<EventDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<EventDto>> GetByIdAsync(int id)
    {
        try
        {
            var evt = await _repository.GetByIdAsync(id);
            if (evt is null)
                return ResponseDto<EventDto>.Fail("Event not found.");

            var dto = _mapper.Map<EventDto>(evt);
            return ResponseDto<EventDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<EventDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<EventDto>> CreateAsync(EventCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Event>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<EventDto>(entity);
            return ResponseDto<EventDto>.Ok(resultDto, "Event created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<EventDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<EventDto>> UpdateAsync(EventUpdateDto dto)
    {
        try
        {
            var evt = await _repository.GetByIdAsync(dto.Id);
            if (evt is null)
                return ResponseDto<EventDto>.Fail("Event not found.");

            _mapper.Map(dto, evt);
            _repository.Update(evt);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<EventDto>(evt);
            return ResponseDto<EventDto>.Ok(resultDto, "Event updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<EventDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var evt = await _repository.GetByIdAsync(id);
            if (evt is null)
                return ResponseDto<bool>.Fail("Event not found.");

            _repository.Delete(evt);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Event deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<EventDto>>> GetUpcomingEventsAsync()
    {
        try
        {
            var allEvents = await _repository.FindAsync(e => e.IsActive && e.EventDate >= DateTime.UtcNow);
            var dtos = _mapper.Map<IEnumerable<EventDto>>(allEvents.OrderBy(e => e.EventDate));
            return ResponseDto<IEnumerable<EventDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<EventDto>>.Fail(ex);
        }
    }
}
