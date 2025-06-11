using Microsoft.AspNetCore.Components.Routing;

namespace SkipperWeb.Components.Layout;

public class NavMenuItem
{
    public string Text { get; set; } = "";
    public string Href { get; set; } = "";
    public string Icon { get; set; } = "";
    public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
}