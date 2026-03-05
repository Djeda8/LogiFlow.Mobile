namespace LogiFlow.Mobile.Resources.Icons
{
    /// <summary>
    /// XAML markup extension that provides a <see cref="FontImageSource"/> for Material Symbols icons.
    /// </summary>
    [AcceptEmptyServiceProvider]
    [ContentProperty(nameof(Icon))]
    public class AppIconExtension : IMarkupExtension<ImageSource>
    {
        /// <summary>
        /// Gets or sets the icon glyph to display.
        /// </summary>
        public AppIconGlyph Icon { get; set; }

        /// <summary>
        /// Gets or sets the icon color. Defaults to black.
        /// </summary>
        public Color Color { get; set; } = Colors.Black;

        /// <summary>
        /// Gets or sets the icon size in pixels. Defaults to 24.
        /// </summary>
        public double Size { get; set; } = 24;

        /// <inheritdoc/>
        public ImageSource ProvideValue(IServiceProvider serviceProvider)
        {
            return new FontImageSource
            {
                Glyph = AppIconGlyphMapper.GetGlyph(Icon),
                FontFamily = "MaterialIconsOutlined",
                Color = Color,
                Size = Size,
            };
        }

        /// <inheritdoc/>
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
            => ProvideValue(serviceProvider);
    }
}
