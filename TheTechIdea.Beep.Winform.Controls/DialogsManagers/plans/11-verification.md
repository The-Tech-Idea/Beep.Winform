# Stage 11 — the verification harness

Every claim in stages 01–10 is asserted here. Built as `scratchpad/DialogProbe`, following the
`DockProbe` / `NavBarProbe` pattern that is working.

**Runs first.** The baseline must exist before stage 01 changes a line, or most of the conformance
checks have nothing to compare against.

## The rules that govern this stage

**A check must be able to fail for the reason it was written.** Across the preceding programs this
caught more defects in the *checks* than in the code, and two of those failures will recur here
unless the harness is built to avoid them:

- **Accessibility traversal measures itself.** A stock `Form` reports `-1` from `GetChildCount()` and
  MSAA enumerates the window hierarchy, so a check that counts "some" accessible children passes
  against a control with no accessibility implementation. Stage [01](01-focus-and-accessibility.md)
  is the largest stage here and is entirely exposed to this. **Every accessibility count runs against
  a stock-form control group first**, and the run records both numbers.
- **Rendering offscreen validates the wrong mechanism.** In the dock program, DPI scaling and icon
  tinting both looked correct in a bitmap harness and were wrong on screen — one because
  `Graphics.DpiX` reports 96 in a paint handler, the other because the tint parameter was discarded
  for SVGs. **Dialogs are rendered as shown windows and captured from the screen**, not by
  `DrawToBitmap` on an unparented form. A dialog is a window; measuring it as a bitmap measures
  something else.

A third, learned in the same programs: **verify the fixture before trusting anything measured with
it.** Icons that render as brand logos instead of tinted glyphs, items with no `ImagePath`, a control
that is docked when the test assumed it was not — each produced confident numbers about nothing. The
first checks below are fixture checks.

## Baseline capture — run first

1. Render all eight dialog forms × five severities × {plain, tinted} to PNG. This is the corpus every
   conformance stage diffs against.
2. Record, per form: the accessible child count, `AccessibleRole`, `AccessibleName`, whether
   `AcceptButton`/`CancelButton` are set, and the initial `ActiveControl`.
3. Record the focusable count and whether Tab escapes the dialog.
4. Record button rectangles for all button roles and layouts.
5. Record backdrop count and opacity for one, two and three nested dialogs.

Stages that change rendering diff against this corpus and **justify every difference**. An
unexplained pixel change is a finding, not noise.

## Fixture checks — before anything else

- Every icon the fixture references exists, and renders **ink** rather than an empty field.
- A rendered dialog is not a uniform fill.
- The control group (stock `Form` + `Label` + two `Button`s) does **not** report the numbers the
  accessibility checks expect.

## Ground rules — mechanical, run every time

- no bare `catch` / `catch (Exception)` whose body neither rethrows nor reports.
  **This folder is already clean; the check exists to keep it clean.**
- no public property on `DialogConfig` without a read outside `Models/` — [05](05-dead-config-surface.md).
  *Current count: 8 dead, plus `CloseOnEscape` which is written twice and read never.*
- no second source for a decision: one reader for backdrop dismissal
  ([03](03-backdrop-dismiss-policy.md)), one for button captions ([07](07-button-hierarchy.md)),
  one severity resolver ([06](06-severity-and-headers.md)).
- no `Keys.Escape` handling outside the shell and the command palette — [02](02-escape-and-default-buttons.md).
- no colour literal in a dialog painter without a theme resolver behind it — [06](06-severity-and-headers.md).
- no control flow in `InitializeComponent`.

## Checks by stage

### Focus and accessibility — [01](01-focus-and-accessibility.md)
- role, name and description set on all eight forms, against the control group
- initial focus non-null; the input for input dialogs; **never** the destructive button
- Tab `n+1` times returns to the first control and never leaves the dialog
- focus restores to the opener on close
- `AcceptButton` and `CancelButton` set on all eight *(today: one)*
- no dialog button has an empty `AccessibleName`

### Keyboard — [02](02-escape-and-default-buttons.md)
- Escape closes when `CloseOnEscape`, and **does not** when it is false
- Enter commits; Enter in a multi-line field inserts a newline instead

### Dismissal and confirmation — [03](03-backdrop-dismiss-policy.md), [04](04-typed-confirmation.md)
- three backdrop policies produce three outcomes; nudge honours `ReducedMotion`
- destructive primary starts disabled; exact match enables; case, whitespace and prefix do not
- empty keyword with confirmation required throws at show time

### Config surface — [05](05-dead-config-surface.md)
- `ButtonLayout` produces three distinct arrangements
- verification checkbox appears, returns its state, and is absent when the text is empty
- acknowledgement gates the primary both ways
- a failing validator blocks **and** explains
- undo reverses within the grace period and disappears after it

### Visual conformance — [06](06-severity-and-headers.md)–[08](08-body-layouts-and-callouts.md)
- five severities → five distinct headers; `IconType` alone moves the header
- three header modes distinct; headerless still has a reachable close
- tinted vs plain distinct; tinted body text ≥ 4.5:1
- four button roles pairwise distinct; destructive is error-coloured in a non-error dialog
- two body layouts distinct, and centred text is measurably centred
- four icon treatments distinct
- a warning callout inside an error dialog renders in the callout's severity
- **theme switch moves every severity colour** — the check a hardcoded palette fails

### Async and overflow — [09](09-async-and-long-content.md)
- pending holds the dialog open and disables every dismissal path
- failure keeps the dialog, clears pending, shows the error, preserves input
- 5,000-character message: footer buttons inside bounds and hit-testable
- dialog never exceeds the working area

### Adaptive — [10](10-adaptive-presentation.md)
- breakpoint switches button layout and width; **nothing changes above it**
- three nested dialogs → one backdrop at single-dialog opacity
- Escape addresses the topmost only
- background does not scroll under a modal

## Deliverable

`scratchpad/DialogProbe`, printing `=== N passed, M failed ===` with each check carrying an
expectation, so a run before the fixes land reports honestly: a red marked `RedAtBaseline` is the
measurement its stage exists to move, and only an *unexpected* red sets the exit code. Plus the PNG
corpus for eyeball review against `Example_images/dialog1.png`–`dialog6.png`.

A check that has never failed is not yet a check. Break each one deliberately and confirm it goes red
before trusting it green.
