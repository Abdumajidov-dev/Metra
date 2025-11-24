using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Metra.Desktop.ViewModels;

namespace Metra.Desktop.Views.Windows;

/// <summary>
/// Filial qo'shish va tahrirlash oynasi
/// </summary>
public partial class FilialAddEditWindow : Window
{
    private readonly FilialViewModel _viewModel;

    public FilialAddEditWindow(FilialViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        // Window resources
        Resources.Add("BoolToVisibilityConverter", new System.Windows.Controls.BooleanToVisibilityConverter());
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var result = await _viewModel.SaveFilialAsync();
        if (result)
        {
            DialogResult = true;
            Close();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.HasChanges())
        {
            var result = MessageBox.Show(
                "Siz o'zgarishlar qildingiz. Haqiqatdan chiqishni xohlaysizmi?",
                "Ogohlantirish",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;
        }

        DialogResult = false;
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        CancelButton_Click(sender, e);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (DialogResult == null && _viewModel.HasChanges())
        {
            var result = MessageBox.Show(
                "Siz o'zgarishlar qildingiz. Haqiqatdan chiqishni xohlaysizmi?",
                "Ogohlantirish",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        base.OnClosing(e);
    }
}
