namespace LogiFlow.Mobile.DTOs
{
    /// <summary>
    /// Represents the result of a login operation.
    /// </summary>
    public class LoginResultDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the login was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the authenticated user identifier.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the authenticated username.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the error message when the login fails.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
