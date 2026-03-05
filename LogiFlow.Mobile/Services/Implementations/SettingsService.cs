using System.Text.Json;
using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides persistent storage and management of application settings using device preferences.
/// </summary>
public class SettingsService : ISettingsService
{
    private const string SettingsKey = "AppSettings";

    private readonly ILogService _logService;
    private readonly IPreferencesService _preferencesService;
    private SettingsDto _currentSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class
    /// and loads the current settings from persistent storage.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    /// <param name="preferencesService">The preferences storage service.</param>
    public SettingsService(ILogService logService, IPreferencesService preferencesService)
    {
        _logService = logService;
        _preferencesService = preferencesService;
        _currentSettings = LoadSettings();
    }

    /// <inheritdoc/>
    public SettingsDto LoadSettings()
    {
        if (_currentSettings != null)
        {
            _logService.Debug("Settings loaded from cache.");
            return _currentSettings;
        }

        if (_preferencesService.ContainsKey(SettingsKey))
        {
            try
            {
                var json = _preferencesService.Get(SettingsKey, string.Empty);
                _currentSettings = JsonSerializer.Deserialize<SettingsDto>(json) ?? GetDefaultSettings();
                _logService.Info("Settings loaded from preferences. URL={Url}, Timeout={Timeout}", _currentSettings.UrlServidor, _currentSettings.Timeout);
            }
            catch (Exception ex)
            {
                _logService.Error("Failed to deserialize settings. Loading defaults.", ex);
                _currentSettings = GetDefaultSettings();
            }
        }
        else
        {
            _logService.Info("No saved settings found. Loading defaults.");
            _currentSettings = GetDefaultSettings();
        }

        return _currentSettings;
    }

    /// <inheritdoc/>
    public void SaveSettings(SettingsDto settings)
    {
        try
        {
            _currentSettings = settings;
            var json = JsonSerializer.Serialize(settings);
            _preferencesService.Set(SettingsKey, json);
            _logService.Info("Settings saved. URL={Url}, Timeout={Timeout}, Environment={Environment}", settings.UrlServidor, settings.Timeout, settings.EntornoServidor);
        }
        catch (Exception ex)
        {
            _logService.Error("Failed to save settings.", ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public SettingsDto GetDefaultSettings() => new();
}
