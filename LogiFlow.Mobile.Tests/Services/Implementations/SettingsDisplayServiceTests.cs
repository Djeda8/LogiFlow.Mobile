using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations;

public class SettingsDisplayServiceTests
{
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly SettingsDisplayService _settingsDisplayService;

    public SettingsDisplayServiceTests()
    {
        _localizationServiceMock = new Mock<ILocalizationService>();
        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => $"Translated_{key}");

        _settingsDisplayService = new SettingsDisplayService(_localizationServiceMock.Object);
    }

    [Fact]
    public void GetThemeDisplay_WithLight_ReturnsTranslatedLight()
    {
        // Act
        var result = _settingsDisplayService.GetThemeDisplay("light");

        // Assert
        Assert.Equal("Translated_ThemeLight", result);
    }

    [Fact]
    public void GetThemeDisplay_WithDark_ReturnsTranslatedDark()
    {
        // Act
        var result = _settingsDisplayService.GetThemeDisplay("dark");

        // Assert
        Assert.Equal("Translated_ThemeDark", result);
    }

    [Fact]
    public void GetThemeDisplay_WithUnknownCode_ReturnsTranslatedLight()
    {
        // Act
        var result = _settingsDisplayService.GetThemeDisplay("unknown");

        // Assert
        Assert.Equal("Translated_ThemeLight", result);
    }

    [Fact]
    public void GetScannerDisplay_WithInternal_ReturnsTranslatedInternal()
    {
        // Act
        var result = _settingsDisplayService.GetScannerDisplay("internal");

        // Assert
        Assert.Equal("Translated_ScannerInternal", result);
    }

    [Fact]
    public void GetScannerDisplay_WithExternal_ReturnsTranslatedExternal()
    {
        // Act
        var result = _settingsDisplayService.GetScannerDisplay("external");

        // Assert
        Assert.Equal("Translated_ScannerExternal", result);
    }

    [Fact]
    public void GetScannerDisplay_WithUnknownCode_ReturnsTranslatedInternal()
    {
        // Act
        var result = _settingsDisplayService.GetScannerDisplay("unknown");

        // Assert
        Assert.Equal("Translated_ScannerInternal", result);
    }

    [Fact]
    public void GetEnvironmentDisplay_WithDemo_ReturnsTranslatedDemo()
    {
        // Act
        var result = _settingsDisplayService.GetEnvironmentDisplay("demo");

        // Assert
        Assert.Equal("Translated_EnvironmentDemo", result);
    }

    [Fact]
    public void GetEnvironmentDisplay_WithProduction_ReturnsTranslatedProduction()
    {
        // Act
        var result = _settingsDisplayService.GetEnvironmentDisplay("production");

        // Assert
        Assert.Equal("Translated_EnvironmentProduction", result);
    }

    [Fact]
    public void GetEnvironmentDisplay_WithUnknownCode_ReturnsTranslatedDemo()
    {
        // Act
        var result = _settingsDisplayService.GetEnvironmentDisplay("unknown");

        // Assert
        Assert.Equal("Translated_EnvironmentDemo", result);
    }

    [Fact]
    public void GetThemeCode_WithTranslatedLight_ReturnsLight()
    {
        // Act
        var result = _settingsDisplayService.GetThemeCode("Translated_ThemeLight");

        // Assert
        Assert.Equal("light", result);
    }

    [Fact]
    public void GetThemeCode_WithTranslatedDark_ReturnsDark()
    {
        // Act
        var result = _settingsDisplayService.GetThemeCode("Translated_ThemeDark");

        // Assert
        Assert.Equal("dark", result);
    }

    [Fact]
    public void GetThemeCode_WithUnknownDisplay_ReturnsLight()
    {
        // Act
        var result = _settingsDisplayService.GetThemeCode("unknown");

        // Assert
        Assert.Equal("light", result);
    }

    [Fact]
    public void GetScannerCode_WithTranslatedInternal_ReturnsInternal()
    {
        // Act
        var result = _settingsDisplayService.GetScannerCode("Translated_ScannerInternal");

        // Assert
        Assert.Equal("internal", result);
    }

    [Fact]
    public void GetScannerCode_WithTranslatedExternal_ReturnsExternal()
    {
        // Act
        var result = _settingsDisplayService.GetScannerCode("Translated_ScannerExternal");

        // Assert
        Assert.Equal("external", result);
    }

    [Fact]
    public void GetScannerCode_WithUnknownDisplay_ReturnsInternal()
    {
        // Act
        var result = _settingsDisplayService.GetScannerCode("unknown");

        // Assert
        Assert.Equal("internal", result);
    }

    [Fact]
    public void GetEnvironmentCode_WithTranslatedDemo_ReturnsDemo()
    {
        // Act
        var result = _settingsDisplayService.GetEnvironmentCode("Translated_EnvironmentDemo");

        // Assert
        Assert.Equal("demo", result);
    }

    [Fact]
    public void GetEnvironmentCode_WithTranslatedProduction_ReturnsProduction()
    {
        // Act
        var result = _settingsDisplayService.GetEnvironmentCode("Translated_EnvironmentProduction");

        // Assert
        Assert.Equal("production", result);
    }

    [Fact]
    public void GetEnvironmentCode_WithUnknownDisplay_ReturnsDemo()
    {
        // Act
        var result = _settingsDisplayService.GetEnvironmentCode("unknown");

        // Assert
        Assert.Equal("demo", result);
    }
}
