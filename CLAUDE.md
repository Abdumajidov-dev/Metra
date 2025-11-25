# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Metra v3.0 is a rental management system (equipment/materials rental) built as a WPF desktop application using .NET 8 and layered architecture. This is a complete rewrite of Metra v2.0 with professional architecture standards.

**Language**: Uzbek (UI, comments, and variable names use Uzbek/Uzbek transliteration)

**Architecture**: 3-Layer Architecture (Presentation → Application → Infrastructure)

## Technology Stack

- **.NET 8.0** (net8.0-windows for Desktop, net8.0 for class libraries)
- **C# 12** (LangVersion: 12, Nullable Reference Types enabled, Implicit Usings enabled)
- **WPF** with MVVM pattern
- **CommunityToolkit.Mvvm** 8.3.2 for MVVM helpers (ObservableObject, RelayCommand, AsyncRelayCommand)
- **Microsoft.Extensions.DependencyInjection** 8.0.1 for IoC container
- **Microsoft.Extensions.Http** 8.0.1 for HttpClientFactory
- **Serilog** 4.1.0 for structured logging (File and Console sinks)
- **MaterialDesignThemes** 5.1.0 and **Syncfusion WPF** 27.1.55 for UI
- **Newtonsoft.Json** 13.0.3 for JSON serialization
- **FreeSpire.PDF** 10.2.0 for PDF generation
- **QRCoder** 1.6.0 for QR code generation
- **Notification.Wpf** 8.0.0 for toast notifications

## Project Structure

The solution follows a 3-Layer Architecture:

```
src/
├── Metra.Desktop/              # Presentation Layer (WPF)
│   ├── Views/                  # XAML files organized by module
│   │   ├── Auth/
│   │   ├── Mijozlar/
│   │   ├── Shartnoma/
│   │   └── [8 other modules]
│   ├── ViewModels/            # ViewModels with presentation logic
│   │   ├── Base/
│   │   │   └── ViewModelBase.cs
│   │   └── [module ViewModels]
│   ├── Converters/            # Value converters
│   │   ├── BoolToVisibilityConverter.cs
│   │   └── DateTimeConverter.cs
│   ├── Controls/              # Custom WPF controls
│   ├── App.xaml.cs           # DI container setup
│   └── MainWindow.xaml       # Main application window
│
├── Metra.Application/         # Application Layer (Business Logic)
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   ├── Base/         # Core service interfaces (Auth, Token, Dialog, Navigation)
│   │   │   └── Malumotlar/   # Data service interfaces (Mijoz, Filial, etc.)
│   │   ├── Implementation/
│   │   │   ├── Base/         # Core service implementations
│   │   │   └── Malumotlar/   # Data service implementations
│   │   └── Service/          # Legacy services (being refactored)
│   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Requests/
│   │   │   └── Malumotlar/   # Request DTOs by module
│   │   └── Responses/
│   │       └── Malumotlar/   # Response DTOs by module
│   ├── Exceptions/           # Application exceptions
│   │   └── ApplicationException.cs (MetraException base + 4 derived)
│   ├── Configuration/        # Application configuration
│   │   └── ApiConfig.cs      # API endpoints and settings
│   └── Validators/           # Input validation (future use)
│
└── Metra.Infrastructure/      # Infrastructure Layer (External Services)
    ├── Persistence/
    │   └── Settings/
    │       └── AppSettings.cs # JSON-based local settings
    ├── Logging/
    │   └── LoggingConfiguration.cs # Serilog setup
    └── Services/
        └── TokenService.cs   # Token management implementation
```

## Build and Run Commands

**Requirements**: .NET 8.0 SDK or later (check with `dotnet --version`)

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run application (from root directory)
dotnet run --project src/Metra.Desktop/Metra.Desktop.csproj

# Build for release
dotnet build --configuration Release

# Clean build artifacts
dotnet clean

# Build specific project
dotnet build src/Metra.Desktop/Metra.Desktop.csproj
```

**Platform Note**: This is a Windows-only application (WPF). Use Windows paths with backslashes (`\`) in commands when on Windows.

## API Configuration

**Base URL**: `http://app.metra-rent.uz/api/` or `http://10.100.104.104:4001/api/` (development server - note: trailing slash required)
**Image Base URL**: `http://app.metra-rent.uz/api/public/storage/`

Configuration location: `src/Metra.Application/Configuration/ApiConfig.cs`

**Note**: The BaseUrl in ApiConfig.cs can be toggled between production and development environments by commenting/uncommenting the appropriate line.

All HTTP clients are registered with HttpClientFactory in DI container with a 60-second default timeout.

**Authentication**: Bearer token (JWT) stored in local settings via `ITokenService` (using `AppSettings`).

## Core Modules

The application manages the following business domains:

1. **Auth** - Login/logout, token management, session management
2. **Mijozlar** (Customers/Clients) - CRUD, search, pagination, client history
3. **Shartnoma** (Contracts) - Contract lifecycle, status tracking, payment schedules, PDF generation
4. **Materiallar** (Materials/Equipment) - Catalog, categories, pricing, availability tracking
5. **Faktura** (Invoices) - Invoice generation, printing, payment tracking, QR codes
6. **Filiallar** (Branches) - Branch management and branch-specific operations
7. **Xodimlar** (Employees) - Employee management, roles, permissions
8. **Ombor** (Warehouse) - Stock management, receiving, dispatch, transfers
9. **Kassa** (Cashier) - Cash income/expenses, expense categories, cash flow
10. **Hisobotlar** (Reports) - Material movement, financial reports, custom date filtering

## Architecture Patterns

### MVVM Implementation

**Base Class**: `src/Metra.Desktop/ViewModels/Base/ViewModelBase.cs`

ViewModelBase provides:
- Inherits from `ObservableObject` (CommunityToolkit.Mvvm)
- `IsBusy` property for loading states
- `IsNotBusy` property (inverse of IsBusy) for enabling controls
- `Title` property for page titles
- `InitializeAsync()` for async initialization
- `Cleanup()` for resource disposal

**MVVM Rules:**
- **ViewModels** inherit from `ViewModelBase` (or use `ObservableObject` from CommunityToolkit.Mvvm)
- **Never** reference Views from ViewModels
- **Views** only set `DataContext` - no business logic in code-behind
- Use **Commands** (ICommand), not event handlers
- Business logic belongs in **Services**, not ViewModels
- Use `AsyncRelayCommand` for async operations
- Use `ObservableCollection<T>` for collections that need UI updates

### Dependency Injection

**Setup Location**: `src/Metra.Desktop/App.xaml.cs`

All dependencies are constructor-injected. Service registration in `App.xaml.cs`:

**Service Lifetimes:**
- **Singleton**: `ITokenService`, `NotificationManager`, `AppSettings`, infrastructure services
- **HttpClient-based**: All API services (Auth, Mijoz, Filial, etc.) registered via `AddHttpClient<TInterface, TImplementation>()`
- **Transient**: ViewModels, Views, Pages, Windows

Example ViewModel constructor:
```csharp
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
    LoadCommand = new AsyncRelayCommand(LoadDataAsync);
}
```

### Error Handling

All service methods should:
1. Log errors using `ILogger<T>`
2. Throw `ApplicationException` with user-friendly Uzbek messages for UI
3. Never expose stack traces or internal errors to users
4. Handle `HttpRequestException` separately for network errors

**Available Application Exceptions** (`src/Metra.Application/Exceptions/ApplicationException.cs`):
- `MetraException` (base class)
- `NotFoundException` - Entity not found
- `ValidationException` - Validation failed
- `UnauthorizedException` - User not authenticated
- `ForbiddenException` - User lacks permissions

Example:
```csharp
try
{
    _logger.LogInformation("Loading mijozlar...");
    var result = await _mijozService.GetAllAsync();
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
```

## Naming Conventions

- **Classes**: PascalCase (e.g., `MijozService`)
- **Interfaces**: I + PascalCase (e.g., `IMijozService`)
- **Private fields**: `_camelCase` (e.g., `_mijozService`)
- **Properties**: PascalCase (e.g., `Name`, `PhoneNumber`)
- **Methods**: PascalCase (e.g., `LoadDataAsync()`)
- **Parameters**: camelCase (e.g., `name`, `phoneNumber`)
- **Constants**: PascalCase (e.g., `ApiVersion`)
- **Enums**: PascalCase (e.g., `ShartnomStatus.Active`)

## Important Implementation Notes

1. **Syncfusion License**: Must be registered in `App.xaml.cs` using `SyncfusionLicenseProvider.RegisterLicense("YOUR_LICENSE_KEY")`

2. **Token Management**: Use `ITokenService` for all token operations. Never directly access `Settings.Default.token`.

3. **Navigation**: Use `INavigationService.NavigateTo<TViewModel>(parameter)` for navigation between views.

4. **Notifications**: Use `NotificationManager` (from Notification.Wpf) to show toast notifications for success/error messages.

5. **Pagination**: API responses use `PaginatedResult<T>` with `Data`, `CurrentPage`, `LastPage`, `Total`, `From`, `To`, `PerPage` properties. For non-paginated responses, use `ResultNotPagination<T>` with `Success` and `Result` properties (note: API uses "resoult" typo in JSON).

6. **Logging**:
   - Configuration: `src/Metra.Infrastructure/Logging/LoggingConfiguration.cs`
   - Logs written to: `%APPDATA%\Metra\logs\metra-YYYY-MM-DD.txt` (daily rolling interval)
   - Use structured logging with Serilog
   - Available levels: Debug, Information, Warning, Error
   - Microsoft/System namespaces filtered to Information/Warning minimum

7. **Settings Storage**:
   - Location: `%APPDATA%\Metra\settings.json`
   - Implementation: `src/Metra.Infrastructure/Persistence/Settings/AppSettings.cs`
   - Methods: `GetSetting<T>()`, `SetSetting<T>()`, `RemoveSetting()`

8. **XAML Binding**: Always use `{Binding}` with `INotifyPropertyChanged` properly implemented. For collections, use `ObservableCollection<T>`.

9. **Value Converters** (located in `src/Metra.Desktop/Converters/`):
   - `BoolToVisibilityConverter` - true → Visible, false → Collapsed
   - `DateTimeConverter` - DateTime formatting

## DTOs (Data Transfer Objects)

The application uses DTOs for all data exchange between layers and with the API.

### Key Response DTOs

**MijozResponse** - `src/Metra.Application/DTOs/Responses/MijozResponse.cs`:
- Customer information from API
- Properties: Id, Name, Phone, Phone2, Address, PassportSeria, PassportNumber, Pnfl, Description, WhenGiven, BirthDay, Image, ImagePassport, BranchId, BranchName, CreatedAt, UpdatedAt

**FilialResponse** - `src/Metra.Application/DTOs/Responses/FilialResponse.cs`:
- Branch information from API
- Properties: Id, Name, Address, Phone, CreatedAt, UpdatedAt

**FakturaResponse** - `src/Metra.Application/DTOs/Responses/FakturaResponse.cs`:
- Invoice information from API

### Key Request DTOs

**MijozCreateRequest** / **MijozUpdateRequest** - `src/Metra.Application/DTOs/Requests/`:
- Data for creating/updating customers

**FilialRequest** - `src/Metra.Application/DTOs/Requests/FilialRequest.cs`:
- Data for branch operations

**LoginRequest** - `src/Metra.Application/DTOs/Requests/LoginRequest.cs`:
- User authentication credentials

## Design Philosophy

**Why No Domain Layer?**

This project intentionally uses a **3-Layer Architecture** instead of Clean Architecture's 4-layer approach:

**Reasons:**
- ✅ **Simplicity**: CRUD operations don't require complex domain models
- ✅ **No Duplication**: DTOs already represent all data structures
- ✅ **API-Driven**: Application consumes external API; no internal business rules
- ✅ **Maintainability**: Less code to maintain, easier to understand

**When to Use Domain Layer:**
- Complex business rules and domain logic
- Rich domain models with behavior
- Domain-driven design requirements
- Internal data persistence with ORM

For this rental management system, DTOs + Services provide the right level of abstraction.

## Migration from v2.0

The project is transitioning from Metra v2.0 using an **incremental refactoring** strategy:
- New features written with MVVM + DI from the start
- Existing modules refactored 1-2 per week by priority
- Priority order: Mijoz → Filial → Material → Shartnoma → Others

When refactoring existing code:
- Extract all API calls into Service classes
- Move business logic from code-behind to ViewModels
- Replace event handlers with Commands
- Register all dependencies in DI container

See `REFACTORING_GUIDE.md` for detailed migration patterns.

## Security Considerations

- **Token Storage**: Use encrypted local storage via AppSettings (not plain `Settings.Default`)
- **HTTPS Only**: Production API calls must use HTTPS (currently HTTP in development)
- **Input Validation**: Validate all user input before API calls
- **Sensitive Data**: Never log passwords, tokens, or personal data
- **Error Messages**: Show user-friendly messages in Uzbek; never expose stack traces

## Common Patterns

### Service Interface Pattern
All services are organized by category:

**Application Services:**
- **Interfaces**: `Metra.Application.Services.Interfaces.Base/` (core) or `.Malumotlar/` (data modules)
- **Implementations**: `Metra.Application.Services.Implementation.Base/` or `.Malumotlar/`

**Important**: Desktop layer does NOT have its own services - all services live in Application layer.

### API Response Format Variations

The API uses two different response formats:

**1. Paginated Response** (for list endpoints):
```json
{
  "data": [...],
  "current_page": 1,
  "last_page": 5,
  "total": 47,
  "per_page": 10,
  "from": 1,
  "to": 10
}
```
Use `PaginatedResult<T>` DTO.

**2. Non-Paginated Response** (for single items/search):
```json
{
  "success": true,
  "failure": false,
  "resoult": {...}  // Note: API has typo in field name
}
```
Use `ResultNotPagination<T>` DTO with `[JsonPropertyName("resoult")]` attribute to handle the API typo. The DTO includes both `Success` and `failure` boolean properties.

### Pagination Pattern
```csharp
public async Task<PaginatedResult<T>?> GetAll(int page = 1, string? search = null)
{
    var url = $"/endpoint?page={page}";
    if (!string.IsNullOrEmpty(search))
        url += $"&search={Uri.EscapeDataString(search)}";

    var response = await _httpClient.GetAsync(url);
    // ... handle response
}
```

### Loading State Pattern
ViewModels should have `IsLoading` property to show/hide loading overlays:
```csharp
private bool _isLoading;
public bool IsLoading
{
    get => _isLoading;
    set => SetProperty(ref _isLoading, value);
}

// In async method:
IsLoading = true;
try
{
    // ... load data
}
finally
{
    IsLoading = false;
}
```

Bind to loading overlay in XAML:
```xml
<Grid Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibilityConverter}}">
    <ProgressBar IsIndeterminate="True"/>
</Grid>
```

### HttpClient with Token Pattern
```csharp
var token = await _tokenService.GetTokenAsync();
_httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

var response = await _httpClient.GetAsync("/endpoint");
```

### Non-Paginated Response Pattern
```csharp
var response = await _httpClient.GetAsync("/endpoint");
response.EnsureSuccessStatusCode();

var content = await response.Content.ReadAsStringAsync();
var result = JsonConvert.DeserializeObject<ResultNotPagination<T>>(content);

if (result?.Success == true)
{
    return result.Result;
}
throw new ApplicationException("API xatolik qaytardi");
```

## File Organization in C# Files

1. Using statements (grouped and sorted)
2. Namespace declaration
3. XML documentation comments for public classes
4. Class definition
5. Private fields
6. Constructor(s)
7. Properties
8. Commands (for ViewModels)
9. Public methods
10. Private methods

## Application Startup Flow

1. `App.xaml.cs` - `OnStartup()` executes:
   - Registers Syncfusion license
   - Configures Serilog logging via `LoggingConfiguration.ConfigureLogging()`
   - Builds DI container with `ConfigureServices()`
   - Creates and shows `MainWindow`

2. Service Registration Order:
   - Logging infrastructure
   - Singletons (NotificationManager, AppSettings, TokenService, etc.)
   - HttpClients with factory
   - Transient services (ViewModels, Views)

3. Navigation:
   - MainWindow contains navigation frame/tabs
   - NavigationService resolves ViewModels from DI container
   - Views are created with ViewModel as DataContext

## Development Workflow

### Adding a New Module

1. **Create Service Interface** in `Metra.Application/Services/Interfaces/`
2. **Implement Service** in `Metra.Application/Services/Implementation/` or dedicated folder
3. **Create DTOs** in `Metra.Application/DTOs/Requests/` and `Responses/`
4. **Create ViewModel** in `Metra.Desktop/ViewModels/[Module]/`
5. **Create View** in `Metra.Desktop/Views/[Module]/`
6. **Register in DI** in `App.xaml.cs` ConfigureServices()
7. **Add Navigation** in MainWindow menu

### Testing a Module

1. Build the solution
2. Run the application
3. Check logs at `%APPDATA%\Metra\logs\metra-[date].txt`
4. Use NotificationManager for user feedback
5. Monitor console output for Serilog messages

### Service Registration Pattern in App.xaml.cs

When adding new services, follow this registration pattern:

```csharp
// For services with HttpClient (most API services)
services.AddHttpClient<IYourService, YourService>(client =>
{
    client.BaseAddress = new Uri(ApiConfig.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds);
});

// For ViewModels (always Transient)
services.AddTransient<YourViewModel>();

// For Views/Pages (always Transient)
services.AddTransient<YourPage>();

// For utility services without HTTP (decide lifetime based on state)
services.AddSingleton<IUtilityService, UtilityService>(); // Stateless, shared
// OR
services.AddTransient<IUtilityService, UtilityService>(); // Stateful, per-use
```
