# Shadow Painter Alignment Plan

## Objectives
- Provide shadow painters for every ModernForm style so elevation/glow matches `FormstylePainters.md`.
- Add missing shadow painter classes to `Styling/ShadowPainters` (one class per style) and ensure lookup code supports the extended `BeepControlStyle` values.
- Review current shadow painters (e.g., `NeonShadowPainter`, `MinimalShadowPainter`) for consistency with updated specs.

## Current Snapshot
- Shadow painters exist for Material, Fluent, Minimal, NeoBrutalist, Neon, etc., but none target the ModernForm-only styles listed below.
- Missing styles: `ArcLinux`, `Brutalist`, `Cartoon`, `ChatBubble`, `Cyberpunk`, `Dracula`, `Glassmorphism`, `Holographic`, `GruvBox`, `Metro2`, `Modern`, `Nord`, `Nordic`, `OneDark`, `Paper`, `Retro`, `Solarized`, `Terminal`, `Tokyo`, `Ubuntu`.

## Required New Shadow Painters
Each painter should expose a `Paint` method matching the existing interface (typically `(Graphics g, Rectangle bounds, GraphicsPath path, ...)`). Key requirements derived from `FormstylePainters.md`:

| Painter | Shadow Behaviour |
|---------|------------------|
| `ArcLinuxShadowPainter` | Minimal outer shadow (blur ~6, offset 3). |
| `BrutalistShadowPainter` | Hard-edged shadow (blur 0, offset 6/6). |
| `CartoonShadowPainter` | Soft drop shadow (blur 15, offset 8). |
| `ChatBubbleShadowPainter` | Soft coloured offset (blur 12, offset 6). |
| `CyberpunkShadowPainter` | Neon-coloured glow shadow (blur 25, offset 8). |
| `DraculaShadowPainter` | Purple-tinted shadow (blur 14, offset 6). |
| `GlassmorphismShadowPainter` | Light airy shadow (blur 15, offset 6). |
| `HolographicShadowPainter` | Prismatic tint shadow (blur 16, offset 8). |
| `GruvBoxShadowPainter` | Warm-toned mid-strength shadow (blur 8, offset 4). |
| `Metro2ShadowPainter` | Tight soft shadow (blur 10, offset 3). |
| `ModernShadowPainter` | Medium blur modern elevation (blur 10, offset 4). |
| `NordShadowPainter` | Soft cool shadow (blur 6, offset 3). |
| `NordicShadowPainter` | Light soft shadow (blur 10, offset 4). |
| `OneDarkShadowPainter` | Modest neutral shadow (blur 10, offset 4). |
| `PaperShadowPainter` | Material card shadow (blur 12, offset 6). |
| `RetroShadowPainter` | Tight offset (blur 3, offset 4). |
| `SolarizedShadowPainter` | Very subtle shadow (blur 7, offset 3). |
| `TerminalShadowPainter` | No shadow (transparent). |
| `TokyoShadowPainter` | Cyan-tinted glow (blur 14, offset 7). |
| `UbuntuShadowPainter` | Warm shadow (blur 12, offset 5). |

## Implementation Steps
1. **Scaffold new classes** (one per style).
2. **Update any shadow factory or resolver** to map `BeepControlStyle` to new painters; add one if missing.
3. **Adjust existing painters** if they must reflect new specs (optional follow-up).
4. **Update documentation** (this plan and `ShadowPainters/README.md`).

## Checklist
- [x] New ModernForm shadow painters created.
- [x] Factory/registration updated.
- [x] Documentation refreshed.
- [ ] Visual behaviour spot-checked.
