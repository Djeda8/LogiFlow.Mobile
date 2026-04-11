namespace LogiFlow.Mobile.DTOs;

/// <summary>
/// Represents a single detail line in a reception, containing logistic data.
/// </summary>
public class ReceptionDetailDto
{
    /// <summary>
    /// Gets or sets the article code.
    /// </summary>
    public string Article { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the article description.
    /// </summary>
    public string ArticleDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity received. Must be greater than 0.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the width in centimeters.
    /// </summary>
    public decimal? Width { get; set; }

    /// <summary>
    /// Gets or sets the height in centimeters.
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// Gets or sets the depth in centimeters.
    /// </summary>
    public decimal? Depth { get; set; }

    /// <summary>
    /// Gets or sets the weight in kilograms.
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Gets or sets the target warehouse location code.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the extra fields, only present in the TEST_SAMPLE flow.
    /// </summary>
    public ReceptionExtraFieldsDto? ExtraFields { get; set; }
}
