using School.Application.DTOs.Common;
using School.Application.DTOs.Dashboard;

namespace School.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<ResponseDto<DashboardDto>> GetDashboardAsync();
}
