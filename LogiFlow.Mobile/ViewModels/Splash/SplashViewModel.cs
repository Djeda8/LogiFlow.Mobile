using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;

namespace LogiFlow.Mobile.ViewModels.Splash
{
    /// <summary>
    /// Manages the splash screen logic, including session validation and initial navigation.
    /// </summary>
    public partial class SplashViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ISessionService _sessionService;
        private readonly ILogService _logService;
        private readonly ISettingsService _settingsService;
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="sessionService">The session management service.</param>
        /// <param name="logService">The logging service.</param>
        /// <param name="settingsService">The settings persistence service.</param>
        /// <param name="localizationService">The localization service.</param>
        public SplashViewModel(INavigationService navigationService, ISessionService sessionService, ILogService logService, ISettingsService settingsService, ILocalizationService localizationService)
        {
            _navigationService = navigationService;
            _sessionService = sessionService;
            _logService = logService;
            _settingsService = settingsService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Starts the splash screen sequence, validates the current session
        /// and navigates to the appropriate screen.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartAsync()
        {
            try
            {
                _logService.Info("Application started. Checking session...");

                await Task.Delay(2500);

                // Cargar idioma guardado
                var settings = _settingsService.LoadSettings();
                _localizationService.SetLanguage(settings.Idioma);

                if (_sessionService.HasActiveSession())
                {
                    _logService.Info("Active session found. Navigating to Menu.");
                    await _navigationService.NavigateToAsync("MenuPage");
                }
                else
                {
                    _logService.Info("No active session. Navigating to Login.");
                    await _navigationService.NavigateToAsync("LoginPage");
                }
            }
            catch (Exception ex)
            {
                _logService.Error("Unexpected error during application startup", ex);
            }
        }
    }
}
