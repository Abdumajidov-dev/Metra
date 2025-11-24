using System.Windows.Controls;
using Metra.Desktop.ViewModels;

namespace Metra.Desktop.Views.Pages;

public partial class MijozListPage : UserControl
{
    public MijozListPage(MijozListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}
