using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Singleton implementation of <see cref="IReceptionSessionService"/>.
/// Holds the active reception state across all reception screens.
/// </summary>
public class ReceptionSessionService : IReceptionSessionService
{
    private readonly ILogService _logService;

    private ReceptionDto? _currentReception;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionSessionService"/> class.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    public ReceptionSessionService(ILogService logService)
    {
        _logService = logService;
    }

    /// <inheritdoc/>
    public ReceptionDto? CurrentReception => _currentReception;

    /// <inheritdoc/>
    public bool HasActiveReception => _currentReception is not null;

    /// <inheritdoc/>
    public void StartReception(ReceptionDto reception)
    {
        _currentReception = reception;
        _logService.Info("Reception session started. ReceptionNumber={ReceptionNumber}, FlowType={FlowType}", reception.ReceptionNumber, reception.FlowType);
    }

    /// <inheritdoc/>
    public void UpdateHeader(ReceptionHeaderDto header)
    {
        if (_currentReception is null)
        {
            _logService.Warning("UpdateHeader called with no active reception.");
            return;
        }

        _currentReception.Header = header;
        _logService.Info("Reception header updated. ReceptionNumber={ReceptionNumber}", _currentReception.ReceptionNumber);
    }

    /// <inheritdoc/>
    public void AddDetail(ReceptionDetailDto detail)
    {
        if (_currentReception is null)
        {
            _logService.Warning("AddDetail called with no active reception.");
            return;
        }

        _currentReception.DetailLines.Add(detail);
        _logService.Info("Detail added to reception. ReceptionNumber={ReceptionNumber}, Article={Article}, Quantity={Quantity}", _currentReception.ReceptionNumber, detail.Article, detail.Quantity);
    }

    /// <inheritdoc/>
    public void SetDetails(List<ReceptionDetailDto> details)
    {
        if (_currentReception is null)
        {
            _logService.Warning("SetDetails called with no active reception.");
            return;
        }

        _currentReception.DetailLines = details;
        _logService.Info("Reception details replaced. ReceptionNumber={ReceptionNumber}, Count={Count}", _currentReception.ReceptionNumber, details.Count);
    }

    /// <inheritdoc/>
    public void ClearReception()
    {
        var number = _currentReception?.ReceptionNumber ?? "none";
        _currentReception = null;
        _logService.Info("Reception session cleared. ReceptionNumber={ReceptionNumber}", number);
    }
}
