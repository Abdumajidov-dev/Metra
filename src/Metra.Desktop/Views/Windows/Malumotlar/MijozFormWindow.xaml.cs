using System.Windows;
using Metra.Desktop.ViewModels.Malumotlar.MIjozlar;

namespace Metra.Desktop.Views.Windows;

public partial class MijozFormWindow : Window
{
    private readonly MijozViewModel _viewModel;

    public MijozFormWindow(MijozViewModel viewModel)
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
