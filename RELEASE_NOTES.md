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
