using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Metra.Application.DTOs.Requests.Malumotlar;
using Metra.Application.DTOs.Responses.Malumotlar;
using Metra.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Notification.Wpf;

namespace Metra.Desktop.ViewModels;

/// <summary>
/// Mijoz qo'shish/tahrirlash ViewModel
/// </summary>
public partial class MijozFormViewModel : ObservableObject
{
    private readonly IMijozService _mijozService;
    private readonly ILogger<MijozFormViewModel> _logger;
    private readonly NotificationManager _notifier;

    private bool _isEditMode;
    private int _editingId;

    public MijozFormViewModel(
        IMijozService mijozService,
        ILogger<MijozFormViewModel> logger,
        NotificationManager notifier)
    {
        _mijozService = mijozService;
        _logger = logger;
        _notifier = notifier;
    }

    public string Title => _isEditMode ? "Mijozni tahrirlash" : "Yangi mijoz";

    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _phone2 = string.Empty;
    [ObservableProperty] private string _address = string.Empty;
    [ObservableProperty] private string _passportSeries = string.Empty;
    [ObservableProperty] private string _passportNumber = string.Empty;
    [ObservableProperty] private string _pnfl = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _whenGiven = string.Empty;
    [ObservableProperty] private string? _birthDay;
    [ObservableProperty] private bool _isLoading;

    public void PrepareForAdd()
    {
        _isEditMode = false;
        ClearForm();
    }

    public void PrepareForEdit(MijozResponse mijoz)
    {
        _isEditMode = true;
        _editingId = mijoz.Id;

        Name = mijoz.Name;
        Phone = mijoz.Phone;
        Phone2 = mijoz.Phone2 ?? string.Empty;
        Address = mijoz.Address ?? string.Empty;
        PassportSeries = mijoz.PassportSeries ?? string.Empty;
        PassportNumber = mijoz.PassportNumber ?? string.Empty;
        Pnfl = mijoz.Pnfl ?? string.Empty;
        Description = mijoz.Description ?? string.Empty;
        WhenGiven = mijoz.WhenGiven ?? string.Empty;
        BirthDay = mijoz.BirthDay;
    }

    public async Task<bool> SaveAsync()
    {
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
            IsLoading = true;

            if (_isEditMode)
            {
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
                    _notifier.Show("Muvaffaqiyat", "Tahrirlandi", NotificationType.Success);

                return success;
            }
            else
            {
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
                    _notifier.Show("Muvaffaqiyat", "Qo'shildi", NotificationType.Success);

                return success;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saqlashda xatolik");
            _notifier.Show("Xatolik", "Saqlanmadi", NotificationType.Error);
            return false;
        }
        finally
        {
            IsLoading = false;
        }
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
}
