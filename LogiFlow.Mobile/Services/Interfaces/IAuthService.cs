using LogiFlow.Mobile.DTOs;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides authentication operations for the application.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="username">The username to authenticate.</param>
    /// <param name="password">The password to authenticate.</param>
    /// <returns>A <see cref="LoginResultDto"/> containing the authentication result.</returns>
    Task<LoginResultDto> LoginAsync(string username, string password);
}
