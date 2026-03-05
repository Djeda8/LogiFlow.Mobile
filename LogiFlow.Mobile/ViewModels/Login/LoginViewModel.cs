using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.Exceptions;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;
using LogiFlow.Mobile.Views.Menu;

namespace LogiFlow.Mobile.ViewModels.Login
{
    /// <summary>
    /// Manages the login screen logic and user authentication flow.
    /// </summary>
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;
        private readonly INavigationService _navigationService;
        private readonly ILogService _logService;
        private readonly IErrorHandlerService _errorHandlerService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _usernameHasError;

        [ObservableProperty]
        private bool _passwordHasError;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="sessionService">The session management service.</param>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="logService">The logging service.</param>
        /// <param name="errorHandlerService">The error handler service.</param>
        public LoginViewModel(
            IAuthService authService,
            ISessionService sessionService,
            INavigationService navigationService,
            ILogService logService,
            IErrorHandlerService errorHandlerService)
        {
            _authService = authService;
            _sessionService = sessionService;
            _navigationService = navigationService;
            _logService = logService;
            _errorHandlerService = errorHandlerService;
            PropertyChanged += OnBasePropertyChanged;
        }

        /// <summary>
        /// Gets a value indicating whether the login action can be executed.
        /// </summary>
        public bool CanLogin =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(Username) &&
            !string.IsNullOrWhiteSpace(Password);

        /// <summary>
        /// Executes the login command, authenticating the user with the provided credentials.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task LoginAsync()
        {
            if (!CanLogin)
            {
                return;
            }

            IsBusy = true;
            HasError = false;

            _logService.OperationStart("Login", Username);

            try
            {
                var result = await _authService.LoginAsync(Username, Password);

                if (!result.IsSuccess)
                {
                    throw new AuthException(Username, result.ErrorMessage ?? "Invalid credentials");
                }

                _sessionService.CreateSession(result);
                _logService.OperationSuccess("Login", Username, $"UserId={result.UserId}");

                await _navigationService.NavigateToAsync(nameof(MenuPage), clearStack: true);
            }
            catch (AuthException ex)
            {
                ErrorMessage = _errorHandlerService.Handle(ex);
                HasError = true;
                UsernameHasError = true;
                PasswordHasError = true;
                _logService.OperationFailure("Login", Username, ex.Message);
            }
            catch (Exception ex)
            {
                ErrorMessage = _errorHandlerService.Handle(ex, "LoginAsync");
                HasError = true;
            }
            finally
            {
                IsBusy = false;
                LoginCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        private void OnBasePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsBusy))
            {
                LoginCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        partial void OnUsernameChanged(string value)
        {
            UsernameHasError = false;
            LoginCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanLogin));
        }

        partial void OnPasswordChanged(string value)
        {
            PasswordHasError = false;
            LoginCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanLogin));
        }
    }
}
