using CommunityToolkit.Mvvm.ComponentModel;

namespace Metra.Desktop.ViewModels.Base;

/// <summary>
/// Barcha ViewModellar uchun bazaviy klass
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    private bool _isBusy;
    private string _title = string.Empty;

    /// <summary>
    /// ViewModel ishlab turishini bildiradi
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            SetProperty(ref _isBusy, value);
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    /// <summary>
    /// ViewModel ishlamayotganini bildiradi
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// ViewModel sarlavhasi (Tab title yoki window title uchun)
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>
    /// ViewModel initialization uchun
    /// </summary>
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// ViewModel tozalash uchun
    /// </summary>
    public virtual void Cleanup()
    {
        // Override this method for cleanup operations
    }
}
