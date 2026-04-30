# Phase 01 - Foundation and Contracts

## Goal
Create stable vNext contracts for `DocumentHost` so later phases can add commercial-grade behavior without breaking internal architecture.

## Target Files
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/IDocumentHostCommandService.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/DocumentHostCommandService.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHostOptions.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/Tokens/DocumentHostTokens.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHostProfiler.cs`

## Execution Checklist
- [ ] Define command target abstraction and context object contract.
- [ ] Add vNext option toggles for transactional docking and routed commands.
- [ ] Define telemetry event contract objects and event categories.
- [ ] Define token extensions for overlays, focus state, compact density, and pinned previews.
- [ ] Add migration-aware defaults that keep old behavior unless vNext enabled.

## Implementation Notes
- Keep control behavior in host/helpers and keep painters render-only.
- Keep fonts and visuals theme-driven via existing theme/token system.
- Follow partial class boundaries and single-responsibility per new class.

## Verification Criteria
- Build succeeds with new contract types integrated.
- Existing open/close/activate behavior still works with defaults.
- New contract objects are consumed by host pathways without null/reference faults.
