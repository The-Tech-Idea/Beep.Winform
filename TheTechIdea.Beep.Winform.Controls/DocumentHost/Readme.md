# BeepDocumentHost — Commercial-Grade MDI / Tabbed Document Host

> **Version** 2.0 · **Updated** 2026-03-01 (Sprint 18)
> Compared feature-set: DevExpress XtraTab/DocumentManager · Telerik RadDock · Syncfusion TabControlAdv · VS Code workspace model · Chrome tab behavior

---

## Overview

`BeepDocumentHost` is a **fully-featured tabbed-document / simulated-MDI container** for
WinForms applications built on the Beep framework. Drop it onto any surface — including
`BeepiFormPro` — to gain:

| Capability | Description |
|---|---|
| **8 tab visual styles** | Chrome, VS Code, Underline, Pill, Flat, Rounded, Trapezoid, Office |
| **Split-view groups** | Horizontal/vertical split with draggable splitter bar |
| **Auto-hide strips** | Tool-window-style collapsible panels on any of the 4 sides |
| **Float & dock-back** | Tear-off document windows with drop-target overlay |
| **Cross-host drag** | Transfer documents between any two `BeepDocumentHost` instances |
| **MRU navigation** | Ctrl+Tab switcher, most-recently-used order tracking |
| **Closed-tab history** | Ctrl+Shift+T reopen (configurable depth) |
| **Keyboard shortcuts** | Ctrl+W/F4/Tab/PgUp/PgDn/1–9, fully configurable |
| **Layout persistence** | JSON save/restore with schema versioning |
| **MVVM & data binding** | `IDocumentViewModel` integration |
| **Rich tooltips** | Thumbnail + metadata hover panels |
| **Quick-switch** | Ctrl+Tab popup with live titles and icons |
| **Badges** | Per-tab notification badges |
| **Tab colouring** | AccentBar / FullBackground / BottomBorder / None modes |
| **Designer support** | Smart-tag actions, verbs, property grid integration |
| **Accessibility** | Custom `AccessibleObject` per tab, keyboard narration |

> **Architecture note**: All controls here inherit from `Control` or `Panel`,
> **not** from `BaseControl`. This is intentional — `BaseControl` inherits
> `ContainerControl` which conflicts with `BeepiFormPro`'s WndProc/WM_ERASEBKGND handling.

---

## Classes

### `BeepDocumentTab` *(data model only — never a WinForms Control)*
| Property | Description |
|---|---|
| `Id` | Unique string that ties the tab to its panel. |
---

## File Map

| File | Purpose |
|---|---|
| `BeepDocumentTab.cs` | Enumerations + `BeepDocumentTab` data model |
| `BeepDocumentPanel.cs` | Content panel for one document |
| `BeepDocumentTabStrip.cs` | Main constructor + timer + handle init |
| `BeepDocumentTabStrip.Properties.cs` | All designable properties + public events |
| `BeepDocumentTabStrip.Painting.cs` | `OnPaint` pipeline + 8 tab style renderers |
| `BeepDocumentTabStrip.Layout.cs` | Tab geometry, DPI, scroll, pinned-tab boundary |
| `BeepDocumentTabStrip.Mouse.cs` | Click, hover, drag-reorder, drag-to-float |
| `BeepDocumentTabStrip.Keyboard.cs` | Ctrl+Tab, Ctrl+W, Ctrl+1–9, focus |
| `BeepDocumentTabStrip.Overflow.cs` | Overflow ▾ dropdown + popup list |
| `BeepDocumentTabStrip.ContextMenu.cs` | Right-click context menu |
| `BeepDocumentTabStrip.Animations.cs` | Active-indicator slide + close-button fade |
| `BeepDocumentTabStrip.Badges.cs` | Per-tab notification badge rendering |
| `BeepDocumentTabStrip.Accessibility.cs` | `AccessibleObject` per tab |
| `BeepDocumentHost.cs` | Core partial: constructor, fields, handle created |
| `BeepDocumentHost.Properties.cs` | Designable properties + events |
| `BeepDocumentHost.Documents.cs` | AddDocument, SetActive, Close, Float, DockBack |
| `BeepDocumentHost.Layout.cs` | RecalculateLayout, splitter bar, group geometry |
| `BeepDocumentHost.Events.cs` | Tab-strip event handlers + float window class |
| `BeepDocumentHost.AutoHide.cs` | Auto-hide strips + slide overlay |
| `BeepDocumentHost.Serialisation.cs` | SaveLayout / RestoreLayout (JSON, SchemaVersion 2 — full layout) |
| `Layout/ILayoutNode.cs` | Hierarchical layout tree interfaces and visitor pattern |
| `Layout/SplitLayoutNode.cs` | Binary split node (orientation + ratio + two children) |
| `Layout/GroupLayoutNode.cs` | Leaf group node (document IDs + selected doc) |
| `Layout/LayoutMigrationService.cs` | Migrate saved JSON v0/v1 → v2; `LayoutRestoreReport` |
| `BeepDocumentHost.Preview.cs` | Tab thumbnail snapshot / rich tooltip |
| `BeepDocumentHost.MVVM.cs` | `IDocumentViewModel` binding |
| `BeepDocumentHost.DataBinding.cs` | WinForms data-source integration |
| `BeepDocumentGroup.cs` | One tab-strip + content area within a split |
| `BeepDocumentDockOverlay.cs` | Translucent docking-guide overlay |
| `BeepDocumentDragData.cs` | Drag state shared between hosts |
| `BeepDocumentQuickSwitch.cs` | Ctrl+Tab MRU popup |
| `BeepDocumentRichTooltip.cs` | Themed hover tooltip with thumbnail |
| `DocumentDescriptor.cs` | DTO for MVVM binding + layout save/restore; `CustomData`, `AccentColor`, `DocumentInitialContent` |
| `Tokens/DocumentHostTokens.cs` | Central design tokens (geometry, timing, breakpoints) |
| `IDocumentViewModel.cs` | MVVM interface |

---

## Class Reference

### `BeepDocumentTab` — data model (not a Control)

| Property | Type | Description |
|---|---|---|
| `Id` | `string` | Stable unique key that ties the tab to its panel. |
| `Title` | `string` | Text displayed on the tab face. |
| `IconPath` | `string?` | Path to a 16 px icon (same convention as `BeepButton`). |
| `IsModified` | `bool` | Shows a dirty `●` indicator; sourced from panel's `IsModified`. |
| `CanClose` | `bool` | When false the × button is hidden. |
| `IsPinned` | `bool` | Collapses the tab to icon-only width on the left. |
| `IsActive` | `bool` | True for the currently visible tab. |
| `AccentColor` | `Color` | Per-tab accent bar / background tint color. |
| `BadgeText` | `string?` | Short text drawn in a badge overlay (e.g. `"3"`, `"M"`). |
| `BadgeColor` | `Color` | Background of the badge bubble. |
| `TabRect` | `Rectangle` | Hit-test rect (set by layout). |
| `CloseRect` | `Rectangle` | Hit-test rect for the × button. |
| `TabColor` | `Color` | Per-document color used by `TabColorMode`. |
| `DocumentCategory` | `string` | Optional category name (used by overflow dropdown grouping). |

---

### `BeepDocumentPanel : Panel`

Content area for a single document.  `AutoScroll = true` by default.

| Property | Type | Default | Description |
|---|---|---|---|
| `DocumentId` | `string` | — | Matches its `BeepDocumentTab.Id`. |
| `DocumentTitle` | `string` | — | Synced to the tab strip automatically. |
| `IsModified` | `bool` | `false` | Dirty-dot indicator; fires `ModifiedChanged`. |
| `CanClose` | `bool` | `true` | Propagated to the close button. |
| `IconPath` | `string?` | `null` | Icon shown in the tab. |
| `ShowStatusBar` | `bool` | `false` | 3 px themed status bar at the bottom. |
| `ThemeName` | `string` | inherited | Name of the active Beep theme. |

---

### `BeepDocumentTabStrip : Control`

Self-painted, double-buffered tab strip. Supports 8 visual styles, DPI scaling, animated active indicator, scrollable overflow, pinned tabs, badges, and drag operations.

#### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Tabs` | `IReadOnlyList<BeepDocumentTab>` | — | All registered tabs. |
| `ActiveTabIndex` | `int` | `-1` | 0-based index of the active tab. |
| `TabStyle` | `DocumentTabStyle` | `Chrome` | Controls how tabs are rendered. |
| `TabSizeMode` | `TabSizeMode` | `Equal` | `Equal / FitToContent / Compact / Fixed`. |
| `FixedTabWidth` | `int` | `120` | Used when `TabSizeMode = Fixed`. |
| `TabHeight` | `int` | `32` | Height of the strip in logical pixels. |
| `ShowAddButton` | `bool` | `true` | Renders the `+` button at the right end. |
| `CloseMode` | `TabCloseMode` | `OnHover` | `Always / OnHover / ActiveOnly / Never`. |
| `TabPosition` | `TabStripPosition` | `Top` | `Top / Bottom / Left / Right / Hidden`. |
| `ColorMode` | `TabColorMode` | `None` | `None / AccentBar / FullBackground / BottomBorder`. |
| `ScrollOffset` | `int` | `0` | Current horizontal scroll position. |
| `TabTooltipMode` | `TabTooltipMode` | `Simple` | `None / Simple / Rich` (thumbnail). |
| `AllowDragFloat` | `bool` | `true` | Enables drag-to-float when user drags vertically. |
| `AllowDragReorder` | `bool` | `true` | Enables horizontal drag-to-reorder. |
| `KeyboardShortcutsEnabled` | `bool` | `true` | Enables Ctrl+Tab/W/F4/1–9 shortcuts. |
| `ThemeName` | `string` | inherited | Beep theme applied to all painting. |

#### Events

| Event | Args | When |
|---|---|---|
| `TabSelected` | `TabEventArgs` | User activates a tab. |
| `TabCloseRequested` | `TabEventArgs` | × clicked (after `TabClosing` is not cancelled). |
| `TabClosing` | `TabClosingEventArgs` | Before close — set `Cancel = true` to abort. |
| `AddButtonClicked` | `EventArgs` | User clicks `+`. |
| `TabReordered` | `TabReorderArgs` | Drag reorder completes. |
| `TabFloatRequested` | `TabEventArgs` | Vertical drag-out or context menu Float. |
| `TabPinToggled` | `TabEventArgs` | Pin/unpin via context menu. |
| `NewDocumentRequested` | `EventArgs` | Double-click on empty strip area. |

#### Visual Styles

| Style | Appearance | Comparable product |
|---|---|---|
| `Chrome` | Rounded top corners, merges with content area bottom | Google Chrome, Edge |
| `VSCode` | Flat rect + top accent bar (theme `PrimaryColor`) | Visual Studio Code |
| `Underline` | No background; sliding bottom underline | Material UI, MUI Tabs |
| `Pill` | Active tab has a filled pill/capsule indicator | iOS segmented, Fluent UI |
| `Flat` | Flat rectangle, coloured top border only | Windows Explorer |
| `Rounded` | Heavy pill — full border-radius all sides | Mobile-first / Tailwind |
| `Trapezoid` | Angled left/right edges | Legacy Chrome, Opera |
| `Office` | Wide, thin top accent, bold title | Microsoft Office ribbon |

---

### `BeepDocumentHost : Panel`  ← **the main control**

Orchestrates tab strips, content panels, groups, float windows, and auto-hide strips.

#### Core Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `ActiveDocumentId` | `string?` | `null` | Id of the currently visible document. |
| `ActivePanel` | `BeepDocumentPanel?` | `null` | Reference to the active panel. |
| `DocumentCount` | `int` | — | Count of docked documents. |
| `TabPosition` | `TabStripPosition` | `Top` | Where the tab strip sits. |
| `TabStyle` | `DocumentTabStyle` | `Chrome` | Tab visual style. |
| `CloseMode` | `TabCloseMode` | `OnHover` | Close button visibility. |
| `ShowAddButton` | `bool` | `true` | Show the `+` button. |
| `ControlStyle` | `DocumentHostStyle` | `Flat` | Host border: `Flat / Thin / Raised`. |
| `ThemeName` | `string` | global | Propagated to all child controls. |
| `TabColorMode` | `TabColorMode` | `None` | Per-tab color blending. |

#### Split-View Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `MaxGroups` | `int` | `4` | Maximum number of simultaneous document groups. |
| `SplitHorizontal` | `bool` | `true` | `true` = side-by-side, `false` = top+bottom. |
| `SplitRatio` | `float` | `0.5` | Fraction of host dimension for group[0]. |

#### Navigation Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `RecentDocuments` | `IReadOnlyList<string>` | — | MRU-ordered document ids. |
| `MaxRecentHistory` | `int` | `20` | Depth of MRU list. |
| `MaxClosedHistory` | `int` | `10` | Number of closed docs that can be reopened. |
| `KeyboardShortcutsEnabled` | `bool` | `true` | Enable/disable all keyboard shortcuts. |

#### Persistence Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `AutoSaveLayout` | `bool` | `false` | Save layout automatically to `SessionFile`. |
| `SessionFile` | `string` | `""` | File path for auto-save. |

---

## Public API Reference

### Document Management

```csharp
// Add a document (auto-generated id)
BeepDocumentPanel panel = host.AddDocument("My Document");
BeepDocumentPanel panel = host.AddDocument("My Document", iconPath: @"icons\file.png");

// Add with explicit id
BeepDocumentPanel panel = host.AddDocument("doc-001", "My Document");

// Activate
host.SetActiveDocument("doc-001");

// Close (fires DocumentClosing → DocumentClosed)
host.CloseDocument("doc-001");

// Close all
host.CloseAllDocuments();

// Float / dock back
host.FloatDocument("doc-001");
host.DockBackDocument("doc-001");

// Auto-hide (collapses into a side strip)
host.AutoHideDocument("doc-001", AutoHideSide.Left);
host.RestoreAutoHideDocument("doc-001");

// Retrieve
BeepDocumentPanel? p = host.GetPanel("doc-001");
bool exists = host.HasDocument("doc-001");
```

### Split / Group Management

```csharp
// Split the host and move a document to a new group
host.SplitDocumentHorizontal("doc-001");   // side-by-side
host.SplitDocumentVertical("doc-001");     // top + bottom

// Move a document between existing groups
host.MoveDocumentToGroup("doc-001", targetGroupId);

// Merge all secondary groups back into the primary
host.MergeAllGroups();
```

### Navigation

```csharp
// Jump by MRU position (0 = most recent)
host.SetActiveDocument(host.RecentDocuments[1]);

// Reopen last closed tab (same as Ctrl+Shift+T)
host.ReopenLastClosed();

// Get next/previous in order
host.ActivateNext();
host.ActivatePrevious();
```

### Layout Persistence

```csharp
// Save the current workspace state
string json = host.SaveLayout();
File.WriteAllText("session.json", json);

// Restore — fires LayoutRestoring so caller can re-open content
host.RestoreLayout(json);
```

### Tab Metadata

```csharp
// Mark tab as modified (dirty dot)
host.SetDocumentModified("doc-001", true);

// Change title
host.SetDocumentTitle("doc-001", "Untitled *");

// Programmatic pin/unpin
host.PinDocument("doc-001");
host.UnpinDocument("doc-001");

// Set per-tab color
host.SetTabColor("doc-001", Color.CornflowerBlue);

// Set badge
host.SetTabBadge("doc-001", "3", Color.Red);
```

---

## Events Reference

| Event | Args | Description |
|---|---|---|
| `ActiveDocumentChanged` | `DocumentEventArgs` | Active document changed. |
| `NewDocumentRequested` | `EventArgs` | `+` clicked — caller adds a document. |
| `DocumentClosing` | `TabClosingEventArgs` | Before close — set `Cancel = true` to abort. |
| `DocumentClosed` | `DocumentEventArgs` | After a document was removed. |
| `DocumentFloated` | `DocumentEventArgs` | Document moved into a float window. |
| `DocumentDocked` | `DocumentEventArgs` | Floated document docked back. |
| `DocumentPinChanged` | `DocumentEventArgs` | Tab was pinned or unpinned. |
| `DocumentDetaching` | `DocumentTransferEventArgs` | About to transfer to another host. |
| `LayoutSerialising` | `DocumentLayoutEventArgs` | Per-doc hook during `SaveLayout`. |
| `LayoutRestoring` | `DocumentLayoutEventArgs` | Per-doc hook during `RestoreLayout`. |

---

## Keyboard Shortcuts

| Shortcut | Action |
|---|---|
| `Ctrl+Tab` | Open MRU quick-switch popup |
| `Ctrl+Shift+Tab` | Cycle backward through tabs |
| `Ctrl+W` / `Ctrl+F4` | Close active document |
| `Ctrl+Shift+T` | Reopen last closed document |
| `Ctrl+1` … `Ctrl+9` | Jump to the *n*-th tab |
| `Ctrl+PgDn` | Activate next tab |
| `Ctrl+PgUp` | Activate previous tab |

All shortcuts are disabled when `KeyboardShortcutsEnabled = false`.

---

## Tab ContextMenu (right-click)

| Item | Action | Disabled when |
|---|---|---|
| Close | Close current tab | `CanClose = false` |
| Close All But This | Close every other docked tab | — |
| Close All to the Right | Close tabs right of focused | — |
| Close All | Close every docked tab | — |
| ─── | separator | — |
| Pin / Unpin | Toggle `IsPinned` | — |
| Float | Move to float window | `AllowDragFloat = false` |
| Copy Path | Copy `DocumentTitle` to clipboard | — |

Customise at runtime:
```csharp
host.TabContextMenuOpening += (s, e) =>
{
    e.Menu.Items.Add(new ToolStripMenuItem("Open in Explorer", null, openHandler));
};
```

---

## Split Views

Splitting creates a second `BeepDocumentGroup` alongside the primary one, separated by a draggable splitter:

```csharp
// Tab "readme.txt" will open side-by-side with the other tabs
host.SplitDocumentHorizontal("readme.txt");
```

- Dragging the splitter bar adjusts `SplitRatio` in real time.
- When the secondary group becomes empty, it collapses automatically.
- The current implementation supports flat (non-nested) group topology; up to `MaxGroups` groups total.

---

## Auto-Hide Panels

Documents collapsed to a side strip show a short icon/title button.
Hovering or clicking that button slides the document panel out as an overlay:

```csharp
host.AutoHideDocument("solution-explorer", AutoHideSide.Left);
```

The slide animation runs at ~60 FPS. To restore the document into the tab strip:

```csharp
host.RestoreAutoHideDocument("solution-explorer");
```

---

## Floating Windows

```csharp
host.FloatDocument("doc-001");   // creates BeepDocumentFloatWindow
```

The float window is a `BeepiFormPro`-derived form with the full Beep theme.
Closing the float window calls the normal `DocumentClosing` / `DocumentClosed`
pipeline. Docking back is available from the float window's title bar or via code.

---

## Cross-Host Drag / Transfer

Two `BeepDocumentHost` instances registered with `BeepDocumentDragManager` can
exchange documents by dragging a tab from one strip to another:

```csharp
BeepDocumentDragManager.Register(hostA);
BeepDocumentDragManager.Register(hostB);
```

Set `AllowDragBetweenHosts = true` on both hosts (default `true`).
The `DocumentDetaching` event on the source fires before transfer — set `Cancel = true` to block.

---

## Layout Persistence (JSON Schema v2)

`SaveLayout()` serialises the **full** layout state:
- Hierarchical layout tree (splits with orientation + ratio, tab groups, document order)
- Pin/modified state, icon paths, document ids per document
- Active document id per group
- Float window bounds and `WindowState`
- Auto-hide side per document
- MRU snapshot
- `customData` dictionary per tab (populated via the `LayoutSerialising` event)

**v1 payloads are automatically migrated** via `LayoutMigrationService` on restore —
no manual intervention required.

Restoring returns a `LayoutRestoreReport` with per-document granularity:

```csharp
host.LayoutRestoring += (s, e) =>
{
    // e.DocumentId, e.Title, e.IconPath, e.CustomData all available
    var file = openFileMap[e.DocumentId];
    var editor = new RichTextBox { Dock = DockStyle.Fill };
    editor.LoadFile(file);
    host.AddDocument(e.DocumentId, e.Title, e.IconPath, activate: false)
        .Controls.Add(editor);
};

// Detailed restore with diagnostics
var report = host.TryRestoreLayout(File.ReadAllText("session.json"), out _);
Console.WriteLine(report);      // "3 restored, 0 skipped, 0 failed (schema v2)"

// Or the simple bool form (backward-compatible)
bool ok = host.RestoreLayout(File.ReadAllText("session.json"));
```

**Attaching custom data** during save:

```csharp
host.LayoutSerialising += (s, e) =>
{
    // attaches data that will survive the round-trip
    e.CustomData["filePath"] = openFileMap[e.DocumentId];
};
```

---

## MVVM / IDocumentViewModel

Implement `IDocumentViewModel` to bind a view-model class to a document panel:

```csharp
public class MyViewModel : IDocumentViewModel
{
    public string DocumentId { get; } = Guid.NewGuid().ToString();
    public string Title => "My Document";
    public string? IconPath => null;
    public bool IsModified => _isDirty;
    public bool CanClose => !IsLocked;
    public Control CreateView() => new MyEditorControl();
}

host.AddViewModelDocument(new MyViewModel());
```

---

## Designer (Visual Studio) Integration

`BeepDocumentHostDesigner` (in `Design.Server`) provides a full commercial-grade
design-time experience:

### Verbs (right-click context menu)
| Verb | Action |
|---|---|
| **Add Document** | Adds a placeholder document tab at design time |
| **Close Active Document** | Removes the currently selected tab |
| **Split Horizontal ↔** | Moves active document into a new side-by-side pane |
| **Split Vertical ↕** | Moves active document into a new stacked pane |
| **Merge All Groups** | Collapses all split groups back to one |
| **Edit Design-Time Documents…** | Opens the `DocumentDescriptorCollectionEditor` |
| **Apply Layout Preset…** | Opens the visual `LayoutPresetPickerDialog` |

### Smart-Tag (▶ panel)
All 7 groups are available in the smart-tag panel:

| Group | Properties / Actions |
|---|---|
| **Documents** | Add New, Close Active, Close All, Reopen Last, Quick Switch, Float, Pin/Unpin |
| **Design-Time** | Edit Design-Time Documents… |
| **Split View** | Split H/V, Merge All Groups, Apply Layout Preset…, MaxGroups, SplitRatio |
| **Tabs** | TabStyle, TabPosition, CloseMode, ShowAddButton, TabColorMode, KeyboardShortcuts |
| **Tab Sizing** | TabSizeMode (Auto/Fixed/Fill), FixedTabWidth |
| **Interaction** | TabTooltipMode, AllowDragFloat |
| **Style Presets** | Chrome / VS Code / Flat / Office / Pill / Underline (one-click) |
| **Appearance** | ControlStyle, ThemeName |
| **Preview** | TabPreviewEnabled |
| **History** | MaxRecentHistory, MaxClosedHistory |
| **Cross-Host Drag** | AllowDragBetweenHosts |
| **Session** | AutoSaveLayout, SessionFile |

### Additional design-time features
| Feature | Details |
|---|---|
| `DoDefaultAction()` | Double-click on the control surface auto-adds a document |
| `OnPaintAdornments()` | When the host has no documents, draws a centred hint overlay |
| `PreFilterEvents()` | Removes 20+ low-level plumbing events from the Events grid |
| `PreFilterProperties()` | Hides irrelevant Panel base properties |
| Snap lines | Content-area edges exposed for sibling alignment |
| `DesignTimeDocuments` | `Collection<DocumentDescriptor>` serialised into `InitializeComponent` |
| `ApplyDesignTimeDocuments()` | Applied automatically in `OnHandleCreated` |
| Undo/Redo | All verb and smart-tag mutations wrapped in `DesignerTransaction` |

### Layout Preset Picker
`LayoutPresetPickerDialog` (in `Design.Server/Designers/`) shows 5 visual tiles:
- **Single** — one tab group fills the area
- **Side-by-Side** — two groups split left/right (horizontal)
- **Stacked** — two groups split top/bottom (vertical)
- **Three-Way** — L-pattern (horizontal + vertical sub-split)
- **Four-Up** — 2×2 grid

### Document Collection Editor
`DesignTimeDocumentsEditor` (in `Design.Server/Editors/`) provides a tailored
`CollectionEditor` for `DocumentDescriptor` items:
- Sets `Id` to a GUID and `Title` to `"New Document"` on Add
- Dialog title: "Edit Design-Time Documents"
- Accessible via the "Edit Design-Time Documents…" smart-tag action

---

## Theming

All visual elements use the active Beep theme automatically:

| Theme token | Used in |
|---|---|
| `PanelBackColor` | Host background, content area, tab strip track |
| `PrimaryColor` | Active tab accent, VS Code overlay bar, focus rings |
| `BorderColor` | Strip border, splitter bar |
| `.ForeColor` | Tab title text |
| `CardBackColor` | Inactive / hover tab fills |

Set a theme globally:
```csharp
BeepThemesManager.SetTheme("BeepDark");
```
Or per-host:
```csharp
host.ThemeName = "BeepLight";
```

---

## Phase 2 Status

| Item | Sprint | Status |
|---|---|---|
| **Serialisation schema v2** — splits, floats, auto-hide, MRU, customData | 11 | ✅ Complete |
| **`LayoutMigrationService`** — v0/v1 → v2 auto-upgrade | 11 | ✅ Complete |
| **`TryRestoreLayout`** + `LayoutRestoreReport` diagnostics | 11 | ✅ Complete |
| **Designer transaction safety** — all verbs + action methods | 11 | ✅ Complete |
| **`DocumentHostTokens`** — central design token constants | 12 | ✅ Complete |
| **`ILayoutNode` tree** — `SplitLayoutNode` + `GroupLayoutNode` | 12 | ✅ Complete |
| **`DocumentDescriptor.CustomData`** — round-trips through JSON | 11 | ✅ Complete |
| **`DocumentDescriptor.AccentColor`** + `DocumentInitialContent` | 12 | ✅ Complete |
| **Proxy properties** — `TabSizeMode`, `FixedTabWidth`, `TabTooltipMode`, `AllowDragFloat` | 12 | ✅ Complete |
| **`DesignTimeDocuments` collection** — seeded at design time, serialised into `InitializeComponent` | 12 | ✅ Complete |
| **`DocumentDescriptorCollectionEditor`** — custom collection editor with GUID-seeded defaults | 12 | ✅ Complete |
| **`LayoutPresetPickerDialog`** — 5-tile visual layout preset picker | 12 | ✅ Complete |
| **Designer: 7 verbs** — Add, Close, Split H/V, Merge, Edit Docs, Layout Preset | 12 | ✅ Complete |
| **Designer: `DoDefaultAction()`** — double-click adds a document | 12 | ✅ Complete |
| **Designer: `OnPaintAdornments()`** — empty-state hint overlay | 12 | ✅ Complete |
| **Designer: `PreFilterEvents()`** — hides 20+ plumbing events | 12 | ✅ Complete |
| **Smart-tag: 12 groups** — Tab Sizing, Interaction, Design-Time, Layout Preset groups added | 12 | ✅ Complete |
| **`LayoutTreeBuilder`** — captures live group topology as `ILayoutNode` tree | 12 | ✅ Complete |
| **`LayoutTreeApplier`** — restores `ILayoutNode` tree onto a live host | 12 | ✅ Complete |
| **`MergeAllGroups()`** — collapses all split groups back to primary | 12 | ✅ Complete |
| **Docking guide adorners** — all 5 zones active + translucent preview rect | 12 | ✅ Complete |
| **Split-dock wiring** — Left/Right/Top/Bottom float-drop creates split panes | 12 | ✅ Complete |
| **`BeginBatchAddDocuments()`/`EndBatchAddDocuments()`** — single layout pass when adding 100+ docs | 13 | ✅ Complete |
| **`BeginBatchAdd()`/`EndBatchAdd()`** on tab strip — deferred `CalculateTabLayout` + `Invalidate` | 13 | ✅ Complete |
| **`_layoutSuspended` guard** in `RecalculateLayout` — no-op during batch add | 13 | ✅ Complete |
| **Lazy deferred snapshot** — `CaptureSnapshot` scheduled via `BeginInvoke` on tab switch | 13 | ✅ Complete |
| **`BeepTabAccessible.Name`** — "Tab N of M: Title [Modified] [Pinned]" screen-reader format | 13 | ✅ Complete |
| **`AccessibilityNotifyClients(Selection)`** — raised when active tab changes | 13 | ✅ Complete |
| **HC `DrawFocusIndicator`** — 3 px rounded rect in `SystemColors.Highlight` under high contrast | 13 | ✅ Complete |
| **`BeepDocumentTabStrip.HighContrast.cs`** — `ApplyHighContrastTheme()` + `EffectiveXxx` colour helpers | 13 | ✅ Complete |
| **Arrow / Home / End key navigation** — Left, Right, Up, Down, Home, End cycle through tabs | 13 | ✅ Complete |
| **`DocumentTabStyle.Fluent`** — new Fluent UI 2 / Windows 11 tab style (translucent fill + 4 px bottom accent) | 14 | ✅ Complete |
| **Token-driven accent bar** — `DrawAccentBar` uses `DocTokens.IndicatorThickness` (3 px) | 14 | ✅ Complete |
| **Inactive tab text alpha** — 70 % opacity via `DocTokens.InactiveTabTextAlpha` in `DrawTabContent` | 14 | ✅ Complete |
| **Chrome hover** — lighter `PanelBackColor + 8 % White` fill (Figma 2026 spec) | 14 | ✅ Complete |
| **Pill style MD3** — active = full `PrimaryColor`; text = `OnPrimaryColor`; inactive hover = `PrimaryColor@15%` | 14 | ✅ Complete |
| **`BeepTabOverflowPopup`** — searchable (fuzzy score), keyboard-nav ↑↓/Enter/Esc, category groups, pinned section | 14 | ✅ Complete |
| **`BeepDocumentPanel` tri-segment status bar** — left / centre / right text segments, `SetStatusLeft/Centre/Right()`, 22 px, lightness-shifted `PrimaryColor` background | 15 | ✅ Complete |
| **Group-collapse animation** — 200 ms ease-out cubic splitter animation when secondary group empties; `CollapseEmptyGroupAnimated` + `CollapseEmptyGroupImmediate` in Layout.cs | 15 | ✅ Complete |
| **Rich tooltip enhancements** — category badge, modified `●` dot, rounded thumbnail corners (`DocTokens.ThumbnailCornerRadius`), footer hint `"Ctrl+W to close · Ctrl+click to pin"`, default delay → `DocTokens.TooltipDelayMs` (800 ms) | 15 | ✅ Complete |
| **Empty-state illustration** — centred document icon + "No open documents" headline + sub-text painted via `_contentArea.Paint`; `ShowEmptyState` property to suppress | 15 | ✅ Complete |
| **`IDocumentHostCommandService`** — interface: `NewDocument`, `CloseDocument`, `CloseAllDocuments`, `ActivateDocument`, `FloatDocument`, `DockBackDocument`, `Pin/Unpin`, `SplitH/V`, `SaveLayout`, `RestoreLayoutAsync` | 16 | ✅ Complete |
| **`DocumentHostCommandService`** — concrete wrapper; `BeepDocumentHost.CommandService` lazy property; `RestoreLayoutAsync` marshals result back to UI thread | 16 | ✅ Complete |
| **`CloseAllDocuments()`** — closes every open document, returns `true` when ≥1 was closed | 16 | ✅ Complete |
| **`DocumentsSource` (INCC)** — `IEnumerable` + `INotifyCollectionChanged` source binding with `DocumentIdSelector`, `DocumentTitleSelector`, `DocumentTemplate` selectors (OpenVm / CloseVm on collection events) | 16 | ✅ Complete |
| **`BeepDocumentHostOptions`** — options class with `DefaultTabStyle`, `MaxGroups`, `KeyboardShortcutsEnabled`, `CloseMode`, `ShowAddButton`, `AutoSaveLayout`, `SessionFile`; `ApplyTo(host)` | 16 | ✅ Complete |
| **`BeepDocumentHostExtensions.Configure()`** — fluent `host.Configure(o => …)` + `ApplyOptions(options)` + DI wiring guide in XML-doc | 16 | ✅ Complete |
| **`DocumentDescriptorCollectionEditor` v2** — custom form with DataGridView (Id / Title / IconPath / IsPinned / CanClose / InitialContent / AccentColor), icon picker via `IconPickerDialog`, up/down reorder, live mini tab-strip preview | 17 | ✅ Complete |
| **`ThemePickerDialog`** — scrollable tile grid with colour swatches, search filter, dark/light badge; integrates with `BeepThemesManager.GetThemes()` + 15-theme fallback | 17 | ✅ Complete |
| **`ChooseTheme…` smart-tag action** — "Choose Theme…" `DesignerActionMethodItem` in the Appearance group; opens `ThemePickerDialog` and writes the selected name to `ThemeName` | 17 | ✅ Complete |
| **Docking guide adorner v2** — 5-point compass (Center / Left / Right / Top / Bottom) painted in `OnPaintAdornments` during drag; active zone highlighted in `PrimaryColor`; on-drop calls `SplitDocumentHorizontal/Vertical` inside a designer transaction | 17 | ✅ Complete |
| **`TabDensityMode` enum** — `Comfortable` (36 px/12 pt), `Compact` (28 px/11 pt), `Dense` (22 px/10 pt) | 18 | ✅ Complete |
| **`TabDensity` property** — changing density updates `Height` + calls `ApplyDensityFont()`; density-aware `TabHeight` computed from `LogicalDensityHeight` | 18 | ✅ Complete |
| **`ResponsiveBreakpoints` + `TabResponsiveMode`** — breakpoints at 480 / 240 / 120 px; `Normal` → `Compact` (title-only) → `IconOnly` → `ActiveOnly`; mode stored in `_responsiveMode` updated in `CalculateTabLayout` | 18 | ✅ Complete |
| **Responsive paint guards** — `ActiveOnly` skips non-active tabs in both paint loops; `Compact` skips icons; `IconOnly`/`ActiveOnly` skip titles (except active tab) via `drawIcon`/`drawTitle` flags in `DrawTabContent` | 18 | ✅ Complete |
| **`BeepDocumentTabStrip.Touch.cs`** — WM_TOUCH registration via `RegisterTouchWindow`; `WndProc` handler; swipe left/right scrolls strip; long-press (600 ms) opens context menu; pinch cycles `TabSizeMode`; swipe-down fires `TabFloatRequested` | 18 | ✅ Complete |
| Automated lifecycle + serialisation tests | 16+ | Planned |
| Visual design system alignment (Figma / Fluent UI 2) | 16+ | Planned |

See [`plans/mdi-phase2-enhancement-plan.md`](../plans/mdi-phase2-enhancement-plan.md) for the full sprint plan (Sprints 11–20).

---

## Quick-Start

```csharp
// On a BeepiFormPro or any Panel-based form:
var host = new BeepDocumentHost
{
    Dock             = DockStyle.Fill,
    TabPosition      = TabStripPosition.Top,
    TabStyle         = DocumentTabStyle.Chrome,
    CloseMode        = TabCloseMode.OnHover,
    ShowAddButton    = true,
    ControlStyle     = DocumentHostStyle.Flat
};
this.Controls.Add(host);

// New document via + button
host.NewDocumentRequested += (s, e) =>
{
    var panel = host.AddDocument($"Document {host.DocumentCount + 1}");
    panel.Controls.Add(new RichTextBox { Dock = DockStyle.Fill });
};

// Pre-seed a welcome tab
var welcome = host.AddDocument("Welcome", @"icons\home.png");
welcome.Controls.Add(new Label
{
    Text      = "Welcome to Beep Document Host!",
    Dock      = DockStyle.Fill,
    TextAlign = ContentAlignment.MiddleCenter
});

// Cross-host drag support
BeepDocumentDragManager.Register(host);

// Layout persistence
string layout = host.SaveLayout();                              // save
host.RestoreLayout(File.ReadAllText("session.json"));           // restore

// Split view
host.SplitDocumentHorizontal("Welcome");

// Auto-hide
host.AutoHideDocument("Welcome", AutoHideSide.Left);
```

---

## Architecture Decisions

| Decision | Rationale |
|---|---|
| Inherits `Panel`, not `BaseControl` | Avoids `ContainerControl` focus/erase conflicts with `BeepiFormPro` |
| Documents keyed by string `Id` | Stable across serialise/restore and cross-host transfer |
| Painter-style tab rendering (8 styles) | Allows new styles without touching interaction code |
| Groups as first-class objects | Clean split-view without nested Control trees |
| Splitter as a thin Panel, not `SplitContainer` | Full control over position, cursor, and theme colour |
| JSON via `System.Text.Json` | No extra NuGet dependency; ships with .NET 6+ |
| Timer-driven indicator animation | Smooth 60-FPS slide without relying on WPF/XAML |

| `Control` base for TabStrip | Avoids `ContainerControl` focus-traversal and `WM_ERASEBKGND` conflicts |
| `Panel` base for panels/host | Lightweight container; `AutoScroll` and `DoubleBuffered` built-in |
| `BeepiFormPro` for float window | Full themed title bar and min/max buttons on detached docs |
| `_theme` field + `BeepThemesManager.ThemeChanged` | Pattern from `TemplateUserControl`; no dependency on BaseControl |
| One panel per document | Simple show/hide swap; no z-order tricks required |

---

## Files

| File | Purpose |
|---|---|
| `BeepDocumentTab.cs` | Data model + enums + event args |
| `BeepDocumentTabStrip.cs` | Self-painted `Control`-derived tab strip |
| `BeepDocumentPanel.cs` | `Panel`-derived content area per document |
| `BeepDocumentHost.cs` | Orchestration host + `BeepDocumentFloatWindow` |
| `Readme.md` | This file |

---

*Last updated: 2025 — Beep.Winform DocumentHost v1.0*
