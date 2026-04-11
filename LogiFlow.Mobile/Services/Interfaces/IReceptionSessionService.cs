using LogiFlow.Mobile.DTOs;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Maintains the state of the active reception flow across all reception screens.
/// Acts as a shared state container for the multi-step reception process.
/// </summary>
public interface IReceptionSessionService
{
    /// <summary>
    /// Gets the reception currently being processed, or null if no reception is active.
    /// </summary>
    ReceptionDto? CurrentReception { get; }

    /// <summary>
    /// Gets a value indicating whether there is an active reception in progress.
    /// </summary>
    bool HasActiveReception { get; }

    /// <summary>
    /// Starts a new reception flow with the loaded reception data.
    /// </summary>
    /// <param name="reception">The reception loaded from the scan.</param>
    void StartReception(ReceptionDto reception);

    /// <summary>
    /// Updates the header data of the current reception.
    /// </summary>
    /// <param name="header">The updated header data.</param>
    void UpdateHeader(ReceptionHeaderDto header);

    /// <summary>
    /// Adds a detail line to the current reception.
    /// </summary>
    /// <param name="detail">The detail line to add.</param>
    void AddDetail(ReceptionDetailDto detail);

    /// <summary>
    /// Replaces all detail lines of the current reception.
    /// </summary>
    /// <param name="details">The new list of detail lines.</param>
    void SetDetails(List<ReceptionDetailDto> details);

    /// <summary>
    /// Clears the current reception session, releasing all state.
    /// </summary>
    void ClearReception();
}
