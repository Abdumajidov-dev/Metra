# ViewModel Tushuntirish - Filial Misoli

## 📚 Mundarija
1. [MVVM Pattern nima?](#mvvm-pattern-nima)
2. [ViewModelBase - Asosiy klass](#viewmodelbase---asosiy-klass)
3. [FilialViewModel - Amaliy misol](#filialviewmodel---amaliy-misol)
4. [Property'lar va Binding](#propertylar-va-binding)
5. [Command'lar va Event'lar](#commandlar-va-eventlar)
6. [View bilan bog'lanish](#view-bilan-boglanish)
7. [To'liq ishlash jarayoni](#toliq-ishlash-jarayoni)

---

## MVVM Pattern nima?

**MVVM** = **M**odel - **V**iew - **V**iew**M**odel

```
┌─────────────────────────────────────────────────────────┐
│                    WPF MVVM PATTERN                      │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  ┌──────────┐         ┌──────────────┐      ┌────────┐ │
│  │  MODEL   │◄────────│  VIEWMODEL   │◄─────│  VIEW  │ │
│  │          │         │              │      │        │ │
│  │ Entities │         │ FilialVM     │      │ XAML   │ │
│  │ Filial   │         │ Properties   │      │ Page   │ │
│  │ Mijoz    │         │ Commands     │      │ UI     │ │
│  └──────────┘         │ Logic        │      └────────┘ │
│                       └──────────────┘                  │
│       ▲                      ▲                           │
│       │                      │                           │
│       │         ┌────────────┴──────────┐               │
│       │         │                       │               │
│       │    ┌────▼──────┐         ┌─────▼─────┐         │
│       │    │  SERVICE  │         │  BINDING  │         │
│       └────│  API Call │         │ {Binding} │         │
│            └───────────┘         └───────────┘         │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

### Qismlar:

1. **Model** - Ma'lumotlar (Filial, Mijoz entities)
2. **View** - UI (XAML fayllar)
3. **ViewModel** - View va Model o'rtasidagi ko'prik

---

## ViewModelBase - Asosiy klass

**Location**: `src/Metra.Desktop/ViewModels/Base/ViewModelBase.cs`

```csharp
public abstract class ViewModelBase : ObservableObject
{
    private bool _isBusy;
    private string _title = string.Empty;

    // IsBusy - yuklanayotganini ko'rsatish uchun
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            SetProperty(ref _isBusy, value);
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    // IsNotBusy - IsBusy'ning teskarisi
    public bool IsNotBusy => !IsBusy;

    // Title - sahifa nomi
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    // Async initialization
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    // Cleanup
    public virtual void Cleanup()
    {
        // Override this for cleanup
    }
}
```

### ViewModelBase nimalar beradi?

#### 1. **ObservableObject** (CommunityToolkit.Mvvm'dan)
   - `SetProperty()` metodi
   - `OnPropertyChanged()` metodi
   - `INotifyPropertyChanged` interfeysi

#### 2. **IsBusy / IsNotBusy**
   ```csharp
   // Ishlatish:
   IsBusy = true;  // Loading animatsiyasini ko'rsatish
   // ... API chaqiruv ...
   IsBusy = false; // Yashirish
   ```

   XAML'da:
   ```xml
   <ProgressBar IsIndeterminate="True"
                Visibility="{Binding IsBusy, Converter={StaticResource BoolToVisibilityConverter}}"/>
   ```

#### 3. **Title** - Sahifa nomi
   ```csharp
   Title = "Filiallar"; // Sahifa sarlavhasi
   ```

#### 4. **InitializeAsync()** - Ma'lumotlarni yuklash
   ```csharp
   public override async Task InitializeAsync()
   {
       await LoadFilialsAsync(); // Filiallarni yuklash
   }
   ```

---

## FilialViewModel - Amaliy misol

**Location**: `src/Metra.Desktop/ViewModels/FilialViewModel.cs`

### ViewModel tuzilishi:

```csharp
public class FilialViewModel : ViewModelBase
{
    // 1. DEPENDENCIES (Constructor orqali kiritiladi)
    private readonly IFilialService _filialService;
    private readonly ILogger<FilialViewModel> _logger;
    private readonly NotificationManager _notifier;

    // 2. PRIVATE FIELDS (backing fields)
    private ObservableCollection<FilialResponse> _filials = new();
    private FilialResponse? _selectedFilial;
    private string _searchText = string.Empty;
    private bool _isAddEditDialogOpen;

    // 3. PUBLIC PROPERTIES (View bilan bog'lanadi)
    public ObservableCollection<FilialResponse> Filials { get; set; }
    public FilialResponse? SelectedFilial { get; set; }
    public string SearchText { get; set; }
    public bool IsAddEditDialogOpen { get; set; }

    // 4. COMMANDS (Button'lar uchun)
    [RelayCommand]
    private async Task LoadFilialsAsync() { }

    [RelayCommand]
    private void OpenAddDialog() { }

    // 5. HELPER METHODS
    private bool HasChanges() { }
}
```

---

## Property'lar va Binding

### Property nima?

Property - bu View'ga (XAML) ulangan o'zgaruvchi.

### Oddiy property vs ViewModel property

#### ❌ Oddiy C# property (WPF'da ishlamaydi):
```csharp
public string Name { get; set; } // UI yangilanmaydi!
```

#### ✅ ViewModel property (WPF uchun to'g'ri):
```csharp
private string _name = string.Empty;

public string Name
{
    get => _name;
    set => SetProperty(ref _name, value); // UI avtomatik yangilanadi!
}
```

### SetProperty() nima qiladi?

```csharp
SetProperty(ref _name, value);
```

Bu metod:
1. Eski qiymat bilan yangi qiymatni solishtiradi
2. Agar farq bo'lsa, qiymatni o'zgartiradi
3. UI'ga xabar beradi: "Hey, Name o'zgardi, UI'ni yangilang!"
4. XAML'dagi `{Binding Name}` avtomatik yangilanadi

### FilialViewModel'dagi misol:

```csharp
// PRIVATE FIELD (ichki storage)
private ObservableCollection<FilialResponse> _filials = new();

// PUBLIC PROPERTY (XAML'ga ulangan)
public ObservableCollection<FilialResponse> Filials
{
    get => _filials;
    set => SetProperty(ref _filials, value); // UI'ga xabar beradi
}
```

XAML'da ishlatish:
```xml
<DataGrid ItemsSource="{Binding Filials}">
    <!-- Filials o'zgarganda DataGrid avtomatik yangilanadi! -->
</DataGrid>
```

---

## Command'lar va Event'lar

### Command nima?

Command - bu Button'ni bosishda bajariladigan metod.

### Oddiy event handler vs Command

#### ❌ Code-behind'da event handler (yomon):
```csharp
// FilialPage.xaml.cs
private async void LoadButton_Click(object sender, RoutedEventArgs e)
{
    // Business logic bu yerda emas!
}
```

#### ✅ ViewModel'da Command (yaxshi):
```csharp
// FilialViewModel.cs
[RelayCommand]
private async Task LoadFilialsAsync()
{
    // Business logic bu yerda!
}
```

XAML'da:
```xml
<Button Command="{Binding LoadFilialsCommand}" Content="Yuklash"/>
```

### RelayCommand nima?

`[RelayCommand]` - bu attribute, avtomatik ravishda Command yaratadi.

```csharp
// Siz yozasiz:
[RelayCommand]
private async Task LoadFilialsAsync() { }

// Kompilyator yaratadi:
public IAsyncRelayCommand LoadFilialsCommand { get; }
```

### FilialViewModel'dagi Command'lar:

```csharp
// 1. ASYNC COMMAND (API call uchun)
[RelayCommand]
private async Task LoadFilialsAsync()
{
    try
    {
        IsBusy = true; // Loading ko'rsatish
        _logger.LogInformation("Filiallar yuklanmoqda...");

        var filials = await _filialService.GetAllAsync(SearchText);

        if (filials != null)
        {
            Filials = new ObservableCollection<FilialResponse>(filials);
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Xatolik");
        _notifier.Show("Xatolik", "Yuklanmadi", NotificationType.Error);
    }
    finally
    {
        IsBusy = false; // Loading yashirish
    }
}

// 2. SYNC COMMAND (oddiy amallar uchun)
[RelayCommand]
private void OpenAddDialog()
{
    IsEditMode = false;
    DialogTitle = "Yangi filial qo'shish";
    FilialName = string.Empty;
    IsAddEditDialogOpen = true;
}

// 3. CONDITIONAL COMMAND (faqat shart bajarilganda)
[RelayCommand(CanExecute = nameof(CanDelete))]
private async Task DeleteFilialAsync()
{
    // ...
}

private bool CanDelete() => SelectedFilial != null;
```

---

## View bilan bog'lanish

### FilialPage.xaml.cs (Code-behind)

```csharp
public partial class FilialPage : UserControl
{
    private readonly FilialViewModel _viewModel;

    public FilialPage(FilialViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        // ViewModel'ni View'ga bog'lash
        DataContext = viewModel;

        // Sahifa ochilganda ma'lumotlarni yuklash
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }

    // Minimal kod! Faqat UI event'lar
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedFilial != null)
        {
            _viewModel.OpenEditDialogCommand.Execute(null);
        }
    }
}
```

### FilialPage.xaml (XAML)

```xml
<!-- ViewModel'dagi property'larga binding -->
<UserControl DataContext="{FilialViewModel}">

    <!-- Filials property'ga bog'langan -->
    <DataGrid ItemsSource="{Binding Filials}"
              SelectedItem="{Binding SelectedFilial}">
    </DataGrid>

    <!-- SearchText property'ga bog'langan -->
    <TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"/>

    <!-- Command'larga bog'langan -->
    <Button Command="{Binding OpenAddDialogCommand}" Content="Yangi filial"/>
    <Button Command="{Binding RefreshCommand}" Content="Yangilash"/>

    <!-- Dialog ochiq/yopiq property'ga bog'langan -->
    <Border Visibility="{Binding IsAddEditDialogOpen,
                                 Converter={StaticResource BoolToVisibilityConverter}}">
        <!-- Dialog content -->
    </Border>
</UserControl>
```

---

## To'liq ishlash jarayoni

### Misol: Filial qo'shish

#### 1. **Foydalanuvchi "Yangi filial" buttonni bosadi**

XAML:
```xml
<Button Command="{Binding OpenAddDialogCommand}" Content="Yangi filial"/>
```

Bu yerda nima bo'ladi:
- WPF `OpenAddDialogCommand`'ni topadi
- Command'ni execute qiladi

---

#### 2. **ViewModel'da OpenAddDialog metodi ishlaydi**

```csharp
[RelayCommand]
private void OpenAddDialog()
{
    IsEditMode = false;
    DialogTitle = "Yangi filial qo'shish";
    FilialName = string.Empty;
    FilialDescription = string.Empty;
    FilialType = "branch";

    // Dastlabki qiymatlarni saqlash
    _originalFilialName = string.Empty;
    _originalFilialDescription = string.Empty;
    _originalFilialType = "branch";

    IsAddEditDialogOpen = true; // ← Dialog'ni ochadi
}
```

`IsAddEditDialogOpen = true` bo'lganda:
- `SetProperty()` UI'ga xabar beradi
- XAML'dagi `{Binding IsAddEditDialogOpen}` yangilanadi
- `BoolToVisibilityConverter` `true` → `Visible` qiladi
- Dialog ko'rinadi!

---

#### 3. **Foydalanuvchi ma'lumotlarni kiritadi**

```xml
<TextBox Text="{Binding FilialName, UpdateSourceTrigger=PropertyChanged}"/>
<TextBox Text="{Binding FilialDescription, UpdateSourceTrigger=PropertyChanged}"/>
<ComboBox SelectedValue="{Binding FilialType}"/>
```

Har bir TextBox'ga yozilganda:
- `UpdateSourceTrigger=PropertyChanged` tezda yangilaydi
- ViewModel'dagi property avtomatik o'zgaradi

Misol:
```
User yozadi: "Toshkent filiali"
              ↓
XAML Binding: FilialName property'ga yozadi
              ↓
ViewModel:    _filialName = "Toshkent filiali"
              ↓
              SetProperty() chaqiriladi
              ↓
              UI'ga xabar beriladi
```

---

#### 4. **Foydalanuvchi "Saqlash" buttonni bosadi**

```xml
<Button Command="{Binding SaveFilialCommand}">
    <TextBlock Text="Saqlash"/>
</Button>
```

---

#### 5. **ViewModel'da SaveFilialAsync metodi ishlaydi**

```csharp
[RelayCommand]
private async Task SaveFilialAsync()
{
    // 1. VALIDATSIYA
    if (string.IsNullOrWhiteSpace(FilialName))
    {
        _notifier.Show("Ogohlantirish", "Filial nomini kiriting",
                       NotificationType.Warning);
        return;
    }

    // 2. LOADING KO'RSATISH
    IsBusy = true; // ← ProgressBar ko'rinadi

    bool success;

    if (IsEditMode)
    {
        // TAHRIRLASH
        var request = new FilialUpdateRequest
        {
            Name = FilialName.Trim(),
            Description = FilialDescription?.Trim(),
            Type = FilialType
        };

        success = await _filialService.UpdateAsync(_editingFilialId, request);
    }
    else
    {
        // YANGI QO'SHISH
        var request = new FilialCreateRequest
        {
            Name = FilialName.Trim(),
            Description = FilialDescription?.Trim(),
            Type = FilialType
        };

        success = await _filialService.CreateAsync(request);
    }

    // 3. NATIJA
    if (success)
    {
        // Dastlabki qiymatlarni yangilash
        _originalFilialName = FilialName;
        _originalFilialDescription = FilialDescription;
        _originalFilialType = FilialType;

        IsAddEditDialogOpen = false; // Dialog yopiladi
        await LoadFilialsAsync();     // Ro'yxatni yangilash

        _notifier.Show("Muvaffaqiyat", "Saqlandi", NotificationType.Success);
    }
    else
    {
        _notifier.Show("Xatolik", "Saqlanmadi", NotificationType.Error);
    }

    IsBusy = false; // Loading yashiriladi
}
```

---

#### 6. **Dialog yopiladi, ro'yxat yangilanadi**

`IsAddEditDialogOpen = false` bo'lganda:
- UI avtomatik yangilanadi
- Dialog Visibility = Collapsed bo'ladi
- Dialog yopiladi

`await LoadFilialsAsync()` chaqirilganda:
- API'dan filiallar yuklanadi
- `Filials` property yangilanadi
- DataGrid avtomatik yangilanadi
- Yangi filial ro'yxatda ko'rinadi!

---

### Misol: Dialog'ni yopishda ogohlantirish

#### 1. **Foydalanuvchi X buttonni bosadi**

```xml
<Button Command="{Binding CloseDialogCommand}">
    <materialDesign:PackIcon Kind="Close"/>
</Button>
```

---

#### 2. **CloseDialog metodi ishlaydi**

```csharp
[RelayCommand]
private void CloseDialog()
{
    // O'zgarishlar bormi tekshirish
    if (HasChanges())
    {
        var result = System.Windows.MessageBox.Show(
            "Siz o'zgarishlar qildingiz. Haqiqatdan chiqishni xohlaysizmi?",
            "Ogohlantirish",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result != System.Windows.MessageBoxResult.Yes)
            return; // User "No" dedi, chiqmaydi
    }

    IsAddEditDialogOpen = false; // Yopiladi
}
```

---

#### 3. **HasChanges() o'zgarishlarni tekshiradi**

```csharp
private bool HasChanges()
{
    // Filial nomi o'zgardimi?
    if (FilialName?.Trim() != _originalFilialName?.Trim())
        return true;

    // Ta'rif o'zgardimi?
    var currentDescription = FilialDescription?.Trim() ?? string.Empty;
    var originalDescription = _originalFilialDescription?.Trim() ?? string.Empty;
    if (currentDescription != originalDescription)
        return true;

    // Turi o'zgardimi?
    if (FilialType != _originalFilialType)
        return true;

    return false; // Hech narsa o'zgarmagan
}
```

**Ishlash jarayoni:**

```
User dialog ochdi
   ↓
ViewModel: _originalFilialName = "Toshkent"
   ↓
User o'zgartirdi: FilialName = "Toshkent filiali"
   ↓
User "X" bosdi
   ↓
CloseDialog() chaqirildi
   ↓
HasChanges() tekshirdi:
   - FilialName ("Toshkent filialdi") != _originalFilialName ("Toshkent")
   - return true
   ↓
MessageBox ko'rsatildi: "Siz o'zgarishlar qildingiz..."
   ↓
User "Yes" → dialog yopiladi
User "No"  → dialog ochiq qoladi
```

---

## Dependency Injection (DI)

### ViewModel qanday yaratiladi?

#### App.xaml.cs'da registration:

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // 1. Services (Singleton)
    services.AddSingleton<NotificationManager>();
    services.AddSingleton<AppSettings>();
    services.AddHttpClient<IFilialService, FilialService>();

    // 2. ViewModels (Transient - har safar yangi instance)
    services.AddTransient<FilialViewModel>();

    // 3. Views (Transient)
    services.AddTransient<FilialPage>();
}
```

---

#### FilialPage yaratilganda:

```csharp
public FilialPage(FilialViewModel viewModel)
{
    // DI container FilialViewModel'ni yaratadi:
    //   1. IFilialService ni topadi
    //   2. ILogger ni topadi
    //   3. NotificationManager ni topadi
    //   4. Constructor'ga inject qiladi
    //   5. FilialViewModel instance yaratadi

    InitializeComponent();
    DataContext = viewModel;
}
```

---

#### FilialViewModel constructor:

```csharp
public FilialViewModel(
    IFilialService filialService,      // ← DI inject qiladi
    ILogger<FilialViewModel> logger,    // ← DI inject qiladi
    NotificationManager notifier)       // ← DI inject qiladi
{
    _filialService = filialService;
    _logger = logger;
    _notifier = notifier;

    Title = "Filiallar";
}
```

---

## Xulosa

### ViewModel'ning asosiy vazifasi:

1. ✅ **View va Model o'rtasidagi ko'prik**
2. ✅ **Business logic joylashgan joy**
3. ✅ **Property'lar orqali UI bilan bog'lanish**
4. ✅ **Command'lar orqali action'larni boshqarish**
5. ✅ **INotifyPropertyChanged orqali UI'ni avtomatik yangilash**

---

### ViewModel vs Code-behind

| Xususiyat | Code-behind (yomon) | ViewModel (yaxshi) |
|-----------|---------------------|---------------------|
| Business logic | XAML.cs faylda | ViewModel'da |
| Testlash | Qiyin | Oson |
| Kod qayta ishlatish | Yo'q | Ha |
| Separation of Concerns | Yo'q | Ha |
| MVVM pattern | Buziladi | To'g'ri ishlaydi |

---

### FilialViewModel tuzilishi (umumiy ko'rinish):

```csharp
public class FilialViewModel : ViewModelBase
{
    // ============================================
    // 1. DEPENDENCIES
    // ============================================
    private readonly IFilialService _filialService;
    private readonly ILogger<FilialViewModel> _logger;
    private readonly NotificationManager _notifier;

    // ============================================
    // 2. PRIVATE FIELDS (backing fields)
    // ============================================
    private ObservableCollection<FilialResponse> _filials = new();
    private FilialResponse? _selectedFilial;
    private string _searchText = string.Empty;
    private bool _isAddEditDialogOpen;
    private bool _isEditMode;
    private bool _isContextMenuOpen;

    // Dialog fields
    private string _filialName = string.Empty;
    private string _filialDescription = string.Empty;
    private string _filialType = "branch";

    // Original values (for change detection)
    private string _originalFilialName = string.Empty;
    private string _originalFilialDescription = string.Empty;
    private string _originalFilialType = "branch";

    // ============================================
    // 3. CONSTRUCTOR (DI)
    // ============================================
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

    // ============================================
    // 4. PUBLIC PROPERTIES (View'ga bog'langan)
    // ============================================
    public ObservableCollection<FilialResponse> Filials
    {
        get => _filials;
        set => SetProperty(ref _filials, value);
    }

    public FilialResponse? SelectedFilial
    {
        get => _selectedFilial;
        set => SetProperty(ref _selectedFilial, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                // Debounce search
                _searchDebounceTimer?.Dispose();
                _searchDebounceTimer = new Timer(
                    async _ => await SearchWithDebounceAsync(),
                    null, 500, Timeout.Infinite);
            }
        }
    }

    public bool IsAddEditDialogOpen
    {
        get => _isAddEditDialogOpen;
        set => SetProperty(ref _isAddEditDialogOpen, value);
    }

    public bool IsContextMenuOpen
    {
        get => _isContextMenuOpen;
        set => SetProperty(ref _isContextMenuOpen, value);
    }

    public string FilialName
    {
        get => _filialName;
        set => SetProperty(ref _filialName, value);
    }

    public string FilialDescription
    {
        get => _filialDescription;
        set => SetProperty(ref _filialDescription, value);
    }

    public string FilialType
    {
        get => _filialType;
        set => SetProperty(ref _filialType, value);
    }

    // ============================================
    // 5. COMMANDS
    // ============================================

    /// <summary>
    /// Filiallarni yuklash
    /// </summary>
    [RelayCommand]
    private async Task LoadFilialsAsync()
    {
        try
        {
            IsBusy = true;
            var filials = await _filialService.GetAllAsync(SearchText);

            if (filials != null)
            {
                for (int i = 0; i < filials.Count; i++)
                    filials[i].Number = i + 1;

                Filials = new ObservableCollection<FilialResponse>(filials);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Xatolik");
            _notifier.Show("Xatolik", "Yuklanmadi", NotificationType.Error);
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

        _originalFilialName = string.Empty;
        _originalFilialDescription = string.Empty;
        _originalFilialType = "branch";

        IsAddEditDialogOpen = true;
    }

    /// <summary>
    /// Tahrirlash dialogini ochish
    /// </summary>
    [RelayCommand]
    private void OpenEditDialog()
    {
        if (SelectedFilial == null) return;

        IsEditMode = true;
        DialogTitle = "Filialni tahrirlash";
        _editingFilialId = SelectedFilial.Id;
        FilialName = SelectedFilial.Name;
        FilialDescription = SelectedFilial.Description ?? string.Empty;
        FilialType = SelectedFilial.Type;

        _originalFilialName = SelectedFilial.Name;
        _originalFilialDescription = SelectedFilial.Description ?? string.Empty;
        _originalFilialType = SelectedFilial.Type;

        IsAddEditDialogOpen = true;
    }

    /// <summary>
    /// Dialogni yopish (ogohlantirish bilan)
    /// </summary>
    [RelayCommand]
    private void CloseDialog()
    {
        if (HasChanges())
        {
            var result = MessageBox.Show(
                "Siz o'zgarishlar qildingiz. Haqiqatdan chiqishni xohlaysizmi?",
                "Ogohlantirish",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;
        }

        IsAddEditDialogOpen = false;
    }

    /// <summary>
    /// Saqlash
    /// </summary>
    [RelayCommand]
    private async Task SaveFilialAsync()
    {
        if (string.IsNullOrWhiteSpace(FilialName))
        {
            _notifier.Show("Ogohlantirish", "Nom kiriting", NotificationType.Warning);
            return;
        }

        IsBusy = true;
        bool success;

        if (IsEditMode)
        {
            var request = new FilialUpdateRequest
            {
                Name = FilialName.Trim(),
                Description = FilialDescription?.Trim(),
                Type = FilialType
            };
            success = await _filialService.UpdateAsync(_editingFilialId, request);
        }
        else
        {
            var request = new FilialCreateRequest
            {
                Name = FilialName.Trim(),
                Description = FilialDescription?.Trim(),
                Type = FilialType
            };
            success = await _filialService.CreateAsync(request);
        }

        if (success)
        {
            _originalFilialName = FilialName;
            _originalFilialDescription = FilialDescription;
            _originalFilialType = FilialType;

            IsAddEditDialogOpen = false;
            await LoadFilialsAsync();
            _notifier.Show("Muvaffaqiyat", "Saqlandi", NotificationType.Success);
        }

        IsBusy = false;
    }

    /// <summary>
    /// O'chirish
    /// </summary>
    [RelayCommand]
    private async Task DeleteFilialAsync()
    {
        if (SelectedFilial == null) return;

        IsBusy = true;
        IsContextMenuOpen = false;

        var success = await _filialService.DeleteAsync(SelectedFilial.Id);

        if (success)
        {
            await LoadFilialsAsync();
            _notifier.Show("Muvaffaqiyat", "O'chirildi", NotificationType.Success);
        }

        IsBusy = false;
    }

    /// <summary>
    /// Context menu'ni yopish
    /// </summary>
    [RelayCommand]
    private void CloseContextMenu()
    {
        IsContextMenuOpen = false;
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

    // ============================================
    // 6. HELPER METHODS
    // ============================================

    private async Task SearchWithDebounceAsync()
    {
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            await LoadFilialsAsync();
        });
    }

    private bool HasChanges()
    {
        if (FilialName?.Trim() != _originalFilialName?.Trim())
            return true;

        var current = FilialDescription?.Trim() ?? string.Empty;
        var original = _originalFilialDescription?.Trim() ?? string.Empty;
        if (current != original)
            return true;

        if (FilialType != _originalFilialType)
            return true;

        return false;
    }

    // ============================================
    // 7. LIFECYCLE METHODS
    // ============================================

    public override async Task InitializeAsync()
    {
        await LoadFilialsAsync();
    }

    public override void Cleanup()
    {
        _searchDebounceTimer?.Dispose();
        base.Cleanup();
    }
}
```

---

## Asosiy tushunchalar yana bir bor:

### 1. **Property** = XAML'ga ulangan o'zgaruvchi
```csharp
public string FilialName
{
    get => _filialName;
    set => SetProperty(ref _filialName, value); // UI'ga xabar beradi!
}
```

### 2. **Command** = Button'ning action'i
```csharp
[RelayCommand]
private async Task SaveFilialAsync() { }
// Yaratiladi: SaveFilialCommand
```

### 3. **Binding** = XAML va ViewModel o'rtasidagi bog'lanish
```xml
<TextBox Text="{Binding FilialName}"/>
<!-- FilialName property'ga bog'langan -->
```

### 4. **IsBusy** = Loading state
```csharp
IsBusy = true;  // Loading ko'rsatish
await DoWorkAsync();
IsBusy = false; // Loading yashirish
```

### 5. **ObservableCollection** = UI'ni avtomatik yangilaydigan ro'yxat
```csharp
Filials = new ObservableCollection<FilialResponse>(filials);
// DataGrid avtomatik yangilanadi!
```

---

## Savol-javoblar

### ❓ Nega SetProperty() kerak?

**Javob:** UI'ga xabar berish uchun.

```csharp
// ❌ Bu ishlamaydi (UI yangilanmaydi):
_filialName = "Yangi nom";

// ✅ Bu ishlaydi (UI avtomatik yangilanadi):
SetProperty(ref _filialName, "Yangi nom");
```

### ❓ Nega [RelayCommand] attribute kerak?

**Javob:** Avtomatik Command yaratish uchun.

```csharp
// Siz yozasiz:
[RelayCommand]
private async Task SaveAsync() { }

// Kompilyator yaratadi:
public IAsyncRelayCommand SaveCommand { get; }
```

### ❓ Nega private field + public property kerak?

**Javob:** SetProperty() faqat reference (ref) bilan ishlaydi.

```csharp
private string _name; // ← backing field

public string Name // ← public property
{
    get => _name;
    set => SetProperty(ref _name, value); // ref kerak!
}
```

### ❓ DataContext nima?

**Javob:** View'ning ViewModel'ga reference'i.

```csharp
// FilialPage.xaml.cs
DataContext = viewModel;

// Endi XAML'da {Binding} ishlaydi:
// {Binding FilialName} → viewModel.FilialName
```

### ❓ UpdateSourceTrigger=PropertyChanged nima?

**Javob:** Har bir belgi yozilganda yangilash.

```xml
<!-- Har bir belgi yozilganda yangilanadi -->
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"/>

<!-- Faqat focus yo'qolganda yangilanadi (default) -->
<TextBox Text="{Binding SearchText}"/>
```

---

## Yakuniy misol: To'liq oqim

```
┌─────────────────────────────────────────────────────────────┐
│                  USER "Yangi filial" BOSADI                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  XAML: <Button Command="{Binding OpenAddDialogCommand}"/>   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  WPF Binding System: OpenAddDialogCommand'ni topadi         │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  ViewModel: OpenAddDialog() metodi chaqiriladi              │
│                                                              │
│  private void OpenAddDialog()                               │
│  {                                                           │
│      IsAddEditDialogOpen = true; // ← Bu qator!             │
│  }                                                           │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  SetProperty() chaqiriladi:                                 │
│  - Eski qiymat: false                                        │
│  - Yangi qiymat: true                                        │
│  - O'zgarish bor!                                            │
│  - OnPropertyChanged("IsAddEditDialogOpen") chaqiriladi     │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  INotifyPropertyChanged event:                              │
│  "Hey XAML, IsAddEditDialogOpen o'zgardi!"                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  XAML Binding yangilanadi:                                  │
│  {Binding IsAddEditDialogOpen} → true                       │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  BoolToVisibilityConverter ishlaydi:                        │
│  true → Visibility.Visible                                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  UI yangilanadi: Dialog ko'rinadi! ✅                        │
└─────────────────────────────────────────────────────────────┘
```

---

**Mana shu - ViewModel'ning butun sirli!** 🎉

Ushbu faylni o'qib, ViewModel qanday ishlashini tushunib olishingiz mumkin.
