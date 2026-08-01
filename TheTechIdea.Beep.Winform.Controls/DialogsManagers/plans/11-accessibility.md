# 11 — Accessibility & Keyboard

**Priority P2. Phase 6.**

## Current behaviour

Enter and Escape **do** work on the message and question dialogs — but through a hand-rolled
override on each form rather than the framework mechanism:

```csharp
// BeepMessageDialog.cs:44
protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
{
    if (keyData == Keys.Enter)  { _okButton.PerformClick(); return true; }
    if (keyData == Keys.Escape) { DialogResult = …DialogResult.Cancel; Close(); return true; }
    return base.ProcessCmdKey(ref msg, keyData);
}
protected override void OnShown(EventArgs e) { base.OnShown(e); _okButton.Focus(); }
```

`BeepQuestionDialog` repeats the same shape. Neither sets `AcceptButton` or `CancelButton`.

Two consequences:

1. **It is duplicated per form** — each dialog re-implements the same key handling, and any dialog
   whose author forgets gets none. `BeepMultiSelectDialog` was checked and has its own copy.
2. **`AcceptButton`/`CancelButton` do more than route keys.** They give the default-button visual
   cue, participate in the framework's dialog semantics, and are what assistive technology reads to
   announce the default action. A `ProcessCmdKey` override provides none of that.

This is a moderate finding, not a missing feature — stated precisely because the first read of this
directory suggested there was no Enter/Escape handling at all, and that was wrong.

## Not yet verified

- Whether the dialogs expose a sensible accessible tree. The `BeepTabs` program found a control that
  exposed **no** accessible children at all while a complete, correct factory for them sat unused —
  so this must be measured, not assumed.
- Focus order and initial focus (only 3 focus-related calls across all forms).
- Whether focus is trapped inside the modal, and restored to the invoker on close.
- High contrast: not rendered.

## What the reference products do

- **Radix / Headless UI `Dialog`** — focus moves into the dialog on open, is trapped while it is
  open, and returns to the trigger on close. `Esc` closes. The surface is `role="dialog"` with
  `aria-modal`, labelled by its title and described by its body.
- **Material 3 / Fluent 2** — same contract, plus a documented default action.
- **WCAG 2.4.3 / 2.1.2** — focus order and no keyboard trap are conformance requirements, not
  polish. A modal that does not restore focus on close fails 2.4.3 in practice.

## Work

1. **Set `AcceptButton` and `CancelButton`** on the scaffold from [05](05-layout-and-composition.md)
   so every dialog gets the contract once, and delete the per-form `ProcessCmdKey` overrides.
2. **Measure the accessible tree** for each dialog before deciding what to build.
3. **Focus: in on open, trapped while open, restored on close.** Verify each of the three separately.
4. **Render in high contrast** — the `BeepTabs` program left this unverified because it is an OS
   setting; it needs a manual pass here too.

## Verification

- ⬜ Probe: every dialog reports a title-labelled accessible object with an appropriate role.
- ⬜ Probe: focus enters the dialog on open and returns to the invoking control on close.
- ⬜ Probe: Tab cycles within the dialog and does not escape to the owner while modal.
- ⬜ Harness: no form declares its own `ProcessCmdKey` for Enter/Escape once the scaffold owns it.

---

## Outcome

Measured before building, against a controlled baseline: a stock `Form` holding a `Label` and a
`Button`, traversed identically.

### What the measurement contradicted

The plan assumed the accessible tree needed work. It did not, and the first version of the check
said so wrongly: it walked `AccessibilityObject.GetChild`/`GetChildCount` from the form and reported
**0 descendants** — "a screen reader sees an empty dialog". The baseline reported **0 too**.
`GetChildCount()` returns -1 by default and MSAA enumerates child controls through the *window*
hierarchy, not the managed object tree, so the traversal was measuring itself. Had the baseline been
skipped, the "fix" would have been a hand-rolled `AccessibleObject` tree that no screen reader reads.

The check now measures per-control accessible names, which is what is actually announced.

### Real defects found and fixed

| Defect | Evidence | Fix |
|---|---|---|
| Dialog window had no accessible name | probe: `name=''`; baseline reported `name='Baseline'` from `Text` | `DialogHelpers.SetTitle` sets the header label and `Form.Text` together |
| Skinned caption bar drawn *and* a header row | `topChrome=72px` on a 195px dialog, painting an empty string | `ShowCaptionBar = false`; client height 195 → 172 |
| `BeepMultiSelectDialog` threw on `Show()` | `NullReferenceException` — `OnResize` → `PositionButtons` dereferenced `_buttonPanel` mid-construction | migrated to `BeepDialogShell`; the crash left with the code that caused it |
| `DialogPosition.CenterScreen` centred on the owner | no `case`; fell through to the default | `DialogPlacementEngine.Place(…, DialogPosition, cornerOffset)` |
| Corner placement never clamped to the screen | an owner at the right edge put the window off-screen | shared `ClampToWorkingArea` with the strategy path |

Already correct, left alone: role, `AcceptButton`/`CancelButton` routing (enabled by `BeepButton :
IButtonControl`), focus on open, and keyboard reachability of every action.

### Harness gap this exposed

The render harness covered 5 of 7 dialogs — only the ones already rewritten — so a form that crashed
on `Show()` was never constructed. All 7 are covered now. The out-of-bounds check also had to learn
that content below the fold of an `AutoScroll` container is not a defect *when a scrollbar exists to
reach it*; it flagged a checkbox that was reachable.
