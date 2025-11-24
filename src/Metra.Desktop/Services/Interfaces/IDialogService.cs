using System.Threading.Tasks;
using System.Windows;

namespace Metra.Desktop.Services.Interfaces;

/// <summary>
/// Dialog oynalarini boshqarish xizmati
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Dialog oynani ochish
    /// </summary>
    /// <typeparam name="TWindow">Window turi</typeparam>
    /// <returns>Dialog natijasi (true/false)</returns>
    Task<bool?> ShowDialogAsync<TWindow>() where TWindow : Window;

    /// <summary>
    /// Xabar ko'rsatish
    /// </summary>
    void ShowMessage(string title, string message, MessageBoxImage icon = MessageBoxImage.Information);

    /// <summary>
    /// Tasdiqlash so'rash
    /// </summary>
    bool ShowConfirmation(string title, string message);

    /// <summary>
    /// Xatolik xabari
    /// </summary>
    void ShowError(string message);

    /// <summary>
    /// Muvaffaqiyat xabari
    /// </summary>
    void ShowSuccess(string message);

    /// <summary>
    /// Ogohlantirish xabari
    /// </summary>
    void ShowWarning(string message);
}
