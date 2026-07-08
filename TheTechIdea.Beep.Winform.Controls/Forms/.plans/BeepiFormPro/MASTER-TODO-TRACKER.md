# BeepiFormPro — Master Enhancement Tracker

**Updated:** 2026-07-09
**Target:** `TheTechIdea.Beep.Winform.Controls/Forms/ModernForm/`
**Painters:** 30+ form painters | **Violations:** 7 remaining

---

## Phase Status Overview

| # | Phase | Status | Priority | Tasks |
|---|---|---|---|---|
| 1 | Baseline & Contract Reset | 🟡 In Progress | CRITICAL | 6 |
| 2 | Theme, Cache & State | 🟡 In Progress | CRITICAL | 5 |
| 3 | Layout, Hit-Test & Input | 🟡 In Progress | HIGH | 8 |
| 4 | **Painter Consolidation** | ⬜ Not Started | HIGH | 6 |
| 5 | Win32, Backdrop & Design-Time | 🟡 In Progress | MEDIUM | 5 |
| 6 | Validation, Docs & Samples | 🟡 In Progress | MEDIUM | 4 |
| 7 | **Framework Compliance** (NEW) | ⬜ Not Started | CRITICAL | 4 |

---

## Phase 7 — Framework Compliance (CRITICAL)

| ID | Task | Status |
|---|---|---|
| FP-01 | Fix BeepiFormPro.Core.cs font violations (4 occurrences) | `[ ]` |
| FP-02 | Fix FormPainterRenderHelper.cs `new Font()` line 149 | `[ ]` |
| FP-03 | Fix TerminalFormPainter.cs `g.DrawString` (2 occurrences) | `[ ]` |
| FP-04 | Audit all 30+ painters for Designer.cs font properties | `[ ]` |

---

## Phase 4 — Painter Consolidation (HIGH)

| ID | Task | Status |
|---|---|---|
| PC-01 | Extract shared `PaintCaptionBackground` helper for gradient/acrylic caption bars | `[ ]` |
| PC-02 | Extract shared `PaintSystemButtons` helper (minimize/maximize/close) | `[ ]` |
| PC-03 | Normalize caption button widths to `FormPainterMetrics` tokens | `[ ]` |
| PC-04 | Remove duplicate `CompositingMode` management from individual painters → shared helper | `[ ]` |
| PC-05 | Audit and deduplicate `GetMetrics` patterns across painters | `[ ]` |
| PC-06 | Create shared `PaintFormBorder` helper with per-style corner radius | `[ ]` |

---

## Remaining Phase 5 — Win32 & Backdrop

| ID | Task | Status |
|---|---|---|
| WB-01 | Complete maximize/restore repaint path | `[ ]` |
| WB-02 | Validate backdrop (acrylic/mica) across all supported Windows versions | `[ ]` |
| WB-03 | Design-time rendering safety for all 30+ painters | `[ ]` |
| WB-04 | Non-client area painting contract finalization | `[ ]` |
| WB-05 | DWM-compatible shadow/glass integration | `[ ]` |

---

## Remaining Phase 6 — Validation

| ID | Task | Status |
|---|---|---|
| VD-01 | Execute runtime validation matrix (theme, backdrop, multi-form) | `[ ]` |
| VD-02 | Execute design-time validation matrix | `[ ]` |
| VD-03 | Execute caption button width audit across all painters | `[ ]` |
| VD-04 | Accessibility validation (screen reader, high contrast, keyboard nav) | `[ ]` |

---

## Files Summary

| File | Phase | Change |
|---|---|---|
| `BeepiFormPro.Core.cs` | 7 | Fix 4 font violations |
| `Painters/FormPainterRenderHelper.cs` | 7 | Fix 1 `new Font()` |
| `Painters/TerminalFormPainter.cs` | 7 | Fix 2 `g.DrawString` |
| `Painters/FormPainterLayoutHelper.cs` | 4 | Add shared caption/system-button helpers |
| `FormPainterMetrics.cs` | 4 | Normalize caption button width tokens |
| `BeepiFormPro.Win32.cs` | 5 | Maximize/restore repaint |

## Verification

1. `dotnet build` — 0 errors
2. 0 `new Font()` in code-behind (Designer files OK)
3. 0 `BeepFontManager` calls
4. 0 `g.DrawString` calls
5. All painters render correctly after consolidation
6. Maximize/restore transitions paint correctly
7. Design-time preview works for all styles

| 8 | **Style Authenticity Audit & Correction** (NEW) | ⬜ Not Started | HIGH | 5 |

### Phase 8 Tasks

| ID | Task | Status |
|---|---|---|
| SA-01 | Categorize all 33 painters (A=authentic, B=partial, C=color-swap) | `[ ]` |
| SA-02 | Fix Category C painters (add distinctive visual traits) | `[ ]` |
| SA-03 | Enhance Category B painters (add missing traits) | `[ ]` |
| SA-04 | Create shared effect helpers (PaintFrostedGlass, PaintDualShadow, PaintNeonGlow, PaintPrismaticGradient, PaintScanLines) | `[ ]` |
| SA-05 | Verify editor theme palettes (Dracula, OneDark, Nord, Tokyo, Solarized, GruvBox) | `[ ]` |

| 9 | **DisplayRectangle & Padding Fix** (NEW) | ⬜ Not Started | CRITICAL | 4 |
