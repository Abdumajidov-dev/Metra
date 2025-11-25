using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Metra.Application.DTOs.Responses.Malumotlar;
using Metra.Application.Services.Interfaces;
using Metra.Desktop.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notification.Wpf;

namespace Metra.Desktop.ViewModels.Malumotlar.MIjozlar;

/// <summary>
/// Mijozlar ro'yxati ViewModel
/// </summary>
public partial class MijozPageViewModel : ViewModelBase
{
    private readonly IMijozService _mijozService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MijozPageViewModel> _logger;
    private readonly NotificationManager _notifier;

    public MijozPageViewModel(
        IMijozService mijozService,
        IServiceProvider serviceProvider,
        ILogger<MijozPageViewModel> logger,
        NotificationManager notifier)
    {
        _mijozService = mijozService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _notifier = notifier;
    }

    [ObservableProperty]
    private ObservableCollection<MijozViewModel> _mijozlar = new();

    [ObservableProperty]
    private MijozViewModel? _selectedMijoz;

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
                var items = result.Data.Select((dto, index) =>
                {
                    var vm = _serviceProvider.GetRequiredService<MijozViewModel>();
                    // Initialize for display mode with correct row number
                    vm.PrepareForEdit(dto); // Load DTO data
                    vm.Number = (CurrentPage - 1) * result.PerPage + index + 1; // Set row number
                    return vm;
                }).ToList();

                Mijozlar = new ObservableCollection<MijozViewModel>(items);
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
        var formVm = _serviceProvider.GetRequiredService<MijozViewModel>();
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

        var formVm = _serviceProvider.GetRequiredService<MijozViewModel>();
        var dto = SelectedMijoz.GetDto();
        if (dto != null)
        {
            formVm.PrepareForEdit(dto);

            var window = new Views.Windows.MijozFormWindow(formVm);
            if (window.ShowDialog() == true)
                _ = LoadDataAsync();
        }
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

    public override async Task InitializeAsync()
    {
        await LoadDataAsync();
    }
}
