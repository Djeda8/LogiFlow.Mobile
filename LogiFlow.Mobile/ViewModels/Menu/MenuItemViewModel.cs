using LogiFlow.Mobile.Resources.Icons;

namespace LogiFlow.Mobile.ViewModels.Menu;

/// <summary>
/// Represents a single item in the main menu, containing its title, route and icon.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MenuItemViewModel"/> class.
/// </remarks>
/// <param name="title">The display title of the menu item.</param>
/// <param name="route">The navigation route key.</param>
/// <param name="icon">The icon to display alongside the title.</param>
public class MenuItemViewModel(string title, string route, AppIconGlyph icon)
{
    /// <summary>
    /// Gets the display title of the menu item.
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// Gets the navigation route key associated with this menu item.
    /// </summary>
    public string Route { get; } = route;

    /// <summary>
    /// Gets the icon associated with this menu item.
    /// </summary>
    public AppIconGlyph Icon { get; } = icon;
}
