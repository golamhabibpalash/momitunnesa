using System.ComponentModel.DataAnnotations;

namespace School.Domain.Entities;

public class SchoolInfo
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string SchoolNameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string SchoolNameBn { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? SloganEn { get; set; }

    [MaxLength(500)]
    public string? SloganBn { get; set; }

    [MaxLength(1000)]
    public string? AddressEn { get; set; }

    [MaxLength(1000)]
    public string? AddressBn { get; set; }

    [MaxLength(100)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(500)]
    public string? FaviconUrl { get; set; }

    [MaxLength(1000)]
    public string? GoogleMapEmbedUrl { get; set; }

    [MaxLength(1000)]
    public string? FooterTextEn { get; set; }

    [MaxLength(1000)]
    public string? FooterTextBn { get; set; }

    public string? AboutShortEn { get; set; }

    public string? AboutShortBn { get; set; }

    public string? MissionEn { get; set; }

    public string? MissionBn { get; set; }

    public string? VisionEn { get; set; }

    public string? VisionBn { get; set; }

    public string? HistoryEn { get; set; }

    public string? HistoryBn { get; set; }

    public string? ObjectiveEn { get; set; }

    public string? ObjectiveBn { get; set; }

    public string? FuturePlanEn { get; set; }

    public string? FuturePlanBn { get; set; }
}
