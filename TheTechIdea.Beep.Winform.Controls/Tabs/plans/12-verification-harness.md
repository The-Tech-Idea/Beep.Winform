# 12 — Verification Harness

**Priority P0. Build this first.**

## Why first

`Tabs/` has 9,638 lines and no harness. Every defect this program lists was found by reading code,
which is the weakest method available — and in the three preceding programs, reading consistently
missed things that one render or one counter exposed immediately:

- **`BeepGridPro`** — a toolbar bug was "fixed" three times against rectangle coordinates before a
  PNG showed a null-brush exception silently aborting half the paint.
- **`BeepTree`** — 25 painters clipped every label; a contact sheet found it at a glance, and the
  same sheet immediately surfaced a painter that rendered nothing and another that truncated.
- **`ToolTips`** — a manual audit found three never-read properties; a reflection check found six.
  The harness also caught two of my own wrong assumptions before they became "fixes".

The harness is also the only way to hold the ground rules. "No stubs", "no bare catches" and
"no duplication" are testable properties, not review habits.

## What it must do

### 1. Enforce the ground rules mechanically

| Check | Fails when |
|---|---|
| Bare catch | any `catch { }` or `catch { return …; }` exists under `Tabs/` |
| Stub | a method body is empty without an explicit `// intentionally empty: <reason>` |
| Dead public surface | a public property on the tab models is never read in the assembly |
| Empty directory | any directory under `Tabs/` contains no files |
| Measure/draw font | a painter names a system font as its draw font (non-fallback) |
| High contrast | more than one file consults `SystemInformation.HighContrast`, or nothing reads `IsHighContrast` |
| Naming honesty | an identifier or comment opens by declaring something `Legacy` or `Phase N` |
| Failure surfacing | a `ReportError` call is not followed by a rethrow in its catch block |
| Error channel | no Release-visible `TabError` event exists |

**Current state: 16 checks, all green.**

### A note on text-matching checks

Three checks in this harness were wrong before they were right, all of them text matching against
C# without parsing it:

- the dead-member check counted declarations as call sites, then — once "fixed" with a negative
  lookbehind — excluded every receiver-prefixed call instead. It is now **informational only**;
  deadness is proven by deleting the member and compiling.
- the never-read check reported 34 inherited `BaseControl` properties until it was given
  `DeclaredOnly`.
- the naming check flagged the corrective prose explaining why the old names were wrong.

The naming detector is therefore **self-tested**: it asserts it flags the three verbatim original
strings and exempts the three corrective sentences, and that self-test runs before its verdict is
trusted. Any new text-matching check in this harness should carry the same.

These are cheap, and each corresponds to a defect already found in this folder.

### 2. Geometry agreement

The measure path and the render path must produce the same rectangles
([02](02-measure-render-pipeline.md)):

- `GetCurrentHeaderTabRects()` equals the host's painted item rects, for the same state.
- Measured text width equals the width the painter draws with, using the same font instance.
- Laid-out tabs plus header actions never exceed the header bounds.

### 3. Rendering

`DrawToBitmap` captures, plus magnified crops where a few pixels matter:

- **Contact sheet**: 7 painters × states (selected / hovered / pressed / disabled / pinned / dirty /
  preview) × ≥3 themes including a hostile one.
- **Distinctness assertion**: no two painters render identically for identical input.
- **DPI sweep**: 100% / 150% / 200% — where measure/draw mismatches surface as clipping.
- **RTL render**: mirrored order, close-button side, overflow direction.
- **High contrast**: every painter, system colours honoured.

### 4. Behaviour

- Overflow: 40 tabs in a 600px header — all reachable, selected visible, pinned always drawn.
- Modes: each of the three yields a different, documented behaviour set.
- Keyboard: every binding drives the expected selection; Ctrl+Tab follows MRU, not position.
- Reorder: drag routes through `MovePage`; constraints honoured; cancel restores order.
- Accessibility: the tree exposes roles, names, selected state and set positions.

### 5. Lifecycle

- Add/remove 1,000 pages: handle and window counts return to baseline.
- Dispose a page without removing it and assert the control does not retain it — the exact leak found
  in `ToolTips`, where the manager retained 20 disposed anchors.

## Work

1. Build `scratchpad/TabsProbe` and **reproduce the known defects first** — the dead `PaintTab`, the
   four bare catches, the `ApplyFontTheme` no-op — so each fix has a failing check to turn green.
2. Add the geometry and rendering checks before touching the pipeline in
   [02](02-measure-render-pipeline.md).
3. Promote it out of the scratchpad once stable, runnable from one command.
4. Store baseline images so later changes diff rather than needing re-review.

## Verification

The harness is verified by reproducing today's known defects. If a fresh run does not fail on the
bare catches, the no-op method and the dead interface member, it is not measuring the right things.

**A check that cannot measure must fail, not pass.** In the tooltip program a distinctness assertion
captured nothing and then compared zero against zero — reporting success while measuring nothing.
Every assertion here states its sample size and fails when that sample is empty.
