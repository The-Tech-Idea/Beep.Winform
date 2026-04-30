# DocumentHost MDI vNext Release Matrix

## Scope
- Component: `TheTechIdea.Beep.Winform.Controls/DocumentHost`
- Release Type: Major version
- Goal: Commercial-grade MDI behavior and operational hardening

## Runtime Validation Matrix
- [ ] Open/activate/close document lifecycle remains stable.
- [ ] Split horizontal and split vertical operations remain deterministic.
- [ ] Float and dock-back flows preserve panel content and tab metadata.
- [ ] Auto-hide restore retains placement and active document correctness.
- [ ] Layout save/restore works across same-version and migrated payloads.

## Command and UX Validation Matrix
- [ ] Command service resolves command IDs and reports `CanExecute` correctly.
- [ ] Command palette and command registry keep usage tracking intact.
- [ ] Routed command toggle (`EnableRoutedCommands`) does not regress legacy behavior.
- [ ] Painter-preferred tab rendering (`PreferPainterRendering`) displays all tab states.

## Performance and Accessibility Matrix
- [ ] Deferred panel loading activates content on first document focus.
- [ ] Large document counts do not freeze layout recalculation.
- [ ] Active document updates accessibility metadata (`AccessibleName/Description`).
- [ ] Restore telemetry emits correlation, duration, and success status when enabled.

## Designer Validation Matrix
- [ ] Smart-tag exposes new vNext toggles.
- [ ] Designer transactions apply without serialization regressions.
- [ ] Design-time documents still serialize and restore into `InitializeComponent`.

## Release Exit Criteria
- [ ] Target project builds without errors.
- [ ] Known issue list reviewed and accepted.
- [ ] Migration notes reviewed by maintainers.
