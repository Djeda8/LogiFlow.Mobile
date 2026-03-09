# Architecture Decision Records — LogiFlow Mobile

This document describes the key architectural decisions made in LogiFlow Mobile, the reasoning behind them, and how the main systems work.

---

## 1. Architecture Pattern — MVVM

### Decision
The application follows the **Model-View-ViewModel (MVVM)** pattern using **CommunityToolkit.Mvvm**.

### Why MVVM
- Native pattern for .NET MAUI and WPF applications
- Clear separation between UI logic (ViewModel) and presentation (View)
- Enables unit testing of all business logic without UI dependencies
- Data binding reduces boilerplate code

### Why CommunityToolkit.Mvvm
- Official Microsoft toolkit, actively maintained
- Source generators for `[ObservableProperty]` and `[RelayCommand]` eliminate boilerplate
- `BaseViewModel` provides `IsBusy`, `HasError` and `ErrorMessage` shared across all ViewModels
- No runtime reflection, uses compile-time source generation

---

## 2. Dependency Injection

### Decision
All services are registered in `MauiProgram.cs` using the built-in .NET DI container.

### Service lifetimes
| Service | Lifetime | Reason |
|---------|----------|--------|
| `ILogService` | Singleton | Single logger instance throughout the app |
| `ILocalizationService` | Singleton | Shared culture state |
| `INavigationService` | Singleton | Single navigation stack |
| `IAuthService` | Singleton | Stateless, safe to share |
| `ISettingsService` | Singleton | Cached settings |
| `ISessionService` | Singleton | Shared session state |
| ViewModels | Transient | Fresh instance per page |
| Pages | Transient | Fresh instance per navigation |

---

## 3. Navigation System

### Decision
Custom `INavigationService` with a page registry dictionary instead of Shell navigation.

### Why not Shell navigation
- Shell adds complexity for simple linear navigation flows
- Custom service is fully testable with `INavigationWindowService` abstraction
- Explicit page registration makes navigation dependencies visible

### How it works
1. Pages are registered in `NavigationService` constructor with a `Dictionary<string, Type>`
2. `NavigateToAsync(pageKey)` resolves the page from DI and pushes it to the navigation stack
3. `clearStack: true` inserts the new page before the root and pops to root — avoids back navigation to login after logout
4. `INavigationWindowService` abstracts `NavigationPage` for unit testing

---

## 4. Error Handling

### Decision
Centralized `IErrorHandlerService` with custom domain exceptions.

### Custom exceptions
| Exception | When used |
|-----------|-----------|
| `ValidationException` | Invalid field values (URL, timeout) |
| `AuthException` | Authentication failures |
| `ConnectionException` | Server connectivity failures |

### Flow
```
ViewModel → catches exception → IErrorHandlerService.Handle() → returns localized message → ViewModel sets ErrorMessage
```

### Why centralized
- Single place to change error message format
- Consistent logging for all error types
- ViewModels stay clean — no inline error formatting

---

## 5. Localization System

### Decision
Custom `TranslateExtension` XAML markup extension with real-time language switching.

### How it works
1. `ILocalizationService` manages the current `CultureInfo` and fires `LanguageChanged` event
2. `TranslateExtension` creates a `TranslationWrapper` per binding
3. `TranslationWrapper` subscribes to `LanguageChanged` and implements `INotifyPropertyChanged`
4. When language changes, all bindings update automatically without page reload

### Why not built-in MAUI localization
- Built-in localization requires app restart to change language
- Custom solution enables real-time switching from Settings screen

### Usage in XAML
```xml
<Label Text="{local:Translate Key=SettingsTitle}" />
```

### Usage in ViewModels and Services
```csharp
_localizationService.GetString(nameof(AppResources.ErrorInvalidUrl))
```

---

## 6. Logging

### Decision
**Serilog** with file sink and Android Logcat sink.

### Log levels
| Level | When used |
|-------|-----------|
| `Debug` | Detailed diagnostic information |
| `Info` | Normal application flow |
| `Warning` | Unexpected but recoverable situations |
| `Error` | Failures that need attention |

### Structured logging pattern
```csharp
_logService.Info("User authenticated. Username={Username}", username);
_logService.OperationStart("Login", username);
_logService.OperationSuccess("Login", username);
_logService.OperationFailure("Login", username, reason);
```

### Log file location
- **Android**: External storage `/LogiFlow/logs/` if available, otherwise app data directory
- **All platforms**: `logs/logiflow-.log` with daily rolling

---

## 7. Settings Persistence

### Decision
`ISettingsService` with JSON serialization stored via `IPreferencesService` (MAUI Preferences abstraction).

### Why abstract Preferences
- `Preferences` is a MAUI platform API not available in unit tests
- `IPreferencesService` wrapper enables full testing of `SettingsService`

### Flow
```
SettingsViewModel → ISettingsService → IPreferencesService → MAUI Preferences → Device storage
```

---

## 8. Unit Testing Strategy

### Decision
xUnit + Moq with 85% code coverage.

### What is tested
- All ViewModels — navigation, validation, error handling, loading states
- All Services — business logic, error paths, logging calls
- Converters and Extensions — edge cases

### What is not tested
- XAML Pages (code-behind) — depend on `InitializeComponent()`, not testable without UI
- Platform wrappers (`PreferencesService`, `NavigationWindowService`, `FileSystemService`) — thin wrappers over platform APIs
- `MauiProgram`, `App`, `AppShell` — infrastructure, no business logic

### Testability patterns
- `INavigationWindowService` abstracts `NavigationPage` for navigation testing
- `IPreferencesService` abstracts MAUI `Preferences` for settings testing
- `IFileSystemService` abstracts `FileSystem` for log path testing

---

## 9. Design System

### Decision
Centralized XAML styles in `Resources/Styles/` following a Design System defined for industrial environments.

### Style files
| File | Contents |
|------|----------|
| `Colors.xaml` | Color palette |
| `Typography.xaml` | Font styles |
| `Buttons.xaml` | Button styles |
| `Entries.xaml` | Input field styles |
| `Labels.xaml` | Label styles |
| `Layout.xaml` | Card and section styles |
| `MenuStyles.xaml` | Menu item styles |

### Design principles
- Minimum touch target 48px for industrial use with gloves
- Every state communicated with icon + color + text, never color alone
- Maximum one Primary button per screen

---

## 10. How to Add a New Module

Follow these steps to add a new module (e.g. `Reception`):

**1. Create the View**
```
Views/Reception/ReceptionPage.xaml
Views/Reception/ReceptionPage.xaml.cs
```

**2. Create the ViewModel**
```
ViewModels/Reception/ReceptionViewModel.cs
```
Inherit from `BaseViewModel` and inject required services.

**3. Register in MauiProgram.cs**
```csharp
builder.Services.AddTransient<ReceptionPage>();
builder.Services.AddTransient<ReceptionViewModel>();
```

**4. Register in NavigationService**
```csharp
{ nameof(ReceptionPage), typeof(ReceptionPage) }
```

**5. Add menu item in MenuViewModel**
```csharp
new(_localizationService.GetString(nameof(AppResources.MenuReception)), nameof(ReceptionPage), AppIconGlyph.Inventory2)
```

**6. Add localization keys to .resx files**
```xml
<data name="MenuReception"><value>Reception</value></data>
```

**7. Write unit tests**
```
Tests/ViewModels/ReceptionViewModelTests.cs
```

---

*LogiFlow Mobile — Daniel Ojeda Ubeda*
