#if ANDROID
using LogiFlow.Mobile.Helpers;
#endif
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Splash;
using LogiFlow.Mobile.Views.Splash;

namespace LogiFlow.Mobile;

/// <summary>
/// The main application class. Initializes the app and sets the root window.
/// </summary>
public partial class App : Application
{
    private readonly SplashViewModel _splashViewModel;
    private readonly IThemeService _themeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class with a splash view model and a theme service.
    /// </summary>
    /// <param name="splashViewModel">The splash screen view model.</param>
    /// <param name="themeService">The service for managing app themes.</param>
    public App(SplashViewModel splashViewModel, IThemeService themeService)
    {
        InitializeComponent();

        _splashViewModel = splashViewModel;
        _themeService = themeService;

        // Cambios de tema del sistema
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
        }

        // Cambios de tema desde tu ThemeService
        _themeService.ThemeChanged += OnThemeChanged;
    }

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState) =>
        new Window(new NavigationPage(new SplashPage(_splashViewModel)));

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
#if ANDROID
        RefreshEntries();
#endif
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
#if ANDROID
        RefreshEntries();
#endif
    }

    private void RefreshEntries()
    {
        var app = Application.Current;
        if (app != null)
        {
            foreach (var window in app.Windows)
            {
                if (window.Page != null)
                {
                    UpdateEntries(window.Page);
                }
            }
        }
    }

    private void UpdateEntries(Element element)
    {
#if ANDROID
        if (element is Entry entry && entry.Handler?.PlatformView is Android.Widget.EditText editText)
        {
            var themeService = IPlatformApplication.Current?.Services
                .GetService<IThemeService>();

            var isDark = themeService?.CurrentTheme == "dark";

            EntryThemeUpdater.ApplyTheme(editText, isDark);
        }
#endif

        if (element is IElementController controller)
        {
            foreach (var child in controller.LogicalChildren)
            {
                if (child is Element childElement)
                {
                    UpdateEntries(childElement);
                }
            }
        }
    }
}
