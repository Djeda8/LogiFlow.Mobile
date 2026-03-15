namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Defines the contract for managing the application visual theme.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Occurs when the app theme changes.
    /// </summary>
    event EventHandler? ThemeChanged;

    /// <summary>
    /// Gets the current theme code.
    /// </summary>
    string CurrentTheme { get; }

    /// <summary>
    /// Applies the theme corresponding to the given code.
    /// </summary>
    /// <param name="themeCode">Theme code: "light" or "dark".</param>
    void ApplyTheme(string themeCode);
}
