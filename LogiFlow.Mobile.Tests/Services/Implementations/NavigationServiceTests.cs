using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Microsoft.Maui.Controls;
using Moq;
using Xunit;

namespace LogiFlow.Mobile.Tests.Services.Implementations
{
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

            // Setup default page resolution - always return FakePage instead of trying to instantiate real pages
            _serviceProviderMock
                .Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(new FakePage());

            _navigationWindowServiceMock.SetupGet(n => n.NavigationStack)
                .Returns(new List<Page> { new FakePage() });

            _navigationService = new NavigationService(
                _serviceProviderMock.Object,
                _logServiceMock.Object,
                _navigationWindowServiceMock.Object);
        }

        [Fact]
        public async Task NavigateToAsync_ShouldPushPage_WhenNavigationPageExists_AndClearStackIsFalse()
        {
            // Arrange
            _navigationWindowServiceMock.Setup(n => n.GetNavigationPage()).Returns(new NavigationPage(new FakePage()));
            var pageKey = "CameraScanPage";

            // Act
            await _navigationService.NavigateToAsync(pageKey);

            // Assert
            _navigationWindowServiceMock.Verify(n => n.PushAsync(It.IsAny<Page>()), Times.Once);
            _navigationWindowServiceMock.Verify(n => n.InsertPageBefore(It.IsAny<Page>(), It.IsAny<Page>()), Times.Never);
            _navigationWindowServiceMock.Verify(n => n.PopToRootAsync(It.IsAny<bool>()), Times.Never);
            _navigationWindowServiceMock.Verify(n => n.SetRootPage(It.IsAny<Page>()), Times.Never);
        }

        [Fact]
        public async Task NavigateToAsync_ShouldInsertPageBeforeAndPopToRoot_WhenClearStackIsTrue()
        {
            // Arrange
            _navigationWindowServiceMock.Setup(n => n.GetNavigationPage()).Returns(new NavigationPage(new FakePage()));
            var pageKey = "CameraScanPage";

            // Act
            await _navigationService.NavigateToAsync(pageKey, clearStack: true);

            // Assert
            _navigationWindowServiceMock.Verify(n => n.InsertPageBefore(It.IsAny<Page>(), It.IsAny<Page>()), Times.Once);
            _navigationWindowServiceMock.Verify(n => n.PopToRootAsync(false), Times.Once);
            _navigationWindowServiceMock.Verify(n => n.PushAsync(It.IsAny<Page>()), Times.Never);
            _navigationWindowServiceMock.Verify(n => n.SetRootPage(It.IsAny<Page>()), Times.Never);
        }

        [Fact]
        public async Task NavigateToAsync_ShouldSetRootPage_WhenNavigationPageIsNull()
        {
            // Arrange
            _navigationWindowServiceMock.Setup(n => n.GetNavigationPage()).Returns(() => null!);
            var pageKey = "CameraScanPage";

            // Act
            await _navigationService.NavigateToAsync(pageKey);

            // Assert
            _navigationWindowServiceMock.Verify(n => n.SetRootPage(It.IsAny<Page>()), Times.Once);
            _navigationWindowServiceMock.Verify(n => n.PushAsync(It.IsAny<Page>()), Times.Never);
            _navigationWindowServiceMock.Verify(n => n.InsertPageBefore(It.IsAny<Page>(), It.IsAny<Page>()), Times.Never);
        }

        [Fact]
        public async Task NavigateToAsync_ShouldThrowArgumentException_WhenPageKeyNotRegistered()
        {
            // Arrange
            var invalidPageKey = "NonExistentPage";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _navigationService.NavigateToAsync(invalidPageKey));
            Assert.Contains("Page not registered", ex.Message);
            _logServiceMock.Verify(l => l.Error(It.Is<string>(s => s.Contains("Page not registered")), It.IsAny<Exception?>()), Times.Once);
        }

        [Fact]
        public async Task NavigateBackAsync_ShouldPopToRoot_WhenToRootIsTrue_AndNavigationPageExists()
        {
            // Arrange
            _navigationWindowServiceMock.Setup(n => n.GetNavigationPage()).Returns(new NavigationPage(new FakePage()));

            // Act
            await _navigationService.NavigateBackAsync(toRoot: true);

            // Assert
            _navigationWindowServiceMock.Verify(n => n.PopToRootAsync(true), Times.Once);
            _navigationWindowServiceMock.Verify(n => n.PopAsync(), Times.Never);
        }

        [Fact]
        public async Task NavigateBackAsync_ShouldPopAsync_WhenStackHasMoreThanOnePage()
        {
            // Arrange
            _navigationWindowServiceMock.Setup(n => n.GetNavigationPage()).Returns(new NavigationPage(new FakePage()));
            _navigationWindowServiceMock.SetupGet(n => n.NavigationStack)
                .Returns(new List<Page> { new FakePage(), new FakePage() });

            // Act
            await _navigationService.NavigateBackAsync();

            // Assert
            _navigationWindowServiceMock.Verify(n => n.PopAsync(), Times.Once);
        }

        [Fact]
        public async Task NavigateBackAsync_ShouldLogWarning_WhenStackHasOnlyOnePage()
        {
            // Arrange
            _navigationWindowServiceMock.Setup(n => n.GetNavigationPage()).Returns(new NavigationPage(new FakePage()));
            _navigationWindowServiceMock.SetupGet(n => n.NavigationStack)
                .Returns(new List<Page> { new FakePage() });

            // Act
            await _navigationService.NavigateBackAsync();

            // Assert
            _navigationWindowServiceMock.Verify(n => n.PopAsync(), Times.Never);
            _logServiceMock.Verify(l => l.Warning(It.Is<string>(s => s.Contains("navigation stack has only one page."))), Times.Once);
        }

        [Fact]
        public async Task NavigateBackAsync_ShouldLogWarning_WhenNavigationPageIsNull()
        {
            // Arrange
            _navigationWindowServiceMock.Setup(n => n.GetNavigationPage()).Returns(() => null!);

            // Act
            await _navigationService.NavigateBackAsync();

            // Assert
            _logServiceMock.Verify(l => l.Warning(It.Is<string>(s => s.Contains("NavigationPage not found"))), Times.Once);
        }

        // Helper fake page for testing
        private class FakePage : Page { }
    }
}
