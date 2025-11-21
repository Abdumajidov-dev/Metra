using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Metra.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(ILogger<MainWindow> logger)
    {
        InitializeComponent();
        _logger = logger;

        _logger.LogInformation("MainWindow yaratildi");

        // NavigationListBox SelectionChanged event handler
        NavigationListBox.SelectionChanged += NavigationListBox_SelectionChanged;
    }

    private void NavigationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NavigationListBox.SelectedItem is not ListBoxItem selectedItem)
            return;

        // Get the text from the ListBoxItem
        var textBlock = selectedItem.Content as TextBlock;
        var itemText = textBlock?.Text ?? string.Empty;

        _logger.LogInformation("Navigation: {ItemText}", itemText);

        // Navigate based on selection
        switch (itemText)
        {
            case "Filiallar":
                NavigateToPage<Views.Pages.FilialPage>();
                break;
            // TODO: Add more navigation cases here
            // case "Mijozlar":
            //     NavigateToPage<Views.Pages.MijozPage>();
            //     break;
            default:
                _logger.LogWarning("Navigation not implemented for: {ItemText}", itemText);
                break;
        }
    }

    private void NavigateToPage<TPage>() where TPage : UserControl
    {
        try
        {
            var page = App.ServiceProvider.GetRequiredService<TPage>();
            ContentArea.Content = page;
            _logger.LogInformation("Navigated to {PageType}", typeof(TPage).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to {PageType}", typeof(TPage).Name);
            MessageBox.Show(
                $"Sahifa ochishda xatolik: {ex.Message}",
                "Xatolik",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}