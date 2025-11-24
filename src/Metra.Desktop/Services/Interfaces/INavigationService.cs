using System.Windows.Controls;

namespace Metra.Desktop.Services.Interfaces;

/// <summary>
/// Sahifalar orasida harakat qilish xizmati
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigation uchun ContentControl ni belgilash
    /// </summary>
    void SetNavigationTarget(ContentControl contentControl);

    /// <summary>
    /// Sahifaga o'tish
    /// </summary>
    void NavigateTo<TPage>() where TPage : UserControl;

    /// <summary>
    /// Orqaga qaytish
    /// </summary>
    void GoBack();

    /// <summary>
    /// Hozirgi sahifani olish
    /// </summary>
    UserControl? CurrentPage { get; }
}
