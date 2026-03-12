using System.Text.Json;
using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services;

public class SettingsServiceTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IPreferencesService> _preferencesServiceMock;

    public SettingsServiceTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _preferencesServiceMock = new Mock<IPreferencesService>();

        _preferencesServiceMock
            .Setup(x => x.ContainsKey(It.IsAny<string>()))
            .Returns(false);
    }

    private SettingsService CreateService() =>
        new(_logServiceMock.Object, _preferencesServiceMock.Object);

    [Fact]
    public void GetDefaultSettings_ReturnsSettingsWithDefaultValues()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetDefaultSettings();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("DemoUser", result.UsuarioActivo);
        Assert.Equal("en", result.Idioma);
        Assert.Equal("light", result.TemaVisual);
        Assert.True(result.ModoDemo);
        Assert.Equal("internal", result.TipoLector);
        Assert.True(result.SonidoCorrecto);
        Assert.True(result.VibracionCorrecta);
        Assert.True(result.SonidoError);
        Assert.Equal("demo", result.EntornoServidor);
        Assert.Equal("https://demo.server.com", result.UrlServidor);
        Assert.Equal(10, result.Timeout);
    }

    [Fact]
    public void SaveSettings_AndLoadSettings_ReturnsSavedSettings()
    {
        // Arrange
        var service = CreateService();
        var settings = new SettingsDto
        {
            UrlServidor = "https://custom.server.com",
            Timeout = 30,
            EntornoServidor = "Producción",
        };

        // Act
        service.SaveSettings(settings);
        var result = service.LoadSettings();

        // Assert
        Assert.Equal("https://custom.server.com", result.UrlServidor);
        Assert.Equal(30, result.Timeout);
        Assert.Equal("Producción", result.EntornoServidor);
    }

    [Fact]
    public void LoadSettings_WhenPreferencesExist_LoadsFromPreferences()
    {
        // Arrange
        var savedSettings = new SettingsDto
        {
            UrlServidor = "https://saved.server.com",
            Timeout = 20,
        };

        var json = JsonSerializer.Serialize(savedSettings);

        _preferencesServiceMock
            .Setup(x => x.ContainsKey(It.IsAny<string>()))
            .Returns(true);

        _preferencesServiceMock
            .Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(json);

        var service = CreateService();

        // Act
        var result = service.LoadSettings();

        // Assert
        Assert.Equal("https://saved.server.com", result.UrlServidor);
        Assert.Equal(20, result.Timeout);
    }

    [Fact]
    public void LoadSettings_WhenNoPreferences_ReturnsDefaults()
    {
        // Arrange
        _preferencesServiceMock
            .Setup(x => x.ContainsKey(It.IsAny<string>()))
            .Returns(false);

        var service = CreateService();

        // Act
        var result = service.LoadSettings();

        // Assert
        Assert.Equal("https://demo.server.com", result.UrlServidor);
        Assert.Equal(10, result.Timeout);
    }

    [Fact]
    public void LoadSettings_CalledTwice_ReturnsCachedSettings()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result1 = service.LoadSettings();
        var result2 = service.LoadSettings();

        // Assert
        Assert.Same(result1, result2);
        _logServiceMock.Verify(
            x => x.Debug(It.Is<string>(s => s.Contains("cache")), It.IsAny<object[]>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public void SaveSettings_LogsSave()
    {
        // Arrange
        var service = CreateService();
        var settings = new SettingsDto
        {
            UrlServidor = "https://demo.server.com",
            Timeout = 10,
            EntornoServidor = "DEMO",
        };

        // Act
        service.SaveSettings(settings);

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("Settings saved")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void SaveSettings_StoresJsonInPreferences()
    {
        // Arrange
        var service = CreateService();
        var settings = new SettingsDto
        {
            UrlServidor = "https://demo.server.com",
            Timeout = 10,
        };

        // Act
        service.SaveSettings(settings);

        // Assert
        _preferencesServiceMock.Verify(
            x => x.Set(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public void LoadSettings_WhenJsonIsCorrupt_LoadsDefaults()
    {
        // Arrange
        _preferencesServiceMock
            .Setup(x => x.ContainsKey(It.IsAny<string>()))
            .Returns(true);

        _preferencesServiceMock
            .Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("this is not valid json {{{");

        var service = CreateService();

        // Act
        var result = service.LoadSettings();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("https://demo.server.com", result.UrlServidor);
        _logServiceMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("Failed to deserialize")), It.IsAny<Exception>()),
            Times.Once);
    }

    [Fact]
    public void SaveSettings_WhenPreferencesThrows_LogsErrorAndRethrows()
    {
        // Arrange
        _preferencesServiceMock
            .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("Preferences error"));

        var service = CreateService();
        var settings = new SettingsDto
        {
            UrlServidor = "https://demo.server.com",
            Timeout = 10,
        };

        // Act & Assert
        Assert.Throws<Exception>(() => service.SaveSettings(settings));
        _logServiceMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("Failed to save")), It.IsAny<Exception>()),
            Times.Once);
    }
}
