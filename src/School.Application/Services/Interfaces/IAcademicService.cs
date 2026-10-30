using School.Application.DTOs.Academic;
using School.Application.DTOs.Common;

namespace School.Application.Services.Interfaces;

/// <summary>Academic calendar, subjects, syllabus files and class routines.</summary>
public interface IAcademicService
{
    Task<ResponseDto<IEnumerable<AcademicCalendarDto>>> GetCalendarEventsAsync(bool activeOnly = false);
    Task<ResponseDto<AcademicCalendarDto>> GetCalendarEventByIdAsync(int id);
    Task<ResponseDto<AcademicCalendarDto>> CreateCalendarEventAsync(AcademicCalendarCreateDto dto);
    Task<ResponseDto<AcademicCalendarDto>> UpdateCalendarEventAsync(AcademicCalendarUpdateDto dto);
    Task<ResponseDto<bool>> DeleteCalendarEventAsync(int id);

    Task<ResponseDto<IEnumerable<SubjectDto>>> GetSubjectsAsync(bool activeOnly = false);
    Task<ResponseDto<SubjectDto>> GetSubjectByIdAsync(int id);
    Task<ResponseDto<SubjectDto>> CreateSubjectAsync(SubjectCreateDto dto);
    Task<ResponseDto<SubjectDto>> UpdateSubjectAsync(SubjectUpdateDto dto);
    Task<ResponseDto<bool>> DeleteSubjectAsync(int id);

    Task<ResponseDto<IEnumerable<SyllabusDto>>> GetSyllabusesAsync(bool activeOnly = false);
    Task<ResponseDto<SyllabusDto>> GetSyllabusByIdAsync(int id);
    Task<ResponseDto<SyllabusDto>> CreateSyllabusAsync(SyllabusCreateDto dto);
    Task<ResponseDto<SyllabusDto>> UpdateSyllabusAsync(SyllabusUpdateDto dto);
    Task<ResponseDto<bool>> DeleteSyllabusAsync(int id);

    Task<ResponseDto<IEnumerable<RoutineDto>>> GetRoutinesAsync(bool activeOnly = false);
    Task<ResponseDto<RoutineDto>> GetRoutineByIdAsync(int id);
    Task<ResponseDto<RoutineDto>> CreateRoutineAsync(RoutineCreateDto dto);
    Task<ResponseDto<RoutineDto>> UpdateRoutineAsync(RoutineUpdateDto dto);
    Task<ResponseDto<bool>> DeleteRoutineAsync(int id);
}
