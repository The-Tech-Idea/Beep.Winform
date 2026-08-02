# 06 — Keyboard docking and accessibility

## What is missing

`grep -rn "AccessibleRole\|CreateAccessibilityInstance"` across `Docking/` returns **0**.

A docking system with no accessible surface is unusable with a screen reader, and a docking system
that can only be rearranged by dragging is unusable without a pointer. Both are true here today.

This is not a small feature bolted on at the end — it is why the tracker recommends threading it
through rather than scheduling it last. Accessibility retrofitted after seven features is
accessibility done twice.

## Reference behaviour

| product | keyboard docking |
|---|---|
| VS Code | `Ctrl+K Ctrl+←/→` move group focus; `Ctrl+K ←/→` split; `Ctrl+W` close; entire layout keyboard-drivable |
| Visual Studio | `Alt+F7` / `Alt+Shift+F7` cycle tool windows; `Ctrl+Alt+←/→` move within a group |
| JetBrains Rider | `Alt+1..9` focus tool windows; `Shift+Esc` hide; `Ctrl+Shift+←/→` resize |

## Design

**Accessible tree.** The docking host is a container of regions. Report:

- host → `AccessibleRole.Client`, children one per dock region and one per floating window
- each panel → `AccessibleRole.Pane`, named by its caption, with `Bounds` and a `Focused` state
- each tab strip → `AccessibleRole.PageTabList` with `PageTab` children

`DisplayContainers` and `Filtering` both needed exactly this and the pattern is established: override
`CreateAccessibilityInstance`, and override `GetChildCount`/`GetChild` — **the defaults return -1 and
MSAA walks the window hierarchy instead**, which is why a traversal check on an un-overridden control
measures nothing and proves nothing.

**Keyboard docking.** Every drag-reachable operation needs a command:

- focus a panel by index; cycle panels forward/back
- move the focused panel to a dock position, or into a neighbouring group
- split, maximise ([04](04-maximise-and-zen.md)), close, float, re-dock
- resize a splitter by keyboard, in a defined increment

**One key handler.** Features 01, 02 and 04 each want bindings. They route through this one, or the
folder acquires four competing `ProcessCmdKey` overrides that shadow each other.

## Work

- [ ] `CreateAccessibilityInstance` on the docking host, panels and tab strips
- [ ] Focus indicator on the focused panel — check whether one already exists before adding it; a
      complete focus ring that nothing enabled has already been found twice in this codebase
- [ ] Command surface for every drag-reachable operation
- [ ] A single key-binding table, with features 01/02/04 registering into it
- [ ] Splitter resize by keyboard
- [ ] High-contrast legibility for captions, guides and rails

## Verification

- Accessible **names** per panel, not a tree walk. Compare an empty host against a populated one so
  the child count is known to be real rather than constant
- Every operation reachable by driving keys, asserted by sending keys — not by reading the handler
- Focus indicator visibly distinct from both hover and active
- Guides and captions legible under `SystemInformation.HighContrast`
