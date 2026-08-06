# Stage 02 — Validation is inconsistent and has no off switch

**Kind:** bug · **Files:** `BeepListofValuesBox.cs` · **Status: done.**

Two defects that only appear when you compare the paths.

## The same rejection behaved two different ways

Typing an off-list key set `ErrorText`, raised `HasError` and showed a notification. Assigning
`SelectedKey` to the same off-list key **reverted in complete silence** — and that is the path a
data-binding caller takes, so a bound value that did not match the list vanished with nothing to
explain it.

Both paths now go through one `ApplyKey`, which accepts or refuses in one place. A refusal always sets
the inline error, always raises the new **`KeyRejected`** event, and always reverts. Typing also gets
the notification, because a person is watching.

## There was no way to allow free text

Validation was unconditional. Oracle Forms distinguishes a validated LOV from a non-validated one, and
a LOV used as a suggestion list — where free text is the point — could not be built.

**`RestrictToList`** (default `true`) is that switch. Setting it to `false` clears any error the old
rule had raised.

## Two re-entrancy bugs found while fixing this

Both were found by checks, not by reading, and both are the same shape: **assigning the key text box
runs the entire validation cycle synchronously.**

1. **The revert cleared its own error.** `RejectKey` reverts the text box, which fires `TextChanged`
   with the reverted key — usually empty, and empty is always valid, so the valid branch called
   `ClearKeyError` and wiped the error the revert had just raised. A `_reverting` guard makes the
   handler ignore its own revert.

2. **The setter clobbered the result of its own assignment.** `SelectedKey`'s setter assigned `Text`
   (which validated, and may have rejected) and then repeated the accept sequence itself, including
   `ClearKeyError`. A refused key ended up looking accepted. The setter now assigns and returns,
   letting the handler decide; it only calls `ApplyKey` directly when the text is already what was
   asked for and no event will fire.

`ValidateKey` was also made side-effect free. Starting the background lookup from inside it meant a
resolver returning an already-completed task ran its continuation **inline, before the caller had
assigned the text box**, so the "is this still the current key" guard compared against the previous
value and never rejected.

## Verification

`reject: an error is raised`, `reject: KeyRejected fires`, `reject: a later valid key clears the
error`, `free entry: an off-list key is kept`, `resolver: an unknown key is eventually refused`.

The last one failed three times across three different causes before passing — each a real bug in the
fix rather than in the check.
