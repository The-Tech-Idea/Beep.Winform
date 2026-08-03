# 02 — Named layout perspectives

## What is missing

`grep -rn "Perspective"` across `Docking/` returns **0**.

The docking system can persist *one* layout. It cannot hold several and switch between them.

| product | feature |
|---|---|
| JetBrains Rider / IntelliJ | *Window Layouts* — save the current arrangement by name, restore it, one designated as default |
| Visual Studio | *Window Layouts* with keyboard shortcuts (`Ctrl+Alt+1..9`) |
| Blender | *Workspaces* — Layout, Modeling, Sculpting, each a complete arrangement, switched by tab |
| Figma / Adobe | named workspaces per task |
| Eclipse | *Perspectives*, the term this feature borrows |

The value is that a developer's arrangement for **writing code** is not their arrangement for
**debugging**, **reviewing a diff**, or **designing a form** — and rebuilding it by hand each time is
the friction this removes.

## What exists to build on

`BeepDockingManager.Persistence.cs` (264 lines) with `LoadLayout(DockLayoutDefinition)` and a
`DockLayoutDefinition` model. Perspectives are that model, keyed by name, plus a switch operation.

## Design

- A perspective is `{ Name, DockLayoutDefinition, IsDefault, Shortcut? }`
- Stored alongside the existing layout persistence, not in a parallel mechanism
- **Switching must be non-destructive**: capture the current arrangement into the active perspective
  before loading another, or the user silently loses their adjustments — the mistake every product
  that got this wrong made first
- Built-in perspectives ship as defaults and can be reset, exactly as Rider's are
- A perspective that references a panel which no longer exists must degrade, not fail (see
  [07](07-persistence-and-migration.md))

## Work

- [ ] `DockPerspective` model and a collection on the manager
- [ ] `SavePerspective(name)`, `ApplyPerspective(name)`, `DeletePerspective(name)`, `ResetToDefault`
- [ ] Capture-before-switch, with an opt-out for a deliberately pristine restore
- [ ] Optional keyboard bindings, routed through feature 06 rather than a second key handler
- [ ] A perspective picker — the navigator (`Runtime/BeepDockingNavigator.cs`) is the natural host
- [ ] Missing-panel degradation shared with feature 07

## Verification

- Save perspective A, rearrange, save B, switch A→B→A; assert the arrangement matches A exactly
- Rearrange without saving, switch away and back; assert the adjustment was captured, not discarded
- Apply a perspective referencing a panel that no longer exists; assert the rest of the layout loads
- Assert switching raises exactly one layout-changed notification, not one per panel moved

---

## Outcome

The premise held: `grep -rn "Perspective"` returned **0** (the few "workspace" hits are unrelated
comments). Unlike [01](01-split-editor-groups.md), nothing was hiding under a different name.

### Built on persistence, not beside it

A `DockPerspective` is a `DockLayoutDefinition` with a name and an `IsDefault` flag, and every
operation routes through `CaptureDefinition` / `MaterializeFromDefinition`. That is the whole design
decision: a perspective inherits schema versioning, missing-panel degradation and hidden-panel
membership from [07](07-persistence-and-migration.md) without restating any of it. Applying a
perspective that names a panel from an uninstalled plugin degrades correctly because the materialiser
already does — it was never taught about perspectives.

This is why 07 was taken before 02, against the tracker's suggested order. Building perspectives on
the load path as it stood would have inherited the stranding defect and the unread version field into
every saved arrangement, and multiplied them by the number of perspectives a user keeps.

### The operations

`SavePerspective`, `ApplyPerspective(name, captureCurrent)`, `RevertPerspective`,
`DeletePerspective`, `SetDefaultPerspective`, `ApplyDefaultPerspective`, `ApplyPerspectiveByIndex`,
plus `Perspectives`, `ActivePerspectiveName`, and `PerspectiveApplied` / `PerspectivesChanged`.

`Ctrl+Alt+1..9` — Visual Studio's binding — through the existing key handler.

### Capture-before-switch, asserted in both directions

This document names it as "the mistake every product that got this wrong made first", so it is
asserted as a behaviour rather than described as an intention:

```
adjusted:            editor:615x447 | explorer:281x447 | output:900x149
after away-and-back: editor:615x447 | explorer:281x447 | output:900x149
```

And the documented opt-out genuinely opts out — `ApplyPerspective(name, captureCurrent: false)`
discards the adjustment rather than capturing it. A "pristine restore" that quietly captured would
be worse than not offering one.

Capture happens only into a perspective that is actually **active**. An arrangement that did not come
from a perspective has nowhere to be captured to, and inventing a slot would create perspectives the
user never asked for.

### Measured

```
Coding:    editor:647x447 | explorer:249x447 | output:900x149
Debugging: editor:599x447 | explorer:297x447 | output:900x149
switching back to Coding: exact match
PerspectiveApplied raised 1 time(s) for one switch
Ctrl+Alt+2 -> 'Debugging', 1 notification
plain Alt+1 -> focuses 'explorer', perspective unchanged
perspective naming a missing panel: no throw, 3/3 placed
```

The one-notification assertion is this document's fourth check: a switch moves every panel, and a
notification per panel would make the event useless for anything that reacts to a layout change.

`Alt+1..9` and `Ctrl+Alt+1..9` share the `Alt` modifier, so **both** are asserted after adding the
second. That is not caution for its own sake — [06](06-keyboard-and-accessibility.md) lost a working
resize binding to exactly this, when a guard that did not exclude `Alt` swallowed the key and
declined to act.

`DockProbe`: **131 passed, 0 failed**. Solution builds with 0 errors.

### Remaining

- [ ] A perspective picker in `Runtime/BeepDockingNavigator.cs` — the commands and events it would
      bind to are all in place; this is the UI surface
- [ ] Shipping named built-in perspectives (Rider's "Debug", Blender's "Sculpting") is an
      **application** concern, not the manager's: it cannot invent arrangements for panels it does
      not know. `IsDefault` plus `ApplyDefaultPerspective` is the hook an application uses to do it
### Perspectives now survive a restart — schema v3

Named layouts lived only on the manager, so they were gone when the application closed. Saving one
was close to pointless. `DockLayoutDefinition.Perspectives` carries them, and a perspective's
arrangement is itself a `DockLayoutDefinition`, so the shape nests — deliberately, because a
perspective then inherits schema versioning, missing-panel degradation and hidden-panel membership
from the same materialiser instead of needing a parallel format that would drift.

```
saved layout carries 1 perspective(s), schema v3
fresh manager: 0 perspectives -> after restore: Debugging (default)
restoring does not activate a perspective
loading a v2 layout with no perspectives: 1 -> 1
```

Two rules the checks pin down:

- **Restoring a layout does not apply one of the perspectives it carries.** The perspectives are
  choices available afterwards; applying one would silently override the arrangement just asked for.
- **A definition carrying no perspectives leaves the stored ones alone.** A v1 or v2 layout predates
  the field, and reading "absent" as "delete them all" would destroy the user's saved layouts on the
  first load of an older file.

### A recursion this introduced, and the guard that caught it

`ApplyPerspective` materialises through the same method a top-level load uses — so once that method
restored perspectives, **applying one perspective replaced the manager's entire perspective list**
with whatever copy that perspective happened to carry. The next check indexed `Perspectives[1]` and
threw.

It surfaced as a printed stack rather than a silent stall only because of the unhandled-exception
guard added in [10](10-verification-harness.md); before that it would have been an invisible modal
dialog on an off-screen window.

Fixed in two places, because either alone would have left the other half wrong:

- `MaterializeFromDefinition` takes `restorePerspectives`. True for a top-level load, false when
  applying a perspective.
- A perspective's stored arrangement no longer carries a perspective list at all. Nesting would grow
  without bound across repeated saves, and let one perspective redefine the others.

`DockProbe`: **223 passed, 0 failed**. Docking suite 48/48. Solution 0 errors.

### Remaining

- [ ] A perspective picker in `Runtime/BeepDockingNavigator.cs` — the commands, events and now the
      persistence it would bind to are all in place; this is the UI surface
