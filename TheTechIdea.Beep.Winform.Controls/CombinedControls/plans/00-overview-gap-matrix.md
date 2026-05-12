# CombinedControls Overview And Gap Matrix

Priority: High
Status: Planning Approved
Depends on: Existing BeepChipListBox / BeepRadioListBox partial-class architecture

## Vision

Evolve the CombinedControls family from composite utility controls into polished, commercial-grade selector surfaces with:

- predictable state coordination between list, chip, and radio affordances
- modern visual density and spacing aligned with Figma component systems
- accessible keyboard and screen-reader behavior
- stronger style presets and theme-aware rendering
- drag-free clarity for selection, search, and filtering workflows

## External Benchmark Inputs

### Commercial reference capabilities

- DevExpress WinForms Scheduler and editor controls: dense but clear state hierarchy, strong DPI behavior, theme consistency, rich view presets, and explicit initialization patterns
- DevExpress checked combo / radio group editors: compact selection affordances, integrated glyph alignment, clear selected-state presentation, and editor-style consistency

### Open-source and design-system reference capabilities

- GitHub-based UI control patterns: modular state machines, separated layout and painter concerns, and explicit hover/focus/selection APIs
- Figma-style component systems: tokens for spacing, radii, typography, elevation, and state layers; consistent variants; clear empty/loading states
- Fluent / Material / shadcn-style patterns: subtle motion, readable hierarchy, chip-like affordances, and high-contrast focus treatment

## Current Architecture Snapshot

- BeepChipListBox already has a partial-class split for core, properties, events, methods, and drawing
- BeepRadioListBox exists as a separate combined selector with a similar partial-class intent
- ChipListBoxStyleCoordinator already maps ListBoxType and ChipStyle pairs
- SelectionSyncHelper already supports bidirectional synchronization between list and chip surfaces

## Key Gaps

1. The two combined controls are feature-rich but not yet governed by a shared component contract for state, variants, and UX rules.
2. Style coordination is explicit, but visual tokens, density tiers, and variant guidance are not documented as a product-level system.
3. Keyboard parity and accessibility behavior are not formalized as a phase-owned deliverable.
4. Search, empty-state, loading-state, and high-volume behavior need clearer UX policy.
5. The controls expose many style combinations, but the recommended commercial-friendly defaults are not documented.
6. There is no phased validation tracker that ties planned behavior to manual QA checkpoints.

## Phase Map

- Phase 1: Shared foundation, tokens, and style contract
- Phase 2: Selection, search, and interaction parity
- Phase 3: Visual variants, accessibility, and performance
- Phase 4: Integration, documentation, and productization

## Definition Of Done For Program

- All phases complete with per-phase DoD satisfied.
- TODO tracker reflects verified status and remaining risks.
- Validation notes cover keyboard, DPI, empty/loading, and variant behavior.
- Plan docs stay aligned with the implementation and the CombinedControls README.
