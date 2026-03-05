using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services;

public class NavigationServiceTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<INavigationWindowService> _navigationWindowServiceMock;
    private readonly NavigationService _navigationService;

    public NavigationServiceTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _logServiceMock = new Mock<ILogService>();
        _navigationWindowServiceMock = new Mock<INavigationWindowService>();

        _navigationService = new NavigationService(
            _serviceProviderMock.Object,
            _logServiceMock.Object,
            _navigationWindowServiceMock.Object);
    }

    [Fact]
    public void Constructor_LogsInitialization()
    {
        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("initialized")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_LogsRegisteredPagesCount()
    {
        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("RegisteredPages")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task NavigateToAsync_WithUnregisteredPage_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _navigationService.NavigateToAsync("UnregisteredPage"));
    }

    [Fact]
    public async Task NavigateToAsync_WithUnregisteredPage_LogsError()
    {
        // Act
        try
        {
            await _navigationService.NavigateToAsync("UnregisteredPage");
        }
        catch (ArgumentException)
        {
            // Expected
        }

        // Assert
        _logServiceMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("not registered")), It.IsAny<Exception?>()),
            Times.Once);
    }

    [Fact]
    public async Task NavigateToAsync_WithRegisteredPage_PushesPage()
    {
        // Arrange
        var pageMock = new Mock<ContentPage>();
        var navigationPageMock = new Mock<NavigationPage>();

        _navigationWindowServiceMock
            .Setup(x => x.GetNavigationPage())
            .Returns(navigationPageMock.Object);

        _navigationWindowServiceMock
            .Setup(x => x.PushAsync(It.IsAny<Page>()))
            .Returns(Task.CompletedTask);

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(LogiFlow.Mobile.Views.Login.LoginPage)))
            .Returns(pageMock.Object);

        // Act
        await _navigationService.NavigateToAsync("LoginPage");

        // Assert
        _navigationWindowServiceMock.Verify(x => x.PushAsync(It.IsAny<Page>()), Times.Once);
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("Navigation completed")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task NavigateToAsync_WithClearStack_InsertsAndPopsToRoot()
    {
        // Arrange
        var pageMock = new Mock<ContentPage>();
        var navigationPageMock = new Mock<NavigationPage>();
        var rootPageMock = new Mock<ContentPage>();

        _navigationWindowServiceMock
            .Setup(x => x.GetNavigationPage())
            .Returns(navigationPageMock.Object);

        _navigationWindowServiceMock
            .Setup(x => x.NavigationStack)
            .Returns(new List<Page> { rootPageMock.Object });

        _navigationWindowServiceMock
            .Setup(x => x.PopToRootAsync(false))
            .Returns(Task.CompletedTask);

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(LogiFlow.Mobile.Views.Login.LoginPage)))
            .Returns(pageMock.Object);

        // Act
        await _navigationService.NavigateToAsync("LoginPage", clearStack: true);

        // Assert
        _navigationWindowServiceMock.Verify(
            x => x.InsertPageBefore(It.IsAny<Page>(), It.IsAny<Page>()),
            Times.Once);
        _navigationWindowServiceMock.Verify(
            x => x.PopToRootAsync(false),
            Times.Once);
    }

    [Fact]
    public async Task NavigateToAsync_WhenNoNavigationPage_SetsRootPage()
    {
        // Arrange
        var pageMock = new Mock<ContentPage>();

        _navigationWindowServiceMock
            .Setup(x => x.GetNavigationPage())
            .Returns((NavigationPage?)null);

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(LogiFlow.Mobile.Views.Login.LoginPage)))
            .Returns(pageMock.Object);

        // Act
        await _navigationService.NavigateToAsync("LoginPage");

        // Assert
        _navigationWindowServiceMock.Verify(
            x => x.SetRootPage(It.IsAny<Page>()),
            Times.Once);
        _logServiceMock.Verify(
            x => x.Warning(It.Is<string>(s => s.Contains("Setting root page")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task NavigateBackAsync_WhenNoNavigationPage_LogsWarning()
    {
        // Arrange
        _navigationWindowServiceMock
            .Setup(x => x.GetNavigationPage())
            .Returns((NavigationPage?)null);

        // Act
        await _navigationService.NavigateBackAsync();

        // Assert
        _logServiceMock.Verify(
            x => x.Warning(It.Is<string>(s => s.Contains("NavigationPage not found")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task NavigateBackAsync_WithSinglePageStack_LogsWarning()
    {
        // Arrange
        var navigationPageMock = new Mock<NavigationPage>();

        _navigationWindowServiceMock
            .Setup(x => x.GetNavigationPage())
            .Returns(navigationPageMock.Object);

        _navigationWindowServiceMock
            .Setup(x => x.NavigationStack)
            .Returns(new List<Page> { new ContentPage() });

        // Act
        await _navigationService.NavigateBackAsync();

        // Assert
        _logServiceMock.Verify(
            x => x.Warning(It.Is<string>(s => s.Contains("only one page")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task NavigateBackAsync_WithMultiplePages_PopsPage()
    {
        // Arrange
        var navigationPageMock = new Mock<NavigationPage>();

        _navigationWindowServiceMock
            .Setup(x => x.GetNavigationPage())
            .Returns(navigationPageMock.Object);

        _navigationWindowServiceMock
            .Setup(x => x.NavigationStack)
            .Returns(new List<Page> { new ContentPage(), new ContentPage() });

        _navigationWindowServiceMock
            .Setup(x => x.PopAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _navigationService.NavigateBackAsync();

        // Assert
        _navigationWindowServiceMock.Verify(x => x.PopAsync(), Times.Once);
    }

    [Fact]
    public async Task NavigateBackAsync_ToRoot_PopsToRoot()
    {
        // Arrange
        var navigationPageMock = new Mock<NavigationPage>();

        _navigationWindowServiceMock
            .Setup(x => x.GetNavigationPage())
            .Returns(navigationPageMock.Object);

        _navigationWindowServiceMock
            .Setup(x => x.PopToRootAsync(true))
            .Returns(Task.CompletedTask);

        // Act
        await _navigationService.NavigateBackAsync(toRoot: true);

        // Assert
        _navigationWindowServiceMock.Verify(x => x.PopToRootAsync(true), Times.Once);
    }
}
