# BeepiFormPro — commercial-grade enhancement plan

Master tracker for `Forms/ModernForm/`. Goal: a form chrome that stands next to DevExpress /
Telerik / SyncFusion — correct in the designer, correct per-monitor, accessible, fast, and
consistent across every one of its skins.

**58 files, 26,741 lines: the form (12 partials), 38 skin painters, metrics, managers, designers.**

## Where it stands today

The foundation is real: custom non-client area via `WM_NCCALCSIZE`, a painter-per-skin
architecture, hit-area + interaction managers, DWM bits already imported
(`DWMWA_SYSTEMBACKDROP_TYPE`, `DwmExtendFrameIntoClientArea`), a theme bridge, a backdrop layer,
and a designer with an action list. What separates it from a commercial product is not missing
ambition — it is correctness at the seams: the designer, resize, DPI, accessibility, and 38
painters that each re-implement the same chrome slightly differently.

## Headline findings (evidence, not opinion)

| # | finding | where |
|---|---|---|
| F1 | **Design-time resize does not refresh content** (the reported bug). `OnResize` is a deliberate no-op — geometry sync is deferred to `OnResizeEnd` / `WM_EXITSIZEMOVE`, which fire on *interactive* resizes only. The VS designer resizes via `SetBoundsCore`/property writes and never sends them, so nothing re-syncs. | `BeepiFormPro.Events.cs:139-149`, `Win32.cs:347-369` |
| F2 | The designer is a **`ParentControlDesigner`** on a *Form* root — the root designer for a document is `DocumentDesigner`; and it hooks component/selection changes but never `Resize`/`SizeChanged`. | `Designers/BeepiFormProDesigner.cs:10` |
| F3 | **`DebouncedInvalidate` swallows the trailing repaint**: leading-edge-only, returns without scheduling, so the *last* invalidate of any burst (< 16 ms after the previous) is silently dropped — the classic "stops one frame stale" bug. | `BeepiFormPro.cs:54-63` |
| F4 | **Two competing window-region authorities** — `UpdateFormRegion` (managed `Region`) and `UpdateWindowRegion` (`SetWindowRgn`), both rebuilding a rounded rect on every size/style change. Already proven hostile: `CustomToolTip` had to abandon this base entirely because its silhouette was overwritten (commit `0dc50a57`). Region-based rounding also kills the DWM drop shadow and gives aliased corners; Win11 has `DWMWA_WINDOW_CORNER_PREFERENCE` for exactly this. | `Drawing.cs:456-499`, `Win32.cs:490-540` |
| F5 | **No per-monitor DPI handling**: no `WM_DPICHANGED` anywhere; the ctor hard-codes `AutoScaleDimensions = (96,96)` directly under a comment saying to remove it — derived forms' designer files set their own, a double-scale hazard. | `BeepiFormPro.cs:70-73`, `Win32.cs` (absent) |
| F6 | **No Win11 snap layouts**: `WM_NCHITTEST` never returns `HTMAXBUTTON`, so hovering the maximize button shows no snap flyout — an immediate "not native" tell next to commercial suites. | `Win32.cs:268+` |
| F7 | **Painted caption buttons are invisible to accessibility**: close/min/max/theme/style/profile/search are hit-area rectangles, not controls — no UIA tree entries, no keyboard activation, no tooltips, no focus visuals. | `Managers/BeepiFormProHitAreaManager.cs`, `Core.cs:802-1240` |
| F8 | **38 painters × 300–700 lines ≈ 16k lines of near-duplicate chrome.** Every painter re-implements caption layout, button glyphs, borders and shadows; `FormPainterMetrics` is 1,455 lines and `FormPainterRenderHelper` 875 on top. Skin parity is unverifiable by reading. | `Painters/*` |

## Stages

| # | Stage | Kind | Status |
|---|---|---|---|
| [01](01-design-time.md) | The designer: resize refresh + a real root designer | **bug** (reported) | ☐ open |
| [02](02-geometry-and-invalidate.md) | One geometry authority; invalidate that cannot drop the last frame | **bug** | ☐ open |
| [03](03-caption-accessibility.md) | Caption bar: UIA, keyboard, tooltips, snap layouts | enhancement | ☐ open |
| [04](04-dpi.md) | Per-monitor DPI v2, `WM_DPICHANGED`, one AutoScale story | **bug** | ☐ open |
| [05](05-painter-architecture.md) | 38 painters on one chrome pipeline; skin parity enforced | refactor | ☐ open |
| [06](06-backdrop-effects.md) | Mica/Acrylic backdrops, DWM shadow, transitions | enhancement | ☐ open |
| [07](07-verification.md) | `FormProbe` — the harness the other stages report through | verification | ☐ open |

Status marks: ☐ open · ◐ in progress · ☑ done

## Order rationale

**01 first** because it is the reported bug and everything later is eyeballed on the design
surface too. **07 is written alongside 01**, not last — every stage needs a check that can fail
before its work starts (repo doctrine: break the instrument first). 02 before 03/05 because the
caption and the painters both sit on the geometry that 02 makes single-owner. 05 before 06 so the
backdrop lands on one pipeline, not 38.

## Standing constraints (CLAUDE.md — these hold in every stage)

- Every catch reports through `BeepLog` (`…Once` in paint paths). No bare `catch { }`.
- Theme slots only; no literal colours; a wrong colour is the theme's bug, fixed in its part file.
- Cached `Font`s are never disposed. Sizes scale through `DpiScalingHelper`.
- Compose from Beep controls where a control exists; no control flow in `InitializeComponent`.
- Delete dead paths rather than keeping them beside the new one; record public-member removals here.
- A check must be able to fail for the reason it was written — break it first, and record which
  break-tests were actually run.
- Renders are **eyeballed**, not just counted. `Control.DrawToBitmap`, never `CopyFromScreen`.

## Known hazards for whoever executes this

- **`WizProbe` in the scratchpad hangs on unmodified library code** (recorded in
  `Wizards/plans/00-MASTER-TRACKER.md`). Do not reuse it; stage 07 builds fresh. `StepperProbe`
  is healthy and is the model to copy.
- The probe/exe lock trap: a hung probe holds its own `.exe`, silently turning every later build
  into a no-op — always verify build error counts *before* trusting a run, and hard-exit +
  watchdog every probe.
- `ControlStyle` pins a bundled theme; theme-responsiveness checks must switch themes *after*
  control creation.
- Derived forms in this repo (wizard forms, dialogs, popups) inherit every behavioural change made
  here; stage exit criteria include a build of the whole library plus a spot-render of one derived
  form.

## Record as you go

A stage marked done must say what was actually done, what was verified (including which
break-it-first tests ran), what was **not** verified, and every public member added or removed.
"Checked and fine" and "not checked" must never look the same to the next reader.
