using System.Windows;
using Metra.Desktop.ViewModels;

namespace Metra.Desktop.Views.Windows;

public partial class MijozFormWindow : Window
{
    private readonly MijozFormViewModel _viewModel;

    public MijozFormWindow(MijozFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var success = await _viewModel.SaveAsync();
        if (success)
        {
            DialogResult = true;
            Close();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
