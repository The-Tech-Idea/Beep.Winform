# Background Painter Alignment Plan

## Objectives
- Mirror the ModernForm painter catalogue with equivalent background painters under `Styling/BackgroundPainters` so application-wide theming remains consistent.
- Add any missing background painter classes and update existing ones whose visuals no longer match the refreshed form documentation (`FormstylePainters.md`). 
- Register every background painter in `BackgroundPainterFactory` so they can be resolved at runtime by style name.

## Current Inventory Snapshot
- Existing painters already cover a subset of form styles: `Fluent`, `Glass`, `Minimal`, `Neon`, `Gnome`, `Kde`, `MacOS`, `Material`, `Metro`, `Neumorphism`, etc.
- Several ModernForm styles have no background counterpart (e.g. `ArcLinux`, `Brutalist`, `Cartoon`, `ChatBubble`, `Cyberpunk`, `Dracula`, `Glassmorphism`, `Holographic`, `GruvBox`, `Metro2`, `Modern`, `Nord`, `Nordic`, `OneDark`, `Paper`, `Retro`, `Solarized`, `Terminal`, `Tokyo`, `Ubuntu`). 
- Some painters exist but may require adjustments to match updated specs (`Fluent`, `Glass`, `Minimal`, `Neon`, `Gnome`, `Kde`, `Material`, `Neumorphism`, `MacOS`, `Metro`). 

## Required New Background Painters
Create painter classes following the naming convention `<StyleName>BackgroundPainter` with behaviour derived from `FormstylePainters.md`. Each should implement the shared interface (see existing painters for patterns) and supply:

| Painter | Background Expectations |
|---------|------------------------|
| `ArcLinuxBackgroundPainter` | Solid Arc palette fill + top highlight line mirroring form painter. |
| `BrutalistBackgroundPainter` | Flat solid background with optional 40px grid lines, no anti-aliasing. |
| `CartoonBackgroundPainter` | Bright fill plus halftone dot grid (8px spacing). |
| `ChatBubbleBackgroundPainter` | Soft solid base with faint diagonal stripes (24px step). |
| `CyberpunkBackgroundPainter` | Dark base + cyan scanlines and occasional glitch blocks. |
| `DraculaBackgroundPainter` | Solid base with vignette (darkened edges). |
| `GlassmorphismBackgroundPainter` | Semi-transparent fill, dotted frost overlay, top sheen. |
| `HolographicBackgroundPainter` | Iridescent gradient plus diagonal shine. |
| `GruvBoxBackgroundPainter` | Warm solid fill with horizontal grain + top glow band. |
| `Metro2BackgroundPainter` | Flat fill with diagonal accent lines every 40px. |
| `ModernBackgroundPainter` | Contemporary flat fill with subtle vertical gradient overlay. |
| `NordBackgroundPainter` | Flat nordic base with frost gradient on top third + icy top line. |
| `NordicBackgroundPainter` | Soft vertical gradient (top lightening), no additional texture. |
| `OneDarkBackgroundPainter` | Deep editor tone with 40px dotted grid to mimic code minimap. |
| `PaperBackgroundPainter` | Solid material paper tone + sparse noise speckles + top highlight. |
| `RetroBackgroundPainter` | Flat fill with scanlines every 3px and 50% hatch for CRT effect. |
| `SolarizedBackgroundPainter` | Pure solid Solarized tone (no gradients). |
| `TerminalBackgroundPainter` | Solid dark base with 2px scanlines and low-alpha green grid. |
| `TokyoBackgroundPainter` | Deep fill, cyan glow over top 100px, neon top line, scanlines. |
| `UbuntuBackgroundPainter` | Warm gradient plus 4px orange accent band on the left edge. |

> Note: If styles share logic (e.g. `Nord`/`Nordic`), encapsulate common helpers to avoid duplication but keep separate classes for discovery.

## Existing Painter Adjustments
- **Fluent / Fluent2 / Mica**: Ensure background noise + gradients align with latest acrylic definitions; reuse cached texture approach from form painter if needed.
- **Glass / GlassAcrylic**: Confirm mica gradient + noise layering order mirrors modern form behaviour.
- **Minimal / NeoBrutalist / Neon / Metro / Material / MacOS / iOS / Gnome / Kde / Neumorphism**: Review against `FormstylePainters.md` descriptions and update gradients, shadows, or patterns for parity (e.g. Minimal vertical highlight, Neon RGB glow alignment, Neumorphism inner shadow direction).
- **BackgroundPainterFactory**: Register new painter types and expose methods to resolve them by the same keys used in `BeepFormStyle` or other consumer enums.

## Implementation Steps
1. **Scaffold new classes** using patterns from existing painters (constructorless static `PaintBackground` methods plus gradients/textures). Centralize shared math/textures in `BackgroundPainterHelpers` where possible.
2. **Enhance existing painters** identified above, referencing the detailed behaviour captured in `FormstylePainters.md`.
3. **Update `BackgroundPainterFactory`** to include all new/renamed painters and ensure consistency with style enums or string keys.
4. **Add Unit/Integration Hooks (optional)**: extend any painter registries or preview tools so new backgrounds are discoverable.
5. **Documentation**: Add brief notes to `Styling/BackgroundPainters/README.md` summarizing newly supported styles.

## Deliverables Checklist
- [x] New background painter classes committed (see table above).
- [ ] Adjusted legacy background painters aligned to current specs.
- [x] Factory registrations updated and validated.
- [x] README or docs refreshed to list supported backgrounds.
- [ ] Spot-tested rendering (where feasible) to verify gradients/patterns.
