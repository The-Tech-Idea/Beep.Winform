# MASTER TODO TRACKER - DocumentHost MDI Commercial Program

## Program Metadata
- Program: `DocumentHost MDI Commercial-Grade Enhancement`
- Repo: `Beep.Winform`
- Scope: `TheTechIdea.Beep.Winform.Controls/DocumentHost`
- Product Goal: make `BeepDocumentHost` the default MDI and docking surface for Beep WinForms.
- Principle: easy by default, advanced by configuration.
- Current Readiness: `85-88% commercial-grade runtime / 60-65% design-time UX` (design-time is the critical gap).
- Detail Plans:
	- `TheTechIdea.Beep.Winform.Controls/DocumentHost/.plans/README.md`
	- `TheTechIdea.Beep.Winform.Controls/DocumentHost/.plans/CURRENT-STATE-AUDIT.md`
	- `TheTechIdea.Beep.Winform.Controls/DocumentHost/.plans/COMMERCIAL-REFERENCE-NOTES.md`
	- `TheTechIdea.Beep.Winform.Controls/DocumentHost/.plans/IMPLEMENTATION-ROADMAP.md`
- Tracking IDs: `DOCMDI-P{phase}-{nnn}`
- Status Workflow: `NotStarted` -> `InProgress` -> `Review` -> `Done` or `Blocked`

## Completed Baseline
- [x] `DOCMDI-P0-001` Tab-strip styles, overflow, MRU, badges, preview, and quick switch exist.
- [x] `DOCMDI-P0-002` Layout tree, split groups, float windows, dock-back, and auto-hide infrastructure exist.
- [x] `DOCMDI-P0-003` Layout persistence, MVVM and data-binding, options, command service, and vNext contract stubs exist.
- [x] `DOCMDI-P0-004` Initial designer hardening exists for handle creation, dispose, remove, and reparent paths.
- [x] `DOCMDI-P0-005` Command palette, chord shortcuts, tab context menu, and batch add or close flows already exist.
- [x] `DOCMDI-P0-006` Layout validation, repair helpers, and restore-report foundations already exist.

## Phase 1 - 1.0 Blockers
- [x] `DOCMDI-P1-001` Finish tab-strip icon rendering through `StyledImagePainter`.
- [x] `DOCMDI-P1-002` Add `OpenDocument` and `OpenOrActivate` API with duplicate-safe behavior.
- [x] `DOCMDI-P1-003` Add public target-group options instead of primary-group-only defaults.
- [x] `DOCMDI-P1-004` Add first-class dock-state model and dock-state changed notifications.
- [x] `DOCMDI-P1-005` Add restore callback and clearer restore diagnostics for missing or deferred content.

## Phase 2 - Stability And Designer Gate
- [ ] `DOCMDI-P2-001` Route all document and group mutations through one internal mutation scope and coordinator.
- [ ] `DOCMDI-P2-002` Complete designer teardown validation for add, remove, delete, reparent, duplicate, and reopen scenarios.
- [ ] `DOCMDI-P2-003` Add deterministic regression scenarios for float, dock-back, split collapse, repeated save or load, and layout repair.
- [ ] `DOCMDI-P2-004` Document safe design-time behavior and known restrictions.
- [ ] `DOCMDI-P2-005` Verify representative sample forms for runtime and designer parity.

Progress note:
- Focused source audit covered `TheTechIdea.Beep.Winform.Default.Views/MainFrm_MDI.Designer.cs` as the representative container-host MDI surface.
- Automated regression coverage now exists for design-mode authored-child remove/reparent, multi-group restore, and floated-document restore.
- Manual Visual Studio designer validation is still required before closing `DOCMDI-P2-002` and `DOCMDI-NEXT-005`.

## Phase 3 - Shell UX And Discoverability
- [ ] `DOCMDI-P3-001` Add default Window-menu builder.
- [ ] `DOCMDI-P3-002` Extend context menus for document, group, and float-window surfaces.
- [ ] `DOCMDI-P3-003` Add policy controls for close, pin, float, split, and move actions.
- [ ] `DOCMDI-P3-004` Expose command-registry keybinding customization.
- [ ] `DOCMDI-P3-005` Add sample host form that demonstrates the intended one-host MDI story.

## Phase 4 - Persistence, Workspaces, And Identity
- [ ] `DOCMDI-P4-001` Add stable persistence keys separate from display titles.
- [ ] `DOCMDI-P4-002` Persist previous docking context or previous group hints where useful.
- [ ] `DOCMDI-P4-003` Add document-factory callbacks for lazy recreation and missing-content fallback.
- [ ] `DOCMDI-P4-004` Add recent workspaces and session history on top of `SessionFile`.
- [ ] `DOCMDI-P4-005` Keep cloud sync behind a stable local-workspace model.

## Phase 5 - Scale, Accessibility, And Optional Extensions
- [ ] `DOCMDI-P5-001` Add performance baselines for 25, 50, and 100 document scenarios.
- [ ] `DOCMDI-P5-002` Expand accessibility beyond the tab strip to groups, float windows, and menus.
- [ ] `DOCMDI-P5-003` Add high-contrast, keyboard-only, and screen-reader checklists.
- [ ] `DOCMDI-P5-004` Add breadcrumb, status surfaces, and navigation history only after the core shell is stable.
- [ ] `DOCMDI-P5-005` Treat cloud sync, terminal, diff, and git integrations as optional post-core features.

## Phase 6 - Commercial Polish Parity
- [ ] `DOCMDI-P6-001` Finish richer drag orchestration with dock-compass previews, cross-group drop targeting, and clearer drag policies.
- [ ] `DOCMDI-P6-002` Expand keyboard coverage with context-aware navigation, shortcut help, and keybinding customization.
- [ ] `DOCMDI-P6-003` Deepen theming and animation for dock overlays, flyouts, palette surfaces, and state transitions.
- [ ] `DOCMDI-P6-004` Polish auto-hide into a more flyout-like shell with delayed hover behavior and stronger close/back policies.
- [ ] `DOCMDI-P6-005` Validate the representative MDI samples for runtime, designer, and layout-persistence parity.

## Phase 7 - Design-Time First (NEW — Highest User-Facing Priority)

*Goal: "easy like DevExpress where I control everything in design time."*
*Every property must be in Properties window. Smart-tag must have inline pickers.*
*Drop-onto-document must work. Policies must exist. Window menu out of the box.*

- [x] `DOCMDI-P7-001` Add `[Category]`, `[Description]`, `[DefaultValue]` to ALL properties in `BeepDocumentHost.Properties.cs`. Use 6 groups: `"Document – Appearance"`, `"Document – Behavior"`, `"Document – Layout"`, `"Document – Policies"`, `"Document – Persistence"`, `"Document – Animation"`.
- [x] `DOCMDI-P7-002` Add `DesignerActionPropertyItem` entries to `DocumentHostActionList` for TabStyle, TabPosition, CloseMode, ShowAddButton, KeyboardShortcutsEnabled. Add method items: "Add Document", "Clear All Documents", "Save Layout Snapshot…".
- [x] `DOCMDI-P7-003` Add designer verbs to `BeepDocumentHostDesigner`: "Export Layout Snapshot…", "Clear All Documents", "Customize Keyboard Shortcuts…".
- [x] `DOCMDI-P7-004` Override `CanParent()` and `OnDragDrop()` in `BeepDocumentHostDesigner` so dropping a WinForms control from the toolbox routes it to the active document panel's `Controls` collection.
- [x] `DOCMDI-P7-005` Add global policy properties to `BeepDocumentHost.Properties.cs`: `AllowFloat`, `AllowSplit`, `AllowPin`, `AllowAutoHide`, `MaxSplitDepth` (default 4), `CloseButtonShowMode` (enum: Always/OnHover/ActiveOnly/Never/Hidden). Wire into context menus and drag-to-float logic.
- [x] `DOCMDI-P7-006` Add `PopulateWindowMenu(ToolStripMenuItem)` and `AttachWindowMenu(MenuStrip, string)` helpers with standard MDI Window menu items (New Tab Group, Move To Group, list of open docs).
- [x] `DOCMDI-P7-007` Add `PersistenceKey` (GUID string, set once on creation) to `DocumentDescriptor`. Use it as primary key in `SaveLayout` / `RestoreLayout` instead of document title.
- [x] `DOCMDI-P7-008` Add `AutoHideHoverDelay` property (default 400 ms). In `BeepDocumentHost.AutoHide.cs`, add hover-timer: start on MouseMove over strip entry, cancel on MouseLeave, open flyout on tick. 0 = click-only.
- [x] `DOCMDI-P7-009` Wire `BeepLayoutUndoRedo.Undo()` / `Redo()` to existing Ctrl+Z / Ctrl+Y keyboard routing in `BeepDocumentHost.Keyboard.cs` / `BeepDocumentHost.Events.cs`.

## Immediate Next Slice
- [x] `DOCMDI-NEXT-001` Finish tab icon rendering.
- [x] `DOCMDI-NEXT-002` Add `DocumentOpenOptions` and `OpenOrActivate`.
- [x] `DOCMDI-NEXT-003` Remove primary-group bias from public open flows.
- [x] `DOCMDI-NEXT-004` Add dock-state and restore-callback contracts.
- [ ] `DOCMDI-NEXT-005` Validate designer delete, remove, and restore on representative host forms.
- [ ] `DOCMDI-NEXT-006` Close the commercial polish slice: drag orchestration, keyboard reach, theming/animation, flyout auto-hide, and sample-form validation.
- [x] `DOCMDI-NEXT-007` Implement Phase 7 Design-Time First items P7-001 through P7-009 (properties categories, smart-tag pickers, drop-onto-document, policy props, Window menu, PersistenceKey, hover-timer, undo-redo wiring).
- [x] `DOCMDI-NEXT-008` Wire AllowFloat/Split/Pin/AutoHide policy props into context menu items and FloatDocument/AutoHideDocument guards (Track C).
- [x] `DOCMDI-NEXT-009` LayoutSerializationCallback model complete: RestoreDocumentFactory, LayoutRestoring event with Cancel; PersistenceKey added to TabLayoutEntryDto + DocumentLayoutEventArgs (Track D).
- [x] `DOCMDI-NEXT-010` Named workspace support: SaveWorkspace/LoadWorkspace/DeleteWorkspace/ListWorkspaces built on SessionFile base path (Track D).
- [x] `DOCMDI-NEXT-011` Extend context menu: Auto-Hide item (PolicyAllowAutoHide), TabAutoHideRequested event wired end-to-end to host (Track C).
- [x] `DOCMDI-NEXT-012` G7 — Remember last auto-hide size: `_ahLastSize` dictionary stores last flyout width/height per document; restored on next ShowAhOverlay(); captured in FinaliseAhClose() (Track G).
- [x] `DOCMDI-NEXT-013` Undo/redo coverage — PushUndoState() added to FloatDocument, DockBackDocument, AutoHideDocument, RestoreAutoHideDocument entry points (all key layout mutations now snapshot before change).
- [x] `DOCMDI-NEXT-014` Persist previous docking context: `PreviousGroupId` now tracked on float/auto-hide/move operations and serialized in layout entries for intelligent re-dock hints (Track D).
- [x] `DOCMDI-NEXT-015` Extend shell context menus: empty-area group actions (new horizontal/vertical group, close all in group) wired for primary and secondary groups; float-window context menu now includes Dock Back and Close (Track C).
- [x] `DOCMDI-NEXT-016` Designer verb now opens a real keyboard shortcut editor dialog backed by `BeepCommandRegistry` (replaces informational message box placeholder) (Track C).

## Cross-Cutting Risks
- [ ] `RISK-001` Primary-group assumptions may leak into advanced split and group scenarios.
- [ ] `RISK-002` Layout tree and runtime group state can drift under nested docking changes.
- [ ] `RISK-003` Layout schema evolution may break persisted workspaces without stable identity and restore callbacks.
- [ ] `RISK-004` Designer-safe guards may diverge from runtime behavior if not regression tested.
- [ ] `RISK-005` Advanced optional features may obscure the core MDI story if added too early.
- [ ] `RISK-006` Shell UX may remain hard to discover if keyboard-only features outpace menus and policies.

## Blocked
- None.

## Done
- Use the "Completed Baseline" section above for already-landed capabilities; move only fully validated work here after release review.
