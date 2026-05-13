# BeepDocumentHost Current-State Audit

*Last updated: May 2026 — full re-audit after code review of all Properties, AutoHide, Drag, Keyboard, and Descriptor files*

## Audit Scope

This audit covers all 105 C# source files in `DocumentHost/` including:

- 12 core host partials (main, properties, documents, layout, serialisation, auto-hide,
  events, keyboard, MVVM, preview, workspace, templates, data-binding)
- 12 tab-strip partials (core, layout, painting, mouse, keyboard, animations, overflow,
  badges, context-menu, accessibility, touch, high-contrast)
- Layout tree subsystem (10 files in `Layout/`)
- 13 painter classes (`Painters/`)
- 9 feature stubs (`Features/`)
- 5 helpers (`Helpers/`)
- 5 cloud/undo-redo files (`Cloud/`)
- 4 template files (`Templates/`)
- Supporting: tokens, command system, workspace, identifiers

It is also informed by:
- DockPanel Suite (GitHub) — most comparable WinForms docking library
- Krypton Docking (GitHub, ComponentFactory) — WinForms, drag-and-drop like Visual Studio
- AvalonDock (GitHub, Dirkster99) — WPF reference for design patterns
- DevExpress DockManager + DocumentManager — commercial design-time standard
- Syncfusion DockingManager — commercial reference

## Current Readiness

**Estimated: 91–93% commercial-grade runtime / 82% design-time UX**

*Upgraded from 85–88% / 60–65% after Track G work was completed.*

The Properties window, policy properties, window-menu builder, auto-hide hover delay,
PersistenceKey, and undo/redo are now all fully implemented.  The remaining gaps are
polish — drag UX quality, smart-tag inline pickers, designer drop-target, a few more
keyboard shortcuts, and a representative sample form.

---

## What Is Already Fully Implemented ✅

### Tab Strip & Painters
- 9 visual styles: Chrome, VSCode, Underline, Pill, Flat, Rounded, Trapezoid, Office, Fluent
- Tab overflow (popup list), MRU tab order, badge pulse animation, dirty indicator (●)
- Preview thumbnails — LRU cache (max 50), eager/lazy capture, configurable size
- Quick-switch popup (Ctrl+Shift+P / Ctrl+P modes)
- Touch gestures: swipe scroll, long-press context menu, pinch cycle size, swipe-down float
- High-contrast mode, full accessibility (IAccessible, per-tab AccessibleObject)
- Badge pulse, animated sliding active-tab indicator, tab open/close width-morph animation
- Drag-to-reorder (insert-mode cursor), drag-to-float with ghost window
- Rich tooltip popup with thumbnail

### Layout & Groups
- Binary split tree: `ILayoutNode`, `GroupLayoutNode`, `SplitLayoutNode`
- `LayoutTreeBuilder`, Validator (8 rules), Repairer (4 strategies), Applier, Visitor
- Multi-group: up to 4 simultaneous groups (configurable via `MaxSplitDepth`)
- Splitter bars, ratio persistence across resize and serialise/restore cycles
- Cross-host drag transfer (`BeepDocumentDragManager`, `DocumentTransferEventArgs`)
- 5-zone dock compass overlay (`BeepDocumentDockOverlay`) with animated zone highlights

### Auto-Hide
- 4-side strips (Left, Right, Top, Bottom), created lazily
- Slide-out overlay with smooth ease-out animation (16 ms timer, 8 steps, quadratic)
- **Hover-trigger delay** — `AutoHideHoverDelay` property (default 400 ms); 0 = click-only
- **Remember last size** — `_ahLastSize` dictionary restores user-resized width/height
- Escape key to close flyout; `RestoreAutoHideDocument()` for code-driven restore
- `PreviousGroupId` stored on auto-hide so restore re-docks to source group

### Properties Window — Fully Categorized ✅ (was critical gap, now resolved)
All public properties carry `[Category]`, `[Description]`, and `[DefaultValue]`:

| Category | Properties |
|----------|-----------|
| `"Document – Appearance"` | TabStyle, TabPosition, TabColorMode, ControlStyle, ThemeName |
| `"Document – Behavior"` | CloseMode, ShowAddButton, KeyboardShortcutsEnabled, MaxActivePanels, ShowBreadcrumb, EnableRoutedCommands, EnableTransactionalDocking, EnableHostTelemetry |
| `"Document – Layout"` | ShowBreadcrumb, ShowEmptyState, ShowStatusBar |
| `"Document – Policies"` | AllowFloat, AllowSplit, AllowPin, AllowAutoHide, MaxSplitDepth, CloseButtonMode, ShowPinButton, ShowMaximizeButton |
| `"Document – Persistence"` | AutoSaveLayout, SessionFile, TrackLayoutHistory |
| `"Document – Animation"` | AutoHideHoverDelay |

### Global Policy Properties ✅ (was high-priority gap, now resolved)
All properties exist with full wiring into the tab strip and validation helpers:
- `AllowFloat` — also gates `AllowDragFloat` on the strip
- `AllowSplit`, `AllowPin`, `AllowAutoHide`, `MaxSplitDepth`
- `CloseButtonMode` (enum: Always/OnHover/ActiveOnly/Never/Hidden) — maps to `TabCloseMode`
- `ShowPinButton`, `ShowMaximizeButton`
- Internal helpers: `CanFloatNow()`, `CanSplitNow(depth)`, `CanPinNow()`, `CanAutoHideNow()`

### Window Menu Builder ✅ (was high-priority gap, now resolved)
- `PopulateWindowMenu(ToolStripMenuItem)` — wires DropDownOpening to rebuild the menu live
- `AttachWindowMenu(MenuStrip, text)` — finds or creates the Window item automatically
- Menu contents: New Horizontal/Vertical Tab Group, Move to Next/Previous Tab Group,
  Close All, separator, numbered list of open documents with active indicator (✓)

### Persistence & Identity ✅ (was medium gap, now resolved)
- `DocumentDescriptor.PersistenceKey` — stable GUID, set once on creation, survives renames
- `DocumentDescriptor.PreviousGroupId` — re-dock hint for intelligent restore
- JSON schema v2, `LayoutMigrationService` (v1→v2), `LayoutRestoreReport` diagnostics
- `BeepLayoutHistory` ring-buffer (20 entries), `PushLayoutVersion()`, file persistence
- `BeepLayoutUndoRedo` — `Push/Undo/Redo` fully wired; Ctrl+Z/Y routes to `UndoLayout/RedoLayout`

### Command System
- `BeepCommandRegistry` (fuzzy search, usage tracking, 30+ built-in bindings)
- Chord system: Ctrl+K prefix + 14 second-key sequences
- Command palette (Ctrl+Shift+P = commands mode, Ctrl+P = go-to-file mode)
- Workspace switcher popup
- Shortcut help popup (Ctrl+K Ctrl+H)

### Designer Foundation
- `BeepDocumentHostDesigner` (ParentControlDesigner subclass)
- 5-zone compass with live split preview
- Layout assistant dialog, text prompt dialog
- Smart-tag ActionList with method items
- `DesignTimeLayoutJson` hidden property — persists seed state across designer sessions

---

## Remaining Gaps — Ranked by User Impact

### 🟠 HIGH: Smart-Tag Lacks Inline Property Pickers (Track G2 — not done)

DevExpress and Krypton smart-tags use `DesignerActionPropertyItem` so users can pick
TabStyle, TabPosition, and CloseMode directly from the smart-tag panel via inline
drop-downs — no dialog required.

**Current state:** `DocumentHostActionList` has method items (dialogs) but no
`DesignerActionPropertyItem` for the top 5 properties.

**Fix:** Add `DesignerActionPropertyItem` for TabStyle, TabPosition, CloseMode,
ShowAddButton, KeyboardShortcutsEnabled. Add quick-action methods: "Add Document",
"Clear All Documents", "Save Layout Snapshot to Clipboard".

---

### 🟠 HIGH: No Drop-Onto-Document in Designer (Track G3 — not done)

DevExpress: drag any UserControl from the Toolbox onto a DockPanel in the designer —
it becomes the panel's content. Krypton: same pattern.

**Current state:** `BeepDocumentHostDesigner` has no `CanParent()` / `OnDragDrop()`
override.  Dropping a control onto the host in the designer does nothing useful.

**Fix:** Override in `BeepDocumentHostDesigner`:
```csharp
public override bool CanParent(Control parent, Type controlType) => true;
```
And `OnDragDrop`: route the dropped control into `_host.ActivePanel.Controls.Add(dropped)`.

---

### 🟡 MEDIUM: Drag Ghost Window Is Not Theme-Aware (Polish)

The float ghost window uses a hardcoded dark `Color.FromArgb(48, 54, 70)` background
and is sized at 140×28 px — looks like a tooltip, not a document ghost.

**Commercial reference:** DevExpress shows a 200×120 themed ghost with semi-transparent
tab strip header + document title. Krypton uses the page's foreground color.

**Gaps:**
1. Ghost color should use `_currentTheme.PanelBackColor` / `TabActiveBackColor`
2. Ghost should be ~200×36 px (or match the actual tab width)
3. No Escape key to cancel the drag-to-float
4. No dock-zone highlight pulse during float drag (overlay exists but not wired to activate on float drag start)

---

### 🟡 MEDIUM: No Animated Drop-Indicator Line Inside Target Group (Polish)

DevExpress: during drag-to-reorder, a 2 px animated indicator line appears between
tabs showing the exact insert position. Currently only the cursor changes.

**Fix:** In `BeepDocumentTabStrip.Painting.cs`, paint a 2px vertical insert-caret
at `_dragInsertIndex` position during drag.

---

### 🟡 MEDIUM: Additional Keyboard Shortcuts (Polish)

**Missing vs. DevExpress/Visual Studio:**
- `Ctrl+Alt+Left/Right` — move active tab to previous/next split group
- `Ctrl+Shift+W` — close all documents to the right of the active tab
- `Ctrl+Shift+M` — maximize/restore active document (full-screen panel)
- `F6` — cycle keyboard focus between split panes (though `Alt+Left/Right` exists for that)

---

### 🟡 MEDIUM: Designer Verb Expansion (Track G8 — not done)

Add DesignerVerbs to `BeepDocumentHostDesigner`:
- "Export Layout Snapshot…" → saves `DesignTimeLayoutJson` to a .json file
- "Clear All Documents" → calls `ClearDesignTimeDocuments()`
- "Customize Keyboard Shortcuts…" → opens the keybinding editor dialog

---

### 🟡 MEDIUM: Auto-Hide Flyout Has No Header (Polish)

The auto-hide overlay panel has no title/caption bar — it is just a plain Panel.
DevExpress and Krypton auto-hide flyouts have a header with:
- Document icon + title
- Pin button (dock back permanently)
- Close button

**Fix:** Add a 28 px header panel inside `_ahOverlay` with title label, theme-colored
background, and Pin/Close buttons. Pin calls `RestoreAutoHideDocument`.

---

### 🟡 MEDIUM: Auto-Collapse Overlay on Focus Loss (Polish)

When the auto-hide flyout is open and the user clicks somewhere outside it, there is no
auto-collapse. The overlay stays visible until another click.

**Fix:** In `ShowAhOverlay()`, subscribe to `_ahOverlay.Leave` / `Deactivate` and start
a short collapse timer (e.g. 500 ms debounce before `CloseAhOverlay(animate: true)`).

---

### 🟢 LOW: Tab Animation Easing Is Linear (Polish)

Tab open/close width-morph (`BeepDocumentTabStrip.Animations.cs`) uses a linear lerp.
The active indicator slide also lerps linearly.

**Fix:** Replace linear lerp with ease-in-out cubic `t*t*(3-2*t)` for width morphing
and ease-out `1-(1-t)^2` for indicator (currently already quadratic — verify).
Badge pulse frequency is not configurable (hard-coded).

---

### 🟢 LOW: Representative Sample MDI Form Needs Expansion

`MainFrm_MDI.cs` in `Beep.Winform.Default.Views` is a minimal constructor stub.
It should demonstrate the intended one-host MDI authoring pattern.

**Needed in the sample:**
- `BeepDocumentHost` on the form with 3 demo documents
- `AttachWindowMenu()` wired to a Window menu item
- `AutoSaveLayout = true`, `SessionFile` pointing to app-local path
- Comments showing how to add documents, respond to `ActiveDocumentChanged`
- Comments showing tab style enumeration and `CloseMode` usage

---

### 🟢 LOW: Feature Stubs (Post-1.0 — do not start)

- Cloud sync (`BeepCloudSyncManager`) — Azure Blob scaffold, intentionally stubbed
- Terminal (`TerminalPanel`, `TerminalHost`) — no shell integration
- Diff viewer (`DiffViewPanel`, `DocumentComparer`) — no content
- Git status (`BeepGitStatusProvider`) — no libgit2sharp
- MiniToolbar, StatusBar content — stubs only

---

## Full Status Table (Updated)

| Area | Status | Remaining Gap |
|------|--------|--------------|
| Tab-strip styles (9) | ✅ Complete | None |
| Drag-to-reorder | ✅ Complete | No painted insert-caret line |
| Drag-to-float ghost | ⚠️ Basic | Not theme-aware; no Escape cancel; no overlay wired |
| Auto-hide strips | ✅ Complete | No flyout header; no auto-collapse on focus loss |
| Auto-hide hover delay | ✅ Complete (400ms) | None |
| Auto-hide last size | ✅ Complete | None |
| Properties categorized | ✅ Complete | None |
| Policy properties | ✅ Complete | None |
| Window menu builder | ✅ Complete | None |
| Undo/Redo | ✅ Complete | None |
| PersistenceKey | ✅ Complete | None |
| PreviousGroupId | ✅ Complete | None |
| Smart-tag dialogs | ✅ Complete | No inline `DesignerActionPropertyItem` |
| Designer drop-onto | ❌ Missing | `CanParent`/`OnDragDrop` not overridden |
| Designer verbs | ⚠️ Basic | Export/Clear/Keybindings verbs missing |
| Keyboard shortcuts | ✅ Extensive | Ctrl+Alt+Arrow, Ctrl+Shift+W, Ctrl+Shift+M missing |
| Tab animations | ✅ Working | Easing curves not configurable |
| Dock compass overlay | ✅ Complete | Not activated on float-drag start |
| Sample MDI form | ❌ Stub | Needs expansion to demonstrate patterns |
| Cloud/Git/Terminal | ⚠️ Stubs | Post-1.0 intentionally |

## Recommended 1.2+ Focus

1. Breadcrumb and navigation history (surface exists, not integrated).
2. Status bar and mini-toolbar (stubs exist).
3. Cloud sync (scaffold exists).
4. Terminal, diff, git (post-core).

## Commercial Benchmark Comparison

| Feature | DevExpress | Krypton | DockPanel Suite | BeepDocumentHost |
|---------|-----------|---------|----------------|-----------------|
| Tab styles | 3+ | 4+ | 6 | ✅ 9 |
| Properties window categories | ✅ Full | ✅ Full | ✅ Full | ❌ None |
| Smart-tag inline pickers | ✅ Full | ✅ Partial | ✅ Partial | ⚠️ Dialogs only |
| Drop control onto panel in designer | ✅ Yes | ✅ Yes | ⚠️ Partial | ❌ No |
| Global policy (AllowFloat, etc.) | ✅ Full | ✅ Full | ✅ Full | ❌ None |
| Window menu builder | ✅ Auto | ✅ Helper | ✅ Helper | ❌ None |
| Float windows | ✅ Full | ✅ Full | ✅ Full | ✅ Full |
| Auto-hide (4 sides) | ✅ Full | ✅ Full | ✅ Full | ✅ Core done |
| Auto-hide hover-trigger | ✅ 400 ms | ✅ Configurable | ✅ Yes | ❌ Click-only |
| Stable persistence identity | ✅ PersistKey | ✅ UniqueName | ✅ PersistString | ❌ Title-based |
| Layout history / undo | ✅ Full | ✅ Partial | ❌ None | ⚠️ Stubs |
| Animated drag drop-indicator | ✅ Full | ✅ Full | ✅ Partial | ❌ None |
| Docking compass | ✅ Full | ✅ Full | ✅ Partial | ✅ 5-zone done |
| MVVM / data-binding | ✅ Full | ⚠️ Basic | ❌ None | ✅ Full |
| 9+ tab painters | ❌ 3-4 | ❌ 4 | ❌ 6 | ✅ 9 |
| Command palette (Ctrl+P) | ❌ None | ❌ None | ❌ None | ✅ Full |
| Cloud sync | ❌ None | ❌ None | ❌ None | ⚠️ Scaffold |