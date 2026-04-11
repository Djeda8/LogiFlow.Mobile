using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.Views.Camera;
using LogiFlow.Mobile.Views.Reception;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides page navigation using a registered page dictionary.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogService _logService;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly Dictionary<string, Type> _pages;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The DI service provider used to resolve pages.</param>
    /// <param name="logService">The logging service.</param>
    /// <param name="navigationWindowService">The navigation window service.</param>
    public NavigationService(
        IServiceProvider serviceProvider,
        ILogService logService,
        INavigationWindowService navigationWindowService)
    {
        _serviceProvider = serviceProvider;
        _logService = logService;
        _navigationWindowService = navigationWindowService;

        _pages = new Dictionary<string, Type>
        {
            { nameof(Views.Login.LoginPage), typeof(Views.Login.LoginPage) },
            { nameof(Views.Splash.SplashPage), typeof(Views.Splash.SplashPage) },
            { nameof(Views.Menu.MenuPage), typeof(Views.Menu.MenuPage) },
            { nameof(Views.Settings.SettingsPage), typeof(Views.Settings.SettingsPage) },

            // ===== Camera =====
            { nameof(CameraScanPage), typeof(CameraScanPage) },

            // ===== Reception =====
            { nameof(ReceptionStartPage), typeof(ReceptionStartPage) },
            { nameof(ReceptionHeaderPage), typeof(ReceptionHeaderPage) },
            { nameof(ReceptionDetailPage), typeof(ReceptionDetailPage) },
            { nameof(ReceptionChecklistPage), typeof(ReceptionChecklistPage) },
            { nameof(ReceptionConfirmationPage), typeof(ReceptionConfirmationPage) },
            { nameof(ReceptionItemsPage), typeof(ReceptionItemsPage) },

            // Add new pages here
        };

        _logService.Info("NavigationService initialized. RegisteredPages={Count}", _pages.Count);
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(string pageKey, bool clearStack = false)
    {
        _logService.Info("Navigating to. PageKey={PageKey}, ClearStack={ClearStack}", pageKey, clearStack);

        var page = CreatePage(pageKey);

        if (_navigationWindowService.GetNavigationPage() is not null)
        {
            if (clearStack)
            {
                _navigationWindowService.InsertPageBefore(page, _navigationWindowService.NavigationStack[0]);
                await _navigationWindowService.PopToRootAsync(animated: false);
            }
            else
            {
                await _navigationWindowService.PushAsync(page);
            }
        }
        else
        {
            _logService.Warning("NavigationPage not found. Setting root page. PageKey={PageKey}", pageKey);
            _navigationWindowService.SetRootPage(page);
        }

        _logService.Info("Navigation completed. PageKey={PageKey}", pageKey);
    }

    /// <inheritdoc/>
    public async Task NavigateBackAsync(bool toRoot = false)
    {
        _logService.Info("Navigating back. ToRoot={ToRoot}", toRoot);

        if (_navigationWindowService.GetNavigationPage() is not null)
        {
            if (toRoot)
            {
                await _navigationWindowService.PopToRootAsync();
            }
            else if (_navigationWindowService.NavigationStack.Count > 1)
            {
                await _navigationWindowService.PopAsync();
            }
            else
            {
                _logService.Warning("NavigateBack called but navigation stack has only one page.");
            }
        }
        else
        {
            _logService.Warning("NavigateBack called but NavigationPage not found.");
        }
    }

    private Page CreatePage(string pageKey)
    {
        if (!_pages.TryGetValue(pageKey, out var pageType))
        {
            _logService.Error($"Page not registered. PageKey={pageKey}");
            throw new ArgumentException($"Page not registered: {pageKey}");
        }

        return (Page)_serviceProvider.GetRequiredService(pageType);
    }
}
