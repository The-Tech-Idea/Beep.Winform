# Phase 5 — Accessibility Hardening & RTL Support

**Priority:** MEDIUM  
**Effort:** ~2 h  
**Goal:** Full WCAG 2.2 AA compliance and bi-directional text support.

---

## 5.1 Contrast Auto-Correction

`ToolTipConfig.MinContrastRatio` is declared (4.5 default) but never enforced.

**Implement:** In `ToolTipThemeHelpers.GetThemeColors()`, after resolving `backColor` + `foreColor`, compute the WCAG relative-luminance contrast ratio. If below `MinContrastRatio`, lighten/darken `foreColor` until the threshold is met.

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipThemeHelpers.cs` | Add `EnforceContrast(ref Color fore, Color back, double minRatio)` |
| `Helpers/ToolTipAccessibilityHelpers.cs` | Add `CalculateContrastRatio(Color, Color)` using WCAG relative-luminance formula |

---

## 5.2 Focus Ring for Interactive Tooltips

When `config.TriggerMode == Click` or `config.Type == Interactive`, the tooltip itself must be focusable and show a 2 px focus ring (WCAG 2.4.7).

### Files to modify

| File | Change |
|---|---|
| `CustomToolTip.Core.cs` | Set `TabStop = true` when interactive |
| `CustomToolTip.Drawing.cs` | Draw 2 px focus ring on `OnPaint` when `Focused` |
| `CustomToolTip.Accessibility.cs` | Report `AccessibleRole.ToolTip` and support `AccessibleEvents.Focus` |

---

## 5.3 Live Region Announce

When a tooltip appears, call `AccessibilityNotifyClients(AccessibleEvents.NameChange, 0)` so screen readers announce the content as an aria-live polite region equivalent.

### Files to modify

| File | Change |
|---|---|
| `CustomToolTip.Methods.cs` | In `ShowAsync()`, call accessibility notification |
| `CustomToolTip.Accessibility.cs` | Expose `AccessibleName` as tooltip text content |

---

## 5.4 RTL (Right-to-Left) Layout

When `RightToLeft == RightToLeftLayout.Yes` on the owner control:

1. Mirror content: icon on right, text aligned right
2. Arrow placement mirrors (LeftStart ↔ RightStart)
3. Shortcut badges right-align → left-align
4. Close button: top-left instead of top-right

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipLayoutHelpers.cs` | Add `MirrorLayout(ToolTipLayoutMetrics, bool isRtl)` |
| `Painters/BeepStyledToolTipPainter.cs` | Pass `isRtl` flag to content painting |
| `CustomToolTip.Core.cs` | Detect RTL from owner or system |

---

## 5.5 Reduced Motion (Already Partially Done)

Verify that `ToolTipAnimationHelpers.ShouldReduceMotion()` is checked in **all** animation entry points:

- `CustomToolTip.Animation.cs: AnimateInAsync()`
- `CustomToolTip.Animation.cs: AnimateOutAsync()`
- Any new hover micro-interaction from Phase 4

---

## Verification

- **Build:** `dotnet build`
- **Accessibility:** Use Windows Narrator; navigate to a control with a tooltip and verify announcement.
- **Contrast:** Set a theme with low-contrast tooltip colors; verify auto-correction brightens text.
- **RTL:** Set `RightToLeft = Yes` on a form; verify tooltip content mirrors correctly.
- **User check:** Ask user to verify with Narrator if available.
