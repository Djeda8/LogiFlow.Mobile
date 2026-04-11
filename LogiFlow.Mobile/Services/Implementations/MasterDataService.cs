using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Demo implementation of <see cref="IMasterDataService"/>.
/// Returns simulated master data. Ready for backend integration.
/// </summary>
public class MasterDataService : IMasterDataService
{
    private static readonly List<string> _articles =
    [
        "ART-001", "ART-002", "ART-003", "VEHICLE", "SAMPLE-A", "SAMPLE-B"
    ];

    private static readonly List<string> _locations =
    [
        "A-01-01", "A-01-02", "A-02-01", "B-01-01", "B-02-01", "QA-ZONE-01"
    ];

    private static readonly List<string> _senders =
    [
        "Supplier A", "Supplier B", "Lab B", "Supplier C"
    ];

    private static readonly List<string> _recipients =
    [
        "Warehouse 1", "Warehouse 2", "QA Zone", "Production"
    ];

    /// <inheritdoc/>
    public Task<List<string>> GetArticlesAsync() =>
        Task.FromResult(new List<string>(_articles));

    /// <inheritdoc/>
    public Task<List<string>> GetLocationsAsync() =>
        Task.FromResult(new List<string>(_locations));

    /// <inheritdoc/>
    public Task<List<string>> GetSendersAsync() =>
        Task.FromResult(new List<string>(_senders));

    /// <inheritdoc/>
    public Task<List<string>> GetRecipientsAsync() =>
        Task.FromResult(new List<string>(_recipients));

    /// <inheritdoc/>
    public Task<bool> IsValidLocationAsync(string locationCode) =>
        Task.FromResult(_locations.Contains(locationCode, StringComparer.OrdinalIgnoreCase));

    /// <inheritdoc/>
    public Task<bool> IsValidArticleAsync(string articleCode) =>
        Task.FromResult(_articles.Contains(articleCode, StringComparer.OrdinalIgnoreCase));
}
