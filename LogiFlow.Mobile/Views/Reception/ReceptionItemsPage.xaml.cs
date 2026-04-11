using LogiFlow.Mobile.ViewModels.Reception;

namespace LogiFlow.Mobile.Views.Reception;

/// <summary>
/// Reception items page. Displays the logistic items generated after confirmation.
/// Final step of the reception flow. Read only.
/// </summary>
public partial class ReceptionItemsPage : ContentPage
{
    private readonly ReceptionItemsViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionItemsPage"/> class.
    /// </summary>
    /// <param name="viewModel">The reception items view model.</param>
    public ReceptionItemsPage(ReceptionItemsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <inheritdoc/>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
