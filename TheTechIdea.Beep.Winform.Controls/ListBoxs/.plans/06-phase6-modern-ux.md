# Phase 6 — Modern UX Polish: Tooltips, Badges, Separators, Focus Ring, Trailing Meta

**Priority**: Medium  
**Status**: Not Started  
**Depends on**: Phase 2 (token-based layout), Phase 5 (three-zone row model)

## Problem

Several `SimpleItem` and `BeepListItem` model properties are fully defined but have **zero rendering support** in `BaseListBoxPainter` or any derived painter. This creates a gap between the data model (which is rich and modern) and the visual output (which ignores most of it).

### Gap Analysis (Model Properties vs. Painter Support)

| Property | Model Location | Rendered? | Standard Support |
|----------|---------------|-----------|------------------|
| `ToolTip` | `SimpleItem.ToolTip` | **No** | MD3: tooltips on hover. Fluent: InfoTip. |
| `BadgeText` | `SimpleItem.BadgeText` + `BadgeBackColor/ForeColor/Shape` | **No** | MD3: badge/chip in trailing zone |
| `IsSeparator` | `SimpleItem.IsSeparator` / `BeepListItem.IsSeparator` | Checked but **not drawn** | MD3: `<Divider>`. Fluent: horizontal rule. |
| `IsDisabled` | `SimpleItem.IsEnabled` / `BeepListItem.IsDisabled` | Skips selection but **no visual dim** | MD3: 38% opacity. Fluent: greyed + no interaction. |
| `TrailingMeta` | `BeepListItem.TrailingMeta` | **No** | MD3: trailing supporting text. Fluent: secondary text right-aligned. |
| `ShortcutText` | `SimpleItem.ShortcutText` | **No** | Fluent: right-aligned dimmed text. VS Code command palette. |
| Focus ring | `FocusOutlineColor`, `FocusOutlineThickness` | Properties exist, **inconsistent** | WCAG 2.4.7: visible focus indicator required |
| `IsPinned` | `BeepListItem.IsPinned` | **No** | MD3: pinned section at top. Outlook: pinned items. |
| `ItemAccentColor` | `BeepListItem.ItemAccentColor` | **No** | MD3: 3px left accent bar |
| Empty search state | Properties exist (`EmptyStateText`) | Only for empty list, **not for 0 search results** | MD3: "No results" illustration |
| Sub-text in base | `SimpleItem.SubText` | Only some painters, **not base** | MD3: two-line list item standard |

---

## Design Principles

### MD3 Three-Zone Row Model

Every row follows this structure:

```
┌─────────────────────────────────────────────────────────────────┐
│ [accent] [leading zone]  [content zone]        [trailing zone] │
│  3px      checkbox+icon   title                  badge/meta    │
│           (fixed 56dp)    sub-text               shortcut      │
│                           (flex)                 (fixed 48dp)  │
└─────────────────────────────────────────────────────────────────┘
```

### State Layer Opacity (from MD3)
- **Normal**: 0%
- **Hover**: 8% overlay
- **Focus**: 12% overlay + 2px ring  
- **Pressed**: 12% overlay  
- **Selected**: 12% overlay + filled indicator  
- **Disabled**: 38% opacity on entire row content  
- **Dragged**: 16% overlay  

---

## Steps

### Step 6.1 — Add missing tokens to ListBoxTokens

**File**: `Tokens/ListBoxTokens.cs`

Add:
```csharp
// ── Tooltips ──────────────────────────────────────────────────────────────

/// <summary>Delay before showing tooltip on hover (ms). MD3 uses 500ms.</summary>
public const int TooltipDelayMs = 500;

/// <summary>Maximum tooltip display duration (ms).</summary>
public const int TooltipDurationMs = 5000;

// ── Trailing zone ─────────────────────────────────────────────────────────

/// <summary>Width reserved for the trailing zone (badge/meta/shortcut).</summary>
public const int TrailingZoneWidth = 48;

/// <summary>Gap between content zone and trailing zone.</summary>
public const int ContentTrailingGap = 8;

// ── Accent bar ────────────────────────────────────────────────────────────

/// <summary>Width of the left accent color bar (px).</summary>
public const int AccentBarWidth = 3;

// ── Pinned section ────────────────────────────────────────────────────────

/// <summary>Height of the "Pinned" section header row.</summary>
public const int PinnedHeaderHeight = 24;
```

---

### Step 6.2 — Render ToolTip on hover

**File**: `BeepListBox.Events.cs`  
**Method**: `OnMouseMove()` or where hover item is tracked

Add a `ToolTip` component and show it when the hovered item has `ToolTip` set:

```csharp
// Field in Core.cs:
private System.Windows.Forms.ToolTip? _itemToolTip;

// In InitializeComponent or constructor:
_itemToolTip = new System.Windows.Forms.ToolTip
{
    InitialDelay = ListBoxTokens.TooltipDelayMs,
    AutoPopDelay = ListBoxTokens.TooltipDurationMs,
    ReshowDelay = 200,
    ShowAlways = true
};

// In hover tracking logic (OnMouseMove):
if (hoveredItem != _previousHoveredItem)
{
    string tip = hoveredItem?.ToolTip;
    if (!string.IsNullOrEmpty(tip))
    {
        _itemToolTip?.SetToolTip(this, tip);
    }
    else
    {
        _itemToolTip?.SetToolTip(this, null);
    }
}
```

Add property:
```csharp
[Browsable(true)]
[Category("Behavior")]
[Description("Show tooltips from item.ToolTip on hover.")]
[DefaultValue(true)]
public bool ShowTooltips { get; set; } = true;
```

Dispose in `Dispose(bool)`:
```csharp
_itemToolTip?.Dispose();
_itemToolTip = null;
```

---

### Step 6.3 — Render BadgeText in trailing zone

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()`, after drawing text

Add trailing zone rendering:
```csharp
// After content zone rendering, before exiting DrawItem:
string badgeText = item.BadgeText;
if (!string.IsNullOrEmpty(badgeText))
{
    DrawBadge(g, itemRect, item);
}
```

Add helper method:
```csharp
/// <summary>Draws a badge pill in the trailing zone of the row.</summary>
protected virtual void DrawBadge(Graphics g, Rectangle rowRect, SimpleItem item)
{
    string text = item.BadgeText;
    if (string.IsNullOrEmpty(text)) return;

    Color bgColor = item.BadgeBackColor.IsEmpty
        ? (_owner.CurrentTheme?.PrimaryColor ?? Color.DodgerBlue)
        : item.BadgeBackColor;
    Color fgColor = item.BadgeForeColor.IsEmpty ? Color.White : item.BadgeForeColor;

    int fontSize = DpiScalingHelper.ScaleValue(ListBoxTokens.BadgeFontSize, _owner);
    using var font = new Font(_owner.TextFont.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Point);
    var textSize = g.MeasureString(text, font);

    int minWidth = DpiScalingHelper.ScaleValue(ListBoxTokens.BadgeMinWidth, _owner);
    int pillW = Math.Max(minWidth, (int)textSize.Width + 10);
    int pillH = (int)textSize.Height + 4;
    int radius = DpiScalingHelper.ScaleValue(ListBoxTokens.BadgePillRadius, _owner);

    int trailingPad = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, _owner);
    int pillX = rowRect.Right - trailingPad - pillW;
    int pillY = rowRect.Y + (rowRect.Height - pillH) / 2;
    var pillRect = new Rectangle(pillX, pillY, pillW, pillH);

    // Draw pill background
    using var path = Styling.BeepStyling.CreateRoundedRectPath(pillRect, radius);
    using var bgBrush = new SolidBrush(bgColor);
    g.FillPath(bgBrush, path);

    // Draw text centered
    using var fgBrush = new SolidBrush(fgColor);
    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
    g.DrawString(text, font, fgBrush, pillRect, sf);
}
```

---

### Step 6.4 — Render separators

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()`, early exit for separators

Add at the top of `DrawItem()`:
```csharp
// Check for separator
bool isSeparator = item.IsSeparator || (item is BeepListItem bli && bli.IsSeparator);
if (isSeparator)
{
    DrawSeparator(g, itemRect);
    return;
}
```

Add helper:
```csharp
/// <summary>Draws a horizontal separator line (non-selectable divider).</summary>
protected virtual void DrawSeparator(Graphics g, Rectangle rowRect)
{
    int pad = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, _owner);
    int y = rowRect.Y + rowRect.Height / 2;
    Color lineColor = Color.FromArgb(60, _helper.GetTextColor());
    using var pen = new Pen(lineColor, 1f);
    g.DrawLine(pen, rowRect.X + pad, y, rowRect.Right - pad, y);
}
```

---

### Step 6.5 — Render disabled state with reduced opacity

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()`, wrap rendered content

Add opacity handling:
```csharp
bool isDisabled = !item.IsEnabled || (item is BeepListItem richItem && richItem.IsDisabled);
if (isDisabled)
{
    // Apply reduced opacity for disabled items
    var originalCompositing = g.CompositingQuality;
    var colorMatrix = new System.Drawing.Imaging.ColorMatrix
    {
        Matrix33 = ListBoxTokens.DisabledAlpha / 255f  // ~39% opacity
    };
    // ... or simply use alpha on text/icon colors:
}
```

A simpler approach (no ColorMatrix):
```csharp
// In the textColor calculation:
Color textColor = isSelected ? Color.White : _helper.GetTextColor();
if (isDisabled)
{
    textColor = Color.FromArgb(ListBoxTokens.DisabledAlpha, textColor);
}
```

---

### Step 6.6 — Render TrailingMeta and ShortcutText

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()`, after main text drawing

```csharp
// Draw trailing metadata (right-aligned, dimmed)
string trailingMeta = (item is BeepListItem richItem2) ? richItem2.TrailingMeta : null;
string shortcutText = item.ShortcutText;
string trailingText = trailingMeta ?? shortcutText;

if (!string.IsNullOrEmpty(trailingText))
{
    int trailingPad = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, _owner);
    Color trailingColor = Color.FromArgb(ListBoxTokens.SubTextAlpha, textColor);
    
    using var trailingFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size * 0.85f);
    var trailingSize = g.MeasureString(trailingText, trailingFont);
    
    int trailingX = itemRect.Right - trailingPad - (int)trailingSize.Width;
    int trailingY = itemRect.Y + (itemRect.Height - (int)trailingSize.Height) / 2;
    
    using var trailingBrush = new SolidBrush(trailingColor);
    g.DrawString(trailingText, trailingFont, trailingBrush, trailingX, trailingY);
}
```

---

### Step 6.7 — Draw left accent bar

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()`, before content rendering

```csharp
// Draw left accent bar if set
Color accentColor = (item is BeepListItem richAccent) ? richAccent.ItemAccentColor : Color.Empty;
if (!accentColor.IsEmpty && accentColor != Color.Empty)
{
    int barWidth = DpiScalingHelper.ScaleValue(ListBoxTokens.AccentBarWidth, _owner);
    var barRect = new Rectangle(itemRect.X, itemRect.Y + 2, barWidth, itemRect.Height - 4);
    using var accentBrush = new SolidBrush(accentColor);
    g.FillRectangle(accentBrush, barRect);
}
```

---

### Step 6.8 — Consistent focus ring on keyboard navigation

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()` or a new `DrawFocusIndicator()` method

The focus ring should be drawn only when:
- The control has keyboard focus (not mouse focus)
- The item matches `_owner.FocusedIndex`

```csharp
protected virtual void DrawFocusRing(Graphics g, Rectangle itemRect, bool isFocused)
{
    if (!isFocused) return;
    
    Color ringColor = _owner.FocusOutlineColor.IsEmpty
        ? (_owner.CurrentTheme?.PrimaryColor ?? Color.DodgerBlue)
        : _owner.FocusOutlineColor;
    int thickness = _owner.FocusOutlineThickness;
    int radius = DpiScalingHelper.ScaleValue(ListBoxTokens.SelectionCornerRadius, _owner);
    
    // Draw inset ring (2px inside the row)
    var ringRect = new Rectangle(
        itemRect.X + 1, itemRect.Y + 1,
        itemRect.Width - 2, itemRect.Height - 2);
    
    using var pen = new Pen(ringColor, thickness);
    using var path = Styling.BeepStyling.CreateRoundedRectPath(ringRect, radius);
    g.DrawPath(pen, path);
}
```

Call it from `DrawItem()`:
```csharp
// After DrawItemBackgroundEx:
bool isFocused = (_owner.Focused && visibleIndex == _owner.FocusedIndex);
DrawFocusRing(g, itemRect, isFocused);
```

---

### Step 6.9 — Empty search state ("No matches found")

**File**: `BeepListBox.Drawing.cs`  
**Location**: After the painter rendering, when search is active and 0 results

```csharp
// In OnPaint / PaintControl, after painter finishes:
if (_showSearch && !string.IsNullOrWhiteSpace(_searchText))
{
    var visible = _helper?.GetVisibleItems();
    if (visible == null || visible.Count == 0)
    {
        DrawEmptySearchState(g, bounds);
    }
}
```

Add method:
```csharp
private void DrawEmptySearchState(Graphics g, Rectangle bounds)
{
    string headline = "No matches found";
    string subText = $"No items match \"{_searchText}\"";
    
    using var headlineFont = new Font(_textFont.FontFamily, ListBoxTokens.EmptyStateHeadlinePt, FontStyle.Bold);
    using var subFont = new Font(_textFont.FontFamily, ListBoxTokens.EmptyStateSubTextPt);
    
    Color headColor = Color.FromArgb(ListBoxTokens.SubTextAlpha, ForeColor);
    Color subColor = Color.FromArgb(ListBoxTokens.DisabledAlpha, ForeColor);
    
    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
    
    int cy = bounds.Y + bounds.Height / 2;
    var headRect = new Rectangle(bounds.X, cy - 20, bounds.Width, 24);
    var subRect = new Rectangle(bounds.X, cy + 4, bounds.Width, 20);
    
    using var headBrush = new SolidBrush(headColor);
    using var subBrush = new SolidBrush(subColor);
    g.DrawString(headline, headlineFont, headBrush, headRect, sf);
    g.DrawString(subText, subFont, subBrush, subRect, sf);
}
```

---

### Step 6.10 — Pinned items sorting

**File**: `Helpers/BeepListBoxHelper.cs`  
**Location**: `GetVisibleItems()`, before returning

Add pinned item promotion:
```csharp
// Before returning the final list:
if (items.Any(i => i is BeepListItem bi && bi.IsPinned))
{
    var pinned = items.Where(i => i is BeepListItem bi && bi.IsPinned).ToList();
    var unpinned = items.Where(i => !(i is BeepListItem bi && bi.IsPinned)).ToList();
    items = new List<SimpleItem>(pinned.Count + unpinned.Count + 1);
    items.AddRange(pinned);
    // Optionally add a separator between pinned and unpinned
    items.AddRange(unpinned);
}
```

---

### Step 6.11 — Sub-text rendering in BaseListBoxPainter

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()` text section

Ensure base painter renders sub-text for all items, not just specific painter variants:
```csharp
string subText = (item is BeepListItem richSub) ? richSub.SubText : item.SubText;
if (!string.IsNullOrEmpty(subText))
{
    // Split content rect into title (top ~60%) and sub-text (bottom ~40%)
    int titleHeight = contentRect.Height * 3 / 5;
    var titleRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width, titleHeight);
    var subRect = new Rectangle(
        contentRect.X,
        contentRect.Y + titleHeight + DpiScalingHelper.ScaleValue(ListBoxTokens.SubTextGap, _owner),
        contentRect.Width,
        contentRect.Height - titleHeight);
    
    DrawItemText(g, titleRect, item.Text, textColor, TextFont ?? _owner.TextFont);
    
    Color subColor = Color.FromArgb(ListBoxTokens.SubTextAlpha, textColor);
    using var subFont = new Font((_owner.TextFont ?? SystemFonts.DefaultFont).FontFamily,
        (_owner.TextFont ?? SystemFonts.DefaultFont).Size * 0.85f);
    DrawItemText(g, subRect, subText, subColor, subFont);
}
else
{
    // Single-line title, vertically centered
    DrawItemText(g, contentRect, item.Text, textColor, TextFont ?? _owner.TextFont);
}
```

---

## Token Reference

| Token | Value | Used For |
|-------|-------|----------|
| `TooltipDelayMs` | 500ms | Delay before showing item tooltip |
| `TooltipDurationMs` | 5000ms | Maximum tooltip display time |
| `TrailingZoneWidth` | 48px | Reserved width for badges/meta |
| `ContentTrailingGap` | 8px | Gap between content and trailing zone |
| `AccentBarWidth` | 3px | Left accent color bar width |
| `PinnedHeaderHeight` | 24px | "Pinned" section header row |
| `BadgePillRadius` | 10px | Badge pill corner radius |
| `BadgeFontSize` | 9pt | Badge label font size |
| `BadgeMinWidth` | 20px | Minimum badge width |
| `SubTextAlpha` | 140 (55%) | Secondary text opacity |
| `DisabledAlpha` | 100 (39%) | Disabled item opacity |
| `SelectionCornerRadius` | 6px | Focus ring corner radius |

---

## Standards Reference

### Material Design 3
- [List component spec](https://m3.material.io/components/lists/overview)
- Three-line list: leading element + headline + supporting text + trailing element
- State layers: hover 8%, focus 12%, pressed 12%, dragged 16%
- Disabled: 38% opacity, no interaction

### Fluent UI 2
- [List component](https://fluent2.microsoft.design/components/web/react/list)
- Single/multi-line items with secondary actions
- Focus ring: 2px solid, 2px offset, high contrast friendly

### WCAG 2.2 AA
- **2.4.7 Focus Visible**: Focus indicator must be visible on keyboard navigation
- **1.4.3 Contrast**: 4.5:1 for text, 3:1 for UI components
- **2.5.5 Target Size**: 44×44 CSS pixels for pointer targets

### Figma Best Practices
- Auto-layout with consistent padding/spacing
- Component variants for each state (default, hovered, focused, selected, disabled)
- Trailing slot as a separate auto-layout frame

---

## Acceptance Criteria

- [ ] `ToolTip` set on item → tooltip appears after 500ms hover, disappears after 5s
- [ ] `BadgeText` set → colored pill rendered in trailing zone (right-aligned, vertically centered)
- [ ] `BadgeShape = Circle` → circular badge; default → pill shape
- [ ] `IsSeparator = true` → thin horizontal line drawn, item not selectable/hoverable
- [ ] `IsEnabled = false` or `IsDisabled = true` → item at 39% opacity, skips selection, skips keyboard focus
- [ ] `TrailingMeta` set → right-aligned dimmed text
- [ ] `ShortcutText` set → right-aligned text (dimmed, smaller font)
- [ ] Badge and TrailingMeta on same item → badge takes priority (or both render if space allows)
- [ ] `ItemAccentColor` set → 3px left bar rendered
- [ ] Focus ring visible only on keyboard navigation (not mouse click)
- [ ] Focus ring uses `FocusOutlineColor` and `FocusOutlineThickness` properties
- [ ] Search filtering to 0 results → "No matches found" message with search query shown
- [ ] `IsPinned = true` → pinned items promoted to top of list
- [ ] Sub-text renders in base painter for all 34 variants (not just specific ones)
- [ ] All features degrade gracefully when unused (no visual artifacts from empty properties)
