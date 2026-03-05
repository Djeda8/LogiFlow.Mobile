using LogiFlow.Mobile.Converters;
using LogiFlow.Mobile.Resources.Icons;
using System.Globalization;

namespace LogiFlow.Mobile.Tests.Converters;

public class AppIconGlyphToStringConverterTests
{
    private readonly AppIconGlyphToStringConverter _converter;

    public AppIconGlyphToStringConverterTests()
    {
        _converter = new AppIconGlyphToStringConverter();
    }

    [Fact]
    public void Convert_WithValidIconGlyph_ReturnsGlyphString()
    {
        // Act
        var result = _converter.Convert(AppIconGlyph.Settings, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
        Assert.NotEmpty((string)result);
    }

    [Fact]
    public void Convert_WithNullValue_ReturnsQuestionMark()
    {
        // Act
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("?", result);
    }

    [Fact]
    public void Convert_WithNonIconGlyphValue_ReturnsQuestionMark()
    {
        // Act
        var result = _converter.Convert("not an icon", typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("?", result);
    }

    [Fact]
    public void Convert_WithInventoryIcon_ReturnsGlyphString()
    {
        // Act
        var result = _converter.Convert(AppIconGlyph.Inventory, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("?", result);
    }

    [Fact]
    public void Convert_WithInfoIcon_ReturnsGlyphString()
    {
        // Act
        var result = _converter.Convert(AppIconGlyph.Info, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("?", result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack("test", typeof(AppIconGlyph), null, CultureInfo.InvariantCulture));
    }
}
