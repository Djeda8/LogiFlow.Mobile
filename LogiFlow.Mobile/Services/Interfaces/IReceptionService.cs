using LogiFlow.Mobile.DTOs;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides reception operations: loading, validating, confirming and generating items.
/// </summary>
public interface IReceptionService
{
    /// <summary>
    /// Loads a reception by its scanned number.
    /// </summary>
    /// <param name="receptionNumber">The scanned reception number.</param>
    /// <returns>The loaded <see cref="ReceptionDto"/>, or null if not found.</returns>
    Task<ReceptionDto?> LoadReceptionAsync(string receptionNumber);

    /// <summary>
    /// Saves the current state of a reception locally.
    /// </summary>
    /// <param name="reception">The reception to save.</param>
    /// /// <returns>A <see cref="Task"/> representing the asynchronous navigation operation.</returns>
    Task SaveReceptionAsync(ReceptionDto reception);

    /// <summary>
    /// Confirms a reception and generates the logistic items.
    /// </summary>
    /// <param name="reception">The completed reception to confirm.</param>
    /// <returns>The list of generated <see cref="ItemDto"/>.</returns>
    Task<List<ItemDto>> ConfirmReceptionAsync(ReceptionDto reception);

    /// <summary>
    /// Rejects a reception with a specified reason.
    /// </summary>
    /// <param name="reception">The reception to reject.</param>
    /// <param name="reason">The rejection reason provided by the operator.</param>
    /// /// <returns>A task representing the asynchronous operation.</returns>
    Task RejectReceptionAsync(ReceptionDto reception, string reason);

    /// <summary>
    /// Determines whether the checklist step is required for the given reception.
    /// </summary>
    /// <param name="reception">The reception to evaluate.</param>
    /// <returns>True if checklist is mandatory.</returns>
    bool IsChecklistRequired(ReceptionDto reception);
}
