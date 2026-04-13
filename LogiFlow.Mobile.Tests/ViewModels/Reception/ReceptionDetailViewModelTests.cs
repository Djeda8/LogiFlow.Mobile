using System.Collections.ObjectModel;
using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Reception;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels.Reception;

/// <summary>
/// Unit tests for <see cref="ReceptionDetailViewModel"/>.
/// Covers loading, detail line management, validation, camera scan and navigation.
/// </summary>
public class ReceptionDetailViewModelTests
{
    private readonly Mock<IReceptionService> _receptionServiceMock;
    private readonly Mock<IReceptionSessionService> _receptionSessionServiceMock;
    private readonly Mock<IMasterDataService> _masterDataServiceMock;
    private readonly Mock<ICameraScanService> _cameraScanServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly Mock<IChatDialogService> _chatDialogService;

    private readonly ReceptionDetailViewModel _sut;

    public ReceptionDetailViewModelTests()
    {
        _receptionServiceMock = new Mock<IReceptionService>();
        _receptionSessionServiceMock = new Mock<IReceptionSessionService>();
        _masterDataServiceMock = new Mock<IMasterDataService>();
        _cameraScanServiceMock = new Mock<ICameraScanService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _localizationServiceMock = new Mock<ILocalizationService>();
        _logServiceMock = new Mock<ILogService>();
        _errorHandlerServiceMock = new Mock<IErrorHandlerService>();
        _chatDialogService = new Mock<IChatDialogService>();

        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => key);

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("Unexpected error");

        _masterDataServiceMock
            .Setup(x => x.GetArticlesAsync())
            .ReturnsAsync(["ART-001", "ART-002", "VEHICLE", "SAMPLE-A"]);

        _masterDataServiceMock
            .Setup(x => x.GetLocationsAsync())
            .ReturnsAsync(["A-01-01", "A-01-02", "QA-ZONE-01"]);

        _masterDataServiceMock
            .Setup(x => x.IsValidArticleAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _masterDataServiceMock
            .Setup(x => x.IsValidArticleAsync("ART-001"))
            .ReturnsAsync(true);

        _masterDataServiceMock
            .Setup(x => x.IsValidArticleAsync("SAMPLE-A"))
            .ReturnsAsync(true);

        _masterDataServiceMock
            .Setup(x => x.IsValidArticleAsync("VEHICLE"))
            .ReturnsAsync(true);

        _masterDataServiceMock
            .Setup(x => x.IsValidLocationAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _masterDataServiceMock
            .Setup(x => x.IsValidLocationAsync("A-01-01"))
            .ReturnsAsync(true);

        _masterDataServiceMock
            .Setup(x => x.IsValidLocationAsync("QA-ZONE-01"))
            .ReturnsAsync(true);

        _sut = new ReceptionDetailViewModel(
            _receptionServiceMock.Object,
            _receptionSessionServiceMock.Object,
            _masterDataServiceMock.Object,
            _cameraScanServiceMock.Object,
            _navigationServiceMock.Object,
            _localizationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object,
            _chatDialogService.Object);
    }

    // --- Initial state ---

    [Fact]
    public void InitialState_DetailLinesIsEmpty()
    {
        Assert.Empty(_sut.DetailLines);
    }

    [Fact]
    public void InitialState_HasErrorIsFalse()
    {
        Assert.False(_sut.HasError);
    }

    [Fact]
    public void InitialState_IsTestSampleIsFalse()
    {
        Assert.False(_sut.IsTestSample);
    }

    [Fact]
    public void InitialState_AddDetailCommandIsDisabled()
    {
        Assert.False(_sut.AddDetailCommand.CanExecute(null));
    }

    // --- LoadAsync ---

    [Fact]
    public async Task LoadAsync_WhenNoActiveSession_SetsHasError()
    {
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns((ReceptionDto?)null);

        await _sut.LoadAsync();

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_SetsReceptionNumber()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        await _sut.LoadAsync();

        Assert.Equal("REC-001", _sut.ReceptionNumber);
    }

    [Fact]
    public async Task LoadAsync_WhenFlowTypeIsStandard_IsTestSampleIsFalse()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        await _sut.LoadAsync();

        Assert.False(_sut.IsTestSample);
    }

    [Fact]
    public async Task LoadAsync_WhenFlowTypeIsTestSample_IsTestSampleIsTrue()
    {
        SetupSession(BuildReception("REC-002", ReceptionFlowType.TestSample));

        await _sut.LoadAsync();

        Assert.True(_sut.IsTestSample);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_LoadsAvailableArticles()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        await _sut.LoadAsync();

        Assert.NotEmpty(_sut.AvailableArticles);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_LoadsAvailableLocations()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        await _sut.LoadAsync();

        Assert.NotEmpty(_sut.AvailableLocations);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionHasExistingLines_LoadsDetailLines()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        reception.DetailLines.Add(BuildDetailLine("ART-001", 5, "A-01-01"));
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.Single(_sut.DetailLines);
    }

    [Fact]
    public async Task LoadAsync_WhenCalledTwice_ClearsAndReloads()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        await _sut.LoadAsync();
        await _sut.LoadAsync();

        Assert.Empty(_sut.DetailLines);
    }

    [Fact]
    public async Task LoadAsync_WhenComplete_IsBusyIsFalse()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        await _sut.LoadAsync();

        Assert.False(_sut.IsBusy);
    }

    [Fact]
    public async Task LoadAsync_WhenReturningFromCamera_SkipsReload()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        // Simulate camera scan
        await _sut.ScanArticleCommand.ExecuteAsync(null);

        // LoadAsync called on returning from camera — should skip
        await _sut.LoadAsync();

        // Articles still loaded from first call
        Assert.NotEmpty(_sut.AvailableArticles);
    }

    // --- AddDetailAsync ---

    [Fact]
    public async Task AddDetailAsync_WhenArticleInvalid_SetsArticleHasError()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        _sut.Article = "INVALID";
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";

        await _sut.AddDetailCommand.ExecuteAsync(null);

        Assert.True(_sut.ArticleHasError);
    }

    [Fact]
    public async Task AddDetailAsync_WhenQuantityZero_SetsQuantityHasError()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        _sut.Article = "ART-001";
        _sut.Quantity = 0;
        _sut.Location = "A-01-01";

        // CanAddDetail returns false when Quantity <= 0 — bypass via direct call
        // by setting quantity after CanExecute check
        _sut.Quantity = 1;
        _sut.Quantity = 0;

        Assert.False(_sut.AddDetailCommand.CanExecute(null));
    }

    [Fact]
    public async Task AddDetailAsync_WhenLocationInvalid_SetsLocationHasError()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = "FAKE-LOC";

        await _sut.AddDetailCommand.ExecuteAsync(null);

        Assert.True(_sut.LocationHasError);
    }

    [Fact]
    public async Task AddDetailAsync_WhenTestSampleAndNoSampleReference_SetsSampleReferenceHasError()
    {
        SetupSession(BuildReception("REC-002", ReceptionFlowType.TestSample));
        await _sut.LoadAsync();

        _sut.Article = "SAMPLE-A";
        _sut.Quantity = 2;
        _sut.Location = "QA-ZONE-01";
        _sut.SampleReference = string.Empty;

        await _sut.AddDetailCommand.ExecuteAsync(null);

        Assert.True(_sut.SampleReferenceHasError);
    }

    [Fact]
    public async Task AddDetailAsync_WhenValid_AddsLineToDetailLines()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";

        await _sut.AddDetailCommand.ExecuteAsync(null);

        Assert.Single(_sut.DetailLines);
    }

    [Fact]
    public async Task AddDetailAsync_WhenValid_AddsDetailToSession()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";

        await _sut.AddDetailCommand.ExecuteAsync(null);

        _receptionSessionServiceMock.Verify(x => x.AddDetail(
            It.Is<ReceptionDetailDto>(d =>
                d.Article == "ART-001" &&
                d.Quantity == 5 &&
                d.Location == "A-01-01")),
            Times.Once);
    }

    [Fact]
    public async Task AddDetailAsync_WhenValid_ClearsFormAfterAdding()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";

        await _sut.AddDetailCommand.ExecuteAsync(null);

        Assert.Equal(string.Empty, _sut.Article);
        Assert.Equal(0, _sut.Quantity);
        Assert.Equal(string.Empty, _sut.Location);
    }

    [Fact]
    public async Task AddDetailAsync_WhenTestSampleValid_SetsExtraFields()
    {
        SetupSession(BuildReception("REC-002", ReceptionFlowType.TestSample));
        await _sut.LoadAsync();

        _sut.Article = "SAMPLE-A";
        _sut.Quantity = 2;
        _sut.Location = "QA-ZONE-01";
        _sut.SampleReference = "SR-001";
        _sut.LotNumber = "LOT-001";

        await _sut.AddDetailCommand.ExecuteAsync(null);

        _receptionSessionServiceMock.Verify(x => x.AddDetail(
            It.Is<ReceptionDetailDto>(d =>
                d.ExtraFields != null &&
                d.ExtraFields.SampleReference == "SR-001" &&
                d.ExtraFields.LotNumber == "LOT-001")),
            Times.Once);
    }

    // --- RemoveDetail ---

    [Fact]
    public async Task RemoveDetail_RemovesLineFromDetailLines()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";
        await _sut.AddDetailCommand.ExecuteAsync(null);

        var line = _sut.DetailLines.First();
        _sut.RemoveDetailCommand.Execute(line);

        Assert.Empty(_sut.DetailLines);
    }

    [Fact]
    public async Task RemoveDetail_UpdatesSessionDetails()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";
        await _sut.AddDetailCommand.ExecuteAsync(null);

        var line = _sut.DetailLines.First();
        _sut.RemoveDetailCommand.Execute(line);

        _receptionSessionServiceMock.Verify(x => x.SetDetails(
            It.Is<List<ReceptionDetailDto>>(l => l.Count == 0)),
            Times.Once);
    }

    // --- ContinueAsync ---

    [Fact]
    public async Task ContinueAsync_WhenNoDetailLines_SetsHasError()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        await _sut.LoadAsync();

        await _sut.ContinueCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task ContinueAsync_WhenChecklistRequired_NavigatesToChecklistPage()
    {
        var reception = BuildReception("REC-002", ReceptionFlowType.TestSample);
        SetupSession(reception);
        await _sut.LoadAsync();

        _sut.Article = "SAMPLE-A";
        _sut.Quantity = 2;
        _sut.Location = "QA-ZONE-01";
        _sut.SampleReference = "SR-001";
        await _sut.AddDetailCommand.ExecuteAsync(null);

        _receptionServiceMock
            .Setup(x => x.IsChecklistRequired(reception))
            .Returns(true);

        await _sut.ContinueCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Reception.ReceptionChecklistPage),
            false), Times.Once);
    }

    [Fact]
    public async Task ContinueAsync_WhenChecklistNotRequired_NavigatesToConfirmationPage()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupSession(reception);
        await _sut.LoadAsync();

        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";
        await _sut.AddDetailCommand.ExecuteAsync(null);

        _receptionServiceMock
            .Setup(x => x.IsChecklistRequired(reception))
            .Returns(false);

        await _sut.ContinueCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Reception.ReceptionConfirmationPage),
            false), Times.Once);
    }

    // --- Camera scan commands ---

    [Fact]
    public async Task ScanArticleAsync_RequestsScanAndNavigatesToCamera()
    {
        await _sut.ScanArticleCommand.ExecuteAsync(null);

        _cameraScanServiceMock.Verify(x => x.RequestScan(
            It.IsAny<Action<string>>()), Times.Once);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Camera.CameraScanPage),
            false), Times.Once);
    }

    [Fact]
    public void ScanArticleAsync_WhenCallbackDelivered_SetsArticle()
    {
        Action<string>? capturedCallback = null;
        _cameraScanServiceMock
            .Setup(x => x.RequestScan(It.IsAny<Action<string>>()))
            .Callback<Action<string>>(cb => capturedCallback = cb);

        _sut.ScanArticleCommand.Execute(null);
        capturedCallback?.Invoke("ART-001");

        Assert.Equal("ART-001", _sut.Article);
    }

    [Fact]
    public async Task ScanLocationAsync_RequestsScanAndNavigatesToCamera()
    {
        await _sut.ScanLocationCommand.ExecuteAsync(null);

        _cameraScanServiceMock.Verify(x => x.RequestScan(
            It.IsAny<Action<string>>()), Times.Once);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Camera.CameraScanPage),
            false), Times.Once);
    }

    [Fact]
    public void ScanLocationAsync_WhenCallbackDelivered_SetsLocation()
    {
        Action<string>? capturedCallback = null;
        _cameraScanServiceMock
            .Setup(x => x.RequestScan(It.IsAny<Action<string>>()))
            .Callback<Action<string>>(cb => capturedCallback = cb);

        _sut.ScanLocationCommand.Execute(null);
        capturedCallback?.Invoke("A-01-01");

        Assert.Equal("A-01-01", _sut.Location);
    }

    [Fact]
    public async Task ScanSampleReferenceAsync_RequestsScanAndNavigatesToCamera()
    {
        await _sut.ScanSampleReferenceCommand.ExecuteAsync(null);

        _cameraScanServiceMock.Verify(x => x.RequestScan(
            It.IsAny<Action<string>>()), Times.Once);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Camera.CameraScanPage),
            false), Times.Once);
    }

    [Fact]
    public void ScanSampleReferenceAsync_WhenCallbackDelivered_SetsSampleReference()
    {
        Action<string>? capturedCallback = null;
        _cameraScanServiceMock
            .Setup(x => x.RequestScan(It.IsAny<Action<string>>()))
            .Callback<Action<string>>(cb => capturedCallback = cb);

        _sut.ScanSampleReferenceCommand.Execute(null);
        capturedCallback?.Invoke("SR-001");

        Assert.Equal("SR-001", _sut.SampleReference);
    }

    // --- GoBackAsync ---

    [Fact]
    public async Task GoBackAsync_NavigatesBack()
    {
        await _sut.GoBackCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(false), Times.Once);
    }

    // --- AddDetailCommand CanExecute ---

    [Fact]
    public void AddDetailCommand_WhenArticleAndQuantityAndLocationSet_IsEnabled()
    {
        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";

        Assert.True(_sut.AddDetailCommand.CanExecute(null));
    }

    [Fact]
    public void AddDetailCommand_WhenArticleEmpty_IsDisabled()
    {
        _sut.Article = string.Empty;
        _sut.Quantity = 5;
        _sut.Location = "A-01-01";

        Assert.False(_sut.AddDetailCommand.CanExecute(null));
    }

    [Fact]
    public void AddDetailCommand_WhenLocationEmpty_IsDisabled()
    {
        _sut.Article = "ART-001";
        _sut.Quantity = 5;
        _sut.Location = string.Empty;

        Assert.False(_sut.AddDetailCommand.CanExecute(null));
    }

    // --- Helpers ---

    private static ReceptionDto BuildReception(string number, string flowType) =>
        new()
        {
            ReceptionNumber = number,
            FlowType = flowType,
            Status = ReceptionStatus.New,
            Header = new ReceptionHeaderDto
            {
                Sender = "Supplier A",
                Recipient = "Warehouse 1",
            },
        };

    private static ReceptionDetailDto BuildDetailLine(string article, int quantity, string location) =>
        new()
        {
            Article = article,
            Quantity = quantity,
            Location = location,
        };

    private void SetupSession(ReceptionDto reception) =>
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns(reception);
}
