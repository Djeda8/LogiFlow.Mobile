using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Exceptions;
using LogiFlow.Mobile.Models;
using LogiFlow.Mobile.Resources.Languages;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;

namespace LogiFlow.Mobile.ViewModels.Settings;

/// <summary>
/// Manages the settings screen logic, including language, theme, scanner and server configuration.
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;
    private readonly IScanService _scanService;
    private readonly IConnectionService _connectionService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogService _logService;
    private readonly IErrorHandlerService _errorHandlerService;
    private readonly ISettingsDisplayService _settingsDisplayService;
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private SettingsDto _settings = new();

    [ObservableProperty]
    private List<string> _availableLanguages = ["English", "Español"];

    [ObservableProperty]
    private bool _urlServidorHasError;

    [ObservableProperty]
    private bool _timeoutHasError;

    [ObservableProperty]
    private string _selectedLanguage = string.Empty;

    [ObservableProperty]
    private SettingsOption? _selectedTheme;

    [ObservableProperty]
    private SettingsOption? _selectedScannerType;

    [ObservableProperty]
    private SettingsOption? _selectedEnvironment;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="settingsService">The settings persistence service.</param>
    /// <param name="scanService">The scanner device service.</param>
    /// <param name="connectionService">The server connection service.</param>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="localizationService">The localization service.</param>
    /// <param name="logService">The logging service.</param>
    /// <param name="errorHandlerService">The error handler service.</param>
    /// <param name="settingsDisplayService">The settings display service.</param>
    /// <param name="themeService">The theme service.</param>
    public SettingsViewModel(
        ISettingsService settingsService,
        IScanService scanService,
        IConnectionService connectionService,
        INavigationService navigationService,
        ILocalizationService localizationService,
        ILogService logService,
        IErrorHandlerService errorHandlerService,
        ISettingsDisplayService settingsDisplayService,
        IThemeService themeService)
    {
        _settingsService = settingsService;
        _scanService = scanService;
        _connectionService = connectionService;
        _navigationService = navigationService;
        _localizationService = localizationService;
        _logService = logService;
        _errorHandlerService = errorHandlerService;
        _settingsDisplayService = settingsDisplayService;

        _localizationService.LanguageChanged += OnLanguageChanged;

        _logService.Info("SettingsViewModel initialized. Current URL={Url}, Timeout={Timeout}", Settings.UrlServidor, Settings.Timeout);
        _themeService = themeService;
    }

    /// <summary>
    /// Gets the current application version string.
    /// </summary>
    public string AppVersion => AppInfo.VersionString;

    /// <summary>
    /// Gets the available visual themes.
    /// </summary>
    public ObservableCollection<SettingsOption> AvailableThemes { get; } = [];

    /// <summary>
    /// Gets the available scanner device types.
    /// </summary>
    public ObservableCollection<SettingsOption> ScannerTypes { get; } = [];

    /// <summary>
    /// Gets the available server environments.
    /// </summary>
    public ObservableCollection<SettingsOption> ServerEnvironments { get; } = [];

    /// <summary>
    /// Loads the settings from the settings service.
    /// </summary>
    public void LoadSettings()
    {
        Settings = _settingsService.LoadSettings();
        LoadStaticLists();
        InitializeSelectedLanguage();
    }

    private static bool IsValidUrl(string url) =>
        !string.IsNullOrWhiteSpace(url) &&
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// Saves the current settings and navigates back.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task SaveAsync()
    {
        if (!ValidateAll())
        {
            _logService.Warning("Settings save aborted due to validation errors. URL={Url}, Timeout={Timeout}", Settings.UrlServidor, Settings.Timeout);
            return;
        }

        IsBusy = true;
        HasError = false;

        _logService.OperationStart("SaveSettings", Settings.UsuarioActivo, $"URL={Settings.UrlServidor}, Timeout={Settings.Timeout}, Environment={Settings.EntornoServidor}");

        try
        {
            _settingsService.SaveSettings(Settings);
            _logService.OperationSuccess("SaveSettings", Settings.UsuarioActivo);
            await _navigationService.NavigateBackAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "SaveSettings");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Restores all settings to their default values.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private void RestoreDefaults()
    {
        _logService.Info("Settings restored to defaults by user={User}", Settings.UsuarioActivo);
        Settings = _settingsService.GetDefaultSettings();
        InitializeSelectedLanguage();
        ClearAllErrors();
    }

    /// <summary>
    /// Tests the configured scanner device.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task TestScannerAsync()
    {
        IsBusy = true;
        HasError = false;

        _logService.OperationStart("TestScanner", Settings.UsuarioActivo, $"ScannerType={Settings.TipoLector}");

        try
        {
            var result = await _scanService.TestScannerAsync(Settings.TipoLector);
            if (!result)
            {
                ErrorMessage = _localizationService.GetString(nameof(AppResources.ErrorScannerTestFailed));
                HasError = true;
                _logService.OperationFailure("TestScanner", Settings.UsuarioActivo, $"Scanner test failed for type={Settings.TipoLector}");
            }
            else
            {
                _logService.OperationSuccess("TestScanner", Settings.UsuarioActivo, $"ScannerType={Settings.TipoLector}");
            }
        }
        catch (ValidationException ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex);
            HasError = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "TestScanner");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Tests the connection to the configured server URL.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task TestConnectionAsync()
    {
        UrlServidorHasError = !IsValidUrl(Settings.UrlServidor);
        if (UrlServidorHasError)
        {
            ErrorMessage = _errorHandlerService.Handle(
                new ValidationException(
                    "UrlServidor",
                    _localizationService.GetString(nameof(AppResources.ErrorInvalidUrl))));
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;

        _logService.OperationStart("TestConnection", Settings.UsuarioActivo, $"URL={Settings.UrlServidor}, Timeout={Settings.Timeout}");

        try
        {
            var result = await _connectionService.TestConnectionAsync(Settings.UrlServidor, Settings.Timeout);

            if (!result)
            {
                ErrorMessage = _localizationService.GetString(nameof(AppResources.Msg_TestErrorConnection));
                HasError = true;
                _logService.OperationFailure("TestConnection", Settings.UsuarioActivo, $"Connection failed. URL={Settings.UrlServidor}");
            }
            else
            {
                _logService.OperationSuccess("TestConnection", Settings.UsuarioActivo, $"URL={Settings.UrlServidor}");
            }
        }
        catch (ValidationException ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex);
            HasError = true;
        }
        catch (ConnectionException ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex);
            HasError = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = _errorHandlerService.Handle(ex, "TestConnection");
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        LoadStaticLists();
        OnPropertyChanged(string.Empty);
        _logService.Info("UI refreshed after language change. Culture={Culture}", _localizationService.CurrentCulture.Name);
    }

    private void InitializeSelectedLanguage()
    {
        SelectedLanguage = Settings.Idioma switch
        {
            "en" => "English",
            "es" => "Español",
            _ => "English",
        };
    }

    partial void OnSelectedThemeChanged(SettingsOption? value)
    {
        if (value is not null)
        {
            Settings.TemaVisual = value.Code;
            _themeService.ApplyTheme(value.Code);
        }
    }

    partial void OnSelectedScannerTypeChanged(SettingsOption? value)
    {
        if (value is not null)
        {
            Settings.TipoLector = value.Code;
        }
    }

    partial void OnSelectedEnvironmentChanged(SettingsOption? value)
    {
        if (value is not null)
        {
            Settings.EntornoServidor = value.Code;
        }
    }

    private void LoadStaticLists()
    {
        var currentTheme = SelectedTheme?.Code ?? Settings?.TemaVisual ?? "light";
        var currentScanner = SelectedScannerType?.Code ?? Settings?.TipoLector ?? "internal";
        var currentEnvironment = SelectedEnvironment?.Code ?? Settings?.EntornoServidor ?? "demo";

        AvailableThemes.Clear();
        AvailableThemes.Add(new SettingsOption { Code = "light", DisplayName = _settingsDisplayService.GetThemeDisplay("light") });
        AvailableThemes.Add(new SettingsOption { Code = "dark", DisplayName = _settingsDisplayService.GetThemeDisplay("dark") });

        ScannerTypes.Clear();
        ScannerTypes.Add(new SettingsOption { Code = "internal", DisplayName = _settingsDisplayService.GetScannerDisplay("internal") });
        ScannerTypes.Add(new SettingsOption { Code = "external", DisplayName = _settingsDisplayService.GetScannerDisplay("external") });

        ServerEnvironments.Clear();
        ServerEnvironments.Add(new SettingsOption { Code = "demo", DisplayName = _settingsDisplayService.GetEnvironmentDisplay("demo") });
        ServerEnvironments.Add(new SettingsOption { Code = "production", DisplayName = _settingsDisplayService.GetEnvironmentDisplay("production") });

        SelectedTheme = AvailableThemes.FirstOrDefault(x => x.Code == currentTheme);
        SelectedScannerType = ScannerTypes.FirstOrDefault(x => x.Code == currentScanner);
        SelectedEnvironment = ServerEnvironments.FirstOrDefault(x => x.Code == currentEnvironment);
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        var cultureCode = value switch
        {
            "English" => "en",
            "Español" => "es",
            _ => "en",
        };

        _localizationService.SetLanguage(cultureCode);
        Settings.Idioma = cultureCode;

        _logService.Info("Language changed to={CultureCode} by user={User}", cultureCode, Settings.UsuarioActivo);
    }

    partial void OnSettingsChanged(SettingsDto value)
    {
        ClearAllErrors();
    }

    private bool ValidateAll()
    {
        UrlServidorHasError = !IsValidUrl(Settings.UrlServidor);
        TimeoutHasError = Settings.Timeout <= 0;

        if (UrlServidorHasError)
        {
            ErrorMessage = _errorHandlerService.Handle(
                new ValidationException("UrlServidor", _localizationService.GetString(nameof(AppResources.ErrorInvalidUrl))));
            HasError = true;
        }
        else if (TimeoutHasError)
        {
            ErrorMessage = _errorHandlerService.Handle(
                new ValidationException("Timeout", _localizationService.GetString(nameof(AppResources.ErrorTimeoutInvalid))));
            HasError = true;
        }

        return !UrlServidorHasError && !TimeoutHasError;
    }

    private void ClearAllErrors()
    {
        UrlServidorHasError = false;
        TimeoutHasError = false;
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
