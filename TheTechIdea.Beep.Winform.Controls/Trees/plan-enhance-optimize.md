# Trees: Enhance & Optimize Plan

Goal: Improve rendering performance, reduce allocations, and stabilize layout/hit-test for `BeepTree` controls.

Scope and priorities

1. Core rendering and layout (highest priority)
 - `BeepTree.Drawing.cs` / `BeepTree.Core.cs` � ensure painting entry points are efficient and avoid allocations in paint path.
 - `BeepTreeLayoutHelper.cs`, `BeepTreeHitTestHelper.cs`, `BeepTreeHelper.cs` � optimize layout caching and hit-testing.
 - `BaseTreePainter.cs`, `BeepTreePainterFactory.cs`, `StandardTreePainter.cs` � reduce transient GDI objects and reuse pens/brushes/fonts.

2. Hot-path painters
 - `Material3TreePainter.cs`, `Fluent2TreePainter.cs`, `iOS15TreePainter.cs`, `MacOSBigSurTreePainter.cs`, `TailwindCardTreePainter.cs`, `StripeDashboardTreePainter.cs`

3. Secondary painters and features
 - Misc painters (FileBrowser, FileManager, Document, Portfolio, Infrastructure, ActivityLog, Component)
 - Image rendering caching (StyledImagePainter) and icon sizes

Benchmarks and profiling

- Create a benchmark to exercise tree painting (baseline): will call the main paint method with a populated visible node list and record CPU time.
- Repeat benchmark after each file optimization and record the delta.

Workflow

1. Create benchmark in `BenchmarkSuite1` that renders a BeepTree to an off-screen bitmap. Validate benchmark compiles.
2. Run the benchmark to capture baseline.
3. Pick the highest-priority file, apply micro-optimizations (reuse brushes/pens, avoid MeasureString in paint, cache fonts), then run build and benchmark.
4. Record results in this plan document with before/after numbers and notes.
5. Repeat per-file until core list is complete.

Per-file task template (used in this document)
- File: <path>
- Goal: (what to optimize)
- Changes: (summary of edits)
- Build: PASS/FAIL
- Benchmark: (baseline -> after)
- Notes: (observations)

Current status
- Plan created: YES
- Benchmark: PENDING creation and validation
- Next: Create benchmark and run build/validation

---

## Phase 0: Critical GDI Corruption Fixes — COMPLETED ✅

**Problem discovered (session 2025-02):** All tree painters were setting `pen.StartCap`, `pen.EndCap`, and `pen.LineJoin` directly on pens returned from `PaintersFactory.GetPen()`, which returns **shared cached** instances. Mutating these objects corrupted rendering globally after the first paint. Several painters also wrapped `PaintersFactory.GetPen/.GetSolidBrush` in `using(...)` blocks, which **disposed the cached object** and broke all subsequent paint calls.

**Root cause:** Same class of bug as the ComboBox BUG-01 fixed in the prior session.

**Fix approach:**
1. Added three protected helpers to `BaseTreePainter`:
   - `DrawChevron(g, rect, color, width, isExpanded)` — draws V-chevron toggle via cloned pen
   - `DrawPlusMinus(g, rect, color, width, isExpanded)` — draws +/- toggle via cloned pen
   - `DrawCheckmark(g, rect, color, width)` — draws tick via cloned pen
   All helpers call `PaintersFactory.GetPen()` then `(Pen)basePen.Clone()` + `using` so the clone is disposed, not the cache.

2. Fixed all pen corruption in every affected painter (see table below).

**Files fixed and changes made:**

| File | Changes |
|---|---|
| `BaseTreePainter.cs` | Added `DrawChevron`, `DrawPlusMinus`, `DrawCheckmark` helpers |
| `Material3TreePainter.cs` | PaintNode toggle → `DrawChevron`; checkmark → `DrawCheckmark`; PaintToggle → `DrawChevron`; PaintDefaultMaterialIcon `LineJoin` → inline clone |
| `Fluent2TreePainter.cs` | PaintNode toggle `using`+caps → `DrawChevron`; checkmark → `DrawCheckmark`; PaintToggle `using`+caps → `DrawChevron`; PaintDefaultFluentIcon `LineJoin using` → inline clone |
| `VercelCleanTreePainter.cs` | PaintNode toggle → `DrawPlusMinus`; checkmark → `DrawCheckmark`; PaintToggle → `DrawPlusMinus` |
| `TailwindCardTreePainter.cs` | PaintNode toggle → `DrawChevron`; checkmark → `DrawCheckmark`; PaintNodeBackground remove `using` on `GetSolidBrush`/`GetPen`; PaintToggle → `DrawChevron` |
| `SyncfusionTreePainter.cs` | PaintNode toggle → `DrawChevron`; checkmark → `DrawCheckmark`; PaintToggle → `DrawChevron` |
| `StripeDashboardTreePainter.cs` | PaintNode toggle → `DrawChevron`; checkmark → `DrawCheckmark`; icon pen → inline clone; badge `using textBrush` → direct var (×2) |
| `iOS15TreePainter.cs` | PaintNode toggle → `DrawChevron`; checkmark (incl. LineJoin) → `DrawCheckmark`; PaintNodeBackground remove `using GetSolidBrush`; PaintToggle → `DrawChevron` |
| `MacOSBigSurTreePainter.cs` | PaintNode checkmark → `DrawCheckmark` |
| `InfrastructureTreePainter.cs` | PaintNode toggle → `DrawChevron` |
| `FileManagerTreePainter.cs` | PaintNode toggle → `DrawChevron`; checkmark → `DrawCheckmark`; PaintNodeBackground remove `using GetPen`; PaintToggle → `DrawChevron` |
| `AntDesignTreePainter.cs` | PaintNode toggle → `DrawChevron`; PaintNode checkmark → inline clone; PaintToggle → `DrawChevron`; PaintCheckbox checkmark → inline clone |
| `ChakraUITreePainter.cs` | PaintNode toggle → `DrawChevron`; checkmark → `DrawCheckmark`; PaintToggle → `DrawChevron` |
| `DiscordTreePainter.cs` | PaintNode toggle → `DrawChevron`; PaintToggle → `DrawChevron` |
| `DocumentTreePainter.cs` | PaintNode toggle → `DrawChevron`; PaintToggle → `DrawChevron` |
| `BootstrapTreePainter.cs` | PaintNode checkmark → `DrawCheckmark` |
| `FigmaCardTreePainter.cs` | Drag handle pen → inline clone; inner `using GetPen` → direct var |
| `FileBrowserTreePainter.cs` | Badge `using GetSolidBrush` → direct var |

**Build result:** ✅ PASS (zero errors, warnings are pre-existing CS0108)

---

Reply `continue` to create and validate the benchmark now.

---

## Phase 1: Core Bug Fixes — COMPLETED ✅

Eight bugs and one standard violation fixed (BUG-01 through BUG-07 + STD-07):

| ID | File | Fix |
|---|---|---|
| BUG-01 | `BeepTree.Layout.cs` | `UpdateScrollBars()` now calls `UpdateDrawingRect()` first so viewport is valid before scroll math |
| BUG-02 | `BeepTree.Drawing.cs` | Removed `RegisterHitAreas()` from inside `DrawContent()` — was O(n) rebuild every frame |
| BUG-03 | `BeepTree.Events.cs` | Removed duplicate `NodeSelected?.Invoke()` inside the else branch of the left-click handler |
| BUG-04 | `BeepTree.Layout.cs` / helpers | Toggle-rect guard added for leaf nodes in `BeepTreeLayoutHelper` |
| BUG-05 | `BeepTree.Events.cs` | Added `OnKeyDown` override: Arrow Up/Down navigation, Left/Right expand/collapse, Enter = select, Space = check |
| BUG-06 | `BeepTree.Core.cs` | All `GetScaled*()` helpers now call `ScaleValue(N)` instead of returning literals |
| BUG-07 | `BeepTree.Drawing.cs` / `BaseTreePainter.cs` | Uncommented `base.ApplyTheme()`; dead code in `GetCurrentPainter()` removed; `BaseTreePainter.Scale()` → `DpiScalingHelper.ScaleValue()` |
| STD-07 | multiple | All `Debug.WriteLine` calls guarded with `#if DEBUG` |

**Build result:** ✅ PASS (zero errors)

---

## Phase 2: Layout Merge & Deduplication — COMPLETED ✅

| Item | File | Change |
|---|---|---|
| Layout merge | `BeepTreeLayoutHelper.cs` | Added `SyncFromVisibleNodes(IReadOnlyList<NodeInfo>)` — O(n) struct-copy from tree's pre-built `_visibleNodes`; eliminates the second full traversal that `RecalculateLayout()` was doing |
| RebuildVisible uses sync | `BeepTree.Layout.cs` | `RebuildVisible()` now calls `_layoutHelper.SyncFromVisibleNodes(_visibleNodes)` instead of `InvalidateCache()` + `RecalculateLayout()` |
| Resize timer | `BeepTree.Core.cs` | `_resizeTimer.Tick` uses `SyncFromVisibleNodes` |
| ToViewport dedup | `BeepTree.Events.cs` | Removed inline `ToViewport` lambda in `LocalHitTest`; all 3 call sites use `_layoutHelper.TransformToViewport()` |
| Font fix | `BaseTreePainter.cs` | `PaintNode()` uses `BeepThemesManager.ToFontForControl()` for correct DPI behaviour |
| Debug guards | `BeepTree.Core.cs` | Remaining `Debug.WriteLine` calls in `HandleCreated` / `VisibleChanged` guarded |

**Build result:** ✅ PASS (zero errors)

---

## Phase 3: Figma + UX Enhancements — COMPLETED ✅

| Item | Files | Change |
|---|---|---|
| FIG-03: Empty state | `Core.cs` (field), `Properties.cs` (`EmptyStateText`), `Drawing.cs` | When `_nodes.Count == 0`, `DrawContent()` renders `EmptyStateText` centred in the drawing area using theme font/colour; returns early |
| FIG-04: Focus ring | `Drawing.cs` | After `painter.Paint()`, if focused and a node is selected, draws a 1.5 px dotted accent-colour rect around the selected row (clipped to drawing area) |
| COM-01: Cancellable events | `Core.cs` (`BeepTreeNodeCancelEventArgs`, `NodeBeforeExpand`, `NodeBeforeCollapse`), `Events.cs` | Toggle handler fires before-event; `Cancel == true` skips the expand/collapse |
| COM-04: Search / filter | `Core.cs` (field), `Properties.cs` (`FilterText`), `Methods.cs` (`ApplyFilter`, `ClearFilter`, `SearchNodes`) | Setting `FilterText` walks tree, builds ancestor set, rebuilds `_visibleNodes`, syncs geometry caches |

**Build result:** ✅ PASS (zero errors)

---

## Phase 4: Benchmark — NEXT

**Goal:** Baseline CPU time for a full tree paint pass so each subsequent optimization can be measured.

**Plan:**
- Create `BenchmarkSuite1` that renders a `BeepTree` (100+ nodes, mixed expanded/collapsed) to an off-screen `Bitmap`.
- Record baseline; re-run after each painter optimization.
- Target files: `StandardTreePainter`, `AntDesignTreePainter`, `Material3TreePainter`, `Fluent2TreePainter`.

Reply `continue` to create the benchmark project and run the baseline.
