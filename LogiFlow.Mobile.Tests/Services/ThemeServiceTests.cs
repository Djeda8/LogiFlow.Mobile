using LogiFlow.Mobile.Services.Implementations;

namespace LogiFlow.Mobile.Tests.Services;

public class ThemeServiceTests
{
    private readonly ThemeService _themeService;

    public ThemeServiceTests()
    {
        _themeService = new ThemeService();
    }

    [Fact]
    public void ApplyTheme_WithNullApplication_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _themeService.ApplyTheme("light"));
        Assert.Null(exception);
    }

    [Fact]
    public void ApplyTheme_WithDarkCode_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _themeService.ApplyTheme("dark"));
        Assert.Null(exception);
    }

    [Fact]
    public void ApplyTheme_WithUnknownCode_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _themeService.ApplyTheme("unknown"));
        Assert.Null(exception);
    }

    [Fact]
    public void ApplyTheme_WithEmptyCode_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _themeService.ApplyTheme(string.Empty));
        Assert.Null(exception);
    }

    [Fact]
    public void ApplyTheme_Dark_UpdatesCurrentTheme()
    {
        // Act
        _themeService.ApplyTheme("dark");

        // Assert
        Assert.Equal("dark", _themeService.CurrentTheme);
    }

    [Fact]
    public void ApplyTheme_Light_UpdatesCurrentTheme()
    {
        // Act
        _themeService.ApplyTheme("light");

        // Assert
        Assert.Equal("light", _themeService.CurrentTheme);
    }
}
