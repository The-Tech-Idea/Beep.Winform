# Paging & OBL Integration — Analysis & Fix Plan

**Date:** 2026-04-04  
**Scope:** FormsManager.Performance.cs ↔ ObservableBindingList ↔ UnitofWork  
**Status:** Phase 7 build passes but 10 integration gaps identified — no runtime-correctness without fixes

---

## Executive Summary

Phase 7 added a standalone `PagingManager`, `LazyLoadMode`, and cache wrappers.
However, none of them are wired to the **existing** paging engine inside `ObservableBindingList<T>` (OBL) or to `UnitofWork<T>`. The result is three independent "PageSize" values and two separate page-state machines that never agree with each other. Navigation after `LoadPageAsync` silently fails when OBL is in paged mode.

---

## Component Map

```
FormsManager
  ├── _pagingManager  (PagingManager.cs — Phase 7 addition)
  │     └── ConcurrentDictionary<blockName, BlockPageState { PageSize, CurrentPage, TotalRecords, FetchAheadDepth }>
  │
  ├── _performanceManager  (PerformanceManager.cs — query-result cache)
  │     └── _blockCache: CachedBlockInfo — FormsManager-level, NOT OBL page cache
  │
  └── DataBlockInfo.UnitOfWork  (UnitofWork<T>)
        ├── PageSize   { get; set; } = 10        ← plain property, not wired to OBL
        ├── TotalItemCount => Units.Count         ← DYNAMIC — reflects current OBL view size
        └── Units  (ObservableBindingList<T>)
              ├── PageSize   { private set; } = 20  ← set only via SetPageSize()
              ├── CurrentPage, TotalPages           ← OBL's own page state
              ├── IsPaged  (_isPagingActive)
              ├── GoToPage(int)       ← client-side slice from originalList
              ├── GoToPageAsync(int)  ← server-side via _dataProvider callback
              ├── SetDataProvider(Func<int,int,Task<List<T>>>)  ← virtual loading
              ├── PrefetchAdjacentPagesAsync()
              ├── InvalidatePageCache()
              └── _trackingsByGuid   ← change tracking — survives page switches ✅
```

---

## Gap Analysis — 10 Issues

### GAP-1 — Three Disconnected PageSize Values (CRITICAL)

| Location | Property | Default | Set By |
|---|---|---|---|
| `_pagingManager` state | `BlockPageState.PageSize` | 50 (default in PagingManager) | `SetBlockPageSize()` |
| `UnitofWork<T>` | `PageSize` | 10 | nobody wires it |
| `ObservableBindingList<T>` | `PageSize` | 20 | only via `OBL.SetPageSize()` |

`FormsManager.SetBlockPageSize(blockName, 50)` updates only the first row.  
The OBL uses 20, the UoW uses 10.  
**Any call to `OBL.GoToPage(n)` slices with the wrong page size.**

---

### GAP-2 — `LoadPageAsync` Navigates Wrong Index (CRITICAL — Silent Failure)

```csharp
// Current code in FormsManager.Performance.cs
var pageInfo = _pagingManager.SetCurrentPage(blockName, pageNumber);
int skip = pageInfo.Skip;                          // e.g., page 3 × size 50 → skip = 100
int total = block.UnitOfWork.TotalItemCount;       // == Units.Count
int targetIndex = Math.Min(skip, Math.Max(0, total - 1));
await NavigateToRecordAsync(blockName, targetIndex);
```

**Scenario A — OBL has been paged via `ApplyPaging()`:**  
`Units.Count = 50` (current page only). `targetIndex = Math.Min(100, 49) = 49`.  
`NavigateToRecordAsync` sets `OBL.CurrentIndex = 49` → last record of the *current (wrong)* page.  
The page number never actually changes.

**Scenario B — OBL has all records in memory:**  
`Units.Count = 10 000`. `targetIndex = 100`. `OBL.MoveTo(100)` succeeds.  
But `OBL.GoToPage` was never called → `OBL.CurrentPage` stays 1 → `OBL.IsPaged` = false.  
Result: cursor is at absolute row 100, not at the start of a logical page.

**Root cause:** `LoadPageAsync` never calls `OBL.GoToPage(pageNumber)` or `OBL.GoToPageAsync(pageNumber)`.

---

### GAP-3 — `TotalItemCount` Reports Page Size, Not Source Count (HIGH)

```csharp
public int TotalItemCount => Units.Count;   // UnitofWork.Core.cs line 170
```

After `OBL.ApplyPaging()`, `Units.Count` = current page item count (min(PageSize, remaining)).  
`FormsManager.GetTotalRecordCount` falls back to `UoW.TotalItemCount` → returns e.g. `50`, not `10 000`.  
`PageInfo.TotalPages` = `ceil(50/50) = 1` → navigation reports only 1 page ever exists.

**Good news:** `UoW.GetIsDirty()` uses `Units.HasChanges` → `_trackingsByGuid.Values.Any(...)` — 
this counts ALL tracked items across all pages, not just visible ones. Dirty detection is correct. ✅

---

### GAP-4 — `SetBlockPageSize` Doesn't Wire OBL or UoW (HIGH)

```csharp
// Current — FormsManager.Performance.cs
public void SetBlockPageSize(string blockName, int pageSize)
{
    _pagingManager.SetPageSize(blockName, pageSize);       // ✅ updates FM state
    block.Configuration.PageSize = pageSize;               // ✅ updates config
    // MISSING: block.UnitOfWork.PageSize = pageSize;      // ❌
    // MISSING: block.UnitOfWork.Units.SetPageSize(pageSize); // ❌
}
```

---

### GAP-5 — `LazyLoadMode.OnDemand/Deferred` Not Connected to OBL DataProvider (MEDIUM)

OBL has a complete virtual loading system:
```csharp
OBL.SetDataProvider(Func<int, int, Task<List<T>>> provider);
OBL.GoToPageAsync(int pageNumber);   // fetches via provider, caches in _pageCache
OBL.PrefetchAdjacentPagesAsync();    // pre-warms adjacent pages
```

`FormsManager.SetLazyLoadMode(blockName, LazyLoadMode.OnDemand)` only sets flags.  
It never registers an OBL data provider.  
`LazyLoadMode.OnDemand` is effectively a no-op.

---

### GAP-6 — Navigation History Records Wrong Page Index (MEDIUM)

In `NavigateToRecordAsync`, before moving:
```csharp
var prevIndex = blockInfo.UnitOfWork?.Units?.CurrentIndex ?? 0;
if (prevIndex != recordIndex)
    _navHistoryManager.Push(blockName, prevIndex);
```

In paged scenario A (OBL has 50 items, skip=100 clamped to 49):  
History records `prevIndex` relative to the *current page slice*.  
Back-navigation pops that page-relative index and calls `NavigateToRecordAsync(blockName, 19)`,  
which navigates to absolute row 19 — on the wrong page.

---

### GAP-7 — `OBL.DirtyItems` Misses Off-Page Modified Records (LOW)

```csharp
// OBL.Tracking.cs
public List<T> DirtyItems
{
    get
    {
        foreach (var item in Items)   // ← only current page! 
        ```
}
```

`DirtyCount`, `AddedCount`, `ModifiedCount` use `_trackingsByGuid.Values` → ALL pages ✅  
`DirtyItems` property iterates `Items` → current page only ❌  
Low priority since FormsManager dirty detection uses `IsDirty/HasChanges`, not `DirtyItems`.

---

### GAP-8 — `FetchAheadDepth` Config Not Used (MEDIUM)

`SetFetchAheadDepth(blockName, 2)` updates `_pagingManager` + `block.Configuration.FetchAheadDepth`.  
Neither `LoadPageAsync` nor any other FM code reads `FetchAheadDepth` to call  
`OBL.PrefetchAdjacentPagesAsync()`.

---

### GAP-9 — `InvalidateBlockCache` Misses OBL Page Cache (LOW)

`FormsManager.InvalidateBlockCache(blockName)` removes from `PerformanceManager._blockCache`  
(the FormsManager query-result cache).  
It does NOT call `block.UnitOfWork.Units.InvalidatePageCache()` which clears OBL's  
virtual-mode `_pageCache` (the `Dictionary<int, List<T>>` inside OBL).  
If the data source is refreshed, OBL's page cache can serve stale data.

---

### GAP-10 — `SetBlockPageSize` Throws When PageSize ≤ 0 in OBL (LOW)

`OBL.SetPageSize(0)` throws `ArgumentException("Page size must be greater than zero.")`.  
`BlockConfiguration.PageSize = 0` means "paging disabled" in our model.  
When `SetBlockPageSize(blockName, 0)` is called to disable paging, the OBL call will throw.  
Need to guard: only call `OBL.SetPageSize` when `pageSize > 0`.

---

## Fix Requirements

### R1: Single-Source-of-Truth PageSize (targets GAP-1, GAP-4, GAP-10)
`SetBlockPageSize` must propagate to:
- `_pagingManager.SetPageSize` ✅
- `block.Configuration.PageSize` ✅
- `block.UnitOfWork.PageSize` (UoW's own property) ❌ → add
- `block.UnitOfWork.Units.SetPageSize(pageSize)` if `pageSize > 0` ❌ → add (with guard)

### R2: `LoadPageAsync` Must Use OBL Paging API (targets GAP-2, GAP-6)
Rewrite `LoadPageAsync`:
1. Sync page size to OBL (R1).
2. If `OBL.IsVirtualMode` → call `await OBL.GoToPageAsync(pageNumber)` then navigate to index 0.
3. Else (client-side) → call `OBL.GoToPage(pageNumber)` then navigate to index 0.
4. Do NOT pass `pageInfo.Skip` as a `NavigateToRecordAsync` index — page navigation resets cursor to 0.

### R3: `TotalItemCount` in Paged Mode (targets GAP-3)
Add `TotalSourceCount` to `ObservableBindingList<T>` that returns `originalList.Count`  
(ignoring paging). Wire `UoW.TotalItemCount` to use this in paged mode.  
Or: simpler, add `int OriginalCount => originalList.Count` public property to OBL,  
and update `UoW.TotalItemCount` to `=> Units?.OriginalCount ?? 0` instead of `Units.Count`.

### R4: Wire LazyLoad to OBL DataProvider (targets GAP-5) 
When `SetLazyLoadMode(blockName, LazyLoadMode.OnDemand)`:
- Register `OBL.SetDataProvider(async (pageIdx, pageSize) => { /* fetch from DataSource */ })`.
- The provider must use `block.UnitOfWork.DataSource` + `block.UnitOfWork.EntityName` to fetch.
When mode is `None`: call `OBL.ClearDataProvider()`.

### R5: FetchAhead Wiring (targets GAP-8)
In `LoadPageAsync`, after loading the page, if `block.Configuration.FetchAheadDepth > 0`:
```csharp
await block.UnitOfWork.Units.PrefetchAdjacentPagesAsync();
```

### R6: Invalidate Both Caches (targets GAP-9)
`FormsManager.InvalidateBlockCache(blockName)` must also call:
```csharp
block.UnitOfWork.Units.InvalidatePageCache();
```

### R7: Fix `DirtyItems` in OBL (targets GAP-7) — optional but clean
Add `AllDirtyItems` property iterating `originalList` (or reading from `_trackingsByGuid`).

---

## Files Affected

| File | Change |
|---|---|
| `ObservableBindingList.cs` | Add `public int OriginalCount => originalList.Count` |
| `UnitofWork.Core.cs` | `TotalItemCount => Units?.OriginalCount ?? Units?.Count ?? 0` |
| `FormsManager.Performance.cs` | Rewrite `SetBlockPageSize`, `LoadPageAsync`, `InvalidateBlockCache`, add `SetBlockLazyDataProvider` helper |
| `FormsManager.Performance.cs` | `SetFetchAheadDepth` wires to `PrefetchAdjacentPagesAsync` after load |
| `ObservableBindingList.Tracking.cs` | Add `AllDirtyItems` (optional) |

---

## Risk Assessment

| Gap | Severity | Breaks existing tests? |
|---|---|---|
| GAP-1/4 | Critical — wrong page size | ⚠ Yes if paging was tested |
| GAP-2 | Critical — navigation silently fails | ⚠ Yes |
| GAP-3 | High — wrong total count | ⚠ Yes |
| GAP-5 | Medium — lazy load is a no-op | No (feature was never wired) |
| GAP-6 | Medium — bad nav history | Only if nav-history tests exist |
| GAP-7 | Low — `DirtyItems` incomplete | No (not used in guards) |
| GAP-8 | Medium — fetch-ahead is no-op | No |
| GAP-9 | Low — stale OBL cache after invalidate | No |
| GAP-10 | Low — throw on disable-paging | Only if SetBlockPageSize(0) called |

---

## Phase 7.6 — Master-Detail at UoW + FormsManager Level

### Background

The OBL-level master-detail system (`ObservableBindingList.MasterDetail.cs`) was removed.
Master-detail must now be owned at two correct levels:

1. **UnitofWork level** — exposes a `CurrentChanged` event that bubbles from `OBL.CurrentChanged`
   via `AttachHandlers`/`DetachHandlers`. This decouples the "cursor moved" signal from FM.
2. **FormsManager level** — subscribes to `UoW.Units.CurrentChanged` in `RegisterBlock`; when fired
   and the block has registered relationships and sync is not suppressed, calls FM's own
   `SynchronizeDetailBlocksAsync(blockName)` (which correctly fires the ON-POPULATE-DETAILS trigger).

Suppress mechanism prevents double-fire: FM suppresses the CurrentChanged hook **before** every
cursor move that is followed by an explicit `SynchronizeDetailBlocksAsync` call.

---

### MD-GAP Inventory

| ID | Description | Severity |
|---|---|---|
| MD-GAP-1 | `UoW.Units.CurrentChanged` is not bubbled to any consumer — cursor-move is a silent event | Critical |
| MD-GAP-2 | `FormsManager.RegisterBlock` only subscribes to `ItemChanged`; no hook on current-record change | Critical |
| MD-GAP-3 | `FormsManager.Navigation.cs` calls `_relationshipManager.SynchronizeDetailBlocksAsync` directly (bypasses ON-POPULATE-DETAILS trigger) | High |
| MD-GAP-4 | No suppress mechanism → cursor-move fires `CurrentChanged` hook AND explicit `SynchronizeDetailBlocksAsync` call both run → double sync | High |
| MD-GAP-5 | `UnregisterBlock` does not clean up `CurrentChanged` handler or suppress state | Medium |

---

### MD Fix Requirements

#### MD-R1: UoW `CurrentChanged` Event (targets MD-GAP-1)
In `UnitofWork.Core.cs`:
- Add `private void OnUnitsCurrentChanged(object sender, EventArgs e) => _units?.CurrentChanged ...(see R1-note)` 
  — actually: subscribe/unsubscribe in `AttachHandlers`/`DetachHandlers` so that anytime `Units` is
  replaced via `SetUnits`, the wire follows.
- **No interface change required** — FM subscribes directly to `unitOfWork.Units.CurrentChanged`
  (same pattern as existing `unitOfWork.Units.ItemChanged`). This avoids the `IUnitofWork<T>` vs
  `IUnitofWork` (non-generic) interface hierarchy mismatch (confirmed: `UnitofWork<T>` only
  implements `IUnitofWork<T>`; `BlockFactory` hands back an `IUnitofWork` via dynamic cast).

#### MD-R2: FM `RegisterBlock` Subscription (targets MD-GAP-2)
In `FormsManager.cs` `RegisterBlock`, after `_itemChangedHandlers[blockName] = handler;`:
```csharp
EventHandler mdHandler = async (s, e) =>
{
    if (!IsSyncSuppressed(blockName) && _relationships.ContainsKey(blockName))
        await SynchronizeDetailBlocksAsync(blockName);
};
unitOfWork.Units.CurrentChanged += mdHandler;
_mdCurrentChangedHandlers[blockName] = mdHandler;
```
Add `_mdCurrentChangedHandlers` and `_syncSuppressCount` fields to `FormsManager.cs`.

#### MD-R3: Fix Direct RelationshipManager Call (targets MD-GAP-3)
In `FormsManager.Navigation.cs`:
- `NavigateToRecordAsync` line ~101: replace `await _relationshipManager.SynchronizeDetailBlocksAsync(blockName)` → `await SynchronizeDetailBlocksAsync(blockName)`
- `NavigateAsync` line ~399: same replacement

#### MD-R4: Suppress Mechanism (targets MD-GAP-4)
Add helpers to `FormsManager.cs`:
```csharp
private void SuppressSync(string blockName) => _syncSuppressCount.AddOrUpdate(blockName, 1, (_, v) => v + 1);
private void ResumeSync(string blockName)   => _syncSuppressCount.AddOrUpdate(blockName, 0, (_, v) => Math.Max(0, v - 1));
private bool IsSyncSuppressed(string blockName) => _syncSuppressCount.TryGetValue(blockName, out var c) && c > 0;
```
Wrap cursor-move + explicit sync calls with `SuppressSync` / `try { ... } finally { ResumeSync }` in:
- `NavigateToRecordAsync`
- `NavigateAsync`
- `InsertRecordEnhancedAsync`
- `DeleteCurrentRecordAsync`

#### MD-R5: UnregisterBlock Cleanup (targets MD-GAP-5)
In `UnregisterBlock`, after `_itemChangedHandlers.TryRemove(blockName, out _)`:
```csharp
if (_mdCurrentChangedHandlers.TryRemove(blockName, out var mdH) && blockInfo.UnitOfWork?.Units != null)
    blockInfo.UnitOfWork.Units.CurrentChanged -= mdH;
_syncSuppressCount.TryRemove(blockName, out _);
```

---

### MD Files Affected

| File | Change |
|---|---|
| `FormsManager.cs` | Add `_mdCurrentChangedHandlers`, `_syncSuppressCount` fields; update `RegisterBlock`, `UnregisterBlock`; add `SuppressSync`/`ResumeSync`/`IsSyncSuppressed` helpers |
| `FormsManager.Navigation.cs` | Fix 2× direct `_relationshipManager.SynchronizeDetailBlocksAsync` → FM version; add suppress in `NavigateToRecordAsync` + `NavigateAsync` |
| `FormsManager.EnhancedOperations.cs` | Add suppress in `InsertRecordEnhancedAsync` |
| `FormsManager.cs` (DeleteCurrentRecordAsync) | Add suppress around `deleteMethod.Invoke` |

---
