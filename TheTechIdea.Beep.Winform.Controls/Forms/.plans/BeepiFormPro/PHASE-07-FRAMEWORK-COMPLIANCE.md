# Phase 7 — Framework Compliance & Violation Elimination

**Status:** Not started | **Priority:** CRITICAL | **Depends on:** None

## Objective

Eliminate the 7 remaining framework violations across the ModernForm directory and ensure all 30+ painters comply with Beep framework rules.

## Violations Found

| File | Violation | Count |
|---|---|---|
| `BeepiFormPro.Core.cs` | `new Font()` or `BeepFontManager` | 4 |
| `Painters/FormPainterRenderHelper.cs` | `new Font()` line 149 | 1 |
| `Painters/TerminalFormPainter.cs` | `g.DrawString` | 2 |

## Tasks

### FP-01: Fix BeepiFormPro.Core.cs font violations
Replace any `BeepFontManager` calls with `BeepThemesManager.ToFont(TypographyStyle)`. Remove any `new Font()` — use theme typography.

### FP-02: Fix FormPainterRenderHelper.cs line 149
Replace `new Font(baseFont.FontFamily, baseFont.Size + 2, FontStyle.Regular)` with `BeepThemesManager.ToFont(theme?.TitleStyle) ?? SystemFonts.DefaultFont`.

### FP-03: Fix TerminalFormPainter.cs g.DrawString
Replace `g.DrawString` with `TextRenderer.DrawText` using appropriate `TextFormatFlags`.

### FP-04: Audit all 30+ painters for Design-Time font properties
Verify that Designer-generated `new Font()` in `.Designer.cs` files are acceptable (overridden by `ApplyTheme()` at runtime).

## Verification
- [ ] `dotnet build` — 0 errors
- [ ] 0 `new Font()` in code-behind
- [ ] 0 `BeepFontManager` calls
- [ ] 0 `g.DrawString` calls
