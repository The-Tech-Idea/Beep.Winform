# CLAUDE.md — TheTechIdea.Beep.Winform.Controls

For AI/code agents working anywhere in this project. Folder-scoped guides (`GridX/Claude.md`,
`*/plans/`) add detail; this file sets the rules that hold everywhere.

**When docs or plans disagree with code, treat the code as authoritative.**

## Hard rules

These are not preferences. Work that breaks one of them is not finished.

### 1. Never swallow an exception — report through `BeepLog`

A bare `catch { }` or a `catch` that only discards is forbidden. Every catch must **report**.

`Diagnostics/BeepLog.cs` is the one place this library reports from. Do not write `Debug.WriteLine`
directly, and do not hand-roll a per-class "report once" helper — `BeepLog` has one.

```csharp
// Wrong. Twice: it discards, and it hides the failure from the one person who can fix it.
try { StyledImagePainter.Paint(g, rect, path); }
catch { }

// Right. Catch a specific failure, say what happened, and make it visible.
catch (Exception ex)
{
    BeepLog.FailureOnce(path, this, $"render icon '{path}'", ex);
    DrawMissingGlyph(g, rect);
}
```

| call | use |
|---|---|
| `BeepLog.Failure(source, action, ex)` | an operation failed |
| `BeepLog.Fallback(source, action, ex)` | a degraded path succeeded by other means |
| `BeepLog.FailureOnce(key, …)` / `FallbackOnce` | **inside `OnPaint` or a timer tick** — same key reports once |
| `BeepLog.Info/Warn/Error(source, action, message)` | no exception in hand |

- **`BeepLog.IsEnabled` is a display switch, not a correctness one.** Off by default in release, on in
  DEBUG. Turning it off silences the message; the surrounding code still takes its fallback path. Never
  use it to make a failure go away.
- **A host routes to its own logger** via the `BeepLog.Reported` event, optionally with
  `WriteToDebug = false`. A subscriber that throws is contained, not propagated — this is called from
  catch blocks and paint handlers.
- **Always use the `…Once` form in a paint or animation path.** A pulsing badge repaints 25×/second; a
  message per paint buries the first occurrence it exists to surface.

Three qualifications that come from real bugs in this repo:

- **Prefer letting it propagate.** An exception thrown by a consumer's event handler belongs to the
  consumer. `Control.Click` does not catch; neither should a `BadgeClick`. Catching it makes their bug
  invisible.
- **Inside `OnPaint`, catching is correct — but must still report.** An exception escaping a paint
  handler leaves the region invalid, so the next `WM_PAINT` throws again and the failure loops. Catch,
  report **once** (not once per paint — a 40ms animation repaints 25×/second), and render something
  that reads as broken.
- **Catching is not error handling if nothing throws.** `StyledImagePainter.Paint` writes a `Debug`
  line and *returns* when it cannot resolve an image. The `catch { }` wrapped around it had never
  caught anything, and removing it changed nothing. Check whether the failure you are handling is
  actually raised — `ImagePainter.HasImage` was the real question.

### 2. No stubs, no legacy paths, no shims

There is no back-compatibility burden on this library's internals. Delete the old thing rather than
keeping it beside the new one. A stub that returns a default is worse than a missing method: it
compiles, runs, and lies.

If a public member must go, remove it and record the decision in the area's `plans/` folder — do not
deprecate-and-keep.

### 3. Nothing assigns colours

Every control resolves its own colours from `BeepThemesManager`. Controls deriving from `BaseControl`
get this automatically: `BaseControl` subscribes to `ThemeChanged` and re-applies itself.

- **Never call `ApplyTheme()` on a child.** Containers that walk their children re-theming them
  re-enter theming and hang construction. This cost a full session once.
- **`IsChild = true` gives a control its parent's *colour*, not transparency** over whatever the parent
  painted. A docked opaque label will cover anything drawn behind it.
- **The one exception is colour that carries meaning** — an error, a warning, a destructive action.
  Resolve those from the theme's semantic slots (`ErrorColor`, `SuccessColor`, `WarningColor`), never
  from literal ARGB, so they follow a dark or high-contrast palette while keeping their meaning.
- A control that does not derive from `BaseControl` must subscribe to `BeepThemesManager.ThemeChanged`
  itself — and unsubscribe on dispose, because the event is static and will otherwise hold every
  instance ever created.
- **A colour the caller set explicitly must survive a theme change.** Track "explicit" separately from
  "themed" or `ApplyTheme` will stomp a deliberate choice.

### 4. Compose from Beep controls; do not hand-paint

A container is a `TableLayoutPanel`, one control per cell — not dock stacks, not flow panels, not
computed coordinates. Text is a `BeepLabel`, an action is a `BeepButton`, an icon is a `BeepImage`.

This is not a style preference. Hand-painting is what produced, repeatedly: DPI that does not scale,
literal colours, an `AccessibleName` that says `"Card"` for every one of 56 layouts, and affordances
that render but cannot be clicked or focused.

- **`BeepImage` for every icon.** It is the control that renders and themes SVGs.
- **`BeepLabel` needs both `WordWrap` and `Multiline`** — one decides where lines break, the other
  renders more than one.
- **Two grids cannot be aligned by adjusting either.** Items that must share a left edge belong in the
  same column of the same `TableLayoutPanel`.
- **Composition belongs in the designer file**, as plain `Controls.Add(control, column, row)`. Method
  calls are not designer-serialisable, and a form composed at runtime shows nothing on the design
  surface. The exception is a count that depends on data.
- **No control flow in `InitializeComponent`** — loops and conditionals there break the VS designer.

### 5. Use the frameworks that exist

- **Wizards**: embed the Wizards framework host. No hand-rolled stage machines.
- **Dialogs**: `DialogsManagers`. **Grids**: `GridX` — see `GridX/Claude.md`.
- Do not modify `BaseControl` to work around something. Use it.

## Verification

**A check must be able to fail for the reason it was written.** Before trusting any check, break the
thing it tests and watch it go red.

This repo has produced more wrong *instruments* than wrong code. Actual examples:

| the check said | the truth |
|---|---|
| 0 event subscribers, attached and detached | the reflection found nothing; it would have passed either way |
| a badge leaked after the leak was fixed | a Debug-build local stays rooted to the end of its method |
| `Custom` and `Rectangle` shapes differ | both badges were parented at `(0,0)` and overlapped |
| composed cards build 20× faster than raw controls | 500 siblings in one panel is O(n²); the baseline was wrong |
| the dialog is unthemed | the sample point landed on the form border, not the control |

Practical consequences:

- **Assert from the control tree, not from pixels.** "Does a control carry this text" cannot be
  defeated by occlusion, paint timing, or a sample point landing a few pixels off. This is most of why
  composition is worth it.
- **`Control.DrawToBitmap`, never `Graphics.CopyFromScreen`.** Screen capture returns whatever window
  is foreground — it captured a file explorer twice.
- **Guard a comparison against being blind.** Before concluding "A renders the same as B", assert that
  two known-different inputs render differently.
- **A result that flatters the change is exactly as suspect as one that condemns it.**
- Locate sample points from control geometry, and convert to screen coordinates before comparing
  bounds across different parents.

## Assigning a child control's value re-enters your own validation

A `BeepTextBox` (and any control that raises `TextChanged`) fires **synchronously** on assignment. If a
container validates in that handler, then assigning the child from a property setter runs the entire
validation cycle *before the setter's next line*.

This produced three separate bugs in one control (`Lovs/`), all found by checks rather than by reading:

- a revert fired `TextChanged` with an empty value, which was valid, which cleared the very error the
  revert had raised
- a property setter assigned the child and then repeated the accept sequence, clobbering a rejection
  the assignment had just performed
- a lookup started from inside a `Validate` method ran its continuation **inline** when the task was
  already complete — before the caller had assigned the child — so a guard comparing against the
  current value saw the previous one

The shape that works:

- **One method decides.** Typing and programmatic assignment both funnel into a single `Apply…`, so
  they cannot diverge. They *had* diverged, and the silent path was the one data binding used.
- **Validation methods are side-effect free.** No lookups, no assignments — return a verdict.
- **Guard your own reverts** with a flag the handler checks, or the revert re-enters as a user edit.
- **The setter assigns and returns.** Only do the work directly when the value is unchanged and no
  event will fire.

## Working with plans

Larger areas carry a `plans/` folder: a `00-MASTER-TRACKER.md` plus one document per stage, each
stating what it changes and what a failing run prints. Keep them current as work lands — a stage marked
done should say what was actually done and what was verified, including decisions taken and the option
not taken.

Record what was **not** verified as explicitly as what was. "Checked and fine" and "not checked" must
never look the same to the next reader.
