using CommunityToolkit.Mvvm.ComponentModel;

namespace LogiFlow.Mobile.ViewModels.Reception;

/// <summary>
/// Represents a single checklist item with its completion state.
/// </summary>
public partial class ChecklistItemViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isChecked;

    /// <summary>Gets the checklist item description.</summary>
    public string Description { get; init; } = string.Empty;
}
