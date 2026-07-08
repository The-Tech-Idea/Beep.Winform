# BeepRibbon Enhancement Plan — Phase C (Commercial Polish)

**Created:** 2026-07-08
**Target:** `TheTechIdea.Beep.Winform.Controls/Ribbon/`
**Reference:** DevExpress RibbonControl, Fluent UI Ribbon, Microsoft Office Ribbon UX Guidelines
**Skill:** `beep-winform-design`

---

## Current State

Phase A-B complete (core APIs, QAT, backstage, search, key tips, theme, minimize). Phase C addresses framework compliance, DPI, and visual polish.

### Critical Violations

| File | Violation |
|---|---|
| `BeepRibbonTabStrip.cs` | `new Font()` ×2, `g.DrawString` ×3, raw `TabControl` base |
| `BeepRibbonControl.Core.cs` | Raw `ToolStrip`/`ToolStripDropDown` ×4, `Height=130` hardcoded |
| `RibbonTheme.cs` | Layout tokens not DPI-scaled (6 values) |
| `Rendering/BeepRibbonRenderer.cs` | Manual GDI+, no BeepStyling |
| `Tooltips/RibbonSuperTooltip.cs` | `new Font()` |

---

## Phases

### Phase 1 — Framework Compliance (CRITICAL)

**RB-01:** Fix `BeepRibbonTabStrip.cs` — `BeepThemesManager.ToFont()` + `TextRenderer.DrawText`

**RB-02:** DPI-scale `RibbonTheme.cs` layout tokens via `DpiScalingHelper.ScaleValue(v, control)`

**RB-03:** Replace raw WinForms (`ToolStrip`, `ToolStripDropDown`, `TabControl`) with Beep equivalents (`FlowLayoutPanel`+`BeepButton`, `BeepPopupForm`, custom tab strip)

**RB-04:** Replace manual GDI+ in `BeepRibbonRenderer.cs` with `BeepStyling.PaintControl`

### Phase 2 — Visual Polish (HIGH)

**RB-05:** Active tab indicator animation (slide bar, `WizardAnimationEngine.EaseOutCubic`)

**RB-06:** Group separator styling with theme `Separator` color

**RB-07:** 150ms hover/pressed transition on interactive items

### Phase 3 — Accessibility & DPI (MEDIUM)

**RB-08:** Keyboard nav audit (Alt→KeyTips, Tab→groups, arrows→items, Enter→activate)

**RB-09:** DPI scaling pass for all hardcoded pixel values

**RB-10:** High contrast mode via `SystemInformation.HighContrast`

### Phase 4 — Backstage & Search (MEDIUM)

**RB-11:** Backstage slide animation (200ms ease-out)

**RB-12:** MRU-boosted search ranking in `RibbonSearchRanking.cs`

---

## Verification

1. `dotnet build` — 0 errors
2. 0 `new Font()`, 0 `g.DrawString`
3. Tab indicator animates smoothly
4. DPI correct at 100/150/200%
5. High contrast mode renders correctly
