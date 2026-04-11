using LogiFlow.Mobile.Exceptions;
using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations;

public class ErrorHandlerServiceTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly ErrorHandlerService _errorHandlerService;

    public ErrorHandlerServiceTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => $"Translated_{key}");

        _errorHandlerService = new ErrorHandlerService(
            _logServiceMock.Object,
            _localizationServiceMock.Object);
    }

    [Fact]
    public void Handle_ValidationException_ReturnsExceptionMessage()
    {
        // Arrange
        var exception = new ValidationException("UrlServidor", "Invalid URL");

        // Act
        var result = _errorHandlerService.Handle(exception);

        // Assert
        Assert.Equal("Invalid URL", result);
    }

    [Fact]
    public void Handle_ValidationException_LogsWarning()
    {
        // Arrange
        var exception = new ValidationException("UrlServidor", "Invalid URL");

        // Act
        _errorHandlerService.Handle(exception);

        // Assert
        _logServiceMock.Verify(
            x => x.Warning(It.Is<string>(s => s.Contains("Validation")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void Handle_AuthException_ReturnsExceptionMessage()
    {
        // Arrange
        var exception = new AuthException("admin", "Invalid credentials");

        // Act
        var result = _errorHandlerService.Handle(exception);

        // Assert
        Assert.Equal("Invalid credentials", result);
    }

    [Fact]
    public void Handle_AuthException_LogsWarning()
    {
        // Arrange
        var exception = new AuthException("admin", "Invalid credentials");

        // Act
        _errorHandlerService.Handle(exception);

        // Assert
        _logServiceMock.Verify(
            x => x.Warning(It.Is<string>(s => s.Contains("Authentication")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void Handle_ConnectionException_ReturnsLocalizedMessage()
    {
        // Arrange
        var exception = new ConnectionException("https://server.com", "Connection failed");

        // Act
        var result = _errorHandlerService.Handle(exception);

        // Assert
        Assert.Equal("Translated_Msg_TestErrorConnection", result);
        _localizationServiceMock.Verify(
            x => x.GetString("Msg_TestErrorConnection"),
            Times.Once);
    }

    [Fact]
    public void Handle_ConnectionException_LogsError()
    {
        // Arrange
        var exception = new ConnectionException("https://server.com", "Connection failed");

        // Act
        _errorHandlerService.Handle(exception);

        // Assert
        _logServiceMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("Connection")), It.IsAny<Exception>()),
            Times.Once);
    }

    [Fact]
    public void Handle_UnexpectedException_ReturnsLocalizedMessage()
    {
        // Arrange
        var exception = new Exception("Something went wrong");

        // Act
        var result = _errorHandlerService.Handle(exception, "TestContext");

        // Assert
        Assert.Equal("Translated_ErrorUnexpected", result);
        _localizationServiceMock.Verify(
            x => x.GetString("ErrorUnexpected"),
            Times.Once);
    }

    [Fact]
    public void Handle_UnexpectedException_LogsError()
    {
        // Arrange
        var exception = new Exception("Something went wrong");

        // Act
        _errorHandlerService.Handle(exception, "TestContext");

        // Assert
        _logServiceMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("Unexpected")), It.IsAny<Exception>()),
            Times.Once);
    }

    [Fact]
    public void Handle_UnexpectedExceptionWithoutContext_ReturnsLocalizedMessage()
    {
        // Arrange
        var exception = new Exception("Something went wrong");

        // Act
        var result = _errorHandlerService.Handle(exception);

        // Assert
        Assert.Equal("Translated_ErrorUnexpected", result);
    }
}
