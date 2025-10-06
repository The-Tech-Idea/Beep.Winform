# BeepGridPro Documentation

This folder documents BeepGridPro, a modern, helper-driven WinForms grid control in TheTechIdea.Beep.Winform.Controls. It provides an overview, class reference, and usage guidance for developers.

Contents
- Overview and concepts
- Quick start
- Class map (what each class does)
- Data binding, columns, selection, editing
- Sorting and filtering (Excel-style)
- Styling and theming
- Extensibility and adapters

Overview
BeepGridPro is a composable grid built from small helper components:
- Rendering, layout, scrolling, selection, editing, navigation, sizing, sorting/filtering, dialogs, and theming are separated into dedicated helpers under `GridX/Helpers`.
- Optional Excel-style filter UI is provided under `GridX/Filters` and helper extensions.
- A thin adapter interface (`BeepSimpleGridLike`) enables non-invasive integration of filter popups.

Key features
- Data binding: BindingSource, IEnumerable<T>, DataTable/DataView, DataSet + DataMember
- Auto-generate columns or design-time columns; includes system columns (checkbox, row number, row ID)
- Custom drawing with sticky columns and custom scrollbars
- Selection via header/row checkboxes and active cell highlighting
- Editing via modeless overlay editor or in-place editor helper
- Sorting/filtering (including Excel-style filter popup)
- Styling via `GridStyle` presets and themes (`BeepThemesManager`)
- Owner-drawn navigator with CRUD and record navigation

Quick start
```csharp
var grid = new BeepGridPro
{
    Dock = DockStyle.Fill,
    GridStyle = BeepGridStyle.Clean,
    ShowNavigator = true,
};

// Bind any list, BindingSource, DataTable/DataView or DataSet + DataMember
grid.DataSource = myBindingSource;      // or a List<T>, DataTable, DataView
grid.DataMember = "Customers";          // only when needed (DataSet/DataViewManager/root object)

// Auto-generate columns from entity/schema
grid.AutoGenerateColumns();

// Optional: ensure system columns (Sel, RowNum, RowID)
grid.EnsureSystemColumns();

// Optional: Excel-like filter icons in headers (hover shows icons)
grid.EnableExcelFilter();

// Optional: auto-size columns/rows
grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
grid.AutoResizeColumnsToFitContent();
```

Events
- `RowSelectionChanged`: raised when row checkbox selection changes
- `CellValueChanged`: raised after an editor commits a value
- `SaveCalled`: raised when Save action is invoked from navigator

Where to go next
- See `Classes.md` for a detailed class-by-class breakdown
- See `Usage.md` for binding, columns, selection, editing
- See `FilteringSorting.md` for sort/filter and Excel filter popup
- See `Styling.md` for appearance presets and theme integration
- See `Extensibility.md` for custom drawers, adapters, and UoW binder
- See `Events.md` for events and integration tips
```