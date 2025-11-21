using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Metra.Application.DTOs.Requests;
using Metra.Application.DTOs.Responses;
using Metra.Application.Services.Interfaces;
using Metra.Desktop.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Notification.Wpf;

namespace Metra.Desktop.ViewModels;

/// <summary>
/// Filiallar sahifasi uchun ViewModel
/// </summary>
public partial class FilialViewModel : ViewModelBase
{
    private readonly IFilialService _filialService;
    private readonly ILogger<FilialViewModel> _logger;
    private readonly NotificationManager _notifier;
    private System.Threading.Timer? _searchDebounceTimer;

    private ObservableCollection<FilialResponse> _filials = new();
    private FilialResponse? _selectedFilial;
    private string _searchText = string.Empty;
    private bool _isAddEditDialogOpen;
    private bool _isEditMode;
    private string _dialogTitle = "Yangi filial qo'shish";

    // Dialog form fields
    private string _filialName = string.Empty;
    private string _filialDescription = string.Empty;
    private string _filialType = "branch";
    private int _editingFilialId;

    public FilialViewModel(
        IFilialService filialService,
        ILogger<FilialViewModel> logger,
        NotificationManager notifier)
    {
        _filialService = filialService;
        _logger = logger;
        _notifier = notifier;

        Title = "Filiallar";
    }

    #region Properties

    /// <summary>
    /// Filiallar ro'yxati
    /// </summary>
    public ObservableCollection<FilialResponse> Filials
    {
        get => _filials;
        set => SetProperty(ref _filials, value);
    }

    /// <summary>
    /// Tanlangan filial
    /// </summary>
    public FilialResponse? SelectedFilial
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
                _searchDebounceTimer = new System.Threading.Timer(
                    async _ => await SearchWithDebounceAsync(),
                    null,
                    500,
                    System.Threading.Timeout.Infinite);
            }
        }
    }

    /// <summary>
    /// Dialog ochiq/yopiq
    /// </summary>
    public bool IsAddEditDialogOpen
    {
        get => _isAddEditDialogOpen;
        set => SetProperty(ref _isAddEditDialogOpen, value);
    }

    /// <summary>
    /// Tahrirlash rejimimi
    /// </summary>
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    /// <summary>
    /// Dialog sarlavhasi
    /// </summary>
    public string DialogTitle
    {
        get => _dialogTitle;
        set => SetProperty(ref _dialogTitle, value);
    }

    /// <summary>
    /// Filial nomi
    /// </summary>
    public string FilialName
    {
        get => _filialName;
        set => SetProperty(ref _filialName, value);
    }

    /// <summary>
    /// Filial ta'rifi
    /// </summary>
    public string FilialDescription
    {
        get => _filialDescription;
        set => SetProperty(ref _filialDescription, value);
    }

    /// <summary>
    /// Filial turi (main, branch, warehouse)
    /// </summary>
    public string FilialType
    {
        get => _filialType;
        set => SetProperty(ref _filialType, value);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Filiallarni yuklash
    /// </summary>
    [RelayCommand]
    private async Task LoadFilialsAsync()
    {
        try
        {
            IsBusy = true;
            _logger.LogInformation("Filiallar yuklanmoqda...");

            var filials = await _filialService.GetAllAsync(string.IsNullOrEmpty(SearchText) ? null : SearchText);

            if (filials != null)
            {
                // Raqamlash
                for (int i = 0; i < filials.Count; i++)
                {
                    filials[i].Number = i + 1;
                }

                Filials = new ObservableCollection<FilialResponse>(filials);
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
    /// Yangi filial qo'shish dialogini ochish
    /// </summary>
    [RelayCommand]
    private void OpenAddDialog()
    {
        IsEditMode = false;
        DialogTitle = "Yangi filial qo'shish";
        FilialName = string.Empty;
        FilialDescription = string.Empty;
        FilialType = "branch";
        IsAddEditDialogOpen = true;
    }

    /// <summary>
    /// Filialni tahrirlash dialogini ochish
    /// </summary>
    [RelayCommand]
    private void OpenEditDialog()
    {
        if (SelectedFilial == null)
        {
            _notifier.Show(
                "Ogohlantirish",
                "Tahrirlamoqchi bo'lgan filialni tanlang",
                NotificationType.Warning);
            return;
        }

        IsEditMode = true;
        DialogTitle = "Filialni tahrirlash";
        _editingFilialId = SelectedFilial.Id;
        FilialName = SelectedFilial.Name;
        FilialDescription = SelectedFilial.Description ?? string.Empty;
        FilialType = SelectedFilial.Type;
        IsAddEditDialogOpen = true;
    }

    /// <summary>
    /// Dialogni yopish
    /// </summary>
    [RelayCommand]
    private void CloseDialog()
    {
        IsAddEditDialogOpen = false;
    }

    /// <summary>
    /// Filialni saqlash (qo'shish yoki tahrirlash)
    /// </summary>
    [RelayCommand]
    private async Task SaveFilialAsync()
    {
        try
        {
            // Validatsiya
            if (string.IsNullOrWhiteSpace(FilialName))
            {
                _notifier.Show(
                    "Ogohlantirish",
                    "Filial nomini kiriting",
                    NotificationType.Warning);
                return;
            }

            IsBusy = true;

            bool success;

            if (IsEditMode)
            {
                // Tahrirlash
                var request = new FilialUpdateRequest
                {
                    Name = FilialName.Trim(),
                    Description = string.IsNullOrWhiteSpace(FilialDescription) ? null : FilialDescription.Trim(),
                    Type = FilialType
                };

                success = await _filialService.UpdateAsync(_editingFilialId, request);

                if (success)
                {
                    _logger.LogInformation("Filial tahrirlandi: {Id}", _editingFilialId);
                    _notifier.Show(
                        "Muvaffaqiyat",
                        "Filial muvaffaqiyatli tahrirlandi",
                        NotificationType.Success);
                }
            }
            else
            {
                // Yangi qo'shish
                var request = new FilialCreateRequest
                {
                    Name = FilialName.Trim(),
                    Description = string.IsNullOrWhiteSpace(FilialDescription) ? null : FilialDescription.Trim(),
                    Type = FilialType
                };

                success = await _filialService.CreateAsync(request);

                if (success)
                {
                    _logger.LogInformation("Yangi filial qo'shildi: {Name}", FilialName);
                    _notifier.Show(
                        "Muvaffaqiyat",
                        "Filial muvaffaqiyatli qo'shildi",
                        NotificationType.Success);
                }
            }

            if (success)
            {
                IsAddEditDialogOpen = false;
                await LoadFilialsAsync();
            }
            else
            {
                _notifier.Show(
                    "Xatolik",
                    "Filial saqlanmadi",
                    NotificationType.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filial saqlashda xatolik");
            _notifier.Show(
                "Xatolik",
                "Filial saqlashda xatolik yuz berdi",
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

        var result = MessageBox.Show(
            $"{SelectedFilial.Name} filialini o'chirmoqchimisiz?",
            "Tasdiqlash",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            IsBusy = true;
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
