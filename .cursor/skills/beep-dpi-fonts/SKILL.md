# Beep DPI Scaling & Theme Font Management

## Overview

This skill documents the correct patterns for DPI-aware pixel scaling and theme-sourced font management in Beep.Winform controls. All custom controls must scale pixel values for high-DPI displays and source fonts from the current theme via `ApplyTheme()`.

---

## 1. DpiScalingHelper API

**Namespace:** `TheTechIdea.Beep.Winform.Controls.Helpers`

### Scaling Values (use `this` or a Control reference -- NEVER cache a float)

```csharp
// In a control -- pass `this` directly
int scaled = DpiScalingHelper.ScaleValue(12, this);
Size s     = DpiScalingHelper.ScaleSize(new Size(24, 24), this);
Point p    = DpiScalingHelper.ScalePoint(new Point(10, 10), this);
Rectangle r = DpiScalingHelper.ScaleRectangle(rect, this);
Padding pad = DpiScalingHelper.ScalePadding(new Padding(5), this);
float sf   = DpiScalingHelper.ScaleValue(8.5f, this);

// In a painter/helper -- pass the owner control
int scaled = DpiScalingHelper.ScaleValue(12, ownerControl);
Size s     = DpiScalingHelper.ScaleSize(new Size(24, 24), ownerControl);

// Font scaling (rarely needed -- prefer theme fonts)
Font f = DpiScalingHelper.ScaleFont(font, this);
```

### Getting the Scale Factor (only when you truly need the raw float)

```csharp
float scale = DpiScalingHelper.GetDpiScaleFactor(control);
float scale = DpiScalingHelper.GetDpiScaleFactor(g);  // from Graphics
```

### Refreshing on DPI Change

```csharp
DpiScalingHelper.RefreshScaleFactors(control, ref scaleX, ref scaleY);
```

---

## 2. Theme-Sourced Font Pattern (CRITICAL)

**All fonts MUST come from the theme via `ApplyTheme()`, stored in `_textFont`.** No control or painter should use `new Font(...)` inline or reference `TabControl.Font` / `this.Font` for text rendering.

### Per-Control Theme Font Properties (IBeepTheme)

Each control type has dedicated `TypographyStyle` properties on `IBeepTheme`:

| Control          | Theme Properties                                                                               |
|------------------|-----------------------------------------------------------------------------------------------|
| **Labels**       | `LabelFont`, `SubLabelFont`                                                                   |
| **Buttons**      | `ButtonFont`, `ButtonHoverFont`, `ButtonSelectedFont`                                          |
| **Tabs**         | `TabFont`, `TabSelectedFont`, `TabHoverFont`                                                   |
| **SideMenu**     | `SideMenuTextFont`, `SideMenuTitleFont`, `SideMenuSubTitleFont`                                |
| **Menu**         | `MenuTitleFont`, `MenuItemSelectedFont`, `MenuItemUnSelectedFont`                              |
| **Navigation**   | `NavigationTitleFont`, `NavigationSelectedFont`, `NavigationUnSelectedFont`                     |
| **StarRating**   | `StarTitleFont`, `StarSubTitleFont`, `StarSelectedFont`, `StarUnSelectedFont`                  |
| **Stepper**      | `StepperTitleFont`, `StepperItemFont`, `StepperSubTitleFont`, `StepperSelectedFont`, `StepperUnSelectedFont` |
| **List**         | `ListTitleFont`, `ListSelectedFont`, `ListUnSelectedFont`                                      |
| **Calendar/Date**| `CalendarTitleFont`, `DateFont`, `DaysFont`, `DaysSelectedFont`, `CalendarSelectedFont`, etc.  |
| **CheckBox**     | `CheckBoxFont`, `CheckBoxCheckedFont`                                                          |
| **ComboBox**     | `ComboBoxItemFont`, `ComboBoxListFont`                                                         |
| **TextBox**      | `TextBoxFont`, `TextBoxHoverFont`, `TextBoxSelectedFont`                                       |
| **ProgressBar**  | `ProgressBarFont`                                                                              |
| **ScrollList**   | `ScrollListTitleFont`, `ScrollListSelectedFont`, `ScrollListUnSelectedFont`                    |
| **Tree**         | `TreeTitleFont`, `TreeNodeSelectedFont`, `TreeNodeUnSelectedFont`                              |
| **Badge**        | `BadgeFont`                                                                                    |
| **Generic**      | `BodyMedium`, `BodySmall`, `BodyLarge`, `TitleSmall`, `TitleMedium`, `TitleLarge`, `LabelSmall`, `LabelMedium`, `LabelLarge`, `CaptionStyle`, `ButtonStyle`, `LinkStyle` |

### Converting TypographyStyle to Font

```csharp
// Primary method -- converts TypographyStyle to System.Drawing.Font
Font font = BeepFontManager.ToFont(_currentTheme.TabFont);
```

**`BeepFontManager.ToFont(TypographyStyle)`** reads the `FontFamily`, `FontSize`, and `FontWeight` from the style and returns a cached `Font` instance.

---

## 3. DPI-Aware Control Pattern

**DO NOT cache a `_dpiScale` field.** Use `DpiScalingHelper.ScaleValue(value, this)` directly. The `DeviceDpi` property used internally is already cached by WinForms, so there is no performance concern.

### Scaling Pixel Constants

```csharp
// WRONG: hardcoded pixels
int padding = 12;
Size iconSize = new Size(24, 24);

// RIGHT: DPI-scaled using `this`
int padding = DpiScalingHelper.ScaleValue(12, this);
Size iconSize = DpiScalingHelper.ScaleSize(new Size(24, 24), this);
```

### Setting Fonts in ApplyTheme

```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Set _textFont from the theme's TypographyStyle for THIS control type
    _textFont = BeepFontManager.ToFont(_currentTheme.TabFont);           // for tabs
    // _textFont = BeepFontManager.ToFont(_currentTheme.LabelFont);      // for labels
    // _textFont = BeepFontManager.ToFont(_currentTheme.NavigationUnSelectedFont); // for navbars
    // _textFont = BeepFontManager.ToFont(_currentTheme.ListUnSelectedFont);       // for listboxes
    // etc.
}
```

### Passing to Painters/Helpers

```csharp
// Pass `this` (the control) and _textFont to painters
_painter.OwnerControl = this;
_painter.TextFont = _textFont;

// Or pass as method parameters to helpers
LayoutHelper.CalculateLayout(bounds, this, _textFont);
// Inside helper: DpiScalingHelper.ScaleValue(value, ownerControl)
```

---

## 4. Painter Font Pattern

Painters receive `_textFont` from the owner control. They **NEVER** access `TabControl.Font`, `this.Font`, or create `new Font(...)` inline.

### Correct Painter Pattern

```csharp
public class MyTabPainter : BaseTabPainter
{
    // Received from owner control
    public Font TextFont { get; set; }
    public Control OwnerControl { get; set; }  // or use existing TabControl/Owner reference

    protected void DrawTabText(Graphics g, RectangleF tabRect, string text, bool isSelected)
    {
        // Derive bold from TextFont -- NEVER use TabControl.Font or new Font("Segoe UI", 9)
        using (Font font = isSelected
            ? new Font(TextFont, FontStyle.Bold)
            : TextFont)
        {
            g.DrawString(text, font, brush, tabRect, sf);
        }
    }

    protected void DrawItem(Graphics g, Rectangle bounds)
    {
        // Scale pixel values using the owner control
        int padding = DpiScalingHelper.ScaleValue(8, OwnerControl);
        int iconSize = DpiScalingHelper.ScaleValue(24, OwnerControl);
        // ... use scaled values for layout
    }
}
```

### What Painters Should NOT Do

```csharp
// WRONG: accessing TabControl.Font
using (Font font = new Font(TabControl.Font, FontStyle.Bold)) { ... }

// WRONG: hardcoded font inline
using (Font font = new Font("Segoe UI", 9f)) { ... }

// WRONG: using this.Font
using (Font font = new Font(this.Font, FontStyle.Regular)) { ... }

// WRONG: hardcoded pixel values
int padding = 12;  // Should be DpiScalingHelper.ScaleValue(12, OwnerControl)

// WRONG: caching a _dpiScale field
private float _dpiScale = 1f;  // DO NOT DO THIS
```

---

## 5. Font Helpers Pattern

Control-specific font helpers (e.g., `TabFontHelpers`, `BreadcrumbFontHelpers`, `MenuFontHelpers`) should:

1. Accept `Control ownerControl = null` parameter (NOT `float dpiScale`)
2. Return fonts sourced from the theme's `TypographyStyle` properties
3. Use `BeepFontManager.ToFont()` to convert
4. Scale min/max font sizes with `DpiScalingHelper.ScaleValue(value, ownerControl)` when applying constraints

```csharp
public static Font GetTabFont(Control ownerControl, BeepControlStyle style, IBeepTheme theme)
{
    if (theme == null) return BeepFontManager.DefaultFont;
    return BeepFontManager.ToFont(theme.TabFont) ?? BeepFontManager.DefaultFont;
}

public static int GetScaledFontSize(int baseSize, Control ownerControl)
{
    int minSize = ownerControl != null ? DpiScalingHelper.ScaleValue(8, ownerControl) : 8;
    return Math.Max(minSize, baseSize);
}
```

---

## 6. Anti-Patterns (AVOID)

### Caching _dpiScale Fields

```csharp
// BAD: caching a scale field
private float _dpiScale = 1f;
protected override void OnHandleCreated(EventArgs e)
{
    _dpiScale = DpiScalingHelper.GetDpiScaleFactor(this);
}
int padding = DpiScalingHelper.ScaleValue(12, _dpiScale);

// GOOD: use DpiScalingHelper directly with `this`
int padding = DpiScalingHelper.ScaleValue(12, this);
```

### Hardcoded Fonts in Drawing Code

```csharp
// BAD: hardcoded font in OnPaint or painter
g.DrawString(text, new Font("Segoe UI", 9f), brush, rect);

// GOOD: use _textFont from ApplyTheme
g.DrawString(text, _textFont, brush, rect);
```

### Using TabControl.Font or this.Font in Painters

```csharp
// BAD: painter accessing control's Font property
Font f = new Font(TabControl.Font, FontStyle.Bold);

// GOOD: painter using received TextFont
Font f = new Font(TextFont, FontStyle.Bold);
```

### Hardcoded Pixel Values

```csharp
// BAD: not scaled
int padding = 12;
Size icon = new Size(24, 24);

// GOOD: DPI-scaled via control reference
int padding = DpiScalingHelper.ScaleValue(12, this);
Size icon = DpiScalingHelper.ScaleSize(new Size(24, 24), this);
```

### Using float dpiScale Parameters in Helpers

```csharp
// BAD: float parameter
public static int GetPadding(float dpiScale = 1f)
{
    return DpiScalingHelper.ScaleValue(12, dpiScale);
}

// GOOD: Control parameter
public static int GetPadding(Control ownerControl = null)
{
    return ownerControl != null ? DpiScalingHelper.ScaleValue(12, ownerControl) : 12;
}
```

---

## 7. Quick Reference

### Adding DPI Support to a New Control

1. In `ApplyTheme()`: set `_textFont = BeepFontManager.ToFont(_currentTheme.XxxFont);`
2. Replace all hardcoded pixel values with `DpiScalingHelper.ScaleValue(value, this)`
3. Pass `this` and `_textFont` to painters and helpers
4. In painters: use `DpiScalingHelper.ScaleValue(value, OwnerControl)` and `TextFont`
5. In static helpers: use `Control ownerControl = null` parameter, not `float dpiScale`

### Key Using Statements

```csharp
using TheTechIdea.Beep.Winform.Controls.Helpers;          // DpiScalingHelper
using TheTechIdea.Beep.Winform.Controls.FontManagement;   // BeepFontManager
using TheTechIdea.Beep.Vis.Modules;                       // TypographyStyle, IBeepTheme
```
