using AutoMapper;
using School.Application.DTOs.Academic;
using School.Application.DTOs.Common;
using School.Application.Services.Interfaces;
using School.Domain.Common;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class AcademicService : IAcademicService
{
    private readonly IRepository<AcademicCalendar> _calendarRepo;
    private readonly IRepository<Subject> _subjectRepo;
    private readonly IRepository<Syllabus> _syllabusRepo;
    private readonly IRepository<Routine> _routineRepo;
    private readonly IMapper _mapper;

    public AcademicService(
        IRepository<AcademicCalendar> calendarRepo,
        IRepository<Subject> subjectRepo,
        IRepository<Syllabus> syllabusRepo,
        IRepository<Routine> routineRepo,
        IMapper mapper)
    {
        _calendarRepo = calendarRepo;
        _subjectRepo = subjectRepo;
        _syllabusRepo = syllabusRepo;
        _routineRepo = routineRepo;
        _mapper = mapper;
    }

    // Academic calendar
    public Task<ResponseDto<IEnumerable<AcademicCalendarDto>>> GetCalendarEventsAsync(bool activeOnly = false) =>
        ListAsync<AcademicCalendar, AcademicCalendarDto>(_calendarRepo, activeOnly ? c => c.IsActive : null, q => q.OrderBy(c => c.EventDate));
    public Task<ResponseDto<AcademicCalendarDto>> GetCalendarEventByIdAsync(int id) => GetAsync<AcademicCalendar, AcademicCalendarDto>(_calendarRepo, id, "Calendar event");
    public Task<ResponseDto<AcademicCalendarDto>> CreateCalendarEventAsync(AcademicCalendarCreateDto dto) => CreateAsync<AcademicCalendar, AcademicCalendarDto>(_calendarRepo, dto);
    public Task<ResponseDto<AcademicCalendarDto>> UpdateCalendarEventAsync(AcademicCalendarUpdateDto dto) => UpdateAsync<AcademicCalendar, AcademicCalendarDto>(_calendarRepo, dto.Id, dto, "Calendar event");
    public Task<ResponseDto<bool>> DeleteCalendarEventAsync(int id) => DeleteAsync(_calendarRepo, id, "Calendar event");

    // Subjects
    public Task<ResponseDto<IEnumerable<SubjectDto>>> GetSubjectsAsync(bool activeOnly = false) =>
        ListAsync<Subject, SubjectDto>(_subjectRepo, activeOnly ? s => s.IsActive : null, q => q.OrderBy(s => s.ClassName).ThenBy(s => s.NameEn));
    public Task<ResponseDto<SubjectDto>> GetSubjectByIdAsync(int id) => GetAsync<Subject, SubjectDto>(_subjectRepo, id, "Subject");
    public Task<ResponseDto<SubjectDto>> CreateSubjectAsync(SubjectCreateDto dto) => CreateAsync<Subject, SubjectDto>(_subjectRepo, dto);
    public Task<ResponseDto<SubjectDto>> UpdateSubjectAsync(SubjectUpdateDto dto) => UpdateAsync<Subject, SubjectDto>(_subjectRepo, dto.Id, dto, "Subject");
    public Task<ResponseDto<bool>> DeleteSubjectAsync(int id) => DeleteAsync(_subjectRepo, id, "Subject");

    // Syllabus
    public Task<ResponseDto<IEnumerable<SyllabusDto>>> GetSyllabusesAsync(bool activeOnly = false) =>
        ListAsync<Syllabus, SyllabusDto>(_syllabusRepo, activeOnly ? s => s.IsActive : null, q => q.OrderBy(s => s.ClassName).ThenBy(s => s.TitleEn));
    public Task<ResponseDto<SyllabusDto>> GetSyllabusByIdAsync(int id) => GetAsync<Syllabus, SyllabusDto>(_syllabusRepo, id, "Syllabus");
    public Task<ResponseDto<SyllabusDto>> CreateSyllabusAsync(SyllabusCreateDto dto) => CreateAsync<Syllabus, SyllabusDto>(_syllabusRepo, dto);
    public Task<ResponseDto<SyllabusDto>> UpdateSyllabusAsync(SyllabusUpdateDto dto) => UpdateAsync<Syllabus, SyllabusDto>(_syllabusRepo, dto.Id, dto, "Syllabus");
    public Task<ResponseDto<bool>> DeleteSyllabusAsync(int id) => DeleteAsync(_syllabusRepo, id, "Syllabus");

    // Routine
    public Task<ResponseDto<IEnumerable<RoutineDto>>> GetRoutinesAsync(bool activeOnly = false) =>
        ListAsync<Routine, RoutineDto>(_routineRepo, activeOnly ? r => r.IsActive : null, q => q.OrderBy(r => r.ClassNameEn).ThenBy(r => r.Section).ThenBy(r => r.StartTime));
    public Task<ResponseDto<RoutineDto>> GetRoutineByIdAsync(int id) => GetAsync<Routine, RoutineDto>(_routineRepo, id, "Routine entry");
    public Task<ResponseDto<RoutineDto>> CreateRoutineAsync(RoutineCreateDto dto) => CreateAsync<Routine, RoutineDto>(_routineRepo, dto);
    public Task<ResponseDto<RoutineDto>> UpdateRoutineAsync(RoutineUpdateDto dto) => UpdateAsync<Routine, RoutineDto>(_routineRepo, dto.Id, dto, "Routine entry");
    public Task<ResponseDto<bool>> DeleteRoutineAsync(int id) => DeleteAsync(_routineRepo, id, "Routine entry");

    private async Task<ResponseDto<IEnumerable<TDto>>> ListAsync<TEntity, TDto>(
        IRepository<TEntity> repo,
        System.Linq.Expressions.Expression<Func<TEntity, bool>>? filter,
        Func<IEnumerable<TEntity>, IEnumerable<TEntity>> order) where TEntity : BaseEntity
    {
        try
        {
            var items = filter is null ? await repo.GetAllAsync() : await repo.FindAsync(filter);
            return ResponseDto<IEnumerable<TDto>>.Ok(_mapper.Map<List<TDto>>(order(items)));
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<TDto>>.Fail(ex);
        }
    }

    private async Task<ResponseDto<TDto>> GetAsync<TEntity, TDto>(IRepository<TEntity> repo, int id, string name) where TEntity : BaseEntity
    {
        try
        {
            var entity = await repo.GetByIdAsync(id);
            return entity is null
                ? ResponseDto<TDto>.Fail($"{name} not found.")
                : ResponseDto<TDto>.Ok(_mapper.Map<TDto>(entity));
        }
        catch (Exception ex)
        {
            return ResponseDto<TDto>.Fail(ex);
        }
    }

    private async Task<ResponseDto<TDto>> CreateAsync<TEntity, TDto>(IRepository<TEntity> repo, object dto) where TEntity : BaseEntity
    {
        try
        {
            var entity = _mapper.Map<TEntity>(dto);
            await repo.AddAsync(entity);
            await repo.SaveChangesAsync();
            return ResponseDto<TDto>.Ok(_mapper.Map<TDto>(entity), "Created successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<TDto>.Fail(ex);
        }
    }

    private async Task<ResponseDto<TDto>> UpdateAsync<TEntity, TDto>(IRepository<TEntity> repo, int id, object dto, string name) where TEntity : BaseEntity
    {
        try
        {
            var entity = await repo.GetByIdAsync(id);
            if (entity is null)
                return ResponseDto<TDto>.Fail($"{name} not found.");

            _mapper.Map(dto, entity, dto.GetType(), typeof(TEntity));
            repo.Update(entity);
            await repo.SaveChangesAsync();
            return ResponseDto<TDto>.Ok(_mapper.Map<TDto>(entity), "Updated successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<TDto>.Fail(ex);
        }
    }

    private static async Task<ResponseDto<bool>> DeleteAsync<TEntity>(IRepository<TEntity> repo, int id, string name) where TEntity : BaseEntity
    {
        try
        {
            var entity = await repo.GetByIdAsync(id);
            if (entity is null)
                return ResponseDto<bool>.Fail($"{name} not found.");

            repo.Delete(entity);
            await repo.SaveChangesAsync();
            return ResponseDto<bool>.Ok(true, "Deleted successfully.");
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail(ex);
        }
    }
}
