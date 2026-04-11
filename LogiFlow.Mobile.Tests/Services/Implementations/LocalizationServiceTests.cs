using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations;

public class LocalizationServiceTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly LocalizationService _localizationService;

    public LocalizationServiceTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _localizationService = new LocalizationService(_logServiceMock.Object);
    }

    [Fact]
    public void Constructor_InitializesWithCurrentCulture()
    {
        // Assert
        Assert.NotNull(_localizationService.CurrentCulture);
    }

    [Fact]
    public void SetLanguage_ToEnglish_UpdatesCurrentCulture()
    {
        // Act
        _localizationService.SetLanguage("en");

        // Assert
        Assert.Equal("en", _localizationService.CurrentCulture.TwoLetterISOLanguageName);
    }

    [Fact]
    public void SetLanguage_ToSpanish_UpdatesCurrentCulture()
    {
        // Act
        _localizationService.SetLanguage("es");

        // Assert
        Assert.Equal("es", _localizationService.CurrentCulture.TwoLetterISOLanguageName);
    }

    [Fact]
    public void SetLanguage_FiresLanguageChangedEvent()
    {
        // Arrange
        var eventFired = false;
        _localizationService.LanguageChanged += (s, e) => eventFired = true;

        // Act
        _localizationService.SetLanguage("en");

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void SetLanguage_LogsLanguageChange()
    {
        // Act
        _localizationService.SetLanguage("es");

        // Assert
        _logServiceMock.Verify(
            x => x.Info(It.Is<string>(s => s.Contains("Language changed")), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void GetString_WithUnknownKey_ReturnsKey()
    {
        // Act
        var result = _localizationService.GetString("UnknownKey");

        // Assert
        Assert.Equal("UnknownKey", result);
    }

    [Fact]
    public void GetString_WithKnownKey_ReturnsTranslatedString()
    {
        // Act
        var result = _localizationService.GetString("SettingsTitle");

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("SettingsTitle", result);
    }
}
