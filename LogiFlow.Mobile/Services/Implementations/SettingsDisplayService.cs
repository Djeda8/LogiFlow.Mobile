using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides display name resolution for settings list values.
/// </summary>
public class SettingsDisplayService : ISettingsDisplayService
{
    private readonly ILocalizationService _localizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsDisplayService"/> class.
    /// </summary>
    /// <param name="localizationService">The localization service.</param>
    public SettingsDisplayService(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    /// <inheritdoc/>
    public string GetThemeDisplay(string code) => code switch
    {
        "light" => _localizationService.GetString(nameof(AppResources.ThemeLight)),
        "dark" => _localizationService.GetString(nameof(AppResources.ThemeDark)),
        _ => _localizationService.GetString(nameof(AppResources.ThemeLight)),
    };

    /// <inheritdoc/>
    public string GetScannerDisplay(string code) => code switch
    {
        "internal" => _localizationService.GetString(nameof(AppResources.ScannerInternal)),
        "external" => _localizationService.GetString(nameof(AppResources.ScannerExternal)),
        _ => _localizationService.GetString(nameof(AppResources.ScannerInternal)),
    };

    /// <inheritdoc/>
    public string GetEnvironmentDisplay(string code) => code switch
    {
        "demo" => _localizationService.GetString(nameof(AppResources.EnvironmentDemo)),
        "production" => _localizationService.GetString(nameof(AppResources.EnvironmentProduction)),
        _ => _localizationService.GetString(nameof(AppResources.EnvironmentDemo)),
    };

    /// <inheritdoc/>
    public string GetThemeCode(string display) => display switch
    {
        var d when d == _localizationService.GetString(nameof(AppResources.ThemeLight)) => "light",
        var d when d == _localizationService.GetString(nameof(AppResources.ThemeDark)) => "dark",
        _ => "light",
    };

    /// <inheritdoc/>
    public string GetScannerCode(string display) => display switch
    {
        var d when d == _localizationService.GetString(nameof(AppResources.ScannerInternal)) => "internal",
        var d when d == _localizationService.GetString(nameof(AppResources.ScannerExternal)) => "external",
        _ => "internal",
    };

    /// <inheritdoc/>
    public string GetEnvironmentCode(string display) => display switch
    {
        var d when d == _localizationService.GetString(nameof(AppResources.EnvironmentDemo)) => "demo",
        var d when d == _localizationService.GetString(nameof(AppResources.EnvironmentProduction)) => "production",
        _ => "demo",
    };
}
