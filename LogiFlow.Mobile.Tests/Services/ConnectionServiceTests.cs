using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services;

public class ConnectionServiceTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly ConnectionService _connectionService;

    public ConnectionServiceTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _connectionService = new ConnectionService(_logServiceMock.Object);
    }

    [Fact]
    public async Task TestConnectionAsync_WithDemoUrl_ReturnsTrue()
    {
        // Act
        var result = await _connectionService.TestConnectionAsync("https://demo.server.com", 10);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestConnectionAsync_WithNonDemoUrl_ReturnsFalse()
    {
        // Act
        var result = await _connectionService.TestConnectionAsync("https://production.server.com", 10);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TestConnectionAsync_WithEmptyUrl_ReturnsFalse()
    {
        // Act
        var result = await _connectionService.TestConnectionAsync(string.Empty, 10);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TestConnectionAsync_WithDemoUrl_LogsSuccess()
    {
        // Act
        await _connectionService.TestConnectionAsync("https://demo.server.com", 10);

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("successful")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task TestConnectionAsync_WithNonDemoUrl_LogsWarning()
    {
        // Act
        await _connectionService.TestConnectionAsync("https://production.server.com", 10);

        // Assert
        _logServiceMock.Verify(
            x => x.Warning(It.Is<string>(s => s.Contains("failed")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task TestConnectionAsync_WithDemoInUpperCase_ReturnsTrue()
    {
        // Act
        var result = await _connectionService.TestConnectionAsync("https://DEMO.server.com", 10);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestConnectionAsync_WhenUnexpectedErrorOccurs_ReturnsFalse()
    {
        // Arrange
        var logServiceMock = new Mock<ILogService>();
        logServiceMock
            .Setup(x => x.Warning(It.IsAny<string>(), It.IsAny<object[]>()))
            .Throws(new Exception("Unexpected error"));

        var connectionService = new ConnectionService(logServiceMock.Object);

        // Act
        var result = await connectionService.TestConnectionAsync("https://production.server.com", 10);

        // Assert
        Assert.False(result);
        logServiceMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("Unexpected error")), It.IsAny<Exception>()),
            Times.Once);
    }
}
