using LogiFlow.Mobile.Exceptions;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides centralized error handling and user notification for the application.
/// </summary>
public class ErrorHandlerService : IErrorHandlerService
{
    private readonly ILogService _logService;
    private readonly ILocalizationService _localizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorHandlerService"/> class.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    /// <param name="localizationService">The localization service.</param>
    public ErrorHandlerService(ILogService logService, ILocalizationService localizationService)
    {
        _logService = logService;
        _localizationService = localizationService;
    }

    /// <inheritdoc/>
    public string Handle(ValidationException exception)
    {
        _logService.Warning(
            "Validation error. Field={Field}, Message={Message}",
            exception.Field,
            exception.Message);
        return exception.Message;
    }

    /// <inheritdoc/>
    public string Handle(ConnectionException exception)
    {
        _logService.Error(
            $"Connection error. URL={exception.Url}, Message={exception.Message}",
            exception.InnerException);
        return _localizationService.GetString(nameof(AppResources.Msg_TestErrorConnection));
    }

    /// <inheritdoc/>
    public string Handle(AuthException exception)
    {
        _logService.Warning(
            "Authentication error. Username={Username}, Message={Message}",
            exception.Username,
            exception.Message);
        return exception.Message;
    }

    /// <inheritdoc/>
    public string Handle(Exception exception, string? context = null)
    {
        _logService.Error(
            $"Unexpected error. Context={context ?? "Unknown"}",
            exception);
        return _localizationService.GetString(nameof(AppResources.ErrorUnexpected));
    }
}
