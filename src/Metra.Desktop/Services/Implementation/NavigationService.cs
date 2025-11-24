using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Metra.Desktop.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Metra.Desktop.Services.Implementation;

/// <summary>
/// Navigation xizmati - sahifalar orasida harakat qilish
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavigationService> _logger;
    private readonly Stack<UserControl> _navigationStack = new();
    private ContentControl? _contentControl;

    public UserControl? CurrentPage { get; private set; }

    public NavigationService(
        IServiceProvider serviceProvider,
        ILogger<NavigationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Navigation uchun ContentControl ni belgilash
    /// </summary>
    public void SetNavigationTarget(ContentControl contentControl)
    {
        _contentControl = contentControl;
    }

    /// <summary>
    /// Sahifaga o'tish
    /// </summary>
    public void NavigateTo<TPage>() where TPage : UserControl
    {
        try
        {
            if (_contentControl == null)
            {
                throw new InvalidOperationException("Navigation target (ContentControl) belgilanmagan. SetNavigationTarget() ni chaqiring.");
            }

            _logger.LogInformation("Sahifaga o'tilmoqda: {PageType}", typeof(TPage).Name);

            // Hozirgi sahifani stackga saqlash
            if (CurrentPage != null)
            {
                _navigationStack.Push(CurrentPage);
            }

            // Yangi sahifani yaratish
            var page = _serviceProvider.GetRequiredService<TPage>();
            CurrentPage = page;
            _contentControl.Content = page;

            _logger.LogInformation("Sahifa yuklandi: {PageType}", typeof(TPage).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sahifaga o'tishda xatolik: {PageType}", typeof(TPage).Name);
            throw;
        }
    }

    /// <summary>
    /// Orqaga qaytish
    /// </summary>
    public void GoBack()
    {
        if (_navigationStack.Count > 0 && _contentControl != null)
        {
            CurrentPage = _navigationStack.Pop();
            _contentControl.Content = CurrentPage;
            _logger.LogInformation("Orqaga qaytildi");
        }
        else
        {
            _logger.LogWarning("Orqaga qaytish uchun sahifa yo'q");
        }
    }
}
