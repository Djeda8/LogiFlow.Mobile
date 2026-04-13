using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Reception;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels.Reception;

/// <summary>
/// Unit tests for <see cref="ReceptionStartViewModel"/>.
/// Covers reception loading, navigation, camera scan, validation and state management.
/// </summary>
public class ReceptionStartViewModelTests
{
    private readonly Mock<IReceptionService> _receptionServiceMock;
    private readonly Mock<IReceptionSessionService> _receptionSessionServiceMock;
    private readonly Mock<ICameraScanService> _cameraScanServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly Mock<IChatDialogService> _chatDialogService;
    private readonly ReceptionStartViewModel _sut;

    public ReceptionStartViewModelTests()
    {
        _receptionServiceMock = new Mock<IReceptionService>();
        _receptionSessionServiceMock = new Mock<IReceptionSessionService>();
        _cameraScanServiceMock = new Mock<ICameraScanService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _localizationServiceMock = new Mock<ILocalizationService>();
        _logServiceMock = new Mock<ILogService>();
        _errorHandlerServiceMock = new Mock<IErrorHandlerService>();
        _chatDialogService = new Mock<IChatDialogService>();

        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => key);

        _sut = new ReceptionStartViewModel(
            _receptionServiceMock.Object,
            _receptionSessionServiceMock.Object,
            _cameraScanServiceMock.Object,
            _navigationServiceMock.Object,
            _localizationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object,
            _chatDialogService.Object);
    }

    // --- Initial state ---

    [Fact]
    public void InitialState_ReceptionNumberIsEmpty()
    {
        Assert.Equal(string.Empty, _sut.ReceptionNumber);
    }

    [Fact]
    public void InitialState_ReceptionLoadedIsFalse()
    {
        Assert.False(_sut.ReceptionLoaded);
    }

    [Fact]
    public void InitialState_HasErrorIsFalse()
    {
        Assert.False(_sut.HasError);
    }

    [Fact]
    public void InitialState_ScanReceptionCommandIsDisabled()
    {
        Assert.False(_sut.ScanReceptionCommand.CanExecute(null));
    }

    [Fact]
    public void InitialState_ContinueCommandIsDisabled()
    {
        Assert.False(_sut.ContinueCommand.CanExecute(null));
    }

    // --- ScanReceptionCommand CanExecute ---

    [Fact]
    public void ScanReceptionCommand_WhenReceptionNumberNotEmpty_IsEnabled()
    {
        _sut.ReceptionNumber = "REC-001";

        Assert.True(_sut.ScanReceptionCommand.CanExecute(null));
    }

    [Fact]
    public void ScanReceptionCommand_WhenReceptionNumberIsWhitespace_IsDisabled()
    {
        _sut.ReceptionNumber = "   ";

        Assert.False(_sut.ScanReceptionCommand.CanExecute(null));
    }

    // --- ScanReceptionAsync ---

    [Fact]
    public async Task ScanReceptionAsync_WhenReceptionFound_SetsReceptionLoaded()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.True(_sut.ReceptionLoaded);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenReceptionFound_SetsFlowTypeDisplay()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.Equal(ReceptionFlowType.Standard, _sut.FlowTypeDisplay);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenReceptionFound_SetsStatusDisplay()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.Equal(ReceptionStatus.New, _sut.StatusDisplay);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenReceptionFound_StartsSession()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        _receptionSessionServiceMock.Verify(x => x.StartReception(reception), Times.Once);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenReceptionFound_EnablesContinueCommand()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.True(_sut.ContinueCommand.CanExecute(null));
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenReceptionNotFound_SetsHasError()
    {
        _receptionServiceMock
            .Setup(x => x.LoadReceptionAsync("REC-999"))
            .ReturnsAsync((ReceptionDto?)null);

        _sut.ReceptionNumber = "REC-999";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenReceptionNotFound_SetsErrorMessage()
    {
        _receptionServiceMock
            .Setup(x => x.LoadReceptionAsync("REC-999"))
            .ReturnsAsync((ReceptionDto?)null);

        _sut.ReceptionNumber = "REC-999";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.NotEmpty(_sut.ErrorMessage);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenReceptionNotFound_ReceptionLoadedIsFalse()
    {
        _receptionServiceMock
            .Setup(x => x.LoadReceptionAsync("REC-999"))
            .ReturnsAsync((ReceptionDto?)null);

        _sut.ReceptionNumber = "REC-999";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.False(_sut.ReceptionLoaded);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenActiveSession_ClearsSessionBeforeLoading()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _receptionSessionServiceMock
            .Setup(x => x.HasActiveReception)
            .Returns(true);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        _receptionSessionServiceMock.Verify(x => x.ClearReception(), Times.Once);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenServiceThrows_SetsHasError()
    {
        _receptionServiceMock
            .Setup(x => x.LoadReceptionAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Service error"));

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("Unexpected error");

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task ScanReceptionAsync_WhenComplete_IsBusyIsFalse()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.False(_sut.IsBusy);
    }

    // --- ContinueAsync ---

    [Fact]
    public async Task ContinueAsync_NavigatesToReceptionHeaderPage()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);
        await _sut.ContinueCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Reception.ReceptionHeaderPage),
            false), Times.Once);
    }

    // --- OpenCameraAsync ---

    [Fact]
    public async Task OpenCameraAsync_RequestsScan()
    {
        await _sut.OpenCameraCommand.ExecuteAsync(null);

        _cameraScanServiceMock.Verify(x => x.RequestScan(
            It.IsAny<Action<string>>()), Times.Once);
    }

    [Fact]
    public async Task OpenCameraAsync_NavigatesToCameraScanPage()
    {
        await _sut.OpenCameraCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Camera.CameraScanPage),
            false), Times.Once);
    }

    [Fact]
    public void OpenCameraAsync_WhenCallbackDelivered_SetsReceptionNumber()
    {
        Action<string>? capturedCallback = null;
        _cameraScanServiceMock
            .Setup(x => x.RequestScan(It.IsAny<Action<string>>()))
            .Callback<Action<string>>(cb => capturedCallback = cb);

        _sut.OpenCameraCommand.Execute(null);
        capturedCallback?.Invoke("REC-001");

        Assert.Equal("REC-001", _sut.ReceptionNumber);
    }

    // --- GoBackAsync ---

    [Fact]
    public async Task GoBackAsync_NavigatesBack()
    {
        await _sut.GoBackCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(false), Times.Once);
    }

    // --- OnAppearing ---

    [Fact]
    public async Task OnAppearing_WhenNotReturningFromCamera_ResetsState()
    {
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        SetupLoadReception("REC-001", reception);

        _sut.ReceptionNumber = "REC-001";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        _sut.OnAppearing();

        Assert.Equal(string.Empty, _sut.ReceptionNumber);
        Assert.False(_sut.ReceptionLoaded);
        Assert.False(_sut.HasError);
    }

    [Fact]
    public async Task OnAppearing_AfterCameraCallback_ReceptionNumberIsPreserved()
    {
        // Arrange — camera delivers result before OnAppearing
        Action<string>? capturedCallback = null;
        _cameraScanServiceMock
            .Setup(x => x.RequestScan(It.IsAny<Action<string>>()))
            .Callback<Action<string>>(cb => capturedCallback = cb);

        await _sut.OpenCameraCommand.ExecuteAsync(null);

        // Camera delivers result — sets ReceptionNumber
        capturedCallback?.Invoke("REC-001");

        // Assert — ReceptionNumber is set before OnAppearing
        Assert.Equal("REC-001", _sut.ReceptionNumber);
    }

    [Fact]
    public void OnAppearing_WhenCalledNormally_ClearsReceptionNumber()
    {
        // Arrange — set some state
        _sut.ReceptionNumber = "REC-001";

        // Act — normal navigation back (not from camera)
        _sut.OnAppearing();

        // Assert — state is cleared
        Assert.Equal(string.Empty, _sut.ReceptionNumber);
    }

    // --- OnReceptionNumberChanged ---

    [Fact]
    public async Task OnReceptionNumberChanged_ClearsError()
    {
        _receptionServiceMock
            .Setup(x => x.LoadReceptionAsync("REC-999"))
            .ReturnsAsync((ReceptionDto?)null);

        _sut.ReceptionNumber = "REC-999";
        await _sut.ScanReceptionCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);

        _sut.ReceptionNumber = "REC-001";

        Assert.False(_sut.HasError);
        Assert.Equal(string.Empty, _sut.ErrorMessage);
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

    private void SetupLoadReception(string number, ReceptionDto reception) =>
        _receptionServiceMock
            .Setup(x => x.LoadReceptionAsync(number))
            .ReturnsAsync(reception);
}
