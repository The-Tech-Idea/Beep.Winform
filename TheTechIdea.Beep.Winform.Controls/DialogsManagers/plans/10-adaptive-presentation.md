# Stage 10 — narrow windows, stacking, scroll lock

**Kind:** enhancement. Three presentation behaviours current frameworks treat as table stakes.

Scheduled last: all three change layout for every dialog, so they want the layouts from
[06](06-severity-and-headers.md)–[09](09-async-and-long-content.md) settled first.

## Narrow presentation

`MinWidth` is 300 and `MaxWidth` 600 (`DialogConfig.cs:252`, `:257`). A 600px dialog with a
horizontal button row is fine on a desktop and wrong in a 480px-wide app window or a remote session
— the buttons crowd or clip.

Every current framework switches presentation below a breakpoint: web dialogs become bottom sheets,
Material specifies full-screen dialogs on compact widths, and the common desktop equivalent is to
let the dialog use the available width and **stack the buttons vertically**.

`ButtonLayout` already has `Vertical` and `DialogHelpers` already implements it
(`Helpers/DialogHelpers.cs:146`) — it is simply never given the config's value. That is stage
[05](05-dead-config-surface.md), and it is why 05 comes first: this stage is mostly *using* the wire
05 connects.

### The fix

A breakpoint on the owner's client width. Below it: width becomes the available width minus a margin
rather than `MaxWidth`, buttons stack vertically with the primary on top, and the icon-left layout
from [08](08-body-layouts-and-callouts.md) falls back to centred. Above it, nothing changes.

## Stacking

A dialog can open another — a confirm on top of a form is ordinary. Today each is an independent
top-level `Form` with its own `DialogBackdropForm`, so two dialogs mean two backdrops, and the second
dims the first. Three mean three.

`BeepDialogManager.Core.cs` has no notion of a dialog stack, so nothing decides z-order, which
backdrop is real, or which dialog Escape addresses.

### The fix

1. One backdrop for the stack, owned by the manager, its dim not compounding with depth.
2. Escape and the focus trap address the **topmost** dialog only.
3. Closing out of order — a caller closing the parent while a child is open — either closes the
   children or is refused, but must not leave an orphaned modal with a dead owner. That is the
   failure that strands a user with an undismissable window.

## Scroll lock

While a modal is open, the content behind it should not scroll. On the web this is `overflow: hidden`
on the body; in WinForms the owner is disabled, so wheel events over it are mostly inert — but a
scrollable child with its own handler, and any control that scrolls on hover, can still move.

This is the smallest item here and worth doing because a background that scrolls under a modal
undermines the modality the whole stage set is about.

## Verification

1. **The breakpoint switches presentation.** At 800px owner width assert horizontal buttons and
   `MaxWidth`; at 420px assert vertical buttons and a width within the owner minus margins. Both
   measured from the same dialog config — the switch is the assertion.
2. **Nothing changes above the breakpoint.** Byte-identical to the pre-stage corpus at desktop
   widths. Adaptive layout that alters the common case is a regression wearing a feature's clothes.
3. **One backdrop for three dialogs.** Open three nested; assert exactly one backdrop form exists and
   its opacity equals the single-dialog value. *Today: three backdrops, compounding dim — capture the
   measured opacity before the change.*
4. **Escape addresses the top.** With three open, Escape closes the third and leaves two.
5. **Out-of-order close leaves nothing orphaned.** Close the parent with a child open; assert no
   modal remains with a disposed owner.
6. **The background does not scroll.** Put a scrollable control on the owner, open a modal, send
   wheel events over the owner, assert its scroll position is unchanged.
