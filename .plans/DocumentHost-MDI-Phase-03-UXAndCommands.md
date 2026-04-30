# Phase 03 - UX and Command Ecosystem

## Goal
Deliver commercial-grade document UX and context-aware command routing with a unified rendering path.

## Target Files
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHost.Keyboard.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHost.Events.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepCommandRegistry.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepCommandPalettePopup.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentTabStrip.Painting.cs`
- `TheTechIdea.Beep.Winform.Controls/DocumentHost/Painters/TabStripPainterFactory.cs`

## Execution Checklist
- [ ] Add preview, pinned, and close-policy behaviors with command bindings.
- [ ] Add command target routing and enablement evaluation in active context.
- [ ] Expand palette commands for pane/window/document actions.
- [ ] Reduce direct draw style branching and route style draw through painters.
- [ ] Add window/pane activation history and keyboard navigation consistency checks.

## Implementation Notes
- Maintain helper/state separation from painters.
- Keep command definitions centralized and searchable.
- Preserve existing shortcuts while allowing vNext overrides.

## Verification Criteria
- Keyboard-only workflows complete for tab switch, close, pin, split, and palette.
- Active document/context correctly controls command enabled state.
- Rendering parity confirmed across supported tab styles.
