using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Metra.Application.DTOs.Responses;
using Metra.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notification.Wpf;

namespace Metra.Desktop.ViewModels;

/// <summary>
/// Mijozlar ro'yxati ViewModel
/// </summary>
public partial class MijozListViewModel : ObservableObject
{
    private readonly IMijozService _mijozService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MijozListViewModel> _logger;
    private readonly NotificationManager _notifier;

    public MijozListViewModel(
        IMijozService mijozService,
        IServiceProvider serviceProvider,
        ILogger<MijozListViewModel> logger,
        NotificationManager notifier)
    {
        _mijozService = mijozService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _notifier = notifier;
    }

    [ObservableProperty]
    private ObservableCollection<MijozResponse> _mijozlar = new();

    [ObservableProperty]
    private MijozResponse? _selectedMijoz;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private bool _isLoading;

    partial void OnSearchTextChanged(string value)
    {
        CurrentPage = 1;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var result = await _mijozService.GetAllAsync(CurrentPage, string.IsNullOrEmpty(SearchText) ? null : SearchText);

            if (result != null)
            {
                for (int i = 0; i < result.Data.Count; i++)
                    result.Data[i].Number = (CurrentPage - 1) * result.PerPage + i + 1;

                Mijozlar = new ObservableCollection<MijozResponse>(result.Data);
                TotalPages = result.LastPage;
                TotalCount = result.Total;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mijozlar yuklashda xatolik");
            _notifier.Show("Xatolik", "Ma'lumotlar yuklanmadi", NotificationType.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        SearchText = string.Empty;
        CurrentPage = 1;
        await LoadDataAsync();
    }

    [RelayCommand]
    private void OpenAddDialog()
    {
        var formVm = _serviceProvider.GetRequiredService<MijozFormViewModel>();
        formVm.PrepareForAdd();

        var window = new Views.Windows.MijozFormWindow(formVm);
        if (window.ShowDialog() == true)
            _ = LoadDataAsync();
    }

    [RelayCommand]
    private void OpenEditDialog()
    {
        if (SelectedMijoz == null)
        {
            _notifier.Show("Ogohlantirish", "Mijozni tanlang", NotificationType.Warning);
            return;
        }

        var formVm = _serviceProvider.GetRequiredService<MijozFormViewModel>();
        formVm.PrepareForEdit(SelectedMijoz);

        var window = new Views.Windows.MijozFormWindow(formVm);
        if (window.ShowDialog() == true)
            _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedMijoz == null)
        {
            _notifier.Show("Ogohlantirish", "Mijozni tanlang", NotificationType.Warning);
            return;
        }

        var result = System.Windows.MessageBox.Show(
            $"{SelectedMijoz.Name} ni o'chirmoqchimisiz?",
            "Tasdiqlash",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;
            var success = await _mijozService.DeleteAsync(SelectedMijoz.Id);

            if (success)
            {
                _notifier.Show("Muvaffaqiyat", "Mijoz o'chirildi", NotificationType.Success);
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "O'chirishda xatolik");
            _notifier.Show("Xatolik", "O'chirilmadi", NotificationType.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task InitializeAsync()
    {
        await LoadDataAsync();
    }
}
