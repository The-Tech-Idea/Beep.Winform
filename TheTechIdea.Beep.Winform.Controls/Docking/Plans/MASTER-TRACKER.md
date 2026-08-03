# Docking — enhancement program: master tracker

`TheTechIdea.Beep.Winform.Controls/Docking/` — 54 files, 14,435 lines, 21 subfolders.

A capable docking system already: drop guides, auto-hide (145 references), floating windows, a
navigator, layout validation, and persistence. This program is about closing the distance to what
users now expect from Visual Studio, JetBrains Rider, VS Code, Figma and the current web docking
libraries (Dockview, Golden Layout, FlexLayout, rc-dock).

## Capability audit

Measured, not assumed — each figure is a reference count across the folder.

| capability | today | assessment |
|---|---|---|
| Auto-hide / fly-out | 145 refs | **strong** |
| Drop guides overlay | 9 refs | present, worth modernising |
| Floating windows | `FloatWindow.cs`, 372 lines | present, single-monitor |
| Layout persistence | 2 refs | thin — **hardened**, see [07](07-persistence-and-migration.md) |
| **Split editor groups** | present | **the audit was wrong** — see [01](01-split-editor-groups.md) |
| **Multi-monitor awareness** | **0** (`Screen.AllScreens`) | absent — **built**, see [03](03-multi-monitor-floating.md) |
| **Layout perspectives / presets** | **0** | absent — **built**, see [02](02-layout-perspectives.md) |
| **Panel maximise / zen** | 1 ref | absent — **built**, see [04](04-maximise-and-zen.md) |
| **Accessibility** | **0** | absent — **built**, see [06](06-keyboard-and-accessibility.md) |

## Features

| # | Feature | Reference products | Status |
|---|---|---|---|
| [01](01-split-editor-groups.md) | Split editor groups | VS Code, Dockview, Rider | ☑ **done** — command API, minimum group size, overlap validation, validator wired in, per-group tab strips verified |
| [02](02-layout-perspectives.md) | Named layout perspectives | Rider layouts, Blender workspaces, VS | ◐ **partial** — model, commands, Ctrl+Alt+1..9, capture-before-switch; picker UI open |
| [03](03-multi-monitor-floating.md) | Multi-monitor floating windows | VS, Rider, Figma | ◐ **partial** — injectable monitor set, device-name restore with clamping, DPI scaling; mixed-DPI unverified, display-edge snapping open |
| [04](04-maximise-and-zen.md) | Panel maximise and zen mode | VS Code `Ctrl+K Z`, Rider `Shift+Esc` | ☑ **done** — transient maximise, tree never mutated; input wiring deferred to 06 |
| [05](05-drop-guides-and-preview.md) | Modern drop guides and preview | Dockview, Golden Layout | ◐ **partial** — result preview, Esc-cancel and tab-index drop done; hover animation and group-level rosette open |
| [06](06-keyboard-and-accessibility.md) | Keyboard docking and a11y | VS Code, WCAG 2.2 | ◐ **partial** — accessible tree, command set, focus ring; high-contrast open |
| [07](07-persistence-and-migration.md) | Layout persistence and migration | VS `.suo`, Rider `layouts` | ◐ **partial** — schema v2, version guard, hidden panels, no more stranding; 02/03 state pending |
| [08](08-manager-decomposition.md) | Decompose `BeepDockingManager` | — | ◐ **partial** — 3,317 → 1,947 across 16 partials; all identified seams taken, core operations remain |
| [09](09-dead-surface.md) | Dead and duplicated surface | — | ☑ **done** |
| [10](10-verification-harness.md) | Verification harness | — | ☑ **done** — capture primitive + ground rules; found a hide/show defect on first run, now fixed |

Features 01–07 add capability. 08–09 are the structural work that makes them safe to build.
10 is how any of it is known to work.

## Suggested order

**09 → 08 → 10** first: remove the dead surface, break up a 3,317-line manager, and get a harness in
place before adding features to it. Then **05 → 01 → 04** (the visible interaction wins), then
**02 → 03 → 07** (layout lifecycle), with **06** threaded through — accessibility retrofitted last is
accessibility done twice.

## Ground rules

Carried from the Tabs, ToolTips, DialogsManagers, DisplayContainers and Filtering programs:

- **No stubs, no legacy, no fallback.** One implementation per concept.
- **No swallowed exceptions.** Absorb only where a failure must not propagate, and report it.
- **No duplication.**
- **Measure before claiming.** Every visual assertion needs a controlled baseline.
- **Implement or remove.** A browsable property that does nothing is the defect, not a placeholder.

## Hazards specific to this codebase

Each of these produced a wrong finding in an earlier program. They are listed because this folder is
larger than any of them and every one applies.

1. **Count within the right boundary.** `Models/DockingEnums.cs` declares **eleven** enums in one
   file. A `grep` for values spans all of them. This exact mistake produced two false findings in the
   Filtering program.
2. **A name can collide across subsystems.** `SupportsAnimations` appeared to have a caller; it was a
   different property of the same name on another painter family. Scope searches to files that
   reference the type you mean.
3. **Deletion plus a clean compile is the authoritative deadness test.** Grep is not: receiver-less
   internal calls are invisible to `\.Method(`, and API used only by a sibling repository is
   invisible to a search of this one.
4. **Sweep every sibling repo before removing public surface.** Removing four `[Obsolete]` aliases
   after checking only this repository broke nine call sites in `Beep.Winform.Data.Integrated`.
5. **`timeout` truncates a cross-repo grep into a false "no consumers".** Scope per repository.
6. **Complete features are commonly missing only their last wire.** Five instances so far — a
   container nothing instantiated, `tab.IsModified` nothing forwarded, a focus ring nothing enabled,
   `FilterPosition` nothing read, `FocusedFilterIndex` nothing set. Search for *callers*, not names.
