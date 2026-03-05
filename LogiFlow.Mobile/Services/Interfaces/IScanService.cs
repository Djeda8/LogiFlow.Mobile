namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides scanner device testing operations.
/// </summary>
public interface IScanService
{
    /// <summary>
    /// Tests the specified scanner device type.
    /// </summary>
    /// <param name="tipoLector">The scanner type identifier to test.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation,
    /// containing <c>true</c> if the scanner test was successful; otherwise, <c>false</c>.</returns>
    Task<bool> TestScannerAsync(string tipoLector);
}
