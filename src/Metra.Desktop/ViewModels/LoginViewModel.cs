using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Metra.Application.DTOs.Requests;
using Metra.Application.Exceptions;
using Metra.Application.Services.Interfaces.Base;
using Metra.Desktop.ViewModels.Base;
using Metra.Desktop.Views;
using Microsoft.Extensions.Logging;
using Notification.Wpf;
using System.Text.RegularExpressions;
using System.Windows;
using WpfApp = System.Windows.Application;

namespace Metra.Desktop.ViewModels;

/// <summary>
/// Login ViewModel
/// </summary>
public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<LoginViewModel> _logger;
    private readonly NotificationManager _notifier;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isLoggingIn = false;

    public LoginViewModel(
        IAuthService authService,
        ILogger<LoginViewModel> logger,
        NotificationManager notifier)
    {
        _authService = authService;
        _logger = logger;
        _notifier = notifier;

        Title = "Login - Metra v3.0";
    }

    /// <summary>
    /// Login Command
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        if (IsLoggingIn) return;

        try
        {
            IsLoggingIn = true;
            IsBusy = true;

            _logger.LogInformation("Login boshlandi: {Phone}", PhoneNumber);

            // Telefon raqamni tozalash
            var cleanPhone = CleanPhoneNumber(PhoneNumber);

            if (string.IsNullOrWhiteSpace(cleanPhone) || cleanPhone.Length < 9)
            {
                _notifier.Show(
                    "Xatolik",
                    "Iltimos telefon raqamni to'g'ri kiriting",
                    NotificationType.Error,
                    expirationTime: TimeSpan.FromSeconds(3));
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                _notifier.Show(
                    "Xatolik",
                    "Iltimos parolni kiriting",
                    NotificationType.Error,
                    expirationTime: TimeSpan.FromSeconds(3));
                return;
            }

            // Login request
            var request = new LoginRequest
            {
                Phone = cleanPhone,
                Password = Password
            };

            var response = await _authService.LoginAsync(request);

            if (response?.Success == true && !string.IsNullOrEmpty(response.Token))
            {
                _logger.LogInformation("Login muvaffaqiyatli");

                _notifier.Show(
                    "Muvaffaqiyatli",
                    "Tizimga muvaffaqiyatli kirdingiz!",
                    NotificationType.Success,
                    expirationTime: TimeSpan.FromSeconds(2));

                // MainWindow ochish
                await WpfApp.Current.Dispatcher.InvokeAsync(() =>
                {
                    var mainWindow = App.ServiceProvider.GetService(typeof(MainWindow)) as MainWindow;
                    if (mainWindow != null)
                    {
                        App.GlobalMainWindow = mainWindow;
                        mainWindow.Show();

                        // Login oynasini yopish
                        foreach (Window window in WpfApp.Current.Windows)
                        {
                            if (window is LoginWindow)
                            {
                                window.Close();
                                break;
                            }
                        }
                    }
                });
            }
            else
            {
                _notifier.Show(
                    "Xatolik",
                    "Login yoki parol noto'g'ri",
                    NotificationType.Error,
                    expirationTime: TimeSpan.FromSeconds(3));
            }
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning(ex, "Login rad etildi");
            _notifier.Show(
                "Xatolik",
                ex.Message,
                NotificationType.Error,
                expirationTime: TimeSpan.FromSeconds(4));
        }
        catch (ApplicationException ex)
        {
            _logger.LogError(ex, "Login xato");
            _notifier.Show(
                "Xatolik",
                ex.Message,
                NotificationType.Error,
                expirationTime: TimeSpan.FromSeconds(4));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kutilmagan xatolik");
            _notifier.Show(
                "Xatolik",
                "Kutilmagan xatolik yuz berdi",
                NotificationType.Error,
                expirationTime: TimeSpan.FromSeconds(4));
        }
        finally
        {
            IsLoggingIn = false;
            IsBusy = false;
        }
    }

    /// <summary>
    /// Login tugmasini faollashtirishni tekshirish
    /// </summary>
    private bool CanLogin()
    {
        return !IsLoggingIn &&
               !string.IsNullOrWhiteSpace(PhoneNumber) &&
               !string.IsNullOrWhiteSpace(Password);
    }

    /// <summary>
    /// Telefon raqamni tozalash (faqat raqamlar qoladi)
    /// </summary>
    private string CleanPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return string.Empty;

        // Faqat raqamlarni qoldirish
        var cleaned = Regex.Replace(phoneNumber, @"[^\d]", "");

        // Agar 998 bilan boshlanmasa va 9 ta raqam bo'lsa, 998 qo'shish
        if (cleaned.Length == 9 && !cleaned.StartsWith("998"))
        {
            cleaned = "998" + cleaned;
        }

        return cleaned;
    }

    /// <summary>
    /// Property o'zgarganda Login command ni qayta tekshirish
    /// </summary>
    partial void OnPhoneNumberChanged(string value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnPasswordChanged(string value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }
}
