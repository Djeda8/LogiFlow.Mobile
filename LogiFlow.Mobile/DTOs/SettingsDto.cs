namespace LogiFlow.Mobile.DTOs
{
    /// <summary>
    /// Represents the application configuration settings.
    /// </summary>
    public class SettingsDto
    {
        /// <summary>
        /// Gets or sets the active username.
        /// </summary>
        public string UsuarioActivo { get; set; } = "DemoUser";

        /// <summary>
        /// Gets or sets the application language code (e.g. "en", "es").
        /// </summary>
        public string Idioma { get; set; } = "en";

        /// <summary>
        /// Gets or sets the visual theme (e.g. "light", "dark").
        /// </summary>
        public string TemaVisual { get; set; } = "light";

        /// <summary>
        /// Gets or sets a value indicating whether demo mode is enabled.
        /// </summary>
        public bool ModoDemo { get; set; } = true;

        /// <summary>
        /// Gets or sets the scanner device type (e.g. "internal", "external").
        /// </summary>
        public string TipoLector { get; set; } = "internal";

        /// <summary>
        /// Gets or sets a value indicating whether the success sound is enabled.
        /// </summary>
        public bool SonidoCorrecto { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the success vibration is enabled.
        /// </summary>
        public bool VibracionCorrecta { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the error sound is enabled.
        /// </summary>
        public bool SonidoError { get; set; } = true;

        /// <summary>
        /// Gets or sets the server environment (e.g. "demo", "production").
        /// </summary>
        public string EntornoServidor { get; set; } = "demo";

        /// <summary>
        /// Gets or sets the server URL.
        /// </summary>
        public string UrlServidor { get; set; } = "https://demo.server.com";

        /// <summary>
        /// Gets or sets the connection timeout in seconds.
        /// </summary>
        public int Timeout { get; set; } = 10;
    }
}
