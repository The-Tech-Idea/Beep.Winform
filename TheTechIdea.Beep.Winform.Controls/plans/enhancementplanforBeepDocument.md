strt# Enhancement Plan — BeepDocumentHost & BeepDocumentTabStrip

> 🏆 **ALL 10 SPRINTS COMPLETE — 2026-02-28** 🏆
> Every feature in this plan has been implemented across Sprints 1–10.
> The `BeepDocumentHost` / `BeepDocumentTabStrip` suite is now at full commercial-grade parity.

> **Benchmarks:** DevExpress XtraTab / DocumentManager, Telerik RadDock / RadTabbedFormControl,
> Syncfusion TabControlAdv, Infragistics UltraTabbedMdiManager, DockPanelSuite, AvalonDock.
>
> **Scope:** Bring BeepDocumentHost to feature-parity with commercial WinForms MDI suites
> while keeping the lightweight, BaseControl-free architecture (Scenario C).

---

## Current State (v 1.0)

| Area | What we have today |
|---|---|
| **Tab styles** | Chrome, VSCode, Underline, Pill |
| **Tab strip** | Scroll overflow (two-pass), animated active indicator, DPI scaling, drag-reorder, middle-click close, mouse wheel, empty-state hint, accent bar |
| **Document host** | Add / Activate / Close / Float / DockBack, ControlStyle (Flat/Thin/Raised), TabPosition (Top/Bottom/Hidden), theme propagation |
| **Panel** | IsModified dirty-dot, CanClose, IconPath, StatusBar, AutoScroll |
| **Float** | Basic `BeepDocumentFloatWindow` (BeepiFormPro-based), DetachPanel, DockBack event |

---

## Phase 1 — Tab Strip Polish & Interaction (Priority: HIGH)

### 1.1 Tab Context Menu (Right-Click) ✅ Sprint 1
> *DevExpress: `ShowTabHeaderMenu`, Telerik: `TabStripContextMenu`*

- [ ] Built-in `ContextMenuStrip` with stock items:  
  **Close | Close All But This | Close All to the Right | Close All**  
  **Separator | Pin/Unpin | Float | Copy Path**
- [ ] `TabContextMenuOpening` event so callers can add/remove items.
- [ ] Respect `CanClose` and `IsPinned` states to grey-out items.

### 1.2 Pinned Tabs ✅ Sprint 1
> *DevExpress: `PinPageOptions`, Chrome: pinned-tab behaviour*

- [ ] Pinned tabs collapse to icon-only width, left-aligned before unpinned tabs.
- [ ] Drag a pinned tab among other pinned tabs but never past the pin boundary.
- [ ] Pin/Unpin toggle via context menu + `BeepDocumentTab.IsPinned` property.
- [ ] `TabPinned` / `TabUnpinned` events on the tab strip.
- [ ] Pinned tab badge or filled-dot visual indicator.

### 1.3 Tab Overflow Dropdown ✅ Sprint 3
> *DevExpress: tab-list drop-down, VS Code: "Show Opened Editors"*

- [ ] When tabs overflow, draw a **▾ chevron** button next to/replacing the right scroll arrow.
- [ ] Clicking it opens a `BeepPopupListBox` (or themed popup) listing all tabs with icons,
  modified indicators, and search-filter input.
- [ ] Selecting from the list activates and scrolls that tab into view.

### 1.4 Close Button Enhancements ✅ Sprint 1
> *DevExpress: `ClosePageButtonShowMode`, Telerik: `CloseButton` property*

- [ ] New `TabCloseMode.ActiveOnly` — show × only on the active tab.
- [ ] Animated fade-in/out of the × when `CloseMode == OnHover` (alpha lerp on invalidate timer).
- [ ] `TabClosing` cancellable event (e.g., "Save changes?" dialog flow).
- [ ] Double-click on empty strip area → `NewDocumentRequested`.

### 1.5 Keyboard Navigation ✅ Sprint 2
> *DevExpress: Ctrl+Tab, Ctrl+F4. VS Code: Ctrl+PgUp / PgDn*

- [ ] `Ctrl+Tab` / `Ctrl+Shift+Tab` — cycle forward/backward through tabs (MRU or sequential).
- [ ] `Ctrl+W` or `Ctrl+F4` — close active document.
- [ ] `Ctrl+Shift+T` — reopen last closed document (needs closed-tab history stack).
- [ ] `Ctrl+1..9` — jump directly to the *n*-th tab.
- [ ] `KeyboardShortcutsEnabled` property to disable/enable all.
- [ ] Expose an `ICommandKeyBindings` interface for user customisation.

### 1.6 Tab Drag-to-Float ✅ Sprint 3
> *DevExpress DocumentManager: drag tab out of the strip to float*

- [ ] When a tab is dragged vertically beyond a threshold distance from the strip,
  create a transparent "ghost" outline form following the cursor.
- [ ] On drop outside the strip → call `FloatDocument(id)`.
- [ ] On drop back over the strip → reorder instead.
- [ ] `AllowDragFloat` property (default `true`).

---

## Phase 2 — Document Management & Navigation (Priority: HIGH)

### 2.1 Document Groups (Split View) ✅ Sprint 10
> *DevExpress: `DocumentGroup`, VS Code: side-by-side editors*

- [x] New class `BeepDocumentGroup` — a lightweight container that holds one tab strip + content area.
- [x] `BeepDocumentHost` switches from "single content area" to "one or more groups arranged in a split".
- [x] `SplitDocumentHorizontal(documentId)` / `SplitDocumentVertical(documentId)` — move a document to a new group.
- [x] When a group becomes empty, collapse it automatically.
- [x] Dragging a tab from one group's strip to another moves the document across groups (via `MoveDocumentToGroup`).
- [x] `MaxGroups` property (default 4), `SplitHorizontal` bool property, `SplitRatio` float property.

### 2.2 Tab Colourise / Category Labels ✅ Sprint 8
> *DevExpress: `PageAppearance`, VS Code: tab colour decorations*

- [ ] Per-document `TabColor` (background tint) and `TabBadge` (e.g., "M" for modified, "RO" for read-only).
- [ ] Property `TabColorMode`: `None`, `AccentBar`, `FullBackground`, `BottomBorder`.
- [ ] `DocumentCategory` string on the panel — host can group tabs by category (visual separator lines or named sections in the overflow dropdown).

### 2.3 MRU (Most Recently Used) Order ✅ Sprint 2
> *VS Code: Ctrl+Tab MRU switching, DevExpress: MRU list*

- [ ] Internal `Stack<string>` tracking activation order.
- [ ] `Ctrl+Tab` opens a VS-style popup showing MRU thumbnails / titles.
- [ ] Public `RecentDocuments` read-only list.
- [ ] `MaxRecentHistory` property (default 20).

### 2.4 Reopen Closed Tabs ✅ Sprint 2
> *Chrome: Ctrl+Shift+T, VS Code: "Reopen Closed Editor"*

- [ ] `ClosedTabStack` holds last *N* closed document metadata (id, title, icon, serialised state).
- [ ] `ReopenLastClosed()` method + `Ctrl+Shift+T` shortcut.
- [ ] `DocumentClosing` event carries a `RestoreData` object the consumer can populate.
- [ ] `MaxClosedHistory` property (default 10).

### 2.5 Tab Search / Quick-Switch ✅ Sprint 4
> *VS Code: Ctrl+P, DevExpress: DocumentSelector*

- [ ] `ShowQuickSwitch()` method opens a themed popup with a search text box.
- [ ] Fuzzy or substring match against `Title`, `DocumentId`, `DocumentCategory`.
- [ ] Arrow keys + Enter to navigate and select; Esc to dismiss.
- [ ] `Ctrl+P` shortcut when `KeyboardShortcutsEnabled` is true.

---

## Phase 3 — Floating & Docking (Priority: MEDIUM)

### 3.1 Enhanced Float Window ✅ Sprint 4
> *DevExpress: `FloatDocuments`, Telerik: FloatingWindow, DockPanelSuite*

- [ ] Resizable, snappable float window with its own mini tab strip (for multi-document float groups).
- [ ] "Dock Back" toolbar button prominent in the title bar.
- [ ] Float window inherits parent Beep theme automatically.
- [ ] `FloatSize`, `FloatLocation`, `FloatSizeMode` (Auto / Fixed / Remember) properties on `BeepDocumentPanel`.
- [ ] Persist float positions across sessions (serialise to JSON).

### 3.2 Dock Targets & Snap Indicators ✅ Sprint 6
> *DevExpress: dock guide diamonds, DockPanelSuite: centre/edge dock targets*

- [ ] When dragging a floating window back over the host, display translucent dock-target indicators
  (centre = re-dock, left/right/top/bottom = split into a new group).
- [ ] Docking previews: semi-transparent highlight showing where the document will land.
- [ ] `AllowDockTargets` property.

### 3.3 Auto-Hide Side Panels (Tool-Window Concept) ✅ Sprint 8
> *DevExpress: AutoHideContainers, VS: auto-hide side panels*

- [ ] Optional thin "tool-window" side-bar strips (left, right, bottom) on a `BeepDocumentHost`.
- [ ] A document panel can be "auto-hidden" — its tab icon appears on the side strip;
  hovering/clicking slides it out as an overlay.
- [ ] `AutoHideDocument(id, AutoHideSide.Left)` API.
- [ ] This is a bonus feature; most MDI apps will skip it, but it matches DevExpress DocumentManager fully.

---

## Phase 4 — Theming & Visual Polish (Priority: MEDIUM)

### 4.1 Additional Tab Styles ✅ Sprint 5
> *DevExpress: multiple skin families, Telerik: 10+ tab shapes*

- [ ] **Flat** — no rounded corners, just a top border colour.
- [ ] **Rounded** — fully rounded pill-like tabs (different from Pill indicator).
- [ ] **Trapezoid** — Chrome-legacy tabs with angled sides.
- [ ] **Office** — Ribbon-style tabs mimicking Microsoft Office look.
- [ ] Each style gets its own `DrawTab*` method in `BeepDocumentTabStrip.Painting.cs`.

### 4.2 Tab Animations & Transitions ✅ Sprint 3
> *VS Code: tab slide-in, DevExpress: tab fade/slide*

- [ ] **Open animation**: new tab slides in from the right and fades in (alpha 0→255 over 150 ms).
- [ ] **Close animation**: closing tab collapses width to 0 over 120 ms, then removed.
- [ ] **Reorder animation**: displaced tabs slide smoothly instead of jumping.
- [ ] Animation durations configurable via `AnimationDuration` property; set to 0 to disable.

### 4.3 Tab Size Modes ✅ Sprint 3
> *DevExpress: `TabPageWidth` = Auto / Fixed / FitToContent*

- [ ] `TabSizeMode` enum: `FitToContent`, `Equal`, `Compact`, `Fixed(int pixels)`.
- [ ] `FitToContent` — each tab's width = icon + text + close + padding; no sharing.
- [ ] `Compact` — uses abbreviated title + tooltip for full path (good for file-based MDI).
- [ ] `FixedWidth` property honoured when mode is `Fixed`.

### 4.4 Icon Rendering Improvements ✅ Sprint 5
> *DevExpress: vector/SVG tab icons, Telerik: ImageList support*

- [ ] Support `ImageList` + `ImageIndex` alongside `IconPath`.
- [ ] Support SVG icon rendering via `StyledImagePainter` (already in the project).
- [ ] Greyscale icon for inactive tabs (subtle de-emphasis like VS Code).
- [ ] Per-theme icon tinting via `StyledImagePainter.TintMode`.

### 4.5 Tooltip Enhancement ✅ Sprint 6
> *DevExpress: rich tooltips with preview image, VS Code: tooltip with path*

- [ ] Replace basic `ToolTip` with a custom-drawn `BeepToolTip` that:
  - Shows full document path / metadata.
  - Shows a live thumbnail snapshot of the document panel content.
  - Has a subtle shadow/rounded-corner style matching the Beep theme.
- [ ] `TooltipMode`: `None`, `Simple`, `Rich` (with thumbnail).

### 4.6 RTL (Right-to-Left) Support ✅ Sprint 8
> *DevExpress: full RTL mirroring*

- [ ] Mirror tab strip layout when `RightToLeft == RightToLeft.Yes`.
- [ ] Scroll buttons and add button switch sides.
- [ ] Close button moves to the left of each tab.

---

## Phase 5 — State Persistence & Serialisation (Priority: MEDIUM)

### 5.1 Layout Serialisation ✅ Sprint 5
> *DevExpress: `SaveLayoutToXml / RestoreLayoutFromXml`, DockPanelSuite: XML persist*

- [ ] `SaveLayout()` → returns a JSON/XML string capturing:
  - Open document ids, order, active doc, pinned state.
  - Document groups (split configuration).
  - Float window positions and sizes.
- [ ] `RestoreLayout(string data)` — re-opens documents per saved state.
- [ ] `LayoutSerialising` / `LayoutRestoring` events so the consumer can hydrate each document panel.

### 5.2 Session Restore ✅ Sprint 8
> *Chrome: "Continue where you left off", VS Code: workspace session*

- [ ] `AutoSaveLayout` property (default `false`) — if enabled, saves layout to a file on close.
- [ ] On next launch, `RestoreLayout()` is called automatically if the file exists.
- [ ] `SessionFile` property specifying the path.

---

## Phase 6 — Accessibility & API (Priority: HIGH)

### 6.1 Accessibility (a11y) ✅ Sprint 2
> *Required for any commercial-grade control*

- [ ] Override `CreateAccessibilityInstance()` in `BeepDocumentTabStrip` to expose tabs as
  `AccessibleRole.PageTab` children.
- [ ] Set `AccessibleName`, `AccessibleDescription`, `AccessibleRole` for each tab from `Title` / `TooltipText`.
- [ ] Support `UI Automation` patterns: `SelectionItem`, `ScrollItem`.
- [ ] Keyboard focus rectangle painted on the focused tab.

### 6.2 Designer Integration ✅ Sprint 6
> *DevExpress: rich Smart Tags, Syncfusion: designer verb menus*

- [ ] Custom `ControlDesigner` for `BeepDocumentHost` with:
  - Smart Tag panel exposing `TabStyle`, `TabPosition`, `CloseMode`, `ControlStyle`, `ThemeName`.
  - "Add Document" verb that creates a `BeepDocumentPanel` at design time.
- [ ] `ComponentEditor` for `BeepDocumentTabStrip` that lets you preview tab styles in the property grid.

### 6.3 Data-Binding API ✅ Sprint 5
> *DevExpress: `DocumentManager.View.Documents` bindable collection*

- [ ] `Documents` observable collection (`BindingList<DocumentDescriptor>` or `ObservableCollection`).
- [ ] `DocumentDescriptor` DTO: `Id`, `Title`, `IconPath`, `IsModified`, `IsPinned`, `Category`, `Tag`.
- [ ] Two-way binding: adding to the list creates a tab + panel; removing closes the document.
- [ ] `DocumentTemplate` property (Func<DocumentDescriptor, Control>) — factory for panel content.

### 6.4 MVVM-Friendly Document Model ✅ Sprint 7
> *AvalonDock-style view-model integration*

- [ ] `IDocumentViewModel` interface: `string Id`, `string Title`, `bool IsModified`, `bool CanClose`, `ImageSource Icon`.
- [ ] `DocumentSource` property on the host (like `ItemsSource` in WPF) — binds a list of view-models.
- [ ] `DocumentContentTemplate` delegate that maps a view-model to a WinForms `Control`.
- [ ] Implement `INotifyPropertyChanged` wiring so tab titles, icons, dirty state update automatically.

---

## Phase 7 — Advanced Features (Priority: LOW)

### 7.1 Tab Preview on Hover (Peek) ✅ Sprint 7
> *Windows 11 taskbar tab previews, DevExpress: TabPageThumbnail*

- [ ] Hovering a tab for > 500 ms shows a small tooltip-like popup with a bitmap snapshot
  of that document's panel content.
- [ ] Capture bitmap via `Control.DrawToBitmap()` on activation or on a lazy timer.
- [ ] `TabPreviewSize` and `TabPreviewEnabled` properties.

### 7.2 Drag Between Multiple Hosts ✅ Sprint 7
> *VS Code: drag tabs between editor groups and windows*

- [ ] Enable OLE `DragDrop` on the tab strip with a custom data format (`BeepDocumentDragData`).
- [ ] A document can be dragged from one `BeepDocumentHost` to another within the same process.
- [ ] The source host raises `DocumentDetaching`; the target host raises `DocumentAttaching`.
- [ ] If the target is a float window's mini-strip, merge into the float group.

### 7.3 Tab Grouping with Headers ✅ Sprint 9
> *VS Code: sticky scroll/editor group labels, Telerik: TabGroupHeader*

- [ ] Visual group headers (e.g., "Models", "Views") inserted between tabs.
- [ ] Groups are collapsible: clicking the header hides/shows the group's tabs.
- [ ] `BeepDocumentTab.Group` property assigns a tab to a group.
- [ ] `TabGroups` collection on the strip with `GroupName`, `GroupColor`, `IsCollapsed`.

### 7.4 Notification Badges ✅ Sprint 7
> *DevExpress: tab badges, Chrome: playing audio indicator*

- [ ] `BeepDocumentTab.BadgeText` — short string ("3", "!", "●") drawn as a coloured pill/circle
  overlapping the tab's top-right corner.
- [ ] `BeepDocumentTab.BadgeColor` — background colour of the badge.
- [ ] Animated pulse when `BadgeText` changes (scale 1.0→1.2→1.0 over 200 ms).

### 7.5 Multi-Row Tab Strip ✅ Sprint 9
> *DevExpress: MultiLine tab mode, Syncfusion: MultilineTextAlignment*

- [ ] `TabRows` property: `SingleRow` (scroll on overflow — current), `MultiRow` (wrap to new row).
- [ ] In multi-row mode, the active tab's row moves to the bottom (closest to content), like classic Windows tab controls.
- [ ] Dynamic height adjustment of the tab strip based on row count.

### 7.6 Side Tabs (Vertical Orientation) ✅ Sprint 9
> *DevExpress: `TabsPosition = Left / Right`, Telerik: vertical tabs*

- [ ] `TabStripPosition.Left` / `TabStripPosition.Right` — rotate tab drawing 90°.
- [ ] Text drawn vertically (or horizontally with a wider sidebar).
- [ ] Scroll becomes vertical.

---

## Implementation Priorities & Phasing

| Phase | Epic | Effort | Impact | Suggested Sprint |
|-------|------|--------|--------|-----------------|
| **1.1** | Tab Context Menu | S | HIGH | ✅ Sprint 1 |
| **1.2** | Pinned Tabs | M | HIGH | ✅ Sprint 1 |
| **1.4** | Close Enhancements + TabClosing event | S | HIGH | ✅ Sprint 1 |
| **1.5** | Keyboard Navigation | M | HIGH | ✅ Sprint 2 |
| **6.1** | Accessibility | M | HIGH | ✅ Sprint 2 |
| **2.3** | MRU Order | S | MEDIUM | ✅ Sprint 2 |
| **2.4** | Reopen Closed Tabs | S | MEDIUM | ✅ Sprint 2 |
| **1.3** | Tab Overflow Dropdown | M | MEDIUM | ✅ Sprint 3 |
| **1.6** | Tab Drag-to-Float | M | MEDIUM | ✅ Sprint 3 |
| **4.2** | Tab Animations | M | MEDIUM | ✅ Sprint 3 |
| **4.3** | Tab Size Modes | S | MEDIUM | ✅ Sprint 3 |
| **2.1** | Document Groups (Split) | L | HIGH | ✅ Sprint 10 |
| **3.1** | Enhanced Float Window | M | MEDIUM | ✅ Sprint 4 |
| **2.5** | Quick Switch Popup | M | MEDIUM | ✅ Sprint 4 |
| **5.1** | Layout Serialisation | M | HIGH | ✅ Sprint 5 |
| **6.3** | Data-Binding API | M | MEDIUM | ✅ Sprint 5 |
| **4.1** | Additional Tab Styles | M | LOW | ✅ Sprint 5 |
| **4.4** | Icon Rendering | S | LOW | ✅ Sprint 5 |
| **3.2** | Dock Targets & Snap | L | MEDIUM | ✅ Sprint 6 |
| **4.5** | Rich Tooltips | M | LOW | ✅ Sprint 6 |
| **6.2** | Designer Integration | M | LOW | ✅ Sprint 6 |
| **6.4** | MVVM Document Model | L | MEDIUM | ✅ Sprint 7 |
| **7.1** | Tab Preview / Peek | M | LOW | ✅ Sprint 7 |
| **7.2** | Drag Between Hosts | L | MEDIUM | ✅ Sprint 7 |
| **7.4** | Notification Badges | S | LOW | ✅ Sprint 7 |
| **2.2** | Tab Colourise / Categories | S | LOW | ✅ Sprint 8 |
| **3.3** | Auto-Hide Side Panels | L | LOW | ✅ Sprint 8 |
| **4.6** | RTL Support | M | LOW | ✅ Sprint 8 |
| **5.2** | Session Restore | S | LOW | ✅ Sprint 8 |
| **7.3** | Tab Groups with Headers | M | LOW | ✅ Sprint 9 |
| **7.5** | Multi-Row Tab Strip | L | LOW | ✅ Sprint 9 |
| **7.6** | Side Tabs (Vertical) | L | LOW | ✅ Sprint 9 |

> **Size key:** S = Small (< 200 LOC), M = Medium (200–600 LOC), L = Large (> 600 LOC)

---

## File Organisation Plan

```
DocumentHost/
│── BeepDocumentTab.cs                         (data model + enums — extend)
│── BeepDocumentTabStrip.cs                    (core partial — extend)
│── BeepDocumentTabStrip.Layout.cs             (layout — extend for multi-row, vertical, pinned)
│── BeepDocumentTabStrip.Painting.cs           (painting — extend for new styles, badges, animations)
│── BeepDocumentTabStrip.Mouse.cs              (mouse — extend for drag-to-float, context menu)
│── BeepDocumentTabStrip.Keyboard.cs           (NEW — keyboard shortcuts, Ctrl+Tab, focus)
│── BeepDocumentTabStrip.Accessibility.cs      (NEW — a11y overrides)
│── BeepDocumentTabStrip.ContextMenu.cs        (NEW — right-click menu logic)
│── BeepDocumentTabStrip.Properties.cs         (properties + events — extend)
│── BeepDocumentTabStrip.Animations.cs         (NEW — open/close/reorder animations)
│── BeepDocumentPanel.cs                       (extend with FloatSize, Category, etc.)
│── BeepDocumentHost.cs                        (core partial — extend)
│── BeepDocumentHost.Layout.cs                 (extend for document groups / split views)
│── BeepDocumentHost.Documents.cs              (extend with reopen, pin, MRU)
│── BeepDocumentHost.Events.cs                 (extend events, BeepDocumentFloatWindow enhance)
│── BeepDocumentHost.Properties.cs             (extend with new properties)
│── BeepDocumentHost.Serialisation.cs          (NEW — save/restore layout)
│── BeepDocumentHost.KeyBindings.cs            (NEW — keyboard command dispatch)
│── BeepDocumentGroup.cs                       (NEW — split-view group container)
│── BeepDocumentQuickSwitch.cs                 (NEW — floating search popup)
│── BeepDocumentOverflowPopup.cs               (NEW — tab overflow dropdown list)
│── BeepDocumentDragData.cs                    (NEW — OLE drag-drop data format)
│── DocumentDescriptor.cs                      (NEW — DTO for data-binding)
│── IDocumentViewModel.cs                      (NEW — MVVM interface)
│── Readme.md                                  (keep updated)
```

---

## Design Principles

1. **Partial-class-per-concern** — never let a single file exceed ~400 lines.
2. **No BaseControl dependency** — keep `Panel`/`Control` inheritance to avoid WM_ERASEBKGND conflicts with `BeepiFormPro`.
3. **DPI-first** — every pixel dimension passes through `S(logical)`.
4. **Theme-first** — all colours come from `IBeepTheme`; never hard-code colours.
5. **Event-first** — expose cancellable events for every user-initiated state change.
6. **Backwards-compatible** — all new features are opt-in via properties; existing consumer code must compile unchanged.

---

## Commercial Feature Comparison Matrix

| Feature | DevExpress | Telerik | Syncfusion | Beep v2.0 | Status |
|---------|-----------|---------|-----------|------------|--------|
| Multiple tab styles | ✅ | ✅ | ✅ | ✅ 8 styles | ✅ Done |
| Scroll overflow | ✅ | ✅ | ✅ | ✅ | ✅ Done |
| Tab context menu | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 1 |
| Pinned tabs | ✅ | ❌ | ❌ | ✅ | ✅ Sprint 1 |
| Keyboard navigation | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 2 |
| Drag reorder | ✅ | ✅ | ✅ | ✅ | ✅ Done |
| Drag to float | ✅ | ✅ | ❌ | ✅ | ✅ Sprint 3 |
| Document groups/split | ✅ | ✅ | ❌ | ✅ | ✅ Sprint 10 |
| MRU switching | ✅ | ❌ | ❌ | ✅ | ✅ Sprint 2 |
| Reopen closed | ❌ | ❌ | ❌ | ✅ | ✅ Sprint 2 |
| Quick switch / search | ❌ | ❌ | ❌ | ✅ | ✅ Sprint 4 |
| Float windows | ✅ | ✅ | ❌ | ✅ enhanced | ✅ Sprint 4 |
| Dock targets | ✅ | ✅ | ❌ | ✅ | ✅ Sprint 6 |
| Tab animations | ✅ | ✅ | ❌ | ✅ full | ✅ Sprint 3 |
| Rich tooltips | ✅ | ❌ | ❌ | ✅ | ✅ Sprint 6 |
| Layout serialisation | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 5 |
| Data binding | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 5 |
| Accessibility | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 2 |
| Designer support | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 6 |
| Multi-row tabs | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 9 |
| Vertical tabs | ✅ | ✅ | ❌ | ✅ | ✅ Sprint 9 |
| Notification badges | ✅ | ❌ | ❌ | ✅ | ✅ Sprint 7 |
| Auto-hide panels | ✅ | ✅ | ❌ | ✅ | ✅ Sprint 8 |
| RTL support | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 8 |
| MVVM / data model | ✅ | ✅ | ✅ | ✅ | ✅ Sprint 7 |
| Tab preview on hover | ✅ | ❌ | ❌ | ✅ | ✅ Sprint 7 |
| Drag between hosts | ✅ | ✅ | ❌ | ✅ | ✅ Sprint 7 |
| Tab group headers | ❌ | ✅ | ❌ | ✅ | ✅ Sprint 9 |
| Session restore | ❌ | ❌ | ❌ | ✅ | ✅ Sprint 8 |

---

*Last updated: 2026-02-28 — 🏆 ALL SPRINTS COMPLETE (Sprint 1–10)*
