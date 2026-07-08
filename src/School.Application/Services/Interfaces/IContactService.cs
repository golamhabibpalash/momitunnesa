using School.Application.DTOs.Common;
using School.Application.DTOs.Contact;

namespace School.Application.Services.Interfaces;

public interface IContactService
{
    Task<ResponseDto<ContactMessageDto>> CreateAsync(ContactCreateDto dto);
    Task<ResponseDto<IEnumerable<ContactMessageDto>>> GetAllMessagesAsync();
    Task<ResponseDto<ContactMessageDto>> GetMessageByIdAsync(int id);
    Task<ResponseDto<bool>> MarkAsReadAsync(int id);
    Task<ResponseDto<bool>> DeleteAsync(int id);
}
