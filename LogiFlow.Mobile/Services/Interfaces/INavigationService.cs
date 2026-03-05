namespace LogiFlow.Mobile.Services.Interfaces;

/// <summary>
/// Provides navigation operations between pages in the application.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the page identified by the specified key.
    /// </summary>
    /// <param name="pageKey">The registered key of the target page.</param>
    /// <param name="clearStack">
    /// If <c>true</c>, replaces the entire navigation stack.
    /// Recommended for Login, Logout and main flow transitions.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous navigation operation.</returns>
    Task NavigateToAsync(string pageKey, bool clearStack = false);

    /// <summary>
    /// Navigates back in the navigation stack.
    /// </summary>
    /// <param name="toRoot">If <c>true</c>, navigates back to the root page.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous navigation operation.</returns>
    Task NavigateBackAsync(bool toRoot = false);
}
