using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Base;

namespace LogiFlow.Mobile.ViewModels.Camera;

/// <summary>
/// Manages the camera scan screen.
/// Receives barcode results from ZXing and delivers them to the requesting ViewModel.
/// </summary>
public partial class CameraScanViewModel : BaseViewModel
{
    private readonly ICameraScanService _cameraScanService;
    private readonly INavigationService _navigationService;
    private readonly ILogService _logService;

    [ObservableProperty]
    private bool _isScanning = true;

    [ObservableProperty]
    private string _lastScannedCode = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraScanViewModel"/> class.
    /// </summary>
    /// <param name="cameraScanService">The camera scan coordination service.</param>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="logService">The logging service.</param>
    public CameraScanViewModel(
        ICameraScanService cameraScanService,
        INavigationService navigationService,
        ILogService logService)
    {
        _cameraScanService = cameraScanService;
        _navigationService = navigationService;
        _logService = logService;

        _logService.Info("CameraScanViewModel initialized.");
    }

    /// <summary>
    /// Called by the ZXing camera view when a barcode is detected.
    /// Delivers the result and navigates back automatically.
    /// </summary>
    /// <param name="code">The detected barcode value.</param>
    [RelayCommand]
    private async Task BarcodeDetectedAsync(string code)
    {
        if (!IsScanning || string.IsNullOrWhiteSpace(code))
        {
            return;
        }

        // Stop scanning immediately to avoid multiple triggers
        IsScanning = false;
        LastScannedCode = code;

        _logService.Info("Barcode detected. Code={Code}", code);

        await _navigationService.NavigateBackAsync();
        _cameraScanService.DeliverResult(code);
    }

    /// <summary>
    /// Cancels the scan and navigates back without delivering a result.
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        IsScanning = false;
        _cameraScanService.CancelScan();
        _logService.Info("Camera scan cancelled by user.");
        await _navigationService.NavigateBackAsync();
    }
}
