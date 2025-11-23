# Stepper Refactor & Enhancement Plan

Purpose
- Modernize and extend `BeepSteppperBar` and associated stepper controls (e.g., `BeepStepperBreadCrumb`) into a painter-based framework for flexible visual styles, while maintaining BaseControl’s hit management, theming, and styling.
- Break the control into partial classes separating layout, painting, hit management, interactions, and animations.
- Implement a set of painter styles mapped to the style samples (see attachments) so designers can pick or extend styles easily.

Scope & Constraints
- Use `BaseControl` hit management and `ControlHitTestHelper` helpers for hover/click and keyboard support.
- Use `StyledImagePainter` for icon/image rendering (cached, tintable, shaped).
- Avoid breaking public API or behavior; provide backward-compatible properties where suitable.
- Keep images referenced by string `ImagePath` and prefer tints over raster changes.

Deliverables
- A new `plan.md` in `Steppers/` with the design and action plan (this document).
- `IStepperPainter` contract & painter implementations for each style.
- Partial class refactor of `BeepSteppperBar` into layout, painter, hit tests, interactions, and animations.
- Sample painter implementations (style 1..12) in `Steppers/Painters/`.
- Visual QA harness or sample form rendering all styles side-by-side.

Design Inspirations & Style Mapping
- Study all images and create style categories (names are temporary):
  - **Style 1 (Circular Nodes)**: Numbered circle nodes connected with thin lines, center text under each step.
  - **Style 2 (Chevron Breadcrumb)**: Pill segments with arrow/chevrons showing progress; polygonal chevrons between steps.
  - **Style 3 (Square/Dashed Connectors)**: Square nodes with dashed connectors; minimal modern look.
  - **Style 4 (Segmented Tabs)**: Segmented rectangular tab-like steps; selected segments filled with primary color.
  - **Style 5 (Progress Bar + CTA)**: Step label centered with Prev/Next buttons and a progress line (like an action-driven stepper).
  - **Style 6 (Simple Dots)**: Small circular dots with an active dot and progress line.
  - **Style 7 (Icon Timeline)**: Horizontal timeline with icons inside shapes (circle/rounded pill) and a partially filled track to show progress.
  - **Style 8 (Vertical Timeline with Cards)**: Vertical stepper with big icons and cards on the side (useful for task timelines).
  - **Style 9 (Gradient / Material)**: Palette-driven gradient segment stepper with large, left-to-right chevrons and gradient fills.
  - **Style 10 (Badge/Status)**: Steps with badges or status indicators (counts) and small numeric badges above nodes.
  - **Style 11 (Alternating Timeline)**: Alternating node alignment (left & right) with connecting vertical line (used for timelines/events).
  - **Style 12 (Compact Inline)**: Minimal inline breadcrumbs with small separators and underlines.

Each style becomes a painter implementing `IStepperPainter`.

IStepperPainter Contract (proposed)
- In `Steppers/Painters/IStepperPainter.cs`:
  - `void Paint(Graphics g, BeepSteppperBar control, StepperPainterContext context)`
  - `void PaintBackground` and `void PaintForeground` if separated
  - `void PaintStepIcon(Graphics g, Rectangle rect, StepModel step, StepperPainterContext context)` — helper
  - `string Name { get; }`
  - `Size GetPreferredSize(Graphics g, BeepSteppperBar control, IEnumerable<StepModel> steps, Orientation orientation)` — layout hint

Painter responsibilities
- Layout details for the step node appearance (radius, capsule width, chevron size, font, spacing) — but not global layout; the `LayoutManager` computes positions using painter-provided hints.
- Paint step nodes, connectors, inner icons and text label per node.
- Provide hit region hints for each node where a click should be handled (or provide `GetStepHitPath(stepIndex)` for shape-based hit testing).

Partial Class Layout
- `BeepSteppperBar.cs` — main control properties and events:
  - Properties: `SelectedIndex`, `Items: IList<StepModel>`, `Orientation`, `PainterName`, `PainterOptions`, `ShowNextPrevButtons`, `StepSize`, `ConnectorThickness`, `StepLabelVisibility`, etc.
  - Events: `SelectionChanged`, `StepClicked`.

- `BeepSteppperBar.Layout.cs` (partial) — layout responsibilities:
  - Measure and compute bounds for the stepper area.
  - Compute node center points and rectangles based on `Size`, `ItemCount`, spacing and orientation, using a flexible `ComputeStepRectangles()`.
  - Lazily recompute layout when properties change or `InvalidateLayout()` is called.
  - Output: `List<Rectangle> StepRects` and `List<Region> StepHitRegions`.

- `BeepSteppperBar.Painters.cs` (partial) — painter wiring:
  - Load `IStepperPainter` by name (via `StepPainterManager` registry or factory) and use its `Paint` method in `OnPaint`.
  - Provide a default painter if none set.
  - Hooks to paint background and foreground decoration.

- `BeepSteppperBar.HitTest.cs` (partial) — hit management:
  - Use `BaseControl` `AddHitArea` (if present) or `ControlHitTestHelper` in `BaseControl` helpers to register clickable areas and hover areas.
  - Map `StepRects` to `HitArea` objects with metadata (i.e., StepIndex), for consistent hit testing.
  - Implement `OnMouseMove`, `OnMouseLeave`, `OnMouseClick`, `OnKeyDown` handling.

- `BeepSteppperBar.Animations.cs` (partial) — animations & transitions:
  - Provide a timer-based or value-lerp animation for transitions: sliding underline, partial progress fill, gradient tween, or node bounce.
  - Define `TransitionDuration` and default animation easing.

- `BeepSteppperBar.Interactions.cs` (partial) — selection & navigation:
  - Implement keyboard navigation: Left/Right/Up/Down to move between steps, Enter to select.
  - `Next` and `Previous` button logic and optional `DisableCompleted` flags.

- `BeepSteppperBar.DesignTime.cs` (partial) — design-time friendly preview:
  - Add `DesignMode` or sample data when in design mode.

- `BeepSteppperBar.Models.cs` (partial) — step item model definition or use existing `SimpleItem` or `StepModel` in `Models/`:
  - Fields: `string Text`, `string ImagePath`, `bool IsComplete`, `bool IsEnabled`, `string Tooltip`, `string Subtitle`, `object Tag`

Implement painters in `Painters/`
- Add `IStepperPainter` and base `StepPainterBase.cs` with helpers for text and icon painting.
- Create painter classes: `CircularNodeStepperPainter`, `ChevronBreadcrumbStepperPainter`, `SegmentedBarStepperPainter`, `ProgressBarStepperPainter`, `DotsStepperPainter`, `IconTimelinePainter`, `VerticalTimelinePainter`, `GradientMaterialStepperPainter`, `BadgeStepperPainter`, `AlternatingTimelinePainter`, `CompactBreadcrumbPainter`.
- Each painter should implement: `Paint(Graphics g, BeepSteppperBar control, StepperPainterContext context)`.
- Keep each painter compact, with a `PaintStep` and `PaintConnector` helper.

StepperPainterContext
- Provide context to painter methods including:
  - `Graphics g`, `Rectangle drawingRect`, `ITheme Theme`, `bool UseThemeColors`, `Color Foreground`, `List<Rectangle> StepRects`, `int SelectedIndex`, `int HoverIndex`, `bool IsCollapsed`.

Layout Management & Sizing
- Use `LayoutManager` approach and add a `ComputeLayout` method that uses list of step widths and total space.
- Respect `Minimum` and `Maximum` sizes and provide `AutoSpacing`.

Theme & ControlStyle Integration
- Rely on `BeepThemesManager` for fonts, primary colors, and theme-level colors. For icons, use `StyledImagePainter.PaintWithTint` when useThemeColors is true.
- Control style: `BeepControlStyle` => determine radius for node shapes via `StyleBorders.GetRadius(style)`.

Hit & Selection Management
- Use `BaseControl` or `ControlHitTestHelper` to add hit areas for each step. Hit area metadata should include step index, and `GetHitElement` should map to step clicks.
- Hover: compute hover region and call `Invalidate()` on hover changes to redraw hover states.
- Click: set `SelectedIndex` and raise `SelectionChanged` event.
- Tooltip: On hover, show `StepModel.Tooltip` via BaseControl's tooltip support.

API & Properties to Add
- `PainterName` / `IStepperPainter SelectedPainter`.
- `Orientation` (Horizontal vs Vertical)
- `ShowNumbers` / `ShowIcons` / `ShowLabels` booleans
- `ConnectorThickness`, `ConnectorStyle` (solid/dashed), `ActiveConnectorColor` and `InactiveConnectorColor`.
- `AnimationMode` (None/ Fade/ Slide/ Grow) and `AnimationDuration`.

Acceptance Criteria
- `BeepSteppperBar` splits into partials and builds correctly.
- Each painter renders properly with theme support and uses `StyledImagePainter` for icons.
- BaseControl's hit management used for mouse/keyboard interactions; hover & click sets `SelectedIndex`.
- Visual parity: the painter renderings match sample styles sufficiently with correct layout and spacing.
- Performance: painters should be efficient (no per-paint heavy allocations); if needed use `Paint` caching and `StyledImagePainter` caching to pre-render tinted images.

Testing & QA
- Unit tests for layout calculation and hit area mapping.
- Manual tests: Use a sample form to display all painters side-by-side with sample content. Test hover/click/key navigation for each painter.
- Visual regression: capture screenshots for each painter, compare with golden images if possible.

Implementation Steps (High-level)
1. Create `IStepperPainter` contract.
2. Create the stepper painter registry (resolve by `PainterName`).
3. Refactor `BeepSteppperBar` into partial files: layout, painters, interactions, animations, hit test.
4. Implement `CircularNodeStepperPainter`, `ChevronBreadcrumbStepperPainter`, `ProgressBarStepperPainter` as the top 3 priority painters.
5. Implement remaining painters as time allows.
6. Create sample form (e.g., `SteppersDemoForm`) displaying all available painters under different themes and sizes.
7. Visual QA pass and smaller design refinements.
8. Unit tests and cleanup.

Timeline & Prioritization
- Phase 1 (2 days): Design contract, partial class scaffolding, and core layout/hit management wiring.
- Phase 2 (2 days): Implement the top 3 painters (circular nodes, chevrons, progress bar), integrate themes, and add sample form.
- Phase 3 (2-3 days): Add 4-6 extra painters (icons, vertical timeline, minimal dot), animations, and QA harness.
- Phase 4 (1-2 days): Add unit tests, visual regression harness, and finalize acceptance criteria.

Implementation Notes & Tips
- When painting icons, always call `StyledImagePainter.PaintWithTint` if theme is being used and there are SVG or vector icons. Retain `ImagePainter` fallback where raster assets are known.
- Keep `Integer` rounding consistent across layout math to avoid blurry antialiasing issues.
- Use `g.SmoothingMode = SmoothingMode.HighQuality` and `TextRenderer` or `GDI+` per available helper for consistent text rendering.
- Use `BaseControl` `AddHitArea`/`RemoveHitArea` to register step hit areas; this ensures consistent focus & keyboard behavior.
- For performance, compute layout in `OnResize` & property change events only, not every `OnPaint` call.

Appendix
- Proposed Painter files under `Steppers/Painters`: 
  - `IStepperPainter.cs`
  - `StepperPainterBase.cs`
  - `CircularNodeStepperPainter.cs`
  - `ChevronBreadcrumbStepperPainter.cs`
  - `SegmentedBarStepperPainter.cs`
  - `ProgressBarStepperPainter.cs`
  - `DotsStepperPainter.cs`
  - `IconTimelinePainter.cs`
  - `VerticalTimelinePainter.cs`
  - `GradientMaterialStepperPainter.cs`
  - `BadgeStepperPainter.cs`
  - `AlternatingTimelinePainter.cs`
  - `CompactBreadcrumbPainter.cs`

- Partial class files under `Steppers/`:
  - `BeepSteppperBar.cs` (main API & properties)
  - `BeepSteppperBar.Layout.cs`
  - `BeepSteppperBar.Painters.cs`
  - `BeepSteppperBar.HitTest.cs`
  - `BeepSteppperBar.Interactions.cs`
  - `BeepSteppperBar.Animations.cs`
  - `BeepSteppperBar.DesignTime.cs`
  - `BeepSteppperBar.Models.cs` (if more properties needed)

---

If this plan looks good to you, I can start implementing the scaffold and priority painters (Style 1–3), wire up the BaseControl hit-management, and create a demo harness to test them. 

Would you like me to proceed with implementing the Painter contract and the first three painter styles (circular nodes, chevron breadcrumb, and progress bar) now? 

✅ Next step (if approved): Implement `IStepperPainter` + partial class scaffolding and a `CircularNodeStepperPainter` prototype with hit regions and basic animation.


---
*Plan generated on: November 23, 2025*