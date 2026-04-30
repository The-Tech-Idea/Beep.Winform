# MASTER TODO TRACKER - DocumentHost MDI Commercial Program

## Program Metadata
- Program: DocumentHost MDI Commercial-Grade Enhancement
- Repo: `Beep.Winform`
- Scope: `TheTechIdea.Beep.Winform.Controls/DocumentHost`
- Tracking IDs: `DOCMDI-P{phase}-{nnn}`
- Status Workflow: `NotStarted` -> `InProgress` -> `Review` -> `Done` or `Blocked`

## Phase 1 - Foundation and Contracts
- [ ] `DOCMDI-P1-001` Define `DocumentHost vNext` API contract surface in `IDocumentHostCommandService` and options model.
- [ ] `DOCMDI-P1-002` Add command routing abstractions for context-aware command enablement and target resolution.
- [ ] `DOCMDI-P1-003` Introduce token contract for host/strip/overlay/float window theming.
- [ ] `DOCMDI-P1-004` Add telemetry contract primitives (event types, correlation, taxonomy).
- [ ] `DOCMDI-P1-005` Validate compatibility shims for migration from existing host behavior.

## Phase 2 - Docking and Layout Robustness
- [ ] `DOCMDI-P2-001` Add transaction-based docking operation model.
- [ ] `DOCMDI-P2-002` Route split and docking mutations through layout-tree orchestrator.
- [ ] `DOCMDI-P2-003` Harden float and auto-hide transitions with deterministic policy rules.
- [ ] `DOCMDI-P2-004` Expand layout migration coverage and restore diagnostics.
- [ ] `DOCMDI-P2-005` Add regression harness for complex save/restore round-trips.

## Phase 3 - UX and Command Ecosystem
- [ ] `DOCMDI-P3-001` Add preview/pin/close-policy document UX rules.
- [ ] `DOCMDI-P3-002` Extend command palette integration with active-context actions.
- [ ] `DOCMDI-P3-003` Unify tab rendering path through painter factory contracts.
- [ ] `DOCMDI-P3-004` Add window menu and activation history interactions.
- [ ] `DOCMDI-P3-005` Validate keyboard-only workflows for major navigation paths.

## Phase 4 - Performance, Accessibility, and Observability
- [ ] `DOCMDI-P4-001` Add lazy panel activation and document panel pooling.
- [ ] `DOCMDI-P4-002` Add host-wide accessibility improvements beyond tab strip.
- [ ] `DOCMDI-P4-003` Add telemetry emission hooks for command, docking, and restore flows.
- [ ] `DOCMDI-P4-004` Add measurable performance baseline tests for high document counts.
- [ ] `DOCMDI-P4-005` Add high-contrast and screen-reader regression checklist.

## Phase 5 - Designer and Release Hardening
- [ ] `DOCMDI-P5-001` Add designer actions for layout presets and docking policies.
- [ ] `DOCMDI-P5-002` Add migration guide and vNext API adoption notes.
- [ ] `DOCMDI-P5-003` Add release validation matrix and known-issues ledger.
- [ ] `DOCMDI-P5-004` Verify design-time and runtime parity on representative forms.
- [ ] `DOCMDI-P5-005` Final release readiness review and sign-off checklist.

## Cross-Cutting Risks
- [ ] `RISK-001` Dual render paths may diverge until painter unification is complete.
- [ ] `RISK-002` Layout tree and runtime group state drift under nested docking changes.
- [ ] `RISK-003` Layout schema evolution may break old persisted workspaces.
- [ ] `RISK-004` Keymap growth may increase shortcut conflict rates.

## Blocked
- None.

## Done
- None.
