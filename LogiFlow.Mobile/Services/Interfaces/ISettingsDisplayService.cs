namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides display name resolution for settings list values.
/// </summary>
public interface ISettingsDisplayService
{
    /// <summary>Gets the display name for a theme code.</summary>
    /// <param name="code">The internal theme code (e.g. "light", "dark").</param>
    /// <returns>The localized display name for the theme.</returns>
    string GetThemeDisplay(string code);

    /// <summary>Gets the display name for a scanner type code.</summary>
    /// <param name="code">The internal scanner type code (e.g. "internal", "external").</param>
    /// <returns>The localized display name for the scanner type.</returns>
    string GetScannerDisplay(string code);

    /// <summary>Gets the display name for an environment code.</summary>
    /// <param name="code">The internal environment code (e.g. "demo", "production").</param>
    /// <returns>The localized display name for the environment.</returns>
    string GetEnvironmentDisplay(string code);

    /// <summary>Gets the theme code from a display name.</summary>
    /// <param name="display">The localized display name of the theme.</param>
    /// <returns>The internal theme code.</returns>
    string GetThemeCode(string display);

    /// <summary>Gets the scanner type code from a display name.</summary>
    /// <param name="display">The localized display name of the scanner type.</param>
    /// <returns>The internal scanner type code.</returns>
    string GetScannerCode(string display);

    /// <summary>Gets the environment code from a display name.</summary>
    /// <param name="display">The localized display name of the environment.</param>
    /// <returns>The internal environment code.</returns>
    string GetEnvironmentCode(string display);
}
