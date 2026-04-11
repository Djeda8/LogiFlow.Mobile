namespace LogiFlow.Mobile.DTOs;

/// <summary>
/// General header data for a reception.
/// </summary>
public class ReceptionHeaderDto
{
    /// <summary>
    /// Gets or sets the sender of the delivery.
    /// </summary>
    public string Sender { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient of the delivery.
    /// </summary>
    public string Recipient { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the delivery note associated with the shipment.
    /// </summary>
    public string DeliveryNote { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets any observations or notes related to the delivery.
    /// </summary>
    public string Observations { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expected delivery date.
    /// </summary>
    public DateTime ExpectedDate { get; set; } = DateTime.Today;
}
