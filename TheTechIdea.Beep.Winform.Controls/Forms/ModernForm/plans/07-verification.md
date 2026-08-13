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
6. One-API fence (no `Backdrop`/`BackdropType`/`EnableMicaBackdrop`/`EnableAcrylicForGlass`
   references remain); the surviving painted effect changes pixels **and** its tint tracks the
   theme across two themes; frame-level DWM (corner preference, shadow) still applies with the
   backdrop path gone; caption contrast ≥ 4.5:1 over the painted surface per theme; opacity ramp /
   reduced-motion jump [06]

   **Retired with the ruling — and worth keeping as a cautionary note:** the DWM attribute
   round-trip and `EffectiveBackdrop`-on-refusal checks are gone, because they test a negotiation
   that no longer happens. More importantly, the round-trip check **would have passed today**:
   `DwmSetWindowAttribute` returns S_OK for an effect the painter immediately covers. A check that
   green-lights an invisible feature is precisely the failure this stage exists to record.
7. Literal-colour fence covering **all four** escape forms (`FromArgb(r,g,b)`, `FromArgb(a,r,g,b)`,
   named `Color.X`, `?? Color.Empty`); per-skin two-theme render differential; zero bare
   `catch { }`; zero `Debug.WriteLine` [08]
8. Cross-cutting: `BeepLog.Reported` collected for the whole run — any unexpected `Failure`
   fails the probe; a derived form (`CardsWizardForm`) constructs, shows, renders, closes.

## Break-it-first ledger

Every check lands with the break that proves it can fail, and this file records which were
actually run — "the check exists" and "the check has been seen red" must never look the same:

| check | break | seen red on |
|---|---|---|
| `runtime: caption tracks the new width` | remove `programmaticNormalResize` from `ShouldRefreshChromeAfterWmSize` | 2026-08-12 — red (584→584 vs client 898) |
| `design-mode: caption tracks the new width` | restore `OnResize` to a no-op | 2026-08-12 — red (0→572 vs client 886) |
| `guard: the two sizes differ` | (guard — fails by construction if sizes are equal) | n/a |
| `the last invalidate of a burst is not dropped` | `DebouncedInvalidate` back to leading-edge only | 2026-08-12 — red (0 invalidates after the suppressed call) |
| `a derived form's own shape survives a style change` | `ReleaseManagedRegion` ignores `OwnsWindowShape` | 2026-08-12 — red |
| `a derived form's own shape survives a resize` (+2) | `ApplyWindowShape` ignores a derived shape | 2026-08-12 — red (3 checks) |
| `the window region follows the derived shape too` | `ShapeBackend` forgets `OwnsWindowShape` | 2026-08-12 — red (Win32 half only) |
| `the default style selects the DWM backend` (+3) | `ShapeBackend` never selects `Dwm` | 2026-08-12 — red (4 checks) |
| `the DWM backend sets no managed region` (+2) | DWM backend leaves a region in place | 2026-08-12 — red |
| `guard: an idle form does not invalidate itself` | (blindness guard — red if anything else repaints) | n/a |
| `guard: this machine can round windows through DWM` | (blindness guard — **fails**, not skips, on Win10) | n/a |
| `the caption exposes one child per visible region` (+3) | `GetChildCount()` returns -1 again | 2026-08-12 — red (4 checks) |
| `toggling a caption button changes the count by one` | child list hardcoded to three | 2026-08-12 — red (this check alone) |
| `the close child actually closes the form` | `DoDefaultAction` made inert | 2026-08-12 — red |
| `the form's own accessible name survives focus` | restore the identity mutation | 2026-08-12 — red |
| `hovering maximize reports HTMAXBUTTON` | never report `HTMAXBUTTON` | 2026-08-12 — red |
| `every caption tooltip says what the reader says` | give the tooltip its own text source | 2026-08-12 — red |
| `clicking maximize still maximizes` | `WM_NCLBUTTONUP` no longer acts | 2026-08-12 — red |
| `metrics a painter receives are DPI-scaled` (+2) | `MetricsFor` drops the DPI argument | 2026-08-12 — red (3 checks) |
| `WM_DPICHANGED discards stale metrics` | gut `HandleDpiChanged` | 2026-08-12 — red |
| `a metrics-driven skin responds to DPI` | one painter bypasses the seam | 2026-08-12 — red |
| `no painter re-scales a pre-scaled metric` | insert 5 real double-scale calls | 2026-08-12 — **STAYED GREEN; check unproven** |

**Two checks that could not fail, both found by breaking them:**

- `WM_DPICHANGED adopts the suggested rect` — WinForms' `base.WndProc` does that, so the check
  passed with our handler gutted. Replaced with the one effect only our handler has.
- `no painter re-scales a pre-scaled metric` — still green with five genuine double-scale calls
  inserted. Cause not found. Recorded as decorative rather than quietly kept; see 04-dpi.md.

**Three instruments were wrong before any code was.** Full account in
`02-geometry-and-invalidate.md`; the short form, because each will recur:

1. Counting `Paint` on an off-screen form — it never receives `WM_PAINT`, so the counter reads 0
   whatever the code does. Count `Invalidated`.
2. Hashing a region at points scaled to the form — a region correctly left alone still hashes
   differently after a resize. Compare reference identity when the question is "was it replaced?".
3. `Thread.Sleep(2)` to sit inside a 16 ms window — sleep is a floor, and on a loaded machine it
   overshoots. The break was **green on a busy machine, red on an idle one**. Remove the timing
   dependency rather than widening the tolerance.

**A break runner must check the build's return code, not grep for `error CS`.** A locked exe fails
with MSB3021/MSB3027, which that grep does not match — the build looks clean, the old binary stays
on disk, and every break reports "still green" because none of them were ever in the running code.

**Probe status:** `FormProbe` exists in the session scratchpad (`scratchpad/FormProbe`), builds
clean, runs in ~4s, watchdog + hard exit, **stages 01/02/02b/03/04 green (37 checks, 0 failures — one of them unproven, see the ledger)**.

**Capture note, learned the hard way:** `DrawToBitmap` returns a blank image for this form —
`WM_PRINTCLIENT` does not invoke the custom `OnPaintBackground`. `Shot()` drives that method over
a magenta-prefilled bitmap instead. Any future check that renders this form must do the same, or
it will "prove" the chrome is missing.

## Exit criteria

Probe green end-to-end on a clean build; every stage's checks present with their break-ledger
rows filled; runtime under 3 minutes so it stays in the loop rather than getting skipped.

## The contact sheet now names its own skins (2026-08-13)

Every band rendered the caption text "BeepiFormPro", so a single render said nothing about which
painter drew it — you had to trust the label the sheet builder printed underneath. `Capture` now sets
`f.Text = style.ToString()`, so each band identifies itself and a screenshot of one band is evidence
on its own.

It paid for itself immediately: with real titles in place, **Cartoon, GNOME and iOS centre their
title text while the other 34 skins left-align it.** That difference was invisible for the whole
session because every title was the same word in the same place.

