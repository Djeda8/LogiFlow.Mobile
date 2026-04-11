using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;
using LogiFlow.Mobile.Views.Menu;

namespace LogiFlow.Mobile.ViewModels.Reception;

/// <summary>
/// Manages the generated items screen.
/// Displays the logistic items created after a successful reception confirmation.
/// This is the final step of the reception flow — read only, no editing allowed.
/// </summary>
public partial class ReceptionItemsViewModel : BaseViewModel
{
    private readonly IReceptionService _receptionService;
    private readonly IReceptionSessionService _receptionSessionService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogService _logService;
    private readonly IErrorHandlerService _errorHandlerService;

    [ObservableProperty]
    private string _receptionNumber = string.Empty;

    [ObservableProperty]
    private int _totalItems;

    [ObservableProperty]
    private int _totalQuantity;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionItemsViewModel"/> class.
    /// Provides the required services for managing reception items.
    /// </summary>
    /// <param name="receptionService">The service used to manage reception-related operations. Cannot be null.</param>
    /// <param name="receptionSessionService">The service responsible for handling reception session state and data. Cannot be null.</param>
    /// <param name="navigationService">The service used to navigate between views. Cannot be null.</param>
    /// <param name="localizationService">The service used to provide localized resources. Cannot be null.</param>
    /// <param name="logService">The service used for logging informational and error messages. Cannot be null.</param>
    /// <param name="errorHandlerService">The service responsible for handling and reporting errors. Cannot be null.</param>
    public ReceptionItemsViewModel(
        IReceptionService receptionService,
        IReceptionSessionService receptionSessionService,
        INavigationService navigationService,
        ILocalizationService localizationService,
        ILogService logService,
        IErrorHandlerService errorHandlerService)
    {
        _receptionService = receptionService;
        _receptionSessionService = receptionSessionService;
        _navigationService = navigationService;
        _localizationService = localizationService;
        _logService = logService;
        _errorHandlerService = errorHandlerService;

        _logService.Info("ReceptionItemsViewModel initialized.");
    }

    /// <summary>
    /// Gets the collection of generated logistic items.
    /// </summary>
    public ObservableCollection<ItemDto> GeneratedItems { get; } = [];

    /// <summary>
    /// Asynchronously loads the reception items for the current reception session.
    /// </summary>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    public async Task LoadAsync()
    {
        IsBusy = true;
        HasError = false;

        try
        {
            var reception = _receptionSessionService.CurrentReception;

            if (reception is null)
            {
                ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorNoActiveSession));
                HasError = true;
                _logService.Warning("LoadAsync called with no active reception session.");
                return;
            }

            ReceptionNumber = reception.ReceptionNumber;

            var items = await _receptionService.ConfirmReceptionAsync(reception);

            GeneratedItems.Clear();
            foreach (var item in items)
            {
                GeneratedItems.Add(item);
            }

            TotalItems = GeneratedItems.Count;
            TotalQuantity = GeneratedItems.Sum(i => i.Quantity);

            _logService.Info("ReceptionItems loaded. ReceptionNumber={ReceptionNumber}, TotalItems={TotalItems}, TotalQuantity={TotalQuantity}", ReceptionNumber, TotalItems, TotalQuantity);
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "ReceptionItemsLoad");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Clears the reception session and navigates back to the main menu.
    /// This is the only exit from the reception flow after confirmation.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task FinishAsync()
    {
        _receptionSessionService.ClearReception();

        _logService.Info("Reception flow finished. ReceptionNumber={ReceptionNumber}", ReceptionNumber);

        try
        {
            await _navigationService.NavigateToAsync(nameof(MenuPage), clearStack: true);
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "ReceptionItemsFinish");
            HasError = true;
        }
    }
}
