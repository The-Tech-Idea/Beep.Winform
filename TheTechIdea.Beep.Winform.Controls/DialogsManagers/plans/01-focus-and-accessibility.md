# Stage 01 — the dialogs are invisible to assistive technology

**Kind:** defect. A screen reader announces a window with no name, role or description.

**Status: done.** 10 of 10 checks green, from 3 red and 2 absent at baseline. Suite total moved from
11 passed / 9 failed to **18 passed / 6 failed**, with no new unexpected failures.

## What was built

| | where | note |
|---|---|---|
| role + accessible name | `DialogHelpers.SetTitle` | **not** `CreateDialog` — see below |
| accessible description | `DialogHelpers.SetMessage` (new) | five `Message` setters now route through it |
| initial focus, question | `BeepQuestionDialog.OnShown` | the **safe** action, `_noButton` |
| initial focus, custom | `BeepCustomDialog.OnShown` | hosted control first, primary action as fallback |
| action names | `DialogHelpers.DescribeActions` | called from each `OnShown` |

**The fix does not live where this plan said it should.** Step 1 specified
`BeepDialogManager.Core.cs CreateDialog`, "so all six forms inherit it rather than six copies". That
is wrong: `CreateDialog` builds only the message, question and custom forms. The input, list and
multi-select dialogs are constructed in `BeepDialogManager.Input.cs` (`:112`, `:130`, `:162`) and
never pass through it — a fix applied there would have covered three of six while looking complete.
`DialogHelpers.SetTitle` is the real single choke point, because every form's `Title` setter already
calls it on every construction path. The harness now exercises the direct-construction path
separately for exactly this reason.

`DescribeActions` runs from `OnShown` rather than the constructor: `TypedButtons` rewrites button
captions after construction, so a name captured in the constructor would have been the designer's
placeholder rather than the caption actually shown.

## Verified by deliberate breakage

Each new check was made to fail for the reason it was written, in one run with three breaks applied:

| break | check that went red |
|---|---|
| description assignment suppressed | *dialogs are described by their message*, *directly-constructed dialogs carry role, name and description* |
| `_yesButton.Focus()` on the confirm dialog | *a confirm dialog does not open focused on the committing action* |
| action naming suppressed | *every action button reports an accessible name* |

Reverted, and all ten are green again. Without this the three new checks would have been green from
birth, which says nothing.

## Left for later

`DescribeActions` copies a button's caption into its `AccessibleName`, which helps only a button that
already has a caption. The icon-only case this stage's step 3 was written for — `dialog3.png`'s trash
glyph — has no caption to copy, and no such button exists in the folder yet. When stage 07 or 12
introduces one, it needs a name from somewhere other than `Text`.

> **This stage was rewritten after checking its own claims.** The first draft asserted that focus is
> never restored, that Tab escapes the dialog, and that `AcceptButton`/`CancelButton` were set in one
> form of eight. All three were wrong, and all three came from greps truncated by `head`. What was
> left after checking is smaller and sharper. The original scope is recorded under *Claims that did
> not survive* so the correction is visible rather than quietly dropped.

## What is actually missing

**Accessibility metadata: zero occurrences in the folder.**

```
AccessibleRole | AccessibleName | AccessibleDescription   →  0 hits across 7,797 lines
CreateAccessibilityInstance                               →  0 hits
```

So every dialog reports the WinForms default role and an empty name. A screen reader user hears that
a window appeared and nothing about what it wants. For a system whose whole purpose is to ask the
user a question, that is the defect.

**Initial focus: set in four of six dialog forms.**

| form | initial focus |
|---|---|
| `BeepInputDialog.cs:110` | `_inputBox.Focus()` + `SelectAll()` |
| `BeepListDialog.cs:79` | `_comboBox.Focus()` |
| `BeepMessageDialog.cs:77` | `_okButton.Focus()` |
| `BeepMultiSelectDialog.cs:127` | first checkbox |
| `BeepCustomDialog` | **none** |
| `BeepQuestionDialog` | **none** |

`BeepQuestionDialog` is the one that matters: it is the confirm dialog, so it is the one most likely
to carry a destructive action, and it is one of the two that does not decide where focus lands.

## Claims that did not survive

Recorded because the plan asserted them and a harness built on them would have encoded them:

- **"Focus is never restored."** False. `BeepDialogManager.Core.cs:355-361` captures
  `owner?.ActiveControl` before showing and restores it after, falling back to the owner.
- **"`AcceptButton`/`CancelButton` set in one form of eight."** False — **6 of 6** real dialog forms
  set both. The other three files are a `TableLayoutPanel`, a modeless dialog and the backdrop, none
  of which should. Several carry comments from a previous pass: *"ProcessCmdKey override removed:
  AcceptButton and CancelButton route Enter and Escape."*
- **"Tab walks out of the dialog."** Unverified, and probably false: `ShowDialog` runs a modal loop
  with the owner disabled, and WinForms already cycles Tab within the dialog's own controls. The
  web's focus-trap problem exists because DOM focus can leave the dialog element; a modal WinForms
  form is a different containment model. **Stage [11](11-verification.md) measures this rather than
  assuming it either way** — if Tab is already contained, there is nothing to build.

## The fix

1. `AccessibleRole = AccessibleRole.Dialog`, `AccessibleName` from `config.Title`,
   `AccessibleDescription` from `config.Message`, set once where dialogs are constructed
   (`BeepDialogManager.Core.cs CreateDialog`) so all six forms inherit it rather than six copies.
2. Initial focus in `BeepCustomDialog` and `BeepQuestionDialog`, following what the other four
   already do. For `BeepQuestionDialog` the target is the **safe** action — a confirm dialog that
   opens with the destructive button focused turns a stray Enter into data loss, which is the risk
   `dialog2.png` and `dialog3.png` are drawn to slow down.
3. Every dialog button gets an `AccessibleName`. A button captioned only by an icon
   (`dialog3.png`'s trash glyph) reads as nothing otherwise.
4. Only if stage 11 shows Tab escaping: contain it. Not before.

## Verification

1. **Control group first.** A stock `Form` with a `Label` and two `Button`s. Every accessibility
   assertion below runs against it first and must **not** produce the expected result — otherwise the
   check is measuring the window hierarchy rather than the implementation, which is how an
   accessibility check passes against a control that has none.
2. **Role, name, description.** For each of the six forms assert role is `Dialog`, name equals the
   title, description equals the message. *Today: default role, both strings empty — 0 of 6.*
3. **Initial focus is set.** `ActiveControl` non-null on show for all six. *Today: 4 of 6.*
4. **Focus does not start on a destructive action.** For a confirm dialog, assert the focused control
   is not the destructive button.
5. **Tab containment — measured, not assumed.** Send Tab `n+1` times; record whether focus ever
   leaves the dialog. This check exists to settle the question; if it passes today it stays as a
   regression guard and the stage does no work here.
6. **Focus restore still works.** Focus a known owner control, open, close, assert focus returned.
   *Expected to pass today* — it is a guard on `Core.cs:355-361`, not a fix.
7. **Every button named.** No dialog button with an empty `AccessibleName`.
