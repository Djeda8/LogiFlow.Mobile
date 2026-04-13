namespace LogiFlow.Mobile.Models.AI;

/// <summary>
/// Represents the WMS screen context sent with each message to provide screen-specific guidance.
/// </summary>
public class WmsScreenContext
{
    /// <summary>
    /// Gets the technical screen identifier (e.g. "ReceptionStartPage").
    /// </summary>
    public string ScreenId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the display name shown to the operator (e.g. "Reception — Start").
    /// </summary>
    public string ScreenDisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Gets optional text describing the current screen state.
    /// </summary>
    public string? ScreenState { get; init; }

    /// <summary>
    /// Gets the application module (e.g. "Reception", "Inventory").
    /// </summary>
    public string Module { get; init; } = string.Empty;
}
