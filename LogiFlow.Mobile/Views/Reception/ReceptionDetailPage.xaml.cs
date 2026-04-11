using LogiFlow.Mobile.ViewModels.Reception;

namespace LogiFlow.Mobile.Views.Reception;

/// <summary>
/// Reception detail page. Defines logistic data for each detail line.
/// </summary>
public partial class ReceptionDetailPage : ContentPage
{
    private readonly ReceptionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionDetailPage"/> class.
    /// </summary>
    /// <param name="viewModel">The reception detail view model.</param>
    public ReceptionDetailPage(ReceptionDetailViewModel viewModel)
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
