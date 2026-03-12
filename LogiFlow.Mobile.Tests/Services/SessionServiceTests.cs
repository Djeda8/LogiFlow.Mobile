using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services;

public class SessionServiceTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly SessionService _sessionService;

    public SessionServiceTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _sessionService = new SessionService(_logServiceMock.Object);
    }

    [Fact]
    public void HasActiveSession_WhenNoSession_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(_sessionService.HasActiveSession());
    }

    [Fact]
    public void HasActiveSession_AfterCreateSession_ReturnsTrue()
    {
        // Arrange
        var loginResult = new LoginResultDto
        {
            IsSuccess = true,
            UserId = "1",
            UserName = "Admin",
        };

        // Act
        _sessionService.CreateSession(loginResult);

        // Assert
        Assert.True(_sessionService.HasActiveSession());
    }

    [Fact]
    public void HasActiveSession_AfterClearSession_ReturnsFalse()
    {
        // Arrange
        var loginResult = new LoginResultDto
        {
            IsSuccess = true,
            UserId = "1",
            UserName = "Admin",
        };
        _sessionService.CreateSession(loginResult);

        // Act
        _sessionService.ClearSession();

        // Assert
        Assert.False(_sessionService.HasActiveSession());
    }

    [Fact]
    public void CreateSession_LogsSessionCreated()
    {
        // Arrange
        var loginResult = new LoginResultDto
        {
            IsSuccess = true,
            UserId = "1",
            UserName = "Admin",
        };

        // Act
        _sessionService.CreateSession(loginResult);

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("created")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void ClearSession_LogsSessionCleared()
    {
        // Arrange
        var loginResult = new LoginResultDto
        {
            IsSuccess = true,
            UserId = "1",
            UserName = "Admin",
        };
        _sessionService.CreateSession(loginResult);

        // Act
        _sessionService.ClearSession();

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("cleared")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void ClearSession_WithActiveSession_ClearsSession()
    {
        // Arrange
        _sessionService.CreateSession(new LoginResultDto
        {
            IsSuccess = true,
            UserId = "user1",
            UserName = "User One"
        });

        // Act
        _sessionService.ClearSession();

        // Assert
        Assert.False(_sessionService.HasActiveSession());
    }

    [Fact]
    public void ClearSession_WithNoActiveSession_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _sessionService.ClearSession());
        Assert.Null(exception);
    }
}
