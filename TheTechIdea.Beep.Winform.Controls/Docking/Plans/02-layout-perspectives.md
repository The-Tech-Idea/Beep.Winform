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
