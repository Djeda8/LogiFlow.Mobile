using Android.App;
using Android.Runtime;

namespace LogiFlow.Mobile.Platforms.Android;

/// <summary>
/// The main Android application class. Bootstraps the MAUI application on the Android platform.
/// </summary>
[Application]
public class MainApplication : MauiApplication
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainApplication"/> class.
    /// </summary>
    /// <param name="handle">The Java Native Interface handle.</param>
    /// <param name="ownership">The handle ownership for JNI.</param>
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <inheritdoc/>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
