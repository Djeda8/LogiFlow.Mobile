using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides access to the device file system paths using MAUI FileSystem.
/// </summary>
public class FileSystemService : IFileSystemService
{
    /// <inheritdoc/>
    public string AppDataDirectory => FileSystem.AppDataDirectory;

    /// <inheritdoc/>
    public string? ExternalStorageDirectory
    {
#if ANDROID
        get => Android.OS.Environment.ExternalStorageDirectory?.AbsolutePath;
#else
        get => null;
#endif
    }
}
