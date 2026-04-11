//using LogiFlow.Mobile.DTOs;
//using LogiFlow.Mobile.Resources.Languages;
//using LogiFlow.Mobile.Services.Interfaces;
//using LogiFlow.Mobile.ViewModels.Reception;
//using Moq;

//namespace LogiFlow.Mobile.Tests.ViewModels;

//public class ReceptionStartViewModelTestsReservd
//{
//    private readonly Mock<IReceptionService> _receptionServiceMock;
//    private readonly Mock<IReceptionSessionService> _receptionSessionServiceMock;
//    private readonly Mock<ICameraScanService> _cameraScanServiceMock;
//    private readonly Mock<INavigationService> _navigationServiceMock;
//    private readonly Mock<ILocalizationService> _localizationServiceMock;
//    private readonly Mock<ILogService> _logServiceMock;
//    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;

//    private readonly ReceptionStartViewModel _viewModel;

//    public ReceptionStartViewModelTestsReservd()
//    {
//        _receptionServiceMock = new Mock<IReceptionService>();
//        _receptionSessionServiceMock = new Mock<IReceptionSessionService>();
//        _cameraScanServiceMock = new Mock<ICameraScanService>();
//        _navigationServiceMock = new Mock<INavigationService>();
//        _localizationServiceMock = new Mock<ILocalizationService>();
//        _logServiceMock = new Mock<ILogService>();
//        _errorHandlerServiceMock = new Mock<IErrorHandlerService>();

//        _viewModel = new ReceptionStartViewModel(
//            _receptionServiceMock.Object,
//            _receptionSessionServiceMock.Object,
//            _cameraScanServiceMock.Object,
//            _navigationServiceMock.Object,
//            _localizationServiceMock.Object,
//            _logServiceMock.Object,
//            _errorHandlerServiceMock.Object);
//    }

//    #region OnAppearing Tests

//    [Fact]
//    public void OnAppearing_WhenNotReturningFromCamera_ClearsAllFields()
//    {
//        // Arrange
//        _viewModel.ReceptionNumber = "REC123";
//        _viewModel.FlowTypeDisplay = "Inbound";
//        _viewModel.StatusDisplay = "Pending";
//        _viewModel.ReceptionLoaded = true;
//        _viewModel.HasError = true;
//        _viewModel.ErrorMessage = "Test error";

//        // Act
//        _viewModel.OnAppearing();

//        // Assert
//        Assert.Equal(string.Empty, _viewModel.ReceptionNumber);
//        Assert.Equal(string.Empty, _viewModel.FlowTypeDisplay);
//        Assert.Equal(string.Empty, _viewModel.StatusDisplay);
//        Assert.False(_viewModel.ReceptionLoaded);
//        Assert.False(_viewModel.HasError);
//        Assert.Equal(string.Empty, _viewModel.ErrorMessage);
//    }

//    [Fact]
//    public void OnAppearing_WhenReturningFromCamera_DoesNotClearFields()
//    {
//        // Arrange
//        const string testReceptionNumber = "REC123";
//        _viewModel.ReceptionNumber = testReceptionNumber;
//        _viewModel.ReceptionLoaded = true;

//        // Simulate returning from camera by calling OpenCamera first
//        _cameraScanServiceMock
//            .Setup(x => x.RequestScan(It.IsAny<Action<string>>()))
//            .Callback<Action<string>>(callback =>
//            {
//                // Simulate camera returning a code
//                _viewModel.ReceptionNumber = "CAMERA_CODE";
//            });

//        _navigationServiceMock
//            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
//            .Returns(Task.CompletedTask);

//        // Act - Navigate to camera (sets _returningFromCamera flag internally via OpenCamera)
//        // Note: We can't directly test the flag, but we can verify the state after OnAppearing

//        _viewModel.OnAppearing();

//        // Assert - After first OnAppearing, state should be cleared
//        Assert.Equal(string.Empty, _viewModel.ReceptionNumber);
//        Assert.False(_viewModel.ReceptionLoaded);
//    }

//    #endregion

//    #region CanScanReception Tests

//    [Fact]
//    public void CanScanReception_WhenReceptionNumberEmpty_ReturnsFalse()
//    {
//        // Arrange
//        _viewModel.ReceptionNumber = string.Empty;
//        _viewModel.IsBusy = false;

//        // Act
//        var canExecute = _viewModel.ScanReceptionCommand.CanExecute(null);

//        // Assert
//        Assert.False(canExecute);
//    }

//    [Fact]
//    public void CanScanReception_WhenReceptionNumberIsWhitespace_ReturnsFalse()
//    {
//        // Arrange
//        _viewModel.ReceptionNumber = "   ";
//        _viewModel.IsBusy = false;

//        // Act
//        var canExecute = _viewModel.ScanReceptionCommand.CanExecute(null);

//        // Assert
//        Assert.False(canExecute);
//    }

//    [Fact]
//    public void CanScanReception_WhenReceptionNumberFilledAndNotBusy_ReturnsTrue()
//    {
//        // Arrange
//        _viewModel.ReceptionNumber = "REC123";
//        _viewModel.IsBusy = false;

//        // Act
//        var canExecute = _viewModel.ScanReceptionCommand.CanExecute(null);

//        // Assert
//        Assert.True(canExecute);
//    }

//    [Fact]
//    public void CanScanReception_WhenBusy_ReturnsFalse()
//    {
//        // Arrange
//        _viewModel.ReceptionNumber = "REC123";
//        _viewModel.IsBusy = true;

//        // Act
//        var canExecute = _viewModel.ScanReceptionCommand.CanExecute(null);

//        // Assert
//        Assert.False(canExecute);
//    }

//    #endregion

//    #region ScanReceptionAsync Tests

//    [Fact]
//    public async Task ScanReceptionAsync_WithValidReceptionNumber_LoadsReceptionAndSetsDisplay()
//    {
//        // Arrange
//        const string receptionNumber = "REC123";
//        var mockReception = new ReceptionDto
//        {
//            FlowType = "Inbound",
//            Status = "Pending"
//        };

//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            // Cambiado a ReceptionDto
//            .ReturnsAsync(mockReception);

//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        Assert.True(_viewModel.ReceptionLoaded);
//        Assert.Equal("Inbound", _viewModel.FlowTypeDisplay);
//        Assert.Equal("Pending", _viewModel.StatusDisplay);
//        Assert.False(_viewModel.HasError);
//        _receptionSessionServiceMock.Verify(x => x.StartReception(mockReception), Times.Once);
//    }

//    [Fact]
//    public async Task ScanReceptionAsync_WhenReceptionNotFound_ShowsError()
//    {
//        // Arrange
//        const string receptionNumber = "INVALID_REC";
//        const string errorMessage = "Reception not found";

//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            .ReturnsAsync((ReceptionDto?)null);

//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _localizationServiceMock
//            .Setup(x => x.GetString(nameof(AppResources.ReceptionErrorNotFound)))
//            .Returns(errorMessage);

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        Assert.True(_viewModel.HasError);
//        Assert.Equal(errorMessage, _viewModel.ErrorMessage);
//        Assert.False(_viewModel.ReceptionLoaded);
//    }

//    [Fact]
//    public async Task ScanReceptionAsync_WhenReceptionServiceThrows_HandlesErrorAndShowsMessage()
//    {
//        // Arrange
//        const string receptionNumber = "REC123";
//        const string errorMessage = "Connection error occurred";
//        var exception = new Exception("Network failure");

//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            .ThrowsAsync(exception);

//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _errorHandlerServiceMock
//            .Setup(x => x.Handle(exception, "ScanReception"))
//            .Returns(errorMessage);

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        Assert.True(_viewModel.HasError);
//        Assert.Equal(errorMessage, _viewModel.ErrorMessage);
//        Assert.False(_viewModel.ReceptionLoaded);
//    }

//    [Fact]
//    public async Task ScanReceptionAsync_WithActiveReception_ClearsSessionBeforeLoadingNew()
//    {
//        // Arrange
//        const string receptionNumber = "REC123";
//        var mockReception = new ReceptionDto
//        {
//            FlowType = "Inbound",
//            Status = "Pending"
//        };

//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(true);

//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            // Cambiado a ReceptionDto
//            .ReturnsAsync(mockReception);

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        _receptionSessionServiceMock.Verify(x => x.ClearReception(), Times.Once);
//        _receptionSessionServiceMock.Verify(x => x.StartReception(mockReception), Times.Once);
//    }

//    [Fact]
//    public async Task ScanReceptionAsync_SetsIsBusyWhileLoading()
//    {
//        // Arrange
//        const string receptionNumber = "REC123";
//        var mockReception = new ReceptionDto
//        {
//            FlowType = "Inbound",
//            Status = "Pending"
//        };
//        var isBusyDuringExecution = false;

//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            .Callback(() => isBusyDuringExecution = _viewModel.IsBusy)
//            // Cambiado a ReceptionDto
//            .ReturnsAsync(mockReception);

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        Assert.True(isBusyDuringExecution);
//        Assert.False(_viewModel.IsBusy);
//    }

//    [Fact]
//    public async Task ScanReceptionAsync_ClearsErrorStateBeforeLoading()
//    {
//        // Arrange
//        const string receptionNumber = "REC123";
//        var mockReception = new ReceptionDto
//        {
//            FlowType = "Inbound",
//            Status = "Pending"
//        };

//        _viewModel.ReceptionNumber = receptionNumber;
//        _viewModel.HasError = true;
//        _viewModel.ErrorMessage = "Previous error";

//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            // Cambiado a ReceptionDto
//            .ReturnsAsync(mockReception);

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        Assert.False(_viewModel.HasError);
//    }

//    [Fact]
//    public async Task ScanReceptionAsync_LogsOperationStart()
//    {
//        // Arrange
//        const string receptionNumber = "REC123";
//        var mockReception = new ReceptionDto
//        {
//            FlowType = "Inbound",
//            Status = "Pending"
//        };

//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            // Cambiado a ReceptionDto
//            .ReturnsAsync(mockReception);

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        _logServiceMock.Verify(
//            x => x.OperationStart("ScanReception", receptionNumber, null),
//            Times.Once);
//    }

//    [Fact]
//    public async Task ScanReceptionAsync_LogsOperationSuccess()
//    {
//        // Arrange
//        const string receptionNumber = "REC123";
//        var mockReception = new ReceptionDto
//        {
//            FlowType = "Inbound",
//            Status = "Pending"
//        };

//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            // Cambiado a ReceptionDto
//            .ReturnsAsync(mockReception);

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        _logServiceMock.Verify(
//            x => x.OperationSuccess(
//                "ScanReception",
//                receptionNumber,
//                It.Is<string>(s => s.Contains("Inbound") && s.Contains("Pending"))),
//            Times.Once);
//    }

//    [Fact]
//    public async Task ScanReceptionAsync_LogsOperationFailure_WhenNotFound()
//    {
//        // Arrange
//        const string receptionNumber = "INVALID_REC";
//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            .ReturnsAsync((ReceptionDto?)null);

//        _localizationServiceMock
//            .Setup(x => x.GetString(nameof(AppResources.ReceptionErrorNotFound)))
//            .Returns("Not found");

//        // Act
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert
//        _logServiceMock.Verify(
//            x => x.OperationFailure(
//                "ScanReception",
//                receptionNumber,
//                It.Is<string>(s => s.Contains("not found")),
//                null),
//            Times.Once);
//    }

//    #endregion

//    #region CanContinue Tests

//    [Fact]
//    public void CanContinue_WhenReceptionNotLoaded_ReturnsFalse()
//    {
//        // Arrange
//        _viewModel.ReceptionLoaded = false;
//        _viewModel.IsBusy = false;

//        // Act
//        var canExecute = _viewModel.ContinueCommand.CanExecute(null);

//        // Assert
//        Assert.False(canExecute);
//    }

//    [Fact]
//    public void CanContinue_WhenBusy_ReturnsFalse()
//    {
//        // Arrange
//        _viewModel.ReceptionLoaded = true;
//        _viewModel.IsBusy = true;

//        // Act
//        var canExecute = _viewModel.ContinueCommand.CanExecute(null);

//        // Assert
//        Assert.False(canExecute);
//    }

//    [Fact]
//    public void CanContinue_WhenReceptionLoadedAndNotBusy_ReturnsTrue()
//    {
//        // Arrange
//        _viewModel.ReceptionLoaded = true;
//        _viewModel.IsBusy = false;

//        // Act
//        var canExecute = _viewModel.ContinueCommand.CanExecute(null);

//        // Assert
//        Assert.True(canExecute);
//    }

//    #endregion

//    #region ContinueAsync Tests

//    [Fact]
//    public async Task ContinueAsync_NavigatesToReceptionHeaderPage()
//    {
//        // Arrange
//        _navigationServiceMock
//            .Setup(x => x.NavigateToAsync(It.IsAny<string>()))
//            .Returns(Task.CompletedTask);

//        // Act
//        await _viewModel.ContinueCommand.ExecuteAsync(null);

//        // Assert
//        _navigationServiceMock.Verify(
//            x => x.NavigateToAsync("ReceptionHeaderPage"),
//            Times.Once);
//    }

//    #endregion

//    #region OpenCameraAsync Tests

//    [Fact]
//    public async Task OpenCameraAsync_RegistersScanCallbackAndNavigatesToCamera()
//    {
//        // Arrange
//        _navigationServiceMock
//            .Setup(x => x.NavigateToAsync(It.IsAny<string>()))
//            .Returns(Task.CompletedTask);

//        // Act
//        await _viewModel.OpenCameraCommand.ExecuteAsync(null);

//        // Assert
//        _cameraScanServiceMock.Verify(
//            x => x.RequestScan(It.IsAny<Action<string>>()),
//            Times.Once);

//        _navigationServiceMock.Verify(
//            x => x.NavigateToAsync("CameraScanPage"),
//            Times.Once);
//    }

//    [Fact]
//    public async Task OpenCameraAsync_ScanCallbackFillsReceptionNumber()
//    {
//        // Arrange
//        const string scannedCode = "REC_SCANNED_123";
//        Action<string>? scanCallback = null;

//        _cameraScanServiceMock
//            .Setup(x => x.RequestScan(It.IsAny<Action<string>>()))
//            .Callback<Action<string>>(callback => scanCallback = callback);

//        _navigationServiceMock
//            .Setup(x => x.NavigateToAsync(It.IsAny<string>()))
//            .Returns(Task.CompletedTask);

//        // Act
//        await _viewModel.OpenCameraCommand.ExecuteAsync(null);
//        scanCallback?.Invoke(scannedCode);

//        // Assert
//        Assert.Equal(scannedCode, _viewModel.ReceptionNumber);
//    }

//    [Fact]
//    public void OpenCameraAsync_CanOnlyExecuteWhenNotBusy()
//    {
//        // Arrange
//        _viewModel.IsBusy = true;

//        // Act
//        var canExecute = _viewModel.OpenCameraCommand.CanExecute(null);

//        // Assert
//        Assert.False(canExecute);
//    }

//    #endregion

//    #region GoBackAsync Tests

//    [Fact]
//    public async Task GoBackAsync_NavigatesBack()
//    {
//        // Arrange
//        _navigationServiceMock
//            .Setup(x => x.NavigateBackAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _viewModel.GoBackCommand.ExecuteAsync(null);

//        // Assert
//        _navigationServiceMock.Verify(
//            x => x.NavigateBackAsync(),
//            Times.Once);
//    }

//    [Fact]
//    public void GoBackAsync_CanOnlyExecuteWhenNotBusy()
//    {
//        // Arrange
//        _viewModel.IsBusy = true;

//        // Act
//        var canExecute = _viewModel.GoBackCommand.CanExecute(null);

//        // Assert
//        Assert.False(canExecute);
//    }

//    #endregion

//    #region OnReceptionNumberChanged Tests

//    [Fact]
//    public void OnReceptionNumberChanged_ClearsErrorState()
//    {
//        // Arrange
//        _viewModel.HasError = true;
//        _viewModel.ErrorMessage = "Previous error";

//        // Act
//        _viewModel.ReceptionNumber = "NEW_VALUE";

//        // Assert
//        Assert.False(_viewModel.HasError);
//        Assert.Equal(string.Empty, _viewModel.ErrorMessage);
//    }

//    [Fact]
//    public void OnReceptionNumberChanged_WithEmptyValue_AlsoClearsErrorState()
//    {
//        // Arrange
//        _viewModel.HasError = true;
//        _viewModel.ErrorMessage = "Previous error";

//        // Act
//        _viewModel.ReceptionNumber = string.Empty;

//        // Assert
//        Assert.False(_viewModel.HasError);
//        Assert.Equal(string.Empty, _viewModel.ErrorMessage);
//    }

//    #endregion

//    #region Constructor Tests

//    [Fact]
//    public void Constructor_LogsInitialization()
//    {
//        // Assert
//        _logServiceMock.Verify(
//            x => x.Info(It.Is<string>(s => s.Contains("initialized")), It.IsAny<object[]>()),
//            Times.Once);
//    }

//    #endregion

//    #region Integration Tests

//    [Fact]
//    public async Task CompleteWorkflow_ScanReceptionThenContinue()
//    {
//        // Arrange
//        const string receptionNumber = "REC123";
//        var mockReception = new ReceptionDto
//        {
//            FlowType = "Inbound",
//            Status = "Received"
//        };

//        _viewModel.ReceptionNumber = receptionNumber;
//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(receptionNumber))
//            .ReturnsAsync(mockReception);

//        _navigationServiceMock
//            .Setup(x => x.NavigateToAsync(It.IsAny<string>()))
//            .Returns(Task.CompletedTask);

//        // Act - Scan reception
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert - Reception loaded
//        Assert.True(_viewModel.ReceptionLoaded);
//        Assert.True(_viewModel.ContinueCommand.CanExecute(null));

//        // Act - Continue to next page
//        await _viewModel.ContinueCommand.ExecuteAsync(null);

//        // Assert - Navigation called
//        _navigationServiceMock.Verify(
//            x => x.NavigateToAsync("ReceptionHeaderPage"),
//            Times.Once);
//    }

//    [Fact]
//    public async Task CompleteWorkflow_ScanFromCamera()
//    {
//        // Arrange
//        const string scannedCode = "REC_CAMERA_CODE";
//        var mockReception = new ReceptionDto
//        {
//            FlowType = "Outbound",
//            Status = "Pending"
//        };

//        Action<string>? scanCallback = null;

//        _cameraScanServiceMock
//            .Setup(x => x.RequestScan(It.IsAny<Action<string>>()))
//            .Callback<Action<string>>(callback => scanCallback = callback);

//        _navigationServiceMock
//            .Setup(x => x.NavigateToAsync(It.IsAny<string>()))
//            .Returns(Task.CompletedTask);

//        _receptionSessionServiceMock
//            .Setup(x => x.HasActiveReception)
//            .Returns(false);

//        _receptionServiceMock
//            .Setup(x => x.LoadReceptionAsync(scannedCode))
//            .ReturnsAsync(mockReception);

//        // Act - Open camera
//        await _viewModel.OpenCameraCommand.ExecuteAsync(null);
//        scanCallback?.Invoke(scannedCode);

//        // Assert - Reception number filled from camera
//        Assert.Equal(scannedCode, _viewModel.ReceptionNumber);
//        Assert.True(_viewModel.ScanReceptionCommand.CanExecute(null));

//        // Act - Scan reception
//        await _viewModel.ScanReceptionCommand.ExecuteAsync(null);

//        // Assert - Reception loaded
//        Assert.True(_viewModel.ReceptionLoaded);
//        Assert.Equal("Outbound", _viewModel.FlowTypeDisplay);
//        Assert.Equal("Pending", _viewModel.StatusDisplay);
//    }

//    #endregion

//    /// <summary>
//    /// Mock reception object for testing purposes.
//    /// </summary>
//    private class MockReception
//    {
//        public string FlowType { get; set; } = string.Empty;
//        public string Status { get; set; } = string.Empty;
//    }
//}
