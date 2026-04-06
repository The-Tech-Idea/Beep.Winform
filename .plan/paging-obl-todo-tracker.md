# Paging & OBL Integration — Todo Tracker

**Plan source:** paging-obl-integration-analysis.md  
**Phase label:** Phase 7.5 — Paging/OBL Integration Fixes  
**Priority order (paging):** GAP-1 → GAP-2 → GAP-3 → GAP-4 → GAP-5 → GAP-8 → GAP-9 → GAP-10 → GAP-7 → GAP-6  
**Priority order (master-detail):** TMD1 → TMD2 → TMD3 → TMD4 → TMD5 → TMD6 (build)  
**Note:** MD tasks are independent of paging tasks — either stream can be worked first.

---

## Task Groups

### T1 — OBL: Add `OriginalCount` Property (targets GAP-3 prerequisite)

**File:** `DataManagementModelsStandard/ObservableBindingList/ObservableBindingList.cs`

- [ ] T1.1 — Add `public int OriginalCount => originalList.Count;` after the `TotalPages` property (~line 63)
- [ ] T1.2 — Add `public int VisibleCount => Items.Count;` (clarifies paging-vs-full distinction)

---

### T2 — UoW: Fix `TotalItemCount` (targets GAP-3)

**File:** `DataManagementEngineStandard/Editor/UOW/UnitofWork.Core.cs`

- [ ] T2.1 — Change `TotalItemCount` getter from `=> Units.Count` to:
  ```csharp
  => Units?.OriginalCount ?? 0;
  ```
  This returns the full source count regardless of paging state.

---

### T3 — FormsManager: Wire `SetBlockPageSize` to OBL and UoW (targets GAP-1, GAP-4, GAP-10)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.Performance.cs`

- [ ] T3.1 — In `SetBlockPageSize`, after setting `block.Configuration.PageSize`:
  ```csharp
  if (block.UnitOfWork != null)
  {
      block.UnitOfWork.PageSize = pageSize;
      if (pageSize > 0)
          block.UnitOfWork.Units?.SetPageSize(pageSize);   // GAP-10 guard
  }
  ```

---

### T4 — FormsManager: Rewrite `LoadPageAsync` (targets GAP-2, GAP-6, GAP-8)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.Performance.cs`

- [ ] T4.1 — Replace the `NavigateToRecordAsync(blockName, targetIndex)` call with OBL paging APIs:
  ```csharp
  var obl = block.UnitOfWork.Units;
  if (obl != null)
  {
      // Ensure OBL PageSize is in sync (GAP-1 follow-up)
      if (obl.PageSize != pageInfo.PageSize)
          obl.SetPageSize(pageInfo.PageSize);

      if (obl.IsVirtualMode)
          await obl.GoToPageAsync(pageInfo.PageNumber).ConfigureAwait(false);
      else
          obl.GoToPage(pageInfo.PageNumber);
      
      // Cursor always resets to first record of the new page (GAP-6 fix)
      obl.MoveTo(0);
  }
  ```
- [ ] T4.2 — Remove the old `int skip`, `int total`, `int targetIndex`, `NavigateToRecordAsync` block.
- [ ] T4.3 — After page load, if `FetchAheadDepth > 0` call pre-fetch (GAP-8):
  ```csharp
  if (block.Configuration.FetchAheadDepth > 0 && obl.IsVirtualMode)
      _ = obl.PrefetchAdjacentPagesAsync();   // fire-and-forget
  ```
- [ ] T4.4 — After `obl.MoveTo(0)`, synchronize detail blocks (keep existing `SynchronizeDetailBlocksAsync` call):
  ```csharp
  await _relationshipManager.SynchronizeDetailBlocksAsync(blockName).ConfigureAwait(false);
  ```

---

### T5 — FormsManager: Wire LazyLoad to OBL DataProvider (targets GAP-5)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.Performance.cs`

- [ ] T5.1 — Add private helper `RegisterLazyLoadProvider(DataBlockInfo block)`:
  ```csharp
  private void RegisterLazyLoadProvider(DataBlockInfo block)
  {
      var obl = block.UnitOfWork?.Units;
      if (obl == null) return;

      obl.SetDataProvider(async (pageIdx, pageSize) =>
      {
          var ds = block.UnitOfWork?.DataSource;
          var entity = block.UnitOfWork?.EntityName;
          if (ds == null || string.IsNullOrEmpty(entity)) return new List<dynamic>();

          // Use DataSource to fetch one page
          // (Specific query API depends on IDataSource implementation)
          int skip = pageIdx * pageSize;
          var filter = $"__skip={skip}&__take={pageSize}";
          var result = ds.GetEntity(entity, filter);
          return result as List<dynamic> ?? new List<dynamic>();
      });

      obl.SetTotalItemCount((int)(block.UnitOfWork?.TotalItemCount ?? 0));
  }
  ```
  > **Note:** The exact DataSource paging API needs to be confirmed against `IDataSource`
  > (the `__skip/__take` filter syntax is a placeholder — see T5.3 below).

- [ ] T5.2 — In `SetLazyLoadMode`, when `mode != LazyLoadMode.None`, call `RegisterLazyLoadProvider(block)`. When `mode == LazyLoadMode.None`, call `block.UnitOfWork?.Units?.ClearDataProvider()`.
- [ ] T5.3 — Research `IDataSource` paging API: find how skip/take is passed to `GetEntity`/`RunQuery` — update the provider lambda.

---

### T6 — FormsManager: Invalidate OBL Page Cache Too (targets GAP-9)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.Performance.cs`

- [ ] T6.1 — In `InvalidateBlockCache(blockName)`, after `_performanceManager.InvalidateBlockCache(blockName)`:
  ```csharp
  var block = GetBlock(blockName);
  block?.UnitOfWork?.Units?.InvalidatePageCache();
  ```

---

### TMD0 — Context: Why & Architecture

Master-detail was removed from OBL (`ObservableBindingList.MasterDetail.cs` deleted).
The correct architecture is:

1. **UoW level** — `UnitofWork<T>` does nothing new to the interface; `OBL.CurrentChanged` is already
   public on `ObservableBindingList<T>`. FM subscribes to it directly via `unitOfWork.Units.CurrentChanged`
   (same pattern as existing `ItemChanged`). No `IUnitofWork<T>` / `IUnitofWork` interface change needed.
2. **FormsManager level** — subscription fires FM's `SynchronizeDetailBlocksAsync` (which triggers
   ON-POPULATE-DETAILS). A suppress counter prevents double-fire when nav code already calls sync explicitly.

**Priority order:** TMD1 → TMD2 → TMD3 → TMD4 → TMD5 → TMD6 (build)

---

### TMD1 — FM: Add Fields to `FormsManager.cs` (targets MD-GAP-2, MD-GAP-4)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.cs` — `#region Fields` block

- [ ] TMD1.1 — After `private readonly ConcurrentDictionary<string, EventHandler<ItemChangedEventArgs<Entity>>> _itemChangedHandlers = new();`, add:
  ```csharp
  // Master-detail: current-record change hook (Phase 7.6)
  private readonly ConcurrentDictionary<string, EventHandler> _mdCurrentChangedHandlers = new(StringComparer.OrdinalIgnoreCase);
  private readonly ConcurrentDictionary<string, int> _syncSuppressCount = new(StringComparer.OrdinalIgnoreCase);
  ```

---

### TMD2 — FM: Subscribe in `RegisterBlock` (targets MD-GAP-2)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.cs`

- [x] TMD2.1 — After `_itemChangedHandlers[blockName] = handler;` inside `RegisterBlock`, add:
  ```csharp
  // MD hook: on current-record change, sync detail blocks (if not suppressed)
  EventHandler mdHandler = async (s, e) =>
  {
      if (!IsSyncSuppressed(blockName) && _relationships.ContainsKey(blockName))
          await SynchronizeDetailBlocksAsync(blockName);
  };
  unitOfWork.Units.CurrentChanged += mdHandler;
  _mdCurrentChangedHandlers[blockName] = mdHandler;
  ```

---

### TMD3 — FM: Unsubscribe in `UnregisterBlock` (targets MD-GAP-5)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.cs`

- [x] TMD3.1 — After `_itemChangedHandlers.TryRemove(blockName, out _);` in `UnregisterBlock`, add:
  ```csharp
  if (_mdCurrentChangedHandlers.TryRemove(blockName, out var mdH) && blockInfo.UnitOfWork?.Units != null)
      blockInfo.UnitOfWork.Units.CurrentChanged -= mdH;
  _syncSuppressCount.TryRemove(blockName, out _);
  ```

---

### TMD4 — FM: Add Suppress Helpers + Suppress CRUD/Nav (targets MD-GAP-4)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.cs` — near `LogOperation`/`LogError`

- [x] TMD4.1 — Add three private helpers (inside the `#region` that closes the class):
  ```csharp
  private void SuppressSync(string blockName) =>
      _syncSuppressCount.AddOrUpdate(blockName, 1, (_, v) => v + 1);
  private void ResumeSync(string blockName) =>
      _syncSuppressCount.AddOrUpdate(blockName, 0, (_, v) => Math.Max(0, v - 1));
  private bool IsSyncSuppressed(string blockName) =>
      _syncSuppressCount.TryGetValue(blockName, out var cnt) && cnt > 0;
  ```
- [x] TMD4.2 — In `DeleteCurrentRecordAsync`, wrap `deleteMethod.Invoke(...)` with suppress:
  ```csharp
  SuppressSync(blockName);
  try { var task = ...; var result = await task; ... } finally { ResumeSync(blockName); }
  ```
- [x] TMD4.3 — In `FormsManager.EnhancedOperations.cs` `InsertRecordEnhancedAsync`, wrap `insertMethod.Invoke(...)` with suppress (same pattern).

---

### TMD5 — FM Navigation: Fix Direct RelationshipManager Calls + Suppress (targets MD-GAP-3, MD-GAP-4)

**File:** `DataManagementEngineStandard/Editor/Forms/FormsManager.Navigation.cs`

- [x] TMD5.1 — `NavigateToRecordAsync`: Replace `await _relationshipManager.SynchronizeDetailBlocksAsync(blockName)` → `await SynchronizeDetailBlocksAsync(blockName)` (FM version — fires ON-POPULATE-DETAILS trigger).
- [x] TMD5.2 — `NavigateToRecordAsync`: Wrap `PerformRecordNavigation(...)` + explicit sync with suppress:
  ```csharp
  SuppressSync(blockName);
  try
  {
      var success = PerformRecordNavigation(blockInfo, recordIndex);
      if (success)
      {
          ...history push...
          await SynchronizeDetailBlocksAsync(blockName);
          OnCurrentChanged?.Invoke(this, currentChangedArgs);
          ...
      }
      return success;
  }
  finally { ResumeSync(blockName); }
  ```
- [x] TMD5.3 — `NavigateAsync`: Replace `await _relationshipManager.SynchronizeDetailBlocksAsync(blockName)` → `await SynchronizeDetailBlocksAsync(blockName)`.
- [x] TMD5.4 — `NavigateAsync`: Wrap `PerformNavigation(...)` + explicit sync with suppress (same pattern as TMD5.2).

---

### T7 — OBL: Add `AllDirtyItems` Cross-Page Property (targets GAP-7, optional)

**File:** `DataManagementModelsStandard/ObservableBindingList/ObservableBindingList.Tracking.cs`

- [ ] T7.1 — Add after `DirtyItems`:
  ```csharp
  /// <summary>All modified/added items across ALL pages (including off-page tracked items).</summary>
  public IReadOnlyList<T> AllDirtyItems
  {
      get
      {
          // Items is only the current page; originalList has everything
          var dirty = new List<T>();
          foreach (var item in originalList)
          {
              var tr = GetTrackingItem(item);
              if (tr != null && (tr.EntityState == EntityState.Modified || tr.EntityState == EntityState.Added))
                  dirty.Add(item);
          }
          return dirty;
      }
  }
  ```

---

### T8 — Build & Verify (all changes)

- [ ] T8.1 — `dotnet build DataManagementEngine.csproj` — zero errors
- [ ] T8.2 — `dotnet build DataManagementModelsStandard` — zero errors
- [ ] T8.3 — Smoke check: write a regression note confirming `TotalItemCount` returns source count in paged mode

---

## Progress

### Paging Stream
**T1 — OBL `OriginalCount`:** 0 / 2  
**T2 — UoW `TotalItemCount`:** 0 / 1  
**T3 — `SetBlockPageSize` wire-up:** 0 / 1  
**T4 — `LoadPageAsync` rewrite:** 0 / 4  
**T5 — LazyLoad DataProvider:** 0 / 3  
**T6 — Invalidate OBL cache:** 0 / 1  
**T7 — `AllDirtyItems` (optional):** 0 / 1  
**T8 — Build:** 0 / 3  

**Paging total: 0 / 16 tasks**

### Master-Detail Stream
**TMD1 — FM fields:** 1 / 1 ✅  
**TMD2 — RegisterBlock subscribe:** 1 / 1 ✅  
**TMD3 — UnregisterBlock cleanup:** 1 / 1 ✅  
**TMD4 — Suppress helpers + CRUD:** 3 / 3 ✅  
**TMD5 — Navigation fix + suppress:** 4 / 4 ✅  

**Master-Detail total: 10 / 10 tasks ✅**

**Grand total: 0 / 26 tasks**
