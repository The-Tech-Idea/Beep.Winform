# Phase 2 — Item Alignment: Apply Design Tokens & Font Metrics Consistently

**Priority**: High  
**Status**: Not Started  
**Depends on**: None (can be done in parallel with Phase 1)

## Problem

Items in the list box are misaligned in several ways:

1. **Horizontal padding is too tight**: Layout helper uses `paddingX = ScaleValue(8)` but `ListBoxTokens.ItemPaddingH = 12`
2. **No vertical padding on text**: Text rect Y is set to the row top, not vertically padded within the row
3. **Checkbox size ignores tokens**: Uses `Math.Min(18, Math.Max(14, ImageSize))` instead of `ListBoxTokens.CheckboxSize` (18)
4. **Icon-text gap is wrong**: Uses `paddingX` (8) as the gap instead of `ListBoxTokens.IconTextGap` (10)
5. **Painter hardcodes image size**: `BaseListBoxPainter.DrawItem()` uses `32px` for image and `36px` for gap regardless of `_owner.ImageSize`
6. **GetPreferredItemHeight() ignores Density**: Returns `Max(fontHeight + 12, 36)` instead of density-based token values
7. **GetPreferredPadding() returns hardcoded values**: Returns `Padding(8, 4, 8, 4)` instead of token-based values

### Visual comparison

```
CURRENT (misaligned):
┌──────────────────────────────────┐
│[✓]🔍  Item text starts here      │  ← 8px padding, checkbox size varies
│[✓]🔍 Item with long text that... │  ← icon-text gap = 8px (too tight)
│[✓]🔍   Short                     │  ← text not vertically centered
└──────────────────────────────────┘

TARGET (token-aligned):
┌──────────────────────────────────────┐
│  [✓]  🔍  Item text starts here     │  ← 12px padding, 18px checkbox, 10px gap
│  [✓]  🔍  Item with long text th... │  ← consistent spacing
│  [✓]  🔍  Short                     │  ← text vertically centered in row
└──────────────────────────────────────┘
```

---

## Steps

### Step 2.1 — Fix BeepListBoxLayoutHelper.CalculateLayout() to use ListBoxTokens

**File**: `Helpers/BeepListBoxLayoutHelper.cs`  
**Location**: Metrics section (around lines 55–63)

Change from:
```csharp
int paddingX = DpiScalingHelper.ScaleValue(8, ctrl);
int checkboxSize = Math.Min(DpiScalingHelper.ScaleValue(18, ctrl), Math.Max(DpiScalingHelper.ScaleValue(14, ctrl), _owner.ImageSize));
int checkboxArea = _owner.ShowCheckBox ? (checkboxSize + paddingX) : 0;
int iconSize = _owner.ImageSize;
int iconArea = iconSize > 0 ? (iconSize + paddingX) : 0;
```

Change to:
```csharp
int paddingX = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, ctrl);
int paddingY = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingV, ctrl);
int checkboxSize = DpiScalingHelper.ScaleValue(ListBoxTokens.CheckboxSize, ctrl);
int checkboxArea = _owner.ShowCheckBox ? (checkboxSize + DpiScalingHelper.ScaleValue(ListBoxTokens.IconTextGap, ctrl)) : 0;
int iconSize = DpiScalingHelper.ScaleValue(_owner.ImageSize, ctrl);
int iconTextGap = DpiScalingHelper.ScaleValue(ListBoxTokens.IconTextGap, ctrl);
int iconArea = iconSize > 0 ? (iconSize + iconTextGap) : 0;
```

Add using at top if not present:
```csharp
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
```

**Why**: 
- `ItemPaddingH` (12) provides more breathing room than hardcoded 8
- `CheckboxSize` (18) is consistent regardless of `ImageSize`
- `IconTextGap` (10) is the proper gap between icon and text, not reusing horizontal padding
- Icon size now properly DPI-scaled

---

### Step 2.2 — Fix text rect vertical centering in layout

**File**: `Helpers/BeepListBoxLayoutHelper.cs`  
**Location**: Text area calculation (around lines 93–95)

Change from:
```csharp
int textLeft = x + paddingX + checkboxArea + (iconRect.IsEmpty ? 0 : iconArea);
Rectangle textRect = new Rectangle(textLeft, screenY, Math.Max(0, x + w - textLeft - paddingX), itemHeight);
```

Change to:
```csharp
int textLeft = x + paddingX + checkboxArea + (iconRect.IsEmpty ? 0 : iconArea);
Rectangle textRect = new Rectangle(
    textLeft,
    screenY + paddingY,
    Math.Max(0, x + w - textLeft - paddingX),
    Math.Max(1, itemHeight - paddingY * 2)
);
```

**Why**: Adds vertical padding so text content doesn't touch the row top/bottom edges. The `paddingY` (6px scaled) creates breathing room matching the design tokens.

---

### Step 2.3 — Fix BaseListBoxPainter.DrawItem() image size

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()` method, image drawing section

Change from:
```csharp
if (!string.IsNullOrEmpty(item.ImagePath))
{
    int imgSz = DpiScalingHelper.ScaleValue(32, _owner);
    int imgGap = DpiScalingHelper.ScaleValue(36, _owner);
    var imageRect = new Rectangle(contentRect.X, contentRect.Y, imgSz, imgSz);
    DrawItemImage(g, imageRect, item.ImagePath);
    contentRect.X += imgGap;
    contentRect.Width -= imgGap;
}
```

Change to:
```csharp
if (!string.IsNullOrEmpty(item.ImagePath))
{
    int imgSz = DpiScalingHelper.ScaleValue(_owner.ImageSize, _owner);
    int imgGap = imgSz + DpiScalingHelper.ScaleValue(ListBoxTokens.IconTextGap, _owner);
    int imgY = contentRect.Y + (contentRect.Height - imgSz) / 2; // vertically center
    var imageRect = new Rectangle(contentRect.X, imgY, imgSz, imgSz);
    DrawItemImage(g, imageRect, item.ImagePath);
    contentRect.X += imgGap;
    contentRect.Width -= imgGap;
}
```

**Why**: 
- Uses `_owner.ImageSize` instead of hardcoded 32 — respects the property set by the consumer
- Gap = image size + token gap instead of hardcoded 36
- Image is vertically centered within the content rect

---

### Step 2.4 — Fix GetPreferredItemHeight() to respect Density mode

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `GetPreferredItemHeight()` method

Change from:
```csharp
public virtual int GetPreferredItemHeight()
{
    return Math.Max(_owner.TextFont.Height + 12, 36);
}
```

Change to:
```csharp
public virtual int GetPreferredItemHeight()
{
    if (_owner == null)
        return ListBoxTokens.ItemHeightComfortable;

    int tokenHeight = _owner.Density switch
    {
        ListDensityMode.Dense   => ListBoxTokens.ItemHeightDense,
        ListDensityMode.Compact => ListBoxTokens.ItemHeightCompact,
        _                       => ListBoxTokens.ItemHeightComfortable
    };
    return DpiScalingHelper.ScaleValue(tokenHeight, _owner);
}
```

**Why**: The old implementation ignored the Density property entirely. The new one maps each density mode to its token value and DPI-scales it.

---

### Step 2.5 — Fix GetPreferredPadding() to use tokens

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `GetPreferredPadding()` method

Change from:
```csharp
public virtual Padding GetPreferredPadding()
{
    return new Padding(8, 4, 8, 4);
}
```

Change to:
```csharp
public virtual Padding GetPreferredPadding()
{
    if (_owner == null)
        return new Padding(ListBoxTokens.ItemPaddingH, ListBoxTokens.ItemPaddingV,
                           ListBoxTokens.ItemPaddingH, ListBoxTokens.ItemPaddingV);

    int h = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, _owner);
    int v = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingV, _owner);
    return new Padding(h, v, h, v);
}
```

**Why**: Aligns padding with the design token system instead of hardcoded values.

---

### Step 2.6 — Fix sub-text vertical positioning in DrawItem()

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()` method, sub-text section

Change from:
```csharp
int subTop = primaryRect.Top + (primaryRect.Height / 2);
var subRect = new Rectangle(primaryRect.Left, subTop, primaryRect.Width, Math.Max(0, primaryRect.Bottom - subTop));
```

Change to:
```csharp
int subGap = DpiScalingHelper.ScaleValue(ListBoxTokens.SubTextGap, _owner);
int halfHeight = primaryRect.Height / 2;
// Title occupies top half
var titleRect = new Rectangle(primaryRect.Left, primaryRect.Top, primaryRect.Width, halfHeight);
DrawItemText(g, titleRect, item.Text, textColor, TextFont ?? _owner.TextFont);
// Sub-text occupies bottom half
int subTop = primaryRect.Top + halfHeight + subGap;
var subRect = new Rectangle(primaryRect.Left, subTop, primaryRect.Width, Math.Max(0, primaryRect.Bottom - subTop));
```

**Note**: This step requires restructuring the DrawItem method slightly to avoid drawing the main text twice. The title text should be drawn in the top half when sub-text exists, or centered when it's title-only. Review the full method during implementation.

**Why**: Uses `SubTextGap` token (2px) for proper spacing between title and sub-text lines.

---

## Token Reference

| Token | Value | Used For |
|-------|-------|----------|
| `ItemPaddingH` | 12px | Left/right padding inside each row |
| `ItemPaddingV` | 6px | Top/bottom padding inside each row |
| `IconTextGap` | 10px | Space between icon and text |
| `SubTextGap` | 2px | Space between title and sub-text line |
| `CheckboxSize` | 18px | Fixed checkbox dimension |
| `IconSize` | 20px | Standard icon size (reference) |
| `ItemHeightComfortable` | 52px | Row height for Comfortable density |
| `ItemHeightCompact` | 40px | Row height for Compact density |
| `ItemHeightDense` | 28px | Row height for Dense density |

---

## Acceptance Criteria

- [ ] Horizontal padding is visually 12px (scaled) on both sides
- [ ] Checkboxes are consistently 18px (scaled) regardless of ImageSize
- [ ] Icon-to-text gap is 10px (scaled), not reusing padding value
- [ ] Image size respects `ImageSize` property, not hardcoded 32px
- [ ] Images are vertically centered within row
- [ ] Text is vertically centered within row (with vertical padding)
- [ ] Sub-text appears below title with 2px gap (not overlapping)
- [ ] Density mode (Comfortable/Compact/Dense) produces correct row heights: 52/40/28 px (scaled)
- [ ] Mixed items (text-only, +icon, +checkbox, +subtext) all align consistently
