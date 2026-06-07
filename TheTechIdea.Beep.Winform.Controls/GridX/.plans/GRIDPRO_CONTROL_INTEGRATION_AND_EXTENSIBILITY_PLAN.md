# GridPro Control Integration & Extensibility Plan

Priority: High | Status: Planning | Date: 2026-06-06

## 1. Current State Analysis

### 1.1 Renderer Architecture (how cells are painted)

GridX currently uses a **hard-coded switch** in `GridRenderHelper.CellContent.cs` for rendering
special column types:

```
Column Type    Rendering Method
-----------    ----------------
Text           TextRenderer.DrawText() — native GDI+
CheckBox       BeepCheckBox.Draw(g, rect) — painted control
ComboBox       BeepComboBox.Draw(g, rect) — painted control
DateTime       BeepDatePicker.Draw(g, rect) — painted control
Image          ImagePainter.DrawImage(g, rect) — GDI+
```

This means:
- Controls like **BeepChart**, **BeepListBox**, **BeepTree**, **BeepRadioGroup**,
  **BeepProgressBar**, **BeepSwitch**, **BeepRating**, etc. CANNOT be rendered in cells.
- Adding a new control type requires modifying grid core code (`CellContent.cs`).
- There is **no pluggable cell renderer interface** — it's a switch statement.

### 1.2 Editor Architecture (how cells are edited)

GridX has a pluggable editor framework:
```
IGridEditor interface → GridEditorFactory → Register(type, editor)
├── BeepGridTextEditor
├── BeepGridComboBoxEditor
├── BeepGridDateDropDownEditor
├── BeepGridNumericEditor
├── BeepGridMaskedEditor
└── (custom via GridEditorFactory.Register)
```

This is already extensible via `GridEditorFactory.Register()`, but editors are **temporary**
(shown and disposed) — they only appear during edit, not for static display.

### 1.3 What BaseControl/IBeepUIComponent Controls Exist

The following Beep controls could provide value as cell renderers/editors:

| Control | Useful as Renderer? | Useful as Editor? | Missing from GridX? |
|---------|:---:|:---:|:---:|
| BeepTextBox | ✅ already | ✅ already | No |
| BeepComboBox | ✅ already | ✅ already | No |
| BeepDatePicker/BeepDateDropDown | ✅ already | ✅ already | No |
| BeepCheckBox | ✅ already | ✅ (via toggle) | No |
| BeepNumericTextBox | No | ✅ already | No |
| **BeepListBox** | ✅ rows/list | ✅ dropdown select | **MISSING** |
| **BeepTree** | ✅ hierarchy | ✅ tree select | **MISSING** |
| **BeepChart** | ✅ bars/lines | No (display-only) | **MISSING** |
| **BeepProgressBar** | ✅ progress | No (display-only) | **MISSING** |
| **BeepSwitch** | ✅ toggle | ✅ toggle click | **MISSING** |
| **BeepRating** | ✅ stars | ✅ click rating | **MISSING** |
| **BeepRadioGroup** | ✅ radio set | ✅ radio select | **MISSING** |
| **BeepBadge/Counter** | ✅ status dots | No (display-only) | **MISSING** |
| **BeepSlider** | ✅ scrub bar | ✅ drag edit | **MISSING** |
| **BeepColorPicker** | ✅ swatch | ✅ color select | **MISSING** |
| **BeepLinkLabel** | ✅ hyperlink | No (display-only) | **MISSING** |
| **BeepImage** | ✅ already | No (display-only) | No |
| **BeepButton** | No | No | Not needed |

---

## 2. Proposed Architecture: Pluggable Cell Renderer System

### 2.1 New Interface: `ICellContentRenderer`

```csharp
public interface ICellContentRenderer
{
    /// Called at paint time. The renderer draws itself into the cell rect.
    void DrawCellContent(Graphics g, Rectangle cellRect, BeepCellConfig cell,
                         BeepRowConfig row, BeepColumnConfig column, IBeepTheme theme,
                         bool isSelected, bool isFocused);

    /// Preferred size for auto-column sizing.
    Size GetPreferredSize(Graphics g, BeepCellConfig cell, BeepColumnConfig column);

    /// Whether the cell is click-active (responds to mouse events).
    bool HandlesMouseInput { get; }

    /// Handle mouse click within a cell (for toggle/rating-like controls).
    void OnCellClick(Point location, BeepCellConfig cell, BeepRowConfig row, BeepColumnConfig column);
}
```

### 2.2 Default Implementations (migrated from existing switch)

```csharp
public class TextCellRenderer : ICellContentRenderer
{
    public void DrawCellContent(Graphics g, Rectangle cellRect, ...)
    {
        TextRenderer.DrawText(g, cell.CellValue?.ToString(), ...);
    }
}

public class CheckBoxCellRenderer : ICellContentRenderer
{
    public bool HandlesMouseInput => true;
    public void DrawCellContent(Graphics g, Rectangle cellRect, ...) { ... }
    public void OnCellClick(...) { cell.CellValue = !(bool?)cell.CellValue; }
}

public class ComboBoxCellRenderer : ICellContentRenderer { ... }
public class DatePickerCellRenderer : ICellContentRenderer { ... }
public class ImageCellRenderer : ICellContentRenderer { ... }
```

### 2.3 New Control Renderers (not implemented yet)

```csharp
// Uses BeepProgressBar.Draw(g, rect) for static display
public class ProgressBarCellRenderer : ICellContentRenderer { ... }

// Uses BeepChart.Draw(g, rect) for miniature inline chart
public class ChartCellRenderer : ICellContentRenderer { ... }

// Uses BeepListBox.Draw(g, rect) for multi-value display
public class ListBoxCellRenderer : ICellContentRenderer { ... }

// Uses BeepRating.Draw(g, rect) for star display + click handling
public class RatingCellRenderer : ICellContentRenderer
{
    public bool HandlesMouseInput => true;
    public void OnCellClick(Point location, ...) { ... }
}

// Uses BeepSwitch.Draw(g, rect) for toggle display + click
public class SwitchCellRenderer : ICellContentRenderer
{
    public bool HandlesMouseInput => true;
    public void OnCellClick(Point location, ...) { cell.CellValue = !(bool?)cell.CellValue; }
}

// Uses BeepColorPicker.Draw(g, rect) for swatch display
public class ColorSwatchCellRenderer : ICellContentRenderer { ... }

// Uses BeepLinkLabel.Draw(g, rect) for hyperlink display
public class LinkCellRenderer : ICellContentRenderer { ... }
```

### 2.4 Renderer Registry (Factory Pattern)

```csharp
public static class CellRendererRegistry
{
    private static readonly Dictionary<BeepColumnType, ICellContentRenderer> _renderers = new();
    private static readonly Dictionary<string, ICellContentRenderer> _customRenderers = new();

    static CellRendererRegistry()
    {
        Register(BeepColumnType.Text, new TextCellRenderer());
        Register(BeepColumnType.CheckBox, new CheckBoxCellRenderer());
        Register(BeepColumnType.ComboBox, new ComboBoxCellRenderer());
        Register(BeepColumnType.DateTime, new DatePickerCellRenderer());
        Register(BeepColumnType.Image, new ImageCellRenderer());
    }

    public static void Register(BeepColumnType type, ICellContentRenderer renderer)
        => _renderers[type] = renderer;

    public static void Register(string columnName, ICellContentRenderer renderer)
        => _customRenderers[columnName] = renderer;

    public static ICellContentRenderer Get(BeepColumnConfig column)
    {
        if (_customRenderers.TryGetValue(column.Name, out var custom))
            return custom;
        if (_renderers.TryGetValue(column.ColumnType, out var typed))
            return typed;
        return _renderers[BeepColumnType.Text]; // fallback
    }
}
```

### 2.5 Integration into GridRenderHelper

Replace the hard-coded switch in `CellContent.cs` with:

```csharp
// Before (hard-coded):
//   case BeepColumnType.CheckBox: BeepCheckBox.Draw(g, rect); break;
//   case BeepColumnType.ComboBox: BeepComboBox.Draw(g, rect); break;

// After (pluggable):
private void DrawCellContent(Graphics g, Rectangle cellRect, BeepCellConfig cell,
                              BeepRowConfig row, BeepColumnConfig column, bool isSelected, bool isFocused)
{
    var renderer = CellRendererRegistry.Get(column);
    renderer.DrawCellContent(g, cellRect, cell, row, column, _theme, isSelected, isFocused);
}
```

---

## 3. Proposed: SmartCellAdapter for Any BaseControl Control

### 3.1 Concept

A generic adapter that wraps ANY `BaseControl` and adapts it to both `ICellContentRenderer`
(static display) AND `IGridEditor` (edit mode):

```csharp
public class SmartCellAdapter<TControl> : ICellContentRenderer, IGridEditor
    where TControl : BaseControl, new()
{
    private TControl _control;
    private Func<TControl, object, object> _valueGetter;
    private Action<TControl, object> _valueSetter;

    public SmartCellAdapter(
        Func<TControl, object, object> valueGetter = null,
        Action<TControl, object> valueSetter = null)
    {
        _valueGetter = valueGetter;
        _valueSetter = valueSetter;
    }

    // ICellContentRenderer — static display
    public void DrawCellContent(Graphics g, Rectangle cellRect, ...)
    {
        _control.Width = cellRect.Width;
        _control.Height = cellRect.Height;
        _control.Draw(g, cellRect);  // Each BaseControl has a Draw(g, rect) method
    }

    // IGridEditor — live editing
    public Control CreateEditor(BeepColumnConfig column, object currentValue) { ... }
    public void SetupEditor(BeepCellConfig cell, BeepColumnConfig column) { ... }
    public object GetValue() { ... }
    public void SetValue(object val) { ... }
}
```

### 3.2 Usage: Integrating BeepChart as a Cell Renderer

```csharp
// Register in form load or app startup:
var chartRenderer = new SmartCellAdapter<BeepChart>(
    valueGetter: (ctrl, data) => ctrl.GetChartData(),
    valueSetter: (ctrl, data) => ctrl.UpdateChartData((IEnumerable<ChartPoint>)data));

CellRendererRegistry.Register(BeepColumnType.Chart, chartRenderer);
GridEditorFactory.Register(BeepColumnType.Chart, chartRenderer);
```

### 3.3 Usage: Integrating BeepListBox as a Cell Renderer

```csharp
CellRendererRegistry.Register("Tags", new SmartCellAdapter<BeepListBox>(
    valueGetter: (ctrl, data) => ctrl.SelectedItems,
    valueSetter: (ctrl, data) => ctrl.SetItems((List<string>)data)));
```

---

## 4. Data Management Improvements

### 4.1 Current Data Binding Gaps

| Gap | Severity | Description |
|-----|----------|-------------|
| ObservableCollection<T> auto-refresh | High | Planning fix — currently rebinds, should do `INotifyCollectionChanged` delta update |
| DataTable fast cell edit | High | Full rebind on edit, should use `DataColumn.Expression` or direct cell write |
| Schema change detection | Medium | Add/remove columns not detected on DataTable |
| INPC on add/remove | Medium | ObservableCollection.Add/Remove triggers full grid refresh when it should just insert/remove rows |
| UoW sync gap | Medium | `IUnitOfWorkWrapper` events fire but mapping to row changes is lossy |
| Filtered view stability | Medium | Filtering causes positions to shift unexpectedly during binding source refresh |

### 4.2 Proposed: `GridDataSourceAdapter<T>` — Unified Fast Path

```csharp
public class GridDataSourceAdapter<T> : IDisposable
{
    private readonly BeepGridPro _grid;
    private readonly List<T> _source;
    private readonly Func<T, object> _keySelector;
    private readonly Dictionary<object, int> _keyToRowIndex = new();

    public void NotifyAdd(T item)
    {
        int idx = _grid.Data.Rows.Count;
        _grid.Data.Rows.Insert(idx, MaterializeRow(item, idx));
        _keyToRowIndex[_keySelector(item)] = idx;
        _grid.InvalidateRow(idx);
    }

    public void NotifyRemove(object key)
    {
        if (_keyToRowIndex.TryGetValue(key, out int idx))
        {
            _grid.Data.Rows.RemoveAt(idx);
            _keyToRowIndex.Remove(key);
            ReindexFrom(idx);
            _grid.Invalidate();
        }
    }

    public void NotifyUpdate(T item)
    {
        var key = _keySelector(item);
        if (_keyToRowIndex.TryGetValue(key, out int idx))
        {
            UpdateRow(_grid.Data.Rows[idx], item);
            _grid.InvalidateRow(idx);
        }
    }
}
```

### 4.3 Performance Targets

| Operation | Target | Current |
|-----------|--------|---------|
| Insert single row | < 1ms | ~5ms (full rebind) |
| Update cell value | < 0.5ms | ~3ms (full row invalidate) |
| Scroll 100 rows | < 16ms (60fps) | ~8ms (already virtualized) |
| Add 1000 rows | < 500ms | ~2000ms |
| Sort 10k rows | < 200ms | Unmeasured |

---

## 5. Extension Registration — Zero-Code User Experience

### 5.1 Design-Time Integration

When a user drops a `BeepChart`, `BeepListBox`, or other control onto a form
alongside a `BeepGridPro`, the grid should **auto-discover** and register them
as available column types.

```csharp
// In BeepGridPro.Load event or constructor:
private void DiscoverAvailableControls()
{
    var form = FindForm();
    if (form == null) return;

    foreach (Control ctrl in form.Controls)
    {
        if (ctrl is BaseControl beepCtrl && beepCtrl is not BeepGridPro)
        {
            RegisterControlAsRenderer(beepCtrl);
        }
    }
}
```

### 5.2 Column Configuration Wizard

Extend the existing column config dialog (`ShowColumnConfigDialog`) to show
available renderer types:

```
Column Type dropdown:
├── Text                    [built-in]
├── CheckBox                [built-in]
├── ComboBox                [built-in]
├── DateTime                [built-in]
├── Image                   [built-in]
├── ─────────────────────
├── Chart    (BeepChart)    [discovered on form]
├── Progress  (BeepProgressBar) [discovered on form]
├── ListVew  (BeepListBox)   [discovered on form]
└── Custom...               [manual registration]
```

---

## 6. Implementation Plan

### Phase 1: Renderer Interface & Registry (2 days)

| Task | Effort |
|------|--------|
| Define `ICellContentRenderer` interface | 0.25d |
| Create `CellRendererRegistry` static class | 0.25d |
| Migrate existing switch code to `TextCellRenderer`, `CheckBoxCellRenderer`, `ComboBoxCellRenderer`, `DatePickerCellRenderer`, `ImageCellRenderer` | 1d |
| Replace `CellContent.cs` switch with registry call | 0.25d |
| Wire mouse click to `ICellContentRenderer.OnCellClick` in `GridInputHelper` | 0.25d |

### Phase 2: New Cell Renderers (3 days)

| Task | Effort |
|------|--------|
| `ProgressBarCellRenderer` — wraps `BeepProgressBar.Draw()` | 0.25d |
| `RatingCellRenderer` — wraps `BeepRating` stars + click | 0.5d |
| `SwitchCellRenderer` — wraps `BeepSwitch` toggle + click | 0.5d |
| `ColorSwatchCellRenderer` — wraps `BeepColorPicker` swatch | 0.25d |
| `LinkCellRenderer` — hyperlink display + click action | 0.25d |
| `ChartCellRenderer` — inline `BeepChart` (miniature, no axes) | 0.5d |
| `ListBoxCellRenderer` — multi-value comma display or `BeepListBox.Draw` | 0.5d |
| `BadgeCellRenderer` — wraps `BeepBadge` for status dots/pills | 0.25d |

### Phase 3: SmartCellAdapter (2 days)

| Task | Effort |
|------|--------|
| Create `SmartCellAdapter<TControl>` generic class | 1d |
| Implement `ICellContentRenderer` via `control.Draw(g, rect)` | 0.25d |
| Implement `IGridEditor` via `control` show/hide lifecycle | 0.5d |
| Add zero-arg registration API: `SmartCellAdapter.CreateFor<T>()` | 0.25d |

### Phase 4: Data Management Improvements (3 days)

| Task | Effort |
|------|--------|
| Implement `GridDataSourceAdapter<T>` for delta updates | 1d |
| Wire `INotifyCollectionChanged` to delta events | 0.5d |
| DataTable fast cell edit (avoid full rebind) | 1d |
| Schema change detection for DataTable (add/remove columns) | 0.5d |

### Phase 5: Auto-Discovery & Design-Time (1 day)

| Task | Effort |
|------|--------|
| Form control scanning for available BaseControl types | 0.25d |
| Column type dropdown populated from discovered controls | 0.25d |
| Property grid integration for `RendererType` on `BeepColumnConfig` | 0.25d |
| `SetColumnRenderer(colIdx, "MyCustom")` runtime API | 0.25d |

---

## 7. API Surface — What the User Writes

### Before (current)
```csharp
// No way to add custom cell renderer
// Must write text values and hope for the best
```

### After (with these changes)
```csharp
// Option 1: Register a built-in type
grid.SetColumnRenderer(3, typeof(BeepProgressBar));
grid.SetColumnRenderer(4, typeof(BeepRating));

// Option 2: Register using SmartCellAdapter
CellRendererRegistry.Register("MyChart", new SmartCellAdapter<BeepChart>());
grid.Columns["Sparkline"].RendererType = "MyChart";

// Option 3: Register via fluent API
grid.Columns["Status"]
    .UseRenderer<ProgressBarCellRenderer>()
    .WithConfig(new { Min = 0, Max = 100, Color = "Green" });

// Option 4: Custom renderer
public class ThermometerRenderer : ICellContentRenderer { ... }
CellRendererRegistry.Register("Thermometer", new ThermometerRenderer());
grid.SetColumnRenderer(5, "Thermometer");

// Data management improvements:
var adapter = new GridDataSourceAdapter<Customer>(grid, customers, c => c.Id);
customers.CollectionChanged += (s, e) =>
{
    if (e.NewItems != null)
        foreach (Customer c in e.NewItems) adapter.NotifyAdd(c);
    if (e.OldItems != null)
        foreach (Customer c in e.OldItems) adapter.NotifyRemove(c.Id);
};
```

---

## 8. Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Breaking change to existing cell rendering | Low | High | ICellContentRenderer defaults to returning null, grid falls back to TextRenderer |
| SmartCellAdapter perf for complex controls (Chart) | Medium | Medium | `GetPreferredSize` returns fixed small size; miniature rendering |
| Registry thread-safety (AddRenderer during paint) | Low | Low | Registry is read-only after form load; `ConcurrentDictionary` if needed |
| ColorPicker popup during inline edit | Medium | Medium | Popup shown as modal overlay, not dropdown (cell too small) |
| Designer serialization of renderer config | Medium | Low | Use `BeepColumnConfig.RendererType` string, serialized to designer.cs |

---

## 9. Timeline

| Phase | Duration | Dependency |
|-------|----------|------------|
| Phase 1: Renderer Interface | 2d | None |
| Phase 2: New Renderers | 3d | Phase 1 |
| Phase 3: SmartCellAdapter | 2d | Phase 1 |
| Phase 4: Data Management | 3d | Partial (independent) |
| Phase 5: Auto-Discovery | 1d | Phase 1 |
| **Total** | **11 days** | |

Phases 2-5 can run largely in parallel after Phase 1.
