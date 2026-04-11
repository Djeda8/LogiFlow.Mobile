namespace LogiFlow.Mobile.DTOs
{
    /// <summary>
    /// Additional fields required for the TEST_SAMPLE flow.
    /// </summary>
    public class ReceptionExtraFieldsDto
    {
        /// <summary>
        /// Gets or sets the sample reference code.
        /// </summary>
        public string SampleReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the lot number associated with the reception.
        /// </summary>
        public string LotNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets any observations or notes related to the reception.
        /// </summary>
        public string Observations { get; set; } = string.Empty;
    }
}
