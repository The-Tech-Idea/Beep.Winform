# Phase 2: Data Binding & Multi-Column Support

## Goal
Transform BeepTree from a single-column tree into a data-aware TreeView-ListView hybrid that can display information as a TREE, a GRID, or a combination of both.

## Current State Analysis
BeepTree currently uses `SimpleItem` nodes with a single `Text` property. There is no column concept, no data binding, and no way to display tabular data within the tree structure.

## Research Insights

### DevExpress TreeList
- Displays data as TREE, GRID, or combination of both
- Bound mode: DataSource + DataMember, KeyFieldName + ParentFieldName for self-referencing hierarchies
- Unbound mode: manual node population with on-demand loading
- Column collection with move, show, hide, resize, pinned columns
- Cell editors within tree cells

### Telerik RadTreeView
- Binds to IList, IListSource, IBindingList
- Self-referencing data: ParentMember + ChildMember + DisplayMember
- XML data binding support
- Load-on-demand via `NodesNeeded` event

### Syncfusion TreeViewAdv
- Data binding with hierarchical data
- Unbound and bound modes
- Virtualization for large bound datasets

## Tasks

### 2.1 Column Architecture
- [ ] Create `BeepTreeColumn` class:
  - [ ] `Name`, `Caption`, `FieldName`, `Width`, `MinWidth`, `MaxWidth`
  - [ ] `Visible`, `ReadOnly`, `Sortable`, `Filterable`
  - [ ] `DataType`, `FormatString`, `Alignment`
  - [ ] `EditorType` (TextBox, ComboBox, DatePicker, etc.)
- [ ] Create `BeepTreeColumnCollection` with design-time support
- [ ] Add `Columns` property to BeepTree
- [ ] Update `NodeInfo` to include cell rectangles per column
- [ ] Update painters to render multi-column layouts

### 2.2 Data Binding Core
- [ ] Add `DataSource` property (accepts DataTable, BindingList, IList, etc.)
- [ ] Add `DataMember` property for complex data sources
- [ ] Support self-referencing data:
  - [ ] `KeyFieldName` (unique ID field)
  - [ ] `ParentFieldName` (parent ID field)
  - [ ] Auto-build hierarchy from flat data
- [ ] Support hierarchical data:
  - [ ] `DisplayMember` (text field)
  - [ ] `ValueMember` (value field)
  - [ ] `ImageMember` (image field)
- [ ] Add `BindingContext` integration
- [ ] CurrencyManager support for current row tracking

### 2.3 Cell Rendering
- [ ] Update `BaseTreePainter` to paint cells in columns:
  - [ ] Column header row (new painter method `PaintColumnHeader`)
  - [ ] Grid lines between columns (optional)
  - [ ] Cell text alignment per column
  - [ ] Cell background/foreground per column
- [ ] Update all 22+ painters for multi-column support
- [ ] Add `FullRowSelect` option (select entire row vs single cell)
- [ ] Add `ShowGridLines` property

### 2.4 Column Headers
- [ ] Create `BeepTreeColumnHeader` renderer
- [ ] Click to sort (if `Sortable`)
  - [ ] Ascending / descending indicator
  - [ ] Multi-column sort (Ctrl+Click)
- [ ] Drag to reorder columns
- [ ] Resize via drag on header edge
- [ ] Context menu: show/hide columns
- [ ] Fixed/pinned columns (stay visible during horizontal scroll)

### 2.5 Unbound Columns
- [ ] Support calculated columns (Expression property)
  - [ ] Simple arithmetic: `{Price} * {Quantity}`
  - [ ] Custom calculation via event: `CalculateCellValue`
- [ ] Support static columns (manually set values)

### 2.6 Load On Demand / Virtual Mode
- [ ] Add `LazyLoad` property
- [ ] Add `NodesNeeded` event (Telerik-style):
  - [ ] Event args: Parent node, Nodes collection to fill
  - [ ] Full lazy mode: expander shown for all, load on expand
  - [ ] Partial lazy mode: load first level in advance
- [ ] Async node loading with cancellation support
- [ ] Loading indicator (spinner) for pending nodes

### 2.7 Design-Time Support
- [ ] Column collection editor
- [ ] Data source configuration wizard
- [ ] Field picker for KeyFieldName/ParentFieldName/DisplayMember
- [ ] Preview data in designer

## Success Criteria
- Bind to DataTable with self-referencing hierarchy and display correctly
- Bind to List<BusinessObject> with parent-child relationships
- Display 5+ columns with sorting and resizing
- Lazy load 100k nodes without UI freeze
- All existing single-column functionality preserved

## Dependencies
- Phase 1: Foundation & Stability

## Estimated Effort
5-7 days

## Research References
- DevExpress TreeList: Bound/Unbound modes, KeyFieldName/ParentFieldName
- Telerik RadTreeView: NodesNeeded event, self-referencing binding
- Syncfusion TreeViewAdv: Data binding, virtualization
- ObjectListView (GitHub): Column architecture for tree+list hybrid
