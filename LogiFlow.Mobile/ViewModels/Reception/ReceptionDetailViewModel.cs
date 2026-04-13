using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Models.AI;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;
using LogiFlow.Mobile.Views.Camera;
using LogiFlow.Mobile.Views.Reception;

namespace LogiFlow.Mobile.ViewModels.Reception;

/// <summary>
/// Manages the reception detail screen: defines logistic data for each detail line.
/// Applies validations based on the active flow type (STANDARD or TEST_SAMPLE).
/// </summary>
public partial class ReceptionDetailViewModel : BaseViewModel
{
    private readonly IReceptionService _receptionService;
    private readonly IReceptionSessionService _receptionSessionService;
    private readonly IMasterDataService _masterDataService;
    private readonly ICameraScanService _cameraScanService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogService _logService;
    private readonly IErrorHandlerService _errorHandlerService;

    [ObservableProperty]
    private string _receptionNumber = string.Empty;

    [ObservableProperty]
    private string _flowType = string.Empty;

    [ObservableProperty]
    private bool _isTestSample;

    // --- Article ---
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddDetailCommand))]
    private string _article = string.Empty;

    [ObservableProperty]
    private bool _articleHasError;

    // --- Quantity ---
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddDetailCommand))]
    private int _quantity;

    [ObservableProperty]
    private bool _quantityHasError;

    // --- Location ---
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddDetailCommand))]
    private string _location = string.Empty;

    [ObservableProperty]
    private bool _locationHasError;

    // --- Dimensions ---
    [ObservableProperty]
    private decimal? _width;

    [ObservableProperty]
    private decimal? _height;

    [ObservableProperty]
    private decimal? _depth;

    [ObservableProperty]
    private decimal? _weight;

    // --- TEST_SAMPLE extra fields ---
    [ObservableProperty]
    private string _sampleReference = string.Empty;

    [ObservableProperty]
    private bool _sampleReferenceHasError;

    [ObservableProperty]
    private string _lotNumber = string.Empty;

    [ObservableProperty]
    private string _extraObservations = string.Empty;

    // --- Detail lines list ---
    [ObservableProperty]
    private ObservableCollection<ReceptionDetailDto> _detailLines = [];

    [ObservableProperty]
    private List<string> _availableArticles = [];

    [ObservableProperty]
    private List<string> _availableLocations = [];

    private bool _returningFromCamera;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionDetailViewModel"/> class.
    /// Provides the required services for reception detail operations.
    /// </summary>
    /// <param name="receptionService">The service used to manage reception-related operations and data.</param>
    /// <param name="receptionSessionService">The service responsible for handling reception session state and lifecycle.</param>
    /// <param name="masterDataService">The service that provides access to master data required by the view model.</param>
    /// <param name="cameraScanService">The service used to perform camera-based scanning operations.</param>
    /// <param name="navigationService">The service that manages navigation between views or pages.</param>
    /// <param name="localizationService">The service used to provide localized resources and translations.</param>
    /// <param name="logService">The service used for logging informational and diagnostic messages.</param>
    /// <param name="errorHandlerService">The service responsible for handling and reporting errors.</param>
    /// <param name="chatDialogService">Service to display AI chat dialogs from the ViewModel.</param>
    public ReceptionDetailViewModel(
        IReceptionService receptionService,
        IReceptionSessionService receptionSessionService,
        IMasterDataService masterDataService,
        ICameraScanService cameraScanService,
        INavigationService navigationService,
        ILocalizationService localizationService,
        ILogService logService,
        IErrorHandlerService errorHandlerService,
        IChatDialogService chatDialogService)
    {
        _receptionService = receptionService;
        _receptionSessionService = receptionSessionService;
        _masterDataService = masterDataService;
        _cameraScanService = cameraScanService;
        _navigationService = navigationService;
        _localizationService = localizationService;
        _logService = logService;
        _errorHandlerService = errorHandlerService;

        ChatDialogService = chatDialogService;

        _logService.Info("ReceptionDetailViewModel initialized.");
    }

    /// <summary>
    /// Asynchronously loads the current reception details and related data into the view model.
    /// </summary>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    public async Task LoadAsync()
    {
        if (_returningFromCamera)
        {
            _returningFromCamera = false;
            return;
        }

        IsBusy = true;
        HasError = false;

        // Limpiar estado local antes de cargar
        DetailLines.Clear();
        ClearDetailForm();

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
            FlowType = reception.FlowType;
            IsTestSample = reception.FlowType == ReceptionFlowType.TestSample;

            AvailableArticles = await _masterDataService.GetArticlesAsync();
            AvailableLocations = await _masterDataService.GetLocationsAsync();

            DetailLines = new ObservableCollection<ReceptionDetailDto>(reception.DetailLines);

            _logService.Info("ReceptionDetail loaded. ReceptionNumber={ReceptionNumber}, FlowType={FlowType}", ReceptionNumber, FlowType);
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "ReceptionDetailLoad");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ── AI context ────────────────────────────────────────────────────────────

    /// <summary>
    /// Gets the AI chat context for the reception checklist page.
    /// </summary>
    /// <returns>A <see cref="WmsScreenContext"/> representing the current screen state.</returns>
    protected override WmsScreenContext GetAiContext() => new()
    {
        ScreenId = nameof(ReceptionDetailPage),
        ScreenDisplayName = "Reception — Detail",
        Module = "Reception",
        ScreenState = $"Reception {ReceptionNumber} — FlowType: {FlowType}, " +
                            $"Lines: {DetailLines.Count}, " +
                            $"Current article: {(string.IsNullOrWhiteSpace(Article) ? "none" : Article)}, " +
                            $"Quantity: {Quantity}, Location: {(string.IsNullOrWhiteSpace(Location) ? "none" : Location)}",
    };

    private bool CanAddDetail() =>
        IsNotBusy &&
        !string.IsNullOrWhiteSpace(Article) &&
        Quantity > 0 &&
        !string.IsNullOrWhiteSpace(Location);

    /// <summary>
    /// Validates and adds the current detail line to the reception.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddDetail))]
    private async Task AddDetailAsync()
    {
        if (!await ValidateDetailAsync())
        {
            return;
        }

        var detail = new ReceptionDetailDto
        {
            Article = Article,
            Quantity = Quantity,
            Location = Location,
            Width = Width,
            Height = Height,
            Depth = Depth,
            Weight = Weight,
            ExtraFields = IsTestSample ? new ReceptionExtraFieldsDto
            {
                SampleReference = SampleReference,
                LotNumber = LotNumber,
                Observations = ExtraObservations,
            }
            : null,
        };

        _receptionSessionService.AddDetail(detail);
        DetailLines.Add(detail);

        _logService.Info("Detail line added. Article={Article}, Quantity={Quantity}, Location={Location}", Article, Quantity, Location);

        ClearDetailForm();
    }

    /// <summary>
    /// Removes a detail line from the reception.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private void RemoveDetail(ReceptionDetailDto detail)
    {
        DetailLines.Remove(detail);
        _receptionSessionService.SetDetails([.. DetailLines]);
        _logService.Info("Detail line removed. Article={Article}", detail.Article);
    }

    /// <summary>
    /// Opens the camera scanner to fill the article field.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task ScanArticleAsync()
    {
        _returningFromCamera = true;
        _cameraScanService.RequestScan(code => Article = code);
        await _navigationService.NavigateToAsync(nameof(CameraScanPage));
    }

    /// <summary>
    /// Opens the camera scanner to fill the location field.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task ScanLocationAsync()
    {
        _returningFromCamera = true;
        _cameraScanService.RequestScan(code => Location = code);
        await _navigationService.NavigateToAsync(nameof(CameraScanPage));
    }

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task ScanSampleReferenceAsync()
    {
        _returningFromCamera = true;
        _cameraScanService.RequestScan(code => SampleReference = code);
        await _navigationService.NavigateToAsync(nameof(CameraScanPage));
    }

    /// <summary>
    /// Validates that at least one detail line exists and navigates to the next step.
    /// Navigates to checklist if required, otherwise directly to confirmation.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task ContinueAsync()
    {
        if (DetailLines.Count == 0)
        {
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorNoDetails));
            HasError = true;
            return;
        }

        var reception = _receptionSessionService.CurrentReception!;
        var requiresChecklist = _receptionService.IsChecklistRequired(reception);

        var nextPage = requiresChecklist
            ? nameof(ReceptionChecklistPage)
            : nameof(ReceptionConfirmationPage);

        _logService.Info("ReceptionDetail continue. RequiresChecklist={RequiresChecklist}", requiresChecklist);

        await _navigationService.NavigateToAsync(nextPage);
    }

    /// <summary>
    /// Navigates back to the header screen.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task GoBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }

    private async Task<bool> ValidateDetailAsync()
    {
        ClearDetailErrors();

        if (string.IsNullOrWhiteSpace(Article))
        {
            ArticleHasError = true;
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorArticleRequired));
            HasError = true;
            return false;
        }

        var articleValid = await _masterDataService.IsValidArticleAsync(Article);
        if (!articleValid)
        {
            ArticleHasError = true;
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorArticleInvalid));
            HasError = true;
            return false;
        }

        if (Quantity <= 0)
        {
            QuantityHasError = true;
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorQuantityZero));
            HasError = true;
            return false;
        }

        if (string.IsNullOrWhiteSpace(Location))
        {
            LocationHasError = true;
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorLocationRequired));
            HasError = true;
            return false;
        }

        var locationValid = await _masterDataService.IsValidLocationAsync(Location);
        if (!locationValid)
        {
            LocationHasError = true;
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorLocationInvalid));
            HasError = true;
            return false;
        }

        if (IsTestSample && string.IsNullOrWhiteSpace(SampleReference))
        {
            SampleReferenceHasError = true;
            ErrorMessage = _localizationService.GetString(nameof(AppResources.ReceptionErrorSampleReferenceRequired));
            HasError = true;
            return false;
        }

        return true;
    }

    private void ClearDetailForm()
    {
        Article = string.Empty;
        Quantity = 0;
        Location = string.Empty;
        Width = null;
        Height = null;
        Depth = null;
        Weight = null;
        SampleReference = string.Empty;
        LotNumber = string.Empty;
        ExtraObservations = string.Empty;
        ClearDetailErrors();
    }

    private void ClearDetailErrors()
    {
        ArticleHasError = false;
        QuantityHasError = false;
        LocationHasError = false;
        SampleReferenceHasError = false;
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
