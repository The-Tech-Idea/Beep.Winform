# Stage 02 — `CloseOnEscape` cannot turn Escape off

**Kind:** defect. A preset asks for a dialog that Escape must not dismiss, and gets one that Escape
dismisses.

**Status: done.** 5 of 5 checks green. Suite total **24 passed / 3 failed, 0 unexpected** — the three
remaining reds are baseline reds owned by stages 04 and 05.

## What was built

`BeepDialogManager.ApplyEscapePolicy` clears `CancelButton` when `config.CloseOnEscape` is false.
That is the entire fix, and it works only because of a second change: **`BeepCustomDialog` and
`BeepInputDialog` intercepted Escape in their own `ProcessCmdKey` and pressed `_cancelButton`
directly**, reading straight past the property. They now route through `CancelButton`, so nulling it
is a single lever that governs the whole folder. Without that, `CancelButton` would have been null
while Escape still closed those two dialogs — the property would have looked fixed.

`ProcessCmdKey` swallows Escape when there is no cancel route, rather than forwarding to a base that
may still act on it — the "necessary but not sufficient" point in the plan, kept.

Enter now inserts a newline instead of committing while a multi-line input has focus
(`BeepInputDialog`). Committing on Enter made the second line of a multi-line box unreachable from
the keyboard.

## The harness lied first, and said so

The first version pressed Escape with `SendKeys`, which posts to the **foreground** window — and a
non-modal probe dialog on a busy desktop often is not it. The result was the most dangerous shape a
check can take:

- *"Escape is refused when CloseOnEscape is false"* — **passed**
- *"Escape still closes a dialog that allows it"* — **failed**

Neither dialog had received the key. The first green was an accident and only the second check
exposed it. `PressKey` now walks the same chain WinForms does — `ProcessCmdKey`, then
`ProcessDialogKey` — by reflection, so it does not depend on window focus.

**The control check is what caught this.** A refusal check alone would have been green and wrong.

## Verified by deliberate breakage

| break | check that went red |
|---|---|
| `ApplyEscapePolicy` made a no-op | *CloseOnEscape = false removes the Escape route*, *Escape is refused when CloseOnEscape is false* |
| a real `catch { }` added to `BeepListDialog` | *no swallowed exception in the folder* |

Both control checks — *Escape still closes a dialog that allows it* and *cancellation is still
available when Escape is off* — stayed green throughout, so the reds were specific rather than a
harness collapsing.

## Also fixed in this pass

- **The swallow detector's year-long false positive.** It reported `BeepDialogManager.Input.cs:440`,
  where there is no catch — the line is a *comment describing* a bare `catch { }` that an earlier pass
  had already removed, and the regex matched the description of the defect rather than the defect.
  The scanner now blanks comments and string literals before matching, replacing characters with
  spaces so reported line numbers still point at real source. Proven still able to catch a genuine
  swallow, above.
- **The multi-select list's horizontal scrollbar** (372px of content in a 355px client). An
  `AutoScroll` `TableLayoutPanel` sizes its percent column from the full client width and only then
  discovers it needs a vertical scrollbar, which takes that width away — so a list that only ever
  needed to scroll vertically grew a horizontal scrollbar. The panel now reserves
  `SystemInformation.VerticalScrollBarWidth` as right padding. This was stage 08's, done here because
  it was the last unexpected red.

> **Rewritten after checking.** The first draft claimed Escape does not work at all and that default
> buttons were unwired. Both were wrong — see *Claims that did not survive*. The real defect is
> narrower and still worth fixing.

## What the survey found

```csharp
public bool CloseOnEscape { get; set; } = true;      // DialogConfig.cs:296
```

Three references in the folder, and **none is a read**:

| site | what it does |
|---|---|
| `DialogConfig.cs:296` | declares it, default `true` |
| `BeepDialogManager.File.cs:262` | sets it `true` |
| `DialogConfig.cs:774` | sets it **`false`** in a preset |

Escape *does* work — every dialog form sets `CancelButton`, and WinForms routes Escape to it. That is
the mechanism, and it is a good one. But it is unconditional: because nothing reads `CloseOnEscape`,
**a caller cannot turn Escape off.**

The preset at `DialogConfig.cs:774` is a dialog whose author decided dismissal must be deliberate —
the "are you certain" case, the one you do not want closed by a stray keypress. It sets the flag,
the flag is ignored, and the dialog is dismissable anyway. The author's decision is silently
overruled, and nothing anywhere reports that.

## Claims that did not survive

- **"Escape does not close the dialogs."** False. `CancelButton` is set in all six dialog forms and
  Escape routes to it.
- **"`AcceptButton`/`CancelButton` set in one form of eight."** False — 6 of 6. Several forms carry
  comments from a previous pass recording that hand-rolled `ProcessCmdKey` overrides were *removed*
  in favour of this routing. That was the right call and this stage keeps it.

## The fix

The mechanism stays; it gains the one condition it is missing.

1. When `CloseOnEscape` is `true` (the default, so nothing changes for existing callers),
   `CancelButton` is set as it is today.
2. When `false`, `CancelButton` is left null **and** `ProcessDialogKey` swallows Escape. Both halves
   are needed: a form with no `CancelButton` can still close on Escape depending on how it is hosted,
   so leaving the property null is necessary but not sufficient.
3. The close glyph and the cancel button are unaffected — refusing *Escape* is not refusing
   *cancellation*. A dialog with no way out at all is a worse bug than the one being fixed, and
   `ShowCloseButton` (`DialogConfig.cs:321`) governs that separately.
4. The command palette keeps its own `ProcessCmdKey` (`BeepCommandPaletteDialog.cs:174-198`) — a
   palette's Escape is part of its interaction, not a dialog policy.

## Verification

1. **Escape closes by default.** For each of the six forms, open with `CloseOnEscape = true`, send
   Escape, assert closed with the cancel result. *Expected to pass today* — a regression guard on
   behaviour that already works.
2. **Escape is refused when asked.** Open with `CloseOnEscape = false`, send Escape, assert the
   dialog is **still open**. *Today this fails: the dialog closes.* This is the check the stage
   exists for.
3. **Cancellation still available when Escape is off.** With `CloseOnEscape = false`, assert the
   cancel button and the close glyph both still dismiss. The guard against fixing this into an
   inescapable dialog.
4. **The destructive preset is protected.** Build the preset at `DialogConfig.cs:774`, send Escape,
   assert it stays open — the concrete case the flag was written for.
5. **Enter still commits**, and Enter inside a multi-line field inserts a newline instead of
   committing.
