using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Services.Implementations;

/// <summary>
/// Provides access to the MAUI application window and navigation page.
/// </summary>
public class NavigationWindowService : INavigationWindowService
{
    /// <inheritdoc/>
    public IReadOnlyList<Page> NavigationStack =>
        GetNavigationPage()?.Navigation.NavigationStack ?? new List<Page>();

    /// <inheritdoc/>
    public NavigationPage? GetNavigationPage() =>
        Application.Current?.Windows[0].Page as NavigationPage;

    /// <inheritdoc/>
    public void SetRootPage(Page page)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(page);
        }
    }

    /// <inheritdoc/>
    public async Task PushAsync(Page page)
    {
        if (GetNavigationPage() is NavigationPage navPage)
        {
            await navPage.PushAsync(page);
        }
    }

    /// <inheritdoc/>
    public async Task PopAsync()
    {
        if (GetNavigationPage() is NavigationPage navPage)
        {
            await navPage.PopAsync();
        }
    }

    /// <inheritdoc/>
    public async Task PopToRootAsync(bool animated = true)
    {
        if (GetNavigationPage() is NavigationPage navPage)
        {
            await navPage.PopToRootAsync(animated);
        }
    }

    /// <inheritdoc/>
    public void InsertPageBefore(Page page, Page before)
    {
        if (GetNavigationPage() is NavigationPage navPage)
        {
            navPage.Navigation.InsertPageBefore(page, before);
        }
    }
}
