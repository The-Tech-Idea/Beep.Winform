# Phase 1 — Typography & Alignment Hardening

**Priority:** CRITICAL  
**Effort:** ~1.5 h  
**Goal:** Eliminate all hard-coded font sizes and heights; wire existing `BeepFontManager`, `StyleTypography`, and `StyleSpacing` into the ToolTip painter pipeline.

---

## Existing Infrastructure (DO NOT DUPLICATE)

The project already has comprehensive typography and spacing systems:

| System | Location | Key APIs |
|---|---|---|
| `BeepFontManager` | `FontManagement/BeepFontManager.cs` | `TooltipFont`, `GetCachedFont()`, `GetFontForPainter()` (DPI), `UIElementType.Tooltip` |
| `StyleTypography` | `Styling/Typography/StyleTypography.cs` | `GetFontFamily(style)`, `GetFontSize(style)`, `GetLineHeight(style)`, `GetLetterSpacing(style)`, `GetFont(style)` |
| `StyleSpacing` | `Styling/Spacing/StyleSpacing.cs` | `GetPadding(style)`, `GetItemSpacing(style)`, `GetIconSize(style)`, `GetItemHeight(style)` |
| `StyledImagePainter` | `Styling/ImagePainters/StyledImagePainter.cs` | Full caching, tinting, shape-based clipping for icons |

> [!IMPORTANT]
> **No new typography or spacing classes needed.** Phase 1 is purely about *wiring* these existing APIs into the ToolTip painters that currently use hard-coded values.

---

## 1.1 Replace Hard-Coded Fonts in `ToolTipPainterBase`

Currently `ToolTipPainterBase.GetTitleFont()` and `GetTextFont()` create `new Font("Segoe UI", ...)` on every call.

**Fix:** Delegate to `BeepFontManager.GetCachedFont()` using style info from `StyleTypography`:

```csharp
protected Font GetTitleFont(ToolTipConfig config)
{
    if (config.Font != null) return config.Font;

    var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
    string family = StyleTypography.GetFontFamily(style).Split(',')[0].Trim();
    float size = StyleTypography.GetFontSize(style) + 1.5f; // title bump
    return BeepFontManager.GetCachedFont(family, size, FontStyle.Bold);
}

protected Font GetTextFont(ToolTipConfig config)
{
    if (config.Font != null) return config.Font;

    var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
    string family = StyleTypography.GetFontFamily(style).Split(',')[0].Trim();
    float size = StyleTypography.GetFontSize(style) - 1f; // tooltip is slightly smaller
    return BeepFontManager.GetCachedFont(family, size, FontStyle.Regular);
}
```

### Files to modify

| File | Change |
|---|---|
| `Painters/ToolTipPainterBase.cs` | Replace `GetTitleFont` / `GetTextFont` to use `StyleTypography` + `BeepFontManager.GetCachedFont()` |

---

## 1.2 Replace Hard-Coded Sizes in `ToolTipLayoutHelpers`

Current hard-coded values:
- `titleHeight = 18` (line 45)
- `new Font("Segoe UI", 10.5f, FontStyle.Bold)` (line 125)
- `new Font("Segoe UI", 9.5f)` (line 134)

**Fix:** Use `StyleTypography.GetFont(style)` for measurement and `StyleSpacing.GetPadding(style)` / `StyleSpacing.GetItemSpacing(style)` for spacing:

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipLayoutHelpers.cs` | `CalculateOptimalSize()` — use `StyleTypography.GetFont(style)` instead of hard-coded fonts |
| `Helpers/ToolTipLayoutHelpers.cs` | `CalculateLayout()` — measure title height dynamically instead of hard-coded 18 px |

---

## 1.3 Expand `ToolTipLayoutMetrics`

Add missing rects needed by painters:

```csharp
public class ToolTipLayoutMetrics
{
    public Rectangle IconRect { get; set; } = Rectangle.Empty;
    public Rectangle TitleRect { get; set; } = Rectangle.Empty;
    public Rectangle TextRect { get; set; } = Rectangle.Empty;
    public Rectangle ArrowRect { get; set; } = Rectangle.Empty;
    // NEW:
    public Rectangle FooterRect { get; set; } = Rectangle.Empty;
    public Rectangle DividerRect { get; set; } = Rectangle.Empty;
    public Rectangle CloseButtonRect { get; set; } = Rectangle.Empty;
    public int ContentPadding { get; set; }
    public int ItemSpacing { get; set; }
}
```

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipLayoutHelpers.cs` | Add new rects to `ToolTipLayoutMetrics`, compute them in `CalculateLayout()` |

---

## 1.4 Text Alignment Tokens

Add text alignment properties to `ToolTipConfig`:

```csharp
public StringAlignment TextHAlign { get; set; } = StringAlignment.Near;
public StringAlignment TextVAlign { get; set; } = StringAlignment.Near;
```

All painters should read these instead of hard-coding `StringAlignment.Near`.

### Files to modify

| File | Change |
|---|---|
| `ToolTipConfig.cs` | Add `TextHAlign` and `TextVAlign` properties |
| `Painters/BeepStyledToolTipPainter.cs` | Use `config.TextHAlign` / `config.TextVAlign` in `PaintContent` |

---

## Verification

- **Build:** `dotnet build TheTechIdea.Beep.Winform.Controls.csproj`
- **Visual:** Run sample app, set `TooltipText` with title + body on a button, observe consistent alignment
- **User check:** Ask user to verify alignment at their preferred DPI setting
