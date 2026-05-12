# Phase 3: Advanced Interaction

## Goal
Implement enterprise-grade interaction features: drag & drop, inline editing, kinetic scrolling, and touch support to match commercial controls.

## Current State Analysis
BeepTree has basic mouse and keyboard interaction:
- Single/multi-select with click
- Expand/collapse with click on toggle
- Checkbox toggle
- Context menu on right-click
- Keyboard navigation (arrows, home/end, page up/down)
- Hover effects

Missing advanced interactions found in commercial products.

## Research Insights

### DevExpress TreeList
- Node drag & drop within tree and between controls
- Accept dropped items from outside (e.g., files from Explorer)
- Copy / Move / Accept Outer Nodes
- Drag Multiple nodes simultaneously
- Comprehensive drag-and-drop API

### Telerik RadTreeView
- Kinetic scrolling for touch
- Lazy mode with NodesNeeded event
- Drag drop with position indication (above/below/on node)
- Auto-scroll and auto-expand during drag
- ShowDragNodeCue semi-transparent image

### Syncfusion TreeViewAdv
- Full drag-drop with TreeViewAdvDragHighlightTracker
- Drop position highlighting (above/below/on)
- QueryAllowedPositionForNode event for validation
- KeepDottedSelection, KeepDragCapturePoint

### FastTree (GitHub)
- Drag & drop with node customizing
- Permitting events (CanUnselectNodeNeeded)
- Wide set of drag&drop events

## Tasks

### 3.1 Drag & Drop Framework
- [ ] Add `AllowDragDrop` property
- [ ] Create `BeepTreeDragDropManager`:
  - [ ] Drag initiation (mouse down + move threshold)
  - [ ] Drag image/cue (semi-transparent node snapshot)
  - [ ] Drop target highlighting (line above/below, box around node)
  - [ ] Auto-scroll during drag near edges
  - [ ] Auto-expand collapsed nodes on hover during drag
- [ ] Events:
  - [ ] `ItemDrag` - drag started
  - [ ] `DragOver` - dragging over potential target
  - [ ] `DragDrop` - dropped
  - [ ] `QueryContinueDrag` - should drag continue?
  - [ ] `DragEnter` / `DragLeave` - enter/leave control bounds
  - [ ] `QueryAllowedPosition` - validate if drop allowed (above/below/on)
- [ ] Support:
  - [ ] Move (remove from source, add to target)
  - [ ] Copy (clone node, add to target)
  - [ ] Multiple node drag (when multi-selected)
  - [ ] Cross-control drag (between two BeepTrees)
  - [ ] External drag (accept files from Windows Explorer)

### 3.2 Inline Editing
- [ ] Add `AllowEdit` property
- [ ] F2 or slow double-click to enter edit mode
- [ ] Create `BeepTreeCellEditor` (integrates with Beep editors):
  - [ ] Text editor (default)
  - [ ] ComboBox editor
  - [ ] CheckBox editor (boolean columns)
  - [ ] DateTime picker
  - [ ] Numeric spinner
  - [ ] Custom editor support
- [ ] Edit validation:
  - [ ] `Validating` event (cancel invalid input)
  - [ ] `ErrorProvider` integration for error indication
  - [ ] Edit masks / format strings
- [ ] Edit modes:
  - [ ] Inline (edit in cell)
  - [ ] Edit form (popup dialog for row)
  - [ ] New item row (blank row at bottom for adding)

### 3.3 Kinetic & Smooth Scrolling
- [ ] Add `EnableKineticScrolling` property
- [ ] Implement touch/mouse drag-to-scroll:
  - [ ] Capture mouse on press, scroll on drag
  - [ ] Momentum/inertia after release
  - [ ] Bounce at edges (optional)
- [ ] Add `ScrollMode` enum:
  - [ ] `Discrete` - one row at a time
  - [ ] `Smooth` - pixel-based scrolling
  - [ ] `Deferred` - no GUI updates until scroll ends + tooltip position
- [ ] Per-pixel vertical scrolling (partial row visibility)
- [ ] Per-pixel horizontal scrolling

### 3.4 Multi-Select Enhancement
- [ ] Ctrl+Click to toggle individual node selection
- [ ] Shift+Click to select range
- [ ] Mouse drag selection (rubber band / marquee)
- [ ] `SelectionMode` enum:
  - [ ] `Single`
  - [ ] `Multiple` (Ctrl/Shift)
  - [ ] `SameLevelOnly`
  - [ ] `Extended` (Windows standard)
- [ ] `SelectedNodes` collection with change notifications

### 3.5 Node Checkboxes Enhancement
- [ ] Three-state checkboxes (unchecked, checked, indeterminate)
  - [ ] Auto-update parent state based on children
  - [ ] Cascade check to children when parent checked
- [ ] Radio buttons per node (mutually exclusive within parent)
- [ ] `CheckStateChanged` event with cascade info

### 3.6 Context Menu Enhancement
- [ ] Built-in context menu items:
  - [ ] Expand/Collapse
  - [ ] Expand All / Collapse All
  - [ ] Sort (if columns enabled)
  - [ ] Delete node
  - [ ] Rename / Edit
  - [ ] Copy / Cut / Paste
- [ ] `ContextMenuOpening` event (cancel or customize)
- [ ] Per-node context menu support

## Success Criteria
- Drag & drop nodes between two BeepTree instances
- Edit node text inline with validation
- Kinetic scroll through 10k nodes smoothly
- Ctrl+Shift multi-select works intuitively
- Three-state checkboxes with cascade

## Dependencies
- Phase 1: Foundation & Stability
- Phase 2: Data Binding & Multi-Column (for cell editing)

## Estimated Effort
5-7 days

## Research References
- DevExpress TreeList: Drag & drop API, cell editors
- Telerik RadTreeView: Kinetic scrolling, drag cues, NodesNeeded
- Syncfusion TreeViewAdv: DragHighlightTracker, drop positions
- FastTree (GitHub): Permitting events, drag & drop
