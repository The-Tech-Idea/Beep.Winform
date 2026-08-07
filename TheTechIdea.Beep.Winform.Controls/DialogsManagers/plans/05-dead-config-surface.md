# Stage 05 — seven more properties with no mechanism

**Kind:** structural. The configuration surface describes capabilities the code does not have.

**Status: ◐ partial — the three cheap builds are done; two heavy ones and a deletion decision remain.**
Suite **51 passed / 1 failed**; the census below is the remaining red, and it is now a smaller number
rather than an untrue one: **43 of 89 unread**, from 48.

### Built

| property | what it does now |
|---|---|
| `ButtonLayout` | reshapes the footer — `Vertical` stacks the actions in one column, `Grid` wraps them. Measured: vertical gives one column and a single x-coordinate for every action |
| `VerificationText` / `VerificationChecked` | renders the checkbox, and **`DialogReturn.WasVerificationChecked` finally carries a value** — it had existed for a state nothing produced |
| `DisablePrimaryUntilAcknowledged` | gates the primary on that checkbox, re-evaluated on every change so unchecking disables it again |

`ButtonLayout` was the instructive one, as the plan predicted: the arrangement code existed and was
complete, and nothing ever passed the config value to it. A wire, not a feature. It is wired to the
footer's own column and row counts now rather than to the older rectangle helper, because the footer
is a `TableLayoutPanel` declared in the designer file.

An acknowledgement gate with no checkbox would disable the primary action permanently, so asking for
the gate now implies the checkbox — with "I understand" as the caption when no text is given.

### Not built, and not claimed

- `FieldValidators` / `ValidationState` — per-field validation. The largest item here and untouched.
- `EnableUndoForDestructiveActions` — still defaults to **`true`** while doing nothing, which remains
  the most misleading default in the folder.
- `MotionProfile` / `StaggerDelay` — **deleted**, on the user's decision. `Animation`,
  `AnimationEasing`, `AnimationDuration` and `ReducedMotion` already cover motion and `ReducedMotion`
  is genuinely wired with 13 readers; a second motion surface with zero readers was the duplication
  this program keeps removing. `DialogManagerOptions.MotionProfile` and the `DialogMotionProfile`
  type are untouched — those are a different, live setting.

### A measurement note

The `ButtonLayout` check first reported a correct vertical stack as a failure: it located "the panel
containing a button" and matched the **shell**, because the question dialog's details toggle is a
`BeepButton` sitting directly in the shell's grid. It excludes the shell now. Third time this session
that a check measured the wrong control and reported working code as broken.

## The group

Measured by counting readers outside `Models/`. All seven are **zero**.

| property | declared | what it promises |
|---|---|---|
| `VerificationText`, `VerificationChecked` | `DialogConfig.cs:98`, `:104` | the "don't show this again" checkbox |
| `ButtonLayout` | `:132` | Horizontal / Vertical / Grid button arrangement |
| `FieldValidators`, `ValidationState` | `:438`, `:439` | per-field validation with messages |
| `EnableUndoForDestructiveActions` | `:444` | undo after a destructive action — **defaults to `true`** |
| `DisablePrimaryUntilAcknowledged` | `:311` | primary disabled until the user acknowledges |
| `MotionProfile` | `:237` | a named motion profile |
| `StaggerDelay` | `:236` | staggered element entrance |

`EnableUndoForDestructiveActions` is the one to look at twice: it defaults to **`true`**, so every
caller currently believes destructive actions in this system are undoable. They are not.

### `ButtonLayout` is the instructive one

The layout code exists and is complete:

```csharp
// Helpers/DialogHelpers.cs:133
public static ... ArrangeButtons(..., DialogButtonLayout layout, int buttonWidth, ...)
    case DialogButtonLayout.Horizontal:   // :142
    case DialogButtonLayout.Vertical:     // :146
    case DialogButtonLayout.Grid:         // :150
```

`DialogHelpers.CalculateButtonAreaSize` (`:281`) handles all three too. Nothing ever passes
`config.ButtonLayout` to either. This is not a missing feature — it is a **wire that was never
connected**, and it is the cheapest item in this stage.

Vertical button stacks are not decoration: they are what a narrow dialog needs, and stage
[10](10-adaptive-presentation.md) depends on this working.

## The decision each one needs

The rule from the previous programs: **a published property either does something or does not
exist.** Deleting published properties breaks consumers, so each of these is build-or-delete and the
answer is not the same for all seven.

**Build — these are cheap and the machinery exists:**

- `ButtonLayout` — pass it to the helper that already implements it.
- `VerificationText` / `VerificationChecked` — a checkbox in the footer whose state comes back on the
  result. The standard "don't show this again", and the reason it belongs to the dialog rather than
  the caller is that the caller cannot add a control to a dialog it did not lay out.
- `DisablePrimaryUntilAcknowledged` — the same gate stage [04](04-typed-confirmation.md) builds for
  typed confirmation, driven by a checkbox instead of a text match. Build it after 04 and reuse it.

**Build, but as real features with their own weight:**

- `FieldValidators` / `ValidationState` — per-field validation with inline messages. The dictionary
  shape is already right. This is the largest item here.
- `EnableUndoForDestructiveActions` — there is a hook already
  (`BeepDialogManager.Core.cs:941 ConfirmDestructiveWithUndo`), so the mechanism is half-present. The
  pattern is Sonner's: perform the action, show a toast with Undo for a grace period, and only commit
  after it expires. That is a notification concern as much as a dialog one and touches
  `BeepDialogManager.Notifications.cs`.

**Delete unless someone wants them:**

- `MotionProfile` and `StaggerDelay`. `Animation`, `AnimationEasing`, `AnimationDuration` and
  `ReducedMotion` already cover motion, and `ReducedMotion` is genuinely wired with 13 readers. A
  second motion-configuration surface with zero readers is the duplication this program keeps
  removing. Deleting them is a published-API break, so it needs a decision — but the default answer
  should be deletion, not implementation.

## Verification

1. **`ButtonLayout` changes the arrangement.** Render three buttons under Horizontal, Vertical and
   Grid; assert the three button rectangles differ pairwise, and that Vertical stacks them in one
   column. *Today all three produce the same row.*
2. **`VerificationText` appears and returns.** With text set, assert a checkbox with that caption is
   present; check it; assert the result reports it checked. Assert the checkbox is **absent** when
   the text is empty — a stray empty checkbox is worse than none.
3. **Acknowledgement gates the primary.** With `DisablePrimaryUntilAcknowledged`, assert primary
   disabled, then enabled after checking, then disabled again after unchecking.
4. **Field validation blocks and explains.** A failing validator must both prevent the primary
   action *and* display its message. Assert both — a validator that blocks silently is a dialog the
   user cannot get out of.
5. **Undo actually undoes.** Perform a destructive action with undo enabled, invoke undo within the
   grace period, assert the action was reversed. Then let the period expire and assert the undo
   affordance is gone.
6. **No dead published property.** For every public property on `DialogConfig`, at least one read
   outside `Models/` and outside the property itself. Mechanical, and it is what would have caught
   all seven of these plus `CloseOnEscape`.
