# 09 — Accessibility

**Priority P1.**

## Current behaviour

More is present here than in most of the framework, but it is incomplete in ways that matter.

Implemented:

- `CustomToolTip.Accessibility.cs` sets MSAA/UIA accessible name and help text from config, handles
  Escape, and exposes `IsHighContrastActive`.
- `ToolTipManager.SetTooltip` forwards text to `control.AccessibleDescription` and the title to
  `AccessibleName` when `EnableAccessibility` is on.
- `ToolTipConfig.MinContrastRatio` (default 4.5) exists and has 2 references.
- `ToolTipAccessibilityHelpers` exists in `Helpers/`.

Gaps:

| Gap | Detail |
|---|---|
| WCAG 1.4.13 "hoverable" | not implemented — see [04](04-interactive-hover.md) |
| WCAG 1.4.13 "persistent" | default `Duration = 3000` auto-hides while the user is still reading |
| Escape from the trigger | only works when the tooltip has focus — see [05](05-dismissal-and-focus.md) |
| Screen-reader announcement | no UIA live-region / `RaiseAutomationNotification`, so a tooltip appearing is not announced |
| Reduced motion | `EnableAnimations` is global; nothing consults the OS "show animations" setting |
| High contrast | `IsHighContrastActive` is exposed — confirm the painters actually branch on it and use system colours rather than theme colours |
| `AccessibleDescription` overwrite | `SetTooltip` overwrites whatever the host had already set on the control, with no restore on `RemoveTooltip` |

## What the reference systems do

- **`role="tooltip"` + `aria-describedby`** on the trigger; the accessible name of the trigger is
  unchanged and the tooltip becomes its description. Note the current code writes the *title* into
  `AccessibleName`, which **replaces** the control's own name — usually wrong. A button labelled
  "Save" with a tooltip titled "Save document" should still be named "Save".
- **Announcement**: the tooltip content is exposed to the screen reader when it appears — on Windows
  UIA this is `AutomationPeer.RaiseNotificationEvent` / `IRawElementProviderSimple` notifications.
- **`prefers-reduced-motion`** disables transitions. The Windows equivalent is
  `SystemInformation.UIEffectsEnabled` and the "Show animations in Windows" setting.

## Work

1. **Stop overwriting `AccessibleName`.** Put tooltip text in `AccessibleDescription` only; leave the
   control's name alone. Preserve and restore any prior value on `RemoveTooltip`.
2. **Announce on show** via UIA notification so screen-reader users receive hover/focus tooltips.
3. **Honour reduced motion** — when `SystemInformation.UIEffectsEnabled` is false, force
   `ToolTipAnimation.None` regardless of config, and document that config cannot override it.
4. **Enforce `MinContrastRatio`.** Audit the 2 existing references; the resolved fore/back pair
   should be checked and corrected at paint time. `ColorUtils.EnsureReadable` already exists in this
   repo (added for the grid) and does exactly this — reuse it rather than writing a second one.
5. **High-contrast mode**: verify every painter honours `IsHighContrastActive` and falls back to
   `SystemColors`; a glassmorphism tooltip in high contrast is unreadable.
6. **Keyboard-only reachability** — a tooltip attached to a control must appear on focus, not only
   hover, whenever `KeyboardTriggerable` is set (it has 4 references; confirm they cover the focus
   path).

## Verification

- Narrator: focus a control with a tooltip; assert the tooltip text is announced and the control's
  own name is unchanged.
- Turn off "Show animations in Windows"; assert tooltips appear without animation.
- Enable High Contrast; render every painter and confirm readable output.
- Assert a contrast ratio ≥ `MinContrastRatio` for the resolved colours of every `ToolTipType` on
  every shipped theme — this is a cheap automated check and it caught real theme bugs in the grid.
