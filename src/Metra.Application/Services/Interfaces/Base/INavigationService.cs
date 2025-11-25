namespace Metra.Application.Services.Interfaces.Base;

/// <summary>
/// Navigatsiya servisi interfeysi
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// ViewModel ga o'tish
    /// </summary>
    void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class;

    /// <summary>
    /// Yangi tabda ochish
    /// </summary>
    void OpenInNewTab(string title, object content);

    /// <summary>
    /// Joriy tabni yopish
    /// </summary>
    void CloseCurrentTab();

    /// <summary>
    /// Orqaga qaytish
    /// </summary>
    bool CanGoBack();

    /// <summary>
    /// Orqaga qaytish
    /// </summary>
    void GoBack();
}
