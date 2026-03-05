using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides simulated user authentication for demo purposes.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ILogService _logService;
    private readonly ILocalizationService _localizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    /// <param name="localizationService">The localization service.</param>
    public AuthService(ILogService logService, ILocalizationService localizationService)
    {
        _logService = logService;
        _localizationService = localizationService;
    }

    /// <inheritdoc/>
    public async Task<LoginResultDto> LoginAsync(string username, string password)
    {
        _logService.Info("Authentication attempt. User={Username}", username);

        try
        {
            await Task.Delay(800);

            if (username == "admin" && password == "1234")
            {
                _logService.Info("Authentication successful. User={Username}", username);
                return new LoginResultDto
                {
                    IsSuccess = true,
                    UserId = "1",
                    UserName = "Admin",
                };
            }

            _logService.Warning("Authentication failed. Invalid credentials for User={Username}", username);
            return new LoginResultDto
            {
                IsSuccess = false,
                ErrorMessage = _localizationService.GetString(nameof(AppResources.ErrorInvalidCredentials)),
            };
        }
        catch (Exception ex)
        {
            _logService.Error($"Unexpected error during authentication. User={username}", ex);
            return new LoginResultDto
            {
                IsSuccess = false,
                ErrorMessage = _localizationService.GetString(nameof(AppResources.ErrorUnexpectedAuth)),
            };
        }
    }
}
