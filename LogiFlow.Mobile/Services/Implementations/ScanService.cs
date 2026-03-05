using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides simulated scanner device testing operations for demo purposes.
/// </summary>
public class ScanService : IScanService
{
    private readonly ILogService _logService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScanService"/> class.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    public ScanService(ILogService logService)
    {
        _logService = logService;
    }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public async Task<bool> TestScannerAsync(string tipoLector)
    {
        _logService.Info("Testing scanner. ScannerType={ScannerType}", tipoLector);

        try
        {
            await Task.Delay(500);

            // Simulation: internal scanner always succeeds, external scanner has 50% failure rate
            var result = tipoLector == "Interno" || new Random().Next(0, 2) == 0;

            if (result)
            {
                _logService.Info("Scanner test successful. ScannerType={ScannerType}", tipoLector);
            }
            else
            {
                _logService.Warning("Scanner test failed. ScannerType={ScannerType}", tipoLector);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logService.Error($"Unexpected error during scanner test. ScannerType={tipoLector}", ex);
            return false;
        }
    }
}
