# BeepTree Enhancement Todo Tracker

## Overview
This document tracks all planned enhancements for BeepTree across 6 phases. Each item includes status, priority, target file(s), and notes.

**Last Updated:** 2026-05-11 (in progress)
**Current Phase:** Phase 1 (Foundation & Stability)

---

## Legend
- [ ] Not started
- [~] In progress
- [x] Completed
- [!] Blocked/Issue

**Priority:** H (High) / M (Medium) / L (Low)

---

## Phase 1: Foundation & Stability

### 1.1 Performance & Benchmarking Infrastructure
- [ ] Create `BeepTree.Benchmarks` project with off-screen rendering tests | H | New Project
- [ ] Benchmark baseline: 100 nodes | H | Benchmarks/
- [ ] Benchmark baseline: 1000 nodes | H | Benchmarks/
- [ ] Benchmark baseline: 10000 nodes | H | Benchmarks/
- [ ] Measure paint time, layout calc time, memory allocations, GC pressure | H | Benchmarks/
- [ ] Add benchmark for painter switching and theme changes | M | Benchmarks/
- [ ] Integrate benchmark runner into CI | L | .github/workflows/

### 1.2 Layout System Hardening
- [ ] Unit tests for `BeepTreeLayoutHelper` coordinate transformation | H | Tests/
- [ ] Unit tests for `BeepTreeLayoutHelper` virtualization range | H | Tests/
- [ ] Unit tests for `BeepTreeLayoutHelper` text measurement caching | M | Tests/
- [ ] Unit tests for `BeepTreeLayoutHelper` indent calculation at DPI scales | M | Tests/
- [ ] Unit tests for `BeepTreeHitTestHelper` hit area registration | H | Tests/
- [ ] Unit tests for `BeepTreeHitTestHelper` viewport edge cases | M | Tests/
- [ ] Performance test: register 1000 nodes < 16ms | H | Tests/
- [ ] Validate struct mutation safety in `RecalculateLayoutCache()` | H | BeepTree.Layout.cs
- [ ] Add defensive null/empty checks in layout paths | M | BeepTree.Layout.cs, Helpers/

### 1.3 Scrollbar & Viewport Stability
- [x] Fix scrollbar flicker during rapid resize | H | BeepTree.Scrolling.cs
- [x] Prevent layout oscillation from scrollbar visibility toggles | H | BeepTree.Scrolling.cs
- [ ] Add smooth scroll option (pixel-based vs row-based) | M | BeepTree.Scrolling.cs
- [ ] Test horizontal scrollbar with deeply nested nodes (level 10+) | M | Tests/
- [ ] Validate mouse wheel delta at different DPI scales | M | BeepTree.Scrolling.cs

### 1.4 Memory & GDI Object Audit
- [x] Audit Material3TreePainter pen mutation | H | Painters/Material3TreePainter.cs
- [x] Audit Fluent2TreePainter pen mutation | H | Painters/Fluent2TreePainter.cs
- [x] Add safe drawing helpers (DrawChevron, DrawPlusMinus, DrawCheckmark) | H | Painters/BaseTreePainter.cs
- [x] Audit all painters for PaintersFactory pen/brush disposal | H | Painters/*.cs
- [ ] Audit Path object disposal in rounded-rectangle painters | H | Painters/*.cs
- [ ] Add Dispose pattern to painters if needed | M | Painters/BaseTreePainter.cs
- [ ] Profile memory for 10k node tree | M | Benchmarks/

### 1.5 Keyboard Navigation Completeness
- [x] Add Ctrl+Home / Ctrl+End support | M | BeepTree.Events.cs
- [x] Add Ctrl+Click multi-select | H | BeepTree.Events.cs
- [x] Add Shift+Click range selection | H | BeepTree.Events.cs
- [x] Add type-ahead search (press letter to jump) | M | BeepTree.Events.cs
- [ ] Ensure all keyboard events fire correct BeepMouseEventArgs | M | BeepTree.Events.cs

### 1.6 Event System Robustness
- [x] Audit NodeBeforeExpand/NodeBeforeCollapse cancellation | H | BeepTree.Events.cs
- [x] Audit NodeSelected firing exactly once per change | H | BeepTree.Properties.cs
- [x] Audit NodeChecked/NodeUnchecked for programmatic changes | M | BeepTree.Events.cs
- [x] Audit context menu events for correct node reference | M | BeepTree.Events.cs
- [ ] Add event sequence validation tests | L | Tests/

---

## Phase 2: Data Binding & Multi-Column Support

### 2.1 Column Architecture
- [x] Create `BeepTreeColumn` class | H | Models/BeepTreeColumn.cs
- [x] Create `BeepTreeColumnCollection` | H | Models/BeepTreeColumnCollection.cs
- [x] Add `Columns` property to BeepTree | H | BeepTree.Properties.cs
- [x] Update `NodeInfo` for cell rectangles per column | H | Models/NodeInfo.cs
- [x] Update painters for multi-column layouts | H | Painters/BaseTreePainter.cs

### 2.2 Data Binding Core
- [x] Add `DataSource` property | H | BeepTree.Properties.cs
- [x] Add `DataMember` property | H | BeepTree.Properties.cs
- [x] Support self-referencing data (KeyFieldName, ParentFieldName) | H | BeepTree.Methods.cs
- [x] Support hierarchical data (DisplayMember, ValueMember, ImageMember) | H | BeepTree.Methods.cs
- [ ] Add BindingContext integration | M | BeepTree.Core.cs
- [ ] CurrencyManager support | M | BeepTree.Core.cs

### 2.3 Cell Rendering
- [ ] Add `PaintColumnHeader` to ITreePainter | H | Painters/ITreePainter.cs
- [ ] Implement column header rendering in BaseTreePainter | H | Painters/BaseTreePainter.cs
- [ ] Add grid lines between columns | M | Painters/BaseTreePainter.cs
- [ ] Add `FullRowSelect` option | M | BeepTree.Properties.cs
- [ ] Add `ShowGridLines` property | M | BeepTree.Properties.cs
- [ ] Update all 22+ painters for multi-column | H | Painters/*.cs

### 2.4 Column Headers
- [x] Create `BeepTreeColumnHeader` renderer | H | Painters/BaseTreePainter.cs
- [x] Click to sort with indicator | H | BeepTree.Events.cs
- [x] Multi-column sort (Ctrl+Click) | M | BeepTree.Events.cs
- [ ] Drag to reorder columns | M | BeepTree.Events.cs
- [ ] Resize via header edge drag | M | BeepTree.Events.cs
- [ ] Context menu: show/hide columns | M | BeepTree.Events.cs
- [ ] Fixed/pinned columns | L | BeepTree.Scrolling.cs

### 2.5 Unbound Columns
- [ ] Calculated columns with Expression | M | Models/BeepTreeColumn.cs
- [ ] Custom calculation via CalculateCellValue event | M | BeepTree.Events.cs
- [ ] Static columns support | L | Models/BeepTreeColumn.cs

### 2.6 Load On Demand / Virtual Mode
- [ ] Add `LazyLoad` property | H | BeepTree.Properties.cs
- [ ] Add `NodesNeeded` event | H | BeepTree.Events.cs
- [ ] Full lazy mode (expander for all, load on expand) | H | BeepTree.Methods.cs
- [ ] Partial lazy mode (load first level in advance) | M | BeepTree.Methods.cs
- [ ] Async node loading with cancellation | M | BeepTree.Methods.cs
- [ ] Loading indicator (spinner) | M | BeepTree.Drawing.cs

### 2.7 Design-Time Support
- [ ] Column collection editor | L | Designers/
- [ ] Data source configuration wizard | L | Designers/
- [ ] Field picker dialog | L | Designers/
- [ ] Preview data in designer | L | Designers/

---

## Phase 3: Advanced Interaction

### 3.1 Drag & Drop Framework
- [ ] Add `AllowDragDrop` property | H | BeepTree.Properties.cs
- [ ] Create `BeepTreeDragDropManager` | H | Helpers/BeepTreeDragDropManager.cs
- [ ] Drag initiation with move threshold | H | BeepTree.Events.cs
- [ ] Drag image/cue (semi-transparent snapshot) | H | Helpers/BeepTreeDragDropManager.cs
- [ ] Drop target highlighting | H | Helpers/BeepTreeDragDropManager.cs
- [ ] Auto-scroll during drag | M | Helpers/BeepTreeDragDropManager.cs
- [ ] Auto-expand on hover during drag | M | Helpers/BeepTreeDragDropManager.cs
- [ ] ItemDrag, DragOver, DragDrop events | H | BeepTree.Events.cs
- [ ] QueryAllowedPosition event | M | BeepTree.Events.cs
- [ ] Move operation | H | BeepTree.Methods.cs
- [ ] Copy operation | M | BeepTree.Methods.cs
- [ ] Multiple node drag | M | BeepTree.Methods.cs
- [ ] Cross-control drag | M | BeepTree.Methods.cs
- [ ] External drag (accept files) | L | BeepTree.Events.cs

### 3.2 Inline Editing
- [ ] Add `AllowEdit` property | H | BeepTree.Properties.cs
- [ ] F2 / slow double-click to edit | H | BeepTree.Events.cs
- [ ] Create `BeepTreeCellEditor` | H | Editors/BeepTreeCellEditor.cs
- [ ] Text editor | H | Editors/BeepTreeCellEditor.cs
- [ ] ComboBox editor | H | Editors/BeepTreeCellEditor.cs
- [ ] CheckBox editor | M | Editors/BeepTreeCellEditor.cs
- [ ] DateTime picker editor | M | Editors/BeepTreeCellEditor.cs
- [ ] Numeric spinner editor | M | Editors/BeepTreeCellEditor.cs
- [ ] Custom editor support | L | Editors/BeepTreeCellEditor.cs
- [ ] Validating event | H | BeepTree.Events.cs
- [ ] ErrorProvider integration | M | BeepTree.Drawing.cs
- [ ] Edit masks / format strings | M | Editors/BeepTreeCellEditor.cs
- [ ] Inline edit mode | H | Editors/BeepTreeCellEditor.cs
- [ ] Edit form mode | L | Editors/BeepTreeCellEditor.cs
- [ ] New item row | L | BeepTree.Drawing.cs

### 3.3 Kinetic & Smooth Scrolling
- [ ] Add `EnableKineticScrolling` property | M | BeepTree.Properties.cs
- [ ] Mouse drag-to-scroll | M | BeepTree.Events.cs
- [ ] Momentum/inertia after release | M | BeepTree.Scrolling.cs
- [ ] Add `ScrollMode` enum (Discrete, Smooth, Deferred) | M | BeepTree.Scrolling.cs
- [ ] Per-pixel vertical scrolling | M | BeepTree.Scrolling.cs
- [ ] Per-pixel horizontal scrolling | M | BeepTree.Scrolling.cs

### 3.4 Multi-Select Enhancement
- [ ] Ctrl+Click toggle selection | H | BeepTree.Events.cs
- [ ] Shift+Click range selection | H | BeepTree.Events.cs
- [ ] Mouse drag selection (rubber band) | M | BeepTree.Events.cs
- [ ] `SelectionMode` enum | M | BeepTree.Properties.cs
- [ ] SelectedNodes collection with change notifications | M | BeepTree.Properties.cs

### 3.5 Node Checkboxes Enhancement
- [ ] Three-state checkboxes | M | BeepTree.Properties.cs
- [ ] Auto-update parent based on children | M | BeepTree.Methods.cs
- [ ] Cascade check to children | M | BeepTree.Methods.cs
- [ ] Radio buttons per node | L | BeepTree.Properties.cs
- [ ] CheckStateChanged event with cascade info | M | BeepTree.Events.cs

### 3.6 Context Menu Enhancement
- [ ] Built-in expand/collapse menu items | L | BeepTree.Events.cs
- [ ] Built-in sort menu items | L | BeepTree.Events.cs
- [ ] Built-in delete/rename/edit items | L | BeepTree.Events.cs
- [ ] Built-in copy/cut/paste items | L | BeepTree.Events.cs
- [ ] ContextMenuOpening event | M | BeepTree.Events.cs
- [ ] Per-node context menu support | L | BeepTree.Events.cs

---

## Phase 4: Data Shaping & Analysis

### 4.1 Sorting
- [ ] Add `SortColumn` and `SortOrder` properties | H | BeepTree.Properties.cs
- [ ] Single column sort via header click | H | BeepTree.Events.cs
- [ ] Multi-column sort (Ctrl+Click) | M | BeepTree.Events.cs
- [ ] Sort indicator in headers | H | Painters/BaseTreePainter.cs
- [ ] Custom sort via CompareNodes event | M | BeepTree.Events.cs
- [ ] Natural string sort | L | Helpers/BeepTreeHelper.cs
- [ ] Sort scope: Global, Level, Group | M | BeepTree.Methods.cs
- [ ] Preserve expand/collapse after sort | M | BeepTree.Methods.cs
- [ ] Async sort for large datasets | L | BeepTree.Methods.cs

### 4.2 Advanced Filtering
- [ ] Excel-style column filter popup | H | New Files
- [ ] Checklist of unique values in popup | H | New Files
- [ ] Search within values | M | New Files
- [ ] Auto Filter Row | H | BeepTree.Drawing.cs
- [ ] Filter operators per cell | H | New Files
- [ ] Filter Criteria Builder (visual) | M | New Files
- [ ] Save/load filter criteria | M | New Files
- [ ] MRU filters list | L | New Files
- [ ] Filter Panel (chips/tags) | M | BeepTree.Drawing.cs
- [ ] Text filters (contains, starts with, etc.) | H | New Files
- [ ] Numeric filters | M | New Files
- [ ] Date filters | M | New Files
- [ ] Custom filter (predicate) | L | BeepTree.Methods.cs

### 4.3 Search & Find
- [ ] Incremental search | M | BeepTree.Events.cs
- [ ] Highlight matching text | M | Painters/BaseTreePainter.cs
- [ ] Find Panel (Ctrl+F) | H | New Files
- [ ] Find next/previous | H | New Files
- [ ] Match case option | M | New Files
- [ ] Match whole word option | M | New Files
- [ ] "X of Y matches" indicator | M | New Files
- [ ] Programmatic FindNode enhancements | M | BeepTree.Methods.cs
- [ ] FindAll method | M | BeepTree.Methods.cs

### 4.4 Summaries & Aggregates
- [ ] Footer row | H | BeepTree.Drawing.cs
- [ ] Summary types: Sum, Count, Average, Min, Max | H | New Files
- [ ] Custom summary via event | M | New Files
- [ ] Group summaries per parent | M | BeepTree.Drawing.cs
- [ ] Summary formatting | M | New Files

### 4.5 Conditional Formatting
- [ ] FormatCondition class | M | Models/FormatCondition.cs
- [ ] Predefined rules (GreaterThan, Between, etc.) | M | Models/FormatCondition.cs
- [ ] Custom rule (predicate) | L | Models/FormatCondition.cs
- [ ] Background/foreground color formatting | M | Painters/BaseTreePainter.cs
- [ ] Font formatting | M | Painters/BaseTreePainter.cs
- [ ] Icon sets | L | Painters/BaseTreePainter.cs
- [ ] Data bars | L | Painters/BaseTreePainter.cs
- [ ] Color scales | L | Painters/BaseTreePainter.cs
- [ ] Rule priority and stop-if-true | L | Models/FormatCondition.cs

---

## Phase 5: Enterprise Features

### 5.1 Data Export
- [ ] Export to Excel (.xlsx) | H | Export/BeepTreeExporter.cs
- [ ] Export to CSV | H | Export/BeepTreeExporter.cs
- [ ] Export to PDF | M | Export/BeepTreeExporter.cs
- [ ] Export to HTML | M | Export/BeepTreeExporter.cs
- [ ] Export to RTF | L | Export/BeepTreeExporter.cs
- [ ] Export to XML | L | Export/BeepTreeExporter.cs
- [ ] Export to JSON | L | Export/BeepTreeExporter.cs
- [ ] ExportAll/ExportSelected/ExportVisible options | H | Export/BeepTreeExporter.cs
- [ ] Include headers option | M | Export/BeepTreeExporter.cs
- [ ] Apply formatting option | M | Export/BeepTreeExporter.cs
- [ ] ExportNode event | M | Export/BeepTreeExporter.cs
- [ ] ExportCell event | M | Export/BeepTreeExporter.cs
- [ ] ExportProgress event | M | Export/BeepTreeExporter.cs
- [ ] Async export with progress | L | Export/BeepTreeExporter.cs

### 5.2 Printing
- [ ] Print() method with PrintDialog | M | Print/BeepTreePrinter.cs
- [ ] PrintPreview() with preview dialog | M | Print/BeepTreePrinter.cs
- [ ] Page Setup (margins, orientation) | M | Print/BeepTreePrinter.cs
- [ ] Print all pages or page range | L | Print/BeepTreePrinter.cs
- [ ] Fit to page / scale to width | L | Print/BeepTreePrinter.cs
- [ ] Repeat column headers on each page | M | Print/BeepTreePrinter.cs
- [ ] Print selection only | L | Print/BeepTreePrinter.cs
- [ ] Page numbers, date, title in header/footer | M | Print/BeepTreePrinter.cs
- [ ] PrintPage event | L | Print/BeepTreePrinter.cs

### 5.3 Breadcrumb Navigation
- [ ] Add `ShowBreadcrumb` property | M | BeepTree.Properties.cs
- [ ] Create `BeepTreeBreadcrumb` control | M | New File
- [ ] Horizontal path display | M | BeepTreeBreadcrumb.cs
- [ ] Clickable segments | M | BeepTreeBreadcrumb.cs
- [ ] Dropdown for siblings | L | BeepTreeBreadcrumb.cs
- [ ] Update on selection change | M | BeepTree.Events.cs
- [ ] Match tree painter theme | M | BeepTreeBreadcrumb.cs
- [ ] Compact mode | L | BeepTreeBreadcrumb.cs

### 5.4 Layout Persistence
- [ ] SaveLayoutToFile / LoadLayoutFromFile | H | BeepTree.Methods.cs
- [ ] SaveLayoutToStream / LoadLayoutFromStream | H | BeepTree.Methods.cs
- [ ] SaveLayoutToString / LoadLayoutFromString | H | BeepTree.Methods.cs
- [ ] Persist column order, widths, visibility | H | BeepTree.Methods.cs
- [ ] Persist sort state | M | BeepTree.Methods.cs
- [ ] Persist filter state | M | BeepTree.Methods.cs
- [ ] Persist expanded/collapsed state | M | BeepTree.Methods.cs
- [ ] Persist selection state | L | BeepTree.Methods.cs
- [ ] Persist scroll position | L | BeepTree.Methods.cs
- [ ] LayoutSaving/LayoutSaved events | M | BeepTree.Events.cs
- [ ] LayoutLoading/LayoutLoaded events | M | BeepTree.Events.cs
- [ ] Auto-save on form close | L | BeepTree.Core.cs
- [ ] Multiple named layouts | L | BeepTree.Methods.cs

### 5.5 Clipboard Operations
- [ ] Copy selected nodes | M | BeepTree.Methods.cs
- [ ] Copy formats: text, CSV, HTML | M | BeepTree.Methods.cs
- [ ] Paste as new nodes | M | BeepTree.Methods.cs
- [ ] Cut selected nodes | L | BeepTree.Methods.cs
- [ ] Visual indication for cut nodes | L | Painters/BaseTreePainter.cs

### 5.6 Accessibility
- [ ] Implement AccessibleObject for tree | M | BeepTree.Core.cs
- [ ] Implement AccessibleObject for nodes | M | BeepTree.Core.cs
- [ ] Expose hierarchy to screen readers | M | BeepTree.Core.cs
- [ ] Expose selection state | M | BeepTree.Core.cs
- [ ] Screen reader announcements | L | BeepTree.Events.cs
- [ ] Full keyboard operation | M | BeepTree.Events.cs
- [ ] High contrast theme support | L | Painters/*.cs

---

## Phase 6: Polish, Performance & Animation

### 6.1 Animations & Transitions
- [ ] Expand/collapse animation | M | Animation/BeepTreeAnimator.cs
- [ ] Selection background fade | L | Animation/BeepTreeAnimator.cs
- [ ] Hover background fade | L | Animation/BeepTreeAnimator.cs
- [ ] Smooth scroll to node | M | Animation/BeepTreeAnimator.cs
- [ ] Node add/remove fade animation | L | Animation/BeepTreeAnimator.cs
- [ ] BeepAnimation helper class | M | Animation/BeepAnimation.cs
- [ ] BeepTreeAnimator manager | M | Animation/BeepTreeAnimator.cs
- [ ] Respect OS animation settings | L | Animation/BeepTreeAnimator.cs

### 6.2 Variable Row Heights
- [ ] Add `AllowVariableRowHeight` property | M | BeepTree.Properties.cs
- [ ] SimpleItem.RowHeight property | M | Models/SimpleItem.cs
- [ ] Auto-calculate row height from content | M | BeepTreeLayoutHelper.cs
- [ ] Preview sections (PreviewText, PreviewHeight) | L | Models/SimpleItem.cs
- [ ] Cache row heights | M | BeepTreeLayoutHelper.cs
- [ ] Virtualization with variable heights | H | BeepTreeLayoutHelper.cs

### 6.3 Scrollbar Enhancements
- [ ] Scrollbar annotations (search marks) | L | BeepTree.Scrolling.cs
- [ ] Error/warning indicators on scrollbar | L | BeepTree.Scrolling.cs
- [ ] Custom scrollbar styling per theme | M | BeepScrollBar.cs
- [ ] Auto-hide scrollbars | L | BeepTree.Scrolling.cs
- [ ] Overlay scrollbars | L | BeepTree.Scrolling.cs

### 6.4 Empty State & Loading State
- [ ] Empty state icon/image support | L | BeepTree.Drawing.cs
- [ ] Empty state action button | L | BeepTree.Drawing.cs
- [ ] Custom empty state renderer | L | BeepTree.Drawing.cs
- [ ] `IsLoading` property | M | BeepTree.Properties.cs
- [ ] Loading spinner | M | BeepTree.Drawing.cs
- [ ] Skeleton loading (shimmer) | L | BeepTree.Drawing.cs
- [ ] `LoadingText` property | M | BeepTree.Properties.cs
- [ ] Error state with icon and retry | L | BeepTree.Drawing.cs

### 6.5 Custom Draw API
- [ ] CustomDrawNode event | M | BeepTree.Events.cs
- [ ] CustomDrawNodeBackground event | M | BeepTree.Events.cs
- [ ] CustomDrawCell event | M | BeepTree.Events.cs
- [ ] CustomDrawToggle event | L | BeepTree.Events.cs
- [ ] CustomDrawColumnHeader event | L | BeepTree.Events.cs
- [ ] CustomDrawFooter event | L | BeepTree.Events.cs
- [ ] Handled property in event args | M | EventArgs classes
- [ ] DefaultDraw() method | M | EventArgs classes

### 6.6 HTML/CSS Template Support
- [ ] SimpleItem.HtmlTemplate property | L | Models/SimpleItem.cs
- [ ] HTML parser (subset: b, i, color, img, br) | L | Rendering/HtmlRenderer.cs
- [ ] CSS-like inline styles | L | Rendering/HtmlRenderer.cs
- [ ] Data binding placeholders {FieldName} | L | Rendering/HtmlRenderer.cs
- [ ] Template text measurement | L | Rendering/HtmlRenderer.cs

### 6.7 Final Performance Optimization
- [ ] Object pooling for NodeInfo structs | M | Helpers/BeepTreeLayoutHelper.cs
- [ ] Object pooling for Rectangle/Point | L | Helpers/BeepTreeLayoutHelper.cs
- [ ] Reuse GraphicsPath objects | M | Painters/BaseTreePainter.cs
- [ ] Dirty region tracking | M | BeepTree.Drawing.cs
- [ ] Avoid full Invalidate() on small changes | M | BeepTree.Events.cs
- [ ] Lazy load images | M | Helpers/BeepTreeLayoutHelper.cs
- [ ] Image cache with LRU eviction | M | Helpers/ImageCache.cs
- [ ] Dispose unused painter resources | M | Painters/BaseTreePainter.cs
- [ ] Target: 100k nodes load < 1s | H | Benchmarks/
- [ ] Target: 100k nodes scroll at 60 FPS | H | Benchmarks/
- [ ] Target: 1M nodes in virtual mode | L | Benchmarks/

### 6.8 Theme & Skin System Enhancement
- [ ] Per-node BackColor/ForeColor/Font | L | Models/SimpleItem.cs
- [ ] Per-node Style reference | L | Models/SimpleItem.cs
- [ ] Skin loading from XML/JSON | L | Theming/
- [ ] Skin editor tool | L | Tools/
- [ ] Vector-based skins | L | Theming/
- [ ] Dark mode auto-detection | M | BeepTree.Core.cs
- [ ] Automatic color inversion | L | Painters/BaseTreePainter.cs

---

## Cross-Cutting Concerns

### Documentation
- [ ] Update XML documentation for all public APIs | H | All .cs files
- [ ] Create usage examples | M | Examples/
- [ ] Create migration guide from standard TreeView | L | Docs/

### Testing
- [ ] Unit test coverage > 80% | H | Tests/
- [ ] Integration tests for data binding | H | Tests/
- [ ] Performance regression tests | M | Benchmarks/
- [ ] Visual comparison tests for painters | L | Tests/

### Designer Support
- [ ] Toolbox icon for BeepTree | M | Resources/
- [ ] Smart tag panel | L | Designers/
- [ ] Property grid categorization | M | BeepTree.Properties.cs

---

## Completed Work (Pre-Tracking)

### Phase 0: Initial Architecture (Completed before 2026-05-11)
- [x] Create folder structure (Helpers/, Painters/, Models/)
- [x] Create `NodeInfo.cs` struct for layout caching
- [x] Create `ITreePainter.cs` interface
- [x] Create `BaseTreePainter.cs` abstract class
- [x] Create `BeepTreePainterFactory.cs` factory
- [x] Create `BeepTreeHelper.cs` - Data operations
- [x] Create `BeepTreeLayoutHelper.cs` - Layout calculation
- [x] Create `BeepTreeHitTestHelper.cs` - Hit testing
- [x] Split BeepTree into partial classes (Core, Properties, Events, Methods, Drawing, Layout, Scrolling)
- [x] Fix struct mutation bug in RecalculateLayoutCache()
- [x] Implement 22+ tree painters

### Phase 0.5: Critical Fixes (Completed before 2026-05-11)
- [x] Fix GDI corruption (shared pen mutation)
- [x] Fix GDI disposal of cached objects
- [x] Fix UpdateScrollBars viewport validation
- [x] Remove RegisterHitAreas from DrawContent
- [x] Fix duplicate NodeSelected event
- [x] Add keyboard navigation (arrows, home/end, page up/down)
- [x] Add DPI scaling helpers
- [x] Layout merge & deduplication (SyncFromVisibleNodes)
- [x] Empty state rendering
- [x] Focus ring drawing
- [x] Cancellable expand/collapse events
- [x] Search/filter functionality

---

## Notes

### Commercial Feature Comparison
| Feature | DevExpress | Syncfusion | Telerik | BeepTree Current | BeepTree Planned |
|---|---|---|---|---|---|
| Multi-column | Yes | Yes | Yes | No | Phase 2 |
| Data binding | Yes | Yes | Yes | No | Phase 2 |
| Drag & drop | Yes | Yes | Yes | No | Phase 3 |
| Inline editing | Yes | Yes | Yes | No | Phase 3 |
| Sorting | Yes | Yes | Yes | No | Phase 4 |
| Filtering | Yes | Yes | Yes | Basic | Phase 4 |
| Summaries | Yes | No | No | No | Phase 4 |
| Export | Yes | Yes | No | No | Phase 5 |
| Print | Yes | Yes | No | No | Phase 5 |
| Breadcrumb | Yes | No | No | No | Phase 5 |
| Animation | Limited | No | No | No | Phase 6 |
| Custom draw | Yes | Yes | Yes | No | Phase 6 |

### Open Source References
- **FastTree**: Virtual mode, multi-selection, drag-drop, node customizing
- **ObjectListView**: Tree+ListView columns, CanExpandGetter/ChildrenGetter delegates
- **TreeViewAdv**: Model/view architecture, multicolumns, multiselection, incremental search
- **cmdwtf/Treemap**: Drop-in TreeView replacement, API compatibility

### Performance Targets
- 100 nodes: < 16ms paint, < 8ms layout
- 1,000 nodes: < 16ms paint, < 16ms layout
- 10,000 nodes: < 16ms paint (virtualized), < 50ms layout
- 100,000 nodes: virtual mode, load < 1s, scroll 60 FPS
