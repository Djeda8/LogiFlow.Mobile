using CommunityToolkit.Mvvm.ComponentModel;

namespace LogiFlow.Mobile.ViewModels.Base
{
    /// <summary>
    /// Base view model providing common properties for busy state and error handling.
    /// All view models in the application should inherit from this class.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        /// <summary>
        /// Gets a value indicating whether the view model is not currently busy.
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        partial void OnIsBusyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }
}
