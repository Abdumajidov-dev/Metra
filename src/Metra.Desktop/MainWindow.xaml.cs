using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Metra.Desktop;

public partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(ILogger<MainWindow> logger)
    {
        InitializeComponent();
        _logger = logger;
        _logger.LogInformation("MainWindow ochildi");
    }

    private void NavigationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NavigationListBox.SelectedItem is not ListBoxItem selectedItem) return;

        var text = (selectedItem.Content as TextBlock)?.Text ?? string.Empty;
        _logger.LogInformation("Navigation: {Text}", text);

        switch (text)
        {
            case "Mijozlar":
                NavigateTo<Views.Pages.MijozListPage>();
                break;

            case "Filiallar":
                NavigateTo<Views.Pages.FilialPage>();
                break;
            case "Materiallar chiqim sababi":
                break;


            default:
                _logger.LogWarning("Noma'lum sahifa: {Text}", text);
                break;
        }
    }

    private void NavigateTo<TPage>() where TPage : UserControl
    {
        try
        {
            var page = App.ServiceProvider.GetRequiredService<TPage>();
            ContentArea.Content = page;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Navigation xatolik");
            MessageBox.Show($"Xatolik: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
