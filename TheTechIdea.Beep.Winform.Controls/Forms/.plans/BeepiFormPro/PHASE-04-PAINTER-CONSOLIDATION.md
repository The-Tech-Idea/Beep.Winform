# Phase 4 — Painter Consolidation (REVISED)

**Status:** Not started | **Priority:** HIGH | **Depends on:** Phase 1, 2, 3

## Objective

Eliminate ~2,500-3,000 lines of duplicated code across 33 painters by extracting shared helpers without flattening painter identity.

## Audit Findings (2026-07-08)

### Massive Code Duplication

| Pattern | Copies | Lines Each | Total Duplicated |
|---|---|---|---|
| `CreateRoundedRectanglePath` | 33 | ~35 | ~1,155 lines |
| `DrawShadow` | 33 | ~10 | ~330 lines |
| `PaintWithEffects` orchestration | ~25 | ~30 | ~750 lines |
| HSL color conversion (`ShiftLuminance`/`ColorToHsl`/etc.) | 7+ | ~40 | ~280 lines |
| **TOTAL** | | | **~2,500+ lines** |

These 2,500 lines can be reduced to ~200 lines in shared helpers.

### Missing DPI Scaling

| Painter | Issue |
|---|---|
| `MinimalFormPainter` | `iconSize = 16` hardcoded |
| `MetroFormPainter` | `iconSize = 16`, button width `46` hardcoded |
| `FluentFormPainter` | `iconSize = 16` hardcoded |
| `GlassFormPainter` | `iconSize = 16` hardcoded |
| `CustomFormPainter` | `iconSize = 18`, no DPI scaling anywhere |

### Inline Layout Duplication (bypasses FormPainterLayoutHelper)

| Painter |
|---|
| `FluentFormPainter` — complete 100-line inline layout |
| `BrutalistFormPainter` — complete 100-line inline layout |
| `GlassFormPainter` — complete 100-line inline layout |
| `CustomFormPainter` — complete 100-line inline layout |

### Missing CompositingMode (paint accumulation bug)

| Painter |
|---|
| `MetroFormPainter` — no `SourceCopy`/`SourceOver` management |
| `CustomFormPainter` — no compositing mode management at all |

### CustomFormPainter Metrics Bypass

`CustomFormPainter.GetMetrics()` creates a new `FormPainterMetrics` every call instead of using `DefaultForCached()`. No theme integration, no DPI, no caching.

## Tasks

### PC-01: Extract shared `CreateRoundedRectanglePath` helper
Move one implementation to `FormPainterRenderHelper` (already exists at line 725). Update all 33 painters to call the shared version. Remove all private copies.

### PC-02: Extract shared `DrawShadow` helper
Move to `FormPainterRenderHelper` with parameters for color, blur, offset. Remove all 33 private copies.

### PC-03: Extract shared HSL color utilities
Use `FormPainterRenderHelper.Lighten`/`Darken` (already exist). Remove all 7+ private copies.

### PC-04: Consolidate `PaintWithEffects` orchestration
Create `FormPainterRenderHelper.PaintFormWithEffects(g, owner, painter)` that handles shadow→background→clip→borders→caption orchestration. Update 25 painters to call it.

### PC-05: Fix DPI scaling in 5 painters
Replace hardcoded `iconSize = 16/18` with `DpiScalingHelper.ScaleValue(16, owner)`. Replace hardcoded button widths with `metrics.ButtonWidth`.

### PC-06: Fix compositing mode in 2 painters
Add `CompositingMode.SourceCopy` → base fill → `SourceOver` → overlay → restore pattern to MetroFormPainter and CustomFormPainter.

### PC-07: Wire 4 inline-layout painters to FormPainterLayoutHelper
Convert FluentFormPainter, BrutalistFormPainter, GlassFormPainter, and CustomFormPainter to use `TryBuildStandardRightAlignedCaptionLayout()`.

### PC-08: Fix CustomFormPainter metrics
Replace `new FormPainterMetrics()` with `FormPainterMetrics.DefaultForCached(style, theme, useTheme)`.

## Files

| File | Change |
|---|---|
| `Painters/FormPainterRenderHelper.cs` | Add shared CreateRoundedRect, DrawShadow, PaintWithEffects |
| `Painters/FormPainterLayoutHelper.cs` | Ensure all inline painters use it |
| 33 `Painters/*FormPainter.cs` | Remove duplicated methods, call shared helpers |
| `FormPainterMetrics.cs` | Add missing `MaterialYou` case |

## Verification
- [ ] `dotnet build` — 0 errors
- [ ] All 33 painters render identically to before
- [ ] ~2,500 lines of duplicated code removed
- [ ] DPI scaling applied to all hardcoded values
