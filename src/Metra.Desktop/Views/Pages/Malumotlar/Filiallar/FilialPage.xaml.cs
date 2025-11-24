using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Metra.Desktop.ViewModels;
using Metra.Desktop.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

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

        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.PrepareForAdd();
        var window = new FilialAddEditWindow(_viewModel);
        window.ShowDialog();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && _viewModel.SelectedFilial != null)
        {
            _viewModel.PrepareForEdit(_viewModel.SelectedFilial);
            var window = new FilialAddEditWindow(_viewModel);
            window.ShowDialog();
            e.Handled = true;
        }
    }

    private void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        if (dataGrid == null) return;

        var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
        if (row != null && _viewModel.SelectedFilial != null)
        {
            ContextMenuOverlay.Visibility = Visibility.Visible;
            e.Handled = true;
        }
    }

    private void CloseContextMenu_Click(object sender, RoutedEventArgs e)
    {
        ContextMenuOverlay.Visibility = Visibility.Collapsed;
    }

    private void ContextMenuOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source == e.OriginalSource)
        {
            ContextMenuOverlay.Visibility = Visibility.Collapsed;
        }
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedFilial == null) return;

        var result = MessageBox.Show(
            $"{_viewModel.SelectedFilial.Name} filialini o'chirmoqchimisiz?",
            "Tasdiqlash",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            ContextMenuOverlay.Visibility = Visibility.Collapsed;
            await _viewModel.DeleteFilialCommand.ExecuteAsync(null);
        }
        else
        {
            ContextMenuOverlay.Visibility = Visibility.Collapsed;
        }
    }

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
}
