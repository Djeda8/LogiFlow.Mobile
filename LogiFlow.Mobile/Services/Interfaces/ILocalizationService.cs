using System.Globalization;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides localization and language management for the application.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Occurs when the application language has changed.
    /// </summary>
    event EventHandler LanguageChanged;

    /// <summary>
    /// Gets the current culture used by the application.
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// Returns the localized string for the specified resource key.
    /// </summary>
    /// <param name="key">The resource key to look up.</param>
    /// <returns>The localized string value.</returns>
    string GetString(string key);

    /// <summary>
    /// Sets the application language to the specified culture code.
    /// </summary>
    /// <param name="cultureCode">The culture code to apply (e.g. "en", "es").</param>
    void SetLanguage(string cultureCode);
}
