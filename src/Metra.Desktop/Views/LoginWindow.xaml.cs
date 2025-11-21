using Metra.Desktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Metra.Desktop.Views;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    /// <summary>
    /// PasswordBox uchun binding (WPF da PasswordBox.Password bindable emas)
    /// </summary>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox && DataContext is LoginViewModel vm)
        {
            vm.Password = passwordBox.Password;
        }
    }
}
