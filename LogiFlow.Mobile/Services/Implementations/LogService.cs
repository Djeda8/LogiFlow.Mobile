using LogiFlow.Mobile.Services.Interfaces;
using Serilog;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides logging functionality.
/// </summary>
public class LogService : ILogService
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogService"/> class.
    /// </summary>
    /// <param name="fileSystemService">Provides access to the application file system.</param>
    public LogService(IFileSystemService fileSystemService)
    {
        // 📁 Carpeta segura para todas las versiones de Android
        var logDir = Path.Combine(fileSystemService.AppDataDirectory, "logs");

        // Crear carpeta si no existe
        Directory.CreateDirectory(logDir);

        var logPath = Path.Combine(logDir, "logiflow-.log");

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,          // One file per day
                retainedFileCountLimit: 7,                     // Keep 7 days
                fileSizeLimitBytes: 5 * 1024 * 1024,           // 5 MB per file
                rollOnFileSizeLimit: true,                     // Rotate if size exceeded
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),  // Frequent flush
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [T{ThreadId}] {Message:lj}{NewLine}{Exception}");

#if ANDROID
         // Visible in logcat
        loggerConfig.WriteTo.AndroidLog(
            outputTemplate: "[LogiFlow] {Message:lj}{NewLine}{Exception}");
#else
        loggerConfig.WriteTo.Debug(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
#endif

        _logger = loggerConfig.CreateLogger();

        _logger.Information("LogService initialized");
        _logger.Information("Log directory: {LogDir}", logDir);
        _logger.Information("Log path: {LogPath}", logPath);
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
