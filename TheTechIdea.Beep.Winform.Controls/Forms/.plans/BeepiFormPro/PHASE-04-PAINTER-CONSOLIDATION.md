# Phase 4 - Painter Consolidation

## Goal

Reduce painter maintenance cost while preserving the visual identity of each style.

## Evidence From Current Code

- The painter folder contains a large set of style implementations with repeated background, caption, border, and button-layout patterns.
- `Painters/IFormPainter.cs` makes each painter responsible for both painting and layout/hit-area registration, which increases duplication quickly.
- `Painters/Readme.md` explicitly avoids inheritance, which means consolidation must happen through composition, helpers, or shared contracts rather than a deep base class.
- Documentation and enum mapping details are already drifting from the live file set, which is a maintenance warning for the painter catalog itself.

## Scope

- Extract composition-based helpers for repeated geometry, icon rendering, button glyphs, clipping, and border effects.
- Keep unique visual rendering logic inside each painter.
- Tighten the painter catalog so docs, enum mappings, and actual files match.

## Deliverables

- Shared painter helper components for common caption-button families and repeated visual primitives.
- A reduced duplication matrix for right-aligned, left-aligned, and special-layout painters.
- Updated painter inventory documentation tied to the real enum and factory mappings.
- A shared animation primitive layer for hover, press, reveal, and caption transition effects.
- A decision on whether ToolStrip and menu renderers should be bridged to the active form theme.

## Task Breakdown

- [ ] Inventory repeated patterns across the current painter set.
- [ ] Extract shared helper methods or helper objects for common geometry and button rendering.
- [ ] Keep style-specific surfaces, colors, and effects in each painter file.
- [ ] Reconcile painter docs with actual files and `PaintersFactory` mappings.
- [ ] Define a checklist for adding future painters without repeating contract drift.
- [ ] Introduce a shared animation manager instead of per-painter ad hoc timing logic.
- [ ] Evaluate optional renderer bridging for hosted menus and toolstrips so the skin feels system-wide instead of caption-only.

## Consolidation Rules

1. No deep inheritance tree.
2. Shared behavior lives in helper utilities or small composition objects.
3. Painter files remain readable in isolation.
4. Layout ownership stays explicit even when helpers are reused.

## Risks

- Extracting helpers too aggressively can make painters harder to reason about if behavior becomes hidden.
- Leaving the current duplication untouched will keep bug fixes expensive across the full catalog.

## Exit Criteria

- Shared caption and border patterns are implemented once where practical.
- Painter docs and factory mappings match the real file set.
- Adding a new painter no longer requires copying large blocks of layout or interaction code.