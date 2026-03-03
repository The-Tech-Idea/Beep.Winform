# Widget Painters Enhancement Plan
> Based on Figma design standards, Material Design 3, and internal codebase audit.  
> Audit date: 2025 · Painters analyzed: 108 files across 12 categories

---

## Table of Contents
1. [Executive Summary](#1-executive-summary)
2. [DPI & Scaling Audit](#2-dpi--scaling-audit)
3. [Scroll Support Strategy](#3-scroll-support-strategy)
4. [GDI Allocation Hotfixes](#4-gdi-allocation-hotfixes)
5. [Navigation Overflow Patterns](#5-navigation-overflow-patterns)
6. [Category-by-Category Task Table](#6-category-by-category-task-table)
7. [WidgetPainterBase Enhancements](#7-widgetpainterbase-enhancements)
8. [Implementation Phases](#8-implementation-phases)
9. [Design Token Reference](#9-design-token-reference)
10. [Acceptance Criteria](#10-acceptance-criteria)

---

## 1. Executive Summary

108 widget painters across 12 categories share three systemic problems:

| Problem | Impact | Affected painters |
|---|---|---|
| **Hardcoded pixel constants** — font sizes (8f, 9f, 11f), row heights (24px, 28px, 32px), spacings | Blurry/tiny text and clipped layouts on HiDPI (125 %, 150 %, 200 % scale) | All 108 |
| **No scroll support** — painters overflow content silently below the bounding box | Navigation, list, and calendar painters are unusable with >5–8 items | ~25 painters |
| **GDI created per paint call** — `new Font(…)`, `new SolidBrush(…)`, `new Pen(…)` inside `DrawContent` / `DrawBackground` | GC pressure, memory fragmentation, possible GDI handle exhaustion | ~60 painters |

Existing plan docs cover DrawingRect migration (`fixplan.md`) and WidgetPainterBase hit-area patterns (`plan.instructions.md`). **This plan extends those with DPI scaling and scroll.**

---

## 2. DPI & Scaling Audit

### 2.1 Problem Description

WinForms logical pixels equal physical pixels at 96 DPI (100 %). At 150 % the OS reports `g.DpiX = 144`. Any hardcoded pixel constant drawn without scaling appears ~33 % too small.

Painters currently do:
```csharp
// BAD – hardcoded, fails on HiDPI
int itemHeight = 36;
using var navFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
```

### 2.2 Correct Approach – Logical Pixel Helper

Add the following static helper to `WidgetPainterBase`:

```csharp
/// <summary>
/// Convert a logical dp (96-DPI baseline) to physical pixels at the current DPI.
/// Use for sizes, spacings, rect dimensions — NOT for font sizes (use BeepThemesManager.ToFont instead).
/// </summary>
protected int Dp(int logicalPx)
{
    float dpi = Owner?.DeviceDpi ?? 96f;
    return (int)Math.Round(logicalPx * dpi / 96f);
}
protected float Dpf(float logicalPx)
{
    float dpi = Owner?.DeviceDpi ?? 96f;
    return logicalPx * dpi / 96f;
}
```

> `Control.DeviceDpi` is available since .NET Framework 4.7 / .NET Core 3.0.  
> If `Owner` is null during layout, fall back to `96f`.  
> **For fonts**, always use `BeepThemesManager.ToFont(style, applyDpiScaling: true)` — it handles DPI scaling internally and pulls the family from the active theme.

### 2.3 Font Caching Rule

**Rule**: No `new Font(…)` inside `Draw*` or `AdjustLayout`. Never construct fonts manually — use `BeepThemesManager.ToFont()` which:
- Resolves the font family from the active theme (via `BeepFontManager`)
- Applies system DPI scaling when `applyDpiScaling: true`
- Handles bold, underline, strikeout via `TypographyStyle` properties

```csharp
// CORRECT – obtain fonts from BeepThemesManager, cache in Initialize / RebuildFonts
private Font? _navFont;
private Font? _labelFont;

public override void Initialize(BaseControl owner, IBeepTheme theme)
{
    base.Initialize(owner, theme);
    RebuildFonts();
}

protected override void RebuildFonts()
{
    _navFont?.Dispose();
    _labelFont?.Dispose();

    // Use BeepThemesManager.ToFont with applyDpiScaling:true
    // Pass a TypographyStyle from the current theme, or construct one
    var navStyle  = Theme?.SmallStyle  ?? new TypographyStyle { FontSize = 9f };
    var headStyle = Theme?.BodyStyle   ?? new TypographyStyle { FontSize = 11f, FontWeight = FontWeight.Bold };

    _navFont   = BeepThemesManager.ToFont(navStyle,  applyDpiScaling: true);
    _labelFont = BeepThemesManager.ToFont(headStyle, applyDpiScaling: true);
}
```

Implement `IDisposable` to dispose cached fonts when the painter is destroyed.

> **Never** use `Dpf()` for font size arguments — `BeepThemesManager.ToFont` already scales to the system DPI.

### 2.4 Hardcoded Constants Found (grep evidence)

| File | Line pattern | Logical dp value | Fix |
|---|---|---|---|
| `SidebarNavPainter.cs` | `int itemHeight = 36` | 36dp | `Dp(36)` |
| `SidebarNavPainter.cs` | `new Font(…, 9f, …)` | 9sp | `BeepThemesManager.ToFont(Theme.SmallStyle, true)` |
| `SidebarPainter.cs` | `headerRect … height 20`, items `y += 26` | 20dp, 26dp | `Dp(20)`, `Dp(26)` |
| `TaskListPainter.cs` | `Math.Min(28, …)`, `12, 12` checkbox | 28dp, 12dp | `Dp(28)`, `Dp(12)` |
| `TaskListPainter.cs` | `new Font(…, 9f, …)`, `new Font(…, 11f, …)` | 9sp, 11sp | `BeepThemesManager.ToFont(…, true)` |
| `DataTablePainter.cs` | `Math.Min(24, …)` row height | 24dp | `Dp(24)` |
| `DataTablePainter.cs` | `new Font(…, 9f, …)` (×3 call sites) | 9sp | `BeepThemesManager.ToFont(…, true)` |
| `TabsPainter.cs` | tab height constants | various | `Dp(…)` |
| `CalendarTimelineViewPainter.cs` | `new Font(…, 8f, …)` | 8sp | `BeepThemesManager.ToFont(…, true)` |
| `CalendarTimelineViewPainter.cs` | `new Pen(…, 1)`, `new SolidBrush(…)` | — | use PaintersFactory |
| Finance painters (`BalanceCardPainter`, `PaymentCardPainter`) | comment only, no actual scaling | — | add `Dp()` + `BeepThemesManager.ToFont` |
| All 108 | horizontal/vertical spacing ints | various | audit each file |

### 2.5 Figma / Material 3 Grid Reference

Figma 8-point grid:

| Token | DPI-96 px | When to use |
|---|---|---|
| `spacing-xs` | 4 | inner padding, icon margin |
| `spacing-sm` | 8 | component padding |
| `spacing-md` | 16 | section gap |
| `spacing-lg` | 24 | card padding |
| `spacing-xl` | 48 | hero area |
| `item-height-sm` | 32 | dense list rows |
| `item-height-md` | 40 | default list rows / nav items |
| `item-height-lg` | 48 | comfortable list rows |
| `icon-sm` | 16 | inline icons |
| `icon-md` | 20 | nav icons |
| `icon-lg` | 24 | toolbar icons |

All painters should map their constants to these tokens via `Dp(token)`.

---

## 3. Scroll Support Strategy

### 3.1 Current State

Zero painters implement any scroll offset. When content exceeds the bounding rectangle, items are simply clipped/invisible. This is especially critical for:
- **Sidebar / navigation** painters with N loaded menu items
- **List** painters used in data-heavy applications
- **Calendar / Timeline** painters with multi-week/month range

### 3.2 Scroll Mechanism Design

#### 3.2.1 WidgetContext Properties to Add

```csharp
// In WidgetContext
/// <summary>Vertical scroll offset in logical pixels. 0 = top.</summary>
public int ScrollOffsetY { get; set; } = 0;

/// <summary>Horizontal scroll offset in logical pixels. 0 = left.</summary>
public int ScrollOffsetX { get; set; } = 0;

/// <summary>Total virtual content height computed by the painter during AdjustLayout.</summary>
public int TotalContentHeight { get; set; } = 0;

/// <summary>Total virtual content width computed by the painter during AdjustLayout.</summary>
public int TotalContentWidth { get; set; } = 0;
```

#### 3.2.2 WidgetPainterBase Scroll Helpers

```csharp
// In WidgetPainterBase

private const int ScrollBarWidth = 6;   // Figma/Material: thin 6dp track
private const int ScrollThumbMinH = 24; // minimum thumb height

/// <summary>
/// Returns true when the virtual content height exceeds the visible area.
/// </summary>
protected bool NeedsVerticalScroll(WidgetContext ctx)
    => ctx.TotalContentHeight > ctx.ContentRect.Height;

/// <summary>
/// Returns true when the virtual content width exceeds the visible area.
/// </summary>
protected bool NeedsHorizontalScroll(WidgetContext ctx)
    => ctx.TotalContentWidth > ctx.ContentRect.Width;

/// <summary>
/// Clamps ScrollOffsetY to valid range after TotalContentHeight is known.
/// Call at end of AdjustLayout.
/// </summary>
protected void ClampScrollOffset(WidgetContext ctx)
{
    int maxY = Math.Max(0, ctx.TotalContentHeight - ctx.ContentRect.Height);
    ctx.ScrollOffsetY = Math.Clamp(ctx.ScrollOffsetY, 0, maxY);
    int maxX = Math.Max(0, ctx.TotalContentWidth - ctx.ContentRect.Width);
    ctx.ScrollOffsetX = Math.Clamp(ctx.ScrollOffsetX, 0, maxX);
}

/// <summary>
/// Draws a thin Material-3 style vertical scrollbar on the right edge of scrollRect.
/// Call from DrawForegroundAccents.
/// </summary>
protected void DrawVerticalScrollbar(Graphics g, Rectangle scrollRect, WidgetContext ctx, bool hovered = false)
{
    if (!NeedsVerticalScroll(ctx)) return;

    int trackX = scrollRect.Right - Dp(ScrollBarWidth) - Dp(2);
    var trackRect = new Rectangle(trackX, scrollRect.Y + Dp(4), Dp(ScrollBarWidth), scrollRect.Height - Dp(8));

    // Track (very subtle)
    using var trackBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
    using var gp = CreateRoundedPath(trackRect, Dp(3));
    g.FillPath(trackBrush, gp);

    // Thumb size proportional to visible:total ratio
    float ratio = (float)scrollRect.Height / Math.Max(1, ctx.TotalContentHeight);
    int thumbH = Math.Max(Dp(ScrollThumbMinH), (int)(trackRect.Height * ratio));
    int maxThumbY = trackRect.Bottom - thumbH;
    int maxScroll = Math.Max(1, ctx.TotalContentHeight - scrollRect.Height);
    int thumbY = trackRect.Y + (int)((float)ctx.ScrollOffsetY / maxScroll * (maxThumbY - trackRect.Y));

    var thumbRect = new Rectangle(trackRect.X, thumbY, trackRect.Width, thumbH);
    int alpha = hovered ? 140 : 80;
    using var thumbBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
    using var thumbPath = CreateRoundedPath(thumbRect, Dp(3));
    g.FillPath(thumbBrush, thumbPath);
}

/// <summary>
/// Draws a thin horizontal scrollbar on the bottom edge of scrollRect.
/// Call from DrawForegroundAccents.
/// </summary>
protected void DrawHorizontalScrollbar(Graphics g, Rectangle scrollRect, WidgetContext ctx, bool hovered = false)
{
    if (!NeedsHorizontalScroll(ctx)) return;

    int trackY = scrollRect.Bottom - Dp(ScrollBarWidth) - Dp(2);
    var trackRect = new Rectangle(scrollRect.X + Dp(4), trackY, scrollRect.Width - Dp(8), Dp(ScrollBarWidth));

    using var trackBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
    using var gp = CreateRoundedPath(trackRect, Dp(3));
    g.FillPath(trackBrush, gp);

    float ratio = (float)scrollRect.Width / Math.Max(1, ctx.TotalContentWidth);
    int thumbW = Math.Max(Dp(ScrollThumbMinH), (int)(trackRect.Width * ratio));
    int maxThumbX = trackRect.Right - thumbW;
    int maxScroll = Math.Max(1, ctx.TotalContentWidth - scrollRect.Width);
    int thumbX = trackRect.X + (int)((float)ctx.ScrollOffsetX / maxScroll * (maxThumbX - trackRect.X));

    var thumbRect = new Rectangle(thumbX, trackRect.Y, thumbW, trackRect.Height);
    int alpha = hovered ? 140 : 80;
    using var thumbBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
    using var thumbPath = CreateRoundedPath(thumbRect, Dp(3));
    g.FillPath(thumbBrush, thumbPath);
}
```

#### 3.2.3 Mouse Wheel Support Pattern

Add mouse-wheel pass-through in `UpdateHitAreas` (or via `Owner.MouseWheel`):

```csharp
// In each scrollable painter's Initialize():
Owner.MouseWheel += (s, e) =>
{
    if (_lastCtx == null) return;
    int delta = -e.Delta / 3; // ~40px per notch at 120 delta
    _lastCtx.ScrollOffsetY = Math.Clamp(
        _lastCtx.ScrollOffsetY + delta, 0,
        Math.Max(0, _lastCtx.TotalContentHeight - _lastCtx.ContentRect.Height));
    Owner?.Invalidate();
};
```

#### 3.2.4 Content Clipping Pattern

When rendering scrollable items apply the offset then clip to `ContentRect`:

```csharp
// In DrawContent:
var savedClip = g.Clip;
g.SetClip(ctx.ContentRect);

for (int i = 0; i < items.Count; i++)
{
    int y = ctx.ContentRect.Y + i * itemH - ctx.ScrollOffsetY;
    if (y + itemH < ctx.ContentRect.Y) continue; // above viewport
    if (y > ctx.ContentRect.Bottom) break;       // below viewport

    var itemRect = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, itemH);
    // … draw item …
}

g.Clip = savedClip;
```

### 3.3 Painters Requiring Scroll – Priority Order

#### Priority 1 — Navigation (critical, high item count expected)

| Painter | Scroll axis | Notes |
|---|---|---|
| `SidebarNavPainter` | Vertical | Items × 40dp can exceed any panel height |
| `SidebarPainter` | Vertical | Has section headers — scroll must skip over them visually |
| `MenuPainter` | Vertical | Context menus can have 20+ entries |
| `TreeNavigationPainter` | Vertical | Tree nodes are virtually unlimited |

#### Priority 2 — List Painters (data-heavy)

| Painter | Scroll axis | Notes |
|---|---|---|
| `TaskListPainter` | Vertical | Already uses `Math.Min(28, ...)` which compresses items — scroll is better |
| `DataTablePainter` | Vertical | Has paging (good), but scroll supplements for large page sizes |
| `ActivityFeedPainter` | Vertical | Feed grows indefinitely |
| `ProfileListPainter` | Vertical | User lists |
| `RankingListPainter` | Vertical | Leaderboard rows |
| `StatusListPainter` | Vertical | Status entries |

#### Priority 3 — Calendar/Timeline (multi-period)

| Painter | Scroll axis | Notes |
|---|---|---|
| `CalendarTimelineViewPainter` | Horizontal | Multi-day timeline |
| `CalendarMonthViewPainter` | Vertical | Multi-month view |

#### Priority 4 — Media/Gallery

| Painter | Scroll axis | Notes |
|---|---|---|
| `MediaGalleryPainter` | Vertical | Photo grid with many images |
| `PhotoGridPainter` | Vertical | Same |

### 3.4 Scrollbar UX Specification (Figma / Material 3)

```
Track width:  6dp
Track radius: 3dp
Track color:  rgba(0,0,0,0.08)  – nearly invisible

Thumb min H:  24dp
Thumb radius: 3dp
Thumb idle:   rgba(0,0,0,0.31)  (alpha 80)
Thumb hover:  rgba(0,0,0,0.55)  (alpha 140)
Margin from edge: 2dp

Behavior:
  - Auto-hide track when content fits (NeedsScroll == false)
  - Thumb fades in on hover (200ms ease — approximate in WinForms via Invalidate timer if desired)
  - Mouse wheel: 3 × line-height per notch (120 delta = 1 notch)
  - Scroll reserved margin: reserve 8dp right margin in ContentRect when scrollbar visible
```

---

## 4. GDI Allocation Hotfixes

### 4.1 Pattern to Eliminate

```csharp
// BAD — every WM_PAINT creates/destroys GDI objects
public override void DrawContent(Graphics g, WidgetContext ctx)
{
    using var font = new Font(..., 9f);        // WRONG
    using var brush = new SolidBrush(color);   // WRONG
    using var pen = new Pen(color, 1);         // WRONG
    g.DrawString(..., font, brush, rect);
}
```

### 4.2 Correct Pattern — Font via BeepThemesManager, Cache in Initialize

**Fonts** must always come from `BeepThemesManager.ToFont()` — never from `new Font()`. This ensures the correct theme typeface and automatic DPI scaling:

```csharp
// CORRECT
private Font? _itemFont;
private Font? _headerFont;
// NOTE: theme-based color brushes → use PaintersFactory.GetSolidBrush(color)

public override void Initialize(BaseControl owner, IBeepTheme theme)
{
    base.Initialize(owner, theme);
    RebuildFonts();
}

protected override void RebuildFonts()
{
    _itemFont?.Dispose();
    _headerFont?.Dispose();

    _itemFont   = BeepThemesManager.ToFont(Theme?.SmallStyle  ?? new TypographyStyle { FontSize = 9f  }, applyDpiScaling: true);
    _headerFont = BeepThemesManager.ToFont(Theme?.BodyStyle   ?? new TypographyStyle { FontSize = 11f, FontWeight = FontWeight.Bold }, applyDpiScaling: true);
}

public void Dispose()
{
    _itemFont?.Dispose();
    _headerFont?.Dispose();
}
```

### 4.3 PaintersFactory Usage

For theme-color brushes and pens:

```csharp
// CORRECT — PaintersFactory caches; DO NOT DISPOSE
var brush = PaintersFactory.GetSolidBrush(Theme?.PrimaryColor ?? Color.Blue);
var pen   = PaintersFactory.GetPen(Theme?.BorderColor ?? Color.Gray, 1f);
```

> **Rule**: Never pass a `PaintersFactory`-sourced brush/pen to a `using` block or call `.Dispose()` on it.

### 4.4 Icon / Image Rendering — StyledImagePainter

Do **not** call `_imagePainter.DrawSvg(...)` or construct local `ImagePainter` instances inside painters. Use the static `StyledImagePainter` + embedded `SvgsUI` / `Svgs` resource paths:

```csharp
using TheTechIdea.Beep.Icons;  // SvgsUI, Svgs
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;  // StyledImagePainter

// Paint an icon tinted to a theme color:
StyledImagePainter.PaintWithTint(g, iconRect.ToGraphicsPath(), SvgsUI.Home, Theme.PrimaryColor);

// Paint a plain icon:
StyledImagePainter.Paint(g, iconRect.ToGraphicsPath(), SvgsUI.Settings);

// Paint in a circle (avatar, profile pic):
StyledImagePainter.PaintInCircle(g, cx, cy, radius, imagePath, tint: Theme.PrimaryColor);
```

Use `SvgsUI.*` constants for UI icons (arrows, chevrons, navigation):  
```csharp
SvgsUI.ChevronLeft   // scroll-left arrow
SvgsUI.ChevronRight  // scroll-right arrow
SvgsUI.ChevronDown   // expand/dropdown
SvgsUI.ChevronUp     // collapse
SvgsUI.ArrowLeft     // back
SvgsUI.ArrowRight    // forward
SvgsUI.MoreHorizontal // ⋯ overflow
SvgsUI.Home          // dashboard
SvgsUI.Settings      // settings
SvgsUI.Bell          // notifications
SvgsUI.Info          // hint/tooltip
```

Use `Svgs.*` for data/content domain icons, and `SvgsDatasources.*` for datasource icons.

> **Rule**: No `private … ImagePainter _imagePainter` field in any painter. Remove existing instances and replace with `StyledImagePainter` static calls.

### 4.5 Files with Most GDI Allocations (high priority)

A non-exhaustive list based on grep evidence:

- `SidebarNavPainter.cs` — `using var navFont` per item render loop + local `ImagePainter` field
- `SidebarPainter.cs` — fonts and brushes created inside draw loops
- `TaskListPainter.cs` — fonts, pens, brushes in loop body
- `DataTablePainter.cs` — fonts ×3 call sites, brushes everywhere
- `CalendarTimelineViewPainter.cs` — `new Font`, `new Pen`, `new SolidBrush` all in draw
- `TabsPainter.cs` — tab fonts inside loop
- Finance painters — card font/brush per paint

---

## 5. Navigation Overflow Patterns

Beyond vertical scroll, certain navigation painters need special overflow handling when items don't fit horizontally or the component itself is narrow.

### 5.1 TabsPainter / TabContainerPainter — Tab Overflow

**Figma standard**: When tabs exceed the container width, show scroll arrows (chevrons) at the edges.

```
Before:  [Tab 1] [Tab 2] [Tab 3]   [Tab N — clipped]
After:   ◀  [Tab 2] [Tab 3] [Tab 4]  ▶
```

Implementation:
1. Compute `totalTabWidth = sum of all tab widths`
2. If `totalTabWidth > ContentRect.Width`: enter overflow mode
3. Show 24×H arrow buttons at Left and Right edges — draw with `StyledImagePainter.PaintWithTint(g, arrowPath, SvgsUI.ChevronLeft, arrowColor)`
4. Track `_tabScrollOffset` (int, count of tabs scrolled left)
5. Render only visible tabs based on offset
6. Hit-areas for arrows: `"Tabs_ScrollLeft"`, `"Tabs_ScrollRight"`

### 5.2 BreadcrumbPainter — Breadcrumb Collapse

**Figma standard**: When path is too long, collapse middle segments to `…`.

```
Before:  Home > Products > Electronics > Mobile > Smartphones > iPhone 15
After:   Home > … > Smartphones > iPhone 15
```

Implementation:
1. Measure each segment's width
2. While `totalWidth > ContentRect.Width`: remove segments from middle, insert `"…"` node
3. `"…"` is clickable → expands to full breadcrumb (store full path in `WidgetContext`)

### 5.3 WizardStepsPainter & StepIndicatorPainter — Step Compaction

**Figma standard**: When steps > 5, compress label to icon-only with tooltip.

```
Before:  [①Account Setup] [②Verify Email] [③Add Payment] [④Review] [⑤Confirm]
After (narrow): [①] [②] [③] [④] [⑤]  ← labels hidden, tooltip on hover
```

Implementation:
1. If `steps > 5 && availableWidth < steps * 80dp`: switch to compact mode (`_compact = true`)
2. Compact mode: draw numbered circles only, no text beneath
3. Show label in a tooltip-style CallOut on hover

### 5.4 MenuBarPainter — Overflow Menu

**Figma standard**: Toolbar/menu overflow → `More` dropdown (`⋯`).

```
Before:  [File] [Edit] [View] [Tools] [Help] — [clipped items]
After:   [File] [Edit] [View]  ⋯
```

Implementation:
1. Accumulate items left-to-right; when adding next would exceed right edge, stop
2. Add `⋯` button at rightmost position — draw with `StyledImagePainter.PaintWithTint(g, moreRect.ToGraphicsPath(), SvgsUI.MoreHorizontal, Theme.ForeColor)`
3. Click → show remaining items as fly-out list

---

## 6. Category-by-Category Task Table

| Category | Painter | DPI issues | Needs scroll | Overflow pattern | GDI alloc | Priority |
|---|---|---|---|---|---|---|
| **Calendar** | `CalendarMonthViewPainter` | Font 8f, hardcoded row/col heights | Vertical | — | 🔴 High | P1 |
| **Calendar** | `CalendarTimelineViewPainter` | Font 8f, new Pen/Brush per paint | Horizontal | — | 🔴 High | P1 |
| **Calendar** | `CalendarAgendaViewPainter` | Item heights | Vertical | — | Medium | P2 |
| **Calendar** | `CalendarWeekViewPainter` | Column widths | Horizontal | — | Medium | P2 |
| **Calendar** | `CalendarYearViewPainter` | Cell sizes | — | — | Medium | P3 |
| **Chart** | `BarChartPainter` | Bar widths, label fonts | — | — | Medium | P2 |
| **Chart** | `LineChartPainter` | Axis label fonts | Horizontal | — | Medium | P2 |
| **Chart** | `PieChartPainter` | Label fonts | — | — | Low | P3 |
| **Chart** | `RadarChartPainter` | Label fonts | — | — | Low | P3 |
| **Chart** | `AreaChartPainter` | Label fonts | Horizontal | — | Medium | P3 |
| **Control** | `ButtonGroupPainter` | Button heights | — | Horizontal overflow | Low | P3 |
| **Control** | `SearchInputPainter` | Height/font | — | — | Low | P3 |
| **Control** | `SliderPainter` | Track/thumb sizes | — | — | Low | P3 |
| **Dashboard** | `KpiDashboardPainter` | Card sizes | — | — | Medium | P2 |
| **Dashboard** | `OverviewDashboardPainter` | All sizes | — | — | Medium | P2 |
| **Finance** | `BalanceCardPainter` | Comment-only DPI | — | — | Medium | P2 |
| **Finance** | `BudgetTrackerPainter` | Bar/font sizes | — | — | Medium | P2 |
| **Finance** | `ExpenseChartPainter` | Chart element sizes | — | — | Medium | P2 |
| **Finance** | `PaymentCardPainter` | Comment-only DPI | — | — | Medium | P2 |
| **Finance** | `StockTickerPainter` | Font sizes | Horizontal | — | Medium | P2 |
| **Form** | `ContactFormPainter` | Row heights, font sizes | — | — | Medium | P2 |
| **Form** | `FormBuilderPainter` | Field heights | Vertical | — | Medium | P2 |
| **Form** | `LoginFormPainter` | Heights, fonts | — | — | Low | P3 |
| **List** | `ActivityFeedPainter` | Item heights | 🔴 Vertical | — | 🔴 High | P1 |
| **List** | `DataTablePainter` | Row height 24px | Vertical | — | 🔴 High | P1 |
| **List** | `ProfileListPainter` | Avatar/row sizes | 🔴 Vertical | — | Medium | P1 |
| **List** | `RankingListPainter` | Row heights | 🔴 Vertical | — | Medium | P1 |
| **List** | `StatusListPainter` | Row heights | 🔴 Vertical | — | Medium | P1 |
| **List** | `TaskListPainter` | 28px clamp, 9f font | 🔴 Vertical | — | 🔴 High | P1 |
| **Map** | `MapPainter` | Pin sizes, label fonts | — | — | Medium | P3 |
| **Media** | `AudioPlayerPainter` | Control heights | — | — | Low | P3 |
| **Media** | `MediaGalleryPainter` | Photo sizes | 🔴 Vertical | — | Medium | P2 |
| **Media** | `PhotoGridPainter` | Grid cell sizes | 🔴 Vertical | — | Medium | P2 |
| **Media** | `VideoPlayerPainter` | Control heights | — | — | Low | P3 |
| **Metric** | `CounterPainter` | Font sizes | — | — | Low | P3 |
| **Metric** | `GaugePainter` | Arc sizes | — | — | Low | P3 |
| **Metric** | `ProgressPainter` | Bar heights | — | — | Low | P3 |
| **Metric** | `StatCardPainter` | Font/card sizes | — | — | Medium | P2 |
| **Metric** | `TrendmetricPainter` | Font/arrow sizes | — | — | Medium | P2 |
| **Navigation** | `BreadcrumbPainter` | Separator/font sizes | — | 🔴 Collapse ellipsis | Medium | P1 |
| **Navigation** | `MenuBarPainter` | Item heights | — | 🔴 Overflow `⋯` | Medium | P1 |
| **Navigation** | `MenuPainter` | Item heights, 24px → Dp | 🔴 Vertical | — | Medium | P1 |
| **Navigation** | `PaginationPainter` | Button sizes, fonts | — | — | Low | P3 |
| **Navigation** | `ProcessFlowPainter` | Node sizes | — | Horizontal wrap | Medium | P2 |
| **Navigation** | `QuickActionsPainter` | Icon/button sizes | — | Horizontal overflow | Medium | P2 |
| **Navigation** | `SidebarNavPainter` | itemHeight=36, font 9f | 🔴 Vertical | — | 🔴 High | P1 |
| **Navigation** | `SidebarPainter` | Row heights 20/26dp | 🔴 Vertical | — | 🔴 High | P1 |
| **Navigation** | `StepIndicatorPainter` | Step sizes | — | 🟡 Compact mode | Medium | P2 |
| **Navigation** | `TabContainerPainter` | Tab heights, fonts | — | 🟡 Scroll arrows | Medium | P1 |
| **Navigation** | `TabsPainter` | Tab heights, fonts | — | 🟡 Scroll arrows | Medium | P1 |
| **Navigation** | `TreeNavigationPainter` | Node heights | 🔴 Vertical | — | Medium | P1 |
| **Navigation** | `WizardStepsPainter` | Step sizes | — | 🟡 Compact mode | Medium | P2 |
| **Notification** | `AlertBannerPainter` | Font/icon sizes | — | — | Low | P3 |
| **Notification** | `NotificationBellPainter` | Badge/icon sizes | — | — | Low | P3 |
| **Notification** | `ToastNotificationPainter` | Height/font | — | — | Low | P3 |
| **Social** | `CommentFeedPainter` | Avatar/row sizes | 🔴 Vertical | — | Medium | P1 |
| **Social** | `LikesCounterPainter` | Icon/font sizes | — | — | Low | P3 |
| **Social** | `SocialSharePainter` | Button sizes | — | — | Low | P3 |
| **Social** | `UserProfileCardPainter` | Font/avatar sizes | — | — | Low | P3 |

---

## 7. WidgetPainterBase Enhancements

### 7.1 New Members Summary

```csharp
internal abstract class WidgetPainterBase : IWidgetPainter
{
    // --- DPI helpers (NEW) --- use for sizes/spacings/rects ONLY, not for fonts
    protected int   Dp(int logicalPx);
    protected float Dpf(float logicalPx);

    // --- Scroll helpers (NEW) ---
    protected bool NeedsVerticalScroll(WidgetContext ctx);
    protected bool NeedsHorizontalScroll(WidgetContext ctx);
    protected void ClampScrollOffset(WidgetContext ctx);
    protected void DrawVerticalScrollbar(Graphics g, Rectangle scrollRect, WidgetContext ctx, bool hovered = false);
    protected void DrawHorizontalScrollbar(Graphics g, Rectangle scrollRect, WidgetContext ctx, bool hovered = false);

    // --- Font lifecycle (NEW) ---
    // Subclasses override RebuildFonts() and call BeepThemesManager.ToFont(style, applyDpiScaling: true)
    protected virtual void RebuildFonts() { }
    protected virtual void OnThemeChanged(IBeepTheme theme) { Theme = theme; RebuildFonts(); }

    // --- Existing (unchanged) ---
    protected void AddHitAreaToOwner(string name, Rectangle rect, Action? clickAction = null);
    protected void ClearOwnerHitAreas();
    protected bool IsAreaHovered(string areaName);
    protected string? GetHoveredAreaName();
    protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius);
    protected void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int layers = 4, int offset = 2);
    protected void DrawTrendArrow(Graphics g, Rectangle rect, string direction, Color color);
    protected void DrawProgressBar(Graphics g, Rectangle rect, double percentage, Color fillColor, Color backgroundColor);
    protected void DrawValue(Graphics g, Rectangle rect, string value, string units, Font font, Color color, StringAlignment alignment = StringAlignment.Center);
}
```

### 7.2 Key Rules Summary (quick reference)

| What | Correct API | Wrong pattern |
|---|---|---|
| Fonts | `BeepThemesManager.ToFont(style, true)` | `new Font(family, size)` |
| Theme brushes/pens | `PaintersFactory.GetSolidBrush(color)` | `new SolidBrush(color)` inside draw |
| Icon/SVG rendering | `StyledImagePainter.PaintWithTint(g, path, SvgsUI.X, color)` | `_imagePainter.DrawSvg(...)` |
| Icon resource paths | `SvgsUI.ChevronLeft`, `SvgsUI.Home`, etc. | raw string `"home"` |
| Sizes & spacings | `Dp(36)` / `Dpf(9f)` | plain `36` or `9f` constants |
| Font size scaling | Handled internally by `ToFont(…, true)` | `Dpf(9f)` passed to Font ctor |

### 7.2 WidgetContext New Properties Summary

```csharp
// Scroll state (NEW)
int ScrollOffsetY     { get; set; }
int ScrollOffsetX     { get; set; }
int TotalContentHeight { get; set; }
int TotalContentWidth  { get; set; }
```

---

## 8. Implementation Phases

### Phase 1 — Foundation (WidgetPainterBase + WidgetContext)
**Goal**: Add `Dp()`, scroll helpers, and new WidgetContext scroll properties.  
**Files**: `WidgetPainterBase.cs`, `WidgetContext.cs`  
**Effort**: ~2h  
**Status**: ⬜ Not started

### Phase 2 — Scroll: Navigation Painters
**Goal**: Add vertical scroll to 4 navigation painters.  
**Files**: `SidebarNavPainter.cs`, `SidebarPainter.cs`, `MenuPainter.cs`, `TreeNavigationPainter.cs`  
**Pattern**: `TotalContentHeight`, `ScrollOffsetY`, clipping, scrollbar, mouse wheel  
**Effort**: ~4h  
**Status**: ⬜ Not started

### Phase 3 — Scroll: List Painters
**Goal**: Add vertical scroll to 6 list painters.  
**Files**: `TaskListPainter.cs`, `DataTablePainter.cs`, `ActivityFeedPainter.cs`, `ProfileListPainter.cs`, `RankingListPainter.cs`, `StatusListPainter.cs`  
**Effort**: ~4h  
**Status**: ⬜ Not started

### Phase 4 — DPI Fix: P1 Painters (all hardcoded constants)
**Goal**: Replace all hardcoded constants with `Dp()` calls, cache fonts.  
**Files**: All P1 painters from task table (12 painters)  
**Effort**: ~6h  
**Status**: ⬜ Not started

### Phase 5 — Navigation Overflow Patterns
**Goal**: Add arrow overflow to Tabs, ellipsis collapse to Breadcrumb, `⋯` to MenuBar.  
**Files**: `TabsPainter.cs`, `TabContainerPainter.cs`, `BreadcrumbPainter.cs`, `MenuBarPainter.cs`  
**Effort**: ~5h  
**Status**: ⬜ Not started

### Phase 6 — DPI Fix: P2 Painters (remaining categories)
**Goal**: Replace hardcoded constants with `Dp()`, cache fonts across all remaining painters.  
**Files**: All P2 painters from task table (~30 painters)  
**Effort**: ~8h  
**Status**: ⬜ Not started

### Phase 7 — Scroll: Calendar & Media
**Goal**: Add horizontal/vertical scroll to calendar and media gallery painters.  
**Files**: `CalendarTimelineViewPainter.cs`, `CalendarMonthViewPainter.cs`, `MediaGalleryPainter.cs`, `PhotoGridPainter.cs`  
**Effort**: ~3h  
**Status**: ⬜ Not started

### Phase 8 — GDI Hotfixes (all remaining `new Font` in draw loops)
**Goal**: Eliminate all remaining per-paint GDI allocations.  
**Files**: ~60 painters with `new Font/Brush/Pen` in draw methods  
**Effort**: ~8h  
**Status**: ⬜ Not started

---

## 9. Design Token Reference

### Typography Scale (sp = scale-independent pixels, WinForms = logical pt)

| Token | sp (96dpi = pt) | Use case |
|---|---|---|
| `type-xs` | 8 | Timestamps, badges |
| `type-sm` | 9 | Body/label text in dense lists |
| `type-md` | 11 | Default body, nav labels |
| `type-lg` | 13 | Sub-headings, card titles |
| `type-xl` | 16 | Section headings |
| `type-display` | 20–32 | KPI values, metric displays |

All sizes must be applied with `Dpf(size)` to scale with system DPI.

### Color Opacity Scale (alpha values used in painters)

| Role | Alpha | Use |
|---|---|---|
| Surface overlay | 6–8 | Row hover |
| Subtle border | 20 | Card borders |
| Medium overlay | 30 | Active state overlay |
| Icon secondary | 140 | Inactive icons |
| Text secondary | 150–180 | Supporting text |
| Text primary | 255 | Main labels, values |

### Corner Radius (dp)

| Token | dp | Use |
|---|---|---|
| `radius-xs` | 2 | Chips, indicators |
| `radius-sm` | 4–6 | Cards, nav items |
| `radius-md` | 8 | Panels, sidebars |
| `radius-lg` | 12 | Modals, large cards |
| `radius-xl` | 16–24 | Full cards on mobile-style designs |

---

## 10. Acceptance Criteria

### DPI Scaling
- [ ] No hardcoded pixel constant anywhere in any painter's `AdjustLayout`, `DrawBackground`, `DrawContent`, or `DrawForegroundAccents`
- [ ] All spatial sizes/spacings use `Dp(x)` and all font sizes are obtained via `BeepThemesManager.ToFont(style, applyDpiScaling: true)`, cached outside the draw path
- [ ] Painters render correctly at 100 %, 125 %, 150 %, 200 % system DPI
- [ ] No `new Font(…)`, `new SolidBrush(…)`, `new Pen(…)` inside any hot-path draw method

### Scroll
- [ ] All P1 scroll painters: content scrolls when item count × item height > ContentRect.Height
- [ ] Scrollbar renders as 6dp thin Material 3 style with 3dp radius
- [ ] Mouse wheel scrolls 3 × `item-height-md` (= 3 × 40dp = 120dp) per notch
- [ ] Scrollbar auto-hides when `TotalContentHeight <= ContentRect.Height`
- [ ] Keyboard (already implemented in SidebarPainter, TabsPainter) scrolls viewport to keep selection visible
- [ ] `ScrollOffsetY` and `TotalContentHeight` are persisted in `WidgetContext` between paint calls (no reset in AdjustLayout)

### Navigation Overflow
- [ ] TabsPainter: shows `◀ ▶` arrows when tabs exceed container width
- [ ] BreadcrumbPainter: collapses to `…` when total path width exceeds container
- [ ] MenuBarPainter: shows `⋯` overflow button for items that don't fit
- [ ] WizardStepsPainter: switches to icon-only compact mode when steps > 5 in narrow container

### GDI & Rendering
- [ ] Zero `new Font/SolidBrush/Pen` in any draw method path
- [ ] All fonts obtained via `BeepThemesManager.ToFont(style, applyDpiScaling: true)` and cached in fields
- [ ] All `IDisposable` painters properly dispose cached fonts in `Dispose()`
- [ ] No `PaintersFactory`-sourced objects wrapped in `using` or called `.Dispose()`
- [ ] No `private ImagePainter _imagePainter` fields — replaced with `StyledImagePainter` static calls
- [ ] All icon SVG paths use `SvgsUI.*` or `Svgs.*` constants — no raw string icon names

---

*Plan authored by GitHub Copilot · References: fixplan.md, plan.instructions.md, planproperties.md, Material Design 3, Figma 8pt grid.*
