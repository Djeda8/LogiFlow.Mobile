using LogiFlow.Mobile.ViewModels.Splash;
using LogiFlow.Mobile.Views.Splash;

namespace LogiFlow.Mobile;

/// <summary>
/// The main application class. Initializes the app and sets the root window.
/// </summary>
public partial class App : Application
{
    private readonly SplashViewModel _splashViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    /// <param name="splashViewModel">The view model for the splash screen.</param>
    public App(SplashViewModel splashViewModel)
    {
        InitializeComponent();
        _splashViewModel = splashViewModel;
    }

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState) => new Window(new NavigationPage(new SplashPage(_splashViewModel)));
}
