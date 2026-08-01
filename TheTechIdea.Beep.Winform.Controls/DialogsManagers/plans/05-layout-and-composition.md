# 05 — Layout & Composition

**Priority P1. Phase 3.** This is the structural change requested for this program.

## Current behaviour

**No dialog form uses `TableLayoutPanel`.** Measured across every `.cs` and `.Designer.cs` under
`Forms/`: zero occurrences. Every form positions with absolute `Location` plus `Dock`/`Anchor`:

| Form | `Location = new` assignments |
|---|---|
| `BeepQuestionDialog.Designer.cs` | 10 |
| `BeepInputDialog.Designer.cs` | 10 |
| `BeepListDialog.Designer.cs` | 9 |
| `BeepCustomDialog.Designer.cs` | 7 |
| `BeepMultiSelectDialog.cs` | 4 |
| `BeepMessageDialog.Designer.cs` | 4 |

Absolute coordinates in a dialog are the wrong tool for three reasons that all bite in practice:

1. **Content is variable.** A message dialog's height depends on how long the message is, and a
   question dialog's width on how many buttons it has. Fixed coordinates cannot express that, so the
   sizes get hardcoded to the longest string someone tested with.
2. **DPI.** Coordinates authored at 96 dpi do not scale; a `TableLayoutPanel` with percentage and
   `AutoSize` rows does.
3. **Localisation.** A German button caption is routinely 40% longer than English. Absolute layout
   clips it; an `AutoSize` column does not.

This is also a standing rule for this codebase: **views align via `TableLayoutPanel`, not dock stacks
or flow panels.**

## What the reference products do

Every modern dialog system composes from named regions rather than coordinates:

| System | Regions |
|---|---|
| Radix / shadcn `Dialog` | `Overlay`, `Content`, `Header`, `Title`, `Description`, `Footer`, `Close` |
| Material 3 | icon, headline, supporting text, actions |
| Ant Design `Modal` | `title`, `content`, `footer`, `closeIcon` |
| Fluent 2 `Dialog` | `DialogSurface` → `DialogTitle`, `DialogBody`, `DialogActions` |
| VS Code modal | icon + message + detail + button row |

The shared structure is the same everywhere: **header / body / footer**, with the footer holding the
action buttons and the body taking the remaining space. That maps exactly onto a three-row
`TableLayoutPanel`.

## Work

1. **One dialog chrome, composed once.** Introduce a single layout scaffold — a
   `TableLayoutPanel` with three rows:

   | Row | Sizing | Holds |
   |---|---|---|
   | Header | `AutoSize` | icon, title, close affordance |
   | Body | `Percent(100)` | message, input, list, or a caller-supplied control |
   | Footer | `AutoSize` | action buttons, right-aligned |

   Every dialog form composes into that scaffold instead of positioning its own controls. The body
   is the only part that differs between a message, a question, an input and a list dialog — which
   is the separation this program is after.

2. **Buttons in an `AutoSize` footer row.** Button rows are where absolute layout hurts most
   (count varies, captions localise). The footer becomes its own single-row `TableLayoutPanel` with
   one `AutoSize` column per button.

3. **Delete per-form positioning.** Once the scaffold owns structure, the `Location` assignments in
   the six forms above go with it.

4. **Respect the designer constraint.** `InitializeComponent` must contain no loops or conditionals
   — the VS designer parser cannot round-trip them. The scaffold is therefore built with explicit,
   straight-line row/column definitions, and anything variable (button count) is composed at runtime
   *outside* `InitializeComponent`.

## Known limitation — `BeepLabel` cannot be constrained by a container

Four of the five dialogs lay out and render correctly on the shell. The long-message case does not,
and the cause is outside this directory.

`BeepLabel.RecalculateMinimumSize` measures its text with:

```csharp
TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue), …)
```

— deliberately unwrapped — and assigns the result back to `MinimumSize`, re-asserting it on every
recalculation. A three-sentence message produces `MinimumSize.Width = 1788` inside a 428px shell.

That defeats every container-side remedy, all of which were tried and measured:

| Attempt | Result |
|---|---|
| `AutoSize = false` | ignored; `MinimumSize` outranks it |
| Clear `MinimumSize.Width` | control restores it on the next recalculation |
| `MaximumSize.Width = available` | ignored; `MinimumSize` outranks it |
| Assign `Width` directly at layout | reset back to 1788 |

**A container cannot constrain a control that sizes itself.** The fix belongs in `BeepLabel` —
measure against the available width rather than `int.MaxValue` when `WordWrap` is set:

```csharp
int constraint = WordWrap && Width > 0 ? Width : int.MaxValue;
```

This is solution-wide, not a dialog concern: any wrapping `BeepLabel` in any container will overrun
it. The same method also carries a bare `catch { MinimumSize = new Size(120, 28); }` — the swallowed
exception class this program removed from `DialogsManagers`, still present in a shared control.

Deferred pending a decision on changing shared controls.

## Verification

- ⬜ Harness: no `Location = new` in any form under `Forms/`.
- ⬜ Harness: every dialog form's root layout is a `TableLayoutPanel`.
- ⬜ Render at 100%, 150% and 200% DPI and assert the footer buttons stay inside the form and the
   body region absorbs the difference.
- ⬜ Render with a caption 3× longer than the design string and assert nothing clips — the
   localisation case, which is the one absolute layout always fails.
- ⬜ Harness: no control flow in any `InitializeComponent`.

---

## The deferred item, resolved: `BeepLabel.UpdateMinimumSize`

Held back until approved because `BeepLabel` is used by 38 files. Measured with a controlled
baseline before and after — nine label configurations, plus rendered-pixel verification.

### The defect

It measured every label as one unbroken line at `int.MaxValue` width and wrote that to
`MinimumSize`, which outranks both `AutoSize` and the container. A paragraph produced a label pinned
wider than the form holding it (730px inside a 420px dialog; 1788px for a longer message), so it
never wrapped and ran off the edge.

`WordWrap`, `Multiline`, `AutoEllipsis` and an explicit `Width` all produced the **identical** figure,
because none of them were consulted. The paint path had always measured against the available width
when wrapping — only this method disagreed.

### Before → after

| case | before | after | |
|---|---|---|---|
| `short-plain` | 86x26 | 86x26 | unchanged |
| `short-with-sub` | 86x40 | 86x40 | unchanged |
| `long-plain` | 730x26 | 730x26 | unchanged — a single-line label *must* fit its text |
| `long-ellipsis` | 730x26 | **86x26** | the container picks the width; that is what the ellipsis is for |
| `long-wordwrap` | 730x26 | **86x83** | width collapses, height grows to three lines |
| `long-wrap-sized` (W=380) | 730x26 | **86x53** | two lines — proof it measures against the real width |
| `wrap-set-after` | 730x26 | **86x26** | second defect: the flag setters never recalculated |

### Also fixed

- `Multiline` / `WordWrap` / `AutoEllipsis` setters now recalculate. Changing one after `Text` left
  the value measured under the previous setting in force.
- The bare `catch { MinimumSize = new Size(120, 28); }` is gone. It replaced any failure with a fixed
  wrong minimum on every consumer instead of surfacing a diagnosable error.

### A false alarm worth recording

An intermediate check compared the control's height against `TextRenderer.MeasureText(..., WordBreak)`
and reported the long message clipped by 4px. It was wrong: `TextRenderer`'s wrap point depends on the
exact `TextFormatFlags`, so a probe using `WordBreak` and a control using its own flag set differ
without either being incorrect. **Two measurements disagreeing says nothing about which is right.**

Settled with rendered pixels against a stock WinForms `Label` at the same width and font: three text
bands at BeepLabel's preferred 53px, three at a generous 300px, three in the stock Label. Nothing is
clipped.
