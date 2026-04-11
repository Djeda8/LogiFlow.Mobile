using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Demo implementation of <see cref="IReceptionService"/>.
/// Uses simulated data with local persistence. Ready for backend integration.
/// </summary>
public class ReceptionService : IReceptionService
{
    // Simulated valid reception numbers for demo mode
    private static readonly Dictionary<string, (string FlowType, string Sender, string Recipient, string DeliveryNote)> _demoReceptions = new()
    {
        { "REC-001", (ReceptionFlowType.Standard,    "Supplier A", "Warehouse 1", "DN-2024-001") },
        { "REC-002", (ReceptionFlowType.TestSample,  "Lab B",      "QA Zone",     "DN-2024-002") },
        { "REC-003", (ReceptionFlowType.Standard,    "Supplier C", "Warehouse 2", "DN-2024-003") },
    };

    private readonly ILogService _logService;
    private readonly Dictionary<string, ReceptionDto> _localStore = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionService"/> class.
    /// Uses the specified logging service for recording operational events and errors.
    /// </summary>
    /// <param name="logService">The logging service to be used. Cannot be null.</param>
    public ReceptionService(ILogService logService)
    {
        _logService = logService;
    }

    /// <inheritdoc/>
    public Task<ReceptionDto?> LoadReceptionAsync(string receptionNumber)
    {
        // Ignorar store — siempre cargar datos frescos
        if (!_demoReceptions.TryGetValue(receptionNumber.ToUpperInvariant(), out var demo))
        {
            _logService.Warning("Reception not found. ReceptionNumber={ReceptionNumber}", receptionNumber);
            return Task.FromResult<ReceptionDto?>(null);
        }

        var reception = new ReceptionDto
        {
            ReceptionNumber = receptionNumber.ToUpperInvariant(),
            FlowType = demo.FlowType,
            Status = ReceptionStatus.New,
            Header = new ReceptionHeaderDto
            {
                Sender = demo.Sender,
                Recipient = demo.Recipient,
                DeliveryNote = demo.DeliveryNote,
                ExpectedDate = DateTime.Today,
            },
        };

        _logService.Info("Reception loaded. ReceptionNumber={ReceptionNumber}, FlowType={FlowType}", receptionNumber, reception.FlowType);

        return Task.FromResult<ReceptionDto?>(reception);
    }

    /// <inheritdoc/>
    public Task SaveReceptionAsync(ReceptionDto reception)
    {
        _localStore[reception.ReceptionNumber] = reception;
        _logService.Info("Reception saved locally. ReceptionNumber={ReceptionNumber}, Status={Status}", reception.ReceptionNumber, reception.Status);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<List<ItemDto>> ConfirmReceptionAsync(ReceptionDto reception)
    {
        _logService.OperationStart("ConfirmReception", reception.ReceptionNumber);

        reception.Status = ReceptionStatus.Confirmed;
        _localStore[reception.ReceptionNumber] = reception;

        var items = reception.DetailLines.Select((detail, index) => new ItemDto
        {
            ItemCode = $"ITEM-{reception.ReceptionNumber}-{index + 1:D3}",
            Article = detail.Article,
            ArticleDescription = detail.ArticleDescription,
            Quantity = detail.Quantity,
            Location = detail.Location,
            Status = "GENERATED",
        }).ToList();

        _logService.OperationSuccess("ConfirmReception", reception.ReceptionNumber, $"ItemsGenerated={items.Count}");

        return Task.FromResult(items);
    }

    /// <inheritdoc/>
    public Task RejectReceptionAsync(ReceptionDto reception, string reason)
    {
        reception.Status = ReceptionStatus.Rejected;
        _localStore[reception.ReceptionNumber] = reception;

        _logService.Info("Reception rejected. ReceptionNumber={ReceptionNumber}, Reason={Reason}", reception.ReceptionNumber, reason);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool IsChecklistRequired(ReceptionDto reception)
    {
        if (reception.FlowType == ReceptionFlowType.TestSample)
        {
            return true;
        }

        return reception.DetailLines.Any(d =>
            d.Article.Equals("VEHICLE", StringComparison.OrdinalIgnoreCase));
    }
}
