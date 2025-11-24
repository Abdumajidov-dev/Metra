using System;
using System.Threading.Tasks;
using System.Windows;
using Metra.Desktop.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Metra.Desktop.Services.Implementation;

/// <summary>
/// Dialog xizmati - oynalarni boshqarish
/// </summary>
public class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DialogService> _logger;

    public DialogService(
        IServiceProvider serviceProvider,
        ILogger<DialogService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Dialog oynani ochish
    /// </summary>
    public Task<bool?> ShowDialogAsync<TWindow>() where TWindow : Window
    {
        return Task.Run(() =>
        {
            bool? result = null;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    _logger.LogInformation("Dialog ochilmoqda: {WindowType}", typeof(TWindow).Name);

                    var window = _serviceProvider.GetRequiredService<TWindow>();

                    // Owner ni belgilash (agar MainWindow ochiq bo'lsa)
                    if (System.Windows.Application.Current.MainWindow != null && System.Windows.Application.Current.MainWindow.IsLoaded)
                    {
                        window.Owner = System.Windows.Application.Current.MainWindow;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    }
                    else
                    {
                        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }

                    result = window.ShowDialog();

                    _logger.LogInformation("Dialog yopildi: {WindowType}, Natija: {Result}",
                        typeof(TWindow).Name, result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dialog ochishda xatolik: {WindowType}", typeof(TWindow).Name);
                    ShowError($"Oyna ochishda xatolik: {ex.Message}");
                }
            });

            return result;
        });
    }

    /// <summary>
    /// Xabar ko'rsatish
    /// </summary>
    public void ShowMessage(string title, string message, MessageBoxImage icon = MessageBoxImage.Information)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        });
    }

    /// <summary>
    /// Tasdiqlash so'rash
    /// </summary>
    public bool ShowConfirmation(string title, string message)
    {
        bool result = false;
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            var dialogResult = MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            result = dialogResult == MessageBoxResult.Yes;
        });

        return result;
    }

    /// <summary>
    /// Xatolik xabari
    /// </summary>
    public void ShowError(string message)
    {
        ShowMessage("Xatolik", message, MessageBoxImage.Error);
    }

    /// <summary>
    /// Muvaffaqiyat xabari
    /// </summary>
    public void ShowSuccess(string message)
    {
        ShowMessage("Muvaffaqiyat", message, MessageBoxImage.Information);
    }

    /// <summary>
    /// Ogohlantirish xabari
    /// </summary>
    public void ShowWarning(string message)
    {
        ShowMessage("Ogohlantirish", message, MessageBoxImage.Warning);
    }
}
