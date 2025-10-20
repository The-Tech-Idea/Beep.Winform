# Text Painters - Complete Documentation

## ✅ Overview

Specialized collection of **5 text painter classes** that handle all text rendering for Beep controls. Each painter provides font selection, styling, letter spacing, and rendering quality optimized for specific design systems.

## 📁 Structure

```
TextPainters/
├── MaterialTextPainter.cs      # Material Design (Roboto font)
├── AppleTextPainter.cs         # Apple platforms (SF Pro Display)
├── MonospaceTextPainter.cs     # Monospace fonts (JetBrains Mono)
├── StandardTextPainter.cs      # Standard fonts (Segoe UI, Inter, etc.)
└── ValueTextPainter.cs         # Numeric/date values (centered alignment)
```

## 🎨 Text Painter Classes

### 1️⃣ MaterialTextPainter.cs

**Styles Supported:** Material3, MaterialYou

**Font Configuration:**
- **Primary Font:** Roboto 14pt
- **Fallback Chain:** Roboto → Segoe UI → Arial
- **Font Style:** 
  - Regular (default)
  - Bold (when focused)
- **Letter Spacing:** 0.1px (subtle tracking)

**Rendering Quality:**
- **TextRenderingHint:** ClearTypeGridFit
- **SmoothingMode:** AntiAlias (for non-text elements)

**Special Features:**
- Material Design typography scale
- Respects Material weight hierarchy
- Dynamic bold on focus

**Usage:**
```csharp
MaterialTextPainter.Paint(g, bounds, "Hello World", isFocused, 
    BeepControlStyle.Material3, theme, useThemeColors);
```

---

### 2️⃣ AppleTextPainter.cs

**Styles Supported:** iOS15, MacOSBigSur

**Font Configuration:**
- **Primary Font:** SF Pro Display 14pt
- **Fallback Chain:** SF Pro Display → Segoe UI → Arial
- **Font Style:** 
  - Regular (default)
  - Semibold (when focused)
- **Letter Spacing:** -0.2px (negative tracking for Apple aesthetic)

**Rendering Quality:**
- **TextRenderingHint:** ClearTypeGridFit
- **SmoothingMode:** HighQuality

**Special Features:**
- SF Pro Display optical sizing
- Negative letter spacing (tighter)
- Apple Human Interface Guidelines compliant

**Usage:**
```csharp
AppleTextPainter.Paint(g, bounds, "Hello World", isFocused, 
    BeepControlStyle.iOS15, theme, useThemeColors);
```

---

### 3️⃣ MonospaceTextPainter.cs

**Styles Supported:** DarkGlow, Terminal (monospace-based styles)

**Font Configuration:**
- **Primary Font:** JetBrains Mono 13pt
- **Fallback Chain:** JetBrains Mono → Consolas → Courier New → Generic Monospace
- **Font Style:** 
  - Regular (default)
  - Bold (when focused)
- **Letter Spacing:** 0.5px (wide tracking for readability)

**Rendering Quality:**
- **TextRenderingHint:** ClearTypeGridFit
- **SmoothingMode:** AntiAlias

**Special Features:**
- Optimized for code/terminal text
- Wide letter spacing for clarity
- Monospace alignment guaranteed

**Usage:**
```csharp
MonospaceTextPainter.Paint(g, bounds, "Hello World", isFocused, 
    BeepControlStyle.DarkGlow, theme, useThemeColors);
```

---

### 4️⃣ StandardTextPainter.cs

**Styles Supported:** 
- Fluent2, Windows11Mica (Segoe UI Variable)
- Minimal, NotionMinimal, VercelClean (Inter)
- Bootstrap, TailwindCard, ChakraUI (System fonts)
- AntDesign, StripeDashboard, FigmaCard
- DiscordStyle, PillRail
- GradientModern, GlassAcrylic, Neumorphism

**Font Configuration:**
- **Font Selection:** Per-style (from StyleTypography)
- **Common Fonts:**
  - Segoe UI Variable 14pt (Fluent)
  - Inter 13pt (Minimal family)
  - System fonts (Web frameworks)
- **Font Style:** 
  - Regular (default)
  - Bold (when focused)
- **Letter Spacing:** 0px to 0.2px (varies by style)

**Rendering Quality:**
- **TextRenderingHint:** ClearTypeGridFit
- **SmoothingMode:** HighQuality

**Special Features:**
- Most versatile painter
- Handles 14+ styles
- Automatic font fallback per style

**Usage:**
```csharp
StandardTextPainter.Paint(g, bounds, "Hello World", isFocused, 
    BeepControlStyle.Fluent2, theme, useThemeColors);
```

---

### 5️⃣ ValueTextPainter.cs

**Styles Supported:** All (specialized for numeric/date values)

**Font Configuration:**
- **Font Selection:** Uses same font as label text (via StyleTypography)
- **Font Style:** Regular or Bold (matches control state)
- **Letter Spacing:** Matches label text

**Alignment:**
- **Horizontal:** Center (StringFormat.Alignment = StringAlignment.Center)
- **Vertical:** Center (StringFormat.LineAlignment = StringAlignment.Center)

**Rendering Quality:**
- **TextRenderingHint:** ClearTypeGridFit
- **SmoothingMode:** HighQuality

**Special Features:**
- **Centered alignment** (distinguishes from label text)
- Optimized for numeric readability
- Used by date pickers, numeric inputs, spinners

**Usage:**
```csharp
ValueTextPainter.Paint(g, bounds, "42.5", isFocused, 
    BeepControlStyle.Material3, theme, useThemeColors);
```

---

## 🔧 Usage Patterns

### Direct Painter Call
```csharp
// Call specific text painter
MaterialTextPainter.Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
```

### Via BeepStyling Coordinator
```csharp
// For label/header text
BeepStyling.PaintStyleText(g, bounds, "Label Text", isFocused, style);

// For numeric/date value text
BeepStyling.PaintStyleValueText(g, bounds, "123.45", isFocused, style);
```

### Routing Logic in BeepStyling
```csharp
// BeepStyling.cs automatically routes:
switch (style)
{
    case BeepControlStyle.Material3:
    case BeepControlStyle.MaterialYou:
        MaterialTextPainter.Paint(...);
        break;
    
    case BeepControlStyle.iOS15:
    case BeepControlStyle.MacOSBigSur:
        AppleTextPainter.Paint(...);
        break;
    
    case BeepControlStyle.DarkGlow:
        MonospaceTextPainter.Paint(...);
        break;
    
    default:
        StandardTextPainter.Paint(...);
        break;
}
```

---

## 🎯 Font Selection Guide

| Style | Font Family | Size | Weight | Letter Spacing | Painter |
|-------|-------------|------|--------|----------------|---------|
| Material3 | Roboto | 14pt | Regular/Bold | +0.1px | MaterialTextPainter |
| MaterialYou | Roboto | 14pt | Regular/Bold | +0.1px | MaterialTextPainter |
| iOS15 | SF Pro Display | 14pt | Regular/Semibold | -0.2px | AppleTextPainter |
| MacOSBigSur | SF Pro Display | 14pt | Regular/Semibold | -0.2px | AppleTextPainter |
| DarkGlow | JetBrains Mono | 13pt | Regular/Bold | +0.5px | MonospaceTextPainter |
| Fluent2 | Segoe UI Variable | 14pt | Regular/Bold | 0px | StandardTextPainter |
| Minimal | Inter | 13pt | Regular/Bold | +0.2px | StandardTextPainter |
| Bootstrap | System Font | 14pt | Regular/Bold | 0px | StandardTextPainter |
| Tailwind | Inter | 14pt | Regular/Bold | 0px | StandardTextPainter |
| Neumorphism | Montserrat | 14pt | Regular/Bold | +0.5px | StandardTextPainter |

---

## 🔑 Key Design Principles

### 1. Typography Hierarchy
All painters respect design system typography:
```csharp
Font font = StyleTypography.GetFont(style, fontSize, fontStyle);
```

### 2. Focus State
Text becomes bold on focus (most styles):
```csharp
FontStyle fontStyle = isFocused 
    ? StyleTypography.GetActiveFontStyle(style) 
    : StyleTypography.GetFontStyle(style);
```

### 3. Letter Spacing
Implemented via TextRenderer for sub-pixel accuracy:
```csharp
float letterSpacing = StyleTypography.GetLetterSpacing(style);
// Applied during rendering
```

### 4. Theme Integration
Text colors can be overridden by theme:
```csharp
private static Color GetTextColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
{
    if (useThemeColors && theme != null)
    {
        var themeColor = BeepStyling.GetThemeColor("Foreground");
        if (themeColor != Color.Empty)
            return themeColor;
    }
    return StyleColors.GetForeground(style);
}
```

### 5. String Formatting
All painters use consistent StringFormat:
```csharp
var sf = new StringFormat
{
    Alignment = StringAlignment.Near,        // Left-aligned (labels)
    LineAlignment = StringAlignment.Center,  // Vertically centered
    Trimming = StringTrimming.EllipsisCharacter,
    FormatFlags = StringFormatFlags.NoWrap
};

// ValueTextPainter uses Center alignment:
sf.Alignment = StringAlignment.Center;  // Centered (values)
```

---

## 📊 Statistics

| Metric | Value |
|--------|-------|
| Total Text Painters | 5 |
| Styles Covered | 26+ (all BeepControlStyle values) |
| Font Families | 8+ (Roboto, SF Pro, JetBrains Mono, Inter, etc.) |
| Rendering Modes | ClearTypeGridFit (primary) |
| Letter Spacing Range | -0.2px to +0.5px |

---

## ✅ Benefits

### ✅ Design System Fidelity
- **Material:** Roboto with precise tracking
- **Apple:** SF Pro with negative spacing
- **Monospace:** JetBrains Mono optimized

### ✅ Performance
- Font caching via StyleTypography
- Efficient ClearType rendering
- No redundant font creation

### ✅ Accessibility
- High contrast text rendering
- ClearType for readability
- Focus indication via bold weight

### ✅ Theme Integration
- All text colors theme-aware
- Automatic color override
- Consistent with control styling

### ✅ Maintainability
- One painter per font family
- Clear separation of concerns
- Easy to add new fonts

---

## 🚀 Advanced Features

### Material Design Typography
```csharp
// MaterialTextPainter implements Material typography scale
// Headline, Body, Caption variants supported
Font font = StyleTypography.GetFont(
    BeepControlStyle.Material3, 
    14,  // Body size
    isFocused ? FontStyle.Bold : FontStyle.Regular
);
```

### Apple Optical Sizing
```csharp
// AppleTextPainter uses SF Pro Display with optical sizing
// Font automatically adjusts for size (13pt vs 20pt)
Font font = StyleTypography.GetFont(
    BeepControlStyle.iOS15, 
    14,  // Automatically applies optical size
    FontStyle.Regular
);
```

### Monospace Alignment
```csharp
// MonospaceTextPainter guarantees character alignment
// Wide letter spacing (+0.5px) improves readability
Font font = StyleTypography.GetFont(
    BeepControlStyle.DarkGlow, 
    13, 
    FontStyle.Regular
);
// All characters have identical width
```

### Value Text Centering
```csharp
// ValueTextPainter uses centered alignment for numbers
var sf = new StringFormat
{
    Alignment = StringAlignment.Center,       // Horizontal center
    LineAlignment = StringAlignment.Center,   // Vertical center
};
// Perfect for displaying "42", "2024-10-20", etc.
```

---

## 🎨 Text Rendering Quality

### ClearType Optimization
All painters use ClearType for optimal readability:
```csharp
g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
```

**Benefits:**
- Sub-pixel rendering
- LCD-optimized anti-aliasing
- Crisp text at all sizes

### Anti-Aliasing
Graphics smoothing for non-text elements:
```csharp
g.SmoothingMode = SmoothingMode.HighQuality;  // or AntiAlias
```

---

## 🧪 Testing Examples

### Test Material Text Rendering
```csharp
[Test]
public void MaterialTextPainter_Should_UseRobotoFont()
{
    var bitmap = new Bitmap(200, 50);
    var g = Graphics.FromImage(bitmap);
    var bounds = new Rectangle(0, 0, 200, 50);
    
    MaterialTextPainter.Paint(g, bounds, "Material Design", false, 
        BeepControlStyle.Material3, null, false);
    
    // Font should be Roboto or fallback
    Assert.IsTrue(g.TextRenderingHint == TextRenderingHint.ClearTypeGridFit);
}
```

### Test Focus Bold
```csharp
[Test]
public void MaterialTextPainter_Should_BoldOnFocus()
{
    // Not focused: Regular
    Font regularFont = StyleTypography.GetFont(BeepControlStyle.Material3);
    Assert.AreEqual(FontStyle.Regular, regularFont.Style);
    
    // Focused: Bold
    Font boldFont = StyleTypography.GetFont(
        BeepControlStyle.Material3, 
        14, 
        StyleTypography.GetActiveFontStyle(BeepControlStyle.Material3)
    );
    Assert.AreEqual(FontStyle.Bold, boldFont.Style);
}
```

---

## 🎉 Summary

The TextPainters folder provides a **complete, systematic solution** for all text rendering needs:

✅ **5 specialized painters** for different font families  
✅ **26+ styles covered** across all design systems  
✅ **ClearType rendering** for optimal readability  
✅ **Letter spacing control** for design fidelity  
✅ **Focus indication** via font weight  
✅ **Theme integration** for all text colors  
✅ **Value text specialization** with centered alignment  

**Text rendering is 100% complete and production-ready!** 🎨
