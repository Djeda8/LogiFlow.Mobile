using CommunityToolkit.Maui;
using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Login;
using LogiFlow.Mobile.ViewModels.Menu;
using LogiFlow.Mobile.ViewModels.Settings;
using LogiFlow.Mobile.ViewModels.Splash;
using LogiFlow.Mobile.Views.Login;
using LogiFlow.Mobile.Views.Menu;
using LogiFlow.Mobile.Views.Settings;
using LogiFlow.Mobile.Views.Splash;
using Microsoft.Extensions.Logging;
#if ANDROID
using Android.Content.Res;
using Microsoft.Maui.Handlers;
#endif

namespace LogiFlow.Mobile
{
    /// <summary>
    /// Entry point for the MAUI application. Configures fonts, services and dependency injection.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures the <see cref="MauiApp"/> instance.
        /// </summary>
        /// <returns>The configured <see cref="MauiApp"/>.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("PlaywriteCUGuides-Regular.ttf", "PlaywriteCUGuides");
                    fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                    fonts.AddFont("Roboto-Medium.ttf", "RobotoMedium");
                    fonts.AddFont("MaterialIconsOutlined-Regular.otf", "MaterialIconsOutlined");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if ANDROID
            EntryHandler.Mapper.AppendToMapping("UnderlineColor", (handler, view) =>
            {
                var themeService = IPlatformApplication.Current?.Services
                    .GetService<IThemeService>();

                var isDark = themeService?.CurrentTheme == "dark";
                var normalColor = isDark
                    ? Android.Graphics.Color.ParseColor("#374151")   // Divider dark
                    : Android.Graphics.Color.ParseColor("#E5E7EB");  // Divider light
                var focusColor = isDark
                    ? Android.Graphics.Color.ParseColor("#60A5FA")   // PrimaryColor dark
                    : Android.Graphics.Color.ParseColor("#2563EB");  // PrimaryColor light

                handler.PlatformView.BackgroundTintList =
                    ColorStateList.ValueOf(normalColor);

                handler.PlatformView.FocusChange += (s, e) =>
                {
                    var color = e.HasFocus ? focusColor : normalColor;
                    handler.PlatformView.BackgroundTintList =
                        ColorStateList.ValueOf(color);
                };
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Pages
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MenuPage>();
            builder.Services.AddTransient<SettingsPage>();

            // ViewModels
            builder.Services.AddTransient<SplashViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MenuViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            // Services
            builder.Services.AddSingleton<IFileSystemService, FileSystemService>();
            builder.Services.AddSingleton<ILogService, LogService>();
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            builder.Services.AddSingleton<ISessionService, SessionService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IErrorHandlerService, ErrorHandlerService>();
            builder.Services.AddSingleton<INavigationWindowService, NavigationWindowService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IThemeService, ThemeService>();
            builder.Services.AddSingleton<ISettingsDisplayService, SettingsDisplayService>();
            builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IConnectionService, ConnectionService>();
            builder.Services.AddSingleton<IScanService, ScanService>();

            return builder.Build();
        }
    }
}
