# Charts Plans

This folder contains the phased enhancement plan for the Charts family centered on `BeepChart` and its painter/helper ecosystem.

## Plan Index

- [00-overview-gap-matrix.md](00-overview-gap-matrix.md)
- [01-phase1-foundation-chart-contracts.md](01-phase1-foundation-chart-contracts.md)
- [02-phase2-interactions-navigation-and-analysis.md](02-phase2-interactions-navigation-and-analysis.md)
- [03-phase3-visual-variants-accessibility-performance.md](03-phase3-visual-variants-accessibility-performance.md)
- [04-phase4-integration-documentation-productization.md](04-phase4-integration-documentation-productization.md)
- [05-external-benchmark-and-ux-standards.md](05-external-benchmark-and-ux-standards.md)
- [06-phase1-implementation-checklist.md](06-phase1-implementation-checklist.md)
- [07-phase1-file-by-file-matrix.md](07-phase1-file-by-file-matrix.md)
- [08-phase1-review-notes.md](08-phase1-review-notes.md)
- [09-phase1-task-breakdown.md](09-phase1-task-breakdown.md)
- [TODO-TRACKER.md](TODO-TRACKER.md)

## Program Notes

- The plan is grounded in the existing `BeepChart` partial-class architecture.
- Current helper coverage already includes viewport scaling, axis detection, painter selection, theme-derived colors, tooltips, and input helpers.
- The roadmap emphasizes product-grade analytical usability, not just more chart types.
- Figma-style UX standards are treated as first-class requirements for chart surfaces, legends, tooltips, and empty states.
- Phase 1 should begin with painter contracts, chart-type capability rules, and state ownership.
- Phase 1 execution should start with the checklist, file-by-file matrix, and review notes.

## Current Alignment Snapshot

- Runtime implementation has delivered Phase 2 and Phase 3 interaction/accessibility/performance objectives.
- Phase 4 integration APIs are now available in `BeepChart` (streaming append, export, print, snapshot, and state persistence).
- Use `TODO-TRACKER.md` as the source of truth for any remaining planning-track items.

## Recommended First Execution Stream

- Start with Phase 1 painter and layout contracts before any new interaction expansion.
- Reason: rendering ownership and extensibility boundaries should stabilize first, then interaction additions can rely on those contracts.
- Entry set: `06-phase1-implementation-checklist.md`, `07-phase1-file-by-file-matrix.md`, and `09-phase1-task-breakdown.md`.
- Remaining external step: implementation-owner review and signoff on this execution order.