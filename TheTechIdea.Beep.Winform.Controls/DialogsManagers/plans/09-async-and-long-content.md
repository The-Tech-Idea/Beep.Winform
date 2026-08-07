# Stage 09 — pending actions and content that does not fit

**Kind:** enhancement. Two things every current dialog framework assumes and this folder has no
mechanism for.

**Status: ◐ overflow done; pending actions not started.** 6 of 6 overflow checks green; suite
**62 passed / 1 failed**.

### Sizing now has one owner

`DialogHelpers.FitFormToContent` is the single authority. `BeepDialogManager.FitToContent` **states**
bounds — max height, min width, min height — through `DialogHelpers.SetSizeBounds`, and no longer
assigns `ClientSize` in competition with it.

That conflict was the blocker behind three separate failed fixes: `MaxContentHeight` being ignored, a
message row measuring 201px after the designer conversion, and a dialog collapsing to 26px wide when
the body was made scrollable. Each fix was correct in isolation and lost to whichever authority ran
on `Load`. With ownership settled, the scroll landed unchanged from the version that had failed.

### The body scrolls

`DialogOverflow` moves the content rows into an `AutoScroll` panel occupying one row, leaving the
title and the actions in their own rows. It runs last, because callouts, the typed confirmation and
the acknowledgement each add content rows first — and only when the content actually overflows, so a
two-line confirmation is left exactly as its designer file declared it.

A scrolling dialog also takes the full height its bound allows: without a floor, the scrollable body's
small preferred height won and a 4,900-character message opened in a 221px window, making the user
scroll far more than necessary.

Measured: 4,919 characters produce a 517×514 dialog whose buttons are inside the client area and
hit-testable, with a scrolling body; `MaxContentHeight = 240` produces a 392px client.

### A units mistake in the check

The bound check compared the dialog's **outer** height against a **content** figure and failed by 12px
— the caption band `BeepiFormPro` draws inside the client area, which the config says nothing about.
It compares client height with that band named as the tolerance now. Fifth measurement error of the
session; the code was right each time.

## Pending actions

A dialog's primary action usually starts work that takes time — saving, deleting, uploading. The
convention in every current framework is that the dialog **stays open, the primary button enters a
pending state, and the dialog closes when the work resolves**. If it fails, the dialog stays open
and shows why, with the user's input intact.

This folder has no way to express that. `DialogButton` has no pending state, and the manager's
`Show…` methods are synchronous — they return a `DialogReturn` and the dialog is gone. A caller doing
async work has two options today, and both are bad:

- Close the dialog, then do the work — the user gets no feedback and, if it fails, has lost whatever
  they typed and must start over.
- Block the UI thread — the dialog freezes, Windows greys it out and offers to kill the app.

`BeepDialogManager.Progress.cs` (572 lines) has a progress dialog, but that is a *different* thing: a
separate modal reporting a long operation, not the primary action of the dialog the user is already
in.

### The fix

1. `DialogButton` gains `IsPending`, and a pending button shows a spinner in place of its icon,
   keeps its caption, and is disabled along with every other action — including the close glyph and
   Escape, because dismissing a dialog mid-commit is how records get half-written.
2. An async result callback: the button's handler may return a `Task`, and the shell holds the dialog
   open, pending, until it completes. Success closes with that button's result; failure clears the
   pending state and surfaces the error — as a callout from
   [08](08-body-layouts-and-callouts.md), which is what that element is for.
3. Cancellation, where the work supports it: the pending button becomes Cancel rather than the dialog
   staying hostage. `Progress.cs` already models cancellable work (`:232-240`) and that shape carries
   over.

## Content that does not fit

`MaxContentHeight` (`:357`) and `CustomControlMaxHeight` (`:351`) exist, so someone anticipated
overflow. What happens at the limit is unspecified: nothing scrolls.

A dialog whose content exceeds the available height must scroll its **body** while the header and
footer stay put — otherwise the buttons scroll off and the dialog becomes unusable, which is the
failure users hit on small screens and high zoom.

### The fix

1. The body becomes a scrollable region between a fixed header and a fixed footer. The buttons are
   always reachable — that is the requirement, not an optimisation.
2. Scroll appears only when needed. An always-on scrollbar in a two-line confirmation looks broken.
3. `MaxContentHeight` becomes the real bound, resolved against the working area so a dialog never
   exceeds the screen. On a 768px-tall laptop the current default of 360 plus header, footer and
   chrome is already close.
4. Keyboard scrolling works without stealing Tab from the focus trap
   ([01](01-focus-and-accessibility.md)).

## Verification

1. **Pending keeps the dialog open.** Primary handler returns a Task that takes 200ms; assert the
   dialog is still open at 100ms, the button is pending, and it closed by 300ms.
2. **Pending disables dismissal.** While pending, assert Escape does not close, the close glyph does
   not close, and the other buttons are disabled. *This is the check that protects a half-written
   record.*
3. **Failure keeps the dialog and the input.** Handler throws; assert the dialog is open, pending is
   cleared, an error callout is shown, and any text the user entered is still there.
4. **Long content scrolls, buttons stay.** A 5,000-character message: assert the footer buttons are
   within the dialog bounds and hit-testable, and that the body scrolled. *Today the buttons are
   pushed out or the content is clipped — capture which, before the change.*
5. **Short content does not scroll.** No scrollbar for a two-line message.
6. **Never taller than the screen.** With an absurd message, assert the dialog height is within the
   working area.
