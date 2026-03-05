using LogiFlow.Mobile.Services.Interfaces;
using Serilog;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides structured logging using Serilog, including warehouse operation tracking.
/// </summary>
public class LogService : ILogService
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogService"/> class.
    /// </summary>
    /// <param name="fileSystemService">The file system service.</param>
    public LogService(IFileSystemService fileSystemService)
    {
        var logPath = Path.Combine(fileSystemService.AppDataDirectory, "logs", "logiflow-.log");

        if (fileSystemService.ExternalStorageDirectory is not null)
        {
            logPath = Path.Combine(fileSystemService.ExternalStorageDirectory, "Android", "data", "com.companyname.logiflow.mobile", "files", "logs", "logiflow-.log");
        }

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

#if ANDROID
        loggerConfig.WriteTo.AndroidLog(
            outputTemplate: "[LogiFlow] {Message:lj}{NewLine}{Exception}");
#else
        loggerConfig.WriteTo.Debug(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
#endif

        _logger = loggerConfig.CreateLogger();
        _logger.Information("LogService initialized. Log path: {LogPath}", logPath);
    }

    /// <inheritdoc/>
    public void Debug(string message, params object[] properties) =>
        _logger.Debug(message, properties);

    /// <inheritdoc/>
    public void Info(string message, params object[] properties) =>
        _logger.Information(message, properties);

    /// <inheritdoc/>
    public void Warning(string message, params object[] properties) =>
        _logger.Warning(message, properties);

    /// <inheritdoc/>
    public void Error(string message, Exception? exception = null)
    {
        if (exception is null)
        {
            _logger.Error(message);
        }
        else
        {
            _logger.Error(exception, message);
        }
    }

    /// <inheritdoc/>
    public void OperationStart(string operation, string user, string? details = null) =>
        _logger.Information("[OP:START] Operation={Operation} User={User} Details={Details}", operation, user, details ?? "-");

    /// <inheritdoc/>
    public void OperationSuccess(string operation, string user, string? details = null) =>
        _logger.Information("[OP:SUCCESS] Operation={Operation} User={User} Details={Details}", operation, user, details ?? "-");

    /// <inheritdoc/>
    public void OperationFailure(string operation, string user, string reason, Exception? exception = null)
    {
        if (exception is null)
        {
            _logger.Warning("[OP:FAILURE] Operation={Operation} User={User} Reason={Reason}", operation, user, reason);
        }
        else
        {
            _logger.Error(exception, "[OP:FAILURE] Operation={Operation} User={User} Reason={Reason}", operation, user, reason);
        }
    }
}
