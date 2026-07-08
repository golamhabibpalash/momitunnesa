using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Contact;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class ContactService : IContactService
{
    private readonly IRepository<ContactMessage> _repository;
    private readonly IMapper _mapper;

    public ContactService(IRepository<ContactMessage> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<ContactMessageDto>> CreateAsync(ContactCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<ContactMessage>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = _mapper.Map<ContactMessageDto>(entity);
            return ResponseDto<ContactMessageDto>.Ok(resultDto, "Message sent successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<ContactMessageDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<IEnumerable<ContactMessageDto>>> GetAllMessagesAsync()
    {
        try
        {
            var messages = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<ContactMessageDto>>(messages.OrderByDescending(m => m.CreatedAt));
            return ResponseDto<IEnumerable<ContactMessageDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ContactMessageDto>>.Fail(ex);
        }
    }

    public async Task<ResponseDto<ContactMessageDto>> GetMessageByIdAsync(int id)
    {
        try
        {
            var message = await _repository.GetByIdAsync(id);
            if (message is null)
                return ResponseDto<ContactMessageDto>.Fail("Message not found.");

            var dto = _mapper.Map<ContactMessageDto>(message);
            return ResponseDto<ContactMessageDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return ResponseDto<ContactMessageDto>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> MarkAsReadAsync(int id)
    {
        try
        {
            var message = await _repository.GetByIdAsync(id);
            if (message is null)
                return ResponseDto<bool>.Fail("Message not found.");

            message.IsRead = true;
            _repository.Update(message);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Message marked as read.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }

    public async Task<ResponseDto<bool>> DeleteAsync(int id)
    {
        try
        {
            var message = await _repository.GetByIdAsync(id);
            if (message is null)
                return ResponseDto<bool>.Fail("Message not found.");

            _repository.Delete(message);
            await _repository.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Message deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }
}
