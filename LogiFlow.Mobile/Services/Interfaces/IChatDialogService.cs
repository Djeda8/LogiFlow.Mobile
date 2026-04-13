using LogiFlow.Mobile.Models.AI;

namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Displays the AI chat from any ViewModel without coupling to the UI.
/// The ViewModel only calls <see cref="ShowAsync"/> and does not know about Popups or Pages.
/// </summary>
public interface IChatDialogService
{
    /// <summary>
    /// Shows the chat bottom sheet with the specified screen context.
    /// </summary>
    /// <param name="context">The current screen context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ShowAsync(WmsScreenContext context);
}
