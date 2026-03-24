# Phase 4 — Visual Polish & Animation Refinements

**Priority:** MEDIUM  
**Effort:** ~2 h  
**Goal:** Reach Figma/MD3 visual fidelity for shadows, arrow rendering, and motion curves.

---

## 4.1 MD3 Elevation Shadow Tokens

Replace the blurred-loop shadow in `BeepStyledToolTipPainter.PaintShadow()` with a two-layer shadow matching MD3 Elevation Level 2:

- **Ambient layer:** `0, 1px, 3px, Color(0,0,0,0.12)`
- **Key layer:** `0, 4px, 8px, Color(0,0,0,0.14)`

This produces the characteristic soft shadow seen in Figma tooltip components.

### Files to modify

| File | Change |
|---|---|
| `Painters/BeepStyledToolTipPainter.cs` | Replace `PaintShadow` loop with two-pass MD3 shadow |
| `Painters/GlassToolTipPainter.cs` | Apply glass-specific ambient shadow |

---

## 4.2 Arrow Anti-Aliasing & Continuity

`ToolTipArrowPainter.DrawArrow()` draws a triangle but the border doesn't always seamlessly join the tooltip body.

**Fix:**
1. Clip the tooltip body path to exclude the arrow zone, then union the arrow path into the body path before filling — this produces a single continuous shape
2. Or: draw the arrow background first, then overlay the body so they share a seamless fill

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipArrowPainter.cs` | Refactor to produce a combined `GraphicsPath` (body + arrow) for seamless painting |
| `Painters/BeepStyledToolTipPainter.cs` | Use combined path in `PaintBackground` |

---

## 4.3 Animation Curve Polish

Current easing functions are basic quadratic. Add MD3 Standard / Emphasized curves:

| Curve | Values |
|-------|--------|
| MD3 Standard | cubic-bezier(0.2, 0, 0, 1) |
| MD3 Emphasized | cubic-bezier(0.2, 0, 0, 1) with overshoot |
| MD3 EmphasizedDecelerate | cubic-bezier(0.05, 0.7, 0.1, 1) |

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipAnimationHelpers.cs` | Add `MD3Standard`, `MD3Emphasized`, `MD3EmphasizedDecelerate` easing functions |
| `ToolTipEnums.cs` | Add `MD3Standard`, `MD3Emphasized` to `EasingFunction` enum |

---

## 4.4 Hover Micro-Interaction

When `PersistOnHover == true` and the user moves the mouse onto the tooltip itself:

1. Subtle scale bump: `1.0 → 1.02` over 100 ms
2. Border brightens 10 % (using alpha shift on border color)

This signals "interactive" status per WCAG 1.4.13.

### Files to modify

| File | Change |
|---|---|
| `CustomToolTip.Animation.cs` | Add `AnimateHoverIn` / `AnimateHoverOut` micro-interactions |

---

## Verification

- **Build:** `dotnet build`
- **Visual:** Show tooltip with `ShowShadow=true` and compare shadow quality before/after.
- **Visual:** Show tooltip with arrow and inspect border continuity at arrow join point.
- **User check:** Ask user to compare shadow rendering quality.
