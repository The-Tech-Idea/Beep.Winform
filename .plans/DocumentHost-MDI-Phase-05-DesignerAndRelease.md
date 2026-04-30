# Phase 05 - Designer and Release Hardening

## Goal
Finalize design-time productivity, upgrade documentation, and complete release quality gates for vNext `DocumentHost`.

## Target Files
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepDocumentHostDesigner.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/ActionLists/BeepDocumentHostActionList.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/Readme.md`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHostExtensions.cs`
- `docs/` release and migration guides for `DocumentHost`

## Execution Checklist
- [ ] Add designer actions for layout presets and docking policy toggles.
- [ ] Add vNext migration guide and API adoption cookbook.
- [ ] Add release acceptance matrix (runtime + design-time + persistence).
- [ ] Run stabilization pass and capture known issues/workarounds.
- [ ] Finalize release readiness checklist and sign-off records.

## Implementation Notes
- Keep designer-time options discoverable and safe defaults enabled.
- Ensure docs match code behavior and inheritance/contracts.
- Capture any intentional breaking changes with clear migration steps.

## Verification Criteria
- Designer smart actions apply predictable host configuration.
- Migration guide validated on at least one pre-vNext sample.
- Release checklist complete with evidence links and issue status.
