namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides connectivity testing operations against a backend server.
/// </summary>
public interface IConnectionService
{
    /// <summary>
    /// Tests the connection to the specified server URL.
    /// </summary>
    /// <param name="url">The server URL to test.</param>
    /// <param name="timeout">The timeout in seconds for the connection attempt.</param>
    /// <returns><c>true</c> if the connection was successful; otherwise, <c>false</c>.</returns>
    Task<bool> TestConnectionAsync(string url, int timeout);
}
