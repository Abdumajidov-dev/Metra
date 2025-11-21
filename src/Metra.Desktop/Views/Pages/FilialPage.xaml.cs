using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Metra.Desktop.ViewModels;

namespace Metra.Desktop.Views.Pages;

/// <summary>
/// Interaction logic for FilialPage.xaml
/// </summary>
public partial class FilialPage : UserControl
{
    private readonly FilialViewModel _viewModel;

    public FilialPage(FilialViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        // Initialize ViewModel when page loads
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }

    /// <summary>
    /// DataGrid row chap tugma bilan ikki marta bosilganda tahrirlash dialogini ochish
    /// </summary>
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Faqat chap tugma uchun
        if (e.ChangedButton == MouseButton.Left && _viewModel.SelectedFilial != null)
        {
            _viewModel.OpenEditDialogCommand.Execute(null);
            e.Handled = true;
        }
    }

    /// <summary>
    /// DataGrid row o'ng tugma bosilganda context menu ochish
    /// </summary>
    private void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        // Row ustida o'ng klik qilinganini tekshirish
        var dataGrid = sender as DataGrid;
        if (dataGrid == null) return;

        var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
        if (row != null && _viewModel.SelectedFilial != null)
        {
            // Context menu'ni ko'rsatish
            _viewModel.IsContextMenuOpen = true;
            e.Handled = true;
        }
    }

    /// <summary>
    /// Visual tree'da parent element topish
    /// </summary>
    private static T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject
    {
        while (child != null)
        {
            if (child is T parent)
                return parent;

            child = System.Windows.Media.VisualTreeHelper.GetParent(child);
        }
        return null;
    }

    /// <summary>
    /// Context menu overlay bosilganda yopish
    /// </summary>
    private void ContextMenuOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Faqat overlay (background) bosilganda yopish
        if (e.Source == e.OriginalSource)
        {
            _viewModel.CloseContextMenuCommand.Execute(null);
        }
    }
}
