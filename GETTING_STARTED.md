# Metra v3.0 - Getting Started

Loyiha muvaffaqiyatli yaratildi! 🎉

## Proyekt Strukturasi

```
Metra.v3/
├── src/
│   ├── Metra.Desktop/           # WPF UI Layer
│   ├── Metra.Application/       # Business Logic Layer
│   ├── Metra.Domain/            # Domain Entities and Rules
│   └── Metra.Infrastructure/    # External Services (API, Logging, etc.)
├── CLAUDE.md                    # Claude Code development guide
├── METRA_V3_README.md          # Full project documentation
└── Metra.v3.sln                # Solution file
```

## Loyihani Ishga Tushirish

### 1. Talablar
- .NET 8.0 SDK
- Visual Studio 2022 (17.8+) yoki VS Code
- Windows 10/11

### 2. Build qilish

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run application
dotnet run --project src/Metra.Desktop/Metra.Desktop.csproj
```

### 3. Visual Studio'da Ochish

1. `Metra.v3.sln` faylini oching
2. F5 bosing yoki "Debug > Start Debugging"

## Yaratilgan Komponentlar

### ✅ Clean Architecture Layers
- **Metra.Desktop** - WPF presentation layer
- **Metra.Application** - Application services and interfaces
- **Metra.Domain** - Domain entities, enums, exceptions
- **Metra.Infrastructure** - API clients, logging, persistence

### ✅ Base Classes
- `ViewModelBase` - Barcha ViewModellar uchun bazaviy klass
- `BoolToVisibilityConverter` - XAML converter
- `DateTimeConverter` - DateTime formatting converter
- Domain Exceptions (NotFoundException, ValidationException, etc.)

### ✅ Service Interfaces
- `ITokenService` - Token boshqaruvi
- `IAuthService` - Autentifikatsiya
- `INavigationService` - Navigation
- `IDialogService` - Dialog windows

### ✅ Infrastructure
- Serilog logging (file + console)
- AppSettings (JSON-based settings storage)
- API Configuration
- Dependency Injection setup

### ✅ Main Window
- Material Design UI
- Navigation menu with all modules:
  - Mijozlar (Customers)
  - Shartnomalar (Contracts)
  - Materiallar (Materials)
  - Kassa (Cashier)
  - Fakturalar (Invoices)
  - Hisobotlar (Reports)
  - Ombor (Warehouse)
  - Filiallar (Branches)
  - Xodimlar (Employees)

## Keyingi Qadamlar

### 1. Syncfusion License (Optional)
Agar Syncfusion komponentlardan foydalanmoqchi bo'lsangiz, `App.xaml.cs` faylidagi 25-qatorni uncomment qiling va license key kiriting:

```csharp
SyncfusionLicenseProvider.RegisterLicense("YOUR_LICENSE_KEY");
```

### 2. Service Implementatsiyalarini Yaratish
Quyidagi servicelarni implement qilish kerak:
- `AuthService` - Login/Logout funksiyalari
- `NavigationService` - Sahifalar orasida navigatsiya
- `DialogService` - Dialog windowlar

### 3. ViewModellar Yaratish
Har bir modul uchun ViewModel yarating:
- `MijozListViewModel` - Mijozlar ro'yxati
- `MijozEditViewModel` - Mijoz qo'shish/tahrirlash
- Va hokazo...

### 4. API Integration
`Metra.Infrastructure/API/Endpoints/` papkasida API client'larni yarating:
- `MijozApiClient`
- `ShartnomApiClient`
- Va boshqalar...

## Loglar

Ilovaning loglari quyidagi joyda saqlanadi:
```
%APPDATA%/Metra/logs/metra-YYYY-MM-DD.txt
```

## Sozlamalar

Ilova sozlamalari quyidagi joyda saqlanadi:
```
%APPDATA%/Metra/settings.json
```

## Muammolar va Yechimlar

### Build xatoligi
Agar build xatolik bersa:
```bash
dotnet clean
dotnet restore
dotnet build
```

### NuGet paketlari yuklanmasa
```bash
dotnet restore --force
```

## Qo'shimcha Ma'lumot

- **Architecture**: `METRA_V3_README.md` fayliga qarang
- **Development Guide**: `CLAUDE.md` faylini o'qing
- **Refactoring Guide**: `REFACTORING_GUIDE.md` faylida old versiyalardan migrate qilish yo'riqnomasi

## Stack
- .NET 8.0
- WPF (Windows Presentation Foundation)
- Material Design In XAML
- Syncfusion WPF Components
- CommunityToolkit.Mvvm
- Serilog
- Dependency Injection
- Clean Architecture

---

**Omad tilaymiz! 🚀**
