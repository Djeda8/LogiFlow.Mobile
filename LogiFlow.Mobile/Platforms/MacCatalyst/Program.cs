using UIKit;

namespace LogiFlow.Mobile.Platforms.MacCatalyst;

/// <summary>
/// The iOS application entry point.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point of the iOS application.
    /// </summary>
    /// <param name="args">The launch arguments.</param>
    private static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}
