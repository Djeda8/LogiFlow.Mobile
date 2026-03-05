using LogiFlow.Mobile.Resources.Icons;
using Moq;

namespace LogiFlow.Mobile.Tests.Resources;

public class AppIconExtensionTests
{
    [Fact]
    public void Icon_DefaultValue_IsDefault()
    {
        // Assert
        Assert.Equal(default(AppIconGlyph), new AppIconExtension().Icon);
    }

    [Fact]
    public void Color_DefaultValue_IsBlack()
    {
        // Assert
        Assert.Equal(Colors.Black, new AppIconExtension().Color);
    }

    [Fact]
    public void Size_DefaultValue_Is24()
    {
        // Assert
        Assert.Equal(24, new AppIconExtension().Size);
    }

    [Fact]
    public void ProvideValue_ReturnsFontImageSource()
    {
        // Arrange
        var extension = new AppIconExtension
        {
            Icon = AppIconGlyph.Settings,
            Color = Colors.Red,
            Size = 32,
        };
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var result = extension.ProvideValue(serviceProviderMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FontImageSource>(result);
    }

    [Fact]
    public void ProvideValue_FontImageSource_HasCorrectGlyph()
    {
        // Arrange
        var extension = new AppIconExtension { Icon = AppIconGlyph.Settings };
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var result = (FontImageSource)extension.ProvideValue(serviceProviderMock.Object);

        // Assert
        Assert.Equal(AppIconGlyphMapper.GetGlyph(AppIconGlyph.Settings), result.Glyph);
    }

    [Fact]
    public void ProvideValue_FontImageSource_HasCorrectFontFamily()
    {
        // Arrange
        var extension = new AppIconExtension { Icon = AppIconGlyph.Settings };
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var result = (FontImageSource)extension.ProvideValue(serviceProviderMock.Object);

        // Assert
        Assert.Equal("MaterialIconsOutlined", result.FontFamily);
    }

    [Fact]
    public void ProvideValue_FontImageSource_HasCorrectColor()
    {
        // Arrange
        var extension = new AppIconExtension
        {
            Icon = AppIconGlyph.Settings,
            Color = Colors.Blue,
        };
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var result = (FontImageSource)extension.ProvideValue(serviceProviderMock.Object);

        // Assert
        Assert.Equal(Colors.Blue, result.Color);
    }

    [Fact]
    public void ProvideValue_FontImageSource_HasCorrectSize()
    {
        // Arrange
        var extension = new AppIconExtension
        {
            Icon = AppIconGlyph.Settings,
            Size = 48,
        };
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var result = (FontImageSource)extension.ProvideValue(serviceProviderMock.Object);

        // Assert
        Assert.Equal(48, result.Size);
    }
}
