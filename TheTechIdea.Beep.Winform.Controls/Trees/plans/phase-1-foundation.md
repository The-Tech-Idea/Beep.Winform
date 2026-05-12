# Phase 1: Foundation & Stability

## Goal
Establish a rock-solid foundation for BeepTree by fixing known issues, improving performance, and building testing infrastructure before adding major new features.

## Current State Analysis
BeepTree has a solid architecture with partial classes, helpers, painters, and BaseControl integration. However, several areas need stabilization:

- Layout cache invalidation is complex and has had struct-mutation bugs
- Hit-test registration happens frequently (though BUG-02 fixed the paint-path issue)
- No automated benchmarks exist to measure performance improvements
- Virtualization exists but needs validation with large datasets
- GDI object management was recently fixed but needs ongoing vigilance

## Tasks

### 1.1 Performance & Benchmarking Infrastructure
- [ ] Create `BeepTree.Benchmarks` project with off-screen rendering tests
- [ ] Benchmark baseline: 100 nodes, 1000 nodes, 10000 nodes
- [ ] Measure: paint time, layout calc time, memory allocations, GC pressure
- [ ] Add benchmark for painter switching and theme changes
- [ ] Integrate benchmark runner into CI (if CI exists)

### 1.2 Layout System Hardening
- [ ] Add unit tests for `BeepTreeLayoutHelper`:
  - [ ] Coordinate transformation (content <-> viewport)
  - [ ] Virtualization range calculation
  - [ ] Text measurement caching
  - [ ] Indent calculation at various DPI scales
- [ ] Add unit tests for `BeepTreeHitTestHelper`:
  - [ ] Hit area registration accuracy
  - [ ] Hit testing at viewport edges
  - [ ] Performance: register 1000 nodes < 16ms
- [ ] Validate struct mutation safety in `RecalculateLayoutCache()`
- [ ] Add defensive checks for null/empty collections in layout paths

### 1.3 Scrollbar & Viewport Stability
- [ ] Fix remaining scrollbar flicker during rapid resize
- [ ] Ensure scrollbar visibility toggles don't cause layout oscillation
- [ ] Add smooth scroll option (pixel-based vs row-based)
- [ ] Test horizontal scrollbar with deeply nested nodes (level 10+)
- [ ] Validate mouse wheel delta handling at different DPI scales

### 1.4 Memory & GDI Object Audit
- [ ] Audit all painters for cached GDI object disposal:
  - [ ] Fonts (bold/regular) in Material3, Fluent2, iOS15 painters
  - [ ] Pens/brushes from `PaintersFactory` (must not dispose cached instances)
  - [ ] Path objects in rounded-rectangle painters
- [ ] Add `Dispose()` pattern to painters if needed
- [ ] Profile memory with dotMemory or similar for 10k node tree

### 1.5 Keyboard Navigation Completeness
- [ ] Add Ctrl+Home / Ctrl+End support
- [ ] Add Ctrl+Click multi-select (currently only plain click toggles)
- [ ] Add Shift+Click range selection
- [ ] Add type-ahead search (press letter key to jump to matching node)
- [ ] Ensure all keyboard events fire correct `BeepMouseEventArgs`

### 1.6 Event System Robustness
- [ ] Audit all 25+ events for correct firing:
  - [ ] `NodeBeforeExpand` / `NodeBeforeCollapse` cancellation works
  - [ ] `NodeSelected` fires exactly once per selection change
  - [ ] `NodeChecked` / `NodeUnchecked` fire for programmatic changes too
  - [ ] Context menu events provide correct node reference
- [ ] Add event sequence validation tests

## Success Criteria
- All benchmarks run successfully and establish baselines
- Zero known layout/hit-test bugs
- Memory usage stable over 1 hour with 10k nodes
- 60 FPS paint performance on standard hardware
- All keyboard shortcuts work as documented

## Dependencies
- None (this is foundational)

## Estimated Effort
3-4 days

## Research References
- DevExpress TreeList: Performance regardless of dataset size
- Telerik RadTreeView: Kinetic scrolling, smooth scrolling modes
- FastTree (GitHub): Virtual mode reliability
