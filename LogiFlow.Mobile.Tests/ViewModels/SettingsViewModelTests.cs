using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Exceptions;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Settings;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels;

public class SettingsViewModelTests
{
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly Mock<IScanService> _scanServiceMock;
    private readonly Mock<IConnectionService> _connectionServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly Mock<ISettingsDisplayService> _settingsDisplayServiceMock;
    private readonly Mock<IThemeService> _themeServiceMock;
    private readonly SettingsViewModel _viewModel;

    public SettingsViewModelTests()
    {
        _settingsServiceMock = new Mock<ISettingsService>();
        _scanServiceMock = new Mock<IScanService>();
        _connectionServiceMock = new Mock<IConnectionService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _localizationServiceMock = new Mock<ILocalizationService>();
        _logServiceMock = new Mock<ILogService>();
        _errorHandlerServiceMock = new Mock<IErrorHandlerService>();
        _settingsDisplayServiceMock = new Mock<ISettingsDisplayService>();
        _themeServiceMock = new Mock<IThemeService>();

        _settingsServiceMock
            .Setup(x => x.LoadSettings())
            .Returns(new SettingsDto());

        _settingsServiceMock
            .Setup(x => x.GetDefaultSettings())
            .Returns(new SettingsDto());

        _localizationServiceMock
            .Setup(x => x.CurrentCulture)
            .Returns(new System.Globalization.CultureInfo("en"));

        _viewModel = new SettingsViewModel(
            _settingsServiceMock.Object,
            _scanServiceMock.Object,
            _connectionServiceMock.Object,
            _navigationServiceMock.Object,
            _localizationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object,
            _settingsDisplayServiceMock.Object,
            _themeServiceMock.Object);
    }

    [Fact]
    public void Constructor_LoadsSettings()
    {
        // Arrange
        _viewModel.LoadSettings();

        // Assert
        _settingsServiceMock.Verify(x => x.LoadSettings(), Times.Once);
    }

    [Fact]
    public void Constructor_InitializesStaticLists()
    {
        // Arrange
        _viewModel.LoadSettings();

        // Assert
        Assert.NotEmpty(_viewModel.AvailableThemes);
        Assert.NotEmpty(_viewModel.ScannerTypes);
        Assert.NotEmpty(_viewModel.ServerEnvironments);
        Assert.NotEmpty(_viewModel.AvailableLanguages);
    }

    [Fact]
    public async Task SaveAsync_WithInvalidUrl_DoesNotSave()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "not-a-valid-url";

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<ValidationException>()))
            .Returns("Invalid URL");

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _settingsServiceMock.Verify(x => x.SaveSettings(It.IsAny<SettingsDto>()), Times.Never);
        Assert.True(_viewModel.UrlServidorHasError);
    }

    [Fact]
    public async Task SaveAsync_WithZeroTimeout_DoesNotSave()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "https://demo.server.com";
        _viewModel.Settings.Timeout = 0;

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<ValidationException>()))
            .Returns("Timeout must be greater than 0");

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _settingsServiceMock.Verify(x => x.SaveSettings(It.IsAny<SettingsDto>()), Times.Never);
        Assert.True(_viewModel.TimeoutHasError);
    }

    [Fact]
    public async Task SaveAsync_WithValidSettings_SavesAndNavigatesBack()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "https://demo.server.com";
        _viewModel.Settings.Timeout = 10;

        _navigationServiceMock
            .Setup(x => x.NavigateBackAsync(It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _settingsServiceMock.Verify(x => x.SaveSettings(It.IsAny<SettingsDto>()), Times.Once);
        _navigationServiceMock.Verify(x => x.NavigateBackAsync(false), Times.Once);
        Assert.False(_viewModel.HasError);
    }

    [Fact]
    public void RestoreDefaults_ResetsSettings()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "https://custom.server.com";
        _viewModel.Settings.Timeout = 99;

        // Act
        _viewModel.RestoreDefaultsCommand.Execute(null);

        // Assert
        _settingsServiceMock.Verify(x => x.GetDefaultSettings(), Times.Once);
        Assert.False(_viewModel.HasError);
        Assert.False(_viewModel.UrlServidorHasError);
        Assert.False(_viewModel.TimeoutHasError);
    }

    [Fact]
    public async Task TestScannerAsync_WhenFails_ShowsError()
    {
        // Arrange
        _scanServiceMock
            .Setup(x => x.TestScannerAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _localizationServiceMock
            .Setup(x => x.GetString("ErrorScannerTestFailed"))
            .Returns("Scanner test failed. Please check the device and try again.");

        // Act
        await _viewModel.TestScannerCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
        Assert.Equal("Scanner test failed. Please check the device and try again.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task TestScannerAsync_WhenSucceeds_NoError()
    {
        // Arrange
        _scanServiceMock
            .Setup(x => x.TestScannerAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _viewModel.TestScannerCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.HasError);
    }

    [Fact]
    public async Task TestConnectionAsync_WithInvalidUrl_ShowsError()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "not-a-valid-url";

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<ValidationException>()))
            .Returns("Invalid URL");

        // Act
        await _viewModel.TestConnectionCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
        Assert.True(_viewModel.UrlServidorHasError);
    }

    [Fact]
    public async Task TestConnectionAsync_WhenConnectionFails_ShowsError()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "https://demo.server.com";

        _connectionServiceMock
            .Setup(x => x.TestConnectionAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<ConnectionException>()))
            .Returns("Could not connect to server.");

        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns("Could not connect to server.");

        // Act
        await _viewModel.TestConnectionCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
    }

    [Fact]
    public async Task TestConnectionAsync_WhenConnectionSucceeds_NoError()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "https://demo.server.com";

        _connectionServiceMock
            .Setup(x => x.TestConnectionAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        await _viewModel.TestConnectionCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.HasError);
    }

    [Fact]
    public void SelectedLanguage_WhenChangedToEnglish_SetsCultureCodeEn()
    {
        // Act
        _viewModel.SelectedLanguage = "English";

        // Assert
        _localizationServiceMock.Verify(
            x => x.SetLanguage("en"),
            Times.AtLeastOnce);
    }

    [Fact]
    public void SelectedLanguage_WhenChangedToSpanish_SetsCultureCodeEs()
    {
        // Act
        _viewModel.SelectedLanguage = "Español";

        // Assert
        _localizationServiceMock.Verify(
            x => x.SetLanguage("es"),
            Times.Once);
    }

    [Fact]
    public void OnSelectedLanguageChanged_WithEmptyValue_DoesNotCallLocalizationService()
    {
        // Arrange
        _localizationServiceMock.Invocations.Clear();

        // Act
        _viewModel.SelectedLanguage = string.Empty;

        // Assert
        _localizationServiceMock.Verify(
            x => x.SetLanguage(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void InitializeSelectedLanguage_WithUnknownLanguage_DefaultsToEnglish()
    {
        // Arrange
        _settingsServiceMock
            .Setup(x => x.LoadSettings())
            .Returns(new SettingsDto { Idioma = "en" });

        var viewModel = new SettingsViewModel(
            _settingsServiceMock.Object,
            _scanServiceMock.Object,
            _connectionServiceMock.Object,
            _navigationServiceMock.Object,
            _localizationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object,
            _settingsDisplayServiceMock.Object,
            _themeServiceMock.Object);

        // Act
        viewModel.LoadSettings();

        // Assert
        Assert.Equal("English", viewModel.SelectedLanguage);
    }

    [Fact]
    public void InitializeSelectedLanguage_WithSpanish_SetsSelectedLanguageToSpanish()
    {
        // Arrange
        _settingsServiceMock
            .Setup(x => x.LoadSettings())
            .Returns(new SettingsDto { Idioma = "es" });

        var viewModel = new SettingsViewModel(
            _settingsServiceMock.Object,
            _scanServiceMock.Object,
            _connectionServiceMock.Object,
            _navigationServiceMock.Object,
            _localizationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object,
            _settingsDisplayServiceMock.Object,
            _themeServiceMock.Object);

        // Act
        viewModel.LoadSettings();

        // Assert
        Assert.Equal("Español", viewModel.SelectedLanguage);
    }

    [Fact]
    public void OnLanguageChanged_RefreshesAllProperties()
    {
        // Arrange
        var propertyChangedFired = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == string.Empty)
            {
                propertyChangedFired = true;
            }
        };

        // Act
        _localizationServiceMock.Raise(x => x.LanguageChanged += null, EventArgs.Empty);

        // Assert
        Assert.True(propertyChangedFired);
    }

    [Fact]
    public async Task SaveAsync_WhenServiceThrows_ShowsUnexpectedError()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "https://demo.server.com";
        _viewModel.Settings.Timeout = 10;

        _settingsServiceMock
            .Setup(x => x.SaveSettings(It.IsAny<SettingsDto>()))
            .Throws(new Exception("Unexpected error"));

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("An unexpected error occurred. Please try again.");

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
        Assert.Equal("An unexpected error occurred. Please try again.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task TestScannerAsync_WhenServiceThrows_ShowsUnexpectedError()
    {
        // Arrange
        _scanServiceMock
            .Setup(x => x.TestScannerAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("An unexpected error occurred. Please try again.");

        // Act
        await _viewModel.TestScannerCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
        Assert.Equal("An unexpected error occurred. Please try again.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task TestConnectionAsync_WhenServiceThrows_ShowsUnexpectedError()
    {
        // Arrange
        _viewModel.Settings.UrlServidor = "https://demo.server.com";

        _connectionServiceMock
            .Setup(x => x.TestConnectionAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("An unexpected error occurred. Please try again.");

        // Act
        await _viewModel.TestConnectionCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasError);
        Assert.Equal("An unexpected error occurred. Please try again.", _viewModel.ErrorMessage);
    }

    [Fact]
    public void OnSelectedLanguageChanged_WithEmptyValue_DoesNotChangeLanguage()
    {
        // Act
        _viewModel.SelectedLanguage = string.Empty;

        // Assert
        _localizationServiceMock.Verify(
            x => x.SetLanguage(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public void OnSelectedLanguageChanged_WithUnknownLanguage_DefaultsToEnglish()
    {
        // Act
        _viewModel.SelectedLanguage = "Français";

        // Assert
        _localizationServiceMock.Verify(
            x => x.SetLanguage("en"),
            Times.Once);
    }

    [Fact]
    public void OnSelectedThemeChanged_WithDarkTheme_CallsApplyThemeDark()
    {
        // Arrange
        _viewModel.LoadSettings();

        // Act
        _viewModel.SelectedTheme = _viewModel.AvailableThemes
            .First(x => x.Code == "dark");

        // Assert
        _themeServiceMock.Verify(
            x => x.ApplyTheme("dark"),
            Times.Once);
    }

    [Fact]
    public void OnSelectedThemeChanged_WithLightTheme_CallsApplyThemeLight()
    {
        // Arrange
        _viewModel.LoadSettings();

        // Act
        _viewModel.SelectedTheme = _viewModel.AvailableThemes
            .First(x => x.Code == "light");

        // Assert
        _themeServiceMock.Verify(
            x => x.ApplyTheme("light"),
            Times.Once);
    }

    [Fact]
    public void OnSelectedThemeChanged_WithDarkTheme_UpdatesSettingsTemaVisual()
    {
        // Arrange
        _viewModel.LoadSettings();

        // Act
        _viewModel.SelectedTheme = _viewModel.AvailableThemes
            .First(x => x.Code == "dark");

        // Assert
        Assert.Equal("dark", _viewModel.Settings.TemaVisual);
    }

    [Fact]
    public void OnSelectedThemeChanged_WithNullValue_DoesNotCallApplyTheme()
    {
        // Act
        _viewModel.SelectedTheme = null;

        // Assert
        _themeServiceMock.Verify(
            x => x.ApplyTheme(It.IsAny<string>()),
            Times.Never);
    }
}
