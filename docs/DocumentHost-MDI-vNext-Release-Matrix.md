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
- [x] Automated regression coverage exists for floated-document restore and multi-group layout reapply.

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
- [x] Representative container-host audit completed against `TheTechIdea.Beep.Winform.Default.Views/MainFrm_MDI.Designer.cs`.
- [x] Automated regression coverage exists for authored-child remove/reparent on a design-mode host.
- [ ] Manual delete/remove/reparent/reopen validation in the Visual Studio designer is still pending.

## Release Exit Criteria
- [ ] Target project builds without errors.
- [ ] Known issue list reviewed and accepted.
- [ ] Migration notes reviewed by maintainers.
