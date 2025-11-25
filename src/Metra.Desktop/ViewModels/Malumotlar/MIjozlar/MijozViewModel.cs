using CommunityToolkit.Mvvm.ComponentModel;
using Metra.Application.DTOs.Requests.Malumotlar;
using Metra.Application.DTOs.Responses.Malumotlar;
using Metra.Application.Services.Interfaces;
using Metra.Desktop.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Notification.Wpf;

namespace Metra.Desktop.ViewModels.Malumotlar.MIjozlar;

/// <summary>
/// Mijoz ViewModel (DataGrid row va Form uchun)
/// </summary>
public partial class MijozViewModel : ViewModelBase
{
    private readonly IMijozService _mijozService;
    private readonly ILogger<MijozViewModel> _logger;
    private readonly NotificationManager _notifier;

    // Mode tracking
    private bool _isEditMode;
    private int _editingId;
    private MijozResponse? _originalDto;

    // Constructor for display mode (DataGrid row)
    public MijozViewModel(
        MijozResponse dto,
        int number,
        IMijozService mijozService,
        ILogger<MijozViewModel> logger,
        NotificationManager notifier)
    {
        _mijozService = mijozService;
        _logger = logger;
        _notifier = notifier;
        _originalDto = dto;

        Number = number;
        _editingId = dto.Id;

        // Load from DTO for display
        LoadFromDto(dto);
    }

    // Constructor for form mode (Add/Edit)
    public MijozViewModel(
        IMijozService mijozService,
        ILogger<MijozViewModel> logger,
        NotificationManager notifier)
    {
        _mijozService = mijozService;
        _logger = logger;
        _notifier = notifier;

        Number = 0;
        _isEditMode = false;
    }

    #region Display Properties (read-only from DTO)

    public int Number { get; set; }
    public int Id => _editingId;

    public string? BranchName => _originalDto?.BranchName;
    public DateTime? CreatedAt => _originalDto?.CreatedAt;
    public DateTime? UpdatedAt => _originalDto?.UpdatedAt;

    // Computed properties for UI
    public string PassportDisplay => !string.IsNullOrEmpty(PassportSeries) && !string.IsNullOrEmpty(PassportNumber)
        ? $"{PassportSeries} {PassportNumber}"
        : "-";

    public string Date => _originalDto?.CreatedAt.ToString("dd.MM.yyyy") ?? string.Empty;

    public string ImageUrl => !string.IsNullOrEmpty(_originalDto?.Image)
        ? $"http://app.metra-rent.uz/api/public/storage/{_originalDto.Image}"
        : string.Empty;

    public string ImagePassportUrl => !string.IsNullOrEmpty(_originalDto?.ImagePassport)
        ? $"http://app.metra-rent.uz/api/public/storage/{_originalDto.ImagePassport}"
        : string.Empty;

    #endregion

    #region Form Properties (editable)

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    private string _phone2 = string.Empty;

    [ObservableProperty]
    private string _address = string.Empty;

    [ObservableProperty]
    private string _passportSeries = string.Empty;

    [ObservableProperty]
    private string _passportNumber = string.Empty;

    [ObservableProperty]
    private string _pnfl = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _whenGiven = string.Empty;

    [ObservableProperty]
    private string? _birthDay;

    #endregion

    #region Public Methods

    /// <summary>
    /// Yangi mijoz qo'shish uchun tayyorlash
    /// </summary>
    public void PrepareForAdd()
    {
        _isEditMode = false;
        _originalDto = null;
        Title = "Yangi mijoz";
        ClearForm();
    }

    /// <summary>
    /// Mijozni tahrirlash uchun tayyorlash
    /// </summary>
    public void PrepareForEdit(MijozResponse mijoz)
    {
        _isEditMode = true;
        _editingId = mijoz.Id;
        _originalDto = mijoz;
        Title = "Mijozni tahrirlash";
        LoadFromDto(mijoz);
    }

    /// <summary>
    /// Mijozni saqlash (Create yoki Update)
    /// </summary>
    public async Task<bool> SaveAsync()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(Name))
        {
            _notifier.Show("Ogohlantirish", "Nomni kiriting", NotificationType.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(Phone))
        {
            _notifier.Show("Ogohlantirish", "Telefon kiriting", NotificationType.Warning);
            return false;
        }

        try
        {
            IsBusy = true;

            if (_isEditMode)
            {
                // Update
                var request = new MijozUpdateRequest
                {
                    Name = Name.Trim(),
                    Phone = Phone.Trim(),
                    Phone2 = string.IsNullOrWhiteSpace(Phone2) ? null : Phone2.Trim(),
                    Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
                    PassportSeries = string.IsNullOrWhiteSpace(PassportSeries) ? null : PassportSeries.Trim(),
                    PassportNumber = string.IsNullOrWhiteSpace(PassportNumber) ? null : PassportNumber.Trim(),
                    Pnfl = string.IsNullOrWhiteSpace(Pnfl) ? null : Pnfl.Trim(),
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    WhenGiven = string.IsNullOrWhiteSpace(WhenGiven) ? null : WhenGiven.Trim(),
                    BirthDay = BirthDay
                };

                var success = await _mijozService.UpdateAsync(_editingId, request);
                if (success)
                {
                    _logger.LogInformation("Mijoz tahrirlandi: {Id}", _editingId);
                    _notifier.Show("Muvaffaqiyat", "Tahrirlandi", NotificationType.Success);
                }

                return success;
            }
            else
            {
                // Create
                var request = new MijozRequest
                {
                    Name = Name.Trim(),
                    Phone = Phone.Trim(),
                    Phone2 = string.IsNullOrWhiteSpace(Phone2) ? null : Phone2.Trim(),
                    Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
                    PassportSeries = string.IsNullOrWhiteSpace(PassportSeries) ? null : PassportSeries.Trim(),
                    PassportNumber = string.IsNullOrWhiteSpace(PassportNumber) ? null : PassportNumber.Trim(),
                    Pnfl = string.IsNullOrWhiteSpace(Pnfl) ? null : Pnfl.Trim(),
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    WhenGiven = string.IsNullOrWhiteSpace(WhenGiven) ? null : WhenGiven.Trim(),
                    BirthDay = BirthDay
                };

                var success = await _mijozService.CreateAsync(request);
                if (success)
                {
                    _logger.LogInformation("Yangi mijoz qo'shildi: {Name}", Name);
                    _notifier.Show("Muvaffaqiyat", "Qo'shildi", NotificationType.Success);
                }

                return success;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mijozni saqlashda xatolik");
            _notifier.Show("Xatolik", "Saqlanmadi", NotificationType.Error);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Access to original DTO when needed
    /// </summary>
    public MijozResponse? GetDto() => _originalDto;

    #endregion

    #region Private Methods

    private void LoadFromDto(MijozResponse dto)
    {
        Name = dto.Name;
        Phone = dto.Phone;
        Phone2 = dto.Phone2 ?? string.Empty;
        Address = dto.Address ?? string.Empty;
        PassportSeries = dto.PassportSeries ?? string.Empty;
        PassportNumber = dto.PassportNumber ?? string.Empty;
        Pnfl = dto.Pnfl ?? string.Empty;
        Description = dto.Description ?? string.Empty;
        WhenGiven = dto.WhenGiven ?? string.Empty;
        BirthDay = dto.BirthDay;
    }

    private void ClearForm()
    {
        Name = string.Empty;
        Phone = string.Empty;
        Phone2 = string.Empty;
        Address = string.Empty;
        PassportSeries = string.Empty;
        PassportNumber = string.Empty;
        Pnfl = string.Empty;
        Description = string.Empty;
        WhenGiven = string.Empty;
        BirthDay = null;
    }

    #endregion
}
