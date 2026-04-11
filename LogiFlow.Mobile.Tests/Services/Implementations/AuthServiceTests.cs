using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations;

public class AuthServiceTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => $"Translated_{key}");

        _authService = new AuthService(
            _logServiceMock.Object,
            _localizationServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Act
        var result = await _authService.LoginAsync("admin", "1234");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("1", result.UserId);
        Assert.Equal("Admin", result.UserName);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsFailure()
    {
        // Act
        var result = await _authService.LoginAsync("admin", "wrong");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Translated_ErrorInvalidCredentials", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithEmptyUsername_ReturnsFailure()
    {
        // Act
        var result = await _authService.LoginAsync(string.Empty, "1234");

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task LoginAsync_WithEmptyPassword_ReturnsFailure()
    {
        // Act
        var result = await _authService.LoginAsync("admin", string.Empty);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_LogsSuccess()
    {
        // Act
        await _authService.LoginAsync("admin", "1234");

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("successful")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_LogsWarning()
    {
        // Act
        await _authService.LoginAsync("admin", "wrong");

        // Assert
        _logServiceMock.Verify(
            x => x.Warning(It.Is<string>(s => s.Contains("failed")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUnexpectedErrorOccurs_ReturnsFailureWithLocalizedMessage()
    {
        // Arrange
        var logServiceMock = new Mock<ILogService>();
        logServiceMock
            .Setup(x => x.Warning(It.IsAny<string>(), It.IsAny<object[]>()))
            .Throws(new Exception("Unexpected error"));

        var authService = new AuthService(logServiceMock.Object, _localizationServiceMock.Object);

        // Act
        var result = await authService.LoginAsync("admin", "wrong");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Translated_ErrorUnexpectedAuth", result.ErrorMessage);
    }
}
