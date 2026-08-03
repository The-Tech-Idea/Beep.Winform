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
- [ ] Perspectives are not yet part of what the designer serializes into the host — they live on the
      manager for the session. Persisting the collection belongs with
      [07](07-persistence-and-migration.md)'s remaining round-trip work
