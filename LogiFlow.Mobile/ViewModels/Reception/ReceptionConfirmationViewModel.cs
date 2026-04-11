using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;
using LogiFlow.Mobile.Views.Reception;

namespace LogiFlow.Mobile.ViewModels.Reception;

/// <summary>
/// Manages the reception confirmation screen.
/// Allows the operator to confirm or reject the reception.
/// Confirmed receptions generate logistic items.
/// Rejected receptions require a reason and do not generate items.
/// </summary>
public partial class ReceptionConfirmationViewModel : BaseViewModel
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
    private string _flowType = string.Empty;

    [ObservableProperty]
    private string _sender = string.Empty;

    [ObservableProperty]
    private string _recipient = string.Empty;

    [ObservableProperty]
    private int _totalLines;

    [ObservableProperty]
    private int _totalQuantity;

    [ObservableProperty]
    private bool _isRejecting;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RejectCommand))]
    private string _rejectionReason = string.Empty;

    [ObservableProperty]
    private bool _rejectionReasonHasError;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionConfirmationViewModel"/> class.
    /// Provides the required services for reception confirmation functionality.
    /// </summary>
    /// <param name="receptionService">The service used to manage reception-related operations. Cannot be null.</param>
    /// <param name="receptionSessionService">The service responsible for handling reception session state and data. Cannot be null.</param>
    /// <param name="navigationService">The service used to navigate between application views. Cannot be null.</param>
    /// <param name="localizationService">The service that provides localized resources and strings. Cannot be null.</param>
    /// <param name="logService">The service used for logging informational and error messages. Cannot be null.</param>
    /// <param name="errorHandlerService">The service responsible for handling and reporting errors within the view model. Cannot be null.</param>
    public ReceptionConfirmationViewModel(
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

        _logService.Info("ReceptionConfirmationViewModel initialized.");
    }

    /// <summary>
    /// Loads the confirmation summary from the active reception session.
    /// </summary>
    public void Load()
    {
        HasError = false;
        IsRejecting = false;
        RejectionReason = string.Empty;
        RejectionReasonHasError = false;

        var reception = _receptionSessionService.CurrentReception;

        if (reception is null)
        {
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorNoActiveSession));
            HasError = true;
            _logService.Warning("Load called with no active reception session.");
            return;
        }

        ReceptionNumber = reception.ReceptionNumber;
        FlowType = reception.FlowType;
        Sender = reception.Header.Sender;
        Recipient = reception.Header.Recipient;
        TotalLines = reception.DetailLines.Count;
        TotalQuantity = reception.DetailLines.Sum(d => d.Quantity);

        _logService.Info("ReceptionConfirmation loaded. ReceptionNumber={ReceptionNumber}, TotalLines={TotalLines}, TotalQuantity={TotalQuantity}", ReceptionNumber, TotalLines, TotalQuantity);
    }

    /// <summary>
    /// Confirms the reception, generates logistic items and navigates to the items screen.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task ConfirmAsync()
    {
        IsBusy = true;
        HasError = false;

        _logService.OperationStart("ConfirmReception", ReceptionNumber);

        try
        {
            var reception = _receptionSessionService.CurrentReception!;
            var items = await _receptionService.ConfirmReceptionAsync(reception);

            _logService.OperationSuccess("ConfirmReception", ReceptionNumber, $"ItemsGenerated={items.Count}");

            await _navigationService.NavigateToAsync(
                nameof(ReceptionItemsPage));
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "ConfirmReception");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Shows the rejection reason input field.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private void StartRejectAsync()
    {
        IsRejecting = true;
        RejectionReason = string.Empty;
        RejectionReasonHasError = false;
        HasError = false;
    }

    /// <summary>
    /// Cancels the rejection and hides the rejection reason input.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private void CancelReject()
    {
        IsRejecting = false;
        RejectionReason = string.Empty;
        RejectionReasonHasError = false;
        HasError = false;
    }

    private bool CanReject() => IsNotBusy && !string.IsNullOrWhiteSpace(RejectionReason);

    /// <summary>
    /// Rejects the reception with the provided reason and navigates back to the menu.
    /// Rejected receptions do not generate logistic items.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanReject))]
    private async Task RejectAsync()
    {
        if (string.IsNullOrWhiteSpace(RejectionReason))
        {
            RejectionReasonHasError = true;
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorRejectionReasonRequired));
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;

        _logService.OperationStart("RejectReception", ReceptionNumber, $"Reason={RejectionReason}");

        try
        {
            var reception = _receptionSessionService.CurrentReception!;
            await _receptionService.RejectReceptionAsync(reception, RejectionReason);

            _receptionSessionService.ClearReception();

            _logService.OperationSuccess("RejectReception", ReceptionNumber);

            await _navigationService.NavigateToAsync(
                nameof(ReceptionStartPage),
                clearStack: true);
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "RejectReception");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Navigates back to the previous screen.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task GoBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }

    partial void OnRejectionReasonChanged(string value)
    {
        RejectionReasonHasError = false;
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
