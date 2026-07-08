using System.ComponentModel.DataAnnotations;

namespace School.Domain.Entities;

public class VisitorCounter
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int Count { get; set; }

    [MaxLength(50)]
    public string? IPAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [MaxLength(500)]
    public string? PageUrl { get; set; }
}
