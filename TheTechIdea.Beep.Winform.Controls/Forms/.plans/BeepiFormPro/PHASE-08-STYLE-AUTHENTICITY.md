# Phase 8 — Style Authenticity Audit & Correction

**Status:** Not started | **Priority:** HIGH | **Depends on:** Phase 4 (Painter Consolidation)

## Objective

Ensure every painter actually looks like its namesake UI style. The 33 painters must match their corresponding design systems (Material Design 3, Fluent 2, macOS, etc.) — not just be color variations of the same template.

## Problem Statement

Current painters share identical structure (background → caption → borders). Many differ only in `FormPainterMetrics` values (colors, corner radius). A "Glassmorphism" painter that doesn't blur the background isn't glassmorphism. A "Neumorphism" painter without dual shadows isn't neumorphism.

## Painter Categories

### Category A — FULLY AUTHENTIC ✅
Correctly implements the named design system with distinctive visual characteristics.

### Category B — PARTIALLY AUTHENTIC ⚠️
Has some style-specific traits but is missing key elements. Differs from the default template mainly in colors.

### Category C — COLOR SWAP ONLY ❌
Differs from the ModernFormPainter ONLY in `FormPainterMetrics` colors. No style-specific rendering at all.

## Expected Style-Specific Rendering

| Style | Distinctive Visual Traits |
|---|---|
| **Material Design 3** | Surface tint (elevation overlay), 28dp corner radius, tonal color roles, no hard shadows — uses elevation |
| **Material You** | Dynamic color from wallpaper, larger corner radius, personalized color scheme, rounded buttons |
| **Fluent 2 / Windows 11** | Acrylic/mica backdrop, reveal highlight on hover, 8dp corner radius, Segoe UI Variable, caption buttons with hover background circles |
| **macOS / Big Sur** | Traffic-light buttons (red/yellow/green circles), translucent sidebar, large corner radius, SF-style typography, window shadow |
| **iOS / iPadOS** | Large 20+dp corner radius, SF-style, translucent navigation bar, flat depth via translucency |
| **Glassmorphism** | Frosted glass effect, backdrop blur, subtle 1px white border, light source reflection, layered depth |
| **Neumorphism** | Dual shadows (dark bottom-right + light top-left), same bg color, soft extruded look, NO hard borders |
| **Brutalist** | Sharp 0px corners, thick 3-4px black borders, bold typography, high contrast, raw/unfinished aesthetic |
| **Neon / Cyberpunk** | Colored glow border (neon effect), dark background, scan lines (cyberpunk), glow around caption text |
| **Holographic** | Rainbow/prismatic gradients, glass-like transparency, light refraction effects |
| **Terminal** | Monospace font, dark background, green/amber text, scan lines, blinking cursor in caption |
| **Cartoon / ChatBubble** | Bold outlines, speech bubble shapes, playful colors, exaggerated rounded corners |
| **Retro** | Pixel-style, 8-bit aesthetic, limited color palette, sharp edges |
| **Dracula / OneDark / Nord / Tokyo** | Each should match the named editor theme palette exactly — Dracula (purple/green/pink), OneDark (Atom), Nord (arctic blue-grey), Tokyo Night |
| **Ubuntu / GNOME / KDE** | Each should match its OS — Ubuntu (orange accent, dark header), GNOME (Adwaita, flat), KDE (Breeze, blue) |
| **ArcLinux** | Flat design, blue accent, transparent panel, minimal borders |
| **Paper** | Material Design 1 paper aesthetic — cards, ink ripples, elevation shadows |

## Tasks

### SA-01: Categorize all 33 painters
Read every painter and classify as Category A (authentic), B (partial), or C (color-swap-only). Document findings.

### SA-02: Fix Category C painters
For each color-swap-only painter, add the distinctive visual traits listed above:
- Glassmorphism: add `FormPainterRenderHelper.PaintFrostedGlass` with blur
- Neumorphism: add dual-shadow rendering
- Neon/Cyberpunk: add glow effects
- Terminal: add monospace font from theme + scan lines
- Holographic: add prismatic gradient

### SA-03: Enhance Category B painters
Add missing distinctive traits to partially-authentic painters.

### SA-04: Create shared effect helpers
Add to `FormPainterRenderHelper`:
- `PaintFrostedGlass(g, rect, tint, blur)` — for glass/acrylic
- `PaintDualShadow(g, rect, darkColor, lightColor)` — for neumorphism
- `PaintNeonGlow(g, rect, color, spread)` — for neon/cyberpunk
- `PaintPrismaticGradient(g, rect)` — for holographic
- `PaintScanLines(g, rect, spacing, alpha)` — for terminal/cyberpunk

### SA-05: Verify editor theme palettes
Audit Dracula, OneDark, Nord, Tokyo, Solarized, GruvBox for correct palette colors matching the actual editor themes.

## Verification

- [ ] Each painter's visual output matches its namesake design system
- [ ] Glassmorphism has actual blur (not just transparency)
- [ ] Neumorphism has dual shadows
- [ ] macOS has traffic-light buttons
- [ ] Terminal has monospace + scan lines
- [ ] `dotnet build` — 0 errors

## Audit Results (2026-07-09)

### Category A — AUTHENTIC ✅ (18 painters)
NeoMorphism, Neon, Cyberpunk, Holographic, Dracula, OneDark, Nord, Tokyo, Ubuntu, GNOME, KDE, Glassmorphism, Brutalist, Minimal, Custom, Cartoon, ChatBubble, Retro, Solarized, GruvBox, Terminal, Paper, ArcLinux, Metro2

### Category B — PARTIALLY AUTHENTIC ⚠️ (5 painters)

| Painter | Key Issues |
|---|---|
| **MaterialFormPainter** | Corner radius 12 (should be 16-28). Black elevation overlay instead of tonal primary-color tint. No tonal color roles. 6px vertical accent bar is NOT Material pattern. Shadow doesn't match MD3 elevation levels |
| **MaterialYouFormPainter** | Same issues as Material. Animations disabled (should be enabled for Material You) |
| **FluentFormPainter** | Reveal highlight is static (should follow cursor). Acrylic is approximated by noise (GDI+ limitation). Buttons are over-designed. Shadow too subtle. No Segoe UI Variable |
| **MacOSFormPainter** | Shadow too subtle (blur 18 vs macOS ~40). Corner radius 10 (should be 12-16). Border too visible. Caption gradient is painted, not translucent |
| **iOSFormPainter** | **Traffic-light buttons are macOS, NOT iOS**. iOS uses navigation bar with back-chevron, not window buttons. No SF typography |

### Category C — COLOR SWAP ONLY ❌ (0 painters)
All painters have at least some style-specific rendering beyond color changes.

### Critical Fixes

| Priority | Painter | Fix |
|---|---|---|
| P0 | **iOSFormPainter** | Remove traffic-light buttons. Add iOS-style navigation bar with back-chevron |
| P1 | **MaterialFormPainter** | Bump corner radius to 16. Replace black overlay with primary-color tonal elevation. Remove accent bar |
| P1 | **MaterialYouFormPainter** | Same as Material. Enable animations |
| P1 | **FluentFormPainter** | Make reveal highlight cursor-following. Simplify buttons. Enlarge shadow |
| P2 | **MacOSFormPainter** | Enlarge shadow. Reduce border alpha. Bump corner radius |
| P2 | **GlassFormPainter** | Reuse cached noise from Fluent. Add tint layer |
