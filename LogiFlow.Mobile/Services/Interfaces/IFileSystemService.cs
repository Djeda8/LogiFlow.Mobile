namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides access to the device file system paths.
/// </summary>
public interface IFileSystemService
{
    /// <summary>
    /// Gets the application data directory path.
    /// </summary>
    string AppDataDirectory { get; }

    /// <summary>
    /// Gets the external storage directory path, or null if not available.
    /// </summary>
    string? ExternalStorageDirectory { get; }
}
