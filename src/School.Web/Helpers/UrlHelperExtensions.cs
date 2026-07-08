using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace School.Web.Helpers;

public static partial class UrlHelperExtensions
{
    public static string GenerateSlug(this string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
            return string.Empty;

        var str = phrase.ToLowerInvariant();

        str = RemoveDiacritics(str);

        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", "-").Trim();
        str = Regex.Replace(str, @"-+", "-");

        return str.Trim('-');
    }

    public static string ToFriendlyUrl(this string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        return url;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
