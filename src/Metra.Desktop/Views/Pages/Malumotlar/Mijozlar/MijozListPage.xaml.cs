using System.Windows.Controls;
using Metra.Desktop.ViewModels.Malumotlar.MIjozlar;

namespace Metra.Desktop.Views.Pages;

public partial class MijozListPage : UserControl
{
    public MijozListPage(MijozPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}
