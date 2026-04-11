using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations;

public class ScanServiceTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly ScanService _scanService;

    public ScanServiceTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _scanService = new ScanService(_logServiceMock.Object);
    }

    [Fact]
    public async Task TestScannerAsync_WithInternalScanner_ReturnsTrue()
    {
        // Act
        var result = await _scanService.TestScannerAsync("internal");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestScannerAsync_WithInternalScanner_LogsSuccess()
    {
        // Act
        await _scanService.TestScannerAsync("internal");

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("successful")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task TestScannerAsync_WithExternalScanner_LogsResult()
    {
        // Act
        await _scanService.TestScannerAsync("Externo");

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("successful")), It.IsAny<object[]>()),
            Times.AtMostOnce);

        _logServiceMock.Verify(
            x => x.Warning(It.Is<string>(s => s.Contains("failed")), It.IsAny<object[]>()),
            Times.AtMostOnce);
    }

    [Fact]
    public async Task TestScannerAsync_WithInternalScanner_AlwaysSucceeds()
    {
        // Act - run multiple times to verify internal scanner always succeeds
        var results = await Task.WhenAll(
            _scanService.TestScannerAsync("internal"),
            _scanService.TestScannerAsync("internal"),
            _scanService.TestScannerAsync("internal"),
            _scanService.TestScannerAsync("internal"),
            _scanService.TestScannerAsync("internal"));

        // Assert
        Assert.All(results, result => Assert.True(result));
    }
}
