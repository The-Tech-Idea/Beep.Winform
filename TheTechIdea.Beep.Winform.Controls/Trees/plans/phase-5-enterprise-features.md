# Phase 5: Enterprise Features

## Goal
Add enterprise-grade features: data export/import, printing, breadcrumb navigation, and layout persistence to meet business application requirements.

## Current State Analysis
BeepTree has no export, print, breadcrumb, or layout save/restore capabilities. These are standard requirements in commercial controls.

## Research Insights

### DevExpress TreeList
- Export to XLS, XLSX, PDF, HTML, TXT, CSV, RTF
- WYSIWYG Printing
- Print Setup dialog
- Breadcrumb Navigation
- Save/Restore Layouts (column order, widths, visibility, sort, filter)
- Conditional Formatting Rule Editor

### Telerik RadTreeView
- XML serialization (save/load tree structure)
- Rich design-time support

### Syncfusion TreeViewAdv
- Print support
- Export capabilities

## Tasks

### 5.1 Data Export
- [ ] Export formats:
  - [ ] Excel (.xls, .xlsx) - use EPPlus or OpenXML
  - [ ] CSV (with delimiter and encoding options)
  - [ ] PDF (use existing Beep PDF library or iTextSharp)
  - [ ] HTML (styled table with hierarchy indentation)
  - [ ] RTF (rich text format)
  - [ ] XML (hierarchical structure)
  - [ ] JSON (hierarchical structure)
- [ ] Export options:
  - [ ] `ExportAll` vs `ExportSelected` vs `ExportVisible`
  - [ ] Include headers
  - [ ] Apply formatting (colors, fonts)
  - [ ] Apply conditional formatting rules
  - [ ] Expand/collapse state handling
- [ ] Export events:
  - [ ] `ExportNode` - customize node export (skip, modify)
  - [ ] `ExportCell` - customize cell value/format
  - [ ] `ExportProgress` - progress reporting for large exports
- [ ] Async export with progress dialog

### 5.2 Printing
- [ ] Print support:
  - [ ] `Print()` method with PrintDialog
  - [ ] `PrintPreview()` with preview dialog
  - [ ] Page Setup (margins, orientation, paper size)
- [ ] Print options:
  - [ ] Print all pages or page range
  - [ ] Fit to page / scale to width
  - [ ] Repeat column headers on each page
  - [ ] Print expand/collapse indicators
  - [ ] Print selection only
- [ ] Print styling:
  - [ ] Use current painter theme
  - [ ] Optimized for black & white
  - [ ] Page numbers, date, title in header/footer
- [ ] `PrintPage` event for custom page content

### 5.3 Breadcrumb Navigation
- [ ] Add `ShowBreadcrumb` property
- [ ] Create `BeepTreeBreadcrumb` control:
  - [ ] Horizontal path: Root > Parent > Parent > SelectedNode
  - [ ] Each segment is clickable button/dropdown
  - [ ] Dropdown shows siblings at each level
  - [ ] Home icon for root
  - [ ] Separator (/, >, or chevron)
- [ ] Integration:
  - [ ] Update breadcrumb on selection change
  - [ ] Click segment to navigate to that node
  - [ ] Dropdown to select sibling
  - [ ] Optional: show full path in tooltip
- [ ] Styling:
  - [ ] Match current tree painter theme
  - [ ] Compact mode (show only last 3 segments + root)
  - [ ] Editable breadcrumb (type path to navigate)

### 5.4 Layout Persistence
- [ ] Save/Restore layout:
  - [ ] `SaveLayoutToFile(string path)` / `LoadLayoutFromFile(string path)`
  - [ ] `SaveLayoutToStream(Stream)` / `LoadLayoutFromStream(Stream)`
  - [ ] `SaveLayoutToString()` / `LoadLayoutFromString(string)`
- [ ] Persisted settings:
  - [ ] Column order, widths, visibility
  - [ ] Sort state (columns, directions)
  - [ ] Filter state (criteria)
  - [ ] Group state (expanded/collapsed nodes - optional)
  - [ ] Selection state
  - [ ] Scroll position
- [ ] Layout events:
  - [ ] `LayoutSaving` / `LayoutSaved`
  - [ ] `LayoutLoading` / `LayoutLoaded`
  - [ ] `LayoutRestoring` (cancel if version mismatch)
- [ ] Auto-save layout on form close (optional)
- [ ] Multiple named layouts (user can switch profiles)

### 5.5 Clipboard Operations
- [ ] Copy:
  - [ ] Copy selected nodes (with children)
  - [ ] Formats: plain text, CSV, HTML, custom Beep format
  - [ ] `CopyToClipboard()` method
- [ ] Paste:
  - [ ] Paste as new nodes from clipboard
  - [ ] Validate paste compatibility
  - [ ] `PasteFromClipboard()` method
- [ ] Cut:
  - [ ] Cut selected nodes (mark for move)
  - [ ] Visual indication (grayed out)

### 5.6 Accessibility
- [ ] MSAA / UI Automation support:
  - [ ] Implement `AccessibleObject` for tree and nodes
  - [ ] Expose hierarchy, selection, expand/collapse state
  - [ ] Screen reader announcements for selection changes
- [ ] Keyboard accessibility:
  - [ ] Full operation without mouse
  - [ ] Mnemonics and shortcuts
- [ ] High contrast theme support

## Success Criteria
- Export 10k nodes to Excel < 3 seconds
- Print preview shows correct multi-page layout
- Breadcrumb updates and navigates correctly
- Save/restore layout preserves all column and filter settings
- Screen reader correctly announces node count and selection

## Dependencies
- Phase 1: Foundation & Stability
- Phase 2: Data Binding & Multi-Column (export/print need columns)
- Phase 4: Data Shaping (layout persistence includes sort/filter state)

## Estimated Effort
4-5 days

## Research References
- DevExpress TreeList: Export, print, breadcrumb, layout persistence
- Telerik RadTreeView: XML serialization
- Syncfusion TreeViewAdv: Print and export
