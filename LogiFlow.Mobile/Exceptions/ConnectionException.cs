namespace LogiFlow.Mobile.Exceptions;

/// <summary>
/// Exception thrown when a server connection attempt fails.
/// </summary>
public class ConnectionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionException"/> class.
    /// </summary>
    /// <param name="url">The URL that failed to connect.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConnectionException(string url, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Url = url;
    }

    /// <summary>
    /// Gets the URL that failed to connect.
    /// </summary>
    public string Url { get; }
}
