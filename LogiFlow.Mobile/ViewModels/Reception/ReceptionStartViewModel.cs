using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;
using LogiFlow.Mobile.Views.Camera;
using LogiFlow.Mobile.Views.Reception;

namespace LogiFlow.Mobile.ViewModels.Reception;

/// <summary>
/// Manages the reception start screen: scans the transport label and loads the reception.
/// </summary>
public partial class ReceptionStartViewModel : BaseViewModel
{
    private readonly IReceptionService _receptionService;
    private readonly IReceptionSessionService _receptionSessionService;
    private readonly ICameraScanService _cameraScanService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogService _logService;
    private readonly IErrorHandlerService _errorHandlerService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanReceptionCommand))]
    private string _receptionNumber = string.Empty;

    [ObservableProperty]
    private string _flowTypeDisplay = string.Empty;

    [ObservableProperty]
    private string _statusDisplay = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    private bool _receptionLoaded;
    private bool _returningFromCamera;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionStartViewModel"/> class.
    /// Provides the required services for starting a reception process.
    /// </summary>
    /// <param name="receptionService">The service used to manage reception-related operations. Cannot be null.</param>
    /// <param name="receptionSessionService">The service responsible for handling reception session state and data. Cannot be null.</param>
    /// <param name="cameraScanService">The service used for camera-based scanning operations. Cannot be null.</param>
    /// <param name="navigationService">The service used to navigate between application views. Cannot be null.</param>
    /// <param name="localizationService">The service used to provide localized resources. Cannot be null.</param>
    /// <param name="logService">The service used for logging informational and error messages. Cannot be null.</param>
    /// <param name="errorHandlerService">The service responsible for handling and reporting errors. Cannot be null.</param>
    public ReceptionStartViewModel(
        IReceptionService receptionService,
        IReceptionSessionService receptionSessionService,
        ICameraScanService cameraScanService,
        INavigationService navigationService,
        ILocalizationService localizationService,
        ILogService logService,
        IErrorHandlerService errorHandlerService)
    {
        _receptionService = receptionService;
        _receptionSessionService = receptionSessionService;
        _cameraScanService = cameraScanService;
        _navigationService = navigationService;
        _localizationService = localizationService;
        _logService = logService;
        _errorHandlerService = errorHandlerService;

        _logService.Info("ReceptionStartViewModel initialized.");
    }

    /// <summary>
    /// Resets the view model state when the associated view appears.
    /// </summary>
    public void OnAppearing()
    {
        if (_returningFromCamera)
        {
            _returningFromCamera = false;
            return;
        }

        ReceptionNumber = string.Empty;
        FlowTypeDisplay = string.Empty;
        StatusDisplay = string.Empty;
        ReceptionLoaded = false;
        HasError = false;
        ErrorMessage = string.Empty;
    }

    private bool CanScanReception() => IsNotBusy && !string.IsNullOrWhiteSpace(ReceptionNumber);

    /// <summary>
    /// Loads the reception data from the scanned or manually entered reception number.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanScanReception))]
    private async Task ScanReceptionAsync()
    {
        IsBusy = true;
        HasError = false;
        ReceptionLoaded = false;

        // Limpiar sesión anterior si existe
        if (_receptionSessionService.HasActiveReception)
        {
            _receptionSessionService.ClearReception();
        }

        _logService.OperationStart("ScanReception", ReceptionNumber);

        try
        {
            var reception = await _receptionService.LoadReceptionAsync(ReceptionNumber);

            if (reception is null)
            {
                ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorNotFound));
                HasError = true;
                _logService.OperationFailure("ScanReception", ReceptionNumber, "Reception not found.");
                return;
            }

            _receptionSessionService.StartReception(reception);

            FlowTypeDisplay = reception.FlowType;
            StatusDisplay = reception.Status;
            ReceptionLoaded = true;

            _logService.OperationSuccess("ScanReception", ReceptionNumber, $"FlowType={reception.FlowType}, Status={reception.Status}");
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "ScanReception");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanContinue() => IsNotBusy && ReceptionLoaded;

    [RelayCommand(CanExecute = nameof(CanContinue))]
    private async Task ContinueAsync()
    {
        await _navigationService.NavigateToAsync(nameof(ReceptionHeaderPage));
    }

    /// <summary>
    /// Opens the camera scanner and fills the reception number field with the result.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task OpenCameraAsync()
    {
        _cameraScanService.RequestScan(code =>
        {
            ReceptionNumber = code;
        });

        await _navigationService.NavigateToAsync(nameof(CameraScanPage));
    }

    /// <summary>
    /// Navigates back to the main menu.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task GoBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }

    partial void OnReceptionNumberChanged(string value)
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
