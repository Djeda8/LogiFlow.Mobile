using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;
using LogiFlow.Mobile.Views.Reception;

namespace LogiFlow.Mobile.ViewModels.Reception;

/// <summary>
/// Manages the reception checklist screen.
/// Checklist is mandatory for TEST_SAMPLE flow or when article is VEHICLE.
/// </summary>
public partial class ReceptionChecklistViewModel : BaseViewModel
{
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
    private bool _allItemsChecked;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionChecklistViewModel"/> class.
    /// </summary>
    /// <param name="receptionSessionService">Manages the current reception session.</param>
    /// <param name="navigationService">Handles navigation between views.</param>
    /// <param name="localizationService">Provides localized UI strings.</param>
    /// <param name="logService">Logs information and errors.</param>
    /// <param name="errorHandlerService">Handles errors and exceptions.</param>
    public ReceptionChecklistViewModel(
        IReceptionSessionService receptionSessionService,
        INavigationService navigationService,
        ILocalizationService localizationService,
        ILogService logService,
        IErrorHandlerService errorHandlerService)
    {
        _receptionSessionService = receptionSessionService;
        _navigationService = navigationService;
        _localizationService = localizationService;
        _logService = logService;
        _errorHandlerService = errorHandlerService;

        _logService.Info("ReceptionChecklistViewModel initialized.");
    }

    /// <summary>
    /// Gets the collection of checklist items to be completed by the operator.
    /// </summary>
    public ObservableCollection<ChecklistItemViewModel> ChecklistItems { get; } = [];

    /// <summary>
    /// Loads the checklist items based on the active reception flow type.
    /// </summary>
    public void Load()
    {
        HasError = false;

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

        BuildChecklist(reception.FlowType);

        _logService.Info("ReceptionChecklist loaded. ReceptionNumber={ReceptionNumber}, FlowType={FlowType}", ReceptionNumber, FlowType);
    }

    /// <summary>
    /// Updates the overall checklist completion state.
    /// Called when any checklist item changes its checked state.
    /// </summary>
    public void RefreshAllItemsChecked()
    {
        AllItemsChecked = ChecklistItems.Count > 0 && ChecklistItems.All(i => i.IsChecked);
        ContinueCommand.NotifyCanExecuteChanged();
    }

    private bool CanContinue() => IsNotBusy && AllItemsChecked;

    /// <summary>
    /// Validates that all checklist items are completed and navigates to confirmation.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanContinue))]
    private async Task ContinueAsync()
    {
        if (!AllItemsChecked)
        {
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorChecklistIncomplete));
            HasError = true;
            return;
        }

        HasError = false;

        _logService.Info("ReceptionChecklist completed. ReceptionNumber={ReceptionNumber}", ReceptionNumber);

        try
        {
            await _navigationService.NavigateToAsync(nameof(ReceptionConfirmationPage));
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "ReceptionChecklistContinue");
            HasError = true;
        }
    }

    /// <summary>
    /// Navigates back to the detail screen.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task GoBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }

    private void BuildChecklist(string flowType)
    {
        ChecklistItems.Clear();

        // Common checks for all flows
        AddChecklistItem(_localizationService.GetString(nameof(AppResources.ReceptionCheckPackagingOk)));
        AddChecklistItem(_localizationService.GetString(nameof(AppResources.ReceptionCheckQuantityVerified)));
        AddChecklistItem(_localizationService.GetString(nameof(AppResources.ReceptionCheckLabelReadable)));

        // Additional checks for TEST_SAMPLE flow
        if (flowType == ReceptionFlowType.TestSample)
        {
            AddChecklistItem(_localizationService.GetString(nameof(AppResources.ReceptionCheckSampleIntact)));
            AddChecklistItem(_localizationService.GetString(nameof(AppResources.ReceptionCheckSampleLabeled)));
            AddChecklistItem(_localizationService.GetString(nameof(AppResources.ReceptionCheckTemperatureOk)));
        }

        // Additional checks for VEHICLE articles
        var hasVehicle = _receptionSessionService.CurrentReception?.DetailLines
            .Any(d => d.Article.Equals("VEHICLE", StringComparison.OrdinalIgnoreCase)) ?? false;

        if (hasVehicle)
        {
            AddChecklistItem(_localizationService.GetString(nameof(AppResources.ReceptionCheckVehicleCondition)));
            AddChecklistItem(_localizationService.GetString(nameof(AppResources.ReceptionCheckVehicleDocumentation)));
        }

        RefreshAllItemsChecked();
    }

    private void AddChecklistItem(string description)
    {
        var item = new ChecklistItemViewModel { Description = description };
        item.PropertyChanged += (_, _) => RefreshAllItemsChecked();
        ChecklistItems.Add(item);
    }
}
