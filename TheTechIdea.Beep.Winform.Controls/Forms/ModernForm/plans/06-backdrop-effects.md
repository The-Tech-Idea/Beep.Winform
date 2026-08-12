# Stage 06 — Backdrops and effects: Mica, Acrylic, shadow, motion

## The gap

`BeepiFormPro.Backdrop.cs` (449 lines) and the DWM imports (`DWMWA_SYSTEMBACKDROP_TYPE = 38`,
`DwmExtendFrameIntoClientArea`, `DWMWA_NCRENDERING_POLICY` — `Win32.cs:587-645`) show the
intent, but the pieces don't yet add up to what a user sees in a commercial product:

- **Mica/Acrylic** need the DWM path from stage 02 (a `SetWindowRgn` region defeats the system
  backdrop) — which is why this stage comes after it.
- **Shadow**: with regions gone on Win11 the native shadow returns for free; Win10's region
  fallback needs the layered-window drop-shadow or a documented "no shadow on Win10 with rounded
  corners" decision — either is acceptable, silent absence is not.
- **Motion**: commercial chrome animates state — fade on open/close, smooth
  maximize/restore. WinForms can do restrained versions (opacity ramps, DWM transitions); anything
  more belongs to the skins via stage 05 hooks, not hard-coded in the form.
- Theme integration: a Mica backdrop under a light theme and Acrylic under dark must both keep
  caption text at readable contrast — the tooltip Glass lesson (light ink on a light frost) will
  recur here if unchecked.

## The work

1. **`BackdropKind` property**: `None | Solid | Mica | MicaAlt | Acrylic`, applied through
   `DWMWA_SYSTEMBACKDROP_TYPE` where the OS supports it, with per-kind fallbacks (solid theme
   fill) that are *stated* — a fallback the caller can query, not a silent downgrade.
2. **Fallback shadow for the Win10/region path**, or the recorded decision not to ship one.
3. **Open/close fade and maximize/restore easing** behind a single `EnableFormAnimations` flag,
   off in design mode, respecting `SystemInformation` reduced-motion.
4. **Contrast guard**: caption ink resolved against the *effective* surface (backdrop-composited),
   reusing the WCAG pick pattern already shipped in the tooltip and LOV work.
5. Every failure path reports through `BeepLog` (`FallbackOnce` for per-paint DWM refusals).

## Verification

- On a Win11 host: set each `BackdropKind`, read the attribute back via
  `DwmGetWindowAttribute`, assert round-trip; on refusal assert the stated fallback engaged and
  a `BeepLog` fallback was reported (probe subscribes to `BeepLog.Reported`).
- Contrast: for each backdrop × {light, dark} theme, computed caption ink vs effective surface
  ≥ 4.5:1 (blindness guard: a deliberately wrong pairing must fail).
- Animation: open a form with animations on, sample `Opacity` over time — must traverse
  intermediate values; with reduced-motion simulated, must jump straight to 1.0.
- Break-it-first: force `DwmSetWindowAttribute` to return failure (wrong attribute size) and
  confirm the fallback check and the `BeepLog` check both go red without the handling.
- Eyeball: contact sheet of backdrops × two themes.

## Exit criteria

Backdrops selectable with queryable fallbacks; shadow present or its absence a recorded decision;
animations flag-gated and reduced-motion-aware; contrast checks green both themes; break-tests
recorded.
