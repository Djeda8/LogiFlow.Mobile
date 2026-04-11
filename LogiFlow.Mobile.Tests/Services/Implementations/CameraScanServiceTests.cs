using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations;

/// <summary>
/// Unit tests for <see cref="CameraScanService"/>.
/// </summary>
public class CameraScanServiceTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly CameraScanService _sut;

    public CameraScanServiceTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _sut = new CameraScanService(_logServiceMock.Object);
    }

    [Fact]
    public void HasPendingRequest_WhenNoPendingCallback_ReturnsFalse()
    {
        Assert.False(_sut.HasPendingRequest);
    }

    [Fact]
    public void HasPendingRequest_WhenCallbackRegistered_ReturnsTrue()
    {
        _sut.RequestScan(_ => { });

        Assert.True(_sut.HasPendingRequest);
    }

    [Fact]
    public void RequestScan_RegistersCallback()
    {
        var called = false;
        _sut.RequestScan(_ => called = true);

        Assert.True(_sut.HasPendingRequest);
    }

    [Fact]
    public void RequestScan_LogsInfo()
    {
        _sut.RequestScan(_ => { });

        _logServiceMock.Verify(x => x.Info(
            It.IsAny<string>(),
            It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void DeliverResult_WhenPendingCallback_InvokesCallbackWithCode()
    {
        var receivedCode = string.Empty;
        _sut.RequestScan(code => receivedCode = code);

        _sut.DeliverResult("REC-001");

        Assert.Equal("REC-001", receivedCode);
    }

    [Fact]
    public void DeliverResult_WhenPendingCallback_ClearsPendingRequest()
    {
        _sut.RequestScan(_ => { });

        _sut.DeliverResult("REC-001");

        Assert.False(_sut.HasPendingRequest);
    }

    [Fact]
    public void DeliverResult_WhenNoPendingCallback_LogsWarning()
    {
        _sut.DeliverResult("REC-001");

        _logServiceMock.Verify(x => x.Warning(
            It.IsAny<string>(),
            It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void DeliverResult_WhenNoPendingCallback_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.DeliverResult("REC-001"));

        Assert.Null(exception);
    }

    [Fact]
    public void CancelScan_ClearsPendingRequest()
    {
        _sut.RequestScan(_ => { });

        _sut.CancelScan();

        Assert.False(_sut.HasPendingRequest);
    }

    [Fact]
    public void CancelScan_LogsInfo()
    {
        _sut.RequestScan(_ => { });
        _sut.CancelScan();

        _logServiceMock.Verify(x => x.Info(
            It.IsAny<string>(),
            It.IsAny<object[]>()), Times.Exactly(2));
    }

    [Fact]
    public void CancelScan_WhenNoPendingCallback_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.CancelScan());

        Assert.Null(exception);
    }

    [Fact]
    public void RequestScan_ReplacesExistingCallback()
    {
        var firstCalled = false;
        var secondCalled = false;

        _sut.RequestScan(_ => firstCalled = true);
        _sut.RequestScan(_ => secondCalled = true);

        _sut.DeliverResult("REC-001");

        Assert.False(firstCalled);
        Assert.True(secondCalled);
    }

    [Fact]
    public void DeliverResult_AfterCancel_LogsWarning()
    {
        _sut.RequestScan(_ => { });
        _sut.CancelScan();

        _sut.DeliverResult("REC-001");

        _logServiceMock.Verify(x => x.Warning(
            It.IsAny<string>(),
            It.IsAny<object[]>()), Times.Once);
    }
}
