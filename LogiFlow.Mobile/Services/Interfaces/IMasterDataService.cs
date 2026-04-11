namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides access to master data: articles, locations, senders and recipients.
/// </summary>
public interface IMasterDataService
{
    /// <summary>
    /// Returns all available article codes asynchronously.
    /// </summary>
    /// <returns>A list of article codes.</returns>
    Task<List<string>> GetArticlesAsync();

    /// <summary>
    /// Returns all valid warehouse location codes asynchronously.
    /// </summary>
    /// <returns>A list of location codes.</returns>
    Task<List<string>> GetLocationsAsync();

    /// <summary>
    /// Returns all available sender names asynchronously.
    /// </summary>
    /// <returns>A list of sender names.</returns>
    Task<List<string>> GetSendersAsync();

    /// <summary>
    /// Returns all available recipient names asynchronously.
    /// </summary>
    /// <returns>A list of recipient names.</returns>
    Task<List<string>> GetRecipientsAsync();

    /// <summary>
    /// Checks asynchronously whether a location code exists in the master data.
    /// </summary>
    /// <param name="locationCode">The location code to validate.</param>
    /// <returns>True if the location is valid; otherwise, false.</returns>
    Task<bool> IsValidLocationAsync(string locationCode);

    /// <summary>
    /// Checks asynchronously whether an article code exists in the master data.
    /// </summary>
    /// <param name="articleCode">The article code to validate.</param>
    /// <returns>True if the article is valid; otherwise, false.</returns>
    Task<bool> IsValidArticleAsync(string articleCode);
}
