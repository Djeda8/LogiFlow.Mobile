using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Splash;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels;

public class SplashViewModelTests
{
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ISessionService> _sessionServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;

    private readonly SplashViewModel _viewModel;

    public SplashViewModelTests()
    {
        _navigationServiceMock = new Mock<INavigationService>();
        _sessionServiceMock = new Mock<ISessionService>();
        _logServiceMock = new Mock<ILogService>();
        _settingsServiceMock = new Mock<ISettingsService>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _settingsServiceMock
            .Setup(x => x.LoadSettings())
            .Returns(new SettingsDto());

        _viewModel = new SplashViewModel(
            _navigationServiceMock.Object,
            _sessionServiceMock.Object,
            _logServiceMock.Object,
            _settingsServiceMock.Object,
            _localizationServiceMock.Object);
    }

    [Fact]
    public async Task StartAsync_WhenNoActiveSession_NavigatesToLogin()
    {
        // Arrange
        _sessionServiceMock
            .Setup(x => x.HasActiveSession())
            .Returns(false);

        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.StartAsync();

        // Assert
        _navigationServiceMock.Verify(
            x => x.NavigateToAsync("LoginPage", false),
            Times.Once);
    }

    [Fact]
    public async Task StartAsync_WhenActiveSession_NavigatesToMenu()
    {
        // Arrange
        _sessionServiceMock
            .Setup(x => x.HasActiveSession())
            .Returns(true);

        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.StartAsync();

        // Assert
        _navigationServiceMock.Verify(
            x => x.NavigateToAsync("MenuPage", false),
            Times.Once);
    }

    [Fact]
    public async Task StartAsync_WhenNavigationThrows_LogsError()
    {
        // Arrange
        _sessionServiceMock
            .Setup(x => x.HasActiveSession())
            .Returns(false);

        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("Navigation failed"));

        // Act
        await _viewModel.StartAsync();

        // Assert
        _logServiceMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("startup")), It.IsAny<Exception>()),
            Times.Once);
    }

    [Fact]
    public async Task StartAsync_LogsApplicationStarted()
    {
        // Arrange
        _sessionServiceMock
            .Setup(x => x.HasActiveSession())
            .Returns(false);

        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.StartAsync();

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("started")), It.IsAny<object[]>()),
            Times.Once);
    }
}
