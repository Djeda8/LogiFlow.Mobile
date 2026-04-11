namespace LogiFlow.Mobile.Resources.Icons
{
    /// <summary>
    /// Provides Unicode glyph mappings for Material Symbols icons used in the application.
    /// </summary>
    public static class AppIconGlyphMapper
    {
        /// <summary>
        /// Returns the Unicode glyph string corresponding to the specified <see cref="AppIconGlyph"/>.
        /// </summary>
        /// <param name="icon">The icon enum value to map.</param>
        /// <returns>The Unicode glyph string for the icon, or "?" if not found.</returns>
        public static string GetGlyph(AppIconGlyph icon) => icon switch
        {
            AppIconGlyph.Person => "\ue7fd",
            AppIconGlyph.Lock => "\ue897",
            AppIconGlyph.Visibility => "\ue8f4",
            AppIconGlyph.VisibilityOff => "\ue8f5",
            AppIconGlyph.Login => "\uea77",
            AppIconGlyph.ErrorOutline => "\ue001",
            AppIconGlyph.Inventory2 => "\ue1a2",
            AppIconGlyph.SwapHoriz => "\ue8d4",
            AppIconGlyph.ShoppingCart => "\ue8cc",
            AppIconGlyph.Inventory => "\ue1a0",
            AppIconGlyph.Info => "\ue88e",
            AppIconGlyph.Settings => "\ue8b8",
            AppIconGlyph.ChevronRight => "\ue5cc",
            AppIconGlyph.Logout => "\ue9ba",
            AppIconGlyph.QrCodeScanner => "\uf206",
            AppIconGlyph.Dns => "\ue875",
            AppIconGlyph.Receipt => "\ue8b0",
            AppIconGlyph.LocalShipping => "\ue558",
            AppIconGlyph.LocationOn => "\ue0c8",
            AppIconGlyph.Checklist => "\ue6b1",
            AppIconGlyph.CheckCircle => "\ue86c",
            AppIconGlyph.Cancel => "\ue5c9",
            AppIconGlyph.ViewList => "\ue8ef",
            AppIconGlyph.Straighten => "\ue41c",
            AppIconGlyph.Tag => "\ue892",
            _ => "?",
        };
    }
}
