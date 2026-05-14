# TheTechIdea.Beep.Winform.Controls

**Beep WinForms Controls** is a comprehensive, commercially viable set of UI components
built on the Beep framework for .NET 8 + Windows Forms applications.

---

## Highlights

- **BeepDocumentHost / BeepTabbedView** — Full MDI/docking surface with 8 tab styles,
  split-view groups, auto-hide tool panels, float & dock, cross-host drag, MRU switching,
  and full layout persistence (JSON, schema v3).
- **BeepDocumentManager** — Non-visual orchestrator. Drop onto the component tray and wire
  your menu, status strip, workspaces, cloud sync, and keyboard shortcuts without code.
- **BeepButton / BeepAdvancedButton** — Themed, icon-aware buttons with badge support.
- **BeepLabel, BeepPanel, BeepGroupBox** — Themed container and display controls.
- **BeepSimpleGrid** — Data grid with Beep theming and column customisation.
- **BeepCalendar** — Full-featured calendar control (Month / Week / Day / Agenda views).
- **BeepChart** — Themed charting control.
- **Theme system** — `BeepThemesManager` + `BeepStyling` + `StyledImagePainter` pipeline.

---

## Quick Start

```csharp
// 1. Drop BeepTabbedView and BeepDocumentManager onto your form at design time.
// 2. Set manager.View = tabbedView, manager.WindowMenuOwner = menuStrip1.
// 3. Open a document:
var panel = manager.AddDocument("Welcome", iconPath: null, activate: true);
panel!.Controls.Add(new Label { Text = "Hello, Beep!", Dock = DockStyle.Fill });

// 4. Guard close:
manager.DocumentClosing += (_, e) =>
{
    if (e.Panel?.IsModified == true)
        e.Cancel = MessageBox.Show("Unsaved changes. Close?", "",
            MessageBoxButtons.YesNo) == DialogResult.No;
};
```

See the [10-minute IDE shell tutorial](DocumentHost/Tutorials/01-IdeShell.md) for a
complete walkthrough.

---

## Documentation

| Link | Content |
|------|---------|
| [DocumentHost/Readme.md](DocumentHost/Readme.md) | Overview, quick start, full class reference |
| [DocumentHost/BeepDocumentManager.Readme.md](DocumentHost/BeepDocumentManager.Readme.md) | Manager component reference (properties, events, methods) |
| [DocumentHost/Tutorials/01-IdeShell.md](DocumentHost/Tutorials/01-IdeShell.md) | 10-minute IDE shell tutorial |
| [DocumentHost/Tutorials/02-Migrate-from-host-only.md](DocumentHost/Tutorials/02-Migrate-from-host-only.md) | Migration guide from host-only forms |
| [Styling/Readme.md](Styling/Readme.md) | Theme pipeline and styling guide |

---

## Requirements

- .NET 8 Windows (`net8.0-windows`)
- `TheTechIdea.Beep.DataManagementModels`

## License

MIT — see [LICENSE.txt](../../LICENSE.txt).
