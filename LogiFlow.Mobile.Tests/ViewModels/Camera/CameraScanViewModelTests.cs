using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Camera;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels.Camera;

/// <summary>
/// Unit tests for <see cref="CameraScanViewModel"/>.
/// Covers barcode detection, cancellation and navigation behavior.
/// </summary>
public class CameraScanViewModelTests
{
    private readonly Mock<ICameraScanService> _cameraScanServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly CameraScanViewModel _sut;

    public CameraScanViewModelTests()
    {
        _cameraScanServiceMock = new Mock<ICameraScanService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _logServiceMock = new Mock<ILogService>();

        _sut = new CameraScanViewModel(
            _cameraScanServiceMock.Object,
            _navigationServiceMock.Object,
            _logServiceMock.Object);
    }

    // --- Initial state ---

    [Fact]
    public void InitialState_IsScanningIsTrue()
    {
        Assert.True(_sut.IsScanning);
    }

    [Fact]
    public void InitialState_LastScannedCodeIsEmpty()
    {
        Assert.Equal(string.Empty, _sut.LastScannedCode);
    }

    // --- BarcodeDetectedAsync ---

    [Fact]
    public async Task BarcodeDetectedAsync_WhenValidCode_StopsScanning()
    {
        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-001");

        Assert.False(_sut.IsScanning);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenValidCode_SetsLastScannedCode()
    {
        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-001");

        Assert.Equal("REC-001", _sut.LastScannedCode);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenValidCode_NavigatesBackBeforeDelivering()
    {
        var callOrder = new List<string>();

        _navigationServiceMock
            .Setup(x => x.NavigateBackAsync(false))
            .Callback(() => callOrder.Add("NavigateBack"))
            .Returns(Task.CompletedTask);

        _cameraScanServiceMock
            .Setup(x => x.DeliverResult(It.IsAny<string>()))
            .Callback<string>(_ => callOrder.Add("DeliverResult"));

        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-001");

        Assert.Equal("NavigateBack", callOrder[0]);
        Assert.Equal("DeliverResult", callOrder[1]);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenValidCode_DeliversResultWithCode()
    {
        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-001");

        _cameraScanServiceMock.Verify(x => x.DeliverResult("REC-001"), Times.Once);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenValidCode_NavigatesBack()
    {
        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-001");

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(false), Times.Once);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenEmptyCode_DoesNotNavigate()
    {
        await _sut.BarcodeDetectedCommand.ExecuteAsync(string.Empty);

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(
            It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenEmptyCode_DoesNotDeliverResult()
    {
        await _sut.BarcodeDetectedCommand.ExecuteAsync(string.Empty);

        _cameraScanServiceMock.Verify(x => x.DeliverResult(
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenWhitespaceCode_DoesNotNavigate()
    {
        await _sut.BarcodeDetectedCommand.ExecuteAsync("   ");

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(
            It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenNotScanning_DoesNotDeliverResult()
    {
        // First scan stops scanning
        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-001");

        // Second scan should be ignored
        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-002");

        _cameraScanServiceMock.Verify(x => x.DeliverResult(
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task BarcodeDetectedAsync_WhenNotScanning_DoesNotNavigateAgain()
    {
        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-001");
        await _sut.BarcodeDetectedCommand.ExecuteAsync("REC-002");

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(
            It.IsAny<bool>()), Times.Once);
    }

    // --- CancelAsync ---

    [Fact]
    public async Task CancelAsync_StopsScanning()
    {
        await _sut.CancelCommand.ExecuteAsync(null);

        Assert.False(_sut.IsScanning);
    }

    [Fact]
    public async Task CancelAsync_CancelsScan()
    {
        await _sut.CancelCommand.ExecuteAsync(null);

        _cameraScanServiceMock.Verify(x => x.CancelScan(), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_NavigatesBack()
    {
        await _sut.CancelCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(false), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_DoesNotDeliverResult()
    {
        await _sut.CancelCommand.ExecuteAsync(null);

        _cameraScanServiceMock.Verify(x => x.DeliverResult(
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CancelAsync_DoesNotSetLastScannedCode()
    {
        await _sut.CancelCommand.ExecuteAsync(null);

        Assert.Equal(string.Empty, _sut.LastScannedCode);
    }
}
