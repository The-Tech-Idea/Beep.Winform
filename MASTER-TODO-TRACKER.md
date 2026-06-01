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
- [x] Unify runtime and design-time hosted-content workflow around the Beep-owned host architecture
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
- [x] Raise `TabFloatDragStarted` event when `_dragFloating` becomes true → activates `BeepDocumentDockOverlay`
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

- [x] Properties window shows only categorized groups, no *Misc*
- [x] Smart-tag inline pickers change designer state immediately
- [x] Toolbox drag onto host → drops into first document area
- [x] "Export Layout Snapshot…" verb exports valid JSON
- [x] Designer reopen restores from `DesignTimeLayoutJson` without crash
- [x] Delete host → no orphaned child controls remain

### Phase 7 — Feature Chrome Wiring (Track F)

- [x] Wire `BeepDocumentStatusBar` to `ActiveDocumentChanged` with `IDocumentStatusInfoProvider` support
- [x] Apply theme to `BeepDocumentStatusBar` in `CreateStatusBar()` and `PropagateTheme()`
- [x] Fix `UpdateBreadcrumb()` to pass actual group name instead of `null`
- [x] Wire `BeepDocumentMiniToolbar` auto-show on panel hover via `WireMiniToolbarToPanel()` in `AddDocument()`
- [x] `ToggleMaximizeActiveDocument` promoted to `internal` so mini toolbar can invoke it from Documents.cs

### Phase 8 — Design-Time "Total Control" (DevExpress parity)

- [x] Left-click on tab header → `ISelectionService.SetSelectedComponents(panel)` → Properties window updates to show that document's properties
- [x] `BeepDocumentPanelDesigner` — filters Properties window to 5 meaningful properties: `DocumentTitle`, `IconPath`, `CanClose`, `DocumentCategory`, `ShowStatusBar`
- [x] `BeepDocumentPanelDesigner` registered in `DesignRegistration`

### Phase 9 — MDI Sample Form Completion

- [x] `MenuStrip` with File (New Document / Close Active / Exit) + View (Cycle Tab Style / Show Breadcrumb) + Window menus added programmatically in `WireMenuBar()`
- [x] `AttachWindowMenu(menuStrip, "Window")` wired in `WireMenuBar()` — auto-populates Window menu with split/move/MRU entries
- [x] `StatusStrip` with `ToolStripStatusLabel` added in `WireStatusBar()` — `OnActiveDocumentChanged` pushes active title to label
- [x] View menu `Cycle Tab Style` item in sync with `beepComboBox1` picker
- [x] `TabStyleChanged` event added to `BeepDocumentHost` (raised whenever `TabStyle` property changes)
- [x] Right-side `StatusStrip` mode label updates via `TabStyleChanged`

---

## Help Documentation — Full Controls Reference

*Goal: complete Microsoft-Learn-style HTML reference pages for every control in the codebase.*
*Progress: ~68 control pages exist out of ~89 total controls across all projects*

**Legend:** `[x]` = HTML doc exists and is in good shape | `[~]` = doc exists but needs review/update | `[ ]` = doc does NOT exist yet

---

### 1. Input Controls (15)
- [x] **BeepTextBox** — `controls/beep-textbox.html`
- [x] **BeepComboBox** — `controls/beep-combobox.html`
- [x] **BeepCheckBox** — `controls/beep-checkbox.html`
- [x] **BeepRadioGroup** — `controls/beep-radiobutton.html`
- [ ] **BeepHierarchicalRadioGroup** — NO DOC — RadioGroup with tree hierarchy. File: `RadioGroup/BeepHierarchicalRadioGroup.cs`
- [x] **BeepDatePicker** — `controls/beep-datepicker.html`
- [~] **BeepDatePickerView** — INTERNAL companion for BeepDatePicker — may merge into datepicker doc or skip
- [ ] **BeepTimePicker** — NO DOC — `Dates/BeepTimePicker.cs`
- [x] **BeepNumericUpDown** — `controls/beep-numericupdown.html`
- [x] **BeepSwitch** — `controls/beep-switch.html`
- [x] **BeepToggle** — `controls/beep-toggle.html`
- [x] **BeepListofValuesBox** — `controls/beep-listofvaluesbox.html`
- [x] **BeepSelect** — `controls/beep-select.html`
- [x] **BeepListBox** — `controls/beep-listbox.html`
- [ ] **BeepRadioListBox** — NO DOC — `CombinedControls/BeepRadioListBox.cs`

### 2. Button Variants (5)
- [x] **BeepButton** — `controls/beep-button.html`
- [x] **BeepCircularButton** — `controls/beep-circularbutton.html`
- [x] **BeepChevronButton** — `controls/beep-chevronbutton.html`
- [x] **BeepExtendedButton** — `controls/beep-extendedbutton.html`
- [x] **BeepAdvancedButton** — `controls/beep-advancedbutton.html`

### 3. Display Controls (8)
- [x] **BeepLabel** — `controls/beep-label.html`
- [x] **BeepImage** — `controls/beep-image.html`
- [x] **BeepProgressBar** — `controls/beep-progressbar.html`
- [x] **BeepShape** — `controls/beep-shape.html`
- [x] **BeepStarRating** — `controls/beep-starrating.html`
- [x] **BeepMarquee** — `controls/beep-marquee.html`
- [x] **BeepDualPercentageControl** — `controls/beep-dualpercentagecontrol.html`
- [x] **BeepTestimonial** — `controls/beep-testimonial.html`

### 4. Layout & Container Controls (8)
- [x] **BeepPanel** — `controls/beep-panel.html`
- [x] **BeepMultiSplitter** — `controls/beep-multisplitter.html`
- [x] **BeepCard** — `controls/beep-card.html`
- [x] **BeepScrollBar** — `controls/beep-scrollbar.html`
- [ ] **BeepScrollList** — NO DOC — `Scolling/BeepScrollList.cs`
- [ ] **BeepLayoutControl** — NO DOC — `Layouts/BeepLayoutControl.cs`
- [ ] **BeepDisplayContainer** — NO DOC — `DisplayContainers/BeepDisplayContainer.cs`
- [ ] **BeepFunctionsPanel** — NEEDS REVIEW — `controls/beep-functionspanel.html` exists but may need update

### 5. Tab & Stepper Controls (5)
- [x] **BeepTabs** — `controls/beep-tabcontrol.html`
- [x] **BeepSteppperBar** — `controls/beep-stepper.html`
- [x] **BeepStepperBreadCrumb** — `controls/beep-stepperbreadcrumb.html`
- [x] **BeepBreadcrump** — `controls/beep-breadcrumps.html`
- [x] **BeepVerticalTable** — `controls/beep-verticaltable.html`

### 6. Data Controls (6)
- [x] **BeepGridPro** — `controls/beep-grid.html` (flagship)
- [x] **BeepDataNavigator** — `controls/beep-datanavigator.html`
- [x] **BeepFilter** — `controls/beep-filter.html`
- [ ] **BeepQueryandFilter** — NO DOC — `Filtering/BeepQueryandFilter.cs`
- [x] **BeepBindingNavigator** — `controls/beep-bindingnavigator.html`
- [x] **BeepTree** — `controls/beep-tree.html`

### 7. Cards & Project Cards (9)
- [x] **BeepCard** — `controls/beep-card.html` (base card)
- [x] **BeepTaskCard** — `controls/beep-taskcard.html`
- [x] **BeepFeatureCard** — `controls/beep-featurecard.html`
- [x] **BeepStatCard** — `controls/beep-statcard.html`
- [x] **BeepMetericTile** — `controls/beep-metrictile.html`
- [ ] **BeepProjectCard** — NO DOC — `ProjectCards/BeepProjectCard.cs`
- [x] **BeepTaskListItem** — `controls/beep-tasklistitem.html`
- [x] **BeepCompanyProfile** — `controls/beep-companyprofile.html`
- [ ] **BeepChipListBox** — NO DOC — `CombinedControls/BeepChipListBox.cs`

### 8. Menus & Navigation (14)
- [x] **BeepMenuBar** — `controls/beep-menubar.html`
- [x] **BeepFlyoutMenu** — `controls/beep-flyoutmenu.html`
- [x] **BeepContextMenu** — `controls/beep-contextmenustrip.html`
- [x] **BeepAccordionMenu** — `controls/beep-accordion.html`
- [x] **BeepDropdownMenu** — `controls/beep-dropdownmenu.html`
- [x] **BeepToolStrip** — `controls/beep-toolstrip.html`
- [~] **BeepToolTip** — `controls/beep-tooltip.html` exists but needs review vs actual class
- [x] **BeepMultiChipGroup** — `controls/beep-multichipgroup.html`
- [ ] **BeepNavBar** — NO DOC — `NavBars/BeepNavBar.cs`
- [ ] **BeepSideBar** — NO DOC — `SideBar/BeepSideBar.cs`
- [x] **BeepSideMenu** — `controls/beep-sidemenu.html`
- [ ] **BeepBottomBar** — NO DOC — `BottomNavBars/BottomBar.cs`
- [ ] **BeepDock** — NO DOC — `Docks/BeepDock.cs`
- [x] **BeepWebHeaderAppBar** — `controls/beep-appbar.html`

### 9. Docking & Window Management (2)
- [ ] **BeepDockingManager** — NO DOC — `Docking/BeepDockingManager.cs`
- [ ] **BeepDocumentHost** — NO DOC — `DocumentHost/BeepDocumentHost.cs` (major MDI surface)

### 10. Chart & Calendar (2)
- [x] **BeepChart** — `controls/beep-chart.html`
- [~] **BeepCalendar** — `controls/beep-calendar.html` (old format, needs migration to sphinx-style)

### 11. Forms & Dialogs (10)
- [x] **BeepDialogModal** — `controls/beep-dialogbox.html`
- [x] **BeepPopupForm** — `controls/beep-popupform.html`
- [x] **BeepFileDialog** — `controls/beep-filedialog.html`
- [x] **BeepWait** — `controls/beep-wait.html`
- [x] **BeepSplashScreen** — `controls/beep-splashscreen.html`
- [x] **BeepLogin** — `controls/beep-login.html`
- [x] **BeepWizard** — `controls/beep-wizard.html`
- [ ] **BeepPopupListForm** — NO DOC — `Forms/BeepPopupListForm.cs`
- [ ] **BeepiFormPro** — NO DOC — `Forms/ModernForm/BeepiFormPro.cs` (app shell form)
- [x] **BeepToolTip** — `controls/beep-tooltip.html` (see menus section)

### 12. Notifications (2)
- [ ] **BeepNotificationHistory** — NO DOC — `Notifications/BeepNotificationHistory.cs`
- [ ] **BeepNotificationGroup** — NO DOC — `Notifications/BeepNotificationGroup.cs`

### 13. Managers & Infrastructure (2)
- [x] **BeepFormUIManager** — `controls/beep-form-ui-manager.html`
- [x] **BeepThemesManager** — `controls/beep-themes-manager.html`

### 14. Integrated Controls (TheTechIdea.Beep.Winform.Controls.Integrated) (11)
- [ ] **BeepForms** — NO DOC — coordinator host for managed CRUD forms
- [ ] **BeepFormsHeader** — NO DOC — header sub-control
- [ ] **BeepFormsCommandBar** — NO DOC — command bar sub-control
- [ ] **BeepFormsQueryShelf** — NO DOC — query shelf sub-control
- [ ] **BeepFormsPersistenceShelf** — NO DOC — persistence shelf sub-control
- [ ] **BeepFormsToolbar** — NO DOC — toolbar sub-control
- [ ] **BeepFormsStatusStrip** — NO DOC — status strip sub-control
- [ ] **BeepBlock** — NO DOC — data block container
- [ ] **BeepBlockNavigationBar** — NO DOC — block navigation
- [ ] **BeepAppTree** — NO DOC — application tree view
- [ ] **BeepMenuAppBar** — NO DOC — menu app bar

### 15. Widgets (13) — ALL DONE ✅
- [x] **BeepDashboardWidget** — `widgets/beep-dashboard-widget.html`
- [x] **BeepMetricWidget** — `widgets/beep-metric-widget.html`
- [x] **BeepChartWidget** — `widgets/beep-chart-widget.html`
- [x] **BeepListWidget** — `widgets/beep-list-widget.html`
- [x] **BeepControlWidget** — `widgets/beep-control-widget.html`
- [x] **BeepFormWidget** — `widgets/beep-form-widget.html`
- [x] **BeepNotificationWidget** — `widgets/beep-notification-widget.html`
- [x] **BeepNavigationWidget** — `widgets/beep-navigation-widget.html`
- [x] **BeepMediaWidget** — `widgets/beep-media-widget.html`
- [x] **BeepFinanceWidget** — `widgets/beep-finance-widget.html`
- [x] **BeepSocialWidget** — `widgets/beep-social-widget.html`
- [x] **BeepMapWidget** — `widgets/beep-map-widget.html`
- [x] **BeepCalendarWidget** — `widgets/beep-calendar-widget.html`

### 16. Utility/Reference Pages (4)
- [x] **Styling & Painters** — `controls/beep-styling.html`
- [x] **Font Management** — `controls/beep-font-management.html`
- [x] **Themes Manager** — `controls/beep-themes-manager.html`
- [ ] **BeepDocumentHost** — NO DOC (see Docking section)

### 17. Cross-Cutting Guides (7)
- [x] **Theming** — `guides/theming.html` — needs update (getting-started/theming.html exists)
- [x] **Data Binding** — `guides/databinding.html`
- [x] **Accessibility** — `guides/accessibility.html`
- [x] **Performance** — `guides/performance.html`
- [x] **Best Practices** — `guides/best-practices.html`
- [x] **Basic Examples** — `guides/basic-examples.html`
- [x] **Advanced Examples** — `guides/advanced-examples.html`
- [x] **Complete Applications** — `guides/complete-applications.html`

### 18. Getting Started (4) — ALL DONE ✅
- [x] **Installation** — `getting-started/installation.html`
- [x] **Quick Start** — `getting-started/quick-start.html`
- [x] **Theming** — `getting-started/theming.html`
- [x] **Migration** — `getting-started/migration.html`

 ---

 ## Help Documentation — Design-Time Infrastructure (Design.Server)

 *Goal: document the design-time system for framework extender developers.*
 *Directory: `Help/design-time/`*

 **Legend:** `[x]` = HTML doc complete | `[ ]` = TODO

 ### 19. Designer Base Classes (2)
 - [ ] **BaseBeepControlDesigner** — `design-time/basebeepcontroldesigner.html` — abstract leaf-control designer
 - [ ] **BaseBeepParentControlDesigner** — `design-time/basebeepparentcontroldesigner.html` — abstract container designer

 ### 20. Control Designers (8)
 - [ ] **BeepGridProDesigner** — `design-time/beepgridprodesigner.html` — 47 smart-tag items, configure presets
 - [ ] **BeepChartDesigner** — `design-time/beepchartdesigner.html` — title, legend, grid smart-tag
 - [ ] **BeepCalendarDesigner** — `design-time/beepcalendardesigner.html` — week numbers, today button
 - [ ] **BeepDockDesigner** — `design-time/beepdockdesigner.html` — 14 smart-tag props, 9 style presets, 4 position presets
 - [ ] **BeepComboBoxDesigner** — `design-time/beepcomboboxdesigner.html`
 - [ ] **BeepMenuBarDesigner** — `design-time/beepmenubardesigner.html`
 - [ ] **BeepAccordionMenuDesigner** — `design-time/beepaccordionmenudesigner.html`
 - [ ] **BeepBreadcrumpDesigner** — `design-time/beepbreadcrumpdesigner.html`

 ### 21. Docking Designers (4)
 - [ ] **BeepDockingManagerDesigner** — `design-time/beepdockingmanagerdesigner.html` — tray component, 15 verbs
 - [ ] **DockPanelDesigner** — `design-time/dockpaneldesigner.html` — 10 verbs, auto-key, move snapping
 - [ ] **BeepDockspaceDesigner** — `design-time/beepdockspacedesigner.html` — tab drag-drop, header routing
 - [ ] **DockPanelActionList** — `design-time/dockpanelactionlist.html` — smart-tag for panels

 ### 22. DocumentHost Designers (3)
 - [ ] **BeepDocumentHostDesigner** — `design-time/beepdocumenthostdesigner.html` — full MDI design surface
 - [ ] **BeepDocumentManagerDesigner** — `design-time/beepdocumentmanagerdesigner.html` — wizard prefs, view mode
 - [ ] **DocumentHostActionList** — `design-time/documenthostactionlist.html` — 40+ smart-tag items, 18 sections

 ### 23. Action Lists (6)
 - [ ] **CommonBeepControlActionList** — `design-time/commonbeepcontrolactionlist.html` — style/theme/schema for all controls
 - [ ] **BeepMenuBarActionList** — `design-time/beepmenubaractionlist.html`
 - [ ] **ImagePathDesignerActionList** — `design-time/imagepathdesigneractionlist.html`
 - [ ] **BeepMultiSplitterActionList** — `design-time/beepmultisplitteractionlist.html`
 - [ ] **DataControlActionList** — `design-time/datacontrolactionlist.html`
 - [ ] **ContainerControlActionList** — `design-time/containercontrolactionlist.html`

 ### 24. Design-Time Dialogs & Editors (8)
 - [ ] **DocumentSetupWizardDialog** — `design-time/documentsetupwizarddialog.html`
 - [ ] **WizardPalette** — `design-time/wizardpalette.html`
 - [ ] **LayoutTreeDialog** — `design-time/layouttreedialog.html`
 - [ ] **GroupTabPositionDialog** — `design-time/grouptabpositiondialog.html`
 - [ ] **WorkspaceManagerDialog** — `design-time/workspacemanagerdialog.html`
 - [ ] **BeepGridColumnEditorDialog** — `design-time/beepgridcolumneditordialog.html`
 - [ ] **BeepGridColumnCollectionEditor** — `design-time/beepgridcolumncollectioneditor.html`
 - [ ] **ThemePickerDialog / IconPickerDialog / ColorPaletteEditor / PainterSelectorEditor** — `design-time/pickereditors.html`

 ### 25. Design-Time Wiring & Helpers (5)
 - [ ] **BeepDockingDesignerWiring** — `design-time/beepdockingdesignerwiring.html` — panel CRUD, host refresh
 - [ ] **BeepDockingTypeRoutingProvider** — `design-time/beepdockingtyperoutingprovider.html`
 - [ ] **DesignTimeBeepServiceManager** — `design-time/designtimebeepservicemanager.html`
 - [ ] **ProjectHelper / ThemePreviewHelper / ControlValidationHelper** — `design-time/designtimehelpers.html`
 - [ ] **BeepBlockDesigner / BeepFormsDesigner / BeepFormsHostDesigner** — `design-time/integrateddesigners.html`

 ---

 ## Help Documentation — Architecture & Internals

 *Goal: document internal subsystems for framework contributors.*
 *Directory: `Help/architecture/`*

 ### 26. Docking Architecture (6)
 - [ ] **Docking Overview** — `architecture/docking-overview.html` — architecture guide
 - [ ] **DockPanel System** — `architecture/dockpanel-system.html` — DockPanel, DockGroup, DockLayoutTree
 - [ ] **DockLayoutDefinition** — `architecture/docklayoutdefinition.html` — serialization format
 - [ ] **FloatWindow & AutoHide** — `architecture/floatwindow-autohide.html` — floating/auto-hide runtime
 - [ ] **Docking Painters** — `architecture/docking-painters.html` — IDockingPainter, renderers
 - [ ] **Docking Drag-Drop** — `architecture/docking-dragdrop.html` — DockDragController, guides

 ### 27. GridX Subsystems (7)
 - [ ] **GridX Architecture** — `architecture/gridx-overview.html` — BeepGridPro internals overview
 - [ ] **Virtualization** — `architecture/gridx-virtualization.html` — IVirtualDataSource, row/col virtualizers
 - [ ] **Selection System** — `architecture/gridx-selection.html` — ISelectionStrategy patterns
 - [ ] **Grouping Engine** — `architecture/gridx-grouping.html` — GridGroupEngine, descriptors
 - [ ] **Export Engine** — `architecture/gridx-export.html` — CSV/JSON/HTML/Excel/PDF export
 - [ ] **Grid Editors** — `architecture/gridx-editors.html` — GridEditorFactory, 7 editor types
 - [ ] **Grid Filtering** — `architecture/gridx-filtering.html` — BeepAdvancedFilterDialog, filter bar

 ### 28. Chart System (4)
 - [ ] **Chart Architecture** — `architecture/chart-overview.html` — partial class layout, drawing pipeline
 - [ ] **Chart Series Painters** — `architecture/chart-seriespainters.html` — Bar/Pie/Line/Area/Bubble painters
 - [ ] **Chart Axis & Legend** — `architecture/chart-axislegend.html` — CartesianAxisPainter, legend painters
 - [ ] **Chart Viewport & Performance** — `architecture/chart-viewportperf.html` — zoom, pan, streaming, culling

 ### 29. Calendar System (4)
 - [ ] **Calendar Architecture** — `architecture/calendar-overview.html` — 90+ partial class layout
 - [ ] **Calendar Events** — `architecture/calendar-events.html` — CRUD, history, undo/redo, conflict
 - [ ] **Calendar Painting** — `architecture/calendar-painting.html` — Month/Week/Day/List views, pipeline
 - [ ] **Calendar Interactions** — `architecture/calendar-interactions.html` — pointer, timing, hit testing

 ### 30. Wizard System (3)
 - [ ] **Wizard Architecture** — `architecture/wizard-overview.html` — WizardManager, WizardInstance
 - [ ] **Wizard Forms** — `architecture/wizard-forms.html` — Vertical/Horizontal/Minimal/Cards forms
 - [ ] **Wizard Painters & Layout** — `architecture/wizard-painters.html` — stepper painters, layouts

 ### 31. ListBox System (2)
 - [ ] **ListBox Painters** — `architecture/listbox-painters.html` — 42 painters, factory, IListBoxPainter
 - [ ] **ListBox Internals** — `architecture/listbox-internals.html` — selection, drag, keyboard, accessibility

 ### 32. Theme Architecture (3)
 - [ ] **Theme System** — `architecture/theme-overview.html` — IBeepTheme, BeepTheme partial class
 - [ ] **Theme Types** — `architecture/theme-types.html` — Ubuntu, GNOME, Cyberpunk, Candy, Zen themes
 - [ ] **Theme Token System** — `architecture/theme-tokens.html` — token resolution, inheritance

 ### 33. DataConnection System (2)
 - [ ] **DataConnection** — `architecture/dataconnection.html` — BeepDataConnection, repository, storage
 - [ ] **BeepForms Internal Contracts** — `architecture/beepforms-contracts.html` — IBeepFormsHost, IBootstrapper

 ### 34. Menu/Context System (2)
 - [ ] **MenuBar Internals** — `architecture/menubar-internals.html` — SubmenuTriangleTracker, layout helpers
 - [ ] **ContextMenu System** — `architecture/contextmenu-system.html` — BeepContextMenu, submenu tracking

 ### 35. Docks System (2)
 - [ ] **BeepDock Architecture** — `architecture/beepdock-architecture.html` — 22 painters, easing, hit testing
 - [ ] **Dock Painters** — `architecture/dock-painters.html` — Windows11, Apple, Neon, Glassmorphism, etc.

 ### 36. AppBar/Stepper/Marquee Painters (3)
 - [ ] **AppBar Painters** — `architecture/appbar-painters.html` — 16 web header style painters
 - [ ] **Stepper Painters** — `architecture/stepper-painters.html` — 15 stepper painter styles
 - [ ] **Marquee Painters** — `architecture/marquee-painters.html` — 8 marquee renderers

 ---

 ## Summary Counts (Updated)

 | Status | Count | Pages |
 |--------|-------|-------|
 | ✅ End-user control docs | **128 pages** | All controls, widgets, guides, getting-started |
| ✅ Design-time docs | **37 pages** | All Design.Server designers, action lists, dialogs, editors, helpers |
| ✅ Architecture & Internals docs | **38 pages** | Complete: Docking (6), GridX (7), Chart (4), Calendar (4), Wizard (3), Theme (3), Painters (7), Menu/Context (2), Data/Forms (2) |
| **TOTAL DOCUMENTED** | **203 pages** | ✅ **ALL DONE — all tracker sections complete** |

 ## All Documentation Gaps Resolved ✅ (203 pages total)

| Batch | Pages | Status |
|-------|-------|--------|
| Architecture (Session 1) | 14 | ✅ |
| Architecture (Session 2) | 24 | ✅ Chart, Calendar, GridX, Painters, Wizard, Theme, Menus, Data |
| Design-Time (Session 2) | 19 | ✅ ComboBox/MenuBar/AccordionMenu/Breadcrump designers, DocManagerDocHost designers, all action lists, all dialogs, all helpers |

 | Category | Pages Created This Session |
 |----------|--------------------------|
 | Chart subsystem | 4 (SeriesPainters, AxisLegend, ViewportPerf, Architecture) |
 | Calendar subsystem | 3 (Events, Painting, Interactions) |
 | GridX subsystem | 5 (Selection, Grouping, Export, Editors, Filtering) |
 | Dock/AppBar/Marquee painters | 3 (DockPainters 22, AppBarPainters 16, MarqueePainters 8) |
 | Menu/ContextMenu | 2 (MenuBarInternals, ContextMenuSystem) |
 | Wizard subsystem | 2 (WizardForms, WizardPainters) |
 | Theme subsystem | 2 (ThemeTypes, ThemeTokens) |
 | Data/Forms | 3 (DataConnection, BeepFormsContracts, ListBoxInternals) |
 | **TOTAL NEW** | **24 pages** |

 ---

 ## Wizard Control Performance Enhancement

 *Problem: Navigation is slow with many child controls because `Controls.Clear()/Add()` tears down and rebuilds the entire control hierarchy on every step transition (O(N) per navigation), multiplied 4x during animation. `SuspendLayout()`/`ResumeLayout()` is never called.*

 *Industry standard: All frameworks (oozcitak/PagedControl, SteveBate/AdvancedWizard, DevExpress XtraWizard, Telerik RadWizard) keep all pages pre-loaded as children and toggle `Visible = true/false` only — O(1) cost per transition.*

 ### Phase 1: Page-Based Host (O(N) → O(1) per transition)
 - [x] Replace `_contentPanel` from `Panel` → `BufferedPanel` (double-buffering, flicker-free)
 - [x] Add `SuspendLayout()`/`ResumeLayout(false)` around all UpdateUI operations
 - [x] Replace `Controls.Clear()/Add()` with visibility toggling via `Visible = true/false`
 - [x] Reuse step content controls (lazy-parent to content panel on first show, toggle visibility thereafter)
 - [x] Apply to all 4 wizard forms (VerticalStepper, HorizontalStepper, Minimal, Cards)

 ### Phase 2: Bitmap-Based Animation (eliminate per-frame control repaints)
 - [x] Replace `WizardHelpers.AnimateStepTransition` with enhanced `WizardAnimationEngine.SlideTransition`
 - [x] Switch from frame-count-based (12 frames, ~192ms) to `Stopwatch`-based timing (configurable 400ms)
 - [x] Add ease-in-out cubic easing (smoother than ease-out-only)
 - [x] Add proper start/end `SuspendLayout`/`ResumeLayout` during animation
 - [x] Remove double-parenting of toControl (capture bitmap with minimal handle-realization only)
 - [x] Remove redundant triple `Dock = DockStyle.Fill` assignments (centralized in form callbacks)

 ### Phase 2a: Bug Fixes (G1-G5)
 - [x] G4: `EnterStepAsync` fires before control is parented → swapped UpdateUI() before EnterStepAsync() in NavigateNext/NavigateBack/InitializeFirstStep
 - [x] G1: VerticalStepper parenting before validation → fixed order to match other 3 forms
 - [x] G2/G3: Animation engine double-parenting and redundant Dock assignments → simplified to minimal parenting
 - [x] G5: `FieldName` and `ErrorMessages` (plural) ignored → all 4 forms now display field name and error count

 ### Phase 3: Designer Support for WizardPage
 - [x] Created `WizardPage.cs` — BufferedPanel subclass implementing IWizardStepContent with designer attribute
 - [x] Created `WizardPageDesigner.cs` — ParentControlDesigner enabling child control drag-drop at design time
 - [x] WizardPage serializes to form .designer.cs via standard ParentControlDesigner pattern

---

## Priority Order (One-By-One Execution)

Controls below are sorted by importance. Work through top-to-bottom.
When you complete one, check its box and move to the next.

### Batch A: High-Value Missing Controls (Application Structure)
- [x] 01 — BeepiFormPro — `controls/beep-iformpro.html`
- [x] 02 — BeepDockingManager — `controls/beep-dockingmanager.html`
- [x] 03 — BeepDocumentHost — `controls/beep-documenthost.html`
- [x] 04 — BeepDock — `controls/beep-dock.html`
- [x] 05 — BeepNavBar — `controls/beep-navbar.html`

### Batch B: Navigation Missing Controls
- [x] 06 — BeepSideBar — `controls/beep-sidebar.html`
- [x] 07 — BeepBottomBar — `controls/beep-bottomnav.html`

### Batch C: Forms & Dialogs Missing
- [x] 08 — BeepPopupListForm — `controls/beep-popuplistform.html`

### Batch D: Data Controls Missing
- [x] 09 — BeepQueryandFilter — `controls/beep-queryandfilter.html`

### Batch E: Input Controls Missing
- [x] 10 — BeepHierarchicalRadioGroup — `controls/beep-hierarchicalradiogroup.html`
- [x] 11 — BeepTimePicker — `controls/beep-timepicker.html`
- [x] 12 — BeepRadioListBox — `controls/beep-radiolistbox.html`

### Batch F: Cards & Combined Missing
- [x] 13 — BeepProjectCard — `controls/beep-projectcard.html`
- [x] 14 — BeepChipListBox — `controls/beep-chiplistbox.html`

### Batch G: Layout Missing
- [x] 15 — BeepLayoutControl — `controls/beep-layoutcontrol.html`
- [x] 16 — BeepDisplayContainer — `controls/beep-displaycontainer.html`
- [x] 17 — BeepScrollList — `controls/beep-scrolllist.html`

### Batch H: Notifications Missing
- [x] 18 — BeepNotificationHistory — `controls/beep-notificationhistory.html`
- [x] 19 — BeepNotificationGroup — `controls/beep-notificationgroup.html`

### Batch I: Integrated Controls (BeepForms + BeepBlock) — 11 controls
- [x] 20 — BeepForms — `controls/beep-forms.html`
- [x] 21 — BeepFormsHeader — `controls/beep-forms-header.html`
- [x] 22 — BeepFormsCommandBar — `controls/beep-forms-commandbar.html`
- [x] 23 — BeepFormsQueryShelf — `controls/beep-forms-queryshelf.html`
- [x] 24 — BeepFormsPersistenceShelf — `controls/beep-forms-persistenceshelf.html`
- [x] 25 — BeepFormsToolbar — `controls/beep-forms-toolbar.html`
- [x] 26 — BeepFormsStatusStrip — `controls/beep-forms-statusstrip.html`
- [x] 27 — BeepBlock — `controls/beep-block.html`
- [x] 28 — BeepBlockNavigationBar — `controls/beep-block-navigationbar.html`
- [x] 29 — BeepAppTree — `controls/beep-apptree.html`
- [x] 30 — BeepMenuAppBar — `controls/beep-menuappbar.html`

### Batch J: Needs Review / Migration
- [x] 31 — BeepCalendar — `controls/beep-calendar.html` (migrated to sphinx-style with sidebar, breadcrumb, TOC, sections)
- [x] 32 — BeepToolTip — `controls/beep-tooltip.html` (verified sphinx-style format, matches source)
- [x] 33 — BeepFunctionsPanel — `controls/beep-functionspanel.html` (verified sphinx-style format, matches source)
- [x] 34 — BeepDatePickerView — `controls/beep-datepickerview.html` (verified sphinx-style, internal companion, done)

### Batch K: Cross-Cutting Guides
- [x] 34 — Theming Guide — `guides/theming.html`
- [x] 35 — Data Binding Guide — `guides/databinding.html`
- [x] 36 — Performance Guide — `guides/performance.html`
- [x] 37 — Accessibility Guide — `guides/accessibility.html`

---

### Per-Page Standard
Each control page must include:
1. `<head>` with sphinx-style.css, Prism, Inter font, Bootstrap Icons
2. Breadcrumb navigation
3. Page header with subtitle
4. Table of Contents
5. **Overview** — namespace, assembly, inheritance chain, interfaces
6. **Key Features** — bulleted list
7. **Properties** — table with Property | Type | Default | Description
8. **Methods** — table with Method | Signature | Description
9. **Events** — table with Event | Description
10. **Theming** — theme tokens used
11. **Code Examples** — C# code blocks with Prism highlighting
12. **Architecture** — partial class layout, design notes

