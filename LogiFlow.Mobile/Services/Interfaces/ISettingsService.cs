using LogiFlow.Mobile.DTOs;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides persistent storage and management of application settings.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Returns the default application settings.
    /// </summary>
    /// <returns>A <see cref="SettingsDto"/> with default values.</returns>
    SettingsDto GetDefaultSettings();

    /// <summary>
    /// Loads the current application settings from persistent storage.
    /// </summary>
    /// <returns>The stored <see cref="SettingsDto"/>, or defaults if none found.</returns>
    SettingsDto LoadSettings();

    /// <summary>
    /// Saves the provided settings to persistent storage.
    /// </summary>
    /// <param name="settings">The settings to persist.</param>
    void SaveSettings(SettingsDto settings);
}
