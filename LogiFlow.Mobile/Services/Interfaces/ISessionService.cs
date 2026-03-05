using LogiFlow.Mobile.DTOs;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides user session management for the application.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Determines whether there is an active user session.
    /// </summary>
    /// <returns><c>true</c> if a session is active; otherwise, <c>false</c>.</returns>
    bool HasActiveSession();

    /// <summary>
    /// Creates a new user session from the provided login result.
    /// </summary>
    /// <param name="loginResult">The login result containing session data.</param>
    void CreateSession(LoginResultDto loginResult);

    /// <summary>
    /// Clears the current user session.
    /// </summary>
    void ClearSession();
}
