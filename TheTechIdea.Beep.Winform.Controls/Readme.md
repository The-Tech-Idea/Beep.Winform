# TheTechIdea.Beep.Winform.Controls

This project hosts reusable Beep WinForms controls.

## Document Host

`BeepDocumentHost` / `BeepTabbedView` is the main MDI and docking surface.
`BeepDocumentManager` is the companion non-visual component (component tray) that
orchestrates the host — wiring menu, status strip, keyboard shortcuts, layout
persistence, workspace management, and cloud sync.

- [DocumentHost/Readme.md](DocumentHost/Readme.md) — overview, feature list, quick start, class reference
- [DocumentHost/BeepDocumentManager.Readme.md](DocumentHost/BeepDocumentManager.Readme.md) — manager component full reference
- [DocumentHost/Tutorials/01-IdeShell.md](DocumentHost/Tutorials/01-IdeShell.md) — 10-minute IDE shell tutorial
- [DocumentHost/Tutorials/02-Migrate-from-host-only.md](DocumentHost/Tutorials/02-Migrate-from-host-only.md) — migration guide

## Recent update

- 2026-05-12: `Calendar/BeepCalendar` was mechanically split into focused partial files to reduce file size and improve maintainability, with no logic rewrite.
- New partial files include `BeepCalendar.Core.cs`, `BeepCalendar.Fields.cs`, `BeepCalendar.Painting.cs`, `BeepCalendar.LayoutTheme.cs`, `BeepCalendar.EventOperations.cs`, and `BeepCalendar.Types.cs`.
