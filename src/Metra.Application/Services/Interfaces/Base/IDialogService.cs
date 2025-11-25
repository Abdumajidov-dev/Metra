namespace Metra.Application.Services.Interfaces.Base;

/// <summary>
/// Dialog servisi interfeysi
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Xabar ko'rsatish
    /// </summary>
    void ShowMessage(string message, string title = "Ma'lumot");

    /// <summary>
    /// Xatolik ko'rsatish
    /// </summary>
    void ShowError(string message, string title = "Xatolik");

    /// <summary>
    /// Muvaffaqiyat xabari ko'rsatish
    /// </summary>
    void ShowSuccess(string message, string title = "Muvaffaqiyat");

    /// <summary>
    /// Tasdiqlash dialogini ko'rsatish
    /// </summary>
    bool ShowConfirmation(string message, string title = "Tasdiqlash");

    /// <summary>
    /// Loading dialogini ko'rsatish
    /// </summary>
    void ShowLoading(string message = "Yuklanmoqda...");

    /// <summary>
    /// Loading dialogini yopish
    /// </summary>
    void HideLoading();
}
