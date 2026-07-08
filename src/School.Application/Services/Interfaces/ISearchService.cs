using School.Application.DTOs.Common;
using School.Application.DTOs.Search;

namespace School.Application.Services.Interfaces;

public interface ISearchService
{
    Task<ResponseDto<IEnumerable<SearchResultDto>>> SearchAsync(string query);
}
