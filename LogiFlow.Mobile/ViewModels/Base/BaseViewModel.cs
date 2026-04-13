using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogiFlow.Mobile.Models.AI;
using LogiFlow.Mobile.Services.Interfaces;

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

        /// <summary>
        /// Gets or sets the AI chat dialog service for the ViewModel.
        /// </summary>
        protected IChatDialogService? ChatDialogService { get; set; }

        // ── AI Chat ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets the AI chat context for the current ViewModel.
        /// </summary>
        /// <returns>A <see cref="WmsScreenContext"/> representing the current screen context.</returns>
        protected virtual WmsScreenContext GetAiContext() => new()
        {
            ScreenId = GetType().Name,
            ScreenDisplayName = GetType().Name,
            Module = "LogiFlow",
        };

        partial void OnIsBusyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }

        /// <summary>
        /// Comando enlazado al botón 💬 Ask AI en el XAML.
        /// </summary>
        [RelayCommand]
        private async Task OpenAiChatAsync()
        {
            if (ChatDialogService is null)
            {
                return;
            }

            await ChatDialogService.ShowAsync(GetAiContext());
        }
    }
}
