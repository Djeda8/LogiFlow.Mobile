using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Exceptions;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Login;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels;

public class LoginViewModelTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ISessionService> _sessionServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly LoginViewModel _viewModel;

    public LoginViewModelTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _sessionServiceMock = new Mock<ISessionService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _logServiceMock = new Mock<ILogService>();
        _errorHandlerServiceMock = new Mock<IErrorHandlerService>();

        _viewModel = new LoginViewModel(
            _authServiceMock.Object,
            _sessionServiceMock.Object,
            _navigationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object);
    }

    [Fact]
    public void CanLogin_WhenUsernameAndPasswordEmpty_ReturnsFalse()
    {
        // Arrange
        _viewModel.Username = string.Empty;
        _viewModel.Password = string.Empty;

        // Act & Assert
        Assert.False(_viewModel.CanLogin);
    }

    [Fact]
    public void CanLogin_WhenUsernameAndPasswordFilled_ReturnsTrue()
    {
        // Arrange
        _viewModel.Username = "admin";
        _viewModel.Password = "1234";

        // Act & Assert
        Assert.True(_viewModel.CanLogin);
    }

    [Fact]
    public void CanLogin_WhenOnlyUsernameIsFilled_ReturnsFalse()
    {
        // Arrange
        _viewModel.Username = "admin";
        _viewModel.Password = string.Empty;

        // Act & Assert
        Assert.False(_viewModel.CanLogin);
    }

    [Fact]
    public void CanLogin_WhenOnlyPasswordIsFilled_ReturnsFalse()
    {
        // Arrange
        _viewModel.Username = string.Empty;
        _viewModel.Password = "1234";

        // Act & Assert
        Assert.False(_viewModel.CanLogin);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_NavigatesToMenu()
    {
        // Arrange
        _viewModel.Username = "admin";
        _viewModel.Password = "1234";

        _authServiceMock
            .Setup(x => x.LoginAsync("admin", "1234"))
            .ReturnsAsync(new LoginResultDto
            {
                IsSuccess = true,
                UserId = "1",
                UserName = "Admin",
            });

        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.HasError);
        Assert.False(_viewModel.UsernameHasError);
        Assert.False(_viewModel.PasswordHasError);
        _navigationServiceMock.Verify(
            x => x.NavigateToAsync("MenuPage", true),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShowsError()
    {
        // Arrange
        _viewModel.Username = "admin";
        _viewModel.Password = "wrong";

        _authServiceMock
            .Setup(x => x.LoginAsync("admin", "wrong"))
            .ReturnsAsync(new LoginResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Invalid credentials",
            });

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<AuthException>()))
            .Returns("Invalid credentials");

        // Act
        await _viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
        Assert.True(_viewModel.UsernameHasError);
        Assert.True(_viewModel.PasswordHasError);
        Assert.Equal("Invalid credentials", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WhenServiceThrows_ShowsUnexpectedError()
    {
        // Arrange
        _viewModel.Username = "admin";
        _viewModel.Password = "1234";

        _authServiceMock
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Network error"));

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("An unexpected error occurred. Please try again.");

        // Act
        await _viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
        Assert.Equal("An unexpected error occurred. Please try again.", _viewModel.ErrorMessage);
    }

    [Fact]
    public void OnUsernameChanged_ClearsUsernameError()
    {
        // Arrange
        _viewModel.UsernameHasError = true;

        // Act
        _viewModel.Username = "newvalue";

        // Assert
        Assert.False(_viewModel.UsernameHasError);
    }

    [Fact]
    public void OnPasswordChanged_ClearsPasswordError()
    {
        // Arrange
        _viewModel.PasswordHasError = true;

        // Act
        _viewModel.Password = "newvalue";

        // Assert
        Assert.False(_viewModel.PasswordHasError);
    }

    [Fact]
    public async Task LoginAsync_WhenCanLoginIsFalse_DoesNotCallAuthService()
    {
        // Arrange
        _viewModel.Username = string.Empty;
        _viewModel.Password = string.Empty;

        // Act
        await _viewModel.LoginCommand.ExecuteAsync(null);

        // Assert
        _authServiceMock.Verify(
            x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}
