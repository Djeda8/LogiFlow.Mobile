using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides access to device preferences storage using MAUI Preferences.
/// </summary>
public class PreferencesService : IPreferencesService
{
    /// <inheritdoc/>
    public bool ContainsKey(string key) => Preferences.ContainsKey(key);

    /// <inheritdoc/>
    public string Get(string key, string defaultValue) => Preferences.Get(key, defaultValue);

    /// <inheritdoc/>
    public void Set(string key, string value) => Preferences.Set(key, value);
}
