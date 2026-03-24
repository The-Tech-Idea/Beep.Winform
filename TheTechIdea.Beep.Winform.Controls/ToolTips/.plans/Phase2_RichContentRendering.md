# Phase 2 — Rich Content Rendering & Section Layout

**Priority:** HIGH  
**Effort:** ~3 h  
**Goal:** Make `ToolTipContentItem[]` and `LayoutVariant` actually render in the main painter pipeline, matching Figma Card / Preview / Shortcut / Tour layouts.

---

## 2.1 Implement Section Rendering in `BeepStyledToolTipPainter.PaintContent`

Currently `PaintContent` only paints icon + title + text + shortcut badges.  
When `config.ContentItems` is populated it should be the **source of truth** (overriding `Text`/`Title`).

### Rendering rules per ToolTipSection

| Section | Rendering |
|---------|-----------|
| `Header` | Icon (left) + Title bold + optional close-button (right) |
| `Body` | Paragraphs with optional markup (`IsBold`, `IsItalic`, `IsCode`, `IsLink`) |
| `Divider` | 1 px horizontal line with 4 px vertical margin, using `theme.BorderColor` @ 30 % alpha |
| `Footer` | Shortcut badges (left) + action text links (right) |

### Files to modify

| File | Change |
|---|---|
| `Painters/BeepStyledToolTipPainter.cs` | Add `PaintContentItems(g, bounds, contentItems, config, theme)` method; call in `PaintContent` when `ContentItems != null` |
| `Painters/ToolTipPainterBase.cs` | Add `DrawSection(g, ToolTipContentItem, Rectangle, IBeepTheme)` virtual method |

---

## 2.2 `ToolTipMarkupParser` Integration

The existing `ToolTipMarkupParser.cs` (Helpers/) already parses `**bold**`, `*italic*`, `` `code` ``, `[link]` syntax.  
Wire it into the body section renderer when `config.UseMarkup == true`.

### Files to modify

| File | Change |
|---|---|
| `Painters/BeepStyledToolTipPainter.cs` | In body-section rendering, call `ToolTipMarkupParser.Parse()` and draw styled runs |

---

## 2.3 Close-Button Drawing

When `config.Closable == true`, draw a small `×` glyph in the header area.

- Size: 16 × 16 px (DPI-scaled)
- Position: top-right corner with `contentPadding` inset
- Color: `theme.ForeColor` @ 60 % alpha, 100 % on hover (state managed by CustomToolTip)

### Files to modify

| File | Change |
|---|---|
| `Painters/BeepStyledToolTipPainter.cs` | Add `DrawCloseButton(g, Rectangle, Color)` |
| `CustomToolTip.Core.cs` | Track `_isCloseButtonHovered` state |
| `CustomToolTip.Methods.cs` | Handle close-button click in `OnMouseDown` |

---

## 2.4 Card / Preview / Tour Layout Variants

Expand `CalculateLayout()` to handle each `ToolTipLayoutVariant`:

| Variant | Layout |
|---------|--------|
| `Simple` | Single row: icon + text |
| `Rich` | Header + Body (title + multiline text) |
| `Card` | Header / Divider / Body / Divider / Footer |
| `Preview` | Large image top + Title + Subtitle + Footer |
| `Tour` | Header + Body + Divider + Footer with step dots + prev/next buttons |
| `Shortcut` | Icon + label + right-aligned key-cap badges |
| `Glass` | Same as Rich but with glass painter |

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipLayoutHelpers.cs` | Add `CalculateVariantLayout(ToolTipLayoutVariant, ...)` |
| `Painters/BeepStyledToolTipPainter.cs` | Route to variant-specific rendering |

---

## Verification

- **Build:** `dotnet build`
- **Visual:** Create a `ToolTipConfig` with `ContentItems` containing Header + Body + Divider + Footer sections; verify each section renders correctly.
- **User check:** Ask user to verify Card and Tour layouts visually.
