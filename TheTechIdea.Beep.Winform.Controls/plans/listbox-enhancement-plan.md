# BeepListBox — Commercial-Grade Enhancement Plan

> **Author**: Copilot · **Date**: 2026-03-01  
> **Reference Standards**: Figma 2026 component systems · Material Design 3 · Fluent UI 2 / WinUI 3 · DevExpress XtraListBox / WPF ListBox · Telerik RadListBox · Syncfusion SfListBox · shadcn/ui · Radix UI · Chakra UI

---

## 1. Current State Audit

### Strengths
| Area | Current capability |
|---|---|
| Painter strategy | 31 `ListBoxType` variants, pluggable `IListBoxPainter` |
| Architecture | Clean partial-class split (Core / Properties / Events / Methods / Drawing) |
| Helper system | Dedicated layout, hit-test and behaviour helpers |
| Selection | Single, MultiSimple, MultiExtended, checkbox-based |
| Search | `ShowSearch` / `SearchText` with layout-cached filtering |
| Animation | Hover fade timer, `EnableHoverAnimation` |
| Data | `BindingList<SimpleItem>` with change notifications |

### Gaps vs. Commercial Standards (2026)

| Gap | Impact | Reference |
|---|---|---|
| No virtual scrolling — all items laid out and painted at once | Perf: fails at 500+ items | DevExpress, WinUI VirtualizingStackPanel |
| No item grouping with collapsible headers | Grouped painter exists but grouping is only visual, not data-driven | Fluent ListBox, Telerik |
| No per-item sub-text / metadata line | Modern lists always show 2–3 lines | Material 3 List component |
| No per-item badge / count chip | Notifications, counts invisible | Fluent, shadcn |
| No drag-to-reorder | Standard in every commercial app | DevExpress, Telerik |
| No keyboard navigation parity (Home/End/PgUp/PgDn/Ctrl+A/Space) | WCAG 2.2 Level AA required | MDN, ARIA listbox pattern |
| No ARIA-equivalent AccessibleObject per item | Screen-reader incompatible | WCAG 2.2 |
| No empty-state illustration | Design gap — blank canvas shown | Figma, shadcn |
| No multi-select progress / "N selected" status bar | Invisible multi-select state | Telerik, DevExpress |
| No item context menu (right-click) | Expected in all commercial list controls | DevExpress XtraListBox |
| No inline item editing (F2 / double-click) | Missing edit-in-place | WinForms ListBox, Telerik |
| `IListBoxPainter.DrawContent` still referenced in Readme vs actual `Paint` signature | Interface docs mismatch | — |
| No `BeginUpdate()/EndUpdate()` batch API (only internal delayed invalidate) | Public API gap | Every VCL/WinForms list control |
| No item height virtualisation (non-uniform heights) | Cards / avatars all fixed height | DevExpress, Telerik variable height |
| No freeze/pinned rows | Cannot pin header items | Excel-style pinning |
| No footer / summary row | Count / sum row missing | DataGridView, DevExpress |
| SearchText acts only as filter; no highlight of matched text | UX expectation: matched text highlighted | VS Code, Chrome, shadcn |
| No lazy image loading for icon/avatar | Avatar list painter loads all images eagerly | Telerik async image |
| No skeleton / loading state painter | First-load UX gap | Material 3, shadcn |
| No copy-to-clipboard shortcut (Ctrl+C) | Basic OS expectation | Windows ListBox, DevExpress |
| Hover animation is per-control, not per-item smoothly | Hover jumps between items rather than smooth per-item crossfade | Fluent UI 2 |

---

## 2. Enhancement Sprints

### Sprint 1 — Performance Foundation (Virtual Scrolling)
**Goal**: Support 10,000+ items without lag.

#### 1.1 Viewport Virtualisation
Add a `_virtualOffset` (top pixel of visible window) and `_visibleStartIndex` / `_visibleEndIndex` computed from `ScrollOffset` and item height.

```csharp
// BeepListBox.Core.cs — new fields
private int _virtualOffset;       // top pixel of visible content
private int _visibleCount => (int)Math.Ceiling((double)ContentArea.Height / ItemHeight) + 1;
private int _visibleStartIndex => Math.Max(0, _virtualOffset / ItemHeight);
private int _visibleEndIndex   => Math.Min(_listItems.Count - 1, _visibleStartIndex + _visibleCount);
```

`CalculateItemLayouts` only returns layouts for `[_visibleStartIndex .. _visibleEndIndex]`.

#### 1.2 Custom Vertical Scrollbar
Replace WinForms automatic scroll with a styled thin-track scrollbar that matches Beep themes:
- 6 px track, 6 px thumb, rounded; `ThumbColor = theme.PrimaryColor`.
- Auto-hides after 1.5 s of no scroll activity (like macOS).
- Supports mouse scroll wheel and touch swipe (WM_TOUCH).

#### 1.3 Non-Uniform Item Heights
```csharp
// BeepListBox.Properties.cs
public bool AutoItemHeight { get; set; } = false;   // when true, painters can return variable heights
```
Each painter returns a height for a specific item:
```csharp
int GetItemHeight(SimpleItem item);  // added to IListBoxPainter
```
Layout helper accumulates running Y offsets for non-uniform mode.

#### 1.4 `BeginUpdate()` / `EndUpdate()` Public API
```csharp
public void BeginUpdate() { _updateDepth++; }
public void EndUpdate()   { if (--_updateDepth == 0) { _needsLayoutUpdate = true; RequestDelayedInvalidate(); } }
```

---

### Sprint 2 — Data Model Upgrade
**Goal**: Support real-world data patterns — grouping, sub-text, badges, metadata.

#### 2.1 `BeepListItem` — Rich Item Model
Replace `SimpleItem` usage with an optional rich model (backward compatible):
```csharp
public class BeepListItem : SimpleItem
{
    public string?   SubText         { get; set; }   // second line / metadata
    public string?   BadgeText       { get; set; }   // "3", "New", "●"
    public Color     BadgeColor      { get; set; } = Color.Empty;
    public string?   Category        { get; set; }   // group key
    public bool      IsPinned        { get; set; }   // pinned item (stays at top)
    public bool      IsDisabled      { get; set; }   // greyed out, not selectable
    public bool      IsSeparator     { get; set; }   // horizontal rule item
    public Color     ItemAccentColor { get; set; } = Color.Empty;  // left-edge accent bar
    public object?   Tag             { get; set; }   // arbitrary user data
}
```

#### 2.2 Data-Driven Grouping
```csharp
// BeepListBox.Properties.cs
public bool         ShowGroups      { get; set; } = false;
public bool         CollapsibleGroups { get; set; } = true;
```
`BeepListBoxLayoutHelper.CalculateItemLayouts()` inserts `ListItemInfo { IsGroupHeader = true }` nodes.
Group headers paint their own row (separator line + label + collapse chevron).

#### 2.3 Pinned Items
Items with `IsPinned = true` are extracted to the top of the list (before scroll), never virtualised away.
Visual separator line divides pinned from unpinned section.

#### 2.4 Separator Items
Items with `IsSeparator = true` render as a 1 px horizontal rule with optional label; height = 20 px.

---

### Sprint 3 — Interaction & Keyboard Parity
**Goal**: Full ARIA listbox keyboard contract + standard WinForms shortcuts.

#### 3.1 Keyboard Navigation Matrix

| Key | Action |
|---|---|
| `↑ / ↓` | Move focus one item |
| `Home / End` | Jump to first / last item |
| `Page Up / Page Down` | Scroll one visible page |
| `Space` | Toggle checkbox / select item |
| `Enter` | Confirm selection (raise `ItemActivated`) |
| `Ctrl+A` | Select all (MultiExtended) |
| `Ctrl+C` | Copy selected item text to clipboard |
| `Escape` | Clear selection |
| `F2` | Begin inline edit |
| `Delete` | Raise `ItemDeleteRequested` event |
| Printable chars | Type-ahead search (jumps to first matching item) |

All implemented in `BeepListBox.Keyboard.cs` (new partial file).

#### 3.2 Type-Ahead Search
```csharp
private string _typeAheadBuffer = "";
private readonly Timer _typeAheadClearTimer = new Timer { Interval = 800 };
```
On each printable keypress, append to buffer and scroll to first item whose text starts with buffer.
Timer clears the buffer after 800 ms of inactivity.

#### 3.3 Inline Edit (F2 / Double-Click)
Overlay a borderless `TextBox` over the item's `TitleRect`, pre-populated with `item.Text`.
On `Enter` or focus-lost: commit change, raise `ItemTextChanged`.
On `Escape`: discard.

#### 3.4 Context Menu
```csharp
// BeepListBox.Properties.cs
public bool ShowContextMenu { get; set; } = true;
public ContextMenuStrip?    ItemContextMenu { get; set; }  // consumer-provided
```
Default context menu items: **Select**, **Copy**, **Edit**, separator, **Delete**.
Raises `ContextMenuOpening` with cancellable `ListBoxContextMenuEventArgs` before display.

#### 3.5 Drag-to-Reorder
```csharp
public bool AllowItemReorder { get; set; } = false;
```
- `MouseDown` → start drag (after 4 px intent threshold).
- Ghost image: semi-transparent 60 % alpha copy of the item row painted onto a `Form` ghost window.
- Live insertion indicator: 2 px `PrimaryColor` line drawn between items.
- `MouseUp` → move item in `_listItems`, raise `ItemReordered` event.

---

### Sprint 4 — Visual System Upgrade
**Goal**: All painters meet 2026 Figma / Fluent UI 2 quality bar.

#### 4.1 Design Tokens (`ListBoxTokens.cs`)
```csharp
public static class ListBoxTokens
{
    public const int  ItemHeightComfortable = 48;   // px
    public const int  ItemHeightCompact     = 36;
    public const int  ItemHeightDense       = 28;
    public const int  SubTextAlpha          = 140;  // 55% opacity
    public const int  DisabledAlpha         = 100;  // 39% opacity
    public const int  HoverOverlayAlpha     = 18;   // 7% fill
    public const int  ActiveOverlayAlpha    = 38;   // 15% fill
    public const int  FocusRingThickness    = 2;    // px
    public const int  GroupHeaderHeight     = 28;   // px
    public const int  SeparatorHeight       = 20;   // px
    public const int  BadgePillRadius       = 10;   // px
    public const int  PinnedBarWidth        = 3;    // px left-edge accent
    public const int  AvatarSize            = 36;   // px
    public const int  IconSize              = 20;   // px
    public const int  CheckboxSize          = 18;   // px
}
```

#### 4.2 Per-Item Smooth Hover Crossfade
Replace the single `_hoverProgress` with a `Dictionary<SimpleItem, float> _itemHoverProgress`:
- On `MouseMove`: start fade-in for entered item, fade-out for exited item.
- Timer ticks advance each item's float independently.
- Painter reads from `owner.GetItemHoverProgress(item)`.

#### 4.3 Focus Ring
When the control has keyboard focus, a `ListBoxTokens.FocusRingThickness` px ring in `PrimaryColor` is drawn around the focused item row — separate from hover and selection styling.

#### 4.4 Search Match Highlighting
When `SearchText` is non-empty, the painter calls `DrawHighlightedText(g, text, query, rect, font, normalColor, highlightColor)` — a shared utility that finds match spans and draws them with a `theme.PrimaryColor` background pill (like VS Code search).

#### 4.5 Empty-State Illustration
New `EmptyState.cs` inner class:
```csharp
public class ListBoxEmptyState
{
    public string Headline    { get; set; } = "Nothing here yet";
    public string SubText     { get; set; } = "Add items to see them here";
    public string? IconPath   { get; set; }  // path to SVG/PNG icon
    public bool   Visible     { get; set; } = true;
}
// BeepListBox.Properties.cs
public ListBoxEmptyState EmptyState { get; set; } = new();
```
Painted in `DrawContent` when `_listItems.Count == 0 && !_isLoading`.

#### 4.6 Loading / Skeleton State
```csharp
public bool IsLoading { get; set; } = false;
```
When `true`, painter renders N skeleton rows (animated shimmer gradient from left to right, 1.5 s cycle).

#### 4.7 New Painters (Sprint 4)

| Painter class | `ListBoxType` | Description |  
|---|---|---|
| `InfiniteScrollPainter` | `InfiniteScroll = 32` | Appends "Load more…" sentinel at bottom; raises `LoadMoreRequested` |
| `CommandListPainter` | `CommandList = 33` | VS Code command-palette style: icon + title + kbd shortcut right-aligned |
| `NavigationRailPainter` | `NavigationRail = 34` | Left-rail style with large icons + compact labels (mobile nav) |
| `SkeletonPainter` | internal | Used when `IsLoading = true` in any variant |

---

### Sprint 5 — Accessibility (WCAG 2.2 AA)
**Goal**: Full screen-reader support; keyboard-only operation.

#### 5.1 Per-Item `AccessibleObject`
```csharp
internal class BeepListItemAccessible : AccessibleObject
{
    public override string Name        => $"{_item.Text}, list item {_index + 1} of {_total}";
    public override AccessibleRole Role => AccessibleRole.ListItem;
    public override AccessibleStates State => ComputeState();  // Selected, Focused, Checked, Unavailable
    public override string? Value      => _item is BeepListItem r ? r.SubText : null;
}
```
`BeepListBoxAccessible : AccessibleObject` returns `GetChild(int)` per item.

#### 5.2 Live Region Announcements
- Selection change → `AccessibilityNotifyClients(AccessibleEvents.Selection, selectedIndex)`.
- Item count change → `AccessibilityNotifyClients(AccessibleEvents.Reorder, -1)`.
- Search filter active → announce "N items match [query]".

#### 5.3 High-Contrast Overrides
`BeepListBox.HighContrast.cs` (new partial):
- Detect `SystemInformation.HighContrast`.
- Override hover fill → `SystemColors.Highlight`.
- Override text → `SystemColors.HighlightText`.
- Override border → `SystemColors.WindowFrame`.
- 3 px focus rect in `SystemColors.Highlight`.

#### 5.4 Minimum Touch Target
Each item row minimum height = `Math.Max(ItemHeight, S(44))` — 44 logical px is the WCAG 2.5.5 minimum.

---

### Sprint 6 — API Completeness
**Goal**: Match DevExpress `ListBoxControl` / Telerik `RadListBox` public API surface.

#### 6.1 New Events
```csharp
event EventHandler<ListBoxItemEventArgs>  ItemActivated;          // Enter / double-click
event EventHandler<ListBoxItemEventArgs>  ItemDeleteRequested;    // Delete key
event EventHandler<ListBoxItemEventArgs>  ItemTextChanged;        // after inline edit
event EventHandler<ListBoxReorderEventArgs> ItemReordered;        // after drag reorder
event EventHandler<ListBoxContextMenuEventArgs> ContextMenuOpening; // before right-click menu
event EventHandler<ListBoxSearchEventArgs>  SearchTextChanged;    // overrides current event
event EventHandler                        LoadMoreRequested;      // infinite scroll sentinel hit
event EventHandler<ListBoxGroupEventArgs> GroupCollapsed;
event EventHandler<ListBoxGroupEventArgs> GroupExpanded;
```

#### 6.2 New Event Arg Types (`ListBoxEventArgs.cs` extended)
```csharp
public class ListBoxItemEventArgs(int index, SimpleItem item) : EventArgs { }
public class ListBoxReorderEventArgs(int oldIndex, int newIndex, SimpleItem item) : EventArgs { }
public class ListBoxContextMenuEventArgs(int index, SimpleItem? item, ContextMenuStrip menu) : EventArgs
    { public bool Cancel { get; set; } }
public class ListBoxGroupEventArgs(string groupKey) : EventArgs { }
public class ListBoxSearchEventArgs(string query, int matchCount) : EventArgs { }
```

#### 6.3 Density Mode
```csharp
public enum ListDensityMode { Comfortable, Compact, Dense }
public ListDensityMode Density { get; set; } = ListDensityMode.Comfortable;
```
Maps to token heights (48 / 36 / 28 px). Updates `ItemHeight` and re-layouts.

#### 6.4 Scroll API
```csharp
public void ScrollToItem(SimpleItem item);
public void ScrollToIndex(int index);
public void ScrollToTop();
public void ScrollToBottom();
public int  FirstVisibleIndex { get; }
public int  LastVisibleIndex  { get; }
```

#### 6.5 Data Binding Mode
```csharp
public object?   DataSource    { get; set; }   // IList or IBindingList
public string    DisplayMember { get; set; } = "";
public string    ValueMember   { get; set; } = "";
public object?   SelectedValue { get; }
```
When `DataSource` is set, `ListItems` is auto-populated from the source; changes in the source notify the list (via `IBindingList.ListChanged`).

---

### Sprint 7 — Painter Quality Pass
**Goal**: Every existing painter meets the 2026 visual bar; no "placeholder" painters.

#### 7.1 Audit Checklist per Painter
Each painter must:
- [ ] Use `ListBoxTokens` for all dimensions (no magic numbers).  
- [ ] Support `BeepListItem.SubText` (2-line layout).  
- [ ] Support `BeepListItem.BadgeText` (pill badge top-right of item).  
- [ ] Respect `density` token heights.  
- [ ] Support `IsDisabled` (greyed text, no hover/select effects).  
- [ ] Implement `GetItemHeight(SimpleItem)` returning variable heights.  
- [ ] Handle `IsSeparator` rows.  
- [ ] Draw group headers when `ShowGroups = true`.  
- [ ] Draw search-match highlights.  
- [ ] Draw empty-state when list is empty.  
- [ ] Draw skeleton rows when `IsLoading = true`.  
- [ ] Draw focus ring on focused item.  
- [ ] Pass accessibility colour contrast ratio ≥ 4.5:1.

#### 7.2 Priority Painters (highest usage, fix first)
1. `StandardListBoxPainter` — baseline, touches all new features
2. `CheckboxListPainter` — data-entry critical
3. `CardListPainter` + `GradientCardListBoxPainter` — visually prominent
4. `AvatarListBoxPainter` — lazy image loading required
5. `SearchableListPainter` — match highlighting required
6. `GroupedListPainter` — data-driven grouping required

#### 7.3 Retire / Merge Duplicates
The following painters are visually near-identical and should be merged behind the same enum value with a sub-variant option:

| Primary | Redundant | Action |
|---|---|---|
| `FilledListBoxPainter` | `FilledStylePainter` | Merge → `FilledListBoxPainter` |
| `OutlinedListBoxPainter` | `MaterialOutlinedListBoxPainter` | Keep MUIP as sub-style via `ControlStyle` |
| `CompactListPainter` | `MinimalListBoxPainter` | Merge; density mode replaces Compact type |
| `StandardListBoxPainter` | `SimpleListPainter` | Simple = Standard with `Dense` density |

---

## 3. Non-Functional Targets

| Metric | Target |
|---|---|
| Paint time @ 50 visible items | < 3 ms per frame |
| Scroll FPS @ 500 items (virtualised) | ≥ 60 fps |
| First-render @ 10,000 items | < 50 ms (virtual) |
| Memory per 10,000 items (no images) | < 2 MB |
| Keyboard navigation response | < 16 ms (1 frame) |
| Accessibility: WCAG level | AA (2.2) |
| DPI fidelity | 100 / 125 / 150 / 175 / 200 % |
| Minimum touch target | 44 × 44 logical px |

---

## 4. UX Principles (Figma 2026 / MD3 / Fluent)

1. **Proximity** — checkbox, icon, title, sub-text, badge, and trailing action all occupy a single item row with consistent left-to-right visual weight.
2. **Feedback** — every state change (hover, focus, select, check, disable) has a visible transition within 16 ms.
3. **Density** — three canonical densities replace the sprawl of fixed-height painters; the same painter adapts.
4. **Hierarchy** — group headers have stronger visual weight than items; pinned items have a left-edge accent bar.
5. **Reversibility** — drag-reorder shows undo affordance; delete raises an event before committing.
6. **Keyboard parity** — every action achievable with a mouse is achievable with the keyboard.
7. **Theme fidelity** — no raw `Color.FromArgb` in painter code; all colours from `IBeepTheme` via `ListBoxTokens`.
8. **Progressive disclosure** — badges, sub-text, context menu, inline edit only appear when configured; default surface is clean.

---

## 5. File Impact Map

| File | Sprint | Change |
|---|---|---|
| `ListBoxType.cs` | 4 | Add `InfiniteScroll = 32`, `CommandList = 33`, `NavigationRail = 34` |
| `ListBoxEventArgs.cs` | 6 | Add all new event arg classes |
| `IListBoxPainter.cs` | 1, 7 | Add `GetItemHeight(SimpleItem)` |
| `BeepListBox.Core.cs` | 1,2 | Virtual scroll fields, `_itemHoverProgress` dict |
| `BeepListBox.Properties.cs` | 2,3,4,6 | `BeepListItem`, `ShowGroups`, `IsLoading`, `EmptyState`, `Density`, `DataSource` |
| `BeepListBox.Events.cs` | 3,6 | All new events; context menu handler |
| `BeepListBox.Methods.cs` | 1,3,6 | `BeginUpdate/EndUpdate`, scroll API, `StartInlineEdit` |
| `BeepListBox.Drawing.cs` | 1,4 | Virtual scroll paint region; skeleton/empty-state |
| **NEW** `BeepListBox.Keyboard.cs` | 3 | Full keyboard handler partial |
| **NEW** `BeepListBox.Accessibility.cs` | 5 | `BeepListBoxAccessible`, `BeepListItemAccessible` |
| **NEW** `BeepListBox.HighContrast.cs` | 5 | HC colour overrides |
| **NEW** `BeepListBox.DataBinding.cs` | 6 | `DataSource`, `DisplayMember`, `ValueMember` |
| **NEW** `Tokens/ListBoxTokens.cs` | 4 | All design constants |
| **NEW** `Models/BeepListItem.cs` | 2 | Rich item model |
| **NEW** `Painters/InfiniteScrollPainter.cs` | 4 | Load-more painter |
| **NEW** `Painters/CommandListPainter.cs` | 4 | Command-palette painter |
| **NEW** `Painters/NavigationRailPainter.cs` | 4 | Navigation rail painter |
| **MODIFIED** all 31 painters | 7 | Audit + token + sub-text + badge + variable height |

---

## 6. Sprint Priority Order

| Sprint | Priority | Estimated effort |
|---|---|---|
| Sprint 1 — Virtual Scroll | P0 — blocks perf | Large |
| Sprint 2 — Data Model | P0 — foundation for painters | Medium |
| Sprint 3 — Keyboard & Interaction | P1 — compliance | Medium |
| Sprint 7 — Painter Quality Pass | P1 — visible quality | Large |
| Sprint 4 — Visual System | P2 — design tokens + empty/skeleton | Medium |
| Sprint 5 — Accessibility | P2 — compliance | Medium |
| Sprint 6 — API Completeness | P3 — nice-to-have | Small–Medium |
