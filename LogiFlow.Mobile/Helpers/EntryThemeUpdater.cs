#if ANDROID
using Android.Content.Res;
using Android.Widget;

namespace LogiFlow.Mobile.Helpers;

/// <summary>
/// Updates the theme of Entry controls.
/// </summary>
public static class EntryThemeUpdater
{
    /// <summary>
    /// Applies the theme colors to the given EditText.
    /// </summary>
    /// <param name="editText">The native EditText control.</param>
    /// <param name="isDark">Indicates whether the dark theme is active.</param>
    public static void ApplyTheme(EditText editText, bool isDark)
    {
        var normalColor = isDark
            ? Android.Graphics.Color.ParseColor("#374151")
            : Android.Graphics.Color.ParseColor("#E5E7EB");

        var focusColor = isDark
            ? Android.Graphics.Color.ParseColor("#60A5FA")
            : Android.Graphics.Color.ParseColor("#2563EB");

        var color = editText.HasFocus ? focusColor : normalColor;

        editText.BackgroundTintList = ColorStateList.ValueOf(color);

        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
        {
            editText.TextCursorDrawable?.SetTint(color);
        }

        // highlight de selección de texto
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
        {
            // Android 10+
            editText.TextSelectHandle?.SetTint(focusColor);
        }

        editText.SetHighlightColor(focusColor);
    }
}
#endif
