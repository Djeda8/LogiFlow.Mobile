namespace LogiFlow.Mobile.Exceptions;

/// <summary>
/// Exception thrown when an authentication attempt fails.
/// </summary>
public class AuthException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthException"/> class.
    /// </summary>
    /// <param name="username">The username that failed authentication.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AuthException(string username, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Username = username;
    }

    /// <summary>
    /// Gets the username that failed authentication.
    /// </summary>
    public string Username { get; }
}
