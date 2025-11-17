# Metra v2.0 - Refactoring Guide

Bu faylda loyihani to'g'ri MVVM + DI arxitekturasiga o'tkazish uchun qadamma-qadam yo'riqnoma.

## Bosqich 1: Dependencies O'rnatish

```bash
# NuGet Packages
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Http
dotnet add package Microsoft.Extensions.Logging
dotnet add package CommunityToolkit.Mvvm
dotnet add package Serilog
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Extensions.Logging
```

## Bosqich 2: Folder Structure Yaratish

```
src/
├── ViewModels/
│   ├── Base/
│   │   └── ViewModelBase.cs
│   ├── Mijozlar/
│   │   ├── MijozListViewModel.cs
│   │   └── MijozAddViewModel.cs
│   ├── Shartnoma/
│   │   ├── ShartnomListViewModel.cs
│   │   └── ShartnomAddViewModel.cs
│   └── ...
├── Services/
│   ├── Interfaces/
│   │   ├── IMijozService.cs
│   │   ├── INavigationService.cs
│   │   └── ITokenService.cs
│   └── Implementation/
│       ├── MijozService.cs
│       └── NavigationService.cs
├── Models/ (Entities o'rniga)
└── Commands/ (agar CommunityToolkit ishlatmasangiz)
    └── RelayCommand.cs
```

## Bosqich 3: Base Classes

### ViewModelBase.cs
```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Metra_v2._0.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
```

### RelayCommand.cs (Yoki CommunityToolkit.Mvvm ishlatish tavsiya etiladi)
```csharp
using System;
using System.Windows.Input;

namespace Metra_v2._0.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute((T?)parameter);
        }

        public void Execute(object? parameter)
        {
            _execute((T?)parameter);
        }
    }

    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            CommandManager.InvalidateRequerySuggested();

            try
            {
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
```

## Bosqich 4: Service Interfaces

### Services/Interfaces/IMijozService.cs
```csharp
using Metra_v2._0.Entities.Mijozlar;

namespace Metra_v2._0.Services.Interfaces
{
    public interface IMijozService
    {
        Task<PaginatedResult<MijozView>?> SelectAll(int page = 1, string? search = null, int? filialId = null);
        Task<MijozView?> GetById(int id);
        Task<bool> AddClient(MijozAddView client);
        Task<bool> UpdateClient(int id, MijozUpdateView client);
        Task<bool> DeleteAsync(int id);
    }
}
```

### Services/Interfaces/ITokenService.cs
```csharp
namespace Metra_v2._0.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string?> GetTokenAsync();
        void SetToken(string token);
        void ClearToken();
    }
}
```

### Services/Implementation/TokenService.cs
```csharp
using Metra_v2._0.Properties;
using Metra_v2._0.Services.Interfaces;

namespace Metra_v2._0.Services.Implementation
{
    public class TokenService : ITokenService
    {
        public Task<string?> GetTokenAsync()
        {
            return Task.FromResult(Settings.Default.token);
        }

        public void SetToken(string token)
        {
            Settings.Default.token = token;
            Settings.Default.Save();
        }

        public void ClearToken()
        {
            Settings.Default.token = null;
            Settings.Default.Save();
        }
    }
}
```

### Services/Interfaces/INavigationService.cs
```csharp
namespace Metra_v2._0.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class;
        void OpenInNewTab(string title, object content);
        void CloseCurrentTab();
    }
}
```

## Bosqich 5: Refactored MijozService

### Services/Mijozlar/MijozService.cs (Yangi versiya)
```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Metra_v2._0.Entities.Mijozlar;
using Metra_v2._0.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Metra_v2._0.Services.Mijozlar
{
    public class MijozService : IMijozService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly ILogger<MijozService> _logger;

        public MijozService(
            HttpClient httpClient,
            ITokenService tokenService,
            ILogger<MijozService> logger)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<PaginatedResult<MijozView>?> SelectAll(
            int page = 1,
            string? search = null,
            int? filialId = null)
        {
            try
            {
                var token = await _tokenService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token topilmadi");
                    return null;
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var url = $"/client?page={page}";
                if (!string.IsNullOrEmpty(search))
                    url += $"&search={Uri.EscapeDataString(search)}";
                if (filialId.HasValue)
                    url += $"&filial_id={filialId}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Mijozlar yuklanmadi. Status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PaginatedResult<MijozView>>(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP xatolik mijozlarni yuklashda");
                throw new ApplicationException("Server bilan bog'lanishda xatolik");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mijozlarni yuklashda xatolik");
                throw;
            }
        }

        public async Task<bool> AddClient(MijozAddView client)
        {
            try
            {
                var token = await _tokenService.GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(client);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/client", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Mijoz muvaffaqiyatli qo'shildi: {Name}", client.Name);
                    return true;
                }

                _logger.LogWarning("Mijoz qo'shilmadi. Status: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mijoz qo'shishda xatolik");
                throw new ApplicationException("Mijoz qo'shib bo'lmadi");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var token = await _tokenService.GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"/client/{id}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Mijoz o'chirildi: ID={Id}", id);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mijoz o'chirishda xatolik: ID={Id}", id);
                throw new ApplicationException("Mijoz o'chirib bo'lmadi");
            }
        }

        // Qolgan metodlar...
        public Task<MijozView?> GetById(int id) => throw new NotImplementedException();
        public Task<bool> UpdateClient(int id, MijozUpdateView client) => throw new NotImplementedException();
    }
}
```

## Bosqich 6: ViewModel Example

### ViewModels/Mijozlar/MijozListViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using Metra_v2._0.Commands;
using Metra_v2._0.Entities.Mijozlar;
using Metra_v2._0.Services.Interfaces;
using Metra_v2._0.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Notification.Wpf;

namespace Metra_v2._0.ViewModels.Mijozlar
{
    public class MijozListViewModel : ViewModelBase
    {
        private readonly IMijozService _mijozService;
        private readonly INavigationService _navigationService;
        private readonly ILogger<MijozListViewModel> _logger;
        private readonly NotificationManager _notifier;

        // Properties
        private ObservableCollection<MijozView> _mijozlar = new();
        public ObservableCollection<MijozView> Mijozlar
        {
            get => _mijozlar;
            set => SetProperty(ref _mijozlar, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    SearchCommand.Execute(null);
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        // Commands
        public ICommand LoadCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public MijozListViewModel(
            IMijozService mijozService,
            INavigationService navigationService,
            ILogger<MijozListViewModel> logger,
            NotificationManager notifier)
        {
            _mijozService = mijozService;
            _navigationService = navigationService;
            _logger = logger;
            _notifier = notifier;

            // Initialize commands
            LoadCommand = new AsyncRelayCommand(LoadMijozlar);
            SearchCommand = new AsyncRelayCommand(LoadMijozlar);
            AddCommand = new RelayCommand(OpenAddPage);
            EditCommand = new RelayCommand<MijozView>(EditMijoz);
            DeleteCommand = new RelayCommand<MijozView>(async (mijoz) => await DeleteMijoz(mijoz));
            NextPageCommand = new AsyncRelayCommand(NextPage, () => CurrentPage < TotalPages);
            PreviousPageCommand = new AsyncRelayCommand(PreviousPage, () => CurrentPage > 1);

            // Initial load
            LoadCommand.Execute(null);
        }

        private async Task LoadMijozlar()
        {
            try
            {
                IsLoading = true;

                var result = await _mijozService.SelectAll(CurrentPage, SearchText);

                if (result != null && result.Data != null)
                {
                    Mijozlar = new ObservableCollection<MijozView>(result.Data);
                    TotalPages = result.LastPage;
                }
            }
            catch (ApplicationException ex)
            {
                _notifier.Show("Xatolik", ex.Message, NotificationType.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mijozlarni yuklashda kutilmagan xatolik");
                _notifier.Show("Xatolik", "Kutilmagan xatolik yuz berdi", NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OpenAddPage()
        {
            // NavigationService orqali yangi sahifa ochish
            _navigationService.NavigateTo<MijozAddViewModel>();
        }

        private void EditMijoz(MijozView? mijoz)
        {
            if (mijoz == null) return;
            _navigationService.NavigateTo<MijozAddViewModel>(mijoz.Id);
        }

        private async Task DeleteMijoz(MijozView? mijoz)
        {
            if (mijoz == null) return;

            try
            {
                var result = System.Windows.MessageBox.Show(
                    $"{mijoz.Name} ni o'chirmoqchimisiz?",
                    "Tasdiqlash",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result != System.Windows.MessageBoxResult.Yes)
                    return;

                IsLoading = true;

                var success = await _mijozService.DeleteAsync(mijoz.Id);

                if (success)
                {
                    Mijozlar.Remove(mijoz);
                    _notifier.Show("Muvaffaqiyat", "Mijoz o'chirildi", NotificationType.Success);
                }
                else
                {
                    _notifier.Show("Xatolik", "Mijoz o'chirib bo'lmadi", NotificationType.Error);
                }
            }
            catch (ApplicationException ex)
            {
                _notifier.Show("Xatolik", ex.Message, NotificationType.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mijoz o'chirishda xatolik");
                _notifier.Show("Xatolik", "Kutilmagan xatolik", NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadMijozlar();
            }
        }

        private async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadMijozlar();
            }
        }
    }
}
```

### Controls/Mijozlar/MijozPage.xaml.cs (Code-behind - minimallashtirilgan)
```csharp
using System.Windows.Controls;
using Metra_v2._0.ViewModels.Mijozlar;

namespace Metra_v2._0.Controls.Mijozlar
{
    public partial class MijozPage : UserControl
    {
        public MijozPage(MijozListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
```

### Controls/Mijozlar/MijozPage.xaml
```xml
<UserControl x:Class="Metra_v2._0.Controls.Mijozlar.MijozPage"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:md="http://materialdesigninxaml.net/winfx/xaml/themes">

    <Grid>
        <!-- Loading Overlay -->
        <Grid x:Name="loadingOverlay"
              Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibilityConverter}}"
              Background="#80000000"
              Panel.ZIndex="999">
            <ProgressBar IsIndeterminate="True" Width="200" Height="20"/>
        </Grid>

        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>

            <!-- Toolbar -->
            <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="10">
                <TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"
                         md:HintAssist.Hint="Qidirish..."
                         Width="300"
                         Margin="0,0,10,0"/>

                <Button Content="Yangi"
                        Command="{Binding AddCommand}"
                        Style="{StaticResource MaterialDesignRaisedButton}"/>
            </StackPanel>

            <!-- DataGrid -->
            <DataGrid Grid.Row="1"
                      ItemsSource="{Binding Mijozlar}"
                      AutoGenerateColumns="False"
                      IsReadOnly="True"
                      SelectionMode="Single">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="ID" Binding="{Binding Id}"/>
                    <DataGridTextColumn Header="Ism" Binding="{Binding Name}"/>
                    <DataGridTextColumn Header="Telefon" Binding="{Binding Phone}"/>
                    <DataGridTextColumn Header="Manzil" Binding="{Binding Address}"/>

                    <DataGridTemplateColumn Header="Amallar">
                        <DataGridTemplateColumn.CellTemplate>
                            <DataTemplate>
                                <StackPanel Orientation="Horizontal">
                                    <Button Content="Tahrirlash"
                                            Command="{Binding DataContext.EditCommand,
                                                     RelativeSource={RelativeSource AncestorType=DataGrid}}"
                                            CommandParameter="{Binding}"
                                            Margin="5,0"/>

                                    <Button Content="O'chirish"
                                            Command="{Binding DataContext.DeleteCommand,
                                                     RelativeSource={RelativeSource AncestorType=DataGrid}}"
                                            CommandParameter="{Binding}"
                                            Style="{StaticResource MaterialDesignRaisedAccentButton}"/>
                                </StackPanel>
                            </DataTemplate>
                        </DataGridTemplateColumn.CellTemplate>
                    </DataGridTemplateColumn>
                </DataGrid.Columns>
            </DataGrid>

            <!-- Pagination -->
            <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Center" Margin="10">
                <Button Content="◀ Oldingi"
                        Command="{Binding PreviousPageCommand}"
                        Margin="5"/>

                <TextBlock Text="{Binding CurrentPage, StringFormat='Sahifa {0}'}"
                           VerticalAlignment="Center"
                           Margin="10,0"/>

                <TextBlock Text="{Binding TotalPages, StringFormat='/ {0}'}"
                           VerticalAlignment="Center"
                           Margin="0,0,10,0"/>

                <Button Content="Keyingi ▶"
                        Command="{Binding NextPageCommand}"
                        Margin="5"/>
            </StackPanel>
        </Grid>
    </Grid>
</UserControl>
```

## Bosqich 7: App.xaml.cs - DI Setup

```csharp
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Metra_v2._0.Services.Interfaces;
using Metra_v2._0.Services.Implementation;
using Metra_v2._0.Services.Mijozlar;
using Metra_v2._0.ViewModels.Mijozlar;
using Metra_v2._0.Controls.Mijozlar;
using Metra_v2._0.Windows;
using Notification.Wpf;
using Syncfusion.Licensing;

namespace Metra_v2._0
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Syncfusion license
            SyncfusionLicenseProvider.RegisterLicense("YOUR_LICENSE_KEY");

            // Serilog setup
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/metra-.txt", rollingInterval: Serilog.RollingInterval.Day)
                .CreateLogger();

            // DI Container
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Logging
            var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddSerilog();

            base.OnStartup(e);

            // Show login window
            var loginWindow = ServiceProvider.GetRequiredService<LogIn>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Logging
            services.AddLogging(configure => configure.AddSerilog());

            // Notification Manager - Singleton
            services.AddSingleton<NotificationManager>();

            // HttpClient with factory
            services.AddHttpClient<IMijozService, MijozService>(client =>
            {
                client.BaseAddress = new Uri(ApiConfig.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Other HTTP clients
            services.AddHttpClient<IFilialService, FilialService>(client =>
            {
                client.BaseAddress = new Uri(ApiConfig.BaseUrl);
            });

            services.AddHttpClient<IShartnomService, ShartnomaService>(client =>
            {
                client.BaseAddress = new Uri(ApiConfig.BaseUrl);
            });

            // Services
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IAuthService, AuthService>();

            // ViewModels - Transient (har safar yangi instance)
            services.AddTransient<MijozListViewModel>();
            services.AddTransient<MijozAddViewModel>();
            // ... other ViewModels

            // Views/Pages - Transient
            services.AddTransient<MijozPage>();
            // ... other Pages

            // Windows - Transient
            services.AddTransient<LogIn>();
            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Token o'chirish
            var tokenService = ServiceProvider.GetRequiredService<ITokenService>();
            tokenService.ClearToken();

            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
```

## Bosqich 8: Migratsiya Strategiyasi

### Variantlar:

**Variant 1: Big Bang (Tavsiya etilmaydi)**
- Hammasi birdan qayta yoziladi
- Xavfli, uzoq davom etadi

**Variant 2: Incremental (Tavsiya etiladi)**
1. Yangi feature'larni MVVM bilan yozing
2. Har haftada 1-2 ta eski page'ni refactor qiling
3. Priority bo'yicha:
   - Eng ko'p ishlatiladi → birinchi
   - Eng ko'p bug bor → birinchi
   - Eng oddiy → birinchi (tajriba uchun)

### Priority ro'yxat (Tavsiya):
1. ✅ MijozPage (oddiy CRUD)
2. ✅ FilialPage (oddiy CRUD)
3. ✅ MaterialPage
4. ✅ ShartnomPage (murakkab)
5. Qolganlar...

## Bosqich 9: Testing

```csharp
// Tests/ViewModels/MijozListViewModelTests.cs
using Xunit;
using Moq;
using Metra_v2._0.ViewModels.Mijozlar;
using Metra_v2._0.Services.Interfaces;

public class MijozListViewModelTests
{
    [Fact]
    public async Task LoadMijozlar_Success_FillsCollection()
    {
        // Arrange
        var mockService = new Mock<IMijozService>();
        mockService.Setup(s => s.SelectAll(It.IsAny<int>(), It.IsAny<string>(), null))
            .ReturnsAsync(new PaginatedResult<MijozView>
            {
                Data = new List<MijozView> { new MijozView { Id = 1, Name = "Test" } }
            });

        var viewModel = new MijozListViewModel(
            mockService.Object,
            Mock.Of<INavigationService>(),
            Mock.Of<ILogger<MijozListViewModel>>(),
            Mock.Of<NotificationManager>());

        // Act
        await viewModel.LoadCommand.Execute(null);

        // Assert
        Assert.Single(viewModel.Mijozlar);
        Assert.Equal("Test", viewModel.Mijozlar[0].Name);
    }
}
```

## Qo'shimcha Resources

- [Microsoft MVVM docs](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [WPF MVVM Pattern](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)
