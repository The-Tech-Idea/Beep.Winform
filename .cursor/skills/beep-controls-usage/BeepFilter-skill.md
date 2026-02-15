# BeepFilter Skill

## Purpose
Use `BeepFilter` as a reusable filter-UI builder. It creates and edits `FilterConfiguration`, while host controls/services apply that configuration to data (for example via `FilterEngine<T>`).

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Filtering;
```

## Core Architecture (actual implementation)
- `BeepFilter` (UI + interaction + events)
- `IFilterPainter` + concrete painters (8 styles)
- `FilterConfiguration` / `FilterCriteria` data contracts
- `FilterEngine<T>` generic execution engine
- Optional helpers: keyboard, suggestions, validation, autocomplete popup

`BeepFilter` is painter-driven. Layout and hit testing are delegated to painter classes, and interaction routes through hit areas (`FilterHitAreaType`).

## FilterStyle (8 implemented)
```csharp
public enum FilterStyle
{
    TagPills,
    GroupedRows,
    QueryBuilder,
    DropdownMultiSelect,
    InlineRow,
    SidebarPanel,
    QuickSearch,
    AdvancedDialog
}
```

Factory mapping is in `FilterPainterFactory.CreatePainter(...)` and all 8 styles are marked implemented by `IsFullyImplemented(...)`.

## FilterOperator (17)
- Equals, NotEquals
- Contains, NotContains, StartsWith, EndsWith
- GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual
- Between, NotBetween
- IsNull, IsNotNull
- Regex, In, NotIn

Use type-aware operator sets:
```csharp
var ops = FilterOperatorExtensions.GetOperatorsForType(typeof(string));
```

## Important Public Properties (real API)
- `FilterStyle`, `DisplayMode`, `Position`, `IsExpanded`
- `ActiveFilter` (primary config state)
- `FilterCount`, `HasFilters`
- `EntityStructure` (column metadata source)
- `AvailableColumns` (derived from `EntityStructure.Fields`)
- `AutoApply`, `ShowActionButtons`, `EnableDragDrop`, `ShowFilterCountBadge`
- `KeyboardShortcutsEnabled`, `AutocompleteEnabled`, `ValidationEnabled`, `AutocompleteDataSource`

## Event Model (host integration points)
Primary lifecycle/events:
- `FilterChanged`, `FilterApplied`, `FiltersCleared`
- `FilterAdded`, `FilterRemoved`, `FilterModified`
- `FilterEditRequested`

Interaction events:
- `FieldSelectionRequested`
- `OperatorSelectionRequested`
- `ValueInputRequested`
- `FilterDragStarted`
- `SectionToggled`, `SearchFocusRequested`

Configuration events:
- `ConfigurationSaveRequested`, `ConfigurationLoadRequested`

## Correct Usage Pattern (no `AddCriteria` API on control)
`BeepFilter` does not expose `AddCriteria(...)` as a control method. Set/replace `ActiveFilter`.

```csharp
var filter = new BeepFilter
{
    FilterStyle = FilterStyle.TagPills,
    DisplayMode = FilterDisplayMode.AlwaysVisible,
    EntityStructure = entityStructure
};

var config = new FilterConfiguration("Default")
{
    Logic = FilterLogic.And,
    Criteria = new List<FilterCriteria>
    {
        new FilterCriteria("Status", FilterOperator.Equals, "Active")
    }
};

filter.ActiveFilter = config;

filter.FilterApplied += (s, e) =>
{
    var engine = new FilterEngine<MyRow>(rows);
    var result = engine.ApplyFilter(filter.ActiveFilter).ToList();
};
```

## Host Wiring Pattern for Drop-down/Input UX
When a painter emits interaction events, host should supply UI pickers/editors:

```csharp
filter.FieldSelectionRequested += (s, e) => { /* show column picker */ };
filter.OperatorSelectionRequested += (s, e) => { /* show operator picker */ };
filter.ValueInputRequested += (s, e) => { /* show value editor */ };
```

## Keyboard Shortcuts (from `FilterKeyboardHandler`)
Implemented routing includes:
- `Ctrl+F`, `Ctrl+N`, `Ctrl+S`, `Ctrl+O`, `Ctrl+Z`, `Ctrl+Y`
- `Ctrl+Shift+F`, `Ctrl+Shift+C`, `Ctrl+Shift+D`, `Ctrl+Shift+E`, `Ctrl+Shift+I`
- `Alt+Up`, `Alt+Down`, `Alt+1..9`
- `Enter`, `Escape`, `Delete`, `Tab`, `F1`, `F2`

Note: several callbacks currently show "Coming in Phase 2" dialogs (command palette, advanced saved views, import/export, etc.).

## Data Contracts
`FilterConfiguration`
- `Name`, `Description`, `Criteria`, `Logic`, `IsActive`, timestamps

`FilterCriteria`
- `ColumnName`, `Operator`, `Value`, `Value2`, `CaseSensitive`, `IsEnabled`

## Engine Capabilities (`FilterEngine<T>`)
- Reflection-based property cache
- AND/OR logic combination
- Quick search across all/single property
- Returns matching indices (`ApplyFilterGetIndices`)
- Supports all 17 operators including regex and comma-list `In/NotIn`

## Advanced Helpers in Folder
- `FilterSuggestionProvider`: recent/frequent/fuzzy suggestions with cache
- `FilterValidationHelper`: operator/type validation with error/warning messages
- `FilterAutocompletePopup`: themed dropdown with keyboard selection

## Painter Contract Summary
Each painter must implement:
- `CalculateLayout(...)`
- `Paint(...)`
- `HitTest(...)`
- `GetMetrics(...)`
- flags: `SupportsAnimations`, `SupportsDragDrop`

Hit areas and rectangles are represented by `FilterLayoutInfo` and `FilterHitAreaType`.

## Common Mistakes to Avoid
- Do not call non-existent control APIs like `BeepFilter.AddCriteria(...)`.
- Do not expect `BeepFilter` to apply data filtering automatically.
- Always provide valid `EntityStructure` so column dropdown logic has metadata.
- In host integration, update `ActiveFilter` immutably (clone/update/set) to force repaint/layout/event flow.

## File Map (quick reference)
- `BeepFilter.cs` / `.Properties.cs` / `.Events.cs` / `.HitTest.cs` / `.Layout.cs`
- `FilterPainterFactory.cs`, `IFilterPainter.cs`, `BaseFilterPainter.cs`
- `Painters/*.cs` (8 concrete styles)
- `FilterCriteria.cs`, `FilterOperator.cs`, `FilterEngine.cs`
- `FilterKeyboardHandler.cs`, `FilterSuggestionProvider.cs`, `FilterValidationHelper.cs`, `FilterAutocompletePopup.cs`
