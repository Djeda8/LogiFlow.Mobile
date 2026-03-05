namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides access to the application window and navigation page.
/// </summary>
public interface INavigationWindowService
{
    /// <summary>
    /// Gets the current navigation stack.
    /// </summary>
    IReadOnlyList<Page> NavigationStack { get; }

    /// <summary>
    /// Gets the current navigation page.
    /// </summary>
    /// <returns>The current <see cref="NavigationPage"/> or null if not found.</returns>
    NavigationPage? GetNavigationPage();

    /// <summary>
    /// Sets the root page of the application window.
    /// </summary>
    /// <param name="page">The page to set as root.</param>
    void SetRootPage(Page page);

    /// <summary>
    /// Pushes a page onto the navigation stack.
    /// </summary>
    /// <param name="page">The page to push.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task PushAsync(Page page);

    /// <summary>
    /// Pops the current page from the navigation stack.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task PopAsync();

    /// <summary>
    /// Pops all pages except the root from the navigation stack.
    /// </summary>
    /// <param name="animated">Whether to animate the transition.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task PopToRootAsync(bool animated = true);

    /// <summary>
    /// Inserts a page before another page in the navigation stack.
    /// </summary>
    /// <param name="page">The page to insert.</param>
    /// <param name="before">The page to insert before.</param>
    void InsertPageBefore(Page page, Page before);
}
