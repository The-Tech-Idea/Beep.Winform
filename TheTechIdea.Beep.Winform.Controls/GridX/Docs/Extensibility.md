# Extensibility

## Add a New Layout

Implement `IGridLayoutPreset`:
```csharp
public sealed class MyGridLayout : IGridLayoutPreset
{
    public void Apply(BeepGridPro grid)
    {
        if (grid == null) return;

        grid.RowHeight = 28;
        grid.ColumnHeaderHeight = 32;
        grid.Render.ShowGridLines = true;
        grid.Render.ShowRowStripes = true;
    }
}
```

Apply it directly:
```csharp
grid.ApplyLayoutPreset(new MyGridLayout());
```

If you also want enum-based property support:
1. add the enum value to `GridLayoutPreset`
2. extend `BeepGridPro.ApplyLayoutPreset(GridLayoutPreset)`

## Add a New Navigation Painter

1. Create a painter under `GridX/Painters`.
2. Implement the relevant base/interface.
3. Register it in `NavigationPainterFactory`.
4. Add the enum value to `navigationStyle` if needed.

Keep in mind:
- recommended height must be consistent with the layout pipeline
- hit areas must be registered so navigator clicks work

## Add a New Header Painter

1. Implement `IPaintGridHeader` or extend the existing base pattern.
2. Register it in `HeaderPainterFactory`.
3. Make sure sort/filter icon hit rectangles still line up with what `GridInputHelper` expects.

## Add a New Filter Panel Painter

Filter panel painters are the right place for top-panel visual changes.

When changing them:
- keep action keys consistent with `BaseFilterPanelPainter`
- preserve search, advanced, clear-all, and per-column chip hit areas
- keep `GridInputHelper` expectations in sync

## Extend Filtering

### Public advanced filter path
Extend `BeepGridPro.Filtering.cs` when you need:
- richer `FilterConfiguration` support
- new quick-filter semantics
- new `FilterAppliedEventArgs` behavior

### Helper source-level path
Extend `GridSortFilterHelper.cs` when you need:
- new source-level sort/filter strategies
- better local filter fallback
- UOW-aware sorting rules

Document clearly which path you changed.

## Reuse Excel Popup Filtering

Use `BeepGridProAdapter` when you want to reuse `BeepSimpleGridLike` tooling from inside GridX or other code that already has access to the internal sort/filter hooks.

For regular consumer code, prefer:
```csharp
grid.EnableExcelFilter();
```

This is the supported entry point for Excel-style popup reuse from outside the control internals.

## UOW Integration

`GridUnitOfWorkBinder` is the integration point for UOW-backed screens.

Use it when you need to:
- refresh rows after UOW lifecycle events
- preserve selection after UOW rebinding
- coordinate wrapper-only UOW event forwarding

Do not add reflection-heavy UOW plumbing to the main control when the binder can own it.

## Add a New Helper

Helper pattern expectations:
- store a private readonly `BeepGridPro _grid`
- keep helper scope focused
- let `BeepGridPro` own helper construction
- coordinate through `_grid`, not through service locators or dependency injection

## Safe Extension Rules
- prefer changing a helper over patching `DrawContent()`
- update layout and invalidation together when geometry changes
- do not assume every enum value is already wired to runtime behavior
- preserve system-column rules when adding reorder, sort, filter, or sizing features
