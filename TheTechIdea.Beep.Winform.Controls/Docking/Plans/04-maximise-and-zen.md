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

---

## Outcome

Built as designed: maximise is a transient property of the layout pass, and the tree is never
written.

### The split of responsibility

| owns | where | what it does while maximised |
|---|---|---|
| geometry | `DockingLayoutController.MaximisedPanelKey` | allocates the container to one panel and its ancestor groups; emits no splitters |
| controls | `BeepDockingManager.Maximise.cs` | conceals other panels and dockspaces, suppresses chrome for zen |

The layout controller never touches controls and the manager never touches geometry, which is the
existing separation in this folder rather than a new one.

Panels are concealed with `Control.Visible` alone. Moving them to `DockPanelState.Hidden` would have
rewritten the very state a restore has to return — and, since the [10](10-verification-harness.md)
fix, would also have interacted with group pruning.

`BuildMaximisedLayout` returns `false` when the key is stale, so a panel closed or floated while
maximised falls through to the normal arrangement instead of producing an empty layout.

### Commands

`MaximisePanel` / `RestoreFromMaximise` / `ToggleMaximise`, `EnterZenMode` / `ExitZenMode` /
`ToggleZenMode`, plus `IsMaximised`, `IsZenMode` and `IsPanelMaximised(key)` for caption painters,
and `PanelMaximised` / `PanelRestored` events following the folder's existing
`EventHandler<DockPanel>` shape.

Zen suppresses panel captions, sets dockspace headers to the existing `HeaderPosition.None` rather
than inventing a property, and hides the auto-hide rails — recording each prior value so
`ExitZenMode` restores what it actually found instead of a guessed default.

`HidePanel`, `ClosePanel`, `FloatPanel` and `AutoHidePanel` all restore first. Without that, removing
the panel that owns the container would leave every other panel concealed with nothing occupying the
space.

### Measured

The existing bounds-only capture could not test this feature's central claim: relayout would produce
identical pixels from a rewritten tree. So the probe adds a **structural** capture — nesting, ids,
positions, orientations, split ratios at `"R"` precision, active tab, membership — and proves it is
sensitive before trusting it:

```
the tree capture notices a 0.01 split-ratio change     PASS
baseline: the tree capture is blind to a ratio change  PASS (expected-fail)
```

Then:

```
editor while maximised: 900x600 (client 900x600)
before maximise: editor:Fill:464x447 | explorer:Left:249x447 | output:Bottom:900x149 | props:Right:179x447
after  restore:  editor:Fill:464x447 | explorer:Left:249x447 | output:Bottom:900x149 | props:Right:179x447
splitters: 3 docked, 0 while maximised, 3 after restore
```

Tree identical after maximise, after restore, and after three round-trips; every panel bound back to
the pixel; concealed panels keep `State=Docked`; closing the maximised panel leaves the other three
visible; maximising an unknown or hidden panel is refused.

### One piece of dead code caught by asking what the checks did not cover

`ApplyMaximiseVisibility` originally hid splitters. It never showed them again — and the panel-bounds
assertions all passed anyway, because they say nothing about whether edges are draggable. Reading
`SyncSplitters` settled it: a maximised result carries no splitters, so they are disposed as orphans
and rebuilt from the restored result. The hide was redundant *and* implied they survive a maximise
when they do not. Removed, and replaced with a check that measures the thing that actually matters —
`3 → 0 → 3`, all visible.

### Coverage this run did not have

Panels hosted directly on the form give `0` dockspaces, so zen's **header** suppression and the
dockspace-concealment branch were not exercised; the rails were present but their suppression is
asserted only indirectly. The probe prints this rather than leaving the pass looking complete.

`DockProbe`: **56 passed, 0 failed**. Solution builds with 0 errors.

### Remaining

- [ ] Double-click a caption to toggle maximise, and `Esc` to restore — input wiring belongs with
      [06](06-keyboard-and-accessibility.md) so the folder gets one key table
- [ ] Maximise across a perspective switch needs a defined outcome — [02](02-layout-perspectives.md)
- [ ] Zen with a dockspace-hosted layout, to cover the header and rail paths above
