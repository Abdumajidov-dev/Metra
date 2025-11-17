# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Metra v3.0 is a rental management system (equipment/materials rental) built as a WPF desktop application using .NET 8 and Clean Architecture principles. This is a complete rewrite of Metra v2.0 with professional architecture standards.

**Language**: Uzbek (UI, comments, and variable names use Uzbek/Uzbek transliteration)

## Technology Stack

- **.NET 8.0** (net8.0-windows)
- **WPF** with MVVM pattern
- **CommunityToolkit.Mvvm** for MVVM helpers (ObservableObject, RelayCommand, AsyncRelayCommand)
- **Microsoft.Extensions.DependencyInjection** for IoC container
- **Microsoft.Extensions.Http** for HttpClientFactory
- **Serilog** for structured logging (File and Console sinks)
- **MaterialDesignThemes** 5.1.0 and **Syncfusion WPF** 27.1.55 for UI
- **Newtonsoft.Json** for JSON serialization
- **FreeSpire.PDF** for PDF generation
- **QRCoder** for QR code generation
- **Notification.Wpf** for toast notifications

## Project Structure

The solution follows Clean Architecture with 4 main layers:

```
Metra.Desktop/              # Presentation Layer (WPF)
├── Views/                  # XAML files
├── ViewModels/            # ViewModels with presentation logic
└── Converters/            # Value converters

Metra.Application/         # Application Layer
├── Services/              # Business services (Interfaces/ and Implementation/)
├── DTOs/                  # Data Transfer Objects (Requests/ and Responses/)
└── Validators/            # Input validation

Metra.Domain/              # Domain Layer
├── Entities/              # Domain models
├── Enums/                 # Enumerations
└── Exceptions/            # Custom exceptions

Metra.Infrastructure/      # Infrastructure Layer
├── API/                   # HTTP API clients and endpoints
├── Persistence/           # Local storage (settings, cache)
└── Logging/               # Logging configuration
```

## Build and Run Commands

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run application (from root directory)
dotnet run --project src/Metra.Desktop/Metra.Desktop.csproj

# Build for release
dotnet build --configuration Release
```

## API Configuration

**Base URL**: `http://app.metra-rent.uz/api`

The API client configuration is located in `Metra.Infrastructure/API/ApiConfig.cs`. All HTTP clients are registered with HttpClientFactory in DI container with a 30-second default timeout.

**Authentication**: Bearer token (JWT) stored in encrypted local settings via `ITokenService`.

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

- **ViewModels** inherit from `ViewModelBase` (or use `ObservableObject` from CommunityToolkit.Mvvm)
- **Never** reference Views from ViewModels
- **Views** only set `DataContext` - no business logic in code-behind
- Use **Commands** (ICommand), not event handlers
- Business logic belongs in **Services**, not ViewModels
- Use `AsyncRelayCommand` for async operations

### Dependency Injection

All dependencies are constructor-injected. Service registration in `App.xaml.cs`:

- **Singleton**: `INavigationService`, `IDialogService`, `ITokenService`, `NotificationManager`, most business services
- **Transient**: ViewModels, Views, Pages
- **HttpClient**: Registered via `AddHttpClient<TInterface, TImplementation>()` with HttpClientFactory

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
}
```

### Error Handling

All service methods should:
1. Log errors using `ILogger<T>`
2. Throw `ApplicationException` with user-friendly Uzbek messages for UI
3. Never expose stack traces or internal errors to users
4. Handle `HttpRequestException` separately for network errors

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

5. **Pagination**: API responses use `PaginatedResult<T>` with `Data`, `CurrentPage`, `LastPage`, `Total` properties.

6. **Logging**: Logs are written to `logs/metra-.txt` with daily rolling interval. Use structured logging with Serilog.

7. **XAML Binding**: Always use `{Binding}` with `INotifyPropertyChanged` properly implemented. For collections, use `ObservableCollection<T>`.

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

## Security Considerations

- **Token Storage**: Use encrypted local storage (not plain `Settings.Default`)
- **HTTPS Only**: Production API calls must use HTTPS
- **Input Validation**: Validate all user input before API calls
- **Sensitive Data**: Never log passwords, tokens, or personal data
- **Error Messages**: Show user-friendly messages in Uzbek; never expose stack traces

## Common Patterns

### Service Interface Pattern
All services should have an interface in `Services/Interfaces/` and implementation in `Services/Implementation/` or dedicated folder.

### Pagination Pattern
```csharp
public async Task<PaginatedResult<T>?> GetAll(int page = 1, string? search = null)
{
    var url = $"/endpoint?page={page}";
    if (!string.IsNullOrEmpty(search))
        url += $"&search={Uri.EscapeDataString(search)}";
    // ... API call
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
```

Bind to loading overlay in XAML with `BoolToVisibilityConverter`.

## File Organization in C# Files

1. Using statements (grouped and sorted)
2. Namespace declaration
3. XML documentation comments for public classes
4. Class definition
5. Private fields
6. Constructor(s)
7. Properties
8. Commands
9. Public methods
10. Private methods
