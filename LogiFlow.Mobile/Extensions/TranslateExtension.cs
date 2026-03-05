using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Extensions
{
    /// <summary>
    /// XAML markup extension that provides localized string bindings using <see cref="ILocalizationService"/>.
    /// </summary>
    [ContentProperty(nameof(Key))]
    [RequireService([typeof(IProvideValueTarget)])]
    public class TranslateExtension : IMarkupExtension<BindingBase>
    {
        /// <summary>
        /// Gets or sets the resource key to translate.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc/>
        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return new Binding();
            }

            var mauiContext = App.Current?.Handler?.MauiContext;
            var localizationService = mauiContext?.Services.GetService<ILocalizationService>();

            if (localizationService == null)
            {
                return new Binding { Source = $"[{Key}]" };
            }

            var wrapper = new TranslationWrapper(localizationService, Key);

            return new Binding
            {
                Source = wrapper,
                Path = nameof(TranslationWrapper.Value),
                Mode = BindingMode.OneWay,
            };
        }

        /// <inheritdoc/>
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
            => ProvideValue(serviceProvider);
    }
}
