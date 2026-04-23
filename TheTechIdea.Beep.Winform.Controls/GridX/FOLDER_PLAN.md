# BeepGridPro — Folder Plan

This document describes the target folder structure for `GridX` after the BeepGridPro enhancement program.

## Current Structure (As-Is)

```
GridX/
├── Adapters/
│   └── BeepGridProAdapter.cs
├── Docs/
│   ├── BEEPSTYLING_INTEGRATION_SUMMARY.md
│   ├── Classes.md
│   ├── Events.md
│   ├── Extensibility.md
│   ├── FilteringSorting.md
│   ├── ModernPainterGuide.md
│   ├── PAINTER_SYSTEM_INTEGRATION_FIX.md
│   ├── PainterQuickReference.md
│   ├── PainterSystem.md
│   ├── PlanHeaderNavigationPainters.md
│   ├── README.md
│   ├── Styling.md
│   └── Usage.md
├── Filtering/
│   ├── BeepAdvancedFilterDialog.cs
│   ├── BeepDateRangePicker.cs
│   ├── BeepFilterRow.cs
│   ├── BeepQuickFilterBar.cs
│   └── README.md
├── Filters/
│   ├── BeepGridFilterFlyout.cs
│   ├── BeepGridFilterFlyout.designer.cs
│   ├── BeepGridFilterFlyout.resx
│   ├── BeepGridFilterPopup.cs
│   ├── BeepGridFilterPopup.designer.cs
│   └── BeepGridFilterPopup.resx
├── Helpers/
│   └── (32 helper files)
├── Layouts/
│   ├── docs/
│   ├── FooterLayouts/          (empty)
│   ├── HeaderLayouts/          (empty)
│   ├── NavigationLayouts/      (empty)
│   └── (54 layout files)
├── Painters/
│   ├── Claude.md
│   ├── README.md
│   ├── enums.cs
│   └── (46 painter files — header, navigation, filter panel)
├── BeepGridCurrentRow.cs
├── BeepGridPro.ClipboardOps.cs
├── BeepGridPro.ContextMenu.cs
├── BeepGridPro.cs
├── BeepGridPro.DataAccess.cs
├── BeepGridPro.Dialogs.cs
├── BeepGridPro.Events.cs
├── BeepGridPro.Filtering.cs
├── BeepGridPro.Input.cs
├── BeepGridPro.Invalidation.cs
├── BeepGridPro.Navigation.cs
├── BeepGridPro.Properties.cs
├── BeepGridSelectionMode.cs
├── BeepGridPro.resx (implicit)
├── AutoSizeTriggerMode.cs
├── HeaderIconVisibility.cs
├── Claude.md
├── ContextMenuUsageExample.md
├── PLAN_DataSource_Enhancement.md
├── PLAN_GridPro_Filter_DateEditor.md
└── README.md
```

## Target Structure (To-Be)

```
GridX/
├── Adapters/                         # Adapter layer for external integration
│   └── BeepGridProAdapter.cs
│
├── Models/                           # [FUTURE] Data model classes (currently in Controls.Models)
│   └── NOTE: Moving models here causes namespace shadowing. Keep in Controls.Models for now.
│
├── Editors/                          # [NEW] In-place editor components
│   ├── BeepGridDateDropDownEditor.cs # [NEW] Date dropdown editor for grid cells
│   ├── BeepGridComboBoxEditor.cs     # [NEW] Combo box editor with grid-specific behavior
│   ├── BeepGridNumericEditor.cs      # [NEW] Numeric input editor
│   ├── BeepGridMaskedEditor.cs       # [NEW] Masked text editor
│   ├── IGridEditor.cs                # [NEW] Editor interface contract
│   └── GridEditorFactory.cs          # [NEW] Factory for editor creation
│
├── Selection/                        # [NEW] Selection mode implementations
│   ├── CellSelectionStrategy.cs      # [NEW] Single cell selection
│   ├── RowSelectionStrategy.cs       # [NEW] Row-based selection
│   ├── MultiCellSelectionStrategy.cs # [NEW] Multi-cell range selection
│   ├── MultiRowSelectionStrategy.cs  # [NEW] Multi-row selection
│   ├── ColumnSelectionStrategy.cs    # [NEW] Column selection
│   └── ISelectionStrategy.cs         # [NEW] Selection strategy interface
│
├── Export/                           # [NEW] Export functionality
│   ├── GridExportEngine.cs           # [NEW] Core export orchestration
│   ├── GridExcelExporter.cs          # [NEW] Excel (xlsx) export
│   ├── GridCsvExporter.cs            # [NEW] CSV export
│   ├── GridPdfExporter.cs            # [NEW] PDF export
│   ├── GridHtmlExporter.cs           # [NEW] HTML export
│   ├── IGridExporter.cs              # [NEW] Exporter interface
│   └── ExportOptions.cs              # (see Models/)
│
├── Grouping/                         # [NEW] Row grouping functionality
│   ├── GridGroupEngine.cs            # [NEW] Core grouping logic
│   ├── GridGroupHeaderRenderer.cs    # [NEW] Group header painting
│   ├── GridGroupSummaryRow.cs        # [NEW] Summary row per group
│   ├── GroupDescriptor.cs            # (see Models/)
│   └── IGridGrouper.cs               # [NEW] Grouping interface
│
├── Virtualization/                   # [NEW] Large dataset virtualization
│   ├── GridVirtualDataSource.cs      # [NEW] Virtual data source wrapper
│   ├── GridRowVirtualizer.cs         # [NEW] Row virtualization manager
│   ├── GridColumnVirtualizer.cs      # [NEW] Column virtualization manager
│   └── IVirtualDataSource.cs         # [NEW] Virtual data source interface
│
├── Accessibility/                    # [NEW] Accessibility and keyboard nav
│   ├── GridAccessibilityProvider.cs  # [NEW] UIA provider for screen readers
│   ├── GridKeyboardNavigator.cs      # [NEW] Advanced keyboard navigation
│   └── GridFocusManager.cs           # [NEW] Focus tracking and management
│
├── Filtering/                        # Filter panel (migrating to unified toolbar)
│   ├── BeepAdvancedFilterDialog.cs
│   ├── BeepDateRangePicker.cs
│   ├── BeepFilterRow.cs              # [OBSOLETE] Replaced by unified toolbar
│   ├── BeepQuickFilterBar.cs         # [OBSOLETE] Replaced by unified toolbar
│   ├── FilterEditorHelper.cs         # [NEW] On-demand search editor activation
│   └── README.md
│
├── Filters/                          # Existing filter popups/flyouts
│   ├── BeepGridFilterFlyout.cs
│   ├── BeepGridFilterFlyout.designer.cs
│   ├── BeepGridFilterFlyout.resx
│   ├── BeepGridFilterPopup.cs
│   ├── BeepGridFilterPopup.designer.cs
│   └── BeepGridFilterPopup.resx
│
├── Helpers/                          # Existing helper classes (refactor target)
│   └── (32 existing files — see migration plan below)
│
├── Layouts/                          # Existing layout presets
│   ├── docs/
│   ├── FooterLayouts/                # [TODO] Populate with footer layout presets
│   ├── HeaderLayouts/                # [TODO] Populate with header layout presets
│   ├── NavigationLayouts/            # [TODO] Populate with navigation layout presets
│   └── (54 existing layout files)
│
├── Painters/                         # Existing painter system
│   ├── Claude.md
│   ├── README.md
│   ├── enums.cs
│   └── (46 existing painter files)
│
├── Testing/                          # [NEW] Unit and integration test stubs
│   ├── DataSourceTests.cs            # [NEW] DataSource binding tests
│   ├── FilterTests.cs                # [NEW] Filter pipeline tests
│   ├── SelectionTests.cs             # [NEW] Selection mode tests
│   ├── ExportTests.cs                # [NEW] Export functionality tests
│   └── VirtualizationTests.cs        # [NEW] Virtualization tests
│
├── Toolbar/                          # [NEW] Unified grid toolbar (Phase 18: Actions + Search + Filter)
│   ├── BeepGridToolbarState.cs       # [NEW] Toolbar layout state and hit testing
│   └── BeepGridToolbarPainter.cs     # [NEW] Toolbar rendering with SVG icons
│
├── Enhancements/                     # [NEW] Enhancement plans and phase docs
│   ├── PHASE_001_FilterVisibilityFix.md
│   ├── PHASE_002_DateEditorDirect.md
│   ├── PHASE_003_ObservableCollection.md
│   ├── PHASE_004_DataTableFastPath.md
│   ├── PHASE_005_SchemaChangeDetection.md
│   ├── PHASE_006_UowPostCommitSync.md
│   ├── PHASE_007_DeduplicatePositionEvents.md
│   ├── PHASE_008_DeadCodeCleanup.md
│   ├── PHASE_009_DocsAndTests.md
│   ├── PHASE_010_SelectionModes.md
│   ├── PHASE_011_ExportEngine.md
│   ├── PHASE_012_Grouping.md
│   ├── PHASE_013_Virtualization.md
│   ├── PHASE_014_ModernStyleWiring.md
│   ├── PHASE_015_LayoutPresetWiring.md
│   ├── PHASE_016_Accessibility.md
│   ├── PHASE_017_EditorFramework.md
│   ├── PHASE_018_UnifiedToolbar.md       # [NEW] Actions + Search + Filter in one bar
│   └── README.md
│
├── BeepGridCurrentRow.cs
├── BeepGridPro.ClipboardOps.cs
├── BeepGridPro.ContextMenu.cs
├── BeepGridPro.cs
├── BeepGridPro.DataAccess.cs
├── BeepGridPro.Dialogs.cs
├── BeepGridPro.Events.cs
├── BeepGridPro.Filtering.cs
├── BeepGridPro.Input.cs
├── BeepGridPro.Invalidation.cs
├── BeepGridPro.Navigation.cs
├── BeepGridPro.Properties.cs
├── BeepGridSelectionMode.cs
├── AutoSizeTriggerMode.cs
├── HeaderIconVisibility.cs
├── Claude.md
├── ContextMenuUsageExample.md
├── PLAN_DataSource_Enhancement.md    # Legacy — superseded by Enhancements/
├── PLAN_GridPro_Filter_DateEditor.md # Legacy — superseded by Enhancements/
├── TODO_TRACKER.md                   # Master task tracker
├── ENHANCEMENT_PLAN.md               # Master enhancement plan
├── FOLDER_PLAN.md                    # This file
└── README.md
```

## Helper Migration Plan

The existing `Helpers/` folder contains 32 files. These remain in place during Phases 1-9. During later phases, consider splitting:

| Current File | Potential New Location | Phase |
|---|---|---|
| `GridSelectionHelper.cs` | `Selection/` (refactor into strategies) | Phase 10 |
| `GridEditHelper.cs` | `Editors/` (refactor into factory + editors) | Phase 17 |
| `GridClipboardHelper.cs` | Stays in `Helpers/` | — |
| `GridDataHelper.cs` | Stays in `Helpers/` | — |
| `GridDataHelper.Rows.cs` | Stays in `Helpers/` | — |
| `GridDataHelper.ValueConversion.cs` | Stays in `Helpers/` | — |
| `GridRenderHelper.*.cs` | Stays in `Helpers/` | — |
| `GridInputHelper.cs` | Stays in `Helpers/` | — |
| `GridLayoutHelper.cs` | Stays in `Helpers/` | — |
| `GridScrollHelper.cs` | Stays in `Helpers/` | — |
| `GridScrollBarsHelper.cs` | Stays in `Helpers/` | — |
| `GridSortFilterHelper.cs` | Stays in `Helpers/` | — |
| `GridNavigatorHelper.cs` | Stays in `Helpers/` | — |
| `GridNavigationPainterHelper.cs` | Stays in `Helpers/` | — |
| `GridSizingHelper.cs` | Stays in `Helpers/` | — |
| `GridDialogHelper.*.cs` | Stays in `Helpers/` | — |
| `GridUnitOfWorkBinder.cs` | Stays in `Helpers/` | — |
| `GridThemeHelper.cs` | Stays in `Helpers/` | — |
| `GridColumnReorderHelper.cs` | Stays in `Helpers/` | — |
| `GridColumnHeadersPainterHelper.cs` | Stays in `Helpers/` | — |
| `GridFooterPainterHelper.cs` | Stays in `Helpers/` | — |
| `GridHeaderPainterHelper.cs` | Stays in `Helpers/` | — |
| `GridRenderHelper.*.cs` | Stays in `Helpers/` | — |
| `GridRenderHelper.Rendering.cs` | Stays in `Helpers/` | — |
| `GridRenderHelper.CellContent.cs` | Stays in `Helpers/` | — |
| `GridRenderHelper.Pagination.cs` | Stays in `Helpers/` | — |
| `GridRenderHelper.RowCalculations.cs` | Stays in `Helpers/` | — |
| `GridRenderHelper.Utilities.cs` | Stays in `Helpers/` | — |
| `BeepGridProFilterExtensions.cs` | Stays in `Helpers/` | — |
| `ExcelFilterHelper.cs` | Stays in `Helpers/` | — |
| `GridDataHelper.*.cs` | Stays in `Helpers/` | — |
| `MathCompat.cs` | Stays in `Helpers/` | — |

## New Folder Responsibilities

### Models/
Central location for all data model classes currently scattered across `Controls.Models` namespace. Reduces cross-project dependency and clarifies ownership.

### Editors/
Extracts editor logic from `GridEditHelper` into composable, testable editor components. Each editor implements `IGridEditor` and is created by `GridEditorFactory`.

### Selection/
Implements the Strategy pattern for selection modes. Replaces the current monolithic `GridSelectionHelper` with pluggable strategies that can be swapped at runtime.

### Export/
Adds export capabilities (Excel, CSV, PDF, HTML) to BeepGridPro. Each exporter implements `IGridExporter` and receives grid data through a read-only interface.

### Grouping/
Adds row grouping with collapsible group headers and summary rows. Integrates with existing filter and sort pipelines.

### Virtualization/
Enables handling of very large datasets (100K+ rows) through row and column virtualization. Only visible rows/columns are materialized.

### Accessibility/
Adds UI Automation support and advanced keyboard navigation for WCAG compliance.

### Testing/
Unit and integration test stubs for all enhancement phases.

### Enhancements/
Phase-by-phase enhancement documentation. Each phase file contains: objectives, files to modify, implementation steps, acceptance criteria, and rollback plan.
