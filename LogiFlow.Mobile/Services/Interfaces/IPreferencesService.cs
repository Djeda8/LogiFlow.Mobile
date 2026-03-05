namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides access to device preferences storage.
/// </summary>
public interface IPreferencesService
{
    /// <summary>
    /// Determines whether a preference key exists.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to retrieve.</param>
    /// <param name="defaultValue">The default value if the key does not exist.</param>
    /// <returns>The stored value or the default value.</returns>
    string Get(string key, string defaultValue);

    /// <summary>
    /// Sets the value for the specified key.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to store.</param>
    void Set(string key, string value);
}
