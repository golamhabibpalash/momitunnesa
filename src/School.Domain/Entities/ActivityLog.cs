using System.ComponentModel.DataAnnotations;

namespace School.Domain.Entities;

public class ActivityLog
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string? UserId { get; set; }

    [MaxLength(200)]
    public string? UserName { get; set; }

    [MaxLength(200)]
    public string? Action { get; set; }

    [MaxLength(200)]
    public string? Controller { get; set; }

    [MaxLength(50)]
    public string? IPAddress { get; set; }

    public string? Description { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? BrowserInfo { get; set; }
}
