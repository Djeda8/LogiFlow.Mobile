using System.Globalization;
using LogiFlow.Mobile.Resources.Icons;

namespace LogiFlow.Mobile.Converters;

/// <summary>
/// Converts an <see cref="AppIconGlyph"/> enum value to its corresponding Unicode glyph string.
/// </summary>
public class AppIconGlyphToStringConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AppIconGlyph iconGlyph)
        {
            return AppIconGlyphMapper.GetGlyph(iconGlyph);
        }

        return "?";
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
