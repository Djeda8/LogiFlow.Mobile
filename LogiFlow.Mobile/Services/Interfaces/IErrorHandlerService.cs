using LogiFlow.Mobile.Exceptions;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides centralized error handling and user notification for the application.
/// </summary>
public interface IErrorHandlerService
{
    /// <summary>
    /// Handles a validation error, logging it and returning a user-friendly message.
    /// </summary>
    /// <param name="exception">The validation exception to handle.</param>
    /// <returns>A user-friendly error message.</returns>
    string Handle(ValidationException exception);

    /// <summary>
    /// Handles a connection error, logging it and returning a user-friendly message.
    /// </summary>
    /// <param name="exception">The connection exception to handle.</param>
    /// <returns>A user-friendly error message.</returns>
    string Handle(ConnectionException exception);

    /// <summary>
    /// Handles an authentication error, logging it and returning a user-friendly message.
    /// </summary>
    /// <param name="exception">The authentication exception to handle.</param>
    /// <returns>A user-friendly error message.</returns>
    string Handle(AuthException exception);

    /// <summary>
    /// Handles an unexpected error, logging it and returning a user-friendly message.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="context">Optional context describing where the error occurred.</param>
    /// <returns>A user-friendly error message.</returns>
    string Handle(Exception exception, string? context = null);
}
