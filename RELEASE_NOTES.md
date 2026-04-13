## v1.2.0 - AI Contextual Assistant
- Anthropic Claude API (claude-haiku-4-5) integrated as a contextual WMS assistant
- 💬 Ask AI button available on all 6 Reception screens
- Chat bottom sheet with full conversation history per session
- Screen-aware context — Claude knows which screen the operator is on and its current state
- Full dark/light theme support in the chat UI following the app design system
- Fully localized chat UI (English and Spanish)
- MVVM-pure implementation via IChatDialogService — no UI logic in ViewModels
- API key managed via GitHub Secrets in CI/CD pipeline and appsettings.Development.json locally
- ChatViewModel registered as Transient — fresh session per popup open

## v1.1.0 - Reception Module
- Full reception flow: scan, header, detail, checklist, confirmation and item generation
- Camera-based barcode scanning via BarcodeScanning.Native.Maui
- Support for STANDARD and TEST_SAMPLE flow types
- Dynamic checklist mandatory.
- Reception session management across multi-step flow
- Master data service with simulated articles, locations, senders and recipients
- Real-time validation with immediate operator feedback
- Architecture prepared for backend integration
- Test suite expanded to 439 passing tests
- Code coverage at 94%

## v1.0.19 - Entry theme refinements
- Entry underline and cursor color now update in real-time when theme changes
- Entry text selection highlight color adapts to active theme
- Settings screen icons now update in real-time when theme changes
- EntryThemeUpdater helper centralizes native Android Entry styling

## v1.0.18 - Theme visual refinements
- Entry underline color adapts to active theme (normal and focus states)
- Entry borders standardized with EntryBorderStyle
- Logo and menu icons now use DynamicResource for theme-aware colors
- IThemeService exposes CurrentTheme property

## v1.0.17 - Theme improvements
- Entry placeholder color now follows the active theme
- Switch thumb color updated to AccentColor for better visibility in both themes
- Restore Defaults now correctly resets the theme selector and applies light theme

## v1.0.16 - Dark / Light Theme
- Full dark and light theme support with runtime switching
- Theme selector added to Settings screen
- Theme preference persisted across app sessions
- Semantic color system refactored into LightColors.xaml and DarkColors.xaml
- All style files updated to DynamicResource for live theme updates

## v1.0.15 - Tester communications
- Release notes now managed via RELEASE_NOTES.md for full control over tester communications

## v1.0.14 - Settings & Internationalization

- Refactor of the Settings screen with language-independent internal codes
- Full internationalization support (EN/ES) with real-time language switching
- Test coverage improved to 95%
