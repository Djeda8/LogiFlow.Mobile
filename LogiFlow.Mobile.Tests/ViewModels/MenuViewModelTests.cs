using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Menu;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels;

public class MenuViewModelTests
{
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ISessionService> _sessionServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly MenuViewModel _viewModel;

    public MenuViewModelTests()
    {
        _navigationServiceMock = new Mock<INavigationService>();
        _sessionServiceMock = new Mock<ISessionService>();
        _logServiceMock = new Mock<ILogService>();
        _errorHandlerServiceMock = new Mock<IErrorHandlerService>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => $"Translated_{key}");

        _viewModel = new MenuViewModel(
            _navigationServiceMock.Object,
            _sessionServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object,
            _localizationServiceMock.Object);
    }

    [Fact]
    public void Constructor_InitializesMenuItems()
    {
        // Assert
        Assert.NotEmpty(_viewModel.MenuItems);
        Assert.Equal(6, _viewModel.MenuItems.Count);
    }

    [Fact]
    public void Constructor_MenuItemsHaveTranslatedTitles()
    {
        // Assert
        Assert.Contains(_viewModel.MenuItems, x => x.Title == "Translated_MenuReception");
        Assert.Contains(_viewModel.MenuItems, x => x.Title == "Translated_MenuMovements");
        Assert.Contains(_viewModel.MenuItems, x => x.Title == "Translated_MenuPicking");
        Assert.Contains(_viewModel.MenuItems, x => x.Title == "Translated_MenuInventory");
        Assert.Contains(_viewModel.MenuItems, x => x.Title == "Translated_MenuItemInfo");
        Assert.Contains(_viewModel.MenuItems, x => x.Title == "Translated_MenuSettings");
    }

    [Fact]
    public void LanguageChanged_RebuildMenuItems()
    {
        // Arrange
        _localizationServiceMock
            .Setup(x => x.GetString("MenuReception"))
            .Returns("Reception");

        // Act
        _localizationServiceMock.Raise(x => x.LanguageChanged += null, EventArgs.Empty);

        // Assert
        Assert.Contains(_viewModel.MenuItems, x => x.Title == "Reception");
    }

    [Fact]
    public async Task NavigateAsync_WithValidRoute_CallsNavigationService()
    {
        // Arrange
        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.NavigateCommand.ExecuteAsync("SettingsPage");

        // Assert
        _navigationServiceMock.Verify(
            x => x.NavigateToAsync("SettingsPage", false),
            Times.Once);
    }

    [Fact]
    public async Task NavigateAsync_WhenNavigationFails_ShowsError()
    {
        // Arrange
        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("Navigation failed"));

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("An unexpected error occurred. Please try again.");

        // Act
        await _viewModel.NavigateCommand.ExecuteAsync("SettingsPage");

        // Assert
        Assert.True(_viewModel.HasError);
        Assert.Equal("An unexpected error occurred. Please try again.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LogoutAsync_ClearsSession()
    {
        // Arrange
        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.LogoutCommand.ExecuteAsync(null);

        // Assert
        _sessionServiceMock.Verify(x => x.ClearSession(), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_NavigatesToLogin()
    {
        // Arrange
        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.LogoutCommand.ExecuteAsync(null);

        // Assert
        _navigationServiceMock.Verify(
            x => x.NavigateToAsync("LoginPage", true),
            Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_WhenFails_ShowsError()
    {
        // Arrange
        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("Navigation failed"));

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("An unexpected error occurred. Please try again.");

        // Act
        await _viewModel.LogoutCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
    }

    [Fact]
    public void Constructor_LogsInitialization()
    {
        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("initialized")), It.IsAny<object[]>()),
            Times.Once);
    }
}
