using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Models;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides in-memory user session management for the application.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ILogService _logService;
    private UserSession? _currentSession;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionService"/> class.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    public SessionService(ILogService logService)
    {
        _logService = logService;
    }

    /// <inheritdoc/>
    public bool HasActiveSession() => _currentSession != null;

    /// <inheritdoc/>
    public void CreateSession(LoginResultDto loginResult)
    {
        _currentSession = new UserSession
        {
            UserId = loginResult.UserId,
            UserName = loginResult.UserName,
            LoginDate = DateTime.UtcNow,
            Environment = "DEMO",
        };

        _logService.Info("Session created. UserId={UserId}, UserName={UserName}, Environment={Environment}", loginResult.UserId, loginResult.UserName, "DEMO");
    }

    /// <inheritdoc/>
    public void ClearSession()
    {
        _logService.Info("Session cleared. UserId={UserId}", _currentSession?.UserId ?? "unknown");
        _currentSession = null;
    }
}
