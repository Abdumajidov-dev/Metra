using CommunityToolkit.Mvvm.ComponentModel;
using Metra.Application.DTOs.Requests.Malumotlar;
using Metra.Application.DTOs.Responses.Malumotlar;
using Metra.Application.Services.Interfaces.Malumotlar;
using Metra.Desktop.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Notification.Wpf;

namespace Metra.Desktop.ViewModels.Malumotlar.Filiallar;

/// <summary>
/// Filial ViewModel (DataGrid row va Form uchun)
/// </summary>
public partial class FilialViewModel : ViewModelBase
{
    private readonly IFilialService _filialService;
    private readonly ILogger<FilialViewModel> _logger;
    private readonly NotificationManager _notifier;

    // Mode tracking
    private bool _isEditMode;
    private int _editingFilialId;
    private FilialResponse? _originalDto;

    // Constructor for display mode (DataGrid row)
    public FilialViewModel(
        FilialResponse dto,
        int number,
        IFilialService filialService,
        ILogger<FilialViewModel> logger,
        NotificationManager notifier)
    {
        _filialService = filialService;
        _logger = logger;
        _notifier = notifier;
        _originalDto = dto;

        Number = number;
        _editingFilialId = dto.Id;

        // Load from DTO for display
        LoadFromDto(dto);
    }

    // Constructor for form mode (Add/Edit)
    public FilialViewModel(
        IFilialService filialService,
        ILogger<FilialViewModel> logger,
        NotificationManager notifier)
    {
        _filialService = filialService;
        _logger = logger;
        _notifier = notifier;

        Number = 0;
        _isEditMode = false;
        _filialType = "branch"; // Default type
    }

    #region Display Properties (read-only from DTO)

    public int Number { get; set; }
    public int Id => _editingFilialId;

    // Name property for DataGrid binding
    public string Name => FilialName;
    public string? Description => FilialDescription;
    public string Type => FilialType;

    public string? ResponsibleWorker => _originalDto?.ResponsibleWorker;
    public string? Date => _originalDto?.Date;
    public string? UpdatedAt => _originalDto?.UpdatedAt;

    // Computed properties for UI
    public string TypeDisplay => FilialType switch
    {
        "main" => "Asosiy",
        "general" => "Asosiy ombor",
        "branch" => "Filial",
        "warehouse" => "Ombor",
        "store" => "Ombor",
        _ => FilialType
    };

    #endregion

    #region Form Properties (editable)

    [ObservableProperty]
    private string _filialName = string.Empty;

    [ObservableProperty]
    private string _filialDescription = string.Empty;

    [ObservableProperty]
    private string _filialType = "branch";

    // Dastlabki qiymatlar (o'zgarishlarni tekshirish uchun)
    private string _originalFilialName = string.Empty;
    private string _originalFilialDescription = string.Empty;
    private string _originalFilialType = "branch";

    #endregion

    #region Public Methods

    /// <summary>
    /// Yangi filial qo'shish uchun tayyorlash
    /// </summary>
    public void PrepareForAdd()
    {
        _isEditMode = false;
        _originalDto = null;
        Title = "Yangi filial qo'shish";
        FilialName = string.Empty;
        FilialDescription = string.Empty;
        FilialType = "branch";

        // Dastlabki qiymatlarni saqlash
        _originalFilialName = string.Empty;
        _originalFilialDescription = string.Empty;
        _originalFilialType = "branch";
    }

    /// <summary>
    /// Filialni tahrirlash uchun tayyorlash
    /// </summary>
    public void PrepareForEdit(FilialResponse filial)
    {
        _isEditMode = true;
        _editingFilialId = filial.Id;
        _originalDto = filial;
        Title = "Filialni tahrirlash";
        LoadFromDto(filial);
    }

    /// <summary>
    /// Filialni saqlash (Create yoki Update)
    /// </summary>
    public async Task<bool> SaveAsync()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(FilialName))
        {
            _notifier.Show("Ogohlantirish", "Filial nomini kiriting", NotificationType.Warning);
            return false;
        }

        try
        {
            IsBusy = true;

            bool success;

            if (_isEditMode)
            {
                // Update
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
                    _notifier.Show("Muvaffaqiyat", "Filial muvaffaqiyatli tahrirlandi", NotificationType.Success);

                    // Dastlabki qiymatlarni yangilash
                    _originalFilialName = FilialName;
                    _originalFilialDescription = FilialDescription;
                    _originalFilialType = FilialType;
                }
            }
            else
            {
                // Create
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
                    _notifier.Show("Muvaffaqiyat", "Filial muvaffaqiyatli qo'shildi", NotificationType.Success);

                    // Dastlabki qiymatlarni yangilash
                    _originalFilialName = FilialName;
                    _originalFilialDescription = FilialDescription;
                    _originalFilialType = FilialType;
                }
            }

            if (!success)
            {
                _notifier.Show("Xatolik", "Filial saqlanmadi", NotificationType.Error);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filialni saqlashda xatolik");
            _notifier.Show("Xatolik", $"Xatolik yuz berdi: {ex.Message}", NotificationType.Error);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Dialog'da o'zgarishlar borligini tekshirish
    /// </summary>
    public bool HasChanges()
    {
        // Filial nomi o'zgardimi
        if (FilialName?.Trim() != _originalFilialName?.Trim())
            return true;

        // Ta'rif o'zgardimi
        var currentDescription = FilialDescription?.Trim() ?? string.Empty;
        var originalDescription = _originalFilialDescription?.Trim() ?? string.Empty;
        if (currentDescription != originalDescription)
            return true;

        // Turi o'zgardimi
        if (FilialType != _originalFilialType)
            return true;

        return false;
    }

    /// <summary>
    /// Access to original DTO when needed
    /// </summary>
    public FilialResponse? GetDto() => _originalDto;

    #endregion

    #region Private Methods

    private void LoadFromDto(FilialResponse dto)
    {
        FilialName = dto.Name;
        FilialDescription = dto.Description ?? string.Empty;
        FilialType = dto.Type;

        // Dastlabki qiymatlarni saqlash
        _originalFilialName = dto.Name;
        _originalFilialDescription = dto.Description ?? string.Empty;
        _originalFilialType = dto.Type;
    }

    #endregion
}
