namespace School.Web.Helpers;

/// <summary>
/// Maps a stored social platform name onto the Font Awesome icon and the CSS
/// modifier used for its brand colour, so the admin only has to pick a platform.
/// </summary>
public static class SocialPlatformHelper
{
    private static readonly Dictionary<string, (string Icon, string CssClass)> Platforms =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Facebook"] = ("fab fa-facebook-f", "facebook"),
            ["YouTube"] = ("fab fa-youtube", "youtube"),
            ["Twitter"] = ("fab fa-x-twitter", "twitter"),
            ["Instagram"] = ("fab fa-instagram", "instagram"),
            ["LinkedIn"] = ("fab fa-linkedin-in", "linkedin"),
            ["WhatsApp"] = ("fab fa-whatsapp", "whatsapp"),
            ["Telegram"] = ("fab fa-telegram", "telegram"),
        };

    /// <summary>The platforms offered in the admin dropdown, in display order.</summary>
    public static IEnumerable<string> KnownPlatforms => Platforms.Keys;

    /// <summary>
    /// The icon class for a platform. A link that stored its own icon keeps it;
    /// anything unrecognised falls back to a generic link icon.
    /// </summary>
    public static string GetIcon(string? platform, string? storedIcon = null)
    {
        if (!string.IsNullOrWhiteSpace(storedIcon))
            return storedIcon;

        return platform is not null && Platforms.TryGetValue(platform, out var entry)
            ? entry.Icon
            : "fas fa-link";
    }

    /// <summary>The CSS modifier that gives the link its brand colour on hover.</summary>
    public static string GetCssClass(string? platform) =>
        platform is not null && Platforms.TryGetValue(platform, out var entry)
            ? entry.CssClass
            : "generic";
}
