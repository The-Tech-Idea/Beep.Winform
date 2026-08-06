# BeepDialogManager — enhancement and fix program

Master tracker for `TheTechIdea.Beep.Winform.Controls/DialogsManagers/`.
36 C# files, 7,797 lines, 8 dialog forms, 1 command palette, 6 helpers.

Reference designs: `Example_images/dialog1.png` … `dialog6.png`, plus `actualdialog.png`, which is a
capture of this folder's own output and is a *defect record*, not a target.

Read together the six reference images are **four distinct presentations** of the same content — a
tinted title bar, a centred confirmation, a hero band, and a flat oversized mark — not one design with
variations. [Stage 12](12-presentation-styles.md) covers them; stages 06–08 take individual
attributes (severity tint, button hierarchy, icon treatment) out of the same images.

## What the survey found

This folder has already had an exception-policy pass, and it shows: `DialogStateStore.cs:82-95` and
`BeepDialogManager.Input.cs:439-449` carry narrowed, reported catches with comments explaining what
the previous bare `catch { }` concealed. **There are no swallowed exceptions left.** That is the one
standing constraint this folder already satisfies, and the stages below do not need to revisit it.

`DialogConfig` is unusually ambitious — 90+ properties covering severity, backdrop policy, motion
profiles, typed confirmation, undo, field validation and remembered geometry. The presets cite Linear
and Vercel by name (`DialogConfig.cs:649`). The intent is a modern dialog system.

The defects are almost entirely one shape: **the configuration surface describes a dialog system the
code does not implement.** Measured, counting readers outside `Models/`:

| property | readers | consequence |
|---|---|---|
| `RequireTypedConfirmation`, `ConfirmationKeyword` | **0** | `CreateDestructive` sets it true (`:667`); nothing asks the user to type anything |
| `VerificationText`, `VerificationChecked` | **0** | no "don't show this again" exists |
| `ButtonLayout` | **0** | `DialogHelpers` has Horizontal/Vertical/Grid code that is never given the config's value |
| `FieldValidators`, `ValidationState` | **0** | per-field validation is declared only |
| `EnableUndoForDestructiveActions` | **0** | defaults to `true` and does nothing |
| `DisablePrimaryUntilAcknowledged` | **0** | |
| `MotionProfile`, `StaggerDelay` | **0** | |
| `CloseOnEscape` | **written twice, read never** | a preset sets it `false` (`:774`) and Escape ignores it |

And one accessibility number that decides the ordering: **zero** occurrences of `AccessibleRole`,
`AccessibleName` or `AccessibleDescription` across 7,797 lines, and no `CreateAccessibilityInstance`.
Every dialog reports the WinForms default role and an empty name, so a screen reader announces a
window that will not say what it wants.

> **Stages 01 and 02 were rewritten after their claims were checked.** The first drafts also asserted
> that focus is never restored, that Escape does not work, and that `AcceptButton`/`CancelButton` were
> set in one form of eight. All three were wrong — restore exists at `Core.cs:355-361`, Escape routes
> through `CancelButton`, and **6 of 6** real dialog forms set both. Those claims came from greps
> truncated by `head`. The corrected stages are smaller and are worth more for it; each records what
> did not survive. Keyboard handling in this folder has already had a pass, and the comments in the
> forms say so.

## Severity ordering

Stage 01 is the one a user cannot work around. Stages 02–05 are the config surface lying about what
it does. Stages 06–08 are conformance to the reference designs. Stages 09–10 are patterns the
references and current frameworks assume and this folder has no mechanism for. Stage 11 is the
harness the rest report through.

| # | Stage | Kind | Status |
|---|---|---|---|
| [01](01-focus-and-accessibility.md) | The dialogs are invisible to assistive technology | **defect** | ☑ done |
| [02](02-escape-and-default-buttons.md) | `CloseOnEscape` cannot turn Escape off | **defect** | ☑ done |
| [03](03-backdrop-dismiss-policy.md) | Two properties for one backdrop-dismiss decision | structural | ☑ done |
| [04](04-typed-confirmation.md) | The destructive preset promises a confirmation that does not exist | **defect** | ☑ done |
| [05](05-dead-config-surface.md) | Seven more properties with no mechanism | structural | ◐ partial |
| [06](06-severity-and-headers.md) | Severity resolver (appearance moved to the theme) | conformance | ☑ done |
| [07](07-button-hierarchy.md) | Button roles, order and hit targets (colour is the theme's) | conformance | ☑ done |
| [08](08-body-layouts-and-callouts.md) | Inset callouts built; icon treatments are control work | conformance | ◐ partial |
| [09](09-async-and-long-content.md) | Pending actions and content that does not fit | enhancement | ☐ open |
| [10](10-adaptive-presentation.md) | Narrow windows, stacking, scroll lock | enhancement | ☐ open |
| [11](11-verification.md) | A probe harness and the first tests this folder has | verification | ☑ done |
| [12](12-presentation-styles.md) | Four presentations — arrangement built, colour is the theme's | conformance | ◐ partial |

Status marks: ☐ open · ◐ in progress · ☑ done

**Suite: 56 passed / 1 failed, 0 unexpected.** The three remaining reds are baseline reds owned by
stages 04 and 05. Both previously-unexpected failures are fixed: the swallow detector's false
positive on a comment (it now blanks comments and strings before matching, and is proven still able
to catch a real `catch { }`), and the multi-select horizontal scrollbar (the `AutoScroll` panel now
reserves the vertical scrollbar's gutter, so its percent column is sized to a width that survives the
scrollbar appearing).

> **A second lesson, from stages 02 and 06: suspect the harness before the product.** Both stages lost
> runs to a hang that was blamed on library code — `SendKeys` posting to whatever window had focus,
> and a modal `_manager.Show` that nothing closed. Stage 06 cost three bisection runs against the
> library before the harness turned out to be at fault. A check that needs focus, a keystroke, or a
> modal to return is a check that can hang or pass for the wrong reason; drive the code path directly
> instead. The stage 06 "neutral" fixture is the same failure in miniature — `IconType` defaults to
> `Information`, so the fixture was never neutral and the check was describing itself.
>
> **A lesson from stage 01 that applies to every stage below.** The plan said to apply the fix in
> `BeepDialogManager.CreateDialog`, "so all six forms inherit it". `CreateDialog` builds three of the
> six; the input, list and multi-select dialogs are constructed in `BeepDialogManager.Input.cs` and
> never reach it. **There are four construction paths in this folder, not one.** Any stage that says
> "apply it once where dialogs are constructed" must name which path it means, and its harness must
> exercise the direct-construction path separately — otherwise half the folder is untouched and the
> checks still go green.


## Appearance belongs to the theme — settled

Nothing in this folder colours or paints. Every Beep control derives from `BaseControl`, which
subscribes to `BeepThemesManager.ThemeChanged` and re-applies the theme unasked, so a manager that
also assigns colours is competing with the control and loses: the assignment is overwritten by the
control's own `ApplyTheme`, or covered by a control painted on top of it.

Removed on that basis: the severity surface tint, the header band (tried twice — a `TableLayoutPanel`
subclass painting in `OnPaintBackground`, then the panel's own `CellPaint`), and per-role button
colours for primary, secondary and cancel. Four further attempts to make an assignment stick —
`UseFormStylePaint`, `ControlStyle = None`, `IsColorFromTheme`, `UseThemeColors`, and a re-style on
`Shown` — are also gone. None of them could have worked.

**The one exception:** a destructive action takes the theme's `DialogErrorButtonBackColor`. That
colour carries a warning rather than a style, which is the case worth making an exception for.

**What this means for the reference designs.** `dialog1.png`'s tinted header strip, `dialog4.png`'s
saturated colour block and the severity-coloured primary buttons are all *theme* work, not dialog
work. Making them appear means giving the theme the keys — several already exist and nothing reads
them (`DialogErrorButtonBackColor`, `DialogWarningButtonBackColor`, `DialogInformationButtonBackColor`
and siblings) — and having the controls resolve them. [Stage 12](12-presentation-styles.md) should be
read that way: it describes presentations the theme has to supply.

## The designer files are now the source of truth for layout

Composition moved out of runtime code and into each form's `InitializeComponent`, as plain
`Controls.Add(control, column, row)` statements. `BeepDialogShell` — the `TableLayoutPanel` subclass
that used to own the structure behind `SetIcon`/`SetTitle`/`AddContent`/`AddAction` — **is deleted**.
The dialogs use a plain `TableLayoutPanel`; the severity band is painted through that control's own
`CellPaint` event, which needs no subclass.

The reason is concrete: the design surface renders what `InitializeComponent` parents, and it parented
only the shell. Every dialog opened blank in Visual Studio. Method calls are not
designer-serialisable, so composition has to be plain `Controls.Add` statements for the controls to
be visible and editable there.

**Visual Studio has since regenerated three of the six designer files** (`BeepMessageDialog`,
`BeepInputDialog`, `BeepQuestionDialog` — roughly 900–1,600 lines added each), serialising an explicit
`Size` and `Location` for every control. That is the designer working as intended, and it changes who
owns sizing: `_messageLabel.Size = new Size(427, 201)` now comes from the design surface, which is why
the message dialog renders 513×320 rather than the 432×174 it did when runtime code sized it.

**Consequence for the stages below.** Runtime sizing — `FitToContent`, and the `BeepLabel`
`MinimumSize` work recorded under the layout pass — now competes with sizes the designer serialises.
Before any further visual stage, decide which wins: the designer's serialised geometry, or the
content-driven sizing. The probe's pixel checks sample by proportion of the window and will need to
follow that decision either way; three stage 06 checks are red for exactly this reason and are
measuring the size change, not a colour defect.

## Completed out of band — the layout and alignment pass

Done ahead of the numbered stages, because the misalignment was visible in every screenshot and
stages 06–08 all build on top of the arrangement it fixes. Recorded here so the stages below are
read against the current code and not the code the survey described.

**`BeepDialogShell` is now a single `TableLayoutPanel`.** It was a shell containing a header panel
and a body panel — three grids. The header carried an icon column and the body did not, so their
content columns began at different offsets and every dialog rendered its title indented further right
than the message beneath it (`Example_images/actualdialog.png` is that defect, captured from a build).
Nested grids *can* be aligned, by giving the body a matching gutter; a single grid makes the shared
edge structural instead, so there is one column definition rather than the same width maintained in
two places across six dialogs. The shell is now an icon gutter column the icon spans, a text column
holding the title and every content row, and an action row spanning both.

Composition moved with it: `SetHeader`/`SetBody` became `SetIcon`/`SetTitle`/`AddContent`, one row per
control, so the nested panels are gone from all six forms.

**Two `BeepLabel` sizing defects, both of which made a container unable to lay the label out.**

- `UpdateMinimumSize` measured the wrapped height against whatever width the label happened to have —
  which, running on `Text` assignment, is long before a container has assigned one — and froze the
  result in `MinimumSize`. A long message came out 112px tall inside a 61px cell, could not shrink,
  and overflowed onto the control below it. It now measures against the assigned width when there is
  one and falls back to a single line when there is not.
- The same method assigned `Height` outright, and `OnResize` assigned it again. `MinimumSize` is
  already enforced by `SetBoundsCore`, so both were redundant — and they ran *during* layout, so the
  label resized itself while its container was deciding what size to give it.

**`ApplyColorProfile` now respects `IsChild`.** It ran at the end of `ApplyTheme` and overwrote the
parent colour the `IsChild` branch had set four lines earlier, so a docked label painted the profile's
own background as a filled block across its cell.

**Measured result** (stage 11 harness): the three layout checks that were red — a control overflowing
its cell, sibling overlap, and truncated text — are green. Two remain, and both are recorded below.

### Still open from this pass

- **The option list scrolls horizontally** — `TableLayoutPanel content 372px in 355px` in the
  multi-select fixture. A checkbox is wider than the column it sits in. Belongs to stage 08.
- **Label surface colour.** The theme's `BackColor` is 248,250,255 under ModernTheme while the form
  paints 255,255,255, so each label reads as a faint block against the form. This is a disagreement
  between two theme values, not a layout fault, and it should be settled in the theme — stage 06 owns
  the severity/surface resolver and should own this with it. Do not work around it in the shell: the
  attempts to do so are what produced the hang described below.

### Resolved during the pass — do not re-introduce

**Dialog construction hung, and manual theme propagation was the cause.** Every Beep control derives
from `BaseControl`, which subscribes to `BeepThemesManager.ThemeChanged` and re-applies the theme by
itself (`BaseControl.cs:589-617`). Containers therefore must **not** push the theme down. The shell
had grown a tree-walking `ApplySurface` that called `ApplyTheme()` on each label by hand, invoked from
`OnHandleCreated` and from every composition call, plus a `_shell.ApplyTheme(themeName)` in all six
dialogs — all of it duplicating work the controls already do, and re-entering theming that was already
running. The probe hung deterministically after stage 01 and never reached stages 02, 04 or the layout
audit.

Bisection cost three builds and runs and found nothing, because each suspect was a symptom rather than
the cause: `FitToContent`'s `GetPreferredSize` (disabled — still hung), the shell's `OnHandleCreated`
(removed — still hung), `ApplyColorProfile`'s `IsChild` guard (reverted — still hung). Removing the
manual theming wholesale fixed it, and the suite now runs to completion: **11 passed, 9 failed, 2
unexpected.**

What the shell keeps is one handler for its own `BackColor`, because it is a plain `TableLayoutPanel`
rather than a `BaseControl` and nothing themes it. Its contents need nothing — `BackColor` is ambient,
so nested plain panels inherit it, and the Beep controls resolve their own.

> **The rule this establishes:** to restyle Beep controls, change the theme in `BeepThemesManager` and
> let them react. A container that calls `ApplyTheme()` on its children is not helping; it is racing
> the mechanism that already does it.

## Order of work

1. **[11](11-verification.md) baseline capture runs first.** Every conformance stage is "did this
   render change, and was the change intended" — without a corpus there is nothing to compare
   against. Capture before stage 01 touches anything.
2. **01 → 02.** Both touch how a dialog is constructed and how its buttons are wired; doing them
   separately means touching the same code twice.
3. **06 before 07 and 08.** Header tint, button colour and callout accent all resolve from one
   severity source. Building the severity resolver first means the other two consume it rather than
   each inventing a palette.
4. **04 after 07.** Typed confirmation gates the primary button, so the button states have to exist
   before something can disable them.
5. **10 last.** Adaptive presentation changes layout for every dialog; it wants the layouts settled.

## What "latest UI/UX" means here

The reference images are the visual contract. These are the behavioural conventions they assume,
drawn from current web frameworks and shipping products — each is cited where the stage uses it:

- **WAI-ARIA APG dialog pattern** — focus trap, initial focus, focus restore, `aria-modal`,
  labelled by title, described by body, Escape to dismiss. Stage 01.
- **Radix / shadcn Dialog, MUI, Fluent** — `onOpenChange` semantics, controlled dismissal, backdrop
  click as a *policy* rather than a boolean, scroll lock. Stages 02, 03, 10.
- **GitHub / Vercel / Linear destructive confirmation** — type the resource name to enable the
  destructive action. Stage 04. `DialogConfig` already names Linear and Vercel.
- **Material 3 dialogs** — 28dp radius, 24dp padding, tonal surface, icon optional and centred when
  present. Stages 06, 08.
- **Apple HIG** — at most two or three actions, destructive action visually distinct, cancel always
  available. Stage 07.
- **Sonner / react-hot-toast** — the undo affordance after a destructive action. Stage 05.
- **Async form submission** — the primary button enters a pending state and the dialog stays open
  until the work resolves. Stage 09.

## Standing constraints

Carried from the docking, grid and dock programs, and from `CLAUDE.md`:

- No legacy paths, no stubs, no shims. Production-ready code or nothing.
- No duplication or redundancy — stages 03 and 05 exist because of it.
- Never swallow an exception. **This folder is already clean; keep it that way.**
- Do not modify `BaseControl` or `BeepiForm`. Use them.
- Resolve themes through `BeepThemesManager`.
- No control flow in `InitializeComponent`.
- `master` branch only.

## The rule every stage is verified against

Earned across the previous programs: **a check must be able to fail for the reason it was written.**
Every stage states the baseline it measures against and what a failing run prints *today*.

Two traps this folder will hit specifically, both of which have already cost time elsewhere:

- **Accessibility traversal measures itself.** A stock `Form` reports `-1` children and MSAA walks the
  window hierarchy, so a check that counts "some" accessible children proves nothing. Stage 01 states
  its control group.
- **Rendering to a bitmap validates the wrong mechanism.** DPI and tinting both looked correct in an
  offscreen harness and were wrong on screen. Stage 11 renders dialogs as shown windows, not as
  `DrawToBitmap` of an unparented control.

Deletion plus a clean compile is authoritative for deadness. Grep is not.
