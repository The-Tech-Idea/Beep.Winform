# BeepiFormPro Modernization Plan

Goal: Transform `BeepiFormPro` into a modern, stylable window form using multi-painters and BeepStyling, with partial classes and helper managers (layout, hit-test, interaction) similar to BeepTree.

## Objectives
- Use BeepStyling + BeepControlStyle for background/border/caption rendering.
- Support multiple painters (strategy pattern) to swap visual styles at runtime.
- Partial class split: Core, Drawing, Painters, Layout, HitTesting, Interaction, Models.
- LayoutManager: compute caption, footer, sidebars, content, and custom regions.
- HitAreaManager: register/resolve interactive areas and custom regions.
- InteractionManager: hover/press/select handling for regions and caption areas.
- Extensible Region API: add text/drawings anywhere on caption/bottom/side.

## Architecture
- Class: `BeepiFormPro` (partial) with properties:
  - `BeepControlStyle ControlStyle { get; set; }`
  - `IList<IFormPainter> Painters` + `IFormPainter ActivePainter`
  - Managers: `_layout`, `_hits`, `_interact`
  - Regions: `_regions` list of `FormRegion`
- Managers:
  - `BeepiFormProLayoutManager`
  - `BeepiFormProHitAreaManager`
  - `BeepiFormProInteractionManager`
- Painters (initial):
  - `MinimalFormPainter`
  - `MaterialFormPainter`
- Models:
  - `FormRegion` (id, rect, dock, drawing delegate)
  - `HitArea` (name, rect, type, data)
  - `RegionDock` enum (Caption, Bottom, Left, Right, ContentOverlay)

## Milestones
1) Scaffolding: partials, models, managers, painter interfaces, basic render loop
2) Implement Minimal painter (bg + border + caption strip)
3) Implement Material painter variant
4) Region API + sample text region rendering
5) Hit testing + interaction (hover/press/click) over regions and caption
6) Polish: DPI scaling, theme reactivity, window drag via caption

## Change Log
- [Step 1] Scaffolding plan and file structure (this doc)
- [Step 2] Created partial class structure: BeepiFormPro.cs (main), Core, Drawing, Managers, Painters, Models
- [Step 3] Implemented BeepiFormProLayoutManager with caption/content/bottom/left/right rect calculation
- [Step 4] Implemented BeepiFormProHitAreaManager with registration and hit-testing
- [Step 5] Implemented BeepiFormProInteractionManager with hover/press/click routing
- [Step 6] Created IFormPainter interface with PaintBackground/PaintCaption/PaintBorders
- [Step 7] Implemented MinimalFormPainter (underline caption style)
- [Step 8] Implemented MaterialFormPainter (left accent bar style)
- [Step 9] Created FormRegion model with RegionDock enum and OnPaint delegate
- [Step 10] Implemented paint pipeline in Drawing.cs with layout→hit registration→painting
- [Step 11] Added mouse event routing: OnMouseMove/Down/Up → InteractionManager
- [Step 12] Added FormStyle enum with 6 variants: Modern, Classic, Minimal, MacOS, Fluent, Material
- [Step 13] Implemented built-in caption regions: icon, title, minimize, maximize, close buttons
- [Step 14] Added system button layout in LayoutManager: IconRect, TitleRect, MinimizeButtonRect, MaximizeButtonRect, CloseButtonRect
- [Step 15] Wired system button rendering in Drawing.cs for Modern/Minimal/Material styles
- [Step 16] Implemented OnRegionClicked override with minimize/maximize/close actions
- [Step 17] Added window dragging via caption P/Invoke (ReleaseCapture, SendMessage)
- [Step 18] Added ApplyFormStyle method to set FormBorderStyle based on FormStyle
- [Step 19] Added DPI scaling support: UpdateDpiScale(), ScaleDpi() helper, _dpiScale field
- [Step 20] Updated LayoutManager.Calculate() to use DPI-scaled constants (32px button width, 8px margins, 24px icon)
- [Step 21] Added custom action button region (_customActionButton) with gear icon (⚙) and hover/press effects
- [Step 22] Added CustomActionButtonRect to LayoutManager, positioned between title and system buttons
- [Step 23] Added RegionHover and RegionClick events for extensibility without subclassing
- [Step 24] Created RegionEventArgs class with RegionName, Region, and Bounds properties
- [Step 25] Updated InteractionManager to raise RegionHover event on hover state changes
- [Step 26] Updated OnRegionClicked to raise RegionClick event and handle custom action button
- [Step 27] Added OnCustomActionClicked virtual method with default MessageBox implementation
- [Step 28] Implemented GlassFormPainter with gradient glass effect, semi-transparent background, and dual borders
- [Step 29] Updated Drawing.cs to register and render custom action button region
- [Step 30] Updated constructor to call UpdateDpiScale() during initialization
