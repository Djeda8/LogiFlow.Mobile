using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using LogiFlow.Mobile.ViewModels.AI;

namespace LogiFlow.Mobile.Views.AI;

/// <summary>
/// AI chat popup anclado en la parte inferior de la pantalla.
/// Compatible con CommunityToolkit.Maui v9+.
/// </summary>
public partial class ChatBottomSheet : Popup
{
    private readonly ChatViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatBottomSheet"/> class
    /// with the specified <see cref="ChatViewModel"/> and sets up the popup size and bindings.
    /// </summary>
    /// <param name="viewModel">The view model that provides chat messages and logic.</param>
    public ChatBottomSheet(ChatViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        // Explicit size based on the device's screen
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        Size = new Size(screenWidth, screenHeight * 0.75);

        _viewModel.Messages.CollectionChanged += OnMessagesChanged;
        Closed += OnPopupClosed;
    }

    private void OnPopupClosed(object? sender, PopupClosedEventArgs e)
    {
        _viewModel.Messages.CollectionChanged -= OnMessagesChanged;
        _viewModel.CancelPendingRequest();
        Closed -= OnPopupClosed;
    }

    private void OnMessagesChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (_viewModel.Messages.Count == 0)
        {
            return;
        }

        var lastItem = _viewModel.Messages[^1];

        // Delay para que el CollectionView termine de renderizar el item
        // antes de intentar hacer scroll — evita "Invalid target position"
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await Task.Delay(100);
                MessagesCollection.ScrollTo(lastItem, animate: false);
            }
            catch
            {
                // Ignorar si el popup ya se cerró antes del scroll
            }
        });
    }
}
