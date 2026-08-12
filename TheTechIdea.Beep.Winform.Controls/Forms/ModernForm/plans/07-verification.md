# Stage 07 — FormProbe: the harness every other stage reports through

Built **alongside stage 01**, not last: each stage's checks land here as that stage starts, and
must be red before the stage's fix and green after (with the break-test recorded).

## Why a fresh probe

`WizProbe` in the session scratchpad hangs after its first hosted form **on unmodified library
code** (recorded in `Wizards/plans/00-MASTER-TRACKER.md`) — conclusions drawn from it cost a
full revert/re-land cycle. `StepperProbe` is the healthy model: watchdog thread, hard
`Environment.Exit`, per-check `PASS/FAIL` lines with the measured values, renders saved for
eyeballing, and `Console.Out.Flush()` before exit.

## Shape

`Tools/BeepFormProbe/` (WinExe, net8.0-windows, ProjectReference to the Controls csproj — same
shape as `Tools/BeepBlockWizardPreviewHarness`). Non-negotiables learned this session:

- **Watchdog** (90 s, background thread, `Environment.Exit(2)` with a message) — a hung probe
  holds its own `.exe` and silently turns every later build into a no-op.
- **Build-error count checked before every run** — the stale-binary trap produced false
  conclusions three separate times this session.
- **Blindness guards run first and FAIL, never skip** (two themes differ; two sizes differ; a
  flag known to do nothing produces no differential).
- `Control.DrawToBitmap`, never `CopyFromScreen`; composite through the window's `Region` when
  one exists; renders saved and **eyeballed**, counts alone trusted for nothing.
- Differentials over thresholds wherever layout can shift (the Steppers title-band lesson: a
  band derived from step positions moved with the fix and measured *less* ink).

## Check inventory (owner stage in brackets)

1. Design-mode resize refresh; runtime programmatic resize [01]
2. Silhouette hit-tests per corner mode; no managed `Region` on the DWM path; derived-shape
   (`OwnsWindowShape`) never overwritten; debounce trailing-edge paint counter [02]
3. UIA child walk per visible caption button; `DoDefaultAction` reaches the click path; keyboard
   cycle + focus-ring differential; `WM_NCHITTEST` → `HTMAXBUTTON`; tooltip text == UIA name [03]
4. `WM_DPICHANGED` simulation (1.5× metrics within rounding); painter-side `DpiScalingHelper`
   fence at zero; 96-vs-144 render differential [04]
5. All-skins contact sheet ×{active, inactive, maximized} ×2 sizes; per-flag per-skin pixel
   differential; deprecated-helper fence [05]
6. Backdrop attribute round-trip + stated-fallback + `BeepLog.Reported` subscription; caption
   contrast ≥ 4.5:1 per backdrop×theme; opacity ramp / reduced-motion jump [06]
7. One-API backdrop fence; DWM attribute round-trip + `EffectiveBackdrop` fallback + `BeepLog`
   report on refusal; placebo regression (no flat translucent full-client fill); caption contrast
   per backdrop×theme [06]
8. Literal-colour fence covering **all four** escape forms (`FromArgb(r,g,b)`, `FromArgb(a,r,g,b)`,
   named `Color.X`, `?? Color.Empty`); per-skin two-theme render differential; zero bare
   `catch { }`; zero `Debug.WriteLine` [08]
9. Cross-cutting: `BeepLog.Reported` collected for the whole run — any unexpected `Failure`
   fails the probe; a derived form (`CardsWizardForm`) constructs, shows, renders, closes.

## Break-it-first ledger

Every check lands with the break that proves it can fail, and this file records which were
actually run — "the check exists" and "the check has been seen red" must never look the same:

| check | break | seen red on |
|---|---|---|
| (filled in as checks land) | | |

## Exit criteria

Probe green end-to-end on a clean build; every stage's checks present with their break-ledger
rows filled; runtime under 3 minutes so it stays in the loop rather than getting skipped.
