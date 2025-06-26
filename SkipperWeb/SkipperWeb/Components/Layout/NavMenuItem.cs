using Microsoft.AspNetCore.Components.Routing;

namespace SkipperWeb.Components.Layout;

public class NavMenuItem
{
    public required string Text { get; set; }
    public required string Href { get; set; }
    public string? Icon { get; set; }
    public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
}

public class MudNavMenuItem : NavMenuItem
{

}