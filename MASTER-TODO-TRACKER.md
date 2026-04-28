# MASTER TODO TRACKER

## ComboBox Popup + Painter Overhaul

### Phase 0 - Planning Artifacts
- [x] Create master tracker
- [x] Create phase docs under `docs/plans/combobox-overhaul/`

### Phase 1 - Painters Stabilization
- [x] Consolidate shared painter state visuals into base helpers
- [x] Eliminate duplicate loading indicator rendering path
- [x] Move variant painters toward render-state-driven usage
- [ ] Verify visual matrix for all `ComboBoxType` states (manual QA pending)

### Phase 2 - Popup Behavior Correctness
- [x] Unify selectable-row predicate across popup content variants
- [x] Standardize row-kind rendering contract for all variants
- [ ] Validate keyboard navigation parity (manual QA pending)

### Phase 3 - Multi-Select Workflow Parity
- [x] Unify apply/cancel semantics between controls
- [x] Batch select-all/clear-all updates to avoid event storms
- [ ] Validate large-list responsiveness and state consistency (manual QA pending)

### Phase 4 - Theme, DPI, RTL, and Property Contracts
- [x] Wire popup-related properties end-to-end
- [x] Apply explicit precedence: property override > token > fallback
- [x] Complete popup/field RTL parity
- [ ] Validate DPI and theme switching behavior (manual QA pending)

### Phase 5 - Architecture Consolidation
- [x] Consolidate `ComboBoxType` mapping in a single registry
- [x] Extract shared popup plumbing to reduce duplication
- [x] Preserve model fields during host normalization
- [ ] Validate type mapping consistency and behavior parity (manual QA pending)

### Automated Verification Completed
- [x] `dotnet build TheTechIdea.Beep.Winform.Controls/TheTechIdea.Beep.Winform.Controls.csproj` passes after each consolidation wave
- [x] Lint checks on modified `ComboBoxes` and `Popup` files show no diagnostics

### Manual QA Matrix Pending
- [ ] Per-variant visual/state matrix (`normal/hover/focus/open/disabled/loading/validation`)
- [ ] Keyboard-only navigation parity across all popup content variants
- [ ] Multi-select stress pass (`select-all/clear-all/toggle burst`) on large lists
- [ ] DPI pass (`100/125/150/200`) and RTL pass for field + popup alignment
- [x] QA matrix doc created: `docs/plans/combobox-overhaul/manual-qa-matrix.md`

### Manual QA Execution Order
- [ ] Run core field state matrix for all `ComboBoxType` variants
- [ ] Run popup behavior matrix by popup content variant
- [ ] Run row-kind contract matrix (including state rows)
- [ ] Run multi-select stress matrix on large dataset
- [ ] Run property contract matrix and record pass/fail summary
- [x] QA session log template created: `docs/plans/combobox-overhaul/manual-qa-session-log.md`
