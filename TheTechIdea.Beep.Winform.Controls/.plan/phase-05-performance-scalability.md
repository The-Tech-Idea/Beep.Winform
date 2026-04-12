# Phase 5: Performance, Virtualization & Scalability

> **Sprint:** 27 · **Priority:** High · **Complexity:** Medium
> **Dependency:** Phase 3 complete · **Estimated Effort:** 2 weeks

---

## Objective

Optimize the document host for large-scale usage: 60 FPS rendering with 500+ tabs, sub-500ms layout restore for 100+ documents, virtual tab rendering, lazy content loading, and comprehensive performance profiling.

---

## Current Limitations

| Limitation | Current State | Target State |
|------------|--------------|--------------|
| Tab rendering | All tabs painted | Only visible tabs painted |
| Content loading | Eager (all panels created) | Lazy (content on activation) |
| Layout calculation | Full recalculation | Dirty rect tracking |
| Thumbnail snapshots | Lazy deferred | Cached with LRU eviction |
| Layout restore | Blocking | Async non-blocking |
| Batch operations | Basic | Optimized single-pass |
| Memory usage | Unbounded | Bounded with eviction |
| Performance monitoring | None | Built-in profiler |

---

## Tasks

### Task 5.1: Implement Virtual Tab Rendering

**Requirements:**

- Only paint tabs visible in the current viewport
- Calculate visible tab range based on scroll offset and strip width
- Skip painting for tabs outside viewport
- Support smooth scrolling with virtual rendering
- Handle partial visibility (paint partially visible tabs)
- Maintain correct hit-testing for virtual tabs

**Implementation:**

```csharp
private void CalculateVisibleTabRange()
{
    int firstVisible = 0;
    int lastVisible = _tabs.Count - 1;

    for (int i = 0; i < _tabs.Count; i++)
    {
        if (_tabs[i].TabRect.Right > -ScrollOffset)
        {
            firstVisible = i;
            break;
        }
    }

    for (int i = _tabs.Count - 1; i >= 0; i--)
    {
        if (_tabs[i].TabRect.Left < Width - ScrollOffset)
        {
            lastVisible = i;
            break;
        }
    }

    _visibleTabRange = (firstVisible, lastVisible);
}
```

### Task 5.2: Implement Lazy Content Loading

**Requirements:**

- Defer panel content creation until tab is activated
- Show loading placeholder for inactive tabs
- Support content preloading (preload next tab in MRU)
- Support content unloading (unload tabs not accessed recently)
- Configurable preload/unload thresholds
- Content state persistence (save/restore content state)

**Implementation:**

```csharp
public class LazyDocumentPanel : BeepDocumentPanel
{
    private Func<Control> _contentFactory;
    private Control? _loadedContent;
    private bool _isLoaded;

    public void SetContentFactory(Func<Control> factory)
    {
        _contentFactory = factory;
    }

    public void EnsureContentLoaded()
    {
        if (!_isLoaded && _contentFactory != null)
        {
            _loadedContent = _contentFactory();
            Controls.Add(_loadedContent);
            _isLoaded = true;
        }
    }

    public void UnloadContent()
    {
        if (_isLoaded && _loadedContent != null)
        {
            Controls.Remove(_loadedContent);
            _loadedContent.Dispose();
            _loadedContent = null;
            _isLoaded = false;
        }
    }
}
```

### Task 5.3: Optimize Layout Calculation

**Requirements:**

- Dirty rect tracking (only recalculate changed areas)
- Cache layout results (invalidate on resize/theme/DPI change)
- Batch layout calculations (coalesce multiple requests)
- Layout calculation on background thread (marshal results to UI thread)
- Profile layout calculation time

**Implementation:**

```csharp
private void InvalidateLayout(Rectangle? dirtyRect = null)
{
    if (dirtyRect.HasValue)
    {
        _dirtyRects.Add(dirtyRect.Value);
    }
    else
    {
        _dirtyRects.Clear();
        _dirtyRects.Add(ClientRectangle);
    }

    if (!_layoutPending)
    {
        _layoutPending = true;
        BeginInvoke(new Action(FlushLayout));
    }
}
```

### Task 5.4: Enhance Thumbnail Snapshot Caching

**Requirements:**

- LRU cache for thumbnail snapshots
- Configurable cache size (default: 20 thumbnails)
- Evict least recently used thumbnails when cache full
- Async snapshot capture (non-blocking)
- Snapshot invalidation (re-capture on content change)
- Compressed snapshot storage (reduce memory)

### Task 5.5: Implement Async Layout Restore

**Requirements:**

- Non-blocking layout restore
- Show progress indicator during restore
- Restore documents in batches
- Allow cancellation during restore
- Report restore progress (X of Y documents)
- Marshal results back to UI thread

**Implementation:**

```csharp
public async Task<LayoutRestoreReport> RestoreLayoutAsync(string json, CancellationToken ct = default)
{
    var report = new LayoutRestoreReport();
    var tree = await Task.Run(() => DeserializeLayoutTree(json), ct);

    var documents = tree.GetAllDocuments();
    report.TotalDocuments = documents.Count;

    foreach (var doc in documents)
    {
        ct.ThrowIfCancellationRequested();
        await RestoreDocumentAsync(doc);
        report.RestoredDocuments++;
        OnRestoreProgressChanged(report);
    }

    return report;
}
```

### Task 5.6: Optimize Batch Operations

**Requirements:**

- `BeginBatchAddDocuments()` — suspend layout, events, painting
- `EndBatchAddDocuments()` — single layout pass, fire events, repaint
- Batch remove documents
- Batch move documents between groups
- Batch pin/unpin documents
- Progress reporting for large batches
- Cancellation support for large batches

### Task 5.7: Implement Memory Management

**Requirements:**

- Bounded closed-tab history (evict oldest when limit reached)
- Bounded thumbnail cache (LRU eviction)
- Bounded layout history (undo/redo stack limit)
- Memory usage monitoring
- Memory pressure notification (respond to low memory)
- Dispose unused resources promptly

### Task 5.8: Implement Performance Profiler

**Files to Create:**

```
Performance/
├── DocumentHostProfiler.cs          ← Built-in performance profiler
├── PerformanceMetrics.cs            ← Metrics collection
└── PerformanceReport.cs             ← Report generation
```

**Metrics to Track:**

- Tab paint time (ms)
- Layout calculation time (ms)
- Layout restore time (ms)
- Document add time (ms)
- Document close time (ms)
- Memory usage (MB)
- Tab count
- Group count
- Split depth
- FPS during animation

**Profiler Usage:**

```csharp
host.Profiler.IsEnabled = true;
host.Profiler.StartCapture();
// ... perform operations ...
var report = host.Profiler.StopCapture();
Console.WriteLine(report);
```

---

## Acceptance Criteria

- [ ] Virtual tab rendering (only visible tabs painted)
- [ ] Lazy content loading with factory pattern
- [ ] Dirty rect layout tracking
- [ ] LRU thumbnail cache
- [ ] Async layout restore with progress
- [ ] Batch add/remove documents (single layout pass)
- [ ] Bounded memory usage (configurable limits)
- [ ] Built-in performance profiler
- [ ] 60 FPS with 500+ tabs
- [ ] <500ms layout restore for 100 documents
- [ ] <100ms document add
- [ ] <50ms tab switch
- [ ] Memory usage <100MB for 100 documents
- [ ] Progress reporting for large operations
- [ ] Cancellation support for long operations

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Virtual rendering breaks hit-testing | Maintain full hit-test geometry regardless of visibility |
| Lazy loading causes flicker | Show loading placeholder during content load |
| Async restore creates inconsistent state | Transactional restore (all or nothing) |
| Memory bounds too aggressive | Configurable limits with sensible defaults |
| Profiler overhead affects performance | Profiler disabled by default; minimal overhead when enabled |

---

## Files Modified

| File | Change Type |
|------|-------------|
| `BeepDocumentTabStrip.Painting.cs` | Optimize (virtual rendering) |
| `BeepDocumentPanel.cs` | Enhance (lazy loading) |
| `BeepDocumentHost.Layout.cs` | Optimize (dirty rect tracking) |
| `BeepDocumentHost.Preview.cs` | Enhance (LRU cache) |
| `BeepDocumentHost.Serialisation.cs` | Enhance (async restore) |
| `BeepDocumentHost.Documents.cs` | Optimize (batch operations) |

## Files Created

| File | Purpose |
|------|---------|
| `Performance/DocumentHostProfiler.cs` | Performance profiler |
| `Performance/PerformanceMetrics.cs` | Metrics collection |
| `Performance/PerformanceReport.cs` | Report generation |
