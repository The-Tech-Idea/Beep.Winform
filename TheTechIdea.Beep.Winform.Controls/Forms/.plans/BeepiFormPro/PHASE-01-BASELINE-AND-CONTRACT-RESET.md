# Phase 1 - Baseline And Contract Reset

## Goal

Create a stable implementation baseline for `BeepiFormPro` by defining ownership boundaries, lifecycle rules, and source-of-truth documentation before fixing deeper behavior.

## Why This Phase Comes First

The current codebase mixes multiple responsibilities across the main form class, painter layer, and designer/runtime hooks. Until those boundaries are explicit, any fix in later phases can reintroduce stale layout, theme, or interaction behavior.

## Evidence From Current Code

- `BeepiFormPro.cs` constructor performs startup style application, window-region refresh wiring, designer child-hook registration, and global theme synchronization in one path.
- `BeepiFormPro.Core.cs` exposes `ActivePainter` publicly while also keeping a `Painters` list that is not the active source of painter selection.
- `BeepiFormPro.Theme.cs` uses `Theme`, `ThemeName`, and `CurrentTheme` together, but they do not express one clearly documented ownership model.
- `Painters/Readme.md` and other historical plan docs describe the painter subsystem as effectively complete, while the live code still needs contract cleanup.

## Scope

- Document and enforce the runtime lifecycle of `BeepiFormPro`.
- Define who owns layout calculation, hit-area registration, theme resolution, painter activation, and region updates.
- Archive or relabel outdated docs that currently read as completed implementation status.
- Remove or de-emphasize unused or misleading public surface where it confuses the contract.

## Deliverables

- One canonical lifecycle note for initialization, handle creation, theme/style apply, layout invalidation, and disposal.
- One canonical painter contract note covering layout, painting, and hit-area naming.
- Updated legacy planning docs so they point to the new plan set instead of claiming completion.
- A short list of invariants that future painter changes must respect.

## Task Breakdown

- [ ] Document the control lifecycle in code comments or a focused design note.
- [ ] Decide whether `ActivePainter` is the only active painter state, and remove or document any secondary list state.
- [ ] Normalize the public theme/style surface so ownership is obvious before later refactors.
- [ ] Mark older docs as archived, historical, or superseded.
- [ ] Add an invariant checklist for future changes.

## Proposed Invariants

1. One code path chooses the active painter.
2. One code path resolves the current theme object.
3. Layout invalidation never silently leaves hit areas stale.
4. Painter contracts are explicit enough that `OnRegionClicked` does not need to guess intent from multiple naming schemes.
5. Documentation status must match code status.

## Risks

- Over-cleaning public API too early could break designer serialization or downstream callers.
- Documentation-only cleanup without code comments will not hold if future painter additions ignore the contract.

## Exit Criteria

- The active lifecycle is documented in one place.
- Legacy docs no longer read as the current implementation source of truth.
- Later phases can reference explicit ownership rules instead of inferred behavior.