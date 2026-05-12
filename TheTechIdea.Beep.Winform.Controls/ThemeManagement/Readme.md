# ThemeManagement

This folder contains theme orchestration classes for WinForms controls.

## Recent update

- 2026-05-12: `BeepCalendar` was structurally refactored into partial files (`BeepCalendar.Core.cs`, `BeepCalendar.Fields.cs`, `BeepCalendar.Painting.cs`, `BeepCalendar.LayoutTheme.cs`, `BeepCalendar.EventOperations.cs`, `BeepCalendar.Types.cs`) with no behavior rewrite. Theme usage remains centralized through `BeepThemesManager` and existing theme tokens.
