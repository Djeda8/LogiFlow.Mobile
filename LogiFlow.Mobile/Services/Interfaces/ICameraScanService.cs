namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides camera-based barcode scanning coordination between the scan page and the requesting screen.
/// </summary>
public interface ICameraScanService
{
    /// <summary>
    /// Gets a value indicating whether a scan request is currently pending.
    /// </summary>
    bool HasPendingRequest { get; }

    /// <summary>
    /// Registers a callback to be invoked when a barcode is successfully scanned.
    /// Call this before navigating to <c>CameraScanPage</c>.
    /// </summary>
    /// <param name="onResult">The callback to invoke with the scanned code.</param>
    void RequestScan(Action<string> onResult);

    /// <summary>
    /// Delivers the scanned result to the registered callback and clears the request.
    /// Called by <c>CameraScanViewModel</c> after a successful scan.
    /// </summary>
    /// <param name="code">The scanned barcode value.</param>
    void DeliverResult(string code);

    /// <summary>
    /// Cancels the current scan request without delivering a result.
    /// Called when the user navigates back from <c>CameraScanPage</c> without scanning.
    /// </summary>
    void CancelScan();
}
