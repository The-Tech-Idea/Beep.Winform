# Stage 03 — two properties for one backdrop-dismiss decision

**Kind:** structural. Nothing misbehaves yet; two settings answer one question and can disagree.

**Status: done**, with a finding that changes what "done" means here.

`CloseOnClickOutside` is now an `[Obsolete]` projection onto `BackdropClickPolicy` — `true` ↔
`CancelDialog`, `false` ↔ `Ignore` — so the contradictory state the stage exists to eliminate
(`CloseOnClickOutside = true` with `BackdropClickPolicy = Ignore`) is **no longer representable**.
The property is kept rather than deleted because it is published and appears in saved designer state.
The reconciliation that used to sit in `ShowDialogInternal` is gone: there is nothing left to
reconcile. `Nudge` is added to the enum — the behaviour a boolean cannot express.

## Claims that did not survive

The survey said both properties were "partially wired, two readers each". Measured:

```
ShowBackdrop          → 0 readers outside DialogConfig
DialogBackdropForm    → 0 references outside its own file
BackdropClickPolicy   → 1 reader, and it only reconciled the two properties
```

**The backdrop is never displayed at all.** The form exists, `ShowBackdrop` defaults to `true`, and
nothing constructs it — so no click policy can have an effect yet, whichever property expresses it.
The stage's premise understated the problem: this was not two settings disagreeing about a behaviour,
it was two settings describing a behaviour that does not exist.

What is fixed here is the structural half — one setting, no contradictory states, `Nudge` expressible.
**Displaying the backdrop and acting on the policy is unbuilt work** and belongs with stage 10, which
owns modal presentation. It is not claimed here.

## What the survey found

```csharp
public bool CloseOnClickOutside { get; set; } = false;                                    // :288
public DialogBackdropClickPolicy BackdropClickPolicy { get; set; } = ...Ignore;           // :289
```

Adjacent lines, both describing what a click on the backdrop does. Two readers each, so both are
partially wired — which is worse than neither being wired, because the answer depends on which one
the code path happens to consult.

The combinations a caller can express include `CloseOnClickOutside = true` with
`BackdropClickPolicy = Ignore`. There is no correct behaviour for that; there is only whichever
property the code reads.

## Why the enum is the right survivor

`CloseOnClickOutside` is a boolean and the question is not boolean. The behaviours the reference
designs and current frameworks need are at least:

- **Ignore** — clicks do nothing. Correct for `dialog3.png`, a destructive confirm.
- **Close** — dismiss, returning the cancel result. The common case.
- **Nudge** — do not close, but signal that the dialog is waiting: the shake/pulse that Radix, MUI
  and macOS all use for a modal that refuses dismissal. This is the behaviour a boolean cannot
  express, and it is what makes an undismissable dialog feel deliberate rather than broken.

`DialogBackdropClickPolicy` already exists and already has somewhere to put these.

## The fix

1. `BackdropClickPolicy` is the only reader-facing setting. `DialogBackdropClickPolicy` gains
   `Nudge` if it does not already have it.
2. `CloseOnClickOutside` becomes an obsolete alias that maps onto the policy — `true` →
   `Close`, `false` → `Ignore` — so existing callers and saved designer state keep working. It is
   marked `[Obsolete]` with the replacement named, not deleted: it is a published property.
3. `DialogBackdropForm` reads the policy in exactly one place.
4. The nudge is a motion, so it goes through the motion path with the rest — and honours
   `ReducedMotion` (`DialogConfig.cs:238`), which unlike most of this config surface **is** wired,
   with 13 readers.

## Verification

1. **The three policies do three things.** Click the backdrop under each of `Ignore`, `Close`,
   `Nudge`: assert still-open-and-still, closed-with-cancel, still-open-and-moved. *Today only two
   outcomes are reachable and which one you get depends on the code path.*
2. **The alias agrees with the policy.** Set `CloseOnClickOutside = true`, assert
   `BackdropClickPolicy == Close`, and vice versa. Assert the contradictory combination is no longer
   representable.
3. **Nudge respects reduced motion.** With `ReducedMotion = true`, the nudge must not animate — but
   must still not close. A reduced-motion path that quietly starts dismissing dialogs would be worse
   than the animation.
4. **One reader.** Grep: `BackdropClickPolicy` is read in exactly one place.
