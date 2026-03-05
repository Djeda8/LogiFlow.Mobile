using System.Globalization;
using System.Resources;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides localization and language management using .resx resource files.
/// </summary>
public class LocalizationService : ILocalizationService
{
    private readonly ResourceManager _resourceManager;
    private readonly ILogService _logService;
    private CultureInfo _currentCulture;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationService"/> class
    /// and sets the current UI culture as the default language.
    /// </summary>
    /// <param name="logService">The logging service.</param>
    public LocalizationService(ILogService logService)
    {
        _logService = logService;
        _resourceManager = AppResources.ResourceManager;
        _currentCulture = CultureInfo.CurrentUICulture;

        _logService.Info("LocalizationService initialized. Culture={Culture}", _currentCulture.Name);
    }

    /// <inheritdoc/>
    public event EventHandler? LanguageChanged;

    /// <inheritdoc/>
    public CultureInfo CurrentCulture => _currentCulture;

    /// <inheritdoc/>
    public string GetString(string key) => _resourceManager.GetString(key, _currentCulture) ?? key;

    /// <inheritdoc/>
    public void SetLanguage(string cultureCode)
    {
        _currentCulture = new CultureInfo(cultureCode);
        CultureInfo.CurrentUICulture = _currentCulture;
        CultureInfo.CurrentCulture = _currentCulture;
        Thread.CurrentThread.CurrentUICulture = _currentCulture;
        Thread.CurrentThread.CurrentCulture = _currentCulture;
        AppResources.Culture = _currentCulture;

        _logService.Info("Language changed to={CultureCode}", cultureCode);

        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
