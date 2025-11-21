using System.Windows.Controls;
using Metra.Desktop.ViewModels;

namespace Metra.Desktop.Views.Pages;

/// <summary>
/// Interaction logic for FilialPage.xaml
/// </summary>
public partial class FilialPage : UserControl
{
    public FilialPage(FilialViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Initialize ViewModel when page loads
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}
