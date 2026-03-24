# Phase 6 — Performance, Cleanup & Developer Experience

**Priority:** MEDIUM  
**Effort:** ~1.5 h  
**Goal:** Reduce GDI+ allocations, add font/size caching, and improve design-time experience.

---

## 6.1 Font Caching

`ToolTipTypographyScale` (from Phase 1) should cache resolved `Font` instances per `(BeepControlStyle, section)` key. Fonts are expensive to create.

```csharp
private static readonly ConcurrentDictionary<(BeepControlStyle, ToolTipSection), Font> _fontCache = new();
```

Invalidate on `BeepThemesManager.ThemeChanged`.

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipTypographyScale.cs` | Add font cache with `ThemeChanged` invalidation |

---

## 6.2 Size Measurement Cache

`CalculateOptimalSize` is called on every show. Cache the result per `(text, title, layoutVariant, maxWidth)` key with a 64-entry LRU.

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipLayoutHelpers.cs` | Add `_sizeCache` dictionary with capacity limit |

---

## 6.3 GDI+ Allocation Reduction in Painters

Audit all `using (var brush = new SolidBrush(...))` / `using (var pen = ...)` blocks in hot paths.  
Where the same color is reused across multiple items in a single `Paint()` call, hoist the brush/pen to method scope.

### Files to modify

| File | Change |
|---|---|
| `Painters/BeepStyledToolTipPainter.cs` | Hoist shared brushes/pens within `PaintContent` |
| `Painters/TourToolTipPainter.cs` | Same |

---

## 6.4 Dispose Wiring

Ensure `CustomToolTip.Dispose(bool)` calls:
1. `_animationTimer?.Dispose()`
2. `_painter?.Cleanup()` (add `Cleanup()` to `IToolTipPainter` if absent)
3. Font cache entry release (if the tooltip created custom fonts)

### Files to modify

| File | Change |
|---|---|
| `CustomToolTip.Drawing.cs` | Verify Dispose releases animation timer and painter |
| `Painters/IToolTipPainter.cs` | Add `void Cleanup()` if not present |

---

## 6.5 Design-Time Smart Tags

Add a `ToolTipConfigActionList` for Visual Studio designer integration exposing key properties:

- `Type` (dropdown)
- `LayoutVariant` (dropdown)
- `Placement` (dropdown)
- `ShowArrow` (checkbox)
- `Animation` (dropdown)

### Files to create

| File | Purpose |
|---|---|
| `Design/ToolTipConfigActionList.cs` [NEW] | VS designer action list |

---

## Verification

- **Build:** `dotnet build`
- **Perf:** Run a micro-benchmark showing 1000 `CalculateOptimalSize` calls before/after caching.
- **Dispose:** Verify no `ObjectDisposedException` when rapidly creating/destroying tooltips.
- **User check:** Ask user to verify design-time smart tags appear in VS designer.
