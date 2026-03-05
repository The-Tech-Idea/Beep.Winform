# BeepGridPro — DataSource Handling: Investigation & Enhancement Plan

_Generated: 2026-03-04_

---

## 1. How Each DataSource Type Is Currently Handled

### 1.1 DataTable

| Step | What happens |
|------|-------------|
| `DataSource = myTable` | `BeepGridPro.Properties.cs` setter → `Data.Bind(value)` + `Navigator.BindTo(value)` |
| Navigator wrapping | `GridNavigatorHelper.ResolveBindingSource()` wraps the DataTable in a new `BindingSource` |
| Enumeration / Schema | `GetEffectiveEnumerableWithSchema()` detects `DataTable` → returns `dt.DefaultView` as enumerable, `dt` as schema |
| Column generation | Uses `DataTable.Columns` (schema path in `AutoGenerateColumns`) |
| Row population | Items are `DataRowView`; `RefreshRows()` reads `drv.Row[col.ColumnName]` |
| Change notifications | `BindingSource` wraps `DataView` which proxies `DataTable.RowChanged/RowAdded/RowDeleted` → `BindingSource.ListChanged` → `BindingSource_ListChanged` in navigator |

**Gaps identified:**
- `DataRowView` does **not** implement `INotifyPropertyChanged`. The fast-path `canFastRefresh` check in `BindingSource_ListChanged` always fails → every DataTable cell edit causes a full `Data.Bind + InitializeData + Layout.Recalculate` (O(n) cost per keystroke).
- No `DataTable.RowChanged` / `DataTable.TableNewRow` direct subscription inside `GridDataHelper`. Live cell tracking requires the BindingSource proxy, which always does full rebind for `ItemChanged` on DataRowView.
- `_historyDt.Rows.Clear()` triggers `DataTable.TableCleared` → `DataView.ListChanged(Reset)` → full rebind. Acceptable for small history grids; problematic at scale.

---

### 1.2 BindingSource

| Step | What happens |
|------|-------------|
| `DataSource = myBindingSource` | Setter detects `value is BindingSource` → always rebinds |
| Navigator wrapping | `HookBindingSource(bs)` used directly (no new BindingSource created) |
| Change notifications | `ListChanged`, `DataSourceChanged`, `CurrentChanged`, `PositionChanged` all wired in `HookBindingSource` |
| `ItemChanged` fast path | If `Rows[e.NewIndex].RowData is INotifyPropertyChanged` → `InvalidateRow` only |
| All other `ListChangedType` | Full `Data.Bind + InitializeData + Layout.Recalculate` |
| `DataSourceChanged` | Full rebind |

**Gaps identified:**
- When a `BindingSource` wrapping a `DataTable` is assigned, `ResolveDataForBinding` calls `bs.List ?? bs.DataSource`. `bs.List` for a BindingSource-over-DataTable is a `DataView` — correct. However `GetEffectiveEnumerableWithSchema` has a second BindingSource check that is now dead code because `ResolveDataForBinding` already unwrapped it.
- If the BindingSource `DataSource` is changed after bind (e.g., `bs.DataSource = newTable`), `BindingSource_DataSourceChanged` fires and re-executes full bind — correct, but does not re-run `AutoGenerateColumns` for new schema. The new table's columns will be missed if user columns already existed.
- `PositionChanged` and `CurrentChanged` both do the same selection sync — redundant handler pairs causing double selection update per navigation step.

---

### 1.3 List\<T\> / Array / IEnumerable\<T\>

| Step | What happens |
|------|-------------|
| Enumeration | `GetEffectiveEnumerableWithSchema()` returns it as `IEnumerable`, no schema |
| Column generation | Reflection on `T` via `AutoGenerateColumns` |
| Change notifications | **None**. Navigator wraps it in a `BindingSource`, but plain `List<T>` does not fire `ListChanged`. The BindingSource wrapping a `List<T>` does NOT forward mutations (Add/Remove/Clear on the original list are invisible). |
| Manual refresh | User must call `grid.RefreshData()` explicitly |

**Gaps identified:**
- This is a silent correctness trap: assigning `List<T>` works but mutations on that list are **never reflected** automatically. No warning or documentation.
- `BindingList<T>` does work (it implements `IBindingList`) — but it must be set directly, not as `IList<T>`.

---

### 1.4 BindingList\<T\>

| Step | What happens |
|------|-------------|
| Enumeration | `GetEffectiveEnumerableWithSchema()` returns it as `IEnumerable`; BindingSource detects `IBindingList` |
| Change notifications | `BindingSource.ListChanged` fires on Add/Remove/ItemChange. Navigator's `BindingSource_ListChanged` handles it. |
| `ItemChanged` fast path | If items implement `INotifyPropertyChanged` → `InvalidateRow` only |
| Column generation | Reflection on `T` |

**Status: Works correctly.** Best `List<T>` alternative.

---

### 1.5 ObservableCollection\<T\>

| Step | What happens |
|------|-------------|
| Assignment | Wrapped in a new `BindingSource` by Navigator |
| Change notifications | `ObservableCollection<T>` implements `INotifyCollectionChanged`. `BindingSource` wrapping it uses internal `ListChangedEventArgs` proxy via `IBindingList`? Actually NO — BindingSource does NOT automatically subscribe to `INotifyCollectionChanged`. It only handles `IBindingList`. |

**Gaps identified (critical):**
- `ObservableCollection<T>` does **NOT** implement `IBindingList`. When wrapped by `BindingSource`, mutations on the collection (`Add`, `Remove`, `Clear`) are **not forwarded** to `BindingSource.ListChanged`.
- The grid has **no direct `INotifyCollectionChanged` subscription** in `GridDataHelper` for non-UoW paths. Only `GridUnitOfWorkBinder` subscribes to `INotifyCollectionChanged` for UoW Units.
- Result: Assigning `ObservableCollection<T>` as DataSource renders initial data but subsequent mutations are silent.

**Fix needed**: In `GridDataHelper.Bind()`, if the resolved data implements `INotifyCollectionChanged` (and not `IBindingList`), subscribe directly and call `RefreshRows()` on change.

---

### 1.6 UnitofWork\<T\>

| Step | What happens |
|------|-------------|
| Assignment | `Uow` property setter → `_uowBinder.Attach()` |
| Data feed | `GridUnitOfWorkBinder.RefreshBinding()` calls `GetUnits()` = `_uow.Units` → `_grid.Data.Bind(units)` |
| Change notifications | `IBindingList.ListChanged` on `Units` (fast ItemChanged path) AND `INotifyCollectionChanged` on `Units` (full rebind) AND UoW Pre/Post events |
| ItemChanged fast path | `InvalidateRow` if `INotifyPropertyChanged` |
| Insert/Delete/Query events | Full `RefreshBinding()` |
| Commit | `SafeInvalidate()` only (no row re-read) |

**Gaps identified:**
- `HandleUowPostChange` for `PostUpdate/PostEdit` only calls `SafeInvalidate()` — does NOT re-read cell values from the updated entity. If the UoW updates properties on the existing object, those changes are not picked up unless the item implements `INotifyPropertyChanged`.
- `HandleUowPostCommit` only calls `SafeInvalidate()`. After a commit that persists server-generated values (e.g., identity PKs), the grid won't show those new values.
- No test/guard: re-entering `RefreshBinding()` while `_isRefreshingBinding = true` is guarded via flag, but nested UoW events can still cause stale state.

---

## 2. Summary Findings Matrix

| DataSource Type | Initial Render | Add Row | Delete Row | Edit Cell | Schema Change |
|---|---|---|---|---|---|
| `DataTable` (raw) | ✅ | ✅ (via DV/BS proxy, full rebind) | ✅ (full rebind) | ⚠️ Full rebind (no fast path) | ❌ No auto-regenerate |
| `DataTable` via `BindingSource` | ✅ | ✅ (full rebind) | ✅ (full rebind) | ⚠️ Full rebind | ⚠️ Only if `DataSourceChanged` fires |
| `BindingSource` | ✅ | ✅ | ✅ | ✅ (fast if INPC) | ⚠️ Full rebind, no col re-gen |
| `List<T>` | ✅ | ❌ Silent | ❌ Silent | ❌ Silent | ❌ |
| `BindingList<T>` | ✅ | ✅ | ✅ | ✅ (fast if INPC) | ❌ |
| `ObservableCollection<T>` | ✅ | ❌ Silent | ❌ Silent | ❌ Silent | ❌ |
| `UnitofWork<T>` | ✅ | ✅ | ✅ | ⚠️ Fast if INPC, else stale | ❌ |
| `DataSet` + `DataMember` | ✅ | ✅ (via DV/BS proxy) | ✅ | ⚠️ Full rebind | ❌ |

---

## 3. Enhancement Plan

### Phase 1 — Fix ObservableCollection Live Updates (Critical)
**File**: `GridDataHelper.cs`

- In `Bind()`, after `EnsureSystemColumns()`, check if the resolved data implements `INotifyCollectionChanged` but **not** `IBindingList`.
- If so, subscribe to `CollectionChanged` with a handler that calls `RefreshRows()` + `Layout.Recalculate()` + `SafeInvalidate()`.
- Store a reference and unsubscribe on `ClearDataSource()` and on next `Bind()`.
- Add field: `private INotifyCollectionChanged? _subscribedCollectionChanged;`

```csharp
// In ClearDataSource():
if (_subscribedCollectionChanged != null)
{
    _subscribedCollectionChanged.CollectionChanged -= OnCollectionChanged;
    _subscribedCollectionChanged = null;
}

// In Bind(), after resolving:
if (resolved is INotifyCollectionChanged incc && resolved is not IBindingList)
{
    if (!ReferenceEquals(_subscribedCollectionChanged, incc))
    {
        if (_subscribedCollectionChanged != null)
            _subscribedCollectionChanged.CollectionChanged -= OnCollectionChanged;
        _subscribedCollectionChanged = incc;
        incc.CollectionChanged += OnCollectionChanged;
    }
}

private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
{
    RefreshRows();
    _grid.Layout.Recalculate();
    _grid.SafeInvalidate();
}
```

---

### Phase 2 — DataTable Cell-Level Fast Refresh (Performance)
**File**: `GridNavigatorHelper.cs` → `BindingSource_ListChanged`

- `DataRowView` itself fires `PropertyChanged` via `ICustomTypeDescriptor` in some contexts, but the cleaner approach is: in `BindingSource_ListChanged(ItemChanged)`, when `RowData is DataRowView drv`, read the specific column from `drv.Row` and update only the matching cell directly.

```csharp
if (e.ListChangedType == ListChangedType.ItemChanged && e.NewIndex >= 0 && e.NewIndex < _grid.Rows.Count)
{
    var row = _grid.Rows[e.NewIndex];
    if (row.RowData is DataRowView drv && e.PropertyDescriptor != null)
    {
        var col = _grid.Columns.FirstOrDefault(c =>
            string.Equals(c.ColumnName, e.PropertyDescriptor.Name, StringComparison.OrdinalIgnoreCase));
        if (col != null)
        {
            var cell = row.Cells.FirstOrDefault(c => c.ColumnIndex == col.Index);
            if (cell != null && drv.DataView?.Table?.Columns.Contains(col.ColumnName) == true)
            {
                cell.CellValue = drv.Row[col.ColumnName];
                _grid.InvalidateRow(e.NewIndex);
                return;
            }
        }
    }
    // Fallback: full rebind
    ...
}
```

---

### Phase 3 — Schema Change Detection (DataSource Replaced via BindingSource)
**File**: `GridNavigatorHelper.cs` → `BindingSource_DataSourceChanged`

- When `BindingSource.DataSource` is replaced (fires `DataSourceChanged`), check if the new data has a different schema (different column names/types).
- If schema differs: call `Data.AutoGenerateColumns()` to regenerate rather than just `Data.Bind()`.
- If schema is the same: standard `Data.Bind()` suffices.

```csharp
private void BindingSource_DataSourceChanged(object? sender, EventArgs e)
{
    if (IsUowMode) return;
    bool schemaChanged = DetectSchemaChange(_bindingSource);
    if (schemaChanged)
        _grid.Data.AutoGenerateColumns();
    else
        _grid.Data.Bind(_bindingSource);
    _grid.Data.InitializeData();
    _grid.Layout.Recalculate();
    _grid.SafeInvalidate();
}

private bool DetectSchemaChange(BindingSource bs)
{
    // Compare current column names against the new source's first-item property names or DataTable columns
    var currentNames = _grid.Columns
        .Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
        .Select(c => c.ColumnName)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
    // Detect from new bs
    var resolved = _grid.Data.ResolveForSchemaCheck(bs);
    return (resolved != null) && !currentNames.SetEquals(GetColumnNames(resolved));
}
```

---

### Phase 4 — UoW PostCommit / PostUpdate Cell Sync
**File**: `GridUnitOfWorkBinder.cs`

- `HandleUowPostCommit`: after commit, re-call `RefreshBinding()` to pick up server-generated values (PKs, timestamps).
- `HandleUowPostChange` for `PostUpdate`/`PostEdit`: call `SyncRowDataToGrid` for the changed entity rather than just `SafeInvalidate()`.

```csharp
private void HandleUowPostCommit(object sender, UnitofWorkParams e)
{
    // Full re-read to capture server-generated values
    RefreshBinding();
}

private void HandleUowPostChange(object sender, UnitofWorkParams e)
{
    if (e.EventAction == EventAction.PostUpdate || e.EventAction == EventAction.PostEdit)
    {
        // Try to find the row and sync from its data object
        if (e.DirtyColumns != null) // if index hint is available
        {
            // targeted row sync
        }
        // Fallback: full repaint
        _grid.SafeInvalidate();
        return;
    }
    RefreshBinding();
}
```

---

### Phase 5 — Deduplicate PositionChanged / CurrentChanged
**File**: `GridNavigatorHelper.cs`

- `BindingSource_CurrentChanged` and `BindingSource_PositionChanged` do identical selection sync. Remove `PositionChanged` handler body (or unify into one private method `SyncSelectionFromPosition()` called by both) to eliminate double-update per navigation.

```csharp
private void SyncSelectionFromPosition()
{
    if (_bindingSource == null) return;
    int position = _bindingSource.Position;
    if (position >= 0 && position < _grid.Rows.Count)
    {
        if (!_grid.Selection.HasSelection || _grid.Selection.RowIndex != position)
            _grid.SelectCell(position, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
    }
}

private void BindingSource_CurrentChanged(object? sender, EventArgs e)  => SyncSelectionFromPosition();
private void BindingSource_PositionChanged(object? sender, EventArgs e) => SyncSelectionFromPosition();
```

---

### Phase 6 — Dead Code / Correctness Cleanup
**File**: `GridDataHelper.cs` → `GetEffectiveEnumerableWithSchema()`

- The second `if (resolved is BindingSource bs)` check inside `GetEffectiveEnumerableWithSchema` is dead code: `ResolveDataForBinding()` already strips `BindingSource`. Remove it or add a comment.
- `ResolveDataForBinding`: When `BindingSource.List` is null AND `BindingSource.DataSource` is null, return the `BindingSource` itself so the enumerable path gets an empty enumerable, not null → prevents NullReferenceException if DataSource is subsequently set.

---

### Phase 7 — Documentation & Testing
**File**: `GridX/Docs/`

- Add `DataSource_Compatibility.md` documenting supported types, recommended patterns, and gotchas.
- Add unit-test stubs (test class names) for:
  - `DataTable_DirectMutation_TriggersGridUpdate`
  - `ObservableCollection_AddItem_TriggersGridUpdate`
  - `BindingList_AddItem_TriggersGridUpdate`
  - `BindingSource_DataSourceChanged_RegeneratesColumns`
  - `UoW_PostCommit_RefreshesServerValues`
  - `List_DirectMutation_DoesNotAutoUpdate_RequiresRefreshData`

---

## 4. Priority Order

| Priority | Phase | Impact | Effort |
|---|---|---|---|
| P0 | Phase 1 — ObservableCollection | High — silent data loss | Low |
| P0 | Phase 2 — DataTable fast path | High — perf regression on every edit | Medium |
| P1 | Phase 4 — UoW PostCommit sync | Medium — stale PK values | Low |
| P1 | Phase 5 — Deduplicate position events | Low-medium — double repaint | Low |
| P2 | Phase 3 — Schema change detection | Medium — requires API addition | Medium |
| P2 | Phase 6 — Dead code cleanup | Low — correctness/clarity | Low |
| P3 | Phase 7 — Docs & tests | High long-term | High |

---

## 5. Files to Be Modified

| File | Changes |
|---|---|
| `GridX/Helpers/GridDataHelper.cs` | Phase 1, Phase 6 |
| `GridX/Helpers/GridNavigatorHelper.cs` | Phase 2, Phase 3, Phase 5 |
| `GridX/Helpers/GridUnitOfWorkBinder.cs` | Phase 4 |
| `GridX/Docs/DataSource_Compatibility.md` | Phase 7 (new file) |
