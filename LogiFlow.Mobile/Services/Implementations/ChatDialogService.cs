using CommunityToolkit.Maui.Views;
using LogiFlow.Mobile.Models.AI;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.AI;
using LogiFlow.Mobile.Views.AI;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides functionality to display a chat dialog within the application using dependency injection to resolve
/// required services.
/// </summary>
public class ChatDialogService : IChatDialogService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatDialogService"/> class
    /// using the specified service provider to resolve dependencies.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public ChatDialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public async Task ShowAsync(WmsScreenContext context)
    {
        // Resolve from DI — transient, new instance each time
        var chatViewModel = _serviceProvider.GetRequiredService<ChatViewModel>();
        chatViewModel.InitialiseContext(context);

        var sheet = new ChatBottomSheet(chatViewModel);

        // Get active page without coupling ViewModel to any view
        var page = GetCurrentPage();
        if (page is null)
        {
            return;
        }

        await page.ShowPopupAsync(sheet);
    }

    /// <summary>
    /// Gets the currently active page.
    /// </summary>
    /// <returns>The active <see cref="Page"/> or null if not found.</returns>
    private static Page? GetCurrentPage()
    {
        var shell = Shell.Current;
        if (shell is not null)
        {
            return shell.CurrentPage;
        }

        // Fallback for NavigationPage without Shell
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window?.Page is NavigationPage nav)
        {
            return nav.CurrentPage;
        }

        return window?.Page;
    }
}
