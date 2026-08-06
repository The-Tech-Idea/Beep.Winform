# Stage 04 — the destructive preset promises a confirmation that does not exist

**Kind:** defect. A safety mechanism is configured, documented, and absent.

**Status: done.** 7 of 7 checks green; suite **37 passed / 1 failed, 0 unexpected**.

`Helpers/TypedConfirmation.cs` inserts a prompt and a `BeepTextBox` between the body and the actions
when `RequireTypedConfirmation` is set, and gates the primary action behind an **ordinal** comparison
— no trimming, no case folding. The field is never pre-filled. An empty `ConfirmationKeyword` with the
flag set throws at construction rather than rendering a dialog whose primary action can never be
enabled.

Verified by breaking it: removing the gate reddened *starts disabled*, and relaxing the comparison to
`OrdinalIgnoreCase` + `Trim()` reddened *only the exact keyword* — which is the check that exists
because a sloppy comparison passes the happy path and defeats the point of the feature.

**A correction worth recording.** That break run appeared to show the focus handler was redundant, so
it was removed as dead code — and the guarding check went red. The break had left the handler
subscribed and removed only the `Focus()` call, so focus arrived by another route and the conclusion
was wrong. The handler is load-bearing and is back, with a comment saying so. Deleting code because
one experiment left a check green is the same error as trusting a green check that never failed.

## What the survey found

```csharp
public bool RequireTypedConfirmation { get; set; } = false;   // DialogConfig.cs:301
public string ConfirmationKeyword { get; set; } = "";         // DialogConfig.cs:306
```

**Zero readers.** And the preset that exists to protect destructive actions turns it on:

```csharp
// DialogConfig.cs:649 — "Destructive-action confirmation (modelled on Linear / Vercel patterns)."
public static DialogConfig CreateDestructive(string title, string message)
{
    ...
    RequireTypedConfirmation = true,       // :667
    Preset = DialogPreset.DestructiveConfirm,
}
```

So `CreateDestructive` names Linear and Vercel, sets the flag those products' patterns are built on,
and produces a dialog where the destructive button is live from the moment it opens. The caller has
asked for the strongest confirmation the API offers and received the weakest.

This is the most serious of the dead-property findings and is separated from
[05](05-dead-config-surface.md) for that reason: the others make a dialog less capable, this one
makes a dialog less safe than its author believed.

## What the pattern actually is

GitHub, Vercel, Linear, Stripe and AWS all use the same shape, and `dialog3.png` is a picture of the
lighter version of it:

- The dialog names the exact resource — `dialog3.png` shows `"rainy_day.jpeg"` quoted and bolded in
  the body.
- It states the irreversible consequence — *"You can't undo this action."*
- It states the **collateral** consequence — the callout in `dialog3.png`: *"By deleting this media
  **8 connected hotspots** will also be deleted."*
- For the heavier version, the destructive button stays disabled until the user types the resource
  name exactly.

The typed step exists to defeat muscle memory. Its whole value is that the user cannot complete it
without reading — so the field must not be pre-filled, must not accept a paste of the label from the
dialog's own text without the user having selected it deliberately, and must match exactly.

## The fix

1. When `RequireTypedConfirmation` is set, the shell inserts a confirmation field between the body
   and the buttons: a label naming what must be typed, and a single-line input.
2. The primary action is **disabled until the input equals `ConfirmationKeyword` exactly** —
   ordinal comparison, no trimming, no case folding. A confirmation that accepts "DELETE" for
   "delete" is a confirmation the user can complete without reading.
3. `ConfirmationKeyword` empty while `RequireTypedConfirmation` is true is a configuration error and
   throws at show time. Silently treating it as "no confirmation needed" is how this class of bug
   returns.
4. The field is the initial focus target for these dialogs, which fits stage
   [01](01-focus-and-accessibility.md)'s rule that focus never lands on the destructive action.
5. Escape must still cancel — refusing to *confirm* is not refusing to *leave*. That interacts with
   `CloseOnEscape = false` in the preset at `DialogConfig.cs:774`; see
   [02](02-escape-and-default-buttons.md).

## Verification

1. **The primary action starts disabled.** Open `CreateDestructive`, assert the destructive button is
   disabled. *Today it is enabled — this is the check the stage exists for and it fails immediately.*
2. **Exact match enables it.** Type the keyword; assert enabled. Type it with different casing,
   trailing whitespace, or a prefix; assert **still disabled** for each. Three assertions, because a
   sloppy comparison passes the first and fails the point of the feature.
3. **Clearing the field disables it again.** Type the keyword, then backspace one character; assert
   disabled. Catches an implementation that latches on first match.
4. **Misconfiguration is loud.** `RequireTypedConfirmation = true` with an empty keyword throws at
   show time rather than rendering a dialog with an unreachable primary action.
5. **Cancel still works.** Escape and the cancel button both dismiss while the primary is disabled.
6. **Focus is not on the destructive button.** Assert initial focus is the confirmation field.
