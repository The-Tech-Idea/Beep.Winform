# Charts TODO Tracker

## Phase 0 - Planning And Benchmark Alignment

- [x] Create planning index and overview gap matrix
- [x] Create per-phase plan documents for Phase 1 to Phase 4
- [x] Create external benchmark and UX standards note
- [x] Create master todo tracker for Charts
- [x] Capture external benchmark inputs from Telerik, Syncfusion, and broader commercial chart expectations

## Phase 1 - Foundation, Rendering, And Contracts

- [ ] Define painter and layout ownership rules
- [ ] Document chart-type capability matrix and extension rules
- [ ] Normalize viewport, axis, and legend contract guidance
- [ ] Define theme, palette, and surface token defaults
- [ ] Finalize phase 1 foundation review

## Phase 2 - Interactions, Navigation, And Analysis

- [ ] Define zoom, pan, and reset interaction rules
- [x] Define tooltip, hover, and crosshair or trackball roadmap behavior (trackball mode implemented)
- [ ] Document legend interaction and visibility policies
- [ ] Add selection, drill-down, and context action guidance
- [ ] Finalize phase 2 interaction review

## Phase 2 - Implementation Complete ✅

- [x] Add viewport-changed typed event with old/new bounds and source
- [x] Wire viewport change events to wheel, drag, keyboard, and reset paths
- [x] Implement trackball mode with vertical crosshair
- [x] Implement multi-series tooltip in trackball mode
- [x] Add configurable trackball colors and styling
- [x] Add TrackballDataCollected event for host integration
- [x] Add legend isolate (double-click legend to show only one series)
- [x] Add selection state (point/series selection on click)
- [x] Add context menu for copy/export (CSV, JSON, clear selection)
- [x] Finalize phase 2 implementation review

## Phase 3 - Visual Variants, Accessibility, And Performance

- [x] Implement visual preset system for dashboard, analytical, high-contrast, print, and presentation modes
- [x] Add keyboard accessibility: Tab (cycle series), arrows (navigate points), Enter (select), Ctrl+A (select all), focus indicator
- [x] Optimize rendering performance for large datasets (1000+ points): point culling, vertex simplification, cache management
- [x] Add non-color state communication (patterns/hatching for series distinction)
- [x] Add readability rules and dense-data label optimization
- [x] Add real-time data streaming support with incremental rendering
- [x] Finalize phase 3 visual/accessibility/performance review

## Phase 4 - Integration, Documentation, And Productization

- [x] Document host integration and design-time usage patterns
- [x] Define export, print, and persistence roadmap notes
- [x] Map sample scenarios for core chart types and interactions
- [x] Align plans, tracker, and README before rollout
- [x] Finalize phase 4 productization review

## Immediate Next Wave

- [x] Create a Phase 1 implementation checklist for Charts
- [x] Create a Phase 1 file-by-file implementation matrix
- [x] Create a Phase 1 review notes document
- [x] Create a Phase 1 task breakdown
- [ ] Review the chart plan docs with the implementation owner
- [x] Decide whether the first execution stream starts with painter contracts or interaction upgrades (recommended: painter contracts first)
- [x] Add a root Charts README that links to this plan set