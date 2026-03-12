# BeepGridPro Documentation

This folder documents the current `BeepGridPro` implementation in `GridX`.

## Read This First
- `BeepGridPro` uses a helper-based architecture. Public API lives on the control; most behavior lives in helper classes.
- Filtering is split between the public `ActiveFilter` path and the internal `SortFilter` helper path.
- `GridLayoutPreset` and `BeepGridStyle` enums contain values that are ahead of the currently wired runtime switch logic.

## Document Map
- [Usage.md](./Usage.md): binding, columns, selection, editing, navigator, clipboard
- [Classes.md](./Classes.md): class and helper map
- [FilteringSorting.md](./FilteringSorting.md): quick filter, advanced filter, Excel-style popup filtering, and sort behavior
- [Styling.md](./Styling.md): grid style, navigation style, focus styling, layout presets, and current gaps
- [Events.md](./Events.md): events and what actually raises them
- [Extensibility.md](./Extensibility.md): adapters, custom layouts, painters, and UOW integration

## Minimal Example
```csharp
var grid = new BeepGridPro
{
    Dock = DockStyle.Fill,
    GridStyle = BeepGridStyle.Bootstrap,
    ShowNavigator = true,
    ShowTopFilterPanel = true,
    GridTitle = "Customers"
};

grid.DataSource = customersBindingSource;
grid.AutoGenerateColumns();
grid.EnableExcelFilter();
```

At runtime the constructor already hooks `EnableExcelFilter()`, so the explicit call is optional.

## Current Behavior Highlights
- `ShowTopFilterPanel` defaults to `true`.
- `UseInlineQuickSearch` defaults to `true`.
- `FilterIconVisibility` defaults to `Hidden`.
- `Uow` currently expects `IUnitofWork` or `IUnitOfWorkWrapper`.
- `GridLayoutPreset` property support is narrower than the enum itself.
