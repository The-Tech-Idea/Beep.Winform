# BeepDisplayContainer2

A modern, self-contained tab container control for WinForms applications.  
Inherits from `BaseControl` and implements `IDisplayContainer`.

---

## Features

| Category | Feature |
|---|---|
| **Tab Styles** | Classic, Capsule, Underline, Minimal, Segmented, Card, Button |
| **Tab Positions** | Top, Bottom, Left, Right |
| **ControlStyle Mapping** | Tab style auto-derived from `BeepControlStyle` |
| **Theme** | Full `IBeepTheme` integration; font, colors, radii |
| **Rounded Corners** | Shape-aware rendering — rounded edges only on outer boundary |
| **Auto Tab Height** | Measured from live font metrics (`AutoTabHeight = true`) |
| **Scroll Buttons** | Left/Right/Up/Down + New-tab button with hover/pressed/disabled states |
| **Close Buttons** | Per-tab close button with hover state |
| **Context Menu** | Right-click: Close Tab, Close Other Tabs, Close All Tabs |
| **Empty State** | Placeholder icon + message when no tabs are open |
| **Active Indicator** | 3 px DPI-scaled pill bar on the content-facing edge of the active tab |
| **Slide Animation** | Indicator bar lerps (ease-out) from previous to new tab on activation |
| **Keyboard Navigation** | Arrow keys, Home/End, Ctrl+Tab, Ctrl+Shift+Tab, Ctrl+W |
| **Drag to Reorder** | Mouse drag reorders tabs with a ghost preview + drop indicator |
| **Gradient Strip** | Optional `LinearGradientBrush` fill for the tab strip background |
| **Content Bleed Fix** | Content area uses `Region` clip; 1 px seam extension seals the boundary |
| **Theme Propagation** | `ApplyTheme()` propagates to all hosted addin controls in one pass |
| **Batch Mode** | `BeginUpdate()` / `EndUpdate()` suppresses layout/paint flicker |

---

## Key Properties

### Appearance

| Property | Type | Default | Notes |
|---|---|---|---|
| `TabStyle` | `TabStyle` | `Capsule` | Visual style of the tabs |
| `TabPosition` | `TabPosition` | `Top` | Strip placement |
| `AutoTabHeight` | `bool` | `true` | Derive height from font metrics |
| `ShowCloseButtons` | `bool` | `true` | Per-tab close button |
| `ShowEmptyState` | `bool` | `true` | Placeholder when no tabs open |
| `EmptyStateText` | `string` | *"No tabs open …"* | Message below the placeholder icon |
| `UseTabStripGradient` | `bool` | `false` | Gradient instead of solid strip fill |
| `TabStripGradientEndColor` | `Color` | `Color.Empty` | End color; auto-derived when Empty |
| `IsRounded` | `bool` | `true` | Rounded outer corners |
| `BorderRadius` | `int` | `8` | Corner radius in pixels |

### Behavior

| Property | Type | Default | Notes |
|---|---|---|---|
| `EnableKeyboardNavigation` | `bool` | `true` | Arrow/Ctrl+Tab navigation and Ctrl+W close |
| `AllowTabReordering` | `bool` | `true` | Drag-to-reorder tabs |
| `EnableAnimations` | `bool` | `true` | Indicator transition animation |
| `AnimationSpeed` | `AnimationSpeed` | `Normal` | Animation tick speed |

---

## Keyboard Shortcuts

| Key | Action |
|---|---|
| `←` / `→` (horizontal) | Previous / Next tab |
| `↑` / `↓` (vertical) | Previous / Next tab |
| `Home` | First tab |
| `End` | Last tab |
| `Ctrl+Tab` | Next tab (wraps) |
| `Ctrl+Shift+Tab` | Previous tab (wraps) |
| `Ctrl+W` | Close active tab (if `CanClose`) |

The control must have keyboard focus. Click on the control or tab into it.

---

## Drag-to-Reorder

Hold the left mouse button and drag a tab horizontally (or vertically for Left/Right strips).  
After moving more than **5 px** from the drag origin:
- The original tab slot is hidden.
- A **semi-transparent ghost** follows the cursor at 55 % opacity.
- A **2 px drop indicator** (with arrowhead cap) shows the insertion point.
- Releasing the mouse commits the new order; `RecalculateLayout()` fires automatically.

Set `AllowTabReordering = false` to disable.

---

## Gradient Tab Strip

```csharp
container.UseTabStripGradient    = true;
container.TabStripGradientEndColor = Color.Empty; // auto: 8% darker than tab back color
// — or —
container.TabStripGradientEndColor = Color.FromArgb(255, 230, 240, 255);
```

The gradient direction follows the tab position:
- **Top / Bottom** → horizontal (`Left → Right`)
- **Left / Right** → vertical (`Top → Bottom`)

---

## Partial Class Structure

| File | Responsibility |
|---|---|
| `BeepDisplayContainer2.cs` | Fields, constructor, `ApplyTheme`, animation tick |
| `BeepDisplayContainer2.Properties.cs` | All public properties |
| `BeepDisplayContainer2.Mouse.cs` | Mouse events, keyboard nav, drag-to-reorder helpers |
| `BeepDisplayContainer2.Painting.cs` | All GDI+ rendering |
| `BeepDisplayContainer2.Layout.cs` | `RecalculateLayout`, auto tab height, content area |
| `BeepDisplayContainer2.TabManagement.cs` | `AddTab`, `RemoveTab`, `ActivateTab` |
| `BeepDisplayContainer2.Theme.cs` | `ApplyThemeColorsToTabs`, color helpers |
| `BeepDisplayContainer2.Events.cs` | Event declarations |
| `BeepDisplayContainer2.IDisplayContainer.cs` | Interface implementation |
| `BeepDisplayContainer2.Dispose.cs` | `Dispose` / cleanup |
| `BeepDisplayContainer2.Models.cs` | `AddinTab` model |
| `Helpers/TabPaintHelper.cs` | Per-tab paint logic, active indicator |
| `Helpers/TabLayoutHelper.cs` | Tab bounds / overflow calculation |
| `Helpers/TabAnimationHelper.cs` | Per-tab hover animation values |

---

## Enhancement History

| Priority | Item | Status |
|---|---|---|
| P0 | Rounded-corner clip + shape awareness | ✅ Done |
| P0 | Theme propagation — single `ApplyTheme()` pass | ✅ Done |
| P0 | Tab font from `theme.TabFont` | ✅ Done |
| P1 | Auto tab height from font metrics | ✅ Done |
| P1 | Content-area bleed fix (`Region` clip) | ✅ Done |
| P2 | Scroll / new-tab button visual feedback (hover/pressed/disabled) | ✅ Done |
| P2 | Empty-state placeholder | ✅ Done |
| P3 | Active-tab indicator line (position-aware) | ✅ Done |
| P3 | Sliding indicator transition animation | ✅ Done |
| P4 | **Keyboard navigation** | ✅ Done |
| P4 | **Drag-to-reorder tabs** | ✅ Done |
| P4 | **Gradient tab strip background** | ✅ Done |
