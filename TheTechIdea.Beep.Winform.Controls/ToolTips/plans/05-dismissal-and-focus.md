# 05 — Dismissal, Focus & Outside Click

**Priority P1.**

## Current behaviour

Partially implemented, across three unconnected places:

- `CustomToolTip.Accessibility.cs` intercepts `Keys.Escape` in `ProcessCmdKey` to dismiss.
- `OutsideClickMessageFilter` (63 lines) is an `IMessageFilter` for click-outside dismissal.
- `ToolTipManager` has a 5-second `System.Threading.Timer` sweeping for expiry.

What is missing is a single dismissal policy. Specifically:

- **Escape only reaches the tooltip if the tooltip window has focus.** `ProcessCmdKey` fires on the
  focused control's chain; a hover-triggered tooltip never takes focus, so Escape on the *anchor*
  does not dismiss it. WCAG 1.4.13's "dismissible" requires dismissing without moving the pointer —
  i.e. from the keyboard, while focus is still on the trigger.
- **No focus-loss dismissal.** Nothing hides a focus-triggered tooltip when focus moves on.
- **No dismissal on window deactivate.** Alt-Tab away and the tooltip stays on top of the other
  application, because it is a top-level window.
- **No scroll dismissal.** Covered by [03](03-auto-update.md)'s `hide` behaviour.
- **Focus is never returned.** For an interactive tooltip that took focus, Escape should return
  focus to the anchor; nothing does.

## What the reference systems do

Radix and Floating UI model dismissal as explicit, composable interactions —
`useDismiss({ escapeKey, outsidePress, ancestorScroll, referenceHidden })` — and focus management as
a separate concern (`useFocusManager`) covering focus return, focus trapping for modal popovers, and
restoring focus on close.

Commercial WinForms suites (DevExpress `ToolTipController`, Telerik `RadToolTip`) additionally
dismiss on: any mouse-down anywhere, the owning form deactivating, and the anchor being disabled.

## Work

1. **One `ToolTipDismissPolicy`** on config, as flags:
   `EscapeKey | OutsidePress | FocusLoss | WindowDeactivate | AnchorHidden | Timeout`, with sensible
   defaults per `ToolTipType` (a `Notification` times out; a `Descriptive` does not).
2. **Escape from the anchor, not just the tooltip.** Register a keyboard hook scoped to the anchor's
   top-level form while a tooltip is visible, so Escape works with focus still on the trigger.
   `ProcessCmdKey` on the tooltip window stays as the path for interactive tooltips that took focus.
3. **Dismiss on window deactivate** — subscribe to the anchor's form `Deactivate`. A top-level
   tooltip left floating over another application is the most visible symptom of this whole gap.
4. **Focus return.** Record the previously focused control when an interactive tooltip takes focus,
   and restore it on dismiss.
5. **Audit `OutsideClickMessageFilter`.** An `Application.AddMessageFilter` that outlives its
   tooltip is a process-wide leak; confirm it is removed on every dismissal path, including the
   exception paths, and that only one is installed at a time.
6. **Dismiss when the anchor is disabled or removed** — a disabled control gets no `MouseLeave`, so
   its tooltip can otherwise strand.

## Verification

- With a hover tooltip open and focus on the anchor, press Escape → dismissed.
- Open a tooltip, Alt-Tab to another application → dismissed, not floating on top.
- Open an interactive tooltip, Tab into it, press Escape → dismissed *and* focus back on the anchor.
- Disable the anchor while its tooltip is open → dismissed.
- Show and dismiss 100 tooltips, then assert exactly zero message filters remain installed.
