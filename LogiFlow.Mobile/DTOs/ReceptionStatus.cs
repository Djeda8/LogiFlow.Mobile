namespace LogiFlow.Mobile.DTOs;

/// <summary>
/// Defines the possible reception status values.
/// </summary>
public static class ReceptionStatus
{
    /// <summary>
    /// Indicates a new reception.
    /// </summary>
    public const string New = "NEW";

    /// <summary>
    /// Indicates a reception that is currently in progress.
    /// </summary>
    public const string InProgress = "IN_PROGRESS";

    /// <summary>
    /// Indicates a reception that has been confirmed.
    /// </summary>
    public const string Confirmed = "CONFIRMED";

    /// <summary>
    /// Indicates a reception that has been rejected.
    /// </summary>
    public const string Rejected = "REJECTED";

    /// <summary>
    /// Indicates a reception that has been cancelled.
    /// </summary>
    public const string Cancelled = "CANCELLED";
}
