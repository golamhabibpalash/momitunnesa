using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace School.Web.TagHelpers;

[HtmlTargetElement("a", Attributes = "lang-switcher")]
public class LanguageSwitcherTagHelper : TagHelper
{
    [HtmlAttributeName("lang-switcher")]
    public string? TargetLanguage { get; set; }

    [HtmlAttributeName("lang-return-url")]
    public string? ReturnUrl { get; set; }

    private readonly IHttpContextAccessor _httpContextAccessor;

    public LanguageSwitcherTagHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(TargetLanguage))
            return;

        var culture = new CultureInfo(TargetLanguage);
        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));

        var returnUrl = ReturnUrl ?? _httpContextAccessor.HttpContext?.Request.Path.ToString() ?? "/";

        output.Attributes.SetAttribute("href",
            $"?culture={Uri.EscapeDataString(TargetLanguage)}&ui-culture={Uri.EscapeDataString(TargetLanguage)}&returnUrl={Uri.EscapeDataString(returnUrl)}");

        var currentCulture = CultureInfo.CurrentUICulture;
        if (currentCulture.Name.Equals(TargetLanguage, StringComparison.OrdinalIgnoreCase))
        {
            output.Attributes.SetAttribute("class",
                (output.Attributes.TryGetAttribute("class", out var cls) ? cls.Value + " " : "") + "active-language");
        }
    }
}
