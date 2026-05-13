# MASTER TODO TRACKER

## BeepTabs Commercialization Program

### Phase 0 - Planning And Alignment
- [x] Reset `Tabs/.plans/README.md` to the active commercial cutover plan
- [x] Update `Tabs/.plans/00-overview-gap-matrix.md` to reflect the real blockers and phase ownership
- [x] Rewrite `Tabs/.plans/01-phase1-foundation-and-architecture.md` around the Beep-owned page/content model
- [x] Rewrite `Tabs/.plans/02-phase2-overflow-header-actions-and-rich-tabs.md` around the post-cutover header/render contract
- [x] Rewrite `Tabs/.plans/03-phase3-document-workspace-and-advanced-interactions.md` around document/workspace product behavior
- [x] Rewrite `Tabs/.plans/04-phase4-accessibility-design-time-and-quality.md` around the final page model and shipping-quality gates
- [x] Update `Tabs/Readme.md` to remove stale feature-complete language and align with the active roadmap

### Phase 1 - Foundation And Architecture
- [x] Introduce a Beep-owned `BeepTabPage` container type
- [x] Replace premium-facing `TabPage` API seams in `BeepTabs`
- [x] Route runtime selected-content presentation through `BeepTabContentHost`
- [x] Move tab metadata ownership off parallel dictionaries and onto the page model
- [x] Move basic owner/helper/painter item-state and content forwarding off `BeepTabsRuntimeBridge`
- [x] Remove `BeepTabsRuntimeBridge` and the unused header-metrics cache from the tabs core path
- [x] Preserve and self-heal page order, including selected-index notifications after reorder
- [ ] Unify runtime and design-time hosted-content workflow around the Beep-owned host architecture
- [x] Remove the pass-through `BeepTabContentProjection` seam

### Phase 2 - Overflow, Header Actions, And Rich Tabs
- [ ] Unify painters around a single item-based render contract
- [ ] Finalize commercial overflow policies and popup behavior
- [ ] Finalize reusable header action-slot layout and routing
- [ ] Complete rich header metadata layout for icon, badge, subtext, dirty, and busy states
- [ ] Document and validate per-style behavior recipes

### Phase 3 - Document And Workspace Product Behavior
- [ ] Finalize mode-aware policies for Navigation, Documents, and Workspace
- [ ] Harden pinned, preview, dirty, and close-guard behavior
- [ ] Route all document/workspace commands through one shared command model
- [ ] Finalize MRU and quick-switch behavior
- [ ] Decide and document the long-term `DocumentHost` relationship

### Phase 4 - Accessibility, Design-Time, And Quality
- [x] Make designer verbs and smart-tag labels page-centric and expose the live selected-page metadata/workspace surface plus overflow/header preview
- [x] Fix designer header hit testing to use BeepTabs client coordinates
- [x] Persist selected-page tab metadata through page-owned BeepTabPage properties and reset that metadata cleanly
- [x] Route selected-page metadata reset through serializer-visible page properties and designer change notifications
- [x] Limit default designer page creation to newly dropped BeepTabs controls so empty saved tab sets are not recreated on reload
- [x] Hide internal tab header/content hosts from the toolbox
- [x] Hide runtime selection/state command surfaces from designer serialization
- [x] Add focused BeepTabs persistence smoke tests for InitializeComponent-style page/control rehydration
- [x] Run BeepTabs persistence smoke tests (`Passed: 3, Failed: 0`)
- [ ] Rebuild the tabs designer around the Beep-owned page model
- [ ] Manually verify BeepTabs designer save/reopen/run persistence for page add, page remove, child-control add/remove, and intentionally empty tab sets
- [ ] Finalize accessibility, focus, RTL, high-contrast, and touch behavior
- [ ] Expand the tabs demo/sample surface for commercial scenarios
- [ ] Create a tabs regression matrix covering runtime, designer, and performance checks
- [ ] Keep roadmap, README, samples, and tracker aligned after each implementation wave

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

---

## DocumentHost Polish Layer (Commercial Parity)

*Goal: close the remaining gap between BeepDocumentHost and DevExpress/Krypton quality.*
*Plans detail: `TheTechIdea.Beep.Winform.Controls/DocumentHost/.plans/`*

### Phase 1 — Design-Time UX Completion (G2, G3, G8)

- [x] **G2** — Add `DesignerActionPropertyItem` to `DocumentHostActionList` for `TabStyle`, `TabPosition`, `CloseMode`, `ShowAddButton`, `KeyboardShortcutsEnabled`
- [x] **G2** — Add smart-tag quick actions: "Add Document", "Clear All Documents", "Copy Layout Snapshot"
- [x] **G3** — Override `CanParent(Control, Type)` in `BeepDocumentHostDesigner` (accept any `Control`)
- [x] **G3** — Override `OnDragDrop` in `BeepDocumentHostDesigner` → route dropped control to active document area
- [x] **G8** — Add designer verbs: "Export Layout Snapshot…", "Clear All Documents", "Customize Keyboard Shortcuts…"

### Phase 2 — Drag Orchestration Polish

- [x] Theme-aware ghost window — replace `Color.FromArgb(48, 54, 70)` with `_currentTheme?.TabActiveBackColor` in `BeepDocumentTabStrip.Mouse.cs`
- [x] Ghost size matches tab width (~200 px wide, 36 px tall) instead of hardcoded 140×28
- [x] Escape key cancels drag-to-float (handle in `OnKeyDown` while `_dragFloating || _dragging`)
- [ ] Raise `TabFloatDragStarted` event when `_dragFloating` becomes true → activates `BeepDocumentDockOverlay`
- [x] Paint 2 px vertical insert-caret at `_dragInsertIndex` in `BeepDocumentTabStrip.Painting.cs`

### Phase 3 — Keyboard Shortcut Completions

- [x] `Ctrl+Alt+Left` — move active tab to previous split group (`MoveActiveDocumentToAdjacentGroup(-1)`)
- [x] `Ctrl+Alt+Right` — move active tab to next split group (`MoveActiveDocumentToAdjacentGroup(+1)`)
- [x] `Ctrl+Shift+W` — close all tabs to the right of the active tab
- [x] `Ctrl+Shift+M` — maximize / restore active document panel

### Phase 4 — Auto-Hide Flyout Polish

- [x] Add 28 px themed header panel inside `_ahOverlay` (title label + pin button + close button)
- [x] Pin button calls `RestoreAutoHideDocument(documentId)`
- [x] Close button calls `CloseAhOverlay(animate: true)`
- [x] Apply `_currentTheme?.PanelBackColor` / `PanelForeColor` to header
- [x] Subscribe to focus-loss (`Leave` event on `_ahOverlay`); auto-collapse after 600 ms debounce

### Phase 5 — Sample Form + Animation

- [x] Expand `MainFrm_MDI.cs` with `BeepDocumentHost` filling client area + `AutoSaveLayout`
- [x] Add "Add Document" button wired to `AddDocument`, `ActiveDocumentChanged` updates form title
- [x] Replace linear lerp with ease-in-out cubic `t*t*(3-2*t)` in tab open/close animation
- [x] Verify indicator-slide easing (quadratic ease-out confirmed correct — no change needed)

### Phase 6 — Designer Validation (Track B)

- [ ] Properties window shows only categorized groups, no *Misc*
- [ ] Smart-tag inline pickers change designer state immediately
- [ ] Toolbox drag onto host → drops into first document area
- [ ] "Export Layout Snapshot…" verb exports valid JSON
- [ ] Designer reopen restores from `DesignTimeLayoutJson` without crash
- [ ] Delete host → no orphaned child controls remain

