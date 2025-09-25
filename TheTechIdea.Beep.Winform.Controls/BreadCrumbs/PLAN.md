BeepBreadcrump Refactor and Optimization Plan

Goals
- Modernize BeepBreadcrump to align with BaseControl features (Material style, drawing pipeline)
- Introduce a helper-based painter strategy per visual style
- Keep backward compatibility for public API while improving structure, readability and testability
- Small performance optimizations (text measurement caching, reduced allocations)

Key Changes
1) Painter Strategy (Helpers)
   - Create `IBreadcrumbPainter` + `BreadcrumbPainterBase` and individual painters:
     - `ClassicBreadcrumbPainter`
     - `ModernBreadcrumbPainter`
     - `PillBreadcrumbPainter`
     - `FlatBreadcrumbPainter`
     - `ChevronBreadcrumbPainter` (new visual style)
   - Each painter is responsible for:
     - Calculate item rectangle per style
     - Draw item per style
     - Draw separator per style

2) Control Integration
   - BeepBreadcrump selects painter based on `Style` enum
   - Use BaseControl drawing rect (already material-aware through ControlPaintHelper)
   - Use BeepButton/BeepLabel shared instances for drawing content to reduce allocations
   - Maintain existing properties and behavior (Items, SeparatorText, ShowIcons, ShowHomeIcon)

3) Material-aware layout
   - Rely on `DrawingRect` from BaseControl which integrates with Material content rect
   - No additional material painting logic in BeepBreadcrump; base paints background/borders

4) Optimizations
   - Text measurement cache keyed by (text, font hash)
   - Null-safety for SelectedItem setter
   - Use `TextFont` consistently for measurements

5) Non-breaking additions
   - Add `Chevron` style enum value
   - Default remains existing behavior (Modern)

6) Files to add
   - `BreadCrumbs/Helpers/IBreadcrumbPainter.cs`
   - `BreadCrumbs/Helpers/BreadcrumbPainterBase.cs`
   - `BreadCrumbs/Helpers/ClassicBreadcrumbPainter.cs`
   - `BreadCrumbs/Helpers/ModernBreadcrumbPainter.cs`
   - `BreadCrumbs/Helpers/PillBreadcrumbPainter.cs`
   - `BreadCrumbs/Helpers/FlatBreadcrumbPainter.cs`
   - `BreadCrumbs/Helpers/ChevronBreadcrumbPainter.cs`

7) Testing notes
   - Verify all styles render and hit areas work
   - Verify Material style on parent/base works (outline/filled/standard)
   - Verify Items add/remove, NavigateToIndex, selection/hover behavior
