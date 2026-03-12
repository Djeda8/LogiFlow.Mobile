namespace LogiFlow.Mobile.Models;

/// <summary>
/// Represents a settings option with a code and a display name.
/// </summary>
public class SettingsOption
{
    /// <summary>
    /// Gets or sets the internal code (language-independent).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name (localized).
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SettingsOption other && Code == other.Code;

    /// <inheritdoc/>
    public override int GetHashCode() => Code.GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => DisplayName;
}
