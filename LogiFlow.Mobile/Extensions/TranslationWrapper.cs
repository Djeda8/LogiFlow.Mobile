using System.ComponentModel;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Extensions
{
    /// <summary>
    /// Observable wrapper that provides a reactive localized string value,
    /// updating automatically when the application language changes.
    /// </summary>
    public partial class TranslationWrapper : INotifyPropertyChanged
    {
        private readonly ILocalizationService _localizationService;
        private readonly string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationWrapper"/> class.
        /// </summary>
        /// <param name="localizationService">The localization service.</param>
        /// <param name="key">The resource key to translate.</param>
        public TranslationWrapper(ILocalizationService localizationService, string key)
        {
            _localizationService = localizationService;
            _key = key;

            _localizationService.LanguageChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            };
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the current localized string value for the associated resource key.
        /// </summary>
        public string Value => _localizationService.GetString(_key);
    }
}
