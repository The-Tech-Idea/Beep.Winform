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

---

## BeepCalendar Commercialization Program (Design-Time)

*Goal: elevate BeepCalendar's design-time surface to BeepGridPro / BeepBlock quality — smart-tag, Property Grid collection editors, right-click verbs, and design-time tests.*

*Supporting designs: `TheTechIdea.Beep.Winform.Controls/Calendar/plans/` (Phase 1-4) plus the phases below.*

### Phase C0 — Stop the Bleeding (Designer Fix)
- [x] Remove dead `ShowWeekNumbers` / `ShowTodayButton` property items from `BeepCalendarActionList.GetSortedActionItems()` — these properties did not exist on `BeepCalendar`
- [x] Remove dead property accessors from the action list class
- [x] Verify: `dotnet build` → 0 errors on both Controls and Design.Server projects

### Phase C1 — Designer-Surface Property Expansion (25 properties)
- [x] Add `BeepCalendar.Core.PublicApi.DesignerSurface.cs` partial with 25 new `[Category("Appearance|Behavior")]` properties
  - [x] Toolbar: `ShowWeekNumbers`, `ShowTodayButton`, `ShowNavigationButtons`, `ShowViewSelector`, `ShowSearchBox`
  - [x] Week config: `FirstDayOfWeek`, `WorkDays` (new `[Flags] DaysOfWeek` enum on `BeepCalendar.Types.cs`)
  - [x] Interaction toggles: `ReadOnly`, `AllowDragCreate`, `AllowEventMove`, `AllowEventResize`
  - [x] Time/date formatting: `TimeFormat`, `DateFormat`
  - [x] Slot gutter: `TimeInterval`, `SlotLabelInterval`, `BusinessHourStart`, `BusinessHourEnd`
  - [x] View layout: `ShowAllDayArea`, `ShowTimeGutter`, `ShowMinutes`, `Show24Hours`
  - [x] Constraints: `MinAppointmentDuration`, `MaxAppointmentDuration`, `SnapToGrid`
  - [x] Highlighting: `HighlightToday`
- [x] Add backing fields to `BeepCalendar.Fields.cs`
- [x] Verify: 0 build errors; all 25 properties discoverable via `TypeDescriptor.GetProperties`

### Phase C2 — Collection Editors (Events, Categories, Resources)
- [x] `Editors/CalendarEventCollectionEditor.cs` — `UITypeEditor` opening `CalendarEventEditorDialog`
- [x] `Editors/CalendarEventEditorDialog.cs` — code-built `Form`; `ListView` (Title|Start|End) + `PropertyGrid`; Add/Remove/Duplicate/OK/Cancel
- [x] `Editors/EventCategoryCollectionEditor.cs` + `EventCategoryEditorDialog.cs` — ListView + PropertyGrid + Add/Remove/MoveUp/MoveDown
- [x] `Editors/CalendarResourceCollectionEditor.cs` + `CalendarResourceEditorDialog.cs` — same pattern for `CalendarResource`
- [x] Wire `[Editor("...Design.Server...", typeof(UITypeEditor))]` on `Events`, `Categories`, `Resources` properties
- [x] Verify: 0 build errors; `[Editor]` attributes discoverable via reflection

### Phase C3 — Smart-Tag Action List Parity
- [x] Quick Configuration: 6 `ConfigureAs*` methods (Month/Week/WorkWeek/Day/Agenda/Timeline) + `ViewMode` picker
- [x] Sample Data: `AddSampleData` (8 events, 3 categories, 2 resources) + `ClearAllData`
- [x] Data Editors: `EditEvents`, `EditCategories`, `EditResources` (open respective collection dialogs)
- [x] Behavior `DesignerActionPropertyItem`: `ReadOnly`, `AllowDragCreate`, `AllowEventMove`, `AllowEventResize`, `SnapToGrid`, `InteractionSnapIntervalMinutes`, `FirstDayOfWeek`, `ConflictPolicyMode`, `DensityMode`
- [x] Appearance `DesignerActionPropertyItem`: `CalendarStyle`, `ShowSidebar`, `ShowTodayButton`, `ShowNavigationButtons`, `ShowViewSelector`, `ShowWeekNumbers`, `HighlightToday`, `ShowAllDayArea`, `ShowTimeGutter`, `ShowMinutes`, `Show24Hours`, `TimeFormat`, `DateFormat`, `TimeInterval`, `BusinessHourStart`, `BusinessHourEnd`
- [x] Theme verbs: `ApplyTheme` (calls `_designer.ApplyTheme()`), `ChooseTheme` (reuses `ThemePickerDialog`), `ChooseStyle` (reuses `StyleSelectorDialog`)
- [x] `ResetToDefaults` method — resets all ~25 properties to their defaults
- [x] Verify: 0 build errors; smart-tag surfaces ~35 items across 6 sections

### Phase C4 — Verbs (Right-Click Context Menu)
- [x] Add `Verbs` override to `BeepCalendarDesigner` with 6 `DesignerVerb` entries:
  - `Edit Events…`, `Edit Categories…`, `Edit Resources…`, `Add Sample Data`, `Clear All Data`, `Reset to Defaults`
- [x] Cached `_actionList` field avoids re-creating action list per verb call
- [x] Verify: 0 build errors; `new BeepCalendarDesigner().Verbs.Count >= 6`

### Phase C5 — Design-Time Tests
- [x] 5 xUnit tests in `TheTechIdea.Beep.Winform.Controls.Tests/Calendar/BeepCalendarDesignerTests.cs`:
  - [x] `BeepCalendar_Events_HasCollectionEditor` — verifies `[Editor]` attribute
  - [x] `BeepCalendar_Categories_HasCollectionEditor` — verifies `[Editor]` attribute
  - [x] `BeepCalendar_Resources_HasCollectionEditor` — verifies `[Editor]` attribute
  - [x] `BeepCalendar_DesignerSurfaceProperties_ExistOnType` — asserts all 20 new properties exist on type
  - [x] `BeepCalendar_DesignTimeEvents_Exist` — asserts all 7 design-time events exist
- [x] All 5 tests pass (0 failed)
- [ ] (Optional) `Samples/BeepCalendarSampleForm.cs` — deferred for future sample project
- [ ] (Optional) Update `Help/controls/beep-calendar.html` and `Help/design-time/beepcalendardesigner.html` — deferred

### Cross-Phase Governance
- [x] Keep `MASTER-TODO-TRACKER.md` in sync after each phase
- [ ] Update `Calendar/Readme.md` with design-time features — deferred
- [ ] Validate no regressions in existing Calendar runtime paths — deferred

### Build Verification
- [x] `dotnet build TheTechIdea.Beep.Winform.Controls.csproj` → 0 errors after each phase
- [x] `dotnet build TheTechIdea.Beep.Winform.Controls.Design.Server.csproj` → 0 errors after each phase
- [x] `dotnet test` (Calendar-filtered) → 5 passed, 0 failed

### Files Created / Modified
- **New (10):** `BeepCalendar.Core.PublicApi.DesignerSurface.cs`, `CalendarEventCollectionEditor.cs`, `CalendarEventEditorDialog.cs`, `EventCategoryCollectionEditor.cs`, `EventCategoryEditorDialog.cs`, `CalendarResourceCollectionEditor.cs`, `CalendarResourceEditorDialog.cs`, `BeepCalendarDesignerTests.cs`
- **Modified (4):** `BeepCalendarDesigner.cs`, `BeepCalendar.Core.PublicApi.cs`, `BeepCalendar.Fields.cs`, `BeepCalendar.Types.cs`

---

## BeepGridPro Header And Toolbar Commercialization

Target: header band and column headers that match AG Grid / Excel / Telerik / DevExpress behaviour.
Two decisions frame this program: the 13 `IPaintGridHeader` painters get wired into real rendering
(today they are only called for `CalculateHeaderHeight()`), and sort/filter move to the commercial
model — click the header to sort, one menu button for sort + filter + clear.

### Phase 1 - Toolbar Geometry And Sizing
- [x] Add a single vertical-centre helper and apply it to every toolbar rect (search icon, filter, advanced, clear-filter, overflow, button bounds) — icons previously sat ~7px high in the 32px band
- [x] Give labelled and icon-only buttons the same hit height, with the icon centred inside
- [x] Move the search icon inside `SearchBoxRect` and make `SearchIconWidth` the one text inset used by both the painter and the live search editor (painter now draws box before icon)
- [x] Replace the `bounds.Width / 4` title cap with a measured width clamped to remaining space, drawn with `EndEllipsis`
- [x] Define and implement the narrow-width collapse order: labels → export buttons → overflow → search shrink, never overlap
- [x] Fix the overflow test to compare against an absolute right limit instead of `bounds.Width`
- [x] Clamp `BadgeRect` inside the toolbar band

### Phase 2 - Header Painter Contract And Wiring
- [x] Add `CalculateHeaderCellLayout(...)` + `HeaderCellLayout` (TextRect, SortIndicatorRect, MenuButtonRect, SortHitRect) to `IPaintGridHeader`
- [x] Implement the commercial geometry once in `BaseHeaderPainter` (reserved sort slot, right-hand menu button, DPI-scaled, vertically centred)
- [x] Make `GridRenderHelper.DrawHeaderCell` delegate to the active painter and record the returned rects into the sort/filter/sort-hit rect dictionaries
- [x] Cache the header painter on the grid (`BeepGridPro.HeaderPainter`) instead of allocating `GridColumnHeadersPainterHelper` per layout pass
- [x] Verify the painters drive rendering — runtime sweep of all 13 styles produces 7 distinct header renders (was one hardcoded look)
- [ ] Differentiate the styles that still render identically: Standard/Compact, Material/Fluent/AntDesign, Bootstrap/AGGrid/DataTables, Telerik/Tailwind — they share the granular paint methods the renderer calls
- [ ] Override `CalculateHeaderCellLayout` per style where the style calls for different geometry — all 12 currently inherit the base geometry

### Phase 3 - Commercial Sort And Filter Interaction
- [x] Reserve the sort indicator slot for every sortable column so header text stops reflowing on first sort
- [x] Sort on header-text click (asc → desc → none); shift-click appends to multi-sort
- [x] Show the 1-based multi-sort order on the indicator
- [x] Single column menu button replaces the separate funnel icon and owns the filter entry point
- [x] `BeepGridPro.ShowColumnMenu` puts Sort Ascending / Descending / Clear Sort in the same menu as Filter / Clear Filter
- [x] Pad sort/menu hit targets to ≥24px DPI-scaled
- [x] Unify icon visibility across styles: reveal on hover, slot always reserved
- [x] Apply multi-sort to the data pipeline — `ApplyLocalSort` now orders by every sorted column via `SortOrder` (OrderBy/ThenBy)
- [x] Fix `DataSource` assignment never materializing rows when virtualization is off (it always took the `skipRows:true` virtualized path)

### Phase 6 - Theme-Driven Colour And Footer Style
- [x] Header band colour confirmed theme-driven — `Theme.GridHeaderBackColor` / `GridHeaderForeColor` are the only inputs; an accent band (blue with white captions) is a theme change, not a code change
- [x] Replace the unsorted-column indicator: two outlined triangles sharing a base line read as a `◇` diamond at 12px; now two faint filled chevrons with a gap (a proper `⇕`)
- [x] Add `ColorUtils.EnsureReadable` / `ContrastRatio` / `RelativeLuminance` (WCAG) — honours the theme's colour and only substitutes black/white when the pairing is illegible
- [x] Apply the guard to header captions, sort indicators, sort-order badges, funnel icon, cell text, row numbers, toolbar title, and navigator info text across all 13 navigation painters
- [x] Paginated footer verified working — `NavigationStyle = navigationStyle.DataTables` already renders `Previous / 1 / Next` + `Showing 1 to 5 of 5 entries`; the record navigator (`First/Previous/Next/Last` + `Record 1 of 5`) remains the default
- [x] Fix the grey bar above the navigator: `CalculateTotalContentWidth` added `(n-1)` border pixels that do not exist (borders are drawn inside each column's width, layout advances `px += w`), so `AutoSizeColumnsMode.Fill` always over-reported by a few px and raised a phantom horizontal scrollbar whose thumb spanned the whole track
- [ ] `MaterialYouTheme` ships unusable grid colours (header `#101010` on fore `#080808`, ratio ~1.06; a white `AltRowColor` inside a dark theme) — the renderer now compensates, but the theme data itself should be corrected

### Phase 7 - Search Editor Flicker (Custom-Drawn Host + Real Child Control)
Reported: clicking the toolbar search box activates it, then moving the mouse away makes it blink
on and off. Diagnosed with `scratchpad/SearchFlickerProbe`, which injects mouse events and counts
repaints — 12 mouse moves produced 12 full-control repaints.
- [x] Root cause: `GridScrollBarsHelper.HandleMouseMove` called an unconditional full-control `SafeInvalidate()` on every `WM_MOUSEMOVE` just for thumb hover colours. Now repaints only when the hover state actually flips, and only over the scrollbar rects — 12 repaints → 2
- [x] Header hover repaint scoped to `HeaderRect` instead of the whole control (it also repainted the toolbar and any editor child in it)
- [x] Painter stops drawing the search text/placeholder while the editor is visible — the in-place editor owns those pixels, matching the commercial rule that the view stops rendering a value once its editor activates
- [x] Painter fills the search box with the editor's own BackColor while active, so the painted box and the control inside it are one seamless box
- [x] Editor no longer draws a second focus ring (`EnableFocusAnimation`/`ShowFocusIndicator` off, `BorderRadius = 0`) — `IsFrameless` suppressed only the static border, so a blue rounded border was nesting inside the painted amber one
- [x] Editor bounds inset to sit strictly inside the painted border — it previously took the box's full height and ran to its right edge, so its opaque fill covered the border along the top, bottom and right (only the left was safe, because the icon padding already inset it). Now clears the 1px stroke vertically and the 4px corner arc on the right, both DPI-scaled; `SearchBoxRadius`/`SearchBoxBorderWidth` are exposed from the painter so the two cannot drift apart
- [x] Verified: magnified capture shows one box, one continuous border with intact corners, no doubled text; unfocused toolbar unchanged
- [ ] `BeepTextBox.ApplyTheme` unconditionally overwrites `BackColor` (and forces `Parent.BackColor` when `IsChild`), so a host cannot set an editor's background and have it stick. Writing it back from `BackColorChanged` spins against `ApplyTheme` and hung the control — worked around by reading the editor's colour instead of assigning it. The theme layer should expose an opt-out rather than requiring the workaround
- [ ] Other unconditional full-control `SafeInvalidate()` calls remain on hot paths; audit the rest against the scoped-invalidate rule

### Phase 8 - Toolbar Icon Theming
- [x] Confirmed all toolbar icons already come from `IconsManagement/SvgsUIcons.cs` (Search, Filter, Settings, Close, Add, Edit, Delete, Upload, Download, Devices.Printer) — no raw paths or hand-drawn glyphs
- [x] Icons now recolour from the theme. They previously left colour to `ApplyThemeOnImage` + `ImageEmbededin.DataGridView`, which resolves to `GridHeaderForeColor` — not the toolbar's foreground — and rendered a fixed dark grey on every theme, leaving them nearly invisible on dark toolbars (verified by probe: MaterialYou paints a #101010 toolbar; Cyberpunk asks for cyan and got grey)
- [x] Added `StyledImagePainter.PaintSvgRecolored` — `PaintWithTint` applies a *multiplying* ColorMatrix, which cannot lighten a near-black source glyph (asking for cyan on black just zeroes the red channel and darkens it further). The new method replaces the fill outright via `ImagePainter.ApplyColorToAllElements`
- [x] Result cached per path+colour+opacity+size, so a theme change yields a new key instead of needing invalidation, and repeat paints are a blit rather than an SVG re-render — this matters because the toolbar repaints on hover changes. Replaces a `new ImagePainter(...)` per icon per paint
- [x] Icon colour is the toolbar foreground passed through `ColorUtils.EnsureReadable`, so a theme pairing fore and back too closely still yields legible icons
- [x] Active filter icon uses `Theme.AccentColor` rather than just a higher opacity, so filter state reads at a glance
- [x] Verified by render across three themes: Default (dark icons on light), Cyberpunk (cyan on navy), MaterialYou (white on near-black)
- [x] Search editor reverted from `Color.Transparent` to the parent's BackColor per user instruction — transparent rendered black (a transparent BackColor makes the control paint the parent background itself rather than showing it through) and never survived `ApplyTheme` anyway

### Phase 5 - Verified By Probe
Runtime probe (`scratchpad/GridStyleProbe`) against the built control:
- [x] 13 styles render, 7 distinct header looks
- [x] Caption rect identical before/after sorting — `{X=3,Y=0,W=96,H=30}` both times (slot reserved, no reflow)
- [x] Sort slot and menu button reserved before any sorting
- [x] Multi-sort state ordered — `Name#1, Qty#2`
- [x] Binding materializes rows immediately (3 rows) and an empty list clears to 0
- [x] Rendered PNGs inspected: default theme clean (no grey bar, row numbers present, `⇕` glyph, `▲1`/`▲2` badges); `MaterialYouTheme` fully legible after the contrast guard
- [ ] DPI sweep at 150% / 200% still to be eyeballed
- [ ] Narrow-window collapse order still to be eyeballed

### Phase 4 - Verification
- [ ] Toolbar and header verified vertically centred at 100% / 150% / 200% DPI
- [ ] Narrow-window collapse order confirmed with no overlap
- [ ] Sort click causes no horizontal text shift; multi-sort badge correct
- [ ] Column menu applies filters and updates the toolbar active-filter badge
- [ ] Style sweep (Material, AGGrid, Fluent, Telerik) visibly changes the header

---

## BeepDialogManager Enhancement Program

Per-feature documents: `TheTechIdea.Beep.Winform.Controls/DialogsManagers/plans/` (`README.md` is the
local tracker, plus `01`–`12`, one per feature, grouped into six phases).

Written from a full read of **9,829 lines across 42 files**. Same ground rules as the BeepTabs
program: **no stubs, no legacy, no swallowed exceptions, no duplication**, layout via
`TableLayoutPanel`, and verify by measurement rather than by reading.

Target: Radix UI / shadcn Dialog, Headless UI, Material 3, Fluent 2, Ant Design Modal, and the
desktop bar set by DevExpress `XtraDialog`, Telerik `RadDialog` and VS Code's modal + Quick Pick.

### Phases

- [ ] **Phase 1 — Ground truth**: harness (`12`), dead scaffolding (`04`), exception policy (`03`)
- [ ] **Phase 2 — One way to do each thing**: API surface (`01`), result/config model (`02`)
- [ ] **Phase 3 — Structure**: layout & composition (`05`), designer serialization (`06`)
- [ ] **Phase 4 — Pipeline**: placement & motion (`07`), progress & busy (`09`)
- [ ] **Phase 5 — Product surface**: notifications (`08`), command palette (`10`)
- [ ] **Phase 6 — Accessibility & design-time**: (`11`)

### Headline findings (evidence-backed)

- [ ] **`ShowInfo` is not the alias it looks like.** `Warning`/`ShowWarning`, `Error`/`ShowError` and `Question`/`ShowQuestion` are character-identical pairs whose `Show*` form is `[Obsolete]`. `ShowInfo` carries no such attribute and is the one that behaves differently — it constructs `BeepMessageDialog` directly and **bypasses the pipeline**, so theming, animation, placement, state persistence and the `DialogOpened`/`DialogConfirmed` events do not happen. Its own XML comment says so. A caller following the deprecation guidance would assume `ShowInfo` → `Info` is the same trivial rename the other three are
- [ ] **Three types mean "the result of a dialog"** — `DialogReturn` (what every public method returns, defined in `Vis.Modules2.0/IDialogManager.cs`), `DialogsManagers.Models.DialogResult` (243 lines, referenced by one callback signature and two doc comments), and `System.Windows.Forms.DialogResult`. The local class **shadows the framework type**, which is why 44 call sites across 8 files write the framework name fully qualified
- [ ] **`DialogPlacementEngine` has zero callers** while 17 sites set `StartPosition`/`CenterParent` by hand. Seventh instance in this codebase of complete, plausible code that nothing calls — the BeepTabs program found six, and three of those were duplicates of a live seam that would have fought it if connected. **Whether this engine agrees with the 17 hand-rolled sites is unestablished and must be measured before wiring it in**
- [ ] **Every `BeepInputDialog` leaks 30 GDI+ handles at construction.** `InitializeComponent` allocates 30 `GraphicsPath` objects and disposes none (`TestDialogForm` adds 6 more). They are assigned to `BorderPath`/`ContentShape`/`InnerShape` — runtime-computed geometry properties that are designer-serializable, so the designer wrote out 30 *empty* paths and assigns them over the computed values. This is also why that designer file is 2,060 lines. **Root cause is upstream**: those properties need `[Browsable(false)]` + `DesignerSerializationVisibility.Hidden` on the control that declares them, and the blast radius is every designer file in the solution
- [ ] **No dialog form uses `TableLayoutPanel`** — all six position with absolute `Location` plus `Dock`/`Anchor`, against a standing rule for this codebase and against every reference product's header/body/footer composition
- [ ] **Four bare `catch` blocks.** Two in `DialogStateStore` mean a corrupt or unwritable state file silently discards every remembered dialog position (one is commented *"Silently fail — persistence is non-critical"*); one in `DialogResult.GetData<T>` uses `InvalidCastException` as a type test where `value is T` is correct
- [ ] **Shipped scaffolding** — `TestDialogForm` (455-line designer) has no references anywhere in the solution, and `Forms/BeepDialogForm.resx` has no matching `.cs`
- [ ] **`Confirm`/`ConfirmSync` are identical**, and the name asserts an async/sync distinction that does not exist — both are synchronous
- [x] **Checked and NOT a defect:** `BeepMessageDialog` and `BeepQuestionDialog` *do* handle Enter/Escape, via per-form `ProcessCmdKey` rather than `AcceptButton`/`CancelButton`. Duplicated across forms and missing the framework's default-button semantics, but not the missing keyboard support an initial read suggested
- [x] **Checked and NOT stale:** `NOTIFICATIONS.md` documents methods that all exist with matching signatures. It is a usage cookbook to keep and update, not a stale design doc to delete

---

## BeepTabs Enhancement Program

Per-feature documents: `TheTechIdea.Beep.Winform.Controls/Tabs/plans/` (`README.md` is the local
tracker, plus `01`–`12`, one per feature). Architecture notes rewritten in `Tabs/README.md`.
Written from a full read of **9,638 lines across 60 files**; the **nine** previous planning/summary
documents were deleted rather than carried forward.

Ground rules for this program, per explicit instruction: **no stubs, no legacy/back-compat shims, no
swallowed exceptions, no duplication** — and verify by measurement, not by reading.

### Headline findings (evidence-backed)
- [x] ~~**`ITabPainter.PaintTab` is dead but mandatory**~~ — **THIS FINDING WAS WRONG.** `PaintTab` is the per-style extension point, called with no receiver from `BaseTabPainter.PaintTabItem`; acting on this would have broken all seven painters. See the `01` entries below. The genuinely dead member was `PaintBackground`
- [x] **The live render entry point was named `RenderLegacyHeader`** — now `RenderHeader`; the private `PaintLegacyTab` is now `PaintTabItemClipped`; stale `Phase 2` markers removed. Not cosmetic: the matching *"legacy paint overload"* comment is what produced the wrong `PaintTab` finding above
- [ ] **Measurement and rendering live in different subsystems** — sizes from `BeepTabs.Layout` → `painter.MeasureTab`, painting from `BeepTabHeaderHost` → `painter.PaintTabItem`, joined by a copied snapshot. Helpers (`BeepTabLayoutHelper`, `BeepTabOverflowCoordinator`) call *back* into the owner rather than reading the snapshot, so it is not the boundary it appears to be. Same shape as the duplicate-geometry defects already fixed in BeepTree and ToolTips
- [x] **Four bare `catch` blocks**, three in `TabFontHelpers` — the text-measurement code. A font/DPI failure silently returns a hard-coded 16px or measures with `SystemFonts.DefaultFont`, so tabs mis-size with no trace. `catch { return false; }` cannot distinguish "no" from "unanswerable"
- [x] **Errors reach only the debugger** — fixed: public `TabError` event (`BeepTabErrorEventArgs`) in all build configurations, and the five reporting operations now rethrow instead of returning as if they had succeeded
- [x] **`TabFontHelpers.ApplyFontTheme` is an empty public method** documented as "no-op" — deleted, zero callers
- [x] **Every tab was measured in one font and drawn in another** — `BaseTabPainter.MeasureTab` sized with `TabFontHelpers.GetTabFont(Theme, isSelected)` (bold when selected) while `DrawTextInBounds` painted with a hardcoded `SystemFonts.DefaultFont`. Theme fonts never reached a drawn title; selected tabs were measured bold and drawn regular. The correct font was already in a local one line above the call. This is the BeepTree label-clipping defect, in the base class all seven painters inherit
- [x] **High contrast was implemented, documented and never called** — `BeepTabHeaderHost.HighContrast.cs` held a 156-line paint pass whose doc claimed *"Called from OnPaint when IsHighContrast is true"*; nothing called it, so Windows High Contrast did nothing but focus rings. It was also a second, lossy implementation of tab geometry (no icons, badges or subtext). High contrast is now resolved in `TabThemeHelpers` where colours are resolved, ahead of theme *and* custom colours; the duplicate file is deleted
- [x] **Two functions answered "what colour is the close glyph"** — `TabIconHelpers.GetCloseIconColor` (live) and `TabThemeHelpers.GetCloseButtonColor` (zero callers), already disagreeing on the hover fallback. The orphan was deleted
- [x] ~~**`BeepTabRtlLayoutHelper` is referenced by exactly one file — itself.**~~ Fixed: wired into `SyncSnapshot` and proven by measurement — see the `08` entries
- [x] ~~**`HeaderOverflowPolicy` defaults to `None`**~~ Fixed: defaults to `OverflowMenu`, and the three policies nothing implemented were removed — see the `07` entries
- [x] ~~**All seven painters override exactly two members each**, and have never been rendered side by side.~~ Rendered: they were three near-duplicates plus a pair, and each now has its own visual — see the `10` entries
- [x] **Removed while writing the plan**: empty `Tabs/Adapters/` folder (the deleted plan called adapters "temporary internal seams"; the seam went, the folder stayed) and nine stale documents

### Program state

Eleven of the twelve features are complete. What remains is three decisions rather than defects —
tear-out (`09`), whether `Documents` and `Workspace` should differ (`06`), and whether the visible tab
run should be contiguous around the selection (`07`) — plus two things the harness cannot reach:
rendering high contrast (an OS setting) and designer verbs with undo (needs a live VS host).

### Features (P0 first)
- [x] `12` **Harness built first** (`scratchpad/TabsProbe`), reproducing the known defects before any fix. Enforces the ground rules mechanically — bare catches, empty bodies, never-read model properties, empty directories — plus geometry agreement
- [x] **Shipped crash fixed: painters disposed shared cached GDI objects.** Reported as `ArgumentException: Parameter is not valid` from `Graphics.FillPath` in `CardTabPainter`. `PaintersFactory.GetSolidBrush`/`GetPen` return process-wide cached instances; `using` disposed them, and the factory's `_ = brush.Color` probe cannot detect that because `SolidBrush.Color` reads a cached field without touching the native handle — so the colour is poisoned for the whole process and every later `FillPath` throws, in any control. Fixed at 10 sites in `Tabs/Painters` and 5 in `Trees/Painters`. The 23 background painters matching a blanket search were **not** offenders (`CreateLinearGradientBrush` transfers ownership; removing their `using` would leak GDI handles). `PaintersFactory` now documents the ownership split
- [x] **The harness passed that crash as green, and was fixed.** `DrawToBitmap` routes through `Control.PaintWithErrorHandling`, which never surfaces the exception, so a half-painted tab satisfied both the "not blank" and "renders distinctly" checks. The contact sheet now hooks `FirstChanceException` (filtered to the tab painters) and asserts no paint threw **before** any pixel comparison. Its first pixel finding under the old order — "Underline and Minimal render identically" — proved to be an artifact of the crash
- [x] `10` **Style transition never ended** — the timer stopped but left `_styleTransitionProgress` at `1f` and `_transitionFrom != _transitionTo`, so `HasTransition` stayed true forever: **every tab painted twice by two painters on every paint** (one pass at alpha 0 doing all its GDI work for nothing), `PrimaryPainter` never used again, and `DrawHeaderSelectionIndicator`'s settled-state branch unreachable. Fixed by clearing the state, not just stopping the timer
- [x] `10` **Underline and Minimal were the same style** — the two painter classes are byte-identical apart from the name, neither draws an underline, and the accent bar came from `BeepTabs.Animation` drawing it for both. Minimal no longer gets the accent bar. **Remaining:** the two painter classes are still identical code; each should own its own style
- [x] `10` **Selected-tab label was invisible in Underline and Minimal** — neither fills a tab body, but the base class gave them the selected text colour meant for a filled accent, so the title was drawn white on white and vanished on selection. Fixed via `ColorUtils.EnsureReadable` against a new `GetTabSurfaceColor` overridden by the non-filling painters
- [x] `10` **Every close button was a solid dark square** — `close.svg` is a red rounded-square badge with a white cross, not a glyph; tinting multiplied it to black, recolouring filled the whole box. Switched to `x.svg` (one polygon, no fill attribute). A probe asserts the glyph leaves its background visible
- [x] `10` **CORRECTION:** an earlier entry called the "Underline/Minimal identical" finding an artifact of the crash. It was real. The sheet had been capturing mid-cross-fade (fixed ~240 ms wait against a 220 ms animation), so the same binary passed and failed on alternate runs. It now waits on the actual timers and drives a real selection change; stable across repeated runs
- [x] `10` **All four header positions now covered.** Bottom found one defect: Button clipped its captions, because it draws the button inset inside the tab slot and lays the caption out inside the *button* — four fewer pixels than `MeasureTab` reserved. Measure/draw divergence in geometry rather than fonts; `ButtonTabPainter` now adds its inset back when measuring. Nothing else was wrong at Bottom
- [x] `10` **Two more measurement mistakes, both mine.** An assertion that the chrome must touch the top edge of a bottom-docked strip failed all seven styles at once — the signature of a wrong assumption, not seven broken painters (the 44px crop leaves whitespace above a ~30px tab). And the glyph metric counted *dark* pixels, so Classic and Card — which draw white captions on a filled selected tab — scored as having no text while the render showed it plainly. Now a colour-agnostic diff between captioned and caption-free renders at identical geometry, shared by the vertical and bottom checks instead of written twice
- [x] `10` **Every adornment was drawn in the same slot** — badge, dirty dot, busy ring and close button all at `Bounds.Right - edgeP - size`, stacked on each other, so a tab with a badge and a close button showed one of them. `MeasureHorizontalAdornmentWidth` had always reserved the space cumulatively; the layout never used it. Now laid out right-to-left along a cursor
- [x] `10` **Tabs were measured 6px too narrow** — edge padding reserved once, applied at both ends. Invisible only because the caption ran *underneath* the overlapping adornments; separating them made a caption the tab was sized to fit ellipsise. One bug had been hiding the other
- [x] `10` **The dirty dot and busy ring rendered blue-on-blue** — both resolve to the primary colour, which is the selected tab's fill, so they drew the whole time and nothing could see them. Same defect as the Info/Count badge; all three now go through one `SeparateFromSurface` rule
- [x] `10` **Painter geometry was not DPI-scaled** — the style work introduced insets, gaps, radii and rule thicknesses as raw constants, which stay the same physical size at 200% while the text scales. `BaseTabPainter.Scale` added; every literal and design-pixel constant routed through it
- [x] `10` **Vertical (Left/Right) headers have never rendered a caption** — only the close glyph. Three stacked causes: `TextRenderer.DrawText` goes through GDI and **ignores the world transform**, so the `RotateTransform` did nothing and the text landed off the tab (the deleted `DrawTabText` had the same bug, so this predates the program); the adornment helper gave the label a box one text-line high inside a ~30px tab, so captions ellipsised to nothing; and `CalculateTabSizes` sized the vertical run from `MeasureTab().Height` (one line, ~30px) when rotation means the run needs the caption's *width*. Now drawn with `Graphics.DrawString`, with a run-length text box and a width-derived extent. Also removed a double edge-padding reserve that clipped the last glyph
- [x] `10` **The vertical check was wrong twice before it was right** — first counting chrome (passed at 205px with no text on screen), then using an empty-caption baseline that changed the tab size so the delta included geometry, not just glyphs. It now holds captions fixed and toggles `TabTextVisibility` for the baseline; it scored 0px against the broken code and 56px against the fixed code, and its threshold comes from that measured range
- [x] `10` **Badges, the busy indicator and Button's border bypassed the colour seam** — badge kinds read `Theme.ErrorColor`/`WarningColor`/`SuccessColor` directly (so badges stayed themed in high contrast on a system-coloured tab), the busy ring was hardcoded to `SystemColors.ControlDark` (the one adornment that never responded to the theme, invisible on a dark header), and the badge count was drawn in hardcoded `Color.White` — unreadable on a light Warning or Success badge. All now resolve through `TabThemeHelpers`, with the badge count picked for contrast against its own fill. A harness check fails if any painter reads a theme colour directly
- [x] `10` **Every painter carried two implementations of the same visual.** `PaintTab` was overridden by all seven painters and reachable only from `BaseTabPainter.PaintTabItem`, which never runs because all seven override that too — so each style was written twice and only one copy displayed. Proven with a probe subclass, after reading had given the wrong answer twice in opposite directions ("dead", then "live extension point"). `PaintTab` and `DrawTabText` deleted from the interface, base and all seven painters
- [x] `10` **Classic, Capsule and Segmented were one painter with three radius constants**; Minimal and Underline had byte-identical `PaintTabItem` bodies. Each now has a real identity — Classic: open-bottom sheet + dividers; Underline: full-width rule + thick accent + accent label; Capsule: inset floating pill, selected-only fill; Minimal: no chrome, contrast-only selection; Segmented: recessed bordered track + tile; Card: separated bordered cards + accent stripe; Button: bordered buttons. Minimal and Underline no longer override `PaintTabItem` — the shared body is the base
- [x] `10` **Two theme defects the sheet exposed:** under MaterialDesignTheme, Card and Button rendered *unselected* tabs in the primary colour (they read `Theme.ButtonBackColor` directly instead of the seam, so tabs looked inverted), and the selected tab had no fill at all because that theme defines `TabSelectedBackColor` as the header's near-white. The seam now guarantees the selected fill is perceptibly different from the strip. That theme went from 10 near-identical pairs to none
- [x] `10` **Distinctness is measured, not asserted.** Exact equality is too weak — two tabs differing only by a radius pass it. Every pair must now differ by ≥3% of pixels. Before: 0.2–1.4% across several pairs, and Capsule/Minimal at 0.0% under MaterialDesignTheme. After: the closest pair under any theme is 3.1%. I briefly demoted this check to informational because a pixel count cannot separate a radius tweak from a thin salient feature; that was backwards — the fix was to make the styles genuinely differ, not to weaken the measurement
- [x] `10` **Contact sheet built** — 7 painters × 3 themes = 21 renders in `scratchpad/contact-sheets/`. All seven render distinctly under every theme and every painter responds to a theme change. Still to add: high-contrast column, state columns, DPI variants
- [x] `12` **"Declared but never wired" was the program's most productive pattern** — six instances, each complete and plausible code that nothing called: `BeepTabInputPolicy`, `BeepTabAccessibleObjectFactory`, the touch API, `BeepTabRtlLayoutHelper`, `TabColorConfig` and `TabStyleConfig`. Three were duplicates of a live seam that would have fought it if connected; one (the accessibility factory) was a working implementation of a feature the control simply did not have. A sweep for types with no in-assembly reference is now part of the harness, reported **informational** — partial-class files and public API legitimately show zero matches, and deletion plus a clean compile is what actually settles it
- [x] `12` **86 checks green.** Added this round: measure/draw font agreement, single high-contrast decision point, naming honesty, failure surfacing, and Release-visible error channel
- [x] `12` **The state check passed all seven adornments while three were invisible.** It diffed each render against `plain`, but adding an adornment also widens the tab, so the diff registered a change even when the adornment was drawn in the tab's own colour. It now requires the state to introduce a *colour the plain tab does not have*. Third time in this program a check has passed on a confound — every visual check needs a controlled baseline or it measures the wrong thing
- [x] `12` **The DPI check would have missed the defect it was written for.** Its self-test caught that: the real offenders were named constants (`InsetX`, `TrackInset`, `CardGap`), which carry no digit at the use site, while the regex only matched numeric literals. Extended to require every design-pixel constant to reach the surface through `Scale(...)`. Separately, writing that regex through a Python heredoc silently emitted literal **backspace** characters into the C# source in place of `\b` — the same escaping trap hit earlier in this session
- [x] `12` **A fourth harness check was wrong**: the model-property check passed while `BeepTabItem.Bounds` was dead, because it matches property *names* as text and `itemLayout.Bounds` satisfied it. Reworded to claim only what it can prove ("name appears in a read position") and to state that deletion+compile is authoritative. A fifth omitted `DeclaredOnly` and reported six inherited `Control` members (`Margin`, `Padding`, `Size`, …) as design-time defects — the same mistake as the very first harness bug
- [x] `12` Three harness checks were wrong on the first run and were corrected before being trusted: the never-read check reported 34 inherited `BaseControl` properties (fixed with `DeclaredOnly`), the naming check flagged the corrective prose explaining why the old names were wrong (so that detector is now **self-tested** against known-bad and known-good strings before its verdict is trusted), and the dead-member check gave wrong answers in both directions. **The dead-member check is now informational only** — text matching cannot distinguish a declaration from an override from an invocation; deletion + compile is the reliable method, as established during the BeepTree work
- [x] `01` **CORRECTION: `PaintTab` was not dead and must not be deleted.** `BaseTabPainter.PaintTabItem` calls it with **no receiver**, so a `.PaintTab(` search finds nothing while all seven painters override it — it is the per-style extension point. The plan's original instruction to delete it would have broken every painter. The source comment calling it a *"legacy paint overload"* is what caused the error
- [x] `01` The genuinely dead member was **`ITabPainter.PaintBackground`** — removed from the interface and `BaseTabPainter`, proven by a clean compile rather than a grep
- [x] `01` Misleading XML docs corrected: `PaintTab` documented as the extension point (with a note on why it looks uncalled), `PaintTabItem` as the host entry point, `MeasureTab` carrying the measure/draw font rule
- [x] `01` **The measure/draw font rule was being violated by the class that documents it.** `DrawTextInBounds` now takes the font as a parameter — the same one `MeasureTab` measures with — and its unused `isHorizontal` parameter was removed. Harness fails if any painter names a system font as its draw font
- [x] `01` **Two text renderers resolved.** `DrawTabText` went with the unreachable `PaintTab` path; `DrawTextInBounds` is the single text path and now handles rotation for vertical headers — which, it turned out, had never worked in either renderer
- [x] `02` **Renames done** — `RenderLegacyHeader` → `RenderHeader`, `PaintLegacyTab` → `PaintTabItemClipped`, `Phase 2` markers removed. The harness now fails on any identifier or comment that *opens* by declaring something legacy, and that detector is self-tested against the three verbatim originals
- [x] `02` **The snapshot is what gets painted — asserted.** Three tabs, middle one selected: snapshot `{X=67,Width=121}`, painted fill `{X=67,Width=121}`, **Δ0px on both edges**. Painting does not re-derive tab extents, so the measure/render split is a real boundary and not the two-sources-of-geometry defect that cost BeepTree and ToolTips. The helpers that call back into the owner are asking for sizes to build the snapshot *from*, not producing rectangles behind it
- [x] `02` **The measurement failed twice before it worked** — sampling the tab centre traced 2px (that line is the caption) and sampling near the top traced 3px (that line is the focus ring). Taking the modal colour of the tab interior and finding its extent is indifferent to where text and chrome fall. The check now declines to compare when the traced run is under half the tab, rather than blaming the control for a bad sample
- [ ] `02` Remaining: measurement lives on `BeepTabs.Layout`, rendering on `BeepTabHeaderHost` — a structural preference, with no defect now driving a move
- [x] `03` **All four bare catches removed.** Two in `TabFontHelpers` were defensive noise over code that cannot throw once `ResolveSafeFont` has run; the third re-measured with `SystemFonts.DefaultFont`, reporting a width for a *different* font than the painter draws with — the BeepTree label-clipping defect exactly; the fourth (`ScaleTouchTarget`) could only hide a bug as an unscaled touch target, meaning close buttons too small to hit on high-DPI. `IsFontUsable` keeps a narrow `ArgumentException` catch for the disposed-font case, which is genuine handling
- [x] `03` **Failures now surface.** `AddPage`, `ClearPages`, `InsertPageAt`, `RemovePage` and `MovePage` report and rethrow, matching `CreatePage` which already did. `RemovePage`/`MovePage` previously returned `false` for both "no such page" and "it threw". This mattered at design time: `BeepTabsDesigner.ExecuteTabsAction` wraps each call in a `DesignerTransaction`, so a swallowed `AddPage` failure **committed the transaction as though the page had been added** and reported nothing; it now cancels and shows the error
- [x] `03` **`TabError` event added** — public, `BeepTabErrorEventArgs` (context + exception), raised in every build configuration. Documented as a diagnostic channel, not error handling: the operation still throws, and handlers must not throw because the event fires from inside a `catch` that is about to rethrow
- [x] `04` **`ApplyFontTheme` deleted** — empty body, documented as "no-op", **zero callers** (the many `ApplyFontTheme` hits elsewhere belong to other controls' own helpers). Harness confirms no remaining stubs, no empty directories, and no never-read properties on the tab models
- [x] `05` **`BeepTabItem.Bounds` deleted — a second copy of the geometry, and the copies disagreed.** `BeepTabLayoutHelper` wrote the rectangle into both `item.Bounds` and `itemLayout.Bounds`, but `BeepTabRtlLayoutHelper` mirrors only the layout copy, so under RTL the item copy was stale. Nothing read it; proven by deletion + compile
- [x] `05` **`BeepTabItem.Content` deleted — a live `Control` inside a render snapshot**, duplicating what `BeepTabPage` owns and dangling once a page is disposed. Its two uses needed no control reference: an index lookup that allocated a whole snapshot and scanned it by reference (`_hostedPages.IndexOf(page)` already knows), and an initialised sentinel, now the typed `IsPageBacked`. A reflection check fails if the model reacquires geometry or a `Control`
- [ ] `05` Remaining: `Index` on a "snapshot" type — load-bearing across overflow, layout and hit-test, so it stays absent a reason beyond tidiness
- [x] `11` **Design-time verified; no defects found** — recorded as such rather than left ambiguous. No editable-but-unpersisted property on either type (the `Browsable(true)` + `Hidden` trap, same shape as the dead overflow policies); all 14 `ShouldSerialize*` on `BeepTabPage` have a matching `Reset*`; a default page serializes nothing; and the authored-to-runtime round trip holds in the shape a generated `InitializeComponent` uses (no loops or conditionals)
- [x] `11` **The existing `BeepTabsPersistenceTests` could not be run** — the test project fails to compile on a pre-existing, unrelated error in `Dialogs/BeepDialogManagerCreationTests.cs` (`'Form' does not contain a definition for 'CustomContent'`; the symbol exists nowhere in the assembly). The round-trip assertions were reproduced in the harness rather than reporting the behaviour as unverified
- [x] `06` **Mode contract now has one resolution point.** It was re-decided at 20 call sites across 11 files, all spelled `TabMode == BeepTabMode.Navigation`, so nothing named what each guard protected. `BeepTabModeCapabilities` states it once with a member per governed feature (`SupportsPinning`, `SupportsMruOrdering`, `SupportsClosedTabHistory`, `SupportsPreviewTabs`, `SupportsDirtyCloseGuard`, `SupportsDragReorder`, `SupportsTabContextMenu`); the harness fails if any other file names a mode directly
- [x] `06` **`BeepTabInputPolicy` deleted** — 115 lines whose own header said to use it "instead of scattering if/else guards", with zero callers while those guards existed. A second implementation of per-item permissions (already inline in `Metadata`/`WorkspaceCommands`) and of close-key handling (already in `BeepTabs.Keyboard`). Proven dead by deletion + compile
- [ ] `06` **Open product decision: `Documents` and `Workspace` are behaviourally identical.** All 20 guards tested Navigation vs not-Navigation, so the enum declares three modes and the control implements two. Deliberately *not* collapsed the way the overflow policies were: those were actively harmful (tabs dropped with no menu to reach them), whereas which behaviours should differ here — preview tabs, pinning, MRU, split groups — is a product call that cannot be inferred from missing code. The capability type now localises the decision to one file
- [x] `07` **Three of five overflow policies were fiction** — `ScrollButtons`, `ShrinkToFit`, `Multiline` had zero references assembly-wide. Selecting one was worse than `None`: tabs dropped from the run *and* no overflow menu (it is gated on `OverflowMenu`), so they became unreachable — while the designer offered all five as working. Enum now declares only `None` and `OverflowMenu`
- [x] `07` **Default changed from `None` to `OverflowMenu`** so the control has defined behaviour the first time tabs do not fit
- [x] `07` **Selected and pinned tabs can no longer be pushed into overflow.** The run was filled left-to-right until a tab did not fit, so a selected tab vanished when clicked and pinning protected nothing. Space is claimed in priority order (pinned, selected, positional) while tabs still render positionally; the fill loop no longer stops at the first tab that does not fit, so one wide tab cannot hide everything behind it. Verified with 12 tabs in a 420px header (tab 10 selected, 11 pinned): visible `0,10,11`, overflow `1..9`, six assertions
- [x] `07` **Overflow menu is MRU-ordered.** Positional order is wrong for a list that is by definition the tabs that did not fit; `BeepTabWorkspaceMruTracker` already tracked recency and nothing consulted it. Verified: visiting 3 then 5 then settling on 0 yields `5,3,4,6,7,8,9,10,11`. Navigation mode keeps positional order, MRU being a document-mode capability
- [x] `07` **Three more unreachable actions removed** — `BeepTabHeaderActionKind.AddTab`, `ScrollBackward` and `ScrollForward` were declared and dispatched by the router, but no slot was ever created with any of them and all three handlers were `return false`. The scroll pair were remnants of the `ScrollButtons` policy already removed. Enum values, router branches and stubs all deleted
- [ ] `07` Remaining: decide whether the visible run should be a contiguous window around the selection rather than `0,10,11` with a gap
- [x] `08` **High contrast fixed** — see the headline finding above. The state→system-colour mapping from the deleted file was preserved rather than reinvented, each with a comment recording its origin. Harness asserts exactly one file consults `SystemInformation.HighContrast` and that `IsHighContrast` is read outside its own file
- [x] `08` **RTL implemented and proven.** `BeepTabRtlLayoutHelper` is now called from `SyncSnapshot` before anything consumes the snapshot. Mirroring the snapshot alone is sufficient — painting and hit-testing both read those bounds — which makes `FlipPoint` actively wrong (it would mirror twice), so it was deleted. RTL is resolved from the owning `BeepTabs`, since `RightToLeft.Inherit` on the host reports the framework default. Measured, not read: LTR `[0..60] [60..120] [120..180]` becomes RTL `[444..504] [384..444] [324..384]`; five assertions cover change, right-hand placement, containment, non-overlap and width preservation
- [x] `10` **Each painter now owns its style.** The accent bar was a `_tabStyle == …` branch inside `BeepTabs.Animation` — a switch on style outside the painters. `ITabPainter.PaintSelectionAccent` replaces it, called after every tab and outside the per-tab clip (the accent animates between tabs and would be clipped mid-slide). `BaseTabPainter` no-ops it; `UnderlineTabPainter` overrides it; the cross-fade still works by asking both painters to draw at complementary alpha. Underline and Minimal are finally different classes doing different things
- [x] `08` **The control had no accessible tree.** `BeepTabs` never overrode `CreateAccessibilityInstance`, so a screen reader saw one opaque control — tabs could not be enumerated, named or activated — while `BeepTabAccessibleObjectFactory` (228 lines, correct roles/states/actions) had **zero callers** and `BeepTabHeaderHost.Accessibility.cs` was an empty partial promising the work "in a future update". Now reports `PageTabList` with one child per tab and per close button, built from the layout snapshot. Verified: 6 children for 3 tabs, named by caption, selected tab reports `Selected`, and accessible `Select` changes the selection
- [x] `08` **Keyboard contract verified** — Ctrl+Tab walks MRU (0→3→1 then cycle lands on 3, where positional would say 2); Home/End jump to first/last
- [x] `08` **Touch API was dead and unsound** — `ExpandToMinTouchTarget`/`TouchHitTestTabIndex`/`MeetsTouchTarget`/`ScaleTouchTarget` had no callers, and could not have been wired as written: tabs are contiguous, so centred expansion overlaps neighbours and the first-match hit test would make a tab's left edge select its neighbour. Deleted with `MinTouchTargetWidth`, whose single reader copied it into a render-context field nothing consumed. For a tab strip the working mechanism is `HeaderHeight` — default 30px against WCAG 2.5.5's 44dip, and raising it does reach the hit target (48 → 48px tab, verified)
- [x] `08` **CORRECTION:** I first reported `MinTouchTargetWidth` as having zero readers. It had one — `BeepTabLayoutHelper` copied it into `BeepTabRenderContext`. My grep's `head` limit had truncated that hit behind other controls' identically-named properties. The conclusion held but the evidence was wrong; the compiler caught it
- [ ] `08` Remaining: **render** high contrast to prove legibility — `SystemInformation.HighContrast` is an OS setting the harness cannot switch, so this needs a manual pass on a machine with it enabled
- [x] `09` **Dragging bypassed every constraint the menu enforced.** `BeepTabs_DragDrop` called `TryMoveHostedSourceItem` directly with no checks, while Move Left/Right went through `CanMoveHeaderTab` — so a pinned tab could not be moved past an unpinned one via the menu but could be dragged anywhere, and `TabCanReorder=false` was honoured only by the menu. Both paths now share one `CanReorderTabTo(from, to)` predicate; a non-reorderable tab no longer starts a drag at all, since showing drop markers for a move that will be rejected reads as a broken control
- [x] `09` **Pinning meant "immovable" where it should have meant "confined."** The item snapshot cleared `CanReorder` for every pinned tab — redundant with the partition check and stricter than it, so pinned tabs could not be reordered among themselves, which VS/VS Code/Chrome all allow. Removed; the partition check is the single rule
- [x] `09` Verified with 5 tabs (0–1 pinned, 3 non-reorderable): partition uncrossable both ways, reorder allowed within each partition, `TabCanReorder=false` blocks move *and* drag start, Navigation mode refuses reorder, and the menu and drag paths agree on every adjacent move
- [ ] `09` Remaining: decide tear-out explicitly
- [x] `06` **CORRECTION:** when routing the mode guards I mapped `BeepTabs.Interaction.cs:295` to `SupportsDragReorder` from its grep line number without reading the enclosing method — it is the preview-tab double-click promotion, so it is `SupportsPreviewTabs`. Caught by auditing every renamed guard against its enclosing method signature rather than trusting the batch edit
- [x] `10` **The style/colour config models were dead duplicates.** `TabColorConfig` (64 lines) and `TabStyleConfig` (54 lines) carried designer attributes and `ExpandableObjectConverter`, so they were written to appear in the property grid — but no control ever exposed them as a property, and nothing in the assembly referenced either type. They also froze a second copy of the live defaults: `TabColorConfig.HeaderBackgroundColor` is `(245,245,250)` and `TabBackgroundColor` is `(240,240,245)`, exactly the fallbacks `TabThemeHelpers` owns, and `TabStyleConfig.BorderRadius = 4` against `TabStyleHelpers.GetBorderRadius`. Had they ever been wired up they would have fought the seams. Both deleted
- [x] `10` **Live theme switching verified — no defect.** Switching `Theme` on a live control produces geometry and pixels identical to a control built with that theme from the start: tab widths `104/95/79` both ways, **0.0% pixel difference**. `ApplyTheme` re-resolves the font and the painter's theme, then calls `RefreshHeaderLayoutState`, which re-measures and calls `Invalidate`
- [x] `10` **CORRECTION:** I first reported that `ApplyTheme` "never invalidates or refreshes layout" and was about to add an `Invalidate()`. Both halves were wrong — I had read a truncated view of the method and missed the `RefreshHeaderLayoutState()` call on its last line, which does both. Checking before changing is what stopped a redundant edit; the measurement had already shown there was nothing to fix
- [ ] `11` Remaining: designer verbs with undo (the round trip and the no-control-flow shape are verified)

---

## BeepTooltips Enhancement Program

Per-feature documents: `TheTechIdea.Beep.Winform.Controls/ToolTips/plans/`
(`README.md` is the local tracker, plus `01`–`13`, one per feature). Architecture notes rewritten in
`ToolTips/README.md`. The five previous planning/summary documents were deleted — this program was
written from a full read of the ~4,500 lines of source, not from them.

Target: parity with Floating UI / Popper, Radix, Tippy, Material 3, Fluent 2, Ant Design, DevExpress
and Telerik.

### Headline findings (evidence-backed)
- [ ] **Three config properties are declared, documented, and never read by any code** — verified by searching the whole assembly for reads. `PersistOnHover` (only *assigned*, by `BeepPopover`; its XML doc claims WCAG 1.4.13), `Pinnable` (only the `#region` comment), `LoadPreviewAsync` (zero references). The API actively misleads: `PersistOnHover` defaults to `true`
- [ ] **The anchor is a 1×1 rectangle** — `CustomToolTip.CalculatePlacement` builds `new Rectangle(targetPosition, new Size(1, 1))`. Since `*Start`/`*End` alignments are defined against the anchor's edges, `TopStart`, `Top` and `TopEnd` collapse to nearly the same position against a 1px anchor
- [ ] **Two positioning implementations that disagree** — `ToolTipPositioningHelpers.CalculateBoundsForPlacement` (target rect, offset) chooses the placement; `CustomToolTip.AdjustPositionForPlacement` (target point, offset **+ arrow size**) applies it. A placement validated as fitting can be applied where it does not fit. Same defect class already fixed in BeepTree
- [ ] **The arrow does not track the anchor** — `CalculatePositionWithArrow` carries the comment *"we might need to adjust arrow position / For now, just ensure tooltip stays on screen"*. `ArrowOffset` exists in config for this and nothing computes it
- [ ] **Nothing repositions a visible tooltip** — no subscription to anchor move/resize, container scroll, form move, or DPI change. A tooltip is a top-level window, so it does not follow its parent for free
- [ ] **`ToolTipLayoutVariant` declares 7 values; the factory maps 3** — `Simple`, `Rich`, `Card`, `Shortcut` all fall to `BeepStyledToolTipPainter`, which never reads `LayoutVariant`. Layout is implicitly driven by which fields happen to be populated
- [ ] **A new top-level window per show** — `ToolTipInstance` does `new CustomToolTip()` every time; six `catch (ObjectDisposedException)` blocks are the symptom of unclear lifetime ownership

### Features (P0 first)
- [x] `13` **Harness built first**, reproducing the P0 defects before any fix: **5 passed / 15 failed → 19 passed / 3 failed**. It also found a fourth never-read property (`StepTitle`) the manual audit had missed
- [x] `01` Anchor rect & placement engine — `AnchorRect`/`AnchorControl` carry the control's screen rect; `Resolve` runs `offset → flip → shift → arrow`; the duplicate implementation (`AdjustPositionForPlacement` + `ConstrainToScreen`) is deleted. **The 1px anchor was not just an alignment bug — every placement overlapped the anchor**, so tooltips sat on top of the control they described. `TopStart/Top/TopEnd` now resolve to `410/520/630` against an anchor spanning `410..830`
- [x] `01` Explicit placements now flip only to their opposite and otherwise shift along their own edge; the twelve-candidate scorer is reserved for `Auto`
- [x] `02` Arrow tracking — `Resolve` returns the offset satisfying `tooltipCentre + arrowOffset == anchorCentre`; verified at both screen edges and centre (`+48`, `-43`, `0`, each landing exactly on the anchor centre). All four painters already consumed `config.ArrowOffset`; nothing had ever computed it
- [ ] `02` Remaining: clamp the arrow to the corner radius and hide it rather than parking it in a corner; DPI-scale `ArrowSize`
- [x] `03` Auto-update — new `ToolTipAutoUpdate` subscribes to the anchor, its scrollable ancestors and its form; coalesces bursts to ~16ms; skips no-op moves; hides when the anchor is disposed, hidden, minimised or scrolled out of an ancestor's client area. Moving the form 250px moves the tooltip 250px (was 0)

### Duplication / redundancy audit
- [x] **There were THREE implementations of tooltip placement**, not two — `ToolTipPositioningHelpers`, `CustomToolTip.Positioning` and `ToolTipHelpers`, none agreeing. One remains
- [x] Deleted `Helpers/ToolTipAnimator.cs` (172 lines, **zero references**) and `CustomToolTip.Positioning.cs` (comment-only after the refactor)
- [x] Deleted 6 dead methods: `AdjustForScreenEdges`, `DetectCollisions`, `CalculatePositionWithArrow` (its own comment admitted it did not adjust the arrow), `CalculateOptimalPosition`, `CalculatePositionForPlacement`, `MeasureContentSize`
- [x] **Two shadow properties** — `ShowShadow` and `EnableShadow` both defaulted true and every painter tested `ShowShadow || EnableShadow`, so setting either to false did nothing unless you set both. `ShowShadow` is now the single source; `EnableShadow` forwards and is `[Obsolete]`
- [x] **Two easing systems** — the live path uses `ToolTipAnimationHelpers.GetEasingFunction(ToolTipAnimation)` while `ToolTipConfig.AnimationEasing` is typed as `EasingFunction` and fed only the now-deleted animator. The property is inert; wiring it belongs to a future item, not a fake fix
- [ ] **Two pinning implementations** — `ToolTipConfig.Pinnable` (never read) vs the standalone `BeepPinnedTooltip`. Left for `10`, which is a design decision about ownership rather than a deletion
- [x] **Harness bug found and fixed**: the never-read check used `Contains()`, so the themes' `AnimationEasingFunction` counted as a read of `AnimationEasing`, and `#region Pinnable` counted as a read of `Pinnable`. With whole-word matching and `#region` exclusion the true count is **6**, not the 3 the manual audit found: `HideDelay`, `StepTitle`, `TriggerMode`, `PersistOnHover`, `LoadPreviewAsync`, `Pinnable`
- [ ] `TriggerMode` never being read means Focus/Click/Manual triggers are unimplemented — everything is hover-only. Folded into `04`/`05`
- [x] `04` **`TriggerMode` implemented** — the biggest single gap. Declared with four values and read by nothing, so every tooltip was hover-only and keyboard users never saw one. Verified by inspecting the manager's attached handlers: `Hover → [Enter, Leave]`, `Focus → [GotFocus, LostFocus]`, `Click → [Click]`, `Manual → []`, each detaching cleanly on `RemoveTooltip`
- [x] `04` `PersistOnHover` implemented — the pending hide is cancelled while the pointer is over the tooltip (`ToolTipInstance.IsPointerOver`), so it can be read, scrolled or clicked. WCAG 1.4.13 "hoverable"
- [x] `04` `HideDelay` now read — the close delay was a hard-coded 200ms; it also serves as the anchor→tooltip travel grace period
- [x] `04` `KeyboardTriggerable` adds focus triggers on top of hover, since a hover-only tooltip is unreachable from the keyboard
- [x] `04` Fixed a guard that would have left Focus mode silently broken: the post-delay check required the pointer to be over the control, which is right for hover and wrong for focus/click/manual
- [ ] `04` Remaining: safe-polygon bridge for diagonal pointer travel, clickable link spans (`LinkClicked`), and `Duration` defaults that auto-hide interactive content (WCAG 1.4.13 "persistent")
- [x] Never-read config properties down from **6 → 3** (`StepTitle`, `LoadPreviewAsync`, `Pinnable` remain, belonging to `07` and `10`)
- [x] `05` Escape now dismisses from the trigger — `ProcessCmdKey` only fires when the tooltip window has focus, which a hover tooltip never takes, so Escape did nothing in exactly the case WCAG 1.4.13 "dismissible" covers. New `ToolTipEscapeFilter` sees the key wherever focus is and deliberately does not consume it
- [x] `05` **Two real bugs in `OutsideClickMessageFilter`**: (1) `WM_LBUTTONDOWN` carries *client* coordinates but they were compared against a *screen* rectangle, so the "click inside the popover?" test only worked when the clicked window sat near the origin — now uses `Control.MousePosition`; (2) it never self-removed despite its comment claiming it did, so a popover disposed without closing leaked a process-wide filter holding a dead control. `BeepPopover` now also unhooks in `Dispose`, since a `Form` disposed directly never raises `Closing`
- [x] `05` Verified: 100 show/hide cycles leak **zero** message filters; Escape dismisses with focus still on the anchor
- [x] `05` Dismiss on window deactivate — covered by `ToolTipAutoUpdate`'s `Form.Deactivate` subscription
- [ ] `05` Remaining: a single `ToolTipDismissPolicy` flags enum, focus return for interactive tooltips, and dismissal when the anchor is disabled
- [x] `06` **Delay groups implemented** — sweeping a ten-button toolbar cost ten × `ShowDelay` (five seconds to read ten labels). A group's first tooltip still waits; siblings then open instantly for `SkipDelayWindow` (300ms) after the last closes. Groups derive from the anchor's parent when unspecified, so toolbars/ribbons/grid headers work with no configuration
- [x] `06` Guarded against the regression this feature invites: a group is armed only once a tooltip has genuinely been **visible**, so a fast flick across a cold toolbar still makes every button wait — asserted (`[500,500,500,500,500]ms`)
- [x] `07` **`LoadPreviewAsync` implemented** — documented as showing a skeleton until the task completes, invoked by nothing. Now runs fire-and-forget after the tooltip is visible (awaiting would delay the show by the caller's fetch time), then **re-measures and repositions**, since async content changes the size and a repaint alone would leave a skeleton-sized window around a full-size image
- [x] `07` **Performance bug found in the same code**: `PreviewToolTipPainter` called `Image.FromFile` and disposed it **on every paint**, so a visible preview tooltip re-read its image from disk on every repaint. Resolved once into `ResolvedPreviewImage`, owned and disposed by the instance
- [x] `07` Hiding during an in-flight load disposes the image rather than leaking it and never touches the disposed window — both asserted
- [x] `07` `StepTitle` was orphaned in both directions (nothing set it, nothing read it; `TourToolTipPainter` renders `Title`). Now forwards to `Title`, `[Obsolete]`
- [x] `07` **`LayoutVariant` is now authoritative** — `ToolTipSectionPlan` gives each variant a stated contract, consulted by both `PaintContent` and `CalculateSize` (they must agree or the window is sized for content it does not draw). Simple `201x59` / Rich `201x80` / Card `201x113` / Shortcut `286x64`, verified distinct by render. Previously all four were identical, and setting a `Title` silently upgraded a Simple tooltip to a Rich one
- [x] `07` **`ToolTipPainterFactory` had zero call sites** — `ApplyConfig` hard-coded `new BeepStyledToolTipPainter()`, so `PreviewToolTipPainter`, `TourToolTipPainter` and `GlassToolTipPainter` (~700 lines) had **never executed**. That is why Preview/Tour/Glass looked identical in the first contact sheet. Factory now wired; an explicitly assigned `Painter` still wins
- [x] `07` Painter exceptions are now contained — `OnPaint` falls back to the default painter and logs, instead of letting WinForms replace the whole tooltip with its red-X error box
- [x] **`PreviewToolTipPainter` fixed** — it set `WrapMode.Clamp` on a `LinearGradientBrush`, which GDI+ rejects (only Tile modes are legal), so the setter threw `ArgumentException("Parameter is not valid")` on *every* paint. One line destroyed the whole painter, unnoticed because the painter had never been instantiated. Tile (the default) is correct here anyway since the fill and gradient rectangles are identical
- [x] **`GlassToolTipPainter` magenta fixed** — `CustomToolTip` uses `TransparencyKey = Color.Magenta`, and the painter alpha-blended onto that magenta base. The blend never equals the key exactly, so it was not punched out and rendered as a solid magenta box. Colour-key transparency and alpha blending cannot be combined (true glass needs a layered window); the frosted look is now composited against a light base and filled opaquely, leaving overlays free to use alpha
- [x] **`GlassToolTipPainter` clipping fixed** — it draws with the theme's `TitleStyle`/`BodyStyle` but inherited a `CalculateSize` measuring with the base painter's smaller fonts: sized for one font, filled with another, body text clipped. Same defect that clipped every label in BeepTree. Override measures with the fonts it draws with, in **two passes**, since height depends on the width the text wraps at
- [x] All 7 variants verified correct by render: Simple / Rich / Card / Shortcut distinct, Preview shows its skeleton, Tour shows step badge + nav, Glass shows a frosted panel with title and body intact
- [ ] `07` Remaining: wire `LinkClicked`, custom content host
- [x] **Never-read config properties: 6 → 1.** Only `Pinnable` remains, and that is `10`'s ownership decision rather than a gap to quietly fill
- [x] `08` **Sizing against the resolved side** — `CalculateResponsiveSize` clamped only against 80% of the whole screen, so a tooltip above an anchor near the top of the display could be sized far taller than the gap it had to live in. New `AvailableSpaceFor` reports what fits on the side actually chosen (Floating UI's `size` middleware) and the tooltip clamps to it, re-resolving placement if the clamp changed its size
- [x] `08` Default max width is now a readable 360px DPI-scaled, not a fraction of the monitor — 80% of a 4K display is not a tooltip. Verified: long text caps at 360px on a 3440px screen, short text hugs at 150px, and a tooltip requested above a top-edge anchor flips below rather than escaping the working area
- [ ] `08` Remaining: scrollable body for content that still exceeds the clamp (today it ellipsises)
- [x] `09` **A tooltip was renaming the control it described** — `SetTooltip` wrote `config.Title` into `control.AccessibleName`, which *replaces* the name: a button labelled "Save" with a tooltip titled "Save document" announced as "Save document". Title and text now go to `AccessibleDescription` only (`aria-describedby`, not `aria-label`)
- [x] `09` **Removing a tooltip destroyed the host's accessibility text** — `RemoveTooltip` ended with `AccessibleDescription = string.Empty`, so a control with its own description lost it permanently. Prior name/description are captured on attach and restored on detach, verified end to end
- [x] `09` **Two corrections to my own audit**: reduced motion *is* implemented (`SPI_GETCLIENTAREAANIMATION`, consulted by `ShowAsync`/`HideAsync`) and `MinContrastRatio` *is* enforced (`EnforceContrastIfNeeded`). The plan claimed both were missing. Reduced motion is now asserted rather than assumed
- [ ] `09` Remaining: UIA notification so screen readers announce a tooltip on show; high-contrast render pass across painters
- [x] `10` **Pinning duplication resolved by deletion.** `BeepPinnedTooltip` (200 lines, deriving from `BeepPopover`) had **zero references anywhere in the solution** — dead code, like the animator. The integrated path already owns placement, arrow tracking, auto-update, theming and accessibility; a standalone control duplicated all of it and received none of this program's fixes. Control deleted
- [x] `10` `ToolTipConfig.Pinnable` implemented for real: painter draws a pin toggle (outline unpinned / filled pinned); `ToolTipHeaderButtons` computes the pin and close rects **once** so the painter and the hit-test cannot disagree; pinned tooltips are exempt from the `Duration` timer and from hide-on-leave
- [x] `10` Verified end to end — pin rect lands inside the tooltip, clicking it sets `IsPinned`, and a tooltip with a 250ms `Duration` is still on screen 600ms after pinning
- [x] **Every `ToolTipConfig` property is now read by something.** Six were declared, documented and consumed by nothing (`HideDelay`, `TriggerMode`, `PersistOnHover`, `LoadPreviewAsync`, `StepTitle`, `Pinnable`). The reflection check passes clean, so a seventh cannot be added silently
- [x] **Harness: 59 checks, all passing** (from 5 passed / 15 failed at the start)
- [x] `11` **Contrast "enforcement" could not enforce anything** — `AdjustLuminance` multiplies the colour by 1.2 or 0.8 **once** and returns, and `0 x 1.2` is still `0`. Under `MaterialYouTheme` (`#080808` on `#101010`) it produced `#090909`, so **all 21 types measured below 4.5:1 after enforcement**. The nudge is kept for near-misses (preserving hue); the fallback is now `ColorUtils.EnsureReadable` — the same helper the grid uses, not a second implementation. Every type on every tested theme now passes
- [x] `11` Caught a false alarm in my own check first: it measured the *raw* theme resolution rather than the post-enforcement colours the tooltip actually paints, reporting failures the product already fixed. It now runs the same two stages the tooltip does
- [ ] `11` **14 of 21 `ToolTipType` values share one colour pair** (`Default`, `Help`, `Validation`, `Interactive`, `Descriptive`, `Notification`, `Tutorial`, `Shortcut`, `Badge`, `Preview`, `ContextMenu`, `Status`, `Hint`, `Custom`) because the resolver branches on only seven. `Accent` and `Info` are identical everywhere; `Primary` joins them under MaterialYou. The semantic set (Success/Warning/Error/Info/Default) *is* distinct on every theme, so the enum promises more than it delivers rather than being broken — either give the rest their own tokens or shrink the enum
- [ ] `11` Remaining: default `Style` from the active theme instead of the `Material3` literal
- [x] `12` **Measured first, and the plan's assumption was wrong**: tooltip windows do *not* accumulate — 40 show/hide cycles left zero live windows. The window-per-show behaviour is a performance cost, not a leak, so pooling is an optimisation rather than the bug the plan implied
- [x] `12` **The real leak was the anchor map** — `_controlTooltips`/`_attachedHandlers` are keyed by `Control` and nothing released them when a control was disposed without `RemoveTooltip` (the normal case when a form closes). Measured **20 disposed anchors retained** after 20 cycles; the manager now subscribes to `Control.Disposed` and releases itself. Same cycle now retains none
- [x] `12` **Identity keys fixed** — the tooltip key was `control_{GetHashCode()}_{Ticks}`. `GetHashCode` is not an identity (two live controls can share one, and it is not stable per control), so the ticks suffix papered over collisions and produced a key that was neither unique-by-construction nor stable. Now a GUID. The delay-group key uses `RuntimeHelpers.GetHashCode` so a container overriding `GetHashCode` by value cannot merge two unrelated toolbars into one delay group
- [ ] `12` Remaining: window pooling (perf), per-tooltip expiry timers replacing the 5s sweep, thread-affinity audit
- [x] **Harness: 61 checks, all passing.** Build clean at 0 errors, 0 warnings
- [ ] `13` **Verification harness — build first.** Render + magnified crops, geometry invariants, a perimeter edge sweep, contact sheets, and a reflection check for "declared but never read" config properties that would have caught all three above

---

## BeepTree Layout Correctness And Architecture

Per-phase documents: `TheTechIdea.Beep.Winform.Controls/Trees/plans/correctness/`
(`README.md` + `phase-1..5-*.md`). This program is about correctness and architecture of the layout
and paint path; it does **not** replace the feature-oriented `Trees/plans/phase-1..6-*.md` series
(50/320 complete), which tracks data binding, multi-column and enterprise capability.

Reported: second-level nodes align incorrectly. Reproduced with `scratchpad/TreeProbe`, which builds
a tree mixing expandable nodes and leaves at every level, dumps computed rectangles per node, and
renders to PNG. Root cause: `RecalculateLayoutCache` advanced the X cursor past the expander only
for nodes **with children**, so a leaf started its text one box-width left of an expandable sibling.
With `IndentWidth = 16` and `BoxSize = 14`, a leaf at level N landed within 2px of a parent at level
N-1 — children rendered at their own parent's indent and the hierarchy read wrong.

### Phase 0 - Stability
- [x] Fixed `InvalidCastException: BeepTreeAccessibleObject cannot be cast to ControlAccessibleObject` thrown from `Control.OnHandleDestroyed` — the tree crashed whenever its handle was torn down (closing a form, re-parenting, theme change) *after* anything had touched its accessibility object. `CreateAccessibilityInstance()` returned a plain `AccessibleObject`, but WinForms stores that return value in a property-store slot typed `ControlAccessibleObject`, so the cast fails later, far from the cause. Now derives from `Control.ControlAccessibleObject`
- [x] Swept every other `CreateAccessibilityInstance` override in the assembly (Grid, CheckBox, Panel, TextBox, MenuBar) — all already derive correctly; BeepTree was the only one
- [x] Reproduced and verified in the probe: touching `AccessibilityObject` forces creation, then closing the form destroys the handle. This is why the bug hid — layout runs never touch accessibility, so the instance was never created

### Phase 1 - Slot Reservation And Per-Level Alignment
- [x] Expander slot reserved for every node, glyph drawn only when there are children — `ToggleRectContent` stays `Empty` for leaves so painters and hit-testing are unchanged; only the cursor advances
- [x] Verified by probe: distinct text X per level went from `L1 [16, 34] / L2 [32, 50]` to one value per level (`18 / 34 / 50 / 66`), and the render now reads as a correct hierarchy
- [x] Icon slot had the identical defect (`currentX` advanced only when `ImagePath` was set) — fixed behind a new `IconSlotMode` (`Trees/Models/IconSlotMode.cs`) rather than an unconditional reserve, which would have indented every label in text-only trees. Default `WhenAnyNodeHasIcon`; also `Always` and `Never`. One `AnyVisibleNodeHasIcon()` pre-pass sets a single flag before the geometry loop
- [x] Async icon arrival — investigated, no action needed. The original plan warned the flag could flip after layout; it cannot, because both the flag and the per-row decision key off `ImagePath`, which the host sets up front. The async loader only fetches pixels for an already-declared path
- [x] Checkbox slot verified rather than assumed — every `checkbox=True` case passes, text X shifting uniformly by the box width (`[18,34,50,66]` → `[36,52,68,84]`)
- [x] Invariants asserted, not printed: one text X per level, indent strictly increasing with depth, and no intra-row rect overlap — across `{icons} × {checkbox} × {3 slot modes}` = 12 cases, all passing
- [x] Caught an over-strict assertion in the process: `Never` deliberately produces ragged text in mixed-icon trees, so the probe now encodes the contract each mode offers instead of one blanket rule
- [ ] Same assertions at 150% / 200% DPI (deferred to Phase 5 with the rest of the DPI work — `GetScaled*` rounding is where off-by-one indent appears)

### Phase 2 - Collapse The Two Competing Layout Engines
- [x] Measured the divergence before changing anything: the two engines produced text X **4px apart on every node** (`control 42 / helper 46`, `58/62`, `74/78`, …)
- [x] **Correction to the previous entry.** I claimed `BeepTreeLayoutHelper.RecalculateLayout` was "never called by the control". Wrong — my census grepped `layoutHelper\.[A-Za-z]*`, which silently misses `_layoutHelper?.RecalculateLayout()` because of the null-conditional. Deleting the method and letting the compiler answer found **three live callers**
- [x] Two of those callers were actively harmful: the `ControlStyle` and `UseFormStylePaint` setters called `RecalculateLayoutCache()` and then *immediately* `_layoutHelper?.RecalculateLayout()`, overwriting correct geometry with the divergent version. Changing either property at runtime installed the wrong indentation every time — not latent at all. Both follow-up calls removed
- [x] Third caller was `BaseTreePainter`'s paint-time fallback for an empty cache, so a frame arriving with an empty cache drew the tree 4px off from the next frame. Now routes through the single engine
- [x] Duplicate geometry deleted (`RecalculateLayout`, `RecalculateLayoutAsync`, `CalculateNodeLayout`, `CalculateMultiColumnLayout`, `GetCellText`); `RecalculateLayout()` survives only as a thin delegate to the control's engine, since three callers legitimately need "recompute and return the layout"
- [x] Multi-column ported to `BeepTree.CalculateMultiColumnCells` — and this was a real bug, not tidying: `BaseTreePainter` and `BeepTreeCellEditor` both read `GetCellRect(colIndex)`, but the only code that ever called `SetCellRect` sat on the unreachable path, so **every cell rect was `Rectangle.Empty` and multi-column mode rendered no columns at all**
- [x] First-column rect fixed while porting: `column.Width - baseIndent` shrank column 0 by each node's own depth, so column edges did not line up down the tree. Cells now keep full declared width with content indented inside
- [x] Off-UI-thread layout removed — the `> 10000` node `RecalculateLayoutAsync` auto-trigger mutated the cache paint reads from
- [x] `GetVirtualizationRange` now takes a count instead of materialising a `List<SimpleItem>` of the whole tree on every scroll
- [x] Verified: probe reports "engines agree on geometry", all Phase 1 alignment assertions still pass, render unchanged
- [ ] Multi-column not yet verified by eye — the geometry is now real but no one has looked at a multi-column tree
- [ ] `VirtualizeLayout` is nominal: every visible node is laid out eagerly, so the viewport range is bookkeeping and nothing is virtualized. Implement it or retire the property

### Phase 3 - Text Rectangle, Truncation And Content Width
- [x] Ruled out the rect by measuring it: every text rect had 10px of slack (`rectW=78 neededW=68` etc), so the fault was on the draw side, not the measurement
- [x] **Real cause: the painter drew with a different font than the layout measured with.** Layout uses the themed font (`Segoe UI 8pt` via `GetNodeFont`); `AntDesignTreePainter` drew with `_regularFont`, snapshotted from `owner.TextFont` in its constructor (`Arial 10pt`). `UseThemeFont` defaults to true, so they always diverged — a 10pt string drawn into an 8pt-sized rect overflows and is cut mid-word
- [x] Lesson recorded: **the default `TreeStyle` is `AntDesign`, not `Standard`.** The first fix went into `StandardTreePainter`, which was not the active painter, so the render did not change. Printing `GetCurrentPainter().GetType().Name` settled it immediately — ask the object what it is before theorising
- [x] `BaseTreePainter.NodeTextFlags` — one constant for measurement and drawing, with matching `NoPadding` and `EndEllipsis` for graceful truncation
- [x] `BaseTreePainter.DrawNodeLabel(...)` — renders a node label with the font its rect was measured with; the single entry point painters should use
- [x] `BeepTree.GetNodeFont()` — one font resolution replacing three (layout used `ToFont`, `BaseTreePainter` used the DPI-scaled `ToFontForControl`, painters used their own constructor snapshot)
- [x] `AntDesignTreePainter` (default) and `StandardTreePainter` converted; verified by render — all labels render in full including a deliberately over-long one, and alignment assertions still pass
- [x] **All 25 painters converted** to `DrawNodeLabel`. Node-label calls identified by their text argument (`node.Item.Text`, and the `text` parameter of `PaintText` overrides) rather than rewriting all ~70 `DrawText` calls, so badge/count/subtitle draws were left alone
- [x] The fix is **not** "one font for everything" — three styles deliberately differ (`VercelClean` monospace, `FileBrowser` compact, `PillRail` bold-on-selection) and flattening them would erase their identity. Added `ITreePainter.GetNodeFont(BeepTree)`: the painter declares the font it draws labels with and the layout measures with *that*. `PillRail` reports the bold (widest) variant so the emphasised label cannot clip
- [x] `PillRail` also stopped allocating and disposing a bold `Font` on every selected row on every frame — cached now
- [x] Verified by contact sheet: all 25 styles rendered and reviewed, labels render in full everywhere, alignment assertions still pass
- [x] **`FigmaCard` blank render fixed** — a misplaced closing brace left `if (isSelected || isHovered)` open to the end of `PaintNode`, so the toggle, checkbox, icon, eye and label were *all* conditional on hover/selection and an idle tree drew nothing. The tell it was a brace slip rather than intent: STEP 2 re-tests the same condition, which is only meaningful if the surrounding block does not. Block now closes after the card background
- [x] **`StripeDashboard` truncation fixed** — same defect class as the rest of this program: the painter made room for its `99+` metric badge by shrinking the text rect (`textRect.Width - 40`), but the layout had sized that rect as measured-text + 10, so labels collapsed to `Root ...`. Added `ITreePainter.GetLabelTrailingReserve()` (0 by default, `MetricBadgeWidth` for this style); the layout adds it, so the painter's subtraction leaves the label its full measured width. The grey blobs were the badge itself
- [x] **`StripeDashboard` badge position fixed** — the badge rect was built from raw content coordinates while every other element in that method is transformed to viewport, so it ignored scroll and drifted off its row
- [x] Both verified in the re-rendered contact sheet: FigmaCard draws a full tree; StripeDashboard shows complete labels with the badge trailing each one
- [ ] Tooltip on truncated labels
- [ ] Keep `RowWidth` content-only — the existing comment records a real bug where widening it forced a spurious horizontal scrollbar; use a separate row-band rect for full-width selection

### Phase 4 - Layout And Paint Hot-Path Efficiency
- [x] `GetCurrentPainter()` and the theme font hoisted out of the per-node layout loop — the font was being built **twice per node** (~20,000 undisposed `Font` allocations per pass on a 10k tree). Done while unifying fonts in Phase 3
- [ ] `UpdateViewportLayout` materialises `_layoutCache.Select(n => n.Item).ToList()` on every scroll just to compute a range that needs only counts and heights
- [ ] Audit `Invalidate()` calls against the rule learned in BeepGridPro: change-gate them and scope to affected rows (hover is a per-mouse-move path — the same one that produced visible flicker in the grid toolbar)
- [ ] Measure repaint counts before/after, as was done for the grid; do not start this phase before Phase 2 or the work is done twice

### Phase 5 - Permanent Verification Harness
- [ ] Promote `scratchpad/TreeProbe` into the repo — it currently reaches `_visibleNodes` via reflection from a temp folder
- [ ] Convert printed tables into failing assertions: per-level alignment, monotonic indent, leaf/parent parity, no intra-row rect overlap, positive heights, increasing Y
- [ ] Matrix: leaf/parent × icon × checkbox × {100%, 150%, 200%} DPI × single/multi-column (DPI rounding in `GetScaled*` is where off-by-one indent appears)
- [x] Contact sheet built and reviewed — all **25** styles render in one sheet (`painters.png`). Pulled forward from this phase to verify the Phase 3 painter sweep, and immediately earned its keep by surfacing the blank `FigmaCard` and truncated `StripeDashboard` renders
- [ ] Store baselines so future layout changes diff rather than re-eyeball

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


## BeepGridPro Header And Toolbar Commercialization

### Phase 1: Toolbar geometry — DONE (verified by reading, already in the tree)
- [x] Single `CenterY(bandY, bandHeight, itemHeight)` used for every icon rect
- [x] Every button gets the full band height as its hit target
- [x] Search icon moved inside `SearchBoxRect`; `SearchIconWidth` is the one text inset
- [x] Title measured with a floor and clamped to the space left after the reserved right sections
- [x] Collapse order: labels → export → overflow → search shrink, via a two-pass reservation
- [x] Overflow tested against `rightLimit` (derived from `bounds.Right`), not `bounds.Width`
- [x] `BadgeRect` clamped inside the band via `ClampToBand`

### Phase 2: Painter owns header geometry — DONE
- [x] `HeaderCellLayout` (TextRect / SortIndicatorRect / MenuButtonRect / SortHitRect)
- [x] `BaseHeaderPainter.CalculateHeaderCellLayout` with reserved sort slot and >=24px targets
- [x] `GridRenderHelper.DrawHeaderCell` consumes it and records the hit rects
- [x] Per-layout-pass `new GridColumnHeadersPainterHelper` allocation removed

### Phase 3: Commercial sort/filter interaction — DONE
- [x] Sort slot reserved whenever the column is sortable (caption no longer reflows on sort)
- [x] Caption click sorts; shift-click appends; multi-sort order badge drawn
- [x] One column menu button opening sort + filter + clear
- [x] Removed a duplicate sort path in `GridInputHelper`: a second `ToggleColumnSort` on the
      general header click called the non-additive overload, so a shift-click landing in a cell's
      padding strip replaced the sort instead of appending. The fall-through guard was kept.

### Phase 4: Header painters do not reach the pixels — OPEN, needs a decision
Measured, not inferred: `scratchpad/GridProbe` renders the header band per `NavigationStyle` and
compares pairwise, in both the resting and hovered states.

- Header **height** is style-driven (Standard 23, Material 41, AGGrid 33, Fluent 41, Telerik 37,
  Compact 23) — that is `CalculateHeaderHeight`, which the plan already identified as the painters'
  only real use.
- Where two styles share a height, the rendered headers are **pixel-identical**: Standard vs Compact,
  Material vs Fluent. Same result hovered as at rest.
- Cause: `GridColumnHeadersPainterHelper.DrawColumnHeaders` — the only path that calls
  `PaintHeaders` → `PaintHeaderCell`, where every per-style visual lives — has **zero callers**. The
  live path is `GridRenderHelper.DrawColumnHeaders`, which asks the painter only for
  `CalculateHeaderCellLayout` and `PaintColumnMenuButton`.

- [ ] **Decision required.** Two coherent routes, and they are not equivalent:
  - **(a) Move the per-style visuals into the granular seam** — each painter overrides
    `PaintHeaderBackground` / `PaintHeaderText` / `PaintSortIndicator` (12 painters x 3), then delete
    the dead `PaintHeaders` / `PaintHeaderCell` path and `GridColumnHeadersPainterHelper`. Keeps one
    geometry system and the renderer's hit-rect registration intact.
  - **(b) Route the live pipeline through `PaintHeaderCell`** — what the plan literally proposed, but
    each painter's `PaintHeaderCell` computes its own icon geometry, which would reintroduce the
    second geometry system Phase 2 just removed.

  Attempting the naive middle path made things measurably **worse** and was reverted: delegating the
  background to `painter.PaintHeaderBackground` discards the grid-level `UseHeaderGradient` and
  `UseElevation` properties the painter knows nothing about, and style differences shrank from
  3.9% to 2.9%. Recorded so it is not retried.
