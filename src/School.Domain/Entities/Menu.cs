using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class Menu : BaseEntity
{
    public int? ParentMenuId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? TitleBn { get; set; }

    [MaxLength(500)]
    public string? Url { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    public MenuType MenuType { get; set; }

    public MenuLocation MenuLocation { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsExternal { get; set; }

    [MaxLength(100)]
    public string? Controller { get; set; }

    [MaxLength(100)]
    public string? Action { get; set; }

    [MaxLength(50)]
    public string? Target { get; set; }

    [ForeignKey(nameof(ParentMenuId))]
    public virtual Menu? ParentMenu { get; set; }

    public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();
}
