using LogiFlow.Mobile.ViewModels.Reception;

namespace LogiFlow.Mobile.Views.Reception;

/// <summary>
/// Reception header page. Displays and allows editing of general reception data.
/// </summary>
public partial class ReceptionHeaderPage : ContentPage
{
    private readonly ReceptionHeaderViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionHeaderPage"/> class.
    /// </summary>
    /// <param name="viewModel">The reception header view model.</param>
    public ReceptionHeaderPage(ReceptionHeaderViewModel viewModel)
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
