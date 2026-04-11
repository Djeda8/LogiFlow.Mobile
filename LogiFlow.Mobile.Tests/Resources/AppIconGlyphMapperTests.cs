using LogiFlow.Mobile.Resources.Icons;

namespace LogiFlow.Mobile.Tests.Resources;

public class AppIconGlyphMapperTests
{
    [Theory]
    [InlineData(AppIconGlyph.Person, "\ue7fd")]
    [InlineData(AppIconGlyph.Lock, "\ue897")]
    [InlineData(AppIconGlyph.Visibility, "\ue8f4")]
    [InlineData(AppIconGlyph.VisibilityOff, "\ue8f5")]
    [InlineData(AppIconGlyph.Login, "\uea77")]
    [InlineData(AppIconGlyph.ErrorOutline, "\ue001")]
    [InlineData(AppIconGlyph.Inventory2, "\ue1a2")]
    [InlineData(AppIconGlyph.SwapHoriz, "\ue8d4")]
    [InlineData(AppIconGlyph.ShoppingCart, "\ue8cc")]
    [InlineData(AppIconGlyph.Inventory, "\ue1a0")]
    [InlineData(AppIconGlyph.Info, "\ue88e")]
    [InlineData(AppIconGlyph.Settings, "\ue8b8")]
    [InlineData(AppIconGlyph.ChevronRight, "\ue5cc")]
    [InlineData(AppIconGlyph.Logout, "\ue9ba")]
    [InlineData(AppIconGlyph.QrCodeScanner, "\uf206")]
    [InlineData(AppIconGlyph.Dns, "\ue875")]
    [InlineData(AppIconGlyph.Receipt, "\ue8b0")]
    [InlineData(AppIconGlyph.LocalShipping, "\ue558")]
    [InlineData(AppIconGlyph.LocationOn, "\ue0c8")]
    [InlineData(AppIconGlyph.Checklist, "\ue6b1")]
    [InlineData(AppIconGlyph.CheckCircle, "\ue86c")]
    [InlineData(AppIconGlyph.Cancel, "\ue5c9")]
    [InlineData(AppIconGlyph.ViewList, "\ue8ef")]
    [InlineData(AppIconGlyph.Straighten, "\ue41c")]
    [InlineData(AppIconGlyph.Tag, "\ue892")]
    public void GetGlyph_WithKnownIcon_ReturnsCorrectGlyph(AppIconGlyph icon, string expectedGlyph)
    {
        // Act
        var result = AppIconGlyphMapper.GetGlyph(icon);

        // Assert
        Assert.Equal(expectedGlyph, result);
    }

    [Fact]
    public void GetGlyph_WithUnknownIcon_ReturnsQuestionMark()
    {
        // Act
        var result = AppIconGlyphMapper.GetGlyph((AppIconGlyph)999);

        // Assert
        Assert.Equal("?", result);
    }
}
