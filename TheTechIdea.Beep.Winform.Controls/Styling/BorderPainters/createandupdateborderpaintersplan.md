# Border Painter Alignment Plan

## Objectives
- Provide border painters for every ModernForm style so outlines, accent bars, and focus indicators match the form documentation in `FormstylePainters.md`.
- Add missing painter classes to `Styling/BorderPainters` (one class per style) and ensure the runtime can resolve them via the same `BeepControlStyle` keys used by background painters.
- Review legacy border painters for parity with modern specs (accent bar thickness, glow strength, etc.).

## Current Snapshot
- Existing painters focus on design systems such as Material, Fluent, Minimal, Web frameworks, etc.
- No dedicated border painters exist for ModernForm-only styles: `ArcLinux`, `Brutalist`, `Cartoon`, `ChatBubble`, `Cyberpunk`, `Dracula`, `Glassmorphism`, `Holographic`, `GruvBox`, `Metro2`, `Modern`, `Nord`, `Nordic`, `OneDark`, `Paper`, `Retro`, `Solarized`, `Terminal`, `Tokyo`, `Ubuntu`.
- Several painters (e.g., `NeonBorderPainter`, `MinimalBorderPainter`, `GlassAcrylicBorderPainter`) likely need tweaks to stay in sync with the updated ModernForm guidance.

## Required New Border Painters
Create one class per style implementing `IBorderPainter` (see existing files for the pattern). Key behaviours from `FormstylePainters.md`:

| Painter | Border Behaviour |
|---------|------------------|
| `ArcLinuxBorderPainter` | Thin 1px border with slight alpha reduction matching Arc Linux flat design. |
| `BrutalistBorderPainter` | Thick outer rectangle (5px) plus inner accent, no anti-aliasing. |
| `CartoonBorderPainter` | Rounded rectangle with comic outline, uses `metrics.BorderWidth`. |
| `ChatBubbleBorderPainter` | Rounded bubble outline following speech bubble shape. |
| `CyberpunkBorderPainter` | Multi-layer neon glow (outer soft glows + solid core). |
| `DraculaBorderPainter` | Purple glow passes plus thin main stroke. |
| `GlassmorphismBorderPainter` | Subtle double stroke (translucent white overlay). |
| `HolographicBorderPainter` | 2px rainbow gradient path. |
| `GruvBoxBorderPainter` | Warm-tinted 2px path around rounded rectangle. |
| `Metro2BorderPainter` | Flat 1px stroke respecting Metro geometry. |
| `ModernBorderPainter` | Clean 1px anti-aliased stroke. |
| `NordBorderPainter` | 1px cool-toned line matching frost palette. |
| `NordicBorderPainter` | 1px clean outline with soft colour. |
| `OneDarkBorderPainter` | Low-alpha 1px stroke in editor palette. |
| `PaperBorderPainter` | Low-alpha 1px path for paper edge. |
| `RetroBorderPainter` | Multi-line bevel (light top/left, dark bottom/right) plus inner rectangle. |
| `SolarizedBorderPainter` | Simple 1px stroke in Solarized base tone. |
| `TerminalBorderPainter` | 2px square outline with ASCII-style corners. |
| `TokyoBorderPainter` | 2px cyan border for neon frame. |
| `UbuntuBorderPainter` | 1px orange-tinted stroke. |

## Existing Painter Adjustments
- Validate accent bar/focus behaviour for styles that now share names (e.g., `Neon`, `Minimal`, `GlassAcrylic`, `NeoBrutalist`, `Metro`).
- Confirm `IBorderPainter` consumers (factories or style switch statements) are updated to pick new painters based on the extended `BeepControlStyle`.

## Implementation Steps
1. **Scaffold new classes** following existing layout (static helper with `Paint` method).
2. **Introduce or update factories** to include the new painters (create a factory if one does not exist yet).
3. **Tune legacy painters** for parity (optional follow-up).
4. **Refresh documentation** (this plan and `BorderPainters/README.md`).

## Checklist
- [x] New ModernForm border painters created.
- [x] Factory/registration updated.
- [x] Documentation refreshed.
- [ ] Visual behaviour spot-checked.
