using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.Resources.Icons;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;
using LogiFlow.Mobile.Views.Inventory;
using LogiFlow.Mobile.Views.ItemInfo;
using LogiFlow.Mobile.Views.Login;
using LogiFlow.Mobile.Views.Movements;
using LogiFlow.Mobile.Views.Picking;
using LogiFlow.Mobile.Views.Reception;
using LogiFlow.Mobile.Views.Settings;

namespace LogiFlow.Mobile.ViewModels.Menu;

/// <summary>
/// Manages the main menu screen, providing navigation to all application modules.
/// </summary>
public partial class MenuViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly ISessionService _sessionService;
    private readonly ILogService _logService;
    private readonly IErrorHandlerService _errorHandlerService;
    private readonly ILocalizationService _localizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuViewModel"/> class.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="sessionService">The session management service.</param>
    /// <param name="logService">The logging service.</param>
    /// <param name="errorHandlerService">The error handler service.</param>
    /// <param name="localizationService">The localization service.</param>
    public MenuViewModel(
        INavigationService navigationService,
        ISessionService sessionService,
        ILogService logService,
        IErrorHandlerService errorHandlerService,
        ILocalizationService localizationService)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;
        _logService = logService;
        _errorHandlerService = errorHandlerService;
        _localizationService = localizationService;

        BuildMenuItems();

        _localizationService.LanguageChanged += OnLanguageChanged;

        _logService.Info("MenuViewModel initialized.");
    }

    /// <summary>
    /// Gets the collection of menu items displayed in the main menu.
    /// </summary>
    public ObservableCollection<MenuItemViewModel> MenuItems { get; } = [];

    /// <summary>
    /// Navigates to the module identified by the specified route.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task NavigateAsync(string route)
    {
        _logService.Info("Navigating to module. Route={Route}", route);

        try
        {
            IsBusy = true;
            await Task.Yield();
            await _navigationService.NavigateToAsync(route);
            _logService.Info("Navigation successful. Route={Route}", route);
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, $"NavigateAsync to {route}");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Clears the current session and navigates back to the login screen.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task LogoutAsync()
    {
        _logService.OperationStart("Logout", "CurrentUser");

        try
        {
            IsBusy = true;
            _sessionService.ClearSession();
            _logService.OperationSuccess("Logout", "CurrentUser");
            await _navigationService.NavigateToAsync(nameof(LoginPage), clearStack: true);
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "LogoutAsync");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        BuildMenuItems();
        OnPropertyChanged(nameof(MenuItems));
    }

    private void BuildMenuItems()
    {
        MenuItems.Clear();
        MenuItems.Add(new(_localizationService.GetString(nameof(AppResources.MenuReception)), nameof(ReceptionPage), AppIconGlyph.Inventory2));
        MenuItems.Add(new(_localizationService.GetString(nameof(AppResources.MenuMovements)), nameof(MovementsPage), AppIconGlyph.SwapHoriz));
        MenuItems.Add(new(_localizationService.GetString(nameof(AppResources.MenuPicking)), nameof(PickingPage), AppIconGlyph.ShoppingCart));
        MenuItems.Add(new(_localizationService.GetString(nameof(AppResources.MenuInventory)), nameof(InventoryPage), AppIconGlyph.Inventory));
        MenuItems.Add(new(_localizationService.GetString(nameof(AppResources.MenuItemInfo)), nameof(ItemInfoPage), AppIconGlyph.Info));
        MenuItems.Add(new(_localizationService.GetString(nameof(AppResources.MenuSettings)), nameof(SettingsPage), AppIconGlyph.Settings));
    }
}
