# Trees: Enhance & Optimize Plan

Goal: Improve rendering performance, reduce allocations, and stabilize layout/hit-test for `BeepTree` controls.

Scope and priorities

1. Core rendering and layout (highest priority)
 - `BeepTree.Drawing.cs` / `BeepTree.Core.cs` — ensure painting entry points are efficient and avoid allocations in paint path.
 - `BeepTreeLayoutHelper.cs`, `BeepTreeHitTestHelper.cs`, `BeepTreeHelper.cs` — optimize layout caching and hit-testing.
 - `BaseTreePainter.cs`, `BeepTreePainterFactory.cs`, `StandardTreePainter.cs` — reduce transient GDI objects and reuse pens/brushes/fonts.

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

Reply `continue` to create and validate the benchmark now.