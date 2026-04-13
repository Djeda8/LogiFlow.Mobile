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
| `IReceptionService` | Singleton | Stateless, safe to share |
| `IMasterDataService` | Singleton | Stateless master data |
| `IReceptionSessionService` | Singleton | Shared reception flow state |
| `ICameraScanService` | Singleton | Shared scan callback coordination |
| `IClaudeService` | Singleton | Stateless HTTP client wrapper |
| `IChatDialogService` | Singleton | Resolves ChatViewModel from DI and shows popup |
| `ChatViewModel` | Transient | Fresh instance per chat session |
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
- **Android**: `files/logs/logiflow-YYYYMMDD.log` in app data directory
- **All platforms**: `logs/logiflow-.log` with daily rolling
- **Accessible via**: `adb exec-out run-as com.companyname.logiflow.mobile cat /data/data/com.companyname.logiflow.mobile/files/logs/logiflow-YYYYMMDD.log > C:\logs\logiflow.log`

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
- `ThemeService.ApplyThemeToApplication` — depends on `Application.Current`, platform API

### Testability patterns
- `INavigationWindowService` abstracts `NavigationPage` for navigation testing
- `IPreferencesService` abstracts MAUI `Preferences` for settings testing
- `IFileSystemService` abstracts `FileSystem` for log path testing
- `ICameraScanService` coordinates scan callbacks between ViewModels and camera page
- `IChatDialogService` abstracts popup display for AI chat testing

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

## 10. Theme System

### Decision
Runtime dark/light theme switching using swappable `ResourceDictionary` instances.

### Style files
| File | Contents |
|------|----------|
| `LightColors.xaml` | Light theme color palette |
| `DarkColors.xaml` | Dark theme color palette |

Both files expose identical semantic keys so all styles resolve correctly in either theme.

### How it works
1. `IThemeService.ApplyTheme(code)` removes the active color dictionary from `Application.Current.Resources.MergedDictionaries` and adds the new one
2. All style properties use `DynamicResource` — controls update automatically without page reload
3. On startup, `SplashViewModel.StartAsync()` reads `Settings.TemaVisual` and calls `ApplyTheme()` before navigation
4. Theme selection is persisted via `ISettingsService` alongside other user settings

### Why not MAUI AppTheme
- `AppTheme` only supports system-level light/dark, not user-controlled switching
- Custom solution gives full control over colors and allows the same pattern as `ILocalizationService`

### Platform notes
- `Entry` placeholder color uses `PlaceholderColor="{DynamicResource SecondaryText}"` in `PrimaryEntryStyle`
- `Entry` underline, cursor, and text selection colors on Android are managed by `EntryThemeUpdater` (in `Platforms/Android/`) via `EntryHandler.Mapper.AppendToMapping` in `MauiProgram.cs`
- `App.xaml.cs` subscribes to `IThemeService.ThemeChanged` and traverses the visual tree to update all active Entry controls in real-time
- `Entry` fields are wrapped in `EntryBorderStyle` borders for consistent theming of background and stroke colors
- `FontImageSource` elements use `DynamicResource` for real-time theme-aware color updates
- Icons using `AppIconExtension` resolve color once on page creation — correct at startup, update when navigating to a new page

### Design principles
- Semantic keys only — no hardcoded hex values in style files
- New semantic keys added: `PageBackground`, `CardBackground`, `EntryBackground`, `ErrorEntryBackground`
- All pages set `BackgroundColor="{DynamicResource PageBackground}"` explicitly

---

## 11. Camera Barcode Scanning

### Decision
**BarcodeScanning.Native.Maui** with a dedicated `CameraScanPage` and `ICameraScanService` callback coordination.

### Why BarcodeScanning.Native.Maui
- Native ML APIs on Android (Google ML Kit) and iOS (Apple Vision)
- Compatible with .NET MAUI 9
- Better performance and reliability than ZXing.Net.MAUI for .NET 9

### How it works
1. Any ViewModel that needs a scan calls `ICameraScanService.RequestScan(callback)` and navigates to `CameraScanPage`
2. `CameraScanPage` activates the camera on `OnAppearing` and requests camera permission at runtime
3. When a barcode is detected, `CameraScanViewModel` navigates back first, then calls `ICameraScanService.DeliverResult(code)`
4. The callback registered by the requesting ViewModel is invoked with the scanned code
5. Navigation back happens before delivery to ensure the requesting page is active when the callback fires

### Why navigate back before delivering result
- ViewModels are Singleton — the callback always targets the correct instance
- Delivering after navigation back ensures the UI binding updates are processed on the active page
- Prevents race conditions between navigation stack changes and property updates

### Permissions
- Android: `android.permission.CAMERA` in `AndroidManifest.xml` + runtime request via `Permissions.RequestAsync<Permissions.Camera>()`

---

## 12. Reception Module Architecture

### Decision
Multi-step sequential flow with `IReceptionSessionService` as shared state container.

### Flow
```
ReceptionStartPage → ReceptionHeaderPage → ReceptionDetailPage → [ReceptionChecklistPage] → ReceptionConfirmationPage → ReceptionItemsPage
```

### Why IReceptionSessionService
- The reception DTO is built progressively across 6 screens
- Singleton service preserves state across the entire flow
- Each ViewModel reads and writes to the session independently
- No coupling between ViewModels — they only depend on the service

### Flow types
| Type | Checklist | Extra fields |
|------|-----------|--------------|
| STANDARD | Not required | None |
| TEST_SAMPLE | Mandatory | Sample reference, lot number |
| Any with VEHICLE article | Mandatory | None |

### Demo data
Simulated receptions available for testing:
| Code | Flow type | Sender |
|------|-----------|--------|
| REC-001 | STANDARD | Supplier A |
| REC-002 | TEST_SAMPLE | Lab B |
| REC-003 | STANDARD | Supplier C |

---

## 13. AI Assistant — Contextual Chat

### Decision
**Anthropic Claude API** (claude-haiku-4-5) integrated as a contextual WMS assistant available on all Reception screens.

### Architecture
```
BaseViewModel.OpenAiChatCommand
        ↓
IChatDialogService.ShowAsync(WmsScreenContext)
        ↓
ChatDialogService → resolves ChatViewModel from DI
        ↓
ChatBottomSheet (Popup) bound to ChatViewModel
        ↓
IClaudeService.SendAsync(history, context)
        ↓
Anthropic HTTP API → claude-haiku-4-5
```

### Key design decisions

**`IChatDialogService`** — decouples ViewModels from UI. ViewModels call `ShowAsync(context)` without knowing anything about Popups or Pages.

**`BaseViewModel.OpenAiChatCommand`** — the command is defined once in `BaseViewModel`. Any ViewModel that sets `ChatDialogService` and overrides `GetAiContext()` automatically gets the 💬 Ask AI button wired up via XAML binding with zero additional code-behind.

**`WmsScreenContext`** — each ViewModel overrides `GetAiContext()` to provide real-time screen state to Claude: reception number, flow type, current article, lines count, checklist progress, etc. Claude uses this as its system prompt context.

**`DataTemplateSelector`** — chat bubbles use `ChatMessageTemplateSelector` instead of `IsVisible` converters to avoid double-rendering of user and assistant messages.

**`ChatViewModel` as Transient** — fresh instance per chat session. History is cleared on `InitialiseContext()`, so each popup open starts a clean conversation with the current screen context.

### API key management
| Environment | Source |
|-------------|--------|
| Local debug | `appsettings.Development.json` (excluded from git) |
| CI/CD | GitHub Secret `ANTHROPIC_API_KEY` injected into `appsettings.json` at build time |

### Screens with AI chat enabled
All 6 Reception screens expose `OpenAiChatCommand` via their ViewModel and show the 💬 Ask AI button in the page header.

### Why not add AI to Settings
Settings is a technical configuration screen used rarely by operators. There is no operational workflow context where Claude can add value. The AI assistant is intentionally scoped to operational modules only.

---

## 14. How to Add a New Module

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

**7. Add 💬 Ask AI button to XAML header**
```xml
<Button Text="{local:Translate Key=AiChatAskButton}"
        Command="{Binding OpenAiChatCommand}" ... />
```

**8. Write unit tests**
```
Tests/ViewModels/ReceptionViewModelTests.cs
```

---

*LogiFlow Mobile — Daniel Ojeda Ubeda*
