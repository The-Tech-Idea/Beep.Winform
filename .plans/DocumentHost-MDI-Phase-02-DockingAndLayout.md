# Phase 02 - Docking and Layout Robustness

## Goal
Move docking and split behavior to a transaction-driven, layout-tree-first model with reliable persistence and migration diagnostics.

## Target Files
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHost.Layout.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHost.Documents.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHost.AutoHide.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHost.Serialisation.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/Layout/LayoutTreeApplier.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/Layout/LayoutMigrationService.cs`

## Execution Checklist
- [ ] Add transaction scope object for docking/split operations.
- [ ] Ensure all group/layout mutations route through transaction coordinator.
- [ ] Add deterministic auto-hide collapse/expand policies.
- [ ] Harden restore with richer validation and fallback repair path.
- [ ] Add deep round-trip coverage scenarios for nested split + float + auto-hide.

## Implementation Notes
- Avoid ad-hoc mutations in multiple partials.
- Keep `ILayoutNode` tree as single source of truth.
- Keep restore diagnostics structured and actionable.

## Verification Criteria
- Complex layouts restore with no orphaned panes.
- Legacy schema migration logs clear fix-up actions.
- Docking operations do not leave invalid split ratios or null root nodes.
