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

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="sessionService">The session management service.</param>
        /// <param name="logService">The logging service.</param>
        public SplashViewModel(INavigationService navigationService, ISessionService sessionService, ILogService logService)
        {
            _navigationService = navigationService;
            _sessionService = sessionService;
            _logService = logService;
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
