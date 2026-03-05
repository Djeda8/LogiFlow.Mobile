namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides structured logging operations for the application.
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="properties">Optional additional properties.</param>
    void Debug(string message, params object[] properties);

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="properties">Optional additional properties.</param>
    void Info(string message, params object[] properties);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="properties">Optional additional properties.</param>
    void Warning(string message, params object[] properties);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">Optional exception associated with the error.</param>
    void Error(string message, Exception? exception = null);

    /// <summary>
    /// Logs the start of a warehouse operation.
    /// </summary>
    /// <param name="operation">The operation name.</param>
    /// <param name="user">The user performing the operation.</param>
    /// <param name="details">Optional operation details.</param>
    void OperationStart(string operation, string user, string? details = null);

    /// <summary>
    /// Logs the successful completion of a warehouse operation.
    /// </summary>
    /// <param name="operation">The operation name.</param>
    /// <param name="user">The user performing the operation.</param>
    /// <param name="details">Optional operation details.</param>
    void OperationSuccess(string operation, string user, string? details = null);

    /// <summary>
    /// Logs the failure of a warehouse operation.
    /// </summary>
    /// <param name="operation">The operation name.</param>
    /// <param name="user">The user performing the operation.</param>
    /// <param name="reason">The reason for the failure.</param>
    /// <param name="exception">Optional exception associated with the failure.</param>
    void OperationFailure(string operation, string user, string reason, Exception? exception = null);
}
