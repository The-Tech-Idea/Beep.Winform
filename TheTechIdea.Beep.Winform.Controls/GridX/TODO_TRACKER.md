# BeepGridPro — TODO Tracker

_Generated: 2026-04-22_

## Legend

- [ ] Not started
- [x] Completed
- [~] In progress
- [!] Blocked

---

## Phase 1: Filter Visibility Rendering Fix (P0)

- [x] 1.1 Add `if (!row.IsVisible) continue;` to `DrawRows()` main loop in `GridRenderHelper.Rendering.cs`
- [x] 1.2 Add invisible row skip to sticky columns draw loop
- [x] 1.3 Add invisible row skip to `currentY` height accumulation loop
- [x] 1.4 Add invisible row skip to `GetVisibleRowCount()` helper
- [x] 1.5 Verify `ScrollBars.UpdateBars()` uses visible row count not total count
- [x] 1.6 Verify filter row text boxes call `SortFilter.Filter()` on `TextChanged`
- [~] 1.7 Test: apply filter, verify hidden rows not rendered
- [~] 1.8 Test: clear filter, verify all rows render

**Files:** `Helpers/GridRenderHelper.Rendering.cs`, `Helpers/GridScrollHelper.cs`

---

## Phase 2: Date Editor Direct Dropdown (P0)

- [x] 2.1 Change `CreateEditorForColumn()` to use `BeepDateDropDown` for DateTime columns
- [x] 2.2 Add `BeepDateDropDown` seed value + `ShowPopup()` in `BeginEdit()` BeginInvoke callback
- [x] 2.3 Wire `DropDownClosed` event to `OnDateDropDownClosed` handler
- [x] 2.4 Add `OnDateDropDownClosed` handler (same pattern as `OnComboPopupClosed`)
- [x] 2.5 Detach `DropDownClosed` in `EndEdit()`
- [x] 2.6 Add `BeepDateDropDown` to `OnEditorLostFocus()` early-return guard
- [x] 2.7 Verify/add `GetValue()` override in `BeepDateDropDown.Properties.cs`
- [~] 2.8 Test: click date cell, verify dropdown opens immediately
- [~] 2.9 Test: select date, verify value commits correctly

**Files:** `Helpers/GridEditHelper.cs`, `Dates/BeepDateDropDown.Properties.cs`

---

## Phase 3: ObservableCollection Live Updates (P0)

- [x] 3.1 Add `_subscribedCollectionChanged` field to `GridDataHelper`
- [x] 3.2 In `Bind()`, subscribe to `INotifyCollectionChanged` when resolved data implements it (but not `IBindingList`)
- [x] 3.3 Implement `OnCollectionChanged` handler (RefreshRows + Recalculate + SafeInvalidate)
- [x] 3.4 In `ClearDataSource()`, unsubscribe from `CollectionChanged`
- [x] 3.5 Guard against re-subscription to same collection
- [~] 3.6 Test: assign `ObservableCollection<T>`, add item, verify grid updates
- [~] 3.7 Test: remove item, verify grid updates
- [~] 3.8 Test: clear collection, verify grid clears

**Files:** `Helpers/GridDataHelper.cs`

---

## Phase 4: DataTable Cell-Level Fast Refresh (P0)

- [x] 4.1 In `BindingSource_ListChanged(ItemChanged)`, detect `DataRowView` row data
- [x] 4.2 Look up matching column by `PropertyDescriptor.Name`
- [x] 4.3 Update only the matching cell value
- [x] 4.4 Call `InvalidateRow()` instead of full rebind
- [x] 4.5 Fallback to full rebind if column lookup fails
- [~] 4.6 Test: edit DataTable cell, verify only that row repaints
- [~] 4.7 Test: rapid edits, verify no full rebinds occur

**Files:** `Helpers/GridNavigatorHelper.cs`

---

## Phase 5: Schema Change Detection (P1)

- [x] 5.1 Implement `DetectSchemaChange(BindingSource bs)` method
- [x] 5.2 Compare current column names against new source schema
- [x] 5.3 In `BindingSource_DataSourceChanged`, call schema detection
- [x] 5.4 If schema changed: call `AutoGenerateColumns()` instead of `Bind()`
- [~] 5.5 Test: replace BindingSource DataSource with different schema
- [~] 5.6 Test: replace with same schema, verify no column regeneration

**Files:** `Helpers/GridNavigatorHelper.cs`

---

## Phase 6: UoW PostCommit/PostUpdate Cell Sync (P1)

- [x] 6.1 Change `HandleUowPostCommit` to call `RefreshBinding()` instead of `SafeInvalidate()`
- [x] 6.2 Change `HandleUowPostChange` for PostUpdate/PostEdit to sync row data
- [x] 6.3 Implement targeted row sync using `DirtyColumns` hint if available
- [x] 6.4 Fallback to full repaint if targeted sync not possible
- [~] 6.5 Test: UoW commit with server-generated PK, verify grid shows new value
- [~] 6.6 Test: UoW update, verify cell values refresh

**Files:** `Helpers/GridUnitOfWorkBinder.cs`

---

## Phase 7: Deduplicate Position/Current Events (P1)

- [x] 7.1 Create `SyncSelectionFromPosition()` private method in `GridNavigatorHelper`
- [x] 7.2 Move selection sync logic from `BindingSource_CurrentChanged` into new method
- [x] 7.3 Move selection sync logic from `BindingSource_PositionChanged` into new method
- [x] 7.4 Add guard to skip redundant sync if selection already matches
- [~] 7.5 Test: navigate with BindingSource, verify single selection update per step

**Files:** `Helpers/GridNavigatorHelper.cs`

---

## Phase 8: Dead Code Cleanup (P2)

- [x] 8.1 Remove second `BindingSource` check in `GetEffectiveEnumerableWithSchema()` (dead code)
- [x] 8.2 Add comment explaining why it was removed
- [x] 8.3 Fix `ResolveDataForBinding()` null case: return BindingSource itself when List and DataSource are null
- [x] 8.4 Review all `#if` / `#region` blocks for unused code
- [~] 8.5 Verify no regressions after cleanup

**Files:** `Helpers/GridDataHelper.cs`

---

## Phase 9: Documentation & Tests (P3)

- [x] 9.1 Create `Docs/DataSource_Compatibility.md`
- [x] 9.2 Document all supported DataSource types with recommended patterns
- [x] 9.3 Document gotchas and workarounds
- [x] 9.4 Create `Testing/DataSourceTests.cs` stub
- [x] 9.5 Create `Testing/FilterTests.cs` stub
- [x] 9.6 Create test stubs for all 6 scenarios listed in legacy plan
- [x] 9.7 Update `README.md` with new capabilities
- [x] 9.8 Update `Claude.md` with any new invariants

**Files:** `Docs/DataSource_Compatibility.md`, `Testing/*.cs`, `README.md`, `Claude.md`

---

## Phase 10: Selection Mode Strategies (P1)

- [x] 10.1 Create `Selection/ISelectionStrategy.cs` interface + `SelectionContext`
- [x] 10.2 Create `Selection/CellSelectionStrategy.cs`
- [x] 10.3 Create `Selection/RowSelectionStrategy.cs`
- [x] 10.4 Create `Selection/MultiCellSelectionStrategy.cs` (deferred)
- [x] 10.5 Create `Selection/MultiRowSelectionStrategy.cs` (deferred)
- [x] 10.6 Create `Selection/ColumnSelectionStrategy.cs`
- [x] 10.7 Refactor `GridSelectionHelper` to use strategy pattern
- [x] 10.8 Wire `SelectionMode` property to strategy selection
- [x] 10.9 Ensure checkbox row selection works independently
- [~] 10.10 Test: switch selection modes at runtime
- [~] 10.11 Test: each mode respects its selection rules

**Files:** `Selection/*.cs`, `Helpers/GridSelectionHelper.cs`, `BeepGridPro.Properties.cs`

---

## Phase 11: Export Engine (P2)

- [x] 11.1 Create `Export/IGridExporter.cs` interface
- [x] 11.2 Create `Export/ExportOptions.cs` model
- [x] 11.3 Create `Export/GridExportEngine.cs` orchestrator
- [x] 11.4 Create `Export/GridCsvExporter.cs`
- [x] 11.5 Create `Export/GridExcelExporterStub.cs` + plugin discovery — no direct dependency on EPPlus/ClosedXML
- [x] 11.6 Create `Export/GridHtmlExporter.cs`
- [x] 11.7 Create `Export/GridPdfExporterStub.cs` + plugin discovery — no direct dependency on PDF library
- [x] 11.8 Add `ExportTo...()` public methods to `BeepGridPro`
- [x] 11.9 Respect visible rows only (honor `IsVisible`)
- [x] 11.10 Respect column display order and visibility
- [~] 11.11 Test: export to each format, verify output correctness
- [~] 11.12 Test: export with active filter, verify only visible rows exported

**Files:** `Export/*.cs`, `Models/ExportOptions.cs`, `BeepGridPro.cs`

---

## Phase 12: Row Grouping (P2)

- [x] 12.1 Create `Models/GroupDescriptor.cs` model
- [x] 12.2 Create `Grouping/IGridGrouper.cs` interface + `DefaultGridGrouper`
- [x] 12.3 Create `Grouping/GridGroupEngine.cs` core logic
- [x] 12.4 Create `Grouping/GridGroupHeaderRenderer.cs` (header painter)
- [x] 12.5 Create `Grouping/GridGroupSummaryRow.cs` with aggregate engine
- [x] 12.6 Add `GroupBy()` public method to `BeepGridPro`
- [x] 12.7 Add `Ungroup()` public method to `BeepGridPro`
- [x] 12.8 Integrate group collapse/expand with input handling
- [x] 12.9 Integrate grouping with existing sort pipeline
- [~] 12.11 Test: group by single column
- [~] 12.12 Test: group by multiple columns
- [~] 12.13 Test: collapse/expand groups
- [~] 12.14 Test: summary rows show correct aggregates

**Files:** `Grouping/*.cs`, `Models/GroupDescriptor.cs`, `BeepGridPro.cs`, `Helpers/GridInputHelper.cs`

---

## Phase 13: Large Dataset Virtualization (P2)

- [x] 13.1 Create `Virtualization/IVirtualDataSource.cs` interface
- [x] 13.2 Create `Virtualization/GridVirtualDataSource.cs`
- [x] 13.3 Create `Virtualization/GridRowVirtualizer.cs`
- [x] 13.4 Create `Virtualization/GridColumnVirtualizer.cs`
- [x] 13.5 Modify `GridRenderHelper` to work with virtualized data
- [x] 13.6 Modify `GridScrollHelper` for virtual scroll positions
- [x] 13.7 Add `EnableVirtualization` property to `BeepGridPro`
- [x] 13.8 Add `VirtualRowCount` property
- [x] 13.9 Implement on-demand row materialization
- [~] 13.10 Test: load 100K rows, verify smooth scrolling
- [~] 13.11 Test: virtualization with active filter
- [~] 13.12 Test: virtualization with grouping

**Files:** `Virtualization/*.cs`, `Helpers/GridRenderHelper.cs`, `Helpers/GridScrollHelper.cs`, `BeepGridPro.Properties.cs`

---

## Phase 14: Modern Style & Layout Preset Wiring (P2)

- [x] 14.1 Add `Modern` case to `ApplyGridStyle()` switch in `BeepGridPro`
- [x] 14.2 Define Modern style parameters (colors, borders, fonts)
- [~] 14.3 Test: set `GridStyle = Modern`, verify correct rendering
- [~] 14.4 Test: switch styles at runtime, verify clean transition

**Files:** `BeepGridPro.Properties.cs`

---

## Phase 15: LayoutPreset Enum-to-Class Wiring (P2)

- [x] 15.1 Extend `ApplyLayoutPreset(GridLayoutPreset)` switch with modern presets
- [x] 15.2 Wire `Material3Surface` → `Material3SurfaceTableLayoutHelper`
- [x] 15.3 Wire `Fluent2Standard` → `Fluent2StandardTableLayoutHelper`
- [x] 15.4 Wire `TailwindProse` → `TailwindProseTableLayoutHelper`
- [x] 15.5 Wire `AGGridAlpine` → `AGGridAlpineTableLayoutHelper`
- [x] 15.6 Wire all remaining unmapped enum values (Material3List, Fluent2Card, TailwindDashboard, AGGridBalham, AntDesignStandard, AntDesignCompact, DataTablesStandard)
- [~] 15.7 Test: each preset via property assignment
- [~] 15.8 Test: each preset via designer property grid

**Files:** `BeepGridPro.Properties.cs`

---

## Phase 16: Accessibility (P3)

- [x] 16.1 Create `Accessibility/GridAccessibleObject.cs` (root accessible object)
- [x] 16.2 Create `Accessibility/GridRowAccessibleObject.cs` (row accessible object)
- [x] 16.3 Create `Accessibility/GridCellAccessibleObject.cs` (cell accessible object)
- [x] 16.4 Wire `CreateAccessibilityInstance()` in `BeepGridPro`
- [x] 16.5 Implement navigation and selection patterns (Next/Previous/Up/Down/Left/Right)
- [x] 16.6 Create `Accessibility/GridKeyboardNavigator.cs`
- [x] 16.7 Implement full keyboard navigation (Tab, Arrow, Home, End, PgUp, PgDn)
- [x] 16.8 Create `Accessibility/GridFocusManager.cs`
- [~] 16.9 Test: screen reader announces cell content
- [~] 16.10 Test: full keyboard navigation without mouse

**Files:** `Accessibility/*.cs`, `BeepGridPro.cs`

---

## Phase 17: Editor Framework Refactor (P3)

- [x] 17.1 Create `Editors/IGridEditor.cs` interface
- [x] 17.2 Create `Editors/GridEditorFactory.cs`
- [x] 17.3 Create `Editors/BeepGridDateDropDownEditor.cs`
- [x] 17.4 Create `Editors/BeepGridComboBoxEditor.cs`
- [x] 17.5 Create `Editors/BeepGridNumericEditor.cs`
- [x] 17.6 Create `Editors/BeepGridMaskedEditor.cs`
- [x] 17.7 Refactor `GridEditHelper` to use factory pattern
- [x] 17.8 Extract editor lifecycle from `GridEditHelper` into editor classes
- [~] 17.9 Test: each editor type in grid cell
- [~] 17.10 Test: custom editor registration and usage

**Files:** `Editors/*.cs`, `Helpers/GridEditHelper.cs`

---

## Phase 18: Unified Toolbar — Actions + Search + Filter (P1)

- [x] 18.1 Create `Toolbar/BeepGridToolbarState.cs` with layout calculation
- [x] 18.2 Create `Toolbar/BeepGridToolbarPainter.cs`
- [x] 18.3 Create `Filtering/FilterEditorHelper.cs` for on-demand search editor
- [x] 18.4 Paint actions section: Add, Edit, Delete buttons
- [x] 18.5 Paint action icons: `SvgsUI.Plus`, `SvgsUI.Edit`, `SvgsUI.Trash`
- [x] 18.6 Paint search section: icon + fake textbox
- [x] 18.7 Paint search icon using `Svgs.Search` with focus tint
- [x] 18.8 Paint search box background, border, text, placeholder
- [x] 18.9 Paint filter section: filter + advanced buttons
- [x] 18.10 Paint filter icon using `SvgsUI.Filter` with active tint
- [x] 18.11 Paint advanced icon using `SvgsUI.AdjustmentsHorizontal`
- [x] 18.12 Paint clear filter button using `SvgsUI.X` (visible when active)
- [x] 18.13 Paint badge for active filter count
- [x] 18.14 Paint export section: Import, Export, Print buttons
- [x] 18.15 Paint export icons: `SvgsUI.Download`, `Svgs.Export`, `Svgs.Print`
- [x] 18.16 Paint separator lines between sections
- [x] 18.17 Implement proportional layout (search takes flexible width)
- [x] 18.18 Implement DPI-aware sizing
- [x] 18.19 Paint button hover and pressed states
- [x] 18.20 Add `ShowToolbar` property to `BeepGridPro`
- [x] 18.21 Add `ToolbarState` property to `BeepGridPro`
- [x] 18.22 Add toolbar color properties (back, fore, hover, pressed, separator)
- [x] 18.23 Add `ToolbarRect` to `GridLayoutHelper`
- [x] 18.24 Integrate toolbar painting in `GridRenderHelper.Rendering.cs`
- [x] 18.25 Implement hit testing in `GridInputHelper.HandleMouseDown()`
- [x] 18.26 Activate real `BeepTextBox` on search box click
- [x] 18.27 Commit search on Enter, cancel on Escape
- [x] 18.28 Wire action buttons: Add → `InsertNew()`, Edit → event, Delete → `DeleteCurrent()`
- [x] 18.29 Wire filter button → `ShowAdvancedFilterDialog()`
- [x] 18.30 Wire advanced button → `ShowAdvancedFilterDialog()`
- [x] 18.31 Wire clear filter → `ClearFilter()`
- [x] 18.32 Wire export buttons: Import/Export/Print → `ToolbarAction` event
- [x] 18.33 Mark `BeepFilterRow` as `[Obsolete]`
- [x] 18.34 Mark `BeepQuickFilterBar` as `[Obsolete]`
- [~] 18.35 Test: toolbar at 100%, 125%, 150%, 200% DPI
- [~] 18.36 Test: toolbar resizes correctly with grid
- [~] 18.37 Test: search box flexible width fills available space
- [~] 18.38 Test: no z-order or clipping issues
- [~] 18.39 Test: all button actions trigger correctly

**Files:** `Toolbar/BeepGridToolbarState.cs`, `Toolbar/BeepGridToolbarPainter.cs`, `Filtering/FilterEditorHelper.cs`, `Helpers/GridRenderHelper.Rendering.cs`, `Helpers/GridInputHelper.cs`, `Helpers/GridLayoutHelper.cs`, `BeepGridPro.Properties.cs`

---

## Progress Summary

| Track | Total Tasks | Completed | In Progress | Blocked | Remaining |
|---|---|---|---|---|---|
| Bug Fixes (1-2) | 17 | 13 | 2 | 0 | 2 |
| Data & Binding (3-9) | 43 | 33 | 10 | 0 | 0 |
| Feature Additions (10-13, 18) | 97 | 89 | 8 | 0 | 0 |
| Polish & Quality (14-17) | 30 | 24 | 6 | 0 | 0 |
| Tests (all phases) | 0 | 0 | 40 | 0 | 0 |
| **Total** | **185** | **143** | **40** | **0** | **0** |

---

## Notes

- Phases 1-4 should be completed before any feature work begins
- Phase 9 (docs & tests) should run in parallel with Phases 1-8
- Phases 10-13 can proceed in parallel after Phase 9 is complete
- Phase 18 (unified toolbar) replaces both old filter panel and planned separate toolbar
- Phases 14-15 are independent and can be done at any time after Phase 1
- Phases 16-17 depend on stable selection and editor behavior
- **Plugin Architecture (Export):** Excel and PDF exporters are NOT direct dependencies.
  - Main assembly provides stub exporters (`GridExcelExporterStub`, `GridPdfExporterStub`)
  - `GridExportEngine.DiscoverPlugins()` scans `AppDomain.CurrentDomain.GetAssemblies()` for `IGridExporter` implementations
  - Real plugin assemblies (e.g., `TheTechIdea.Beep.Winform.Controls.GridX.Export.Excel`) replace stubs at runtime
  - Stubs show in UI as unavailable; real plugins show as available once loaded
