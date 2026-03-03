# BeepDocumentHost MDI Phase 2 Enhancement Plan

> **Created:** 2026-02-28  
> **Status:** In planning — Sprints 11–20  
> **Benchmarks:** DevExpress DocumentManager v24 · Telerik RadDock 2025 · Syncfusion TabControlAdv · VS Code 1.87 workspace model · Figma Component Design System · Material Design 3 · Fluent UI 2  
> **Prerequisites:** All Phase 1 sprints (1–10) complete.

---

## Phase 1 Completion Summary (Sprints 1–10)

All Sprints 1–10 delivered (see `enhancementplanforBeepDocument.md`).  
Current capability snapshot vs. commercial parity:

| Area | Status |
|---|---|
| 8 tab visual styles | ✅ Complete |
| Pinned tabs, drag-reorder, drag-to-float | ✅ Complete |
| Context menu (Close/Pin/Float/CopyPath) | ✅ Complete |
| Overflow dropdown with search | ✅ Complete |
| Keyboard shortcuts (Ctrl+Tab / W / F4 / 1–9) | ✅ Complete |
| MRU + closed-tab history + Ctrl+Shift+T | ✅ Complete |
| Tab badges, per-tab color modes | ✅ Complete |
| Split-view groups (horizontal/vertical, splitter drag) | ✅ Complete |
| Auto-hide side strips + slide overlay | ✅ Complete |
| Float windows + dock-back + docking overlay | ✅ Complete |
| Cross-host drag/transfer | ✅ Complete |
| Rich tooltips + thumbnail previews | ✅ Complete |
| Quick-switch Ctrl+Tab popup | ✅ Complete |
| MVVM + data binding | ✅ Complete |
| Layout serialisation (JSON schema v1 — tabs only) | ✅ Partial |
| Designer smart-tag + verbs | ✅ Partial |
| Accessibility (AccessibleObject per tab) | ✅ Partial |

---

## Gap Matrix Summary (Remaining Gaps → Phase 2 Targets)

| Gap | Severity | Sprint |
|---|---|---|
| Layout serialisation v2 (splits, float geometry, auto-hide state) | High | 11 |
| Layout migration service / schema forward-compat | High | 11 |
| Designer transaction-safety for all mutating actions | High | 11 |
| Designer serialisation to `InitializeComponent` — groups/descriptors | High | 12 |
| Nested docking layout tree (hierarchical splits) | High | 12 |
| Docking guide adorners (side-docking visual targets) | High | 12 |
| Performance at scale (100+ documents, drag latency) | Medium | 13 |
| Screen-reader narration + high-contrast compliance | Medium | 13 |
| Automated lifecycle/layout/serialisation tests | High | 11 |
| Visual Design System alignment (Figma tokens, corners, spacing) | Medium | 14 |
| Tab overflow: search-filter keyboard navigation | Medium | 14 |
| Document status bar (line/col, encoding, git branch) | Low | 15 |
| Tab thumbnail quality + lazy capture | Medium | 14 |
| Group collapse/expand animation | Low | 15 |

---

## Sprint 11 — Serialisation v2 + Designer Safety + Test Foundation

**Goal:** Close the two highest-severity gaps: complete layout persistence and make the designer safe.

### 11.1 Layout Schema v2

Extend `BeepDocumentHost.Serialisation.cs`:

#### v2 JSON payload structure
```json
{
  "SchemaVersion": 2,
  "ActiveDocumentId": "doc-001",
  "LayoutTree": {
    "Type": "Split",
    "Orientation": "Horizontal",
    "Ratio": 0.55,
    "Children": [
      {
        "Type": "TabGroup",
        "GroupId": "grp-1",
        "SelectedDocumentId": "doc-001",
        "Documents": [
          { "Id": "doc-001", "Title": "main.cs", "IconPath": "icons/cs.png",
            "IsPinned": false, "IsModified": true, "CustomData": {} }
        ]
      },
      {
        "Type": "TabGroup",
        "GroupId": "grp-2",
        "SelectedDocumentId": "doc-042",
        "Documents": [...]
      }
    ]
  },
  "FloatingWindows": [
    { "DocumentId": "doc-007", "Bounds": { "X": 500, "Y": 300, "W": 800, "H": 600 }, "WindowState": "Normal" }
  ],
  "AutoHideEntries": [
    { "DocumentId": "doc-010", "Side": "Left" }
  ],
  "MruSnapshot": ["doc-001", "doc-042", "doc-010"]
}
```

#### Deliverables
- `DocumentDescriptor.cs` — extend DTO with `CustomData` dictionary
- `LayoutTreeNode.cs` — discriminated union (`SplitNode` / `TabGroupNode`)
- `BeepDocumentHost.Serialisation.cs` — rewrite `SaveLayout` / `RestoreLayout` with v2 support
- `LayoutMigrationService.cs` — upgrade v1 → v2 automatically on restore
- Structured `LayoutRestoreReport` — lists skipped, failed, and restored nodes
- `TryRestoreLayout(string json, out LayoutRestoreReport report)` API

### 11.2 Designer Transaction Safety

All smart-tag and verb actions must be wrapped in `DesignerTransaction`:

```csharp
// Pattern for every action in DocumentHostActionList / BeepDocumentHostDesigner:
using var txn = _designerHost.CreateTransaction("Add Document");
try
{
    _changeService.OnComponentChanging(host, null);
    host.AddDocument("Design-time Document " + idx);
    _changeService.OnComponentChanged(host, null, null, null);
    txn.Commit();
}
catch
{
    txn.Cancel();
}
```

Target files:
- `Design.Server/ActionLists/DocumentHostActionList.cs`
- `Design.Server/Designers/BeepDocumentHostDesigner.cs`

All methods that call: `AddDocument`, `CloseDocument`, `FloatDocument`, `SplitDocumentHorizontal/Vertical`, `MergeAllGroups`, `AutoHideDocument`, `RestoreAutoHideDocument`.

### 11.3 Automated Test Suite

Create `tests/DocumentHost/`:

| Test class | Scope |
|---|---|
| `DocumentLifecycleTests` | Add / Activate / Close event ordering and state |
| `LayoutInvariantTests` | Split / move / merge / close always leave valid tree |
| `SerialisationRoundTripTests` | Save + restore for tabs, splits, float, auto-hide |
| `MruOrderingTests` | Activation order, max depth, reopen-last |
| `KeyboardShortcutTests` | Ctrl+W, Ctrl+Tab, Ctrl+Shift+T, Ctrl+1–9 |

---

## Sprint 12 — Hierarchical Docking Layout Tree + Designer Serialisation

**Goal:** Replace the flat group list with an expressive layout tree; add design-time document seed collection.

### 12.1 Hierarchical Layout Tree

#### New Classes

```
DocumentHost/Layout/
├── ILayoutNode.cs          — discriminated interface (SplitNode / GroupNode)
├── SplitLayoutNode.cs      — orientation, ratio, left/right (or top/bottom) child
├── GroupLayoutNode.cs      — group id, document ids, selected doc id
├── LayoutTreeBuilder.cs    — builds tree from current _groups state
└── LayoutTreeApplier.cs    — restores Control hierarchy from a saved tree
```

#### `ILayoutNode`
```csharp
public interface ILayoutNode
{
    string NodeId { get; }
    LayoutNodeType NodeType { get; }   // Split | Group
    Rectangle Bounds { get; set; }    // absolute in client coords (set during layout pass)
}
```

#### Topology rules
- Root is always a `SplitLayoutNode` or a single `GroupLayoutNode`.
- `SplitLayoutNode` has exactly 2 children (any combination of split/group).
- Nesting depth is unlimited but `MaxGroups` caps total leaf-group count.
- Empty `GroupLayoutNode` is removed and its parent split collapses.

### 12.2 Docking Guide Adorners

Add `BeepDocumentDockGuide` control — the 5-target Visual Studio–style docking compass that appears during drag-to-dock operations:

```
         ┌───┐
         │ ↑ │   Top
         └───┘
    ┌───┐ ┌───┐ ┌───┐
    │ ← │ │ C │ │ → │   Left | Center | Right
    └───┘ └───┘ └───┘
         ┌───┐
         │ ↓ │   Bottom
         └───┘
```

- Compass appears when a float window or tab is dragged near a document group.
- Hovering a direction arrow highlights a translucent preview rectangle showing where the panel will land.
- Dropping onto a direction calls `SplitDocumentHorizontal/Vertical` with the correct orientation.

### 12.3 Design-Time Document Seed Collection

Expose `DesignTimeDocuments` as a `Collection<DocumentDescriptor>` property so the designer can serialise initial documents into `InitializeComponent`:

```csharp
[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
public Collection<DocumentDescriptor> DesignTimeDocuments { get; }
```

`DocumentDescriptor` (existing file) — add:
- `InitialContent` enum: `Empty`, `Label`, `RichTextBox`, `WebView`
- `AccentColor`, `IsPinned`, `CanClose`

Custom `CodeDomSerializer` emits:
```csharp
host.DesignTimeDocuments.Add(new DocumentDescriptor
    { Id = "doc-001", Title = "Start Page", IsPinned = true });
```

---

## Sprint 13 — Performance at Scale + Accessibility Hardening

**Goal:** Verified performance for 100+ tabs; full screen-reader compliance.

### 13.1 Performance Targets

| Scenario | Target |
|---|---|
| Open 100 documents | < 300 ms |
| Activate any of 100 documents | < 16 ms (1 frame) |
| Tab strip repaint with 100 tabs | < 8 ms |
| Layout serialise 100 tabs | < 50 ms |
| Drag any tab 100+ document session | 60 FPS, no allocation spike |

#### Implementation approach
- `_tabs` list stays; painting uses `_tabArea` clip + per-tab visibility pre-filter.
- Add `_paintBatch` flag — coalesce layout + invalidate calls during AddDocument loop.
- Virtualise tab geometry: don't measure/draw tabs fully outside the `_tabArea` clip rect.
- `Dictionary<string, Rectangle>` hit-test cache — rebuild only when layout changes.
- Thumbnail capture: lazy + async (`Task.Run` + `BeginInvoke` to marshal result back).

### 13.2 Accessibility Hardening

#### Requirements (WCAG 2.2 AA + Windows Accessibility)

| Test | Requirement |
|---|---|
| Tab navigation | Every tab reachable by keyboard arrow keys |
| Screen reader (Narrator) | Reads "Tab N of M: Title [modified] [pinned]" |
| Focus indicator | 3 px high-contrast focus ring on keyboard-active tab |
| High contrast | All colors resolve from `SystemColors` when `SystemInformation.HighContrast = true` |
| Narrate context menu | Narrator announces each menu item |
| Color-blind safe | Never use red/green alone; pair with icons or shapes |

#### Implementation
- `BeepDocumentTabStrip.Accessibility.cs` — expand `TabAccessibleObject` with `Name`, `Role`, `State`, `Navigate`, `GetChild`
- Add `OnPaintFocusIndicator()` — draws rounded rectangle outline when `ContainsFocus` and `_keyboardFocusIndex >= 0`
- `ApplyHighContrastTheme()` — maps all paint colors to `SystemColors` equivalents when `SystemInformation.HighContrast`

---

## Sprint 14 — Figma Design System Alignment + Tab UI/UX Polish

**Goal:** Full alignment with 2025–2026 Figma component standards (Material Design 3, Fluent UI 2, Radix Themes).

### 14.1 Design Token System

Define constants in `DocumentHost/Tokens/DocumentHostTokens.cs`:

```csharp
public static class DocTokens
{
    // Geometry
    public const int TabHeight          = 36;   // matches MD3 NavigationBar item height
    public const int TabPaddingH        = 16;   // Fluent 2 horizontal padding
    public const int TabPaddingV        = 8;
    public const int TabMinWidth        = 80;
    public const int TabMaxWidth        = 240;
    public const int TabIconSize        = 16;
    public const int TabCornerRadius    = 6;    // Chrome/Edge 2025 value
    public const int PinnedTabWidth     = 48;   // icon + small pad
    public const int IndicatorThickness = 3;    // Underline/VSCode styles
    public const int BadgeSize         = 16;
    public const int BadgeFontSize     = 10;
    public const int StripBorderWidth  = 1;

    // Timing (ms)
    public const int IndicatorAnimMs    = 200;
    public const int CloseFadeMs        = 150;
    public const int SlideOverlayMs     = 180;
    public const int DockGuideHoverMs   = 80;
}
```

### 14.2 Tab Visual Improvements (per Figma 2026 trends)

#### Chrome style improvements
- Corner radius = `DocTokens.TabCornerRadius` (6 px — matches Edge 2025).
- Bottom-edge anti-alias merge: blend stroke colour with content area back colour.
- Hover state: lighter fill interpolated from theme `CardBackColor` + 10 % white.
- Modified indicator: small filled circle `●` aligned top-right (not inside title text).
- Pinned tab: shows only favicon; no text; 48 px wide.

#### VS Code style improvements
- Top accent bar thickness from 3 → `DocTokens.IndicatorThickness` (token-driven).
- Inactive tab text: slightly dimmer alpha (70%) for better visual hierarchy.
- Close button: `×` replaced by `⊗` icon at 14 px; fades in over 150 ms.

#### Material / Pill style improvements
- Pill background uses `PrimaryColor` at 15% opacity for inactive; 100% for active — matches MD3 NavigationBar.
- Active pill uses `PrimaryColor`; text uses `OnPrimaryColor` (auto-calculated contrast).
- Hover: pill scale transform (102%) via existing animation infrastructure.

#### New "Fluent" style
- Inspired by Fluent UI 2 / Windows 11 Settings tabs.
- Translucent acrylic-style background (alpha blend with parent surface).
- 4 px bottom border on active tab in `PrimaryColor`.
- Smooth width transitions when tabs are added/removed.

### 14.3 Overflow Dropdown Polish

- Fuzzy search using trigram matching (not just `Contains`).
- Group entries by `BeepDocumentTab.DocumentCategory` with section headers.
- Keyboard-navigable list with ↑↓ + Enter/Esc.
- Recent/pinned tabs section at the top.

### 14.4 Thumbnail Quality

- Capture at 2× logical resolution for HiDPI displays.
- Apply a 4 px border radius to the thumbnail preview corners.
- Show spinning loader indicator if capture is in progress (async).
- Cache invalidated on `IsModified` change.

---

## Sprint 15 — Document Status Bar + Group UI + Polish

**Goal:** Completion-quality document status bar, animated group transitions, minor UX polish.

### 15.1 Enhanced Document Status Bar

Extend `BeepDocumentPanel`:

| Segment | Content | Example |
|---|---|---|
| Left | Document-specific info (set by host consumer via `SetStatusText`) | `Ln 45, Col 12` |
| Centre | App/context status | `Ready` |
| Right | Encoding · CRLF · Git branch indicator | `UTF-8 · CRLF · main` |

```csharp
panel.SetStatusLeft("Ln 45, Col 12");
panel.SetStatusRight("UTF-8 · CRLF · main", gitIcon: @"icons/git.png");
panel.ShowStatusBar = true;
```

Status bar height: 22 px (matches VS Code). Background: `PanelBackColor` shifted ±10% lightness.

### 15.2 Group Collapse / Expand Animation

When a secondary group becomes empty and collapses, animate the splitter bar sliding from its current position to the edge:

- Duration: 200 ms (ease-out cubic).
- Prevent interaction during animation (`_groups[1].TabStrip.Enabled = false`).
- Re-enable and remove the group only after animation completes.

### 15.3 Tab Tooltip Enhancements

Extend `BeepDocumentRichTooltip`:

| Section | Content |
|---|---|
| Header | Icon + Title + Modified badge |
| Thumbnail | Live 200×120 px snapshot with rounded corners |
| Meta row 1 | `DocumentCategory` label if set |
| Meta row 2 | File path / `TooltipText` set by caller |
| Footer | `Ctrl+W to close  ·  Ctrl+click to pin` hint |

Trigger delay: 800 ms (matches VS Code). Dismiss on mouse leave with 100 ms fade-out.

### 15.4 Empty State Illustration

When `DocumentCount == 0`, paint a centered empty-state panel in the content area:

```
[ Icon: document-outline.svg (64 px) ]
[ "No open documents"                ]
[ "Press Ctrl+N or click + to start" ]
```

Colors and font from Beep theme tokens. Can be disabled via `ShowEmptyState = false`.

---

## Sprint 16 — MVVM v2 + Command Service

**Goal:** First-class MVVM support matching WPF TabControl/DocumentPane patterns.

### 16.1 `IDocumentHostCommandService`

```csharp
public interface IDocumentHostCommandService
{
    void NewDocument(string? documentId = null, string? title = null);
    bool CloseDocument(string documentId);
    bool CloseAllDocuments();
    void ActivateDocument(string documentId);
    void FloatDocument(string documentId);
    void DockBackDocument(string documentId);
    void PinDocument(string documentId);
    void UnpinDocument(string documentId);
    void SplitHorizontal(string documentId);
    void SplitVertical(string documentId);
    void SaveLayout(string filePath);
    Task<bool> RestoreLayoutAsync(string filePath);
}
```

### 16.2 Source Collection Binding

```csharp
// Bind a collection of view-models directly
host.DocumentsSource = myViewModels;      // IEnumerable<IDocumentViewModel>
host.DocumentTemplate = vm => vm.CreateView();
host.DocumentIdSelector = vm => vm.DocumentId;
host.DocumentTitleSelector = vm => vm.Title;
```

Observes `INotifyCollectionChanged` — add/remove view-models automatically, open/close tabs.

### 16.3 Dependency-Injection Register Pattern

For modern .NET DI containers:

```csharp
// In app startup  
services.AddBeepDocumentHost(options =>
{
    options.DefaultTabStyle = DocumentTabStyle.VSCode;
    options.MaxGroups = 3;
    options.KeyboardShortcutsEnabled = true;
});
```

---

## Sprint 17 — Designer UX v2

**Goal:** Full commercial-grade Visual Studio SDK designer experience.

### 17.1 Docking Guide Adorners in Designer

Show the 5-point docking compass when a document descriptor is dragged in the designer surface:
- Same `BeepDocumentDockGuide` control from Sprint 12.
- Hit-test against the compass arrows using `BehaviorService`.
- On drop, call `designer.DoDefaultAction()` → `SplitDocumentHorizontal/Vertical`.

### 17.2 Collection Editor

`DocumentDescriptorCollectionEditor` — invoked from smart-tag "Edit Design-Time Documents…":
- Grid view: Id / Title / IconPath / IsPinned / CanClose / InitialContent / AccentColor.
- Icon picker column: opens `BeepImagePickerDialog`.
- Reorder with up/down buttons.
- Preview: shows a mini tab strip with the configured tabs.

### 17.3 Layout Picker Dialog

`DocumentLayoutPickerDialog` — visual picker for common starting layouts:

| Preset | Description |
|---|---|
| Single | One tab group, full area |
| Side-by-Side | Two groups, horizontal split |
| Stacked | Two groups, vertical split |
| Three-Way | Three groups in L-pattern |
| Four-Up | Four groups in 2×2 grid |

Selecting a preset calls `ApplyLayoutPreset(preset)` on the host.

### 17.4 Theme Picker Smart-Tag Item

Add "Choose Theme…" to the smart-tag which opens a visual theme grid (uses existing `BeepImagePickerDialog` pattern).

---

## Sprint 18 — Touch, Pointer & Responsive Skin

**Goal:** Native touch + gesture support; adaptive tab strip for very narrow containers.

### 18.1 Touch Gestures

| Gesture | Action |
|---|---|
| Swipe left / right on tab strip | Scroll tabs |
| Long-press on tab | Open context menu |
| Pinch on tab strip | Cycle through `TabSizeMode` (Equal → Compact → Fixed) |
| Swipe down + hold on tab | Drag-to-float ghost |

Use `WM_TOUCH` (`CreateParams` override) for WinForms 7+ compatibility.

### 18.2 Responsive Tab Strip

When strip width drops below threshold:

| Width | Behaviour |
|---|---|
| > 480 px | Normal: title + icon + close |
| 240–480 px | Compact: title only, close on hover |
| 120–240 px | Icon-only with overflow dropdown |
| < 120 px | Only the active tab visible + overflow button |

New property: `ResponsiveBreakpoints` (configurable thresholds).

### 18.3 Density Modes

| Mode | Tab height | Font size |
|---|---|---|
| `Comfortable` | 36 px (default) | 12 px |
| `Compact` | 28 px | 11 px |
| `Dense` | 22 px | 10 px |

```csharp
host.TabDensity = TabDensityMode.Compact;
```

---

## Sprint 19 — Theming v2 + Dark/Light/High-Contrast

**Goal:** True per-document theming; first-class dark mode; automatic high-contrast.

### 19.1 Per-Document Theme Override

```csharp
host.SetDocumentTheme("doc-001", "BeepDark");   // that panel uses a different theme
```

Renders the content area with theme-specific BackColor; title bar and tab get an accent border in the document's theme `PrimaryColor`.

### 19.2 System Dark Mode Sync

```csharp
SystemEvents.UserPreferenceChanged += (s, e) =>
{
    if (e.Category == UserPreferenceCategory.General)
        host.SyncSystemTheme();    // new method
};
```

`SyncSystemTheme()` maps:
- `SystemParameters.UseLightTheme == false` → `"BeepDark"`
- High contrast → `"BeepHighContrast"`

### 19.3 Accent Color Extraction

```csharp
host.UseSystemAccentColor = true;  // reads DWM accent color on Windows 10/11
```

Maps system accent → `theme.PrimaryColor` override for the active indicator bar.

---

## Sprint 20 — Shell Integration + File-Oriented Tracking

**Goal:** Real-world IDE-style file tracking; process-level MDI coordination.

### 20.1 File-Backed Document Tracking

`BeepDocumentFileTracker` — optional service that watches the file system:

```csharp
host.FileTracker = new BeepDocumentFileTracker();
host.FileTracker.Register("doc-001", @"C:\project\main.cs");
```

When the file is changed externally:
- `DocumentFileChanged` event fires.
- Default behavior: shows a non-modal info box inside the panel ("File changed. Reload?").
- `FileTracker.AutoReload` option silently reloads.

### 20.2 Recent Files / Workspace Restore

Persist the last workspace JSON to `%AppData%\Beep\<AppName>\workspace.json` automatically when `AutoSaveLayout = true`.

On next launch, a `WorkspaceRestorePrompt` overlay asks:
"Restore 3 documents from your last session? [Restore] [Start Fresh]"

### 20.3 Multi-Monitor Float Geometry

Float windows remember their last screen-relative position per `documentId`.
Positions are included in schema v2 and restored to the correct monitor even after monitor configuration changes (best-effort: if monitor is missing, centre on primary).

---

## Non-Functional Targets (Phase 2 Overall)

| Target | Metric |
|---|---|
| Tab strip repaint | < 8 ms at 100 tabs |
| Activate document | < 16 ms (1 frame) |
| Open 100 documents | < 300 ms total |
| Save layout (100 tabs + groups) | < 50 ms |
| Restore layout (100 tabs + groups) | < 200 ms |
| GC pressure during normal use | 0 gen-2 allocations per scroll/tab-click |
| DPI scaling fidelity | Pixel-perfect at 100 / 125 / 150 / 175 / 200 % |
| WCAG 2.2 compliance | Level AA |
| Memory (100 tabs, no thumbnails) | < 15 MB additional |
| Memory (100 tabs, thumbnails at 200×120) | < 60 MB additional |

---

## UX Principles (Figma / Commercial Alignment)

Based on 2026 Figma component systems and UX audits of VS Code, DevExpress, and Telerik:

1. **Proximity** — related actions (close, pin, float) are in the tab header, not in a distant toolbar.
2. **Feedback** — every state change (modified, pinned, floated) has a visible indicator within 16 ms.
3. **Reversibility** — every destructive action (close) is reversible via Ctrl+Shift+T for `MaxClosedHistory` documents.
4. **Keyboard parity** — every action achievable with a mouse is achievable with the keyboard.
5. **Progressive disclosure** — advanced options (float, split, auto-hide) live in the context menu, not on the default surface.
6. **Consistent spacing** — all geometry driven by `DocTokens` constants, never hard-coded pixel values.
7. **Theme fidelity** — no raw `Color.FromArgb` in paint code; all colours sourced from `IBeepTheme` properties.
8. **Quiet defaults** — animations run at correct speed by default (200 ms); disabled with `ReducedMotion` detection.

---

## Deliverable Checklist

### Sprint 11
- [ ] `DocumentHost/Layout/LayoutTreeNode.cs`
- [ ] `DocumentHost/Layout/LayoutMigrationService.cs`
- [ ] `DocumentHost/DocumentDescriptor.cs` — extended with `CustomData`
- [ ] `BeepDocumentHost.Serialisation.cs` — schema v2 save/restore
- [ ] `Design.Server/ActionLists/DocumentHostActionList.cs` — transaction-wrapped
- [ ] `Design.Server/Designers/BeepDocumentHostDesigner.cs` — transaction-wrapped
- [ ] `tests/DocumentHost/DocumentLifecycleTests.cs`
- [ ] `tests/DocumentHost/SerialisationRoundTripTests.cs`

### Sprint 12
- [ ] `DocumentHost/Layout/SplitLayoutNode.cs`
- [ ] `DocumentHost/Layout/GroupLayoutNode.cs`
- [ ] `DocumentHost/Layout/LayoutTreeApplier.cs`
- [ ] `BeepDocumentDockGuide.cs`
- [ ] `DocumentHost/Tokens/DocumentHostTokens.cs`
- [ ] `Design.Server/Editors/DocumentDescriptorCollectionEditor.cs`
- [ ] Updated `BeepDocumentHost.Documents.cs` — layout-tree-aware split/merge

### Sprint 13
- [ ] `BeepDocumentTabStrip.Accessibility.cs` — full `AccessibleObject` redesign
- [ ] `DocumentHost/BeepDocumentPerfMonitor.cs` — benchmark harness
- [ ] Paint-virtualisation in `BeepDocumentTabStrip.Painting.cs`
- [ ] Hit-test cache in `BeepDocumentTabStrip.Layout.cs`

### Sprint 14
- [ ] `DocumentHostTokens.cs` — full token set
- [ ] `BeepDocumentTabStrip.Painting.cs` — new Fluent style + Polish to all 8 styles
- [ ] `BeepDocumentTabStrip.Overflow.cs` — fuzzy search + category grouping
- [ ] `BeepDocumentRichTooltip.cs` — enhanced layout + HiDPI thumbnail

### Sprints 15–20
- [ ] `BeepDocumentPanel` status bar redesign
- [ ] Group collapse animation in `BeepDocumentHost.Layout.cs`
- [ ] `IDocumentHostCommandService.cs`
- [ ] `BeepDocumentHost.MVVM.cs` — source collection binding
- [ ] `Design.Server/Editors/DocumentLayoutPickerDialog.cs`
- [ ] `BeepDocumentFileTracker.cs`
- [ ] Workspace auto-restore prompt
- [ ] Per-document theme override
- [ ] System dark-mode sync
- [ ] Touch gesture support
- [ ] Responsive breakpoints in tab strip
