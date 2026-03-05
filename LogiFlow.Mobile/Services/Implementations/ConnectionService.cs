using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides simulated server connectivity testing for demo purposes.
/// </summary>
public class ConnectionService : IConnectionService
{
    private readonly ILogService _logService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionService"/> class.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    public ConnectionService(ILogService logService)
    {
        _logService = logService;
    }

    /// <inheritdoc/>
    public async Task<bool> TestConnectionAsync(string url, int timeout)
    {
        _logService.Info("Testing connection. URL={Url}, Timeout={Timeout}", url, timeout);

        try
        {
            await Task.Delay(500);

            var result = !string.IsNullOrWhiteSpace(url) &&
                         url.Contains("demo", StringComparison.OrdinalIgnoreCase);

            if (result)
            {
                _logService.Info("Connection test successful. URL={Url}", url);
            }
            else
            {
                _logService.Warning("Connection test failed. URL={Url}", url);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logService.Error($"Unexpected error during connection test. URL={url}", ex);
            return false;
        }
    }
}
