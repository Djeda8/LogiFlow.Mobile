namespace LogiFlow.Mobile.DTOs;

/// <summary>
/// Represents a reception document with its header and detail lines.
/// </summary>
public class ReceptionDto
{
    /// <summary>
    /// Gets or sets the reception identifier scanned from the transport label.
    /// </summary>
    public string ReceptionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the flow type. Possible values: STANDARD or TEST_SAMPLE.
    /// </summary>
    public string FlowType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current reception status.
    /// </summary>
    public string Status { get; set; } = ReceptionStatus.New;

    /// <summary>
    /// Gets or sets the general header data of the reception.
    /// </summary>
    public ReceptionHeaderDto Header { get; set; } = new();

    /// <summary>
    /// Gets or sets the detail lines containing logistic data.
    /// </summary>
    public List<ReceptionDetailDto> DetailLines { get; set; } = new();
}
