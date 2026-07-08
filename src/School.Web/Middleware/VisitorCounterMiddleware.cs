using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;
using School.Infrastructure.Data;

namespace School.Web.Middleware;

public class VisitorCounterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<VisitorCounterMiddleware> _logger;

    public VisitorCounterMiddleware(RequestDelegate next, ILogger<VisitorCounterMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory)
    {
        if (!context.Items.ContainsKey("VisitorCounted"))
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (!IsAdminArea(path))
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var today = DateTime.UtcNow.Date;
                    var visitorIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var userAgent = context.Request.Headers.UserAgent.ToString();

                    var existing = await dbContext.VisitorCounters
                        .FirstOrDefaultAsync(v =>
                            v.Date == today &&
                            v.IPAddress == visitorIp &&
                            v.PageUrl == path &&
                            v.UserAgent == userAgent);

                    if (existing != null)
                    {
                        existing.Count++;
                        dbContext.VisitorCounters.Update(existing);
                    }
                    else
                    {
                        var visitor = new VisitorCounter
                        {
                            Date = today,
                            Count = 1,
                            IPAddress = visitorIp,
                            UserAgent = userAgent,
                            PageUrl = path
                        };
                        dbContext.VisitorCounters.Add(visitor);
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to record visitor counter for path: {Path}", path);
                }

                context.Items["VisitorCounted"] = true;
            }
        }

        await _next(context);
    }

    private static bool IsAdminArea(string path)
    {
        return path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase);
    }
}
