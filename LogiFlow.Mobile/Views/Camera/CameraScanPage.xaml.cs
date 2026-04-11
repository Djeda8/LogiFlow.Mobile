using LogiFlow.Mobile.ViewModels.Camera;

#if ANDROID || IOS || MACCATALYST
using BarcodeScanning;
#endif

namespace LogiFlow.Mobile.Views.Camera;

/// <summary>
/// Camera scan page. Hosts the barcode reader and forwards results to the ViewModel.
/// </summary>
public partial class CameraScanPage : ContentPage
{
    private readonly CameraScanViewModel _viewModel;

#if ANDROID || IOS || MACCATALYST
    private CameraView? _cameraView;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraScanPage"/> class.
    /// </summary>
    /// <param name="viewModel">The camera scan view model.</param>
    public CameraScanPage(CameraScanViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <inheritdoc/>
    protected override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID || IOS || MACCATALYST
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await _viewModel.CancelCommand.ExecuteAsync(null);
                return;
            }

            if (_cameraView is null)
            {
                _cameraView = new CameraView
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    CameraEnabled = true,
                    BarcodeSymbologies = BarcodeFormats.All,
                };
                _cameraView.OnDetectionFinished += OnDetectionFinished;
                CameraContainer.Insert(0, _cameraView);
            }
            else
            {
                _cameraView.CameraEnabled = true;
            }
        });
#endif
        _viewModel.IsScanning = true;
    }

    /// <inheritdoc/>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

#if ANDROID || IOS || MACCATALYST
        if (_cameraView is not null)
        {
            _cameraView.CameraEnabled = false;
        }
#endif
        _viewModel.IsScanning = false;
    }

#if ANDROID || IOS || MACCATALYST
    private void OnDetectionFinished(object? sender, OnDetectionFinishedEventArg e)
    {
        if (e.BarcodeResults.Count == 0)
        {
            return;
        }

        var first = e.BarcodeResults.First();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _viewModel.BarcodeDetectedCommand.ExecuteAsync(first.RawValue);
        });
    }
#endif
}
