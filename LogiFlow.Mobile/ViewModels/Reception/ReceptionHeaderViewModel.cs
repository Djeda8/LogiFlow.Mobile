using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;
using LogiFlow.Mobile.Views.Reception;

namespace LogiFlow.Mobile.ViewModels.Reception;

/// <summary>
/// Represents the view model for the reception header, providing properties and commands to manage and validate header
/// information during a reception workflow.
/// </summary>
/// <remarks>This view model coordinates the loading, validation, and saving of reception header data, and manages
/// navigation between related screens. It relies on injected services for session management, master data retrieval,
/// navigation, localization, logging, and error handling. Properties such as sender, recipient, delivery note, and
/// expected date are exposed for data binding in the user interface. Commands are provided to load data, continue to
/// the next step, or navigate back. The view model is intended for use in scenarios where a user must enter or review
/// reception header details as part of a larger reception process.</remarks>
public partial class ReceptionHeaderViewModel : BaseViewModel
{
    private readonly IReceptionSessionService _receptionSessionService;
    private readonly IMasterDataService _masterDataService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogService _logService;
    private readonly IErrorHandlerService _errorHandlerService;

    [ObservableProperty]
    private string _receptionNumber = string.Empty;

    [ObservableProperty]
    private string _flowType = string.Empty;

    [ObservableProperty]
    private string _sender = string.Empty;

    [ObservableProperty]
    private string _recipient = string.Empty;

    [ObservableProperty]
    private string _deliveryNote = string.Empty;

    [ObservableProperty]
    private string _observations = string.Empty;

    [ObservableProperty]
    private DateTime _expectedDate = DateTime.Today;

    [ObservableProperty]
    private List<string> _availableSenders = [];

    [ObservableProperty]
    private List<string> _availableRecipients = [];

    private bool _returningFromCamera;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionHeaderViewModel"/> class.
    /// </summary>
    /// <param name="receptionSessionService">Manages the current reception session.</param>
    /// <param name="masterDataService">Provides access to master data.</param>
    /// <param name="navigationService">Handles navigation between views.</param>
    /// <param name="localizationService">Provides localized UI strings.</param>
    /// <param name="logService">Logs information and errors.</param>
    /// <param name="errorHandlerService">Handles errors and exceptions.</param>
    public ReceptionHeaderViewModel(
        IReceptionSessionService receptionSessionService,
        IMasterDataService masterDataService,
        INavigationService navigationService,
        ILocalizationService localizationService,
        ILogService logService,
        IErrorHandlerService errorHandlerService)
    {
        _receptionSessionService = receptionSessionService;
        _masterDataService = masterDataService;
        _navigationService = navigationService;
        _localizationService = localizationService;
        _logService = logService;
        _errorHandlerService = errorHandlerService;

        _logService.Info("ReceptionHeaderViewModel initialized.");
    }

    /// <summary>
    /// Loads header data from the current reception session and master data.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task LoadAsync()
    {
        IsBusy = true;
        HasError = false;

        if (_returningFromCamera)
        {
            _returningFromCamera = false;
            return;
        }

        // Limpiar estado local antes de cargar
        Observations = string.Empty;
        Sender = string.Empty;
        Recipient = string.Empty;
        DeliveryNote = string.Empty;

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

            AvailableSenders = await _masterDataService.GetSendersAsync();
            AvailableRecipients = await _masterDataService.GetRecipientsAsync();

            ReceptionNumber = reception.ReceptionNumber;
            FlowType = reception.FlowType;
            Sender = reception.Header.Sender;
            Recipient = reception.Header.Recipient;
            DeliveryNote = reception.Header.DeliveryNote;
            Observations = reception.Header.Observations;
            ExpectedDate = reception.Header.ExpectedDate;

            _logService.Info("ReceptionHeader loaded. ReceptionNumber={ReceptionNumber}", ReceptionNumber);
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "ReceptionHeaderLoad");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Validates and saves the header data, then navigates to the detail screen.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task ContinueAsync()
    {
        if (!ValidateHeader())
        {
            return;
        }

        IsBusy = true;
        HasError = false;

        try
        {
            var header = new ReceptionHeaderDto
            {
                Sender = Sender,
                Recipient = Recipient,
                DeliveryNote = DeliveryNote,
                Observations = Observations,
                ExpectedDate = ExpectedDate,
            };

            _receptionSessionService.UpdateHeader(header);

            _logService.Info("ReceptionHeader saved. ReceptionNumber={ReceptionNumber}", ReceptionNumber);

            await _navigationService.NavigateToAsync(nameof(ReceptionDetailPage));
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "ReceptionHeaderContinue");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Navigates back to the reception start screen.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task GoBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }

    private bool ValidateHeader()
    {
        if (string.IsNullOrWhiteSpace(Sender))
        {
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorSenderRequired));
            HasError = true;
            return false;
        }

        if (string.IsNullOrWhiteSpace(Recipient))
        {
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorRecipientRequired));
            HasError = true;
            return false;
        }

        return true;
    }
}
