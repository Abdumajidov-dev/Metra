using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Metra.Application.Services.Interfaces.Malumotlar;
using Metra.Desktop.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notification.Wpf;

namespace Metra.Desktop.ViewModels.Malumotlar.Filiallar;

/// <summary>
/// Filiallar sahifasi uchun ViewModel
/// </summary>
public partial class FilialPageViewModel : ViewModelBase
{
    private readonly IFilialService _filialService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FilialPageViewModel> _logger;
    private readonly NotificationManager _notifier;
    private Timer? _searchDebounceTimer;

    private ObservableCollection<FilialViewModel> _filials = new();
    private FilialViewModel? _selectedFilial;
    private string _searchText = string.Empty;
    private bool _isContextMenuOpen;

    public FilialPageViewModel(
        IFilialService filialService,
        IServiceProvider serviceProvider,
        ILogger<FilialPageViewModel> logger,
        NotificationManager notifier)
    {
        _filialService = filialService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _notifier = notifier;

        Title = "Filiallar";
    }

    #region Properties

    /// <summary>
    /// Filiallar ro'yxati
    /// </summary>
    public ObservableCollection<FilialViewModel> Filials
    {
        get => _filials;
        set => SetProperty(ref _filials, value);
    }

    /// <summary>
    /// Tanlangan filial
    /// </summary>
    public FilialViewModel? SelectedFilial
    {
        get => _selectedFilial;
        set => SetProperty(ref _selectedFilial, value);
    }

    /// <summary>
    /// Qidiruv matni
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                // Debounce search - 500ms kutadi
                _searchDebounceTimer?.Dispose();
                _searchDebounceTimer = new Timer(
                    async _ => await SearchWithDebounceAsync(),
                    null,
                    500,
                    Timeout.Infinite);
            }
        }
    }

    /// <summary>
    /// Context menu ochiq/yopiq
    /// </summary>
    public bool IsContextMenuOpen
    {
        get => _isContextMenuOpen;
        set => SetProperty(ref _isContextMenuOpen, value);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Filiallarni yuklash
    /// </summary>
    [RelayCommand]
    public async Task LoadFilialsAsync()
    {
        try
        {
            IsBusy = true;
            _logger.LogInformation("Filiallar yuklanmoqda...");

            var filials = await _filialService.GetAllAsync(string.IsNullOrEmpty(SearchText) ? null : SearchText);

            if (filials != null)
            {
                var items = filials.Select((dto, index) =>
                {
                    var vm = _serviceProvider.GetRequiredService<FilialViewModel>();
                    vm.PrepareForEdit(dto); // Load DTO data into ViewModel
                    vm.Number = index + 1; // Set row number (1-based, not paginated for Filial)
                    return vm;
                }).ToList();

                Filials = new ObservableCollection<FilialViewModel>(items);
                _logger.LogInformation("Filiallar yuklandi: {Count}", filials.Count);
            }
            else
            {
                Filials.Clear();
                _logger.LogWarning("Filiallar yuklanmadi");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filiallar yuklashda xatolik");
            _notifier.Show(
                "Xatolik",
                "Filiallar yuklanishda xatolik yuz berdi",
                NotificationType.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Filialni o'chirish
    /// </summary>
    [RelayCommand]
    private async Task DeleteFilialAsync()
    {
        if (SelectedFilial == null)
        {
            _notifier.Show(
                "Ogohlantirish",
                "O'chirmoqchi bo'lgan filialni tanlang",
                NotificationType.Warning);
            return;
        }

        try
        {
            IsBusy = true;
            IsContextMenuOpen = false; // Context menu'ni yopish
            _logger.LogInformation("Filial o'chirilmoqda: {Id}", SelectedFilial.Id);

            var success = await _filialService.DeleteAsync(SelectedFilial.Id);

            if (success)
            {
                _logger.LogInformation("Filial o'chirildi: {Id}", SelectedFilial.Id);
                _notifier.Show(
                    "Muvaffaqiyat",
                    "Filial muvaffaqiyatli o'chirildi",
                    NotificationType.Success);

                await LoadFilialsAsync();
            }
            else
            {
                _notifier.Show(
                    "Xatolik",
                    "Filial o'chirilmadi",
                    NotificationType.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filial o'chirishda xatolik");
            _notifier.Show(
                "Xatolik",
                "Filial o'chirishda xatolik yuz berdi",
                NotificationType.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Yangilash
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        SearchText = string.Empty;
        await LoadFilialsAsync();
    }

    /// <summary>
    /// Context menu'ni yopish
    /// </summary>
    [RelayCommand]
    private void CloseContextMenu()
    {
        IsContextMenuOpen = false;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Debounced qidiruv
    /// </summary>
    private async Task SearchWithDebounceAsync()
    {
        await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            await LoadFilialsAsync();
        });
    }

    #endregion

    /// <summary>
    /// ViewModel initialization
    /// </summary>
    public override async Task InitializeAsync()
    {
        await LoadFilialsAsync();
    }

    /// <summary>
    /// Cleanup
    /// </summary>
    public override void Cleanup()
    {
        _searchDebounceTimer?.Dispose();
        base.Cleanup();
    }
}
