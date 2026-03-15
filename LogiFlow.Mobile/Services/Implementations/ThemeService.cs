using LogiFlow.Mobile.Resources.Styles;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Manages the application visual theme by swapping color resource dictionaries at runtime.
/// </summary>
public class ThemeService : IThemeService
{
    /// <inheritdoc/>
    public event EventHandler? ThemeChanged;

    /// <inheritdoc/>
    public string CurrentTheme { get; private set; } = "light";

    /// <inheritdoc/>
    public void ApplyTheme(string themeCode)
    {
        CurrentTheme = themeCode;

        var mergedDictionaries = Application.Current?.Resources.MergedDictionaries;
        if (mergedDictionaries is null)
        {
            return;
        }

        var existing = mergedDictionaries
            .FirstOrDefault(d => d.Source?.OriginalString.Contains("Colors.xaml") == true);

        if (existing is not null)
        {
            mergedDictionaries.Remove(existing);
        }

        var newDictionary = themeCode == "dark"
            ? (ResourceDictionary)new DarkColors()
            : (ResourceDictionary)new LightColors();

        mergedDictionaries.Add(newDictionary);

        // 🔔 Notificar cambio de tema
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }
}
