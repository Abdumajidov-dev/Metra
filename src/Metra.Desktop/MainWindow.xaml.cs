using System.Windows;
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
    }
}