using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Singleton implementation of <see cref="ICameraScanService"/>.
/// Coordinates barcode scan requests between requesting ViewModels and the camera scan page.
/// </summary>
public class CameraScanService : ICameraScanService
{
    private readonly ILogService _logService;
    private Action<string>? _pendingCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraScanService"/> class.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    public CameraScanService(ILogService logService)
    {
        _logService = logService;
    }

    /// <inheritdoc/>
    public bool HasPendingRequest => _pendingCallback is not null;

    /// <inheritdoc/>
    public void RequestScan(Action<string> onResult)
    {
        _pendingCallback = onResult;
        _logService.Info("Camera scan requested.");
    }

    /// <inheritdoc/>
    public void DeliverResult(string code)
    {
        if (_pendingCallback is null)
        {
            _logService.Warning("DeliverResult called with no pending callback. Code={Code}", code);
            return;
        }

        _logService.Info("Camera scan result delivered. Code={Code}", code);
        var callback = _pendingCallback;
        _pendingCallback = null;
        callback.Invoke(code);
    }

    /// <inheritdoc/>
    public void CancelScan()
    {
        _pendingCallback = null;
        _logService.Info("Camera scan cancelled.");
    }
}
