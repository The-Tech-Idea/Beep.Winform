# Stage 07 — primary / secondary / destructive, and button shape

**Kind:** conformance to `dialog1.png`, `dialog2.png`, `dialog3.png`, `dialog4.png`, `dialog5.png`.

**Status: ◐ partial — 3 of 6 checks green, and the other 3 are blocked on shared infrastructure.**
Suite **33 passed / 6 failed**. The three reds are real and are left red rather than adjusted away.

## Done

| | note |
|---|---|
| `DialogButtonRole { Primary, Secondary, Destructive, Cancel }` | `Destructive` is a role, not "primary in red" — that separation is what lets it be visually dominant and still never take initial focus |
| `ResolvedRole` | projects the three legacy booleans (`IsPrimary`/`IsGhost`/`IsDanger`) into one answer; danger outranks primary because `DialogButton.Delete()` sets both |
| `Icon` + `IconPlacement`, `IsPending` | model support for `dialog3.png`'s trailing glyph and stage 09 |
| `DialogButtonShape { Rounded, Pill }` on `DialogConfig` | shape is a property of the dialog, not of a role |
| **One caption source** | `TypedButtons` wins outright, stated in `ResolveTypedButtons`. The question dialog's `CustomButtonLabels` property — assigned by the manager, read by nothing — is deleted |
| **Order** | cancel/secondary left of the primary action, applied centrally via `BeepDialogShell.OrderActions` rather than by each form's add order |
| **Hit targets** | 32×64 logical floor, DPI-scaled, applied to `MinimumSize` so it raises the floor without fighting the layout |

**A gap found on the way:** `BeepMessageDialog` had no `TypedButtons` property at all, so a caller
setting `DialogConfig.TypedButtons` had it honoured by the custom and question dialogs and **silently
ignored** by the message dialog. "DialogButton is the single representation" was not true of a folder
where one form never read it. Added, and the manager now feeds it.

Specs are matched to controls by id/caption, never by position: the question dialog adds No before
Yes while `ResolveTypedButtons` returns Yes first, so index pairing would have styled the wrong
control silently. Controls no spec names — the multi-select dialog's "Select All", a form utility
rather than an answer to the dialog — are left alone rather than given a default role.

## Blocked: role colours do not reach the screen

`BeepButton` delegates its fill to `BeepStyling.PaintStyleBackground`, and **that method takes no
per-control colour** — it paints from the current `BeepControlStyle` and the global theme alone. So
every dialog action renders the same Material3 blue regardless of `BackColor`, and three checks stay
red:

- *the four button roles render distinctly* — 1 distinct of 4
- *a destructive action is error-coloured inside an Information dialog*
- *pill and rounded buttons render differently* — the corner geometry is painted by the same style path

Opting individual buttons out (`UseFormStylePaint = false`, `ControlStyle = None`) was tried. It
changed the shade without making the roles differ, **and** removed those buttons from the library's
styling — a behavioural change for a result that did not work. It was reverted.

The real fix is a per-control colour override in `BeepStyling`. That is shared infrastructure used
well beyond this folder, with the same blast radius as the `ApplyThemeToSvg` tint defect the dock
program flagged, so it is a decision to take deliberately rather than a change to make quietly from a
dialog helper. **The three checks stay red until it is made** — they are measuring something true.

## Not built

Verification 4 (icon placement produces different layouts without overlapping the caption) has model
support but no check, because there is no icon-bearing button in the folder yet to measure. It lands
with the same work as stage 01's icon-only accessible-name gap.

## What the references specify

Every reference has the same two-role structure and varies only the styling:

| reference | secondary | primary |
|---|---|---|
| `dialog1.png` | "Secondary" — outlined, white fill | "Primary Action" — filled in the severity colour |
| `dialog2.png` | "No, keep it." — muted grey fill | "Yes, Delete!" — **filled red** |
| `dialog3.png` | "Cancel" — muted slate fill, pill | "Delete media 🗑" — filled red, pill, **trailing icon** |
| `dialog4.png` | — | "Got It!" — **outlined** pill, single action |
| `dialog5.png` | — | "CONTINUE" / "TRY AGAIN" — outlined pill in the severity colour |

Three things to take from this:

1. **The destructive action is the filled, high-contrast one.** `dialog2.png` and `dialog3.png` both
   put the dangerous action in saturated red and the safe one in muted grey. That is deliberate and
   it is what Apple HIG and Material both specify: make the consequential action unmistakable rather
   than hiding it. It also means "primary" and "destructive" are different roles, not the same role
   with a different colour — a destructive primary must never be the initial focus
   ([01](01-focus-and-accessibility.md)) even though it is the visually dominant control.
2. **Shape varies by design, not by role** — square-ish rounded rectangles in `dialog1.png`, full
   pills in `dialog3.png`/`dialog4.png`/`dialog5.png`.
3. **Buttons carry icons** — `dialog3.png`'s trailing trash glyph.

## What exists

`DialogButton` (`Models/DialogButton.cs`) and `TypedButtons` (`DialogConfig.cs:90`, 8 readers) are
the modern path and are wired. Alongside them:

- `Buttons` — a `BeepDialogButtons[]` (`:82`)
- `CustomButtonLabels` — a dictionary keyed by `BeepDialogButtons` (`:367`)
- `CustomButtonColors` — another dictionary keyed the same way (`:373`)
- `ButtonOrder` — a third (`:379`)
- `DefaultButton` (`:127`), `MinButtonWidth` (`:384`), `ButtonHeight` (`:389`), `ButtonSpacing` (`:394`)

So a button's caption, colour and order can each come from either the typed object or one of three
side dictionaries. That is the [03](03-backdrop-dismiss-policy.md) shape again, three times over:
when `TypedButtons` and `CustomButtonLabels` disagree about a caption, which wins is decided by
whichever the code reads.

## The fix

1. **`DialogButton` is the single representation.** It gains what the references need and it does not
   currently express: `Role { Primary, Secondary, Destructive, Cancel }`, `Icon` with
   `IconPlacement { Leading, Trailing }`, and `IsPending` for stage
   [09](09-async-and-long-content.md).
2. The legacy `Buttons` array and the three side dictionaries are **projected into `DialogButton[]`
   at one entry point**, and everything downstream reads only the typed list. They stay as inputs —
   deleting them breaks callers — but they stop being a parallel source of truth.
3. **Role decides styling, severity decides colour.** `Destructive` is filled in the error colour
   regardless of the dialog's severity — `dialog3.png` is an error dialog *and* has a destructive
   button, and they are the same red for a reason. `Primary` is filled in the dialog's severity
   colour; `Secondary` and `Cancel` are outlined or muted.
4. **`DialogButtonShape { Rounded, Pill }`**, defaulting to the theme's radius so it follows the rest
   of the library, with `Pill` for the `dialog3`/`dialog4`/`dialog5` designs.
5. Order follows platform convention — cancel/secondary to the left of primary on Windows — and
   `ButtonOrder` overrides it when set.

## Verification

1. **The four roles render distinctly.** Assert pairwise-distinct renders for Primary, Secondary,
   Destructive and Cancel at one severity. *This is the check that catches "destructive is just
   primary with another colour".*
2. **Destructive is error-coloured in a non-error dialog.** Put a destructive button in an
   `Information` dialog; assert it is the error colour, not the info colour.
3. **One source for captions.** Set a caption in `TypedButtons` and a different one in
   `CustomButtonLabels` for the same button; assert a defined winner and that the rendered text
   matches it. *Today this is undefined.*
4. **Icons place correctly.** Leading and trailing produce different layouts, and neither overlaps
   the caption. `dialog3.png`'s trailing glyph is the case to match.
5. **Shape changes.** Rounded and Pill differ; Pill's radius is half the button height.
6. **Order.** Default order puts cancel before primary; `ButtonOrder` overrides it.
7. **Hit targets.** Every button is at least 32px tall and 64px wide at 100%, and scales with DPI —
   the reference buttons are generous and a 24px button is not clickable.
