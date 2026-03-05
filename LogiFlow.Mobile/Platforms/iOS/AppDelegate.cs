using Foundation;

namespace LogiFlow.Mobile.Platforms.IOS;

/// <summary>
/// The iOS application delegate. Bootstraps the MAUI application on the iOS platform.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    /// <inheritdoc/>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
