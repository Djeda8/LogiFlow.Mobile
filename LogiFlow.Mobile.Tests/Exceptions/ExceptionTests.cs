using LogiFlow.Mobile.Exceptions;

namespace LogiFlow.Mobile.Tests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void ValidationException_StoresFieldAndMessage()
    {
        // Arrange & Act
        var exception = new ValidationException("UrlServidor", "Invalid URL");

        // Assert
        Assert.Equal("UrlServidor", exception.Field);
        Assert.Equal("Invalid URL", exception.Message);
    }

    [Fact]
    public void ValidationException_IsException()
    {
        // Arrange & Act
        var exception = new ValidationException("UrlServidor", "Invalid URL");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void AuthException_StoresUsernameAndMessage()
    {
        // Arrange & Act
        var exception = new AuthException("admin", "Invalid credentials");

        // Assert
        Assert.Equal("admin", exception.Username);
        Assert.Equal("Invalid credentials", exception.Message);
    }

    [Fact]
    public void AuthException_WithInnerException_StoresInnerException()
    {
        // Arrange
        var innerException = new Exception("Inner error");

        // Act
        var exception = new AuthException("admin", "Invalid credentials", innerException);

        // Assert
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void AuthException_IsException()
    {
        // Arrange & Act
        var exception = new AuthException("admin", "Invalid credentials");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void ConnectionException_StoresUrlAndMessage()
    {
        // Arrange & Act
        var exception = new ConnectionException("https://server.com", "Connection failed");

        // Assert
        Assert.Equal("https://server.com", exception.Url);
        Assert.Equal("Connection failed", exception.Message);
    }

    [Fact]
    public void ConnectionException_WithInnerException_StoresInnerException()
    {
        // Arrange
        var innerException = new Exception("Inner error");

        // Act
        var exception = new ConnectionException("https://server.com", "Connection failed", innerException);

        // Assert
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void ConnectionException_IsException()
    {
        // Arrange & Act
        var exception = new ConnectionException("https://server.com", "Connection failed");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}
