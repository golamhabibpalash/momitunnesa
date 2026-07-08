using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace School.Web.TagHelpers;

[HtmlTargetElement("a", Attributes = "active-controller")]
[HtmlTargetElement("li", Attributes = "active-controller")]
public class ActiveRouteTagHelper : TagHelper
{
    [HtmlAttributeName("active-controller")]
    public string? ActiveController { get; set; }

    [HtmlAttributeName("active-action")]
    public string? ActiveAction { get; set; }

    [HtmlAttributeName("active-area")]
    public string? ActiveArea { get; set; }

    [HtmlAttributeName("active-class")]
    public string ActiveClass { get; set; } = "active";

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var routeData = ViewContext.RouteData.Values;
        var currentController = routeData["controller"]?.ToString() ?? string.Empty;
        var currentAction = routeData["action"]?.ToString() ?? string.Empty;
        var currentArea = routeData["area"]?.ToString() ?? string.Empty;

        var controllerMatch = string.IsNullOrEmpty(ActiveController) ||
                              currentController.Equals(ActiveController, StringComparison.OrdinalIgnoreCase);

        var actionMatch = string.IsNullOrEmpty(ActiveAction) ||
                          currentAction.Equals(ActiveAction, StringComparison.OrdinalIgnoreCase);

        var areaMatch = string.IsNullOrEmpty(ActiveArea) ||
                        currentArea.Equals(ActiveArea, StringComparison.OrdinalIgnoreCase);

        if (controllerMatch && actionMatch && areaMatch)
        {
            var existingClasses = output.Attributes.TryGetAttribute("class", out var classAttr)
                ? classAttr.Value?.ToString() ?? string.Empty
                : string.Empty;

            var classes = existingClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!classes.Contains(ActiveClass))
            {
                classes.Add(ActiveClass);
            }

            output.Attributes.SetAttribute("class", string.Join(" ", classes));
        }
    }
}
