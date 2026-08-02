# 04 — Panel maximise and zen mode

## What is missing

`grep -rn "Maximize"` across `Docking/` returns **1** reference. There is no way to temporarily give
one panel the whole workspace and then put everything back.

| product | feature |
|---|---|
| VS Code | `Ctrl+K Z` zen mode; `Ctrl+K Ctrl+M` maximise the editor group |
| JetBrains Rider | `Shift+Esc` hides the active tool window; `Ctrl+Shift+F12` maximises the editor |
| Visual Studio | double-click a document tab to maximise the group |
| Blender | `Ctrl+Space` maximises the area under the cursor |

Every one of them uses the same interaction: a **reversible** takeover. That reversibility is the
whole feature — the user is temporarily concentrating, not rearranging.

## Design

Two distinct modes, often confused:

- **Maximise panel** — one panel fills the docking host. Everything else is hidden, not closed, and
  not moved. Restoring returns the exact prior arrangement.
- **Zen / distraction-free** — maximise plus hiding chrome (tab strips, auto-hide rails, navigator).
  A superset, and worth having as a separate command because users reach for them differently.

The critical property in both: **the layout tree is not mutated.** Maximise sets a transient
"maximised node" on the controller and layout renders around it. If maximise instead rearranged the
tree, restoring would be a best-effort reconstruction and would drift — which is how implementations
that took the shortcut ended up losing splitter positions.

Interaction, matching the reference products:

- double-click a panel caption toggles maximise
- `Esc` restores when maximised, before any other `Esc` handling
- a maximised panel that is closed restores the layout rather than leaving an empty maximised slot

## Work

- [ ] Transient `MaximisedNode` on `DockingLayoutController`; layout honours it without tree mutation
- [ ] `ToggleMaximise(panel)`, `RestoreLayout()`, and a `IsMaximised` state the caption painter reads
- [ ] Zen mode as maximise + chrome suppression, separately commanded
- [ ] Double-click caption, and keyboard bindings via feature 06
- [ ] Close-while-maximised restores first, then closes
- [ ] Auto-hide rails and the navigator respect zen mode rather than painting over it

## Verification

- Capture the full layout tree, maximise, restore; assert the tree is **identical**, including
  splitter ratios to the pixel
- Maximise, close the maximised panel; assert the previous arrangement returns and no empty slot
  remains
- Maximise, switch perspective ([02](02-layout-perspectives.md)), switch back; assert a defined
  outcome — either the maximise is captured or it is dropped, but not a half-restored layout
- Zen mode: assert tab strips, auto-hide rails and navigator are all suppressed, not just the first
