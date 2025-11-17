# Metra v3.0 - Rental Management System

## 📋 Project Overview

Metra v3.0 - bu jihozlar ijarasi (rent) va shartnomalarni boshqarish uchun zamonaviy WPF desktop ilovasi. Bu loyiha Metra v2.0 ning to'liq qayta yozilgan versiyasi bo'lib, professional arxitektura standartlari bilan qurilgan.

### Asosiy Maqsadlar
- ✅ Clean Architecture va MVVM pattern
- ✅ Dependency Injection (DI)
- ✅ Testable kod (Unit tests)
- ✅ Maintainable va scalable arxitektura
- ✅ Modern UI/UX (Material Design + Syncfusion)
- ✅ Proper error handling va logging
- ✅ Secure token management
- ✅ Offline-first capabilities (future)

## 🛠 Technology Stack

### Core
- **.NET 8.0** (net8.0-windows)
- **WPF (Windows Presentation Foundation)**
- **C# 12** with nullable reference types

### UI Framework
- **MaterialDesignThemes** 5.1.0 - Material Design components
- **Syncfusion WPF** 27.1.55 - Advanced UI controls
- **Notification.Wpf** - Toast notifications

### Architecture & Patterns
- **CommunityToolkit.Mvvm** - MVVM helpers (ObservableObject, RelayCommand)
- **Microsoft.Extensions.DependencyInjection** - IoC Container
- **Microsoft.Extensions.Http** - HttpClientFactory

### Logging & Diagnostics
- **Serilog** - Structured logging
- **Serilog.Sinks.File** - File logging
- **Serilog.Sinks.Console** - Console logging (debug)

### Data & API
- **Newtonsoft.Json** - JSON serialization
- **System.Net.Http** - HTTP client

### Utilities
- **FreeSpire.PDF** - PDF generation
- **QRCoder** - QR code generation
- **Magick.NET** - Image processing (agar kerak bo'lsa)

### Testing (Future)
- **xUnit** - Unit testing framework
- **Moq** - Mocking library
- **FluentAssertions** - Assertion library

## 🏗 Architecture

### Clean Architecture Layers

```
Metra.Desktop (Presentation Layer)
├── Views/              # XAML files
├── ViewModels/         # ViewModels with business logic
└── Converters/         # Value converters

Metra.Application (Application Layer)
├── Services/           # Business services
├── Interfaces/         # Service interfaces
├── DTOs/              # Data Transfer Objects
└── Validators/        # Input validation

Metra.Domain (Domain Layer)
├── Entities/          # Domain models
├── Enums/            # Enumerations
└── Exceptions/       # Custom exceptions

Metra.Infrastructure (Infrastructure Layer)
├── API/              # HTTP API clients
├── Persistence/      # Local storage (settings, cache)
└── Logging/          # Logging configuration
```

### MVVM Pattern

```
View (XAML)
  ↕ DataBinding
ViewModel (Logic + State)
  ↕ Commands/Methods
Services (Business Logic)
  ↕ HTTP/API
API Backend
```

### Dependency Injection

Barcha dependency'lar constructor orqali inject qilinadi:

```csharp
public class MijozViewModel
{
    private readonly IMijozService _mijozService;
    private readonly INavigationService _navigationService;

    public MijozViewModel(
        IMijozService mijozService,
        INavigationService navigationService)
    {
        _mijozService = mijozService;
        _navigationService = navigationService;
    }
}
```

## 📁 Project Structure

```
Metra.v3/
├── src/
│   ├── Metra.Desktop/                    # WPF Presentation Layer
│   │   ├── App.xaml / App.xaml.cs       # Application entry point
│   │   ├── Views/                        # XAML Views
│   │   │   ├── MainWindow.xaml
│   │   │   ├── Auth/
│   │   │   │   └── LoginView.xaml
│   │   │   ├── Mijozlar/
│   │   │   │   ├── MijozListView.xaml
│   │   │   │   └── MijozEditView.xaml
│   │   │   ├── Shartnoma/
│   │   │   │   ├── ShartnomListView.xaml
│   │   │   │   └── ShartnomEditView.xaml
│   │   │   ├── Materiallar/
│   │   │   ├── Faktura/
│   │   │   ├── Filiallar/
│   │   │   ├── Hisobotlar/
│   │   │   ├── Kassa/
│   │   │   ├── Ombor/
│   │   │   └── Xodimlar/
│   │   ├── ViewModels/                   # ViewModels
│   │   │   ├── Base/
│   │   │   │   └── ViewModelBase.cs
│   │   │   ├── MainWindowViewModel.cs
│   │   │   ├── Auth/
│   │   │   │   └── LoginViewModel.cs
│   │   │   ├── Mijozlar/
│   │   │   │   ├── MijozListViewModel.cs
│   │   │   │   └── MijozEditViewModel.cs
│   │   │   └── ... (other modules)
│   │   ├── Converters/                   # Value Converters
│   │   │   ├── BoolToVisibilityConverter.cs
│   │   │   └── DateTimeConverter.cs
│   │   ├── Controls/                     # Reusable UserControls
│   │   │   ├── LoadingOverlay.xaml
│   │   │   └── PaginationControl.xaml
│   │   ├── Themes/                       # Resource Dictionaries
│   │   │   ├── Colors.xaml
│   │   │   ├── Styles.xaml
│   │   │   └── Generic.xaml
│   │   └── Assets/                       # Images, Icons
│   │       └── Icons/
│   │
│   ├── Metra.Application/                # Application Layer
│   │   ├── Services/                     # Business Services
│   │   │   ├── Interfaces/
│   │   │   │   ├── IMijozService.cs
│   │   │   │   ├── IShartnomService.cs
│   │   │   │   ├── IAuthService.cs
│   │   │   │   ├── INavigationService.cs
│   │   │   │   ├── IDialogService.cs
│   │   │   │   └── ITokenService.cs
│   │   │   └── Implementation/
│   │   │       ├── MijozService.cs
│   │   │       ├── ShartnomService.cs
│   │   │       ├── NavigationService.cs
│   │   │       └── DialogService.cs
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   │   ├── Requests/
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   └── CreateMijozRequest.cs
│   │   │   └── Responses/
│   │   │       ├── LoginResponse.cs
│   │   │       ├── MijozResponse.cs
│   │   │       └── PaginatedResponse.cs
│   │   ├── Validators/                   # FluentValidation validators
│   │   │   ├── MijozValidator.cs
│   │   │   └── ShartnomValidator.cs
│   │   └── Mappings/                     # AutoMapper profiles (optional)
│   │
│   ├── Metra.Domain/                     # Domain Layer
│   │   ├── Entities/                     # Domain Models
│   │   │   ├── Mijoz.cs
│   │   │   ├── Shartnoma.cs
│   │   │   ├── Material.cs
│   │   │   ├── Faktura.cs
│   │   │   ├── Filial.cs
│   │   │   └── Xodim.cs
│   │   ├── Enums/                        # Enumerations
│   │   │   ├── ShartnomStatus.cs
│   │   │   ├── TolovTuri.cs
│   │   │   └── UserRole.cs
│   │   └── Exceptions/                   # Domain Exceptions
│   │       ├── NotFoundException.cs
│   │       ├── ValidationException.cs
│   │       └── UnauthorizedException.cs
│   │
│   └── Metra.Infrastructure/             # Infrastructure Layer
│       ├── API/                          # API Clients
│       │   ├── ApiClient.cs             # Base HTTP client
│       │   ├── ApiConfig.cs             # API configuration
│       │   ├── Endpoints/               # API endpoints
│       │   │   ├── MijozApiClient.cs
│       │   │   ├── ShartnomApiClient.cs
│       │   │   └── AuthApiClient.cs
│       │   └── Interceptors/            # HTTP interceptors
│       │       └── AuthInterceptor.cs
│       ├── Persistence/                  # Local Storage
│       │   ├── Settings/
│       │   │   └── AppSettings.cs
│       │   └── Cache/
│       │       └── MemoryCache.cs
│       └── Logging/                      # Logging Setup
│           └── LoggingConfiguration.cs
│
├── tests/
│   ├── Metra.Desktop.Tests/             # UI Tests
│   ├── Metra.Application.Tests/         # Service Tests
│   └── Metra.Domain.Tests/              # Domain Tests
│
├── docs/                                 # Documentation
│   ├── Architecture.md
│   ├── API.md
│   └── Development.md
│
├── .gitignore
├── README.md                             # This file
├── CLAUDE.md                             # Claude Code instructions
├── Metra.v3.sln                         # Solution file
└── Directory.Build.props                 # Common MSBuild properties
```

## 🎯 Features (Modules)

### 1. Authentication (Auth)
- Login / Logout
- Token management (JWT)
- User session management
- Permission-based access control

### 2. Mijozlar (Customers/Clients)
- CRUD operations
- Search & filter
- Pagination
- Client history
- Contact information management

### 3. Shartnoma (Contracts)
- Create new contract
- Edit/Update contract
- Cancel contract
- Contract status tracking
- Payment schedule
- Contract documents (PDF generation)

### 4. Materiallar (Materials/Equipment)
- Material catalog
- Material categories
- Price management
- Availability tracking
- Material history

### 5. Faktura (Invoices)
- Invoice generation
- Invoice printing
- Payment tracking
- QR code integration
- Invoice history

### 6. Filiallar (Branches)
- Branch management
- Branch-specific operations
- Branch reports

### 7. Xodimlar (Employees)
- Employee management
- Role and permission management
- Employee activity tracking

### 8. Ombor (Warehouse)
- Material receiving
- Material dispatch
- Stock management
- Transfer between branches
- Supplier payments

### 9. Kassa (Cashier)
- Cash income
- Cash expenses
- Expense categories
- Cash flow reports

### 10. Hisobotlar (Reports)
- Material movement reports
- Cash flow reports
- Financial reports
- Contract reports
- Custom date range filtering

## ⚙️ Configuration

### appsettings.json (Future implementation)
```json
{
  "ApiSettings": {
    "BaseUrl": "http://app.metra-rent.uz/api",
    "ImageBaseUrl": "http://app.metra-rent.uz/api/public/storage/",
    "Timeout": 30
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    },
    "File": {
      "Path": "logs/metra-.txt",
      "RollingInterval": "Day"
    }
  },
  "Syncfusion": {
    "LicenseKey": "YOUR_LICENSE_KEY"
  }
}
```

### Environment-specific configs
- Development: `appsettings.Development.json`
- Production: `appsettings.Production.json`

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022 (17.8+)
- .NET 8.0 SDK
- Windows 10/11

### Installation

```bash
# 1. Clone repository
git clone <repository-url>
cd Metra.v3

# 2. Restore NuGet packages
dotnet restore

# 3. Build solution
dotnet build

# 4. Run application
dotnet run --project src/Metra.Desktop/Metra.Desktop.csproj
```

### First-time Setup

1. Configure API endpoints in `ApiConfig.cs`
2. Set Syncfusion license key in `App.xaml.cs`
3. Run the application
4. Login with credentials

## 📝 Coding Standards

### Naming Conventions

```csharp
// Classes - PascalCase
public class MijozService { }

// Interfaces - I + PascalCase
public interface IMijozService { }

// Private fields - _camelCase
private readonly IMijozService _mijozService;

// Properties - PascalCase
public string Name { get; set; }

// Methods - PascalCase
public async Task LoadDataAsync() { }

// Parameters - camelCase
public void AddMijoz(string name, string phone) { }

// Constants - PascalCase
private const string ApiVersion = "v1";

// Enums - PascalCase
public enum ShartnomStatus { Active, Cancelled, Completed }
```

### File Organization

```csharp
// 1. Using statements (grouped)
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Metra.Application.Services.Interfaces;
using Metra.Domain.Entities;

// 2. Namespace
namespace Metra.Desktop.ViewModels.Mijozlar;

// 3. Class with XML comments
/// <summary>
/// ViewModel for managing customer list
/// </summary>
public class MijozListViewModel : ViewModelBase
{
    // 4. Private fields
    private readonly IMijozService _mijozService;
    private readonly ILogger<MijozListViewModel> _logger;

    // 5. Constructor
    public MijozListViewModel(
        IMijozService mijozService,
        ILogger<MijozListViewModel> logger)
    {
        _mijozService = mijozService;
        _logger = logger;
        InitializeCommands();
    }

    // 6. Properties
    public ObservableCollection<Mijoz> Mijozlar { get; set; }

    // 7. Commands
    public ICommand LoadCommand { get; private set; }

    // 8. Public methods
    public async Task LoadDataAsync() { }

    // 9. Private methods
    private void InitializeCommands() { }
}
```

### MVVM Best Practices

1. **ViewModels never reference Views**
2. **Views only set DataContext, nothing else**
3. **Use Commands, not event handlers**
4. **Business logic in Services, not ViewModels**
5. **Use INotifyPropertyChanged properly**
6. **Async operations with AsyncRelayCommand**

### Error Handling

```csharp
public async Task<List<Mijoz>> LoadMijozlarAsync()
{
    try
    {
        _logger.LogInformation("Loading mijozlar...");

        var result = await _mijozService.GetAllAsync();

        _logger.LogInformation("Loaded {Count} mijozlar", result.Count);
        return result;
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Network error while loading mijozlar");
        throw new ApplicationException("Server bilan bog'lanishda xatolik", ex);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error while loading mijozlar");
        throw;
    }
}
```

### Dependency Injection Registration

```csharp
// App.xaml.cs
private void ConfigureServices(IServiceCollection services)
{
    // Infrastructure
    services.AddLogging(/*...*/);
    services.AddHttpClient<IMijozService, MijozService>(/*...*/);

    // Application Services - Singleton
    services.AddSingleton<INavigationService, NavigationService>();
    services.AddSingleton<IDialogService, DialogService>();
    services.AddSingleton<ITokenService, TokenService>();

    // Business Services - Scoped (or Singleton for desktop)
    services.AddSingleton<IMijozService, MijozService>();
    services.AddSingleton<IShartnomService, ShartnomService>();

    // ViewModels - Transient
    services.AddTransient<MijozListViewModel>();
    services.AddTransient<MijozEditViewModel>();

    // Views - Transient
    services.AddTransient<MijozListView>();
    services.AddTransient<MijozEditView>();
}
```

## 🧪 Testing

### Unit Test Example

```csharp
public class MijozServiceTests
{
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<MijozService>> _loggerMock;
    private readonly MijozService _service;

    public MijozServiceTests()
    {
        _httpClientMock = new Mock<HttpClient>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<MijozService>>();

        _service = new MijozService(
            _httpClientMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfMijoz()
    {
        // Arrange
        _tokenServiceMock.Setup(x => x.GetTokenAsync())
            .ReturnsAsync("test-token");

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Mijoz>>(result);
    }
}
```

## 📚 API Integration

### Base URL
```
Production: http://app.metra-rent.uz/api
Development: http://localhost:4001/api (or your local server)
```

### Authentication
- Bearer token (JWT)
- Token stored securely in encrypted settings
- Auto token refresh (future)

### Common Endpoints

```
POST   /auth/login
POST   /auth/logout
GET    /auth/user

GET    /client?page={page}&search={search}
POST   /client
PUT    /client/{id}
DELETE /client/{id}

GET    /shartnoma?page={page}
POST   /shartnoma
PUT    /shartnoma/{id}
DELETE /shartnoma/{id}

... (other endpoints)
```

## 🔐 Security

1. **Token Storage**: Encrypted local storage (not plain Settings.Default)
2. **Sensitive Data**: Never log passwords, tokens, or personal data
3. **HTTPS Only**: Production API calls always HTTPS
4. **Input Validation**: All user input validated before sending to API
5. **Error Messages**: Never expose stack traces or internal errors to users

## 📖 Documentation

- **Architecture.md** - Detailed architecture documentation
- **API.md** - API endpoint documentation
- **Development.md** - Development workflow and guidelines
- **CLAUDE.md** - Instructions for Claude Code

## 🤝 Contributing

### Git Workflow

```bash
# Create feature branch
git checkout -b feature/mijoz-search

# Make changes and commit
git add .
git commit -m "feat: add advanced search for mijozlar"

# Push and create PR
git push origin feature/mijoz-search
```

### Commit Message Convention

```
feat: add new feature
fix: bug fix
refactor: code refactoring
docs: documentation update
test: add tests
style: formatting changes
chore: maintenance tasks
```

## 📋 Development Roadmap

### Phase 1: Foundation (Week 1-2)
- [x] Project setup with proper architecture
- [ ] DI configuration
- [ ] Base ViewModels and Services
- [ ] Navigation infrastructure
- [ ] Logging setup
- [ ] API client base implementation

### Phase 2: Core Features (Week 3-6)
- [ ] Authentication module
- [ ] Mijozlar module
- [ ] Shartnoma module
- [ ] Materiallar module
- [ ] Filiallar module

### Phase 3: Advanced Features (Week 7-10)
- [ ] Faktura module with PDF
- [ ] Kassa module
- [ ] Ombor module
- [ ] Hisobotlar module
- [ ] Xodimlar & Permissions

### Phase 4: Polish & Testing (Week 11-12)
- [ ] Unit tests (80% coverage target)
- [ ] Performance optimization
- [ ] UI/UX polish
- [ ] Documentation completion
- [ ] Deployment setup

### Phase 5: Future Enhancements
- [ ] Offline mode
- [ ] Real-time notifications
- [ ] Advanced reporting
- [ ] Mobile companion app
- [ ] Multi-language support

## 🐛 Troubleshooting

### Common Issues

**Issue**: DI container not resolving services
```csharp
// Solution: Ensure services are registered in App.xaml.cs
services.AddSingleton<IMijozService, MijozService>();
```

**Issue**: HttpClient timeout
```csharp
// Solution: Increase timeout in HttpClient configuration
services.AddHttpClient<IMijozService, MijozService>(client => {
    client.Timeout = TimeSpan.FromSeconds(60);
});
```

**Issue**: Token expired
```csharp
// Solution: Implement token refresh or redirect to login
// Check AuthService for token validation
```

## 📞 Support

- **Email**: support@metra-rent.uz
- **Issues**: GitHub Issues
- **Documentation**: See `/docs` folder

## 📄 License

Proprietary - Metra Rent © 2024

---

**Built with ❤️ using WPF, .NET 8, and Clean Architecture**

---

## 🎓 Learning Resources

- [WPF MVVM Pattern](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Material Design in XAML](http://materialdesigninxaml.net/)

---

## Developer Notes

### Ko'p ishlatiladigan patterns:

1. **Repository Pattern** (agar kerak bo'lsa)
2. **Factory Pattern** (ViewModels uchun)
3. **Observer Pattern** (INotifyPropertyChanged orqali)
4. **Command Pattern** (ICommand orqali)
5. **Service Locator** (DI Container orqali)

### Performance Tips:

1. ObservableCollection'ni to'g'ri ishlatish
2. Async operations uchun CancellationToken ishlatish
3. Large lists uchun virtualization
4. Image caching
5. Memory leak'lardan qochish (event unsubscribe)

### Security Checklist:

- [ ] Token encryption
- [ ] Secure API communication (HTTPS)
- [ ] Input validation
- [ ] SQL injection prevention (backend)
- [ ] XSS prevention (agar web view bo'lsa)
- [ ] Sensitive data logging prevention
- [ ] Proper error messages (no stack traces to users)
