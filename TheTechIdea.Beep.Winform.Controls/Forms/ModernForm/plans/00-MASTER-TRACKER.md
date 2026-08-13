# BeepiFormPro — commercial-grade enhancement plan

Master tracker for `Forms/ModernForm/`. Goal: form chrome that stands next to DevExpress /
Telerik / SyncFusion — correct in the designer, correct per-monitor, accessible to automation,
themed, and consistent across every one of its skins.

**58 files, 26,741 lines: the form (12 partials), 38 skin painters, 1,455-line metrics,
875-line render helper, hit-area + interaction managers, designer + action list.**

> **Revision 2.** The first draft was written from a structural survey. Every partial has since
> been read, and four of its findings were wrong or imprecise — corrected below and marked
> ⚠︎. Recorded rather than quietly edited, because "the plan said X" gets repeated downstream.

## What is already good (do not rebuild these)

Worth stating, because a plan that lists only faults invites replacing working machinery:

- **Custom non-client chrome** via `WM_NCCALCSIZE`/`WM_NCHITTEST` with a real hit-area manager
  and interaction manager — the hard part, and it works.
- **Keyboard operation of the caption already exists**: `F6`/`Shift+F6` cycles caption regions,
  `Ctrl+F` focuses the search box, `Alt+Space` opens the system menu, Enter activates the focused
  region, and a focus indicator is painted (`Win32.cs:739-800`, `Core.cs:502-600`).
- **Accessible names, roles and descriptions** are authored per caption region
  (`Core.cs:620-680`) — the vocabulary exists; only its delivery is wrong (F7).
- **A real DWM/composition backdrop implementation** — `BeepiFormPro.Backdrop.cs` does
  `DwmSetWindowAttribute(DWMWA_SYSTEMBACKDROP_TYPE)`, `SetWindowCompositionAttribute` acrylic,
  blur-behind, with OS-version gating (Win10 vs Win11 build 22000).
- **Central DPI scaling of metrics** — `FormPainterMetrics.cs:1420-1450` scales caption height,
  button widths, icon size, spacing, search box and paddings in one place.
- A layout cache keyed on size/style/every `Show*` flag (`Core.cs:36-60`), and a painter registry.

## Headline findings

| # | finding | where |
|---|---|---|
| F1 ☑ | **FIXED (stage 01).** **Design-time resize does not refresh** (the reported bug). `OnResize` is a deliberate no-op; all geometry sync is deferred to `OnResizeEnd`/`WM_EXITSIZEMOVE`, which fire only for *interactive* resizes. The designer resizes by property write, so the sync never runs. Affects runtime `form.Size = …` identically. | `Events.cs:139-149`, `Win32.cs:347-369` |
| F2 | Root designer is a **`ParentControlDesigner`** on a *Form* (root documents use `DocumentDesigner`), and hooks component/selection changes but never `Resize`/`SizeChanged`. | `Designers/BeepiFormProDesigner.cs:10` |
| F3 ☑ | **FIXED (stage 02).** **`DebouncedInvalidate` drops the trailing repaint** — leading-edge only, returns without scheduling, so the last invalidate of a burst is silently lost. | `BeepiFormPro.cs:54-63` |
| F4 ☑ | **FIXED (stage 02)** — one authority (`ApplyWindowShape`), plus a DWM backend and `OwnsWindowShape`. **Two window-region authorities** (`UpdateFormRegion` managed `Region` + `UpdateWindowRegion` `SetWindowRgn`), both rebuilding a rounded rect. Already forced `CustomToolTip` off this base (commit `0dc50a57`). Regions also give aliased corners and **kill the DWM shadow**; Win11 has `DWMWA_WINDOW_CORNER_PREFERENCE`. | `Drawing.cs:456-499`, `Win32.cs:490-540` |
| F5 | **No `WM_DPICHANGED`** anywhere; ctor hard-codes `AutoScaleDimensions = (96,96)` under a comment saying to remove it, while derived designer files serialize their own. | `BeepiFormPro.cs:70-73` |
| F6 ☑ | **FIXED (stage 03).** **No Win11 snap layouts** — `WM_NCHITTEST` never returns `HTMAXBUTTON`. | `Win32.cs:268+` |
| F7 ☑⚠︎ | **FIXED (stage 03)** — a real UIA child per visible caption region; identity mutation deleted; tooltips added. **Accessibility is delivered by mutating the form's own identity.** Correction: names/roles/keyboard all exist, but there are **zero `AccessibleObject`s** in the folder — focus changes swap the *form's* `AccessibleName`/`Role`/`Description` (`CaptureCaptionAccessibilitySnapshot`, `_baseAccessibleName`). A screen reader sees one object whose identity mutates; UIA cannot enumerate the buttons, and automation cannot invoke them. Also no per-button tooltips. | `Core.cs:333-680` |
| F8 | **38 painters, ~16k lines** of near-duplicate chrome, atop a 1,455-line metrics file and 875-line render helper. Skin parity is unverifiable by reading. | `Painters/*` |
| F9 ⚠︎ **revised twice** | **Four overlapping backdrop switches, and the DWM ones cannot work here.** `BackdropEffect` (`Core.cs:950`) paints a flat translucent literal; `Backdrop`/`BackdropType`, `EnableAcrylicForGlass` and `EnableMicaBackdrop` (`Backdrop.cs`) call real DWM APIs — which **succeed and stay invisible**: `DwmExtendFrameIntoClientArea` is declared and never called, 30/38 painters fill the whole client rect, and **36/38 set `CompositingMode.SourceCopy` deliberately** to replace pixels. A system backdrop shows only through transparent pixels; this architecture has none. Rev 2 called the painted one "the placebo" and the DWM one "real" — backwards in terms of visible effect. **Ruling: option A** — drop DWM backdrops, keep one honestly-named painted effect. | `Core.cs:948`, `Drawing.cs:193`, `Backdrop.cs`, `Painters/*` |
| F10 ⚠︎ **revised** | Colour literals: 1,026 total, but **the skin painters are exempt by ruling** — a painter's palette *is* the skin, and each matches its own theme (Dracula ↔ Dracula, Nord ↔ Nord). Exempt: `Painters/*FormPainter.cs` (566) and `FormPainterMetrics.cs` (535, per-style palette defaults). **Actually in scope: ~30** — `Core.cs` (17, incl. a hard-coded close-button red and a light/dark grey ladder the theme already publishes), `FormPainterRenderHelper.cs` (7, shared code not a skin), `Drawing.cs` (6, deleted by stage 06). | measured split in [08](08-theme-and-reporting.md) |
| F11 **new** | **28 bare `catch { }` and zero `BeepLog` calls** in 26.7k lines; `Backdrop.cs` reports 9 failures to `Debug.WriteLine`. Every backdrop, layout and style failure is currently silent. | folder-wide |
| F12 ⚠︎ | DPI: correction — metrics **are** centrally scaled (`FormPainterMetrics.cs:1420-1450`). The residual risk is narrower than first stated: **6 painters** also call `DpiScalingHelper` directly, which is where double-scaling can enter. | `Painters/{Linear,MaterialYou,Modern,NextJS,RadixUI,…}` |

## Stages

| # | Stage | Kind | Status |
|---|---|---|---|
| [01](01-design-time.md) | The designer: resize refresh + a real root designer | **bug** (reported) | ☑ **done** — resize fixed; `DocumentDesigner` rebase deferred (see 01) |
| [02](02-geometry-and-invalidate.md) | One geometry authority; invalidate that cannot drop a frame | **bug** | ☑ **done** — items 1–4; live-resize (item 5) deferred, see 02 |
| [03](03-caption-accessibility.md) | Caption: real UIA children, tooltips, snap layouts | **bug** | ☑ **done** — items 1–4; system-menu parity not verified, see 03 |
| [04](04-dpi.md) | Per-monitor DPI v2; close the painter-side scaling hole | **bug** | ☐ open |
| [05](05-painter-architecture.md) | 38 painters onto one chrome pipeline; parity enforced | refactor | ◐ harness + audit; custom action fixed (33 dead skins → 2); migration target revised to the 25 hand-rolled layouts |
| [05a](05a-painter-read-log.md) | Read all 38 painters in full, in stages | evidence | ◐ 1/38 read (Modern — 6 findings, 3 bugs no existing check would catch) |
| [06](06-backdrop-effects.md) | One backdrop API — drop DWM backdrops, keep a real painted effect | **bug** | ☐ open |
| [07](07-verification.md) | `FormProbe` — the harness every stage reports through | verification | ◐ live — stages 01/02/02b/03/04 green (37 checks), 17 breaks recorded, 1 check unproven |
| [08](08-theme-and-reporting.md) | 28 silent catches → `BeepLog`; ~30 non-skin literals → slots | **rule 1** (+4, scoped) | ☐ open |

Status marks: ☐ open · ◐ in progress · ☑ done

## Order rationale

**01 first** — the reported bug, and every later stage is eyeballed on the design surface.
**07 is written alongside 01**, never last: each stage's checks must be seen red before its fix
(repo doctrine — break the instrument first). **02 before 03/05** because caption and painters
both sit on the geometry it makes single-owner.

**06's ordering changed with its ruling.** It used to sit behind 02 because "a `SetWindowRgn`
region defeats the system backdrop" — void now that there is no system backdrop. What remains is a
property collapse (which can land immediately, nothing depends on it) plus one painted effect that
belongs with stage 05's skin descriptor.

**08 runs in parallel** with anything after 02 — a sweep, not a redesign, and its fences protect
every later stage.

## Standing constraints (CLAUDE.md — hold in every stage)

- Every catch reports through `BeepLog` (`…Once` in paint/timer paths). No bare `catch { }`.
- Theme slots only — **with one standing exemption**: the 38 skin painters and
  `FormPainterMetrics`' per-style defaults may use colour literals, because a skin's palette *is*
  the skin and matches its corresponding theme. That exemption covers colour only, never silence:
  the `BeepLog` rule applies to painters like everything else. Everywhere outside those files, a
  wrong-looking colour is the **theme's** bug, fixed in its part file.
- Cached `Font`s are never disposed. Sizes scale through `DpiScalingHelper` — once.
- Compose from Beep controls where one exists; no control flow in `InitializeComponent`.
- Delete dead paths rather than keeping them beside the new one; record public-member removals here.
- A check must be able to fail for the reason it was written. Record which break-tests were run.
- Renders are **eyeballed**, not counted. `Control.DrawToBitmap`, never `CopyFromScreen`.

## Hazards already paid for in this repo

- **`WizProbe` hangs on unmodified library code** (`Wizards/plans/00-MASTER-TRACKER.md`) — do not
  reuse it. `StepperProbe` is the healthy model: watchdog, hard exit, flush.
- **The stale-binary trap**: a hung probe holds its own `.exe`, silently turning later builds into
  no-ops. Check build error counts *before* trusting any run — this produced three false
  conclusions in one session.
- **Differentials beat thresholds** where layout can shift: a Steppers check measured a band
  derived from step positions, and the fix moved the band — it reported *less* ink after working.
- **`ControlStyle` pins a bundled theme**; theme-responsiveness checks must switch themes *after*
  construction.
- Every derived form (wizards, dialogs, popups) inherits changes made here — each stage exits with
  a full-library build plus a spot-render of one derivative.

## Public-member changes already known

Seeded from the survey so a breaking change is never a surprise at merge time; filled in as
stages land.

| member | stage | change | notes |
|---|---|---|---|
| `BeepiFormPro.Backdrop` (`BackdropType`) | 06 | **remove** | cannot work while a painter owns the pixels |
| `BeepiFormPro.EnableAcrylicForGlass` | 06 | **remove** | same |
| `BeepiFormPro.EnableMicaBackdrop` | 06 | **remove** | same; duplicates `Backdrop = Mica` |
| `BeepFormUIManager.Backdrop` (`BackdropType?`) | 06 | **remove** | external consumer at `Managers/BeepFormUIManager.cs:135,397`; **not** on `IBeepFormUIManager`, so the interface is unaffected |
| `Forms/Effects/BackdropType.cs` | 06 | **remove** | only after both consumers above |
| `BeepiFormPro.BackdropEffect` | 06 | **rename** | to `SurfaceOverlay` (name to confirm): what survives is a painted treatment, not a compositor backdrop |
| `BeepiFormPro.OwnsWindowShape` | 02 | **add** (protected virtual) | lets a derivative own its silhouette - the API `CustomToolTip` needed and never had |
| `CreateAccessibilityInstance` + caption child objects | 03 | **add** | real UIA tree replacing the identity-mutation approach |

## Record as you go

A finished stage must state what was done, what was verified (naming the break-tests that ran),
what was **not** verified, and every public member added or removed. "Checked and fine" and
"not checked" must never look the same to the next reader.
