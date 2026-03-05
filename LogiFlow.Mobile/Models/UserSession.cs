namespace LogiFlow.Mobile.Models
{
    /// <summary>
    /// Represents an active user session in the application.
    /// </summary>
    public class UserSession
    {
        /// <summary>
        /// Gets or sets the unique identifier of the authenticated user.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the username of the authenticated user.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the session was created.
        /// </summary>
        public DateTime LoginDate { get; set; }

        /// <summary>
        /// Gets or sets the environment in which the session was created (e.g. "DEMO", "Producción").
        /// </summary>
        public string? Environment { get; set; }
    }
}
