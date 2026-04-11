using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
#if ANDROID
using LogiFlow.Mobile.Helpers;
#endif
using BarcodeScanning;
using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Camera;
using LogiFlow.Mobile.ViewModels.Login;
using LogiFlow.Mobile.ViewModels.Menu;
using LogiFlow.Mobile.ViewModels.Reception;
using LogiFlow.Mobile.ViewModels.Settings;
using LogiFlow.Mobile.ViewModels.Splash;
using LogiFlow.Mobile.Views.Camera;
using LogiFlow.Mobile.Views.Login;
using LogiFlow.Mobile.Views.Menu;
using LogiFlow.Mobile.Views.Reception;
using LogiFlow.Mobile.Views.Settings;
using LogiFlow.Mobile.Views.Splash;
using Microsoft.Maui.Handlers;

#if ANDROID
using Android.Content.Res;
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

    .UseBarcodeScanning()

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

                var editText = handler.PlatformView;

                void Apply()
                {
                    var isDark = themeService?.CurrentTheme == "dark";
                    EntryThemeUpdater.ApplyTheme(editText, isDark);
                }

                Apply();

                editText.FocusChange += (s, e) =>
                {
                    Apply();
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

            // ===== Camera =====
            builder.Services.AddTransient<CameraScanPage>();

            // ===== Reception =====
            builder.Services.AddTransient<ReceptionStartPage>();
            builder.Services.AddTransient<ReceptionHeaderPage>();
            builder.Services.AddTransient<ReceptionDetailPage>();
            builder.Services.AddTransient<ReceptionChecklistPage>();
            builder.Services.AddTransient<ReceptionConfirmationPage>();
            builder.Services.AddTransient<ReceptionItemsPage>();

            // ViewModels
            builder.Services.AddTransient<SplashViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MenuViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            // ===== Camera =====
            builder.Services.AddTransient<CameraScanViewModel>();

            // ===== Reception =====
            builder.Services.AddSingleton<ReceptionStartViewModel>();
            builder.Services.AddSingleton<ReceptionHeaderViewModel>();
            builder.Services.AddSingleton<ReceptionDetailViewModel>();
            builder.Services.AddSingleton<ReceptionChecklistViewModel>();
            builder.Services.AddSingleton<ReceptionConfirmationViewModel>();
            builder.Services.AddSingleton<ReceptionItemsViewModel>();

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

            // ===== Reception =====
            builder.Services.AddSingleton<IReceptionSessionService, ReceptionSessionService>();
            builder.Services.AddSingleton<ICameraScanService, CameraScanService>();
            builder.Services.AddSingleton<IReceptionService, ReceptionService>();
            builder.Services.AddSingleton<IMasterDataService, MasterDataService>();

            return builder.Build();
        }
    }
}
