# Formstyle Painters Reference

## Overview
- This reference consolidates background and border behaviour across the ModernForm painters in `TheTechIdea.Beep.Winform.Controls/Forms/ModernForm/Painters`.
- Findings combine source review with notes from `Readme.md`, `skinplan.md`, `IMPLEMENTATION_SUMMARY.md`, `fixcaptioniconsplan.md`, and `VISUAL_REFERENCE.md`.
- Every painter implements `IFormPainter`, placing base fills in `PaintBackground`, border work in `PaintBorders`, and orchestration (shadows, clipping, captions) in `PaintWithEffects` as highlighted in `Readme.md` and `skinplan.md`.

## Documentation Highlights
- **Readme.md:** Catalogues the painter set and defines the paint pipeline (background → caption → borders → effects) along with enum mappings.
- **skinplan.md:** Details the required painting order, corner-radius handling, and emphasises that `PaintWithEffects` applies shadows before delegating to background and border routines.
- **IMPLEMENTATION_SUMMARY.md**, **fixcaptioniconsplan.md**, **VISUAL_REFERENCE.md:** Focus on caption icon cohesion; while they mostly address button glyphs, they explain the design language that the background and border treatments reinforce.

## Shared Rendering Notes
- Most painters rely on `owner.BorderShape` so borders respect per-form corner radii; background fills frequently use the same path when gradients must stay inside the shape.
- Theme shadows are produced in `PaintWithEffects` via painter-specific `ShadowEffect` values; the shadow characteristics are listed per painter below.
- `FormPainterRenderHelper.cs` centralises hover outlines and button drawing helpers without altering background or border surfaces.

## Painter Background and Border Profiles

### ArcLinuxFormPainter.cs
- **Background:** Solid fill using `metrics.BackgroundColor` with a translucent 1px white line at the top edge to hint at elevation.
- **Borders:** Draws `owner.BorderShape` with a 1px pen whose alpha is reduced from `metrics.BorderColor`, keeping the outline subtle and anti-aliased.
- **Shadow:** Returns a shallow outer shadow (blur 6, offset 3) to maintain the flat Arc Linux presentation.
- **Notes:** Relies on minimal decoration; no gradients or textures, so the top highlight and thin border carry the separation.

### BrutalistFormPainter.cs
- **Background:** Flat solid fill with anti-aliasing turned off, optionally overlaying vertical grid lines every 40px via a faint `metrics.BorderColor` pen.
- **Borders:** Uses a 5px rectangular outline plus an inner 2px accent rectangle, all rendered without smoothing to preserve the geometric slab.
- **Shadow:** Emits a hard edge shadow (blur 0, offsets 6/6) consistent with the heavy industrial feel.
- **Notes:** Caption and border styling deliver the 3D look; gradients and soft effects are deliberately absent.

### CartoonFormPainter.cs
- **Background:** Fills the client area with `metrics.BackgroundColor` and scatters 1px halftone dots on an 8px grid for comic-book texture.
- **Borders:** Draws the rounded `owner.BorderShape` with `metrics.BorderColor` and configured width, using anti-aliasing.
- **Shadow:** Adds a soft outer shadow (blur 15, offset 8) to float the form above the playful background.
- **Notes:** The background-effects hook intentionally no-ops; the halftone dots provide the needed noise.

### ChatBubbleFormPainter.cs
- **Background:** Solid base colour followed by faint diagonal stripes drawn every 24px with a low-alpha pen to resemble messenger card patterns.
- **Borders:** Renders the rounded border shape with `metrics.BorderColor` and width, anti-aliased for smooth bubble edges.
- **Shadow:** Employs a coloured drop shadow (blur 12, offset 6) matching the speech-bubble aesthetic.
- **Notes:** No additional background effects beyond the stripe overlay; speech bubble geometry is handled in `PaintCaption`.

### CustomFormPainter.cs
- **Background:** Provides a reusable gradient-or-solid fill that honours the current `CornerRadius`, controlled by `DefaultUseGradients`.
- **Borders:** Draws either a rounded path or rectangle depending on the corner settings, using `metrics.BorderColor` and `metrics.BorderWidth`.
- **Shadow:** Supplies a configurable shadow (blur 10 by default) that can be overridden or disabled by inheritors.
- **Notes:** Serves as the extensible base; derived painters reuse its order of background → borders → caption inside `PaintWithEffects`.

### CyberpunkFormPainter.cs
- **Background:** Dark solid fill clipped to the rounded path, layered with cyan scanlines every 4px and occasional glitch rectangles for digital noise.
- **Borders:** Builds a neon silhouette with three expanding glow pens (30/15/8px) before a 2px solid core path.
- **Shadow:** Creates a coloured glow shadow (blur 25, offset 8) to amplify the neon aura.
- **Notes:** Glitches are randomized per render to keep the background feeling alive.

### DraculaFormPainter.cs
- **Background:** Draws the base colour then overlays a path-gradient vignette that darkens edges while leaving the centre clear.
- **Borders:** Applies two purple glow passes (8px and 4px pens) before a thin 1px outline derived from `metrics.BorderColor`.
- **Shadow:** Uses a purple-tinted shadow (blur 14, offset 6) to reinforce the nocturnal style.
- **Notes:** Background relies solely on the vignette; additional mood comes from the caption fangs.

### FluentFormPainter.cs
- **Background:** Base fill uses a soft blue-gray solid, while `PaintBackgroundEffects` overlays tiled acrylic noise and a vertical gradient for depth.
- **Borders:** Draws the rounded border shape with `metrics.BorderColor`/`metrics.BorderWidth` and supports non-client inset rendering for reveal effects.
- **Shadow:** Returns a light blur shadow (blur 8, offset 4) to avoid muddying the translucent acrylic.
- **Notes:** Acrylic shimmer and noise are cached for performance and only applied within the clipped region.

### GlassFormPainter.cs
- **Background:** Tints the form with a high-alpha version of the caption colour, then `PaintBackgroundEffects` adds a mica gradient and noise texture.
- **Borders:** Uses a 1px translucent white path around the rounded border to imply glass thickness.
- **Shadow:** Employs a broad but soft shadow (blur 20, offset 6) so the glass panel feels elevated without harsh edges.
- **Notes:** Caption-specific frosting builds on top; the core background stays limited to gradient and noise layers.

### GlassmorphismFormPainter.cs
- **Background:** Fills the rounded path with a semi-transparent base colour, overlays a dotted hatch for frost, and caps with a white sheen on the top third.
- **Borders:** First draws `metrics.BorderColor` at 1px then a 2px low-alpha white path to simulate light catching the glass edge.
- **Shadow:** Provides a soft blur shadow (blur 15, offset 6) with low opacity to keep the panel airy.
- **Notes:** All transparency handling occurs in `PaintBackground`; there is no separate background-effects routine.

### GNOMEFormPainter.cs
- **Background:** Uses a rounded path to fill the form with `metrics.BackgroundColor`, anti-aliased for the Adwaita look.
- **Borders:** Draws the inset rounded border with `metrics.BorderColor` and `metrics.BorderWidth`.
- **Shadow:** Supplies a medium blur shadow (blur 12, offset 4) consistent with GNOME’s soft elevation cues.
- **Notes:** Background remains smooth and monochrome; gradients live in the caption headerbar.

### HolographicFormPainter.cs
- **Background:** After a solid fill, overlays an iridescent gradient using `ColorBlend` and adds a diagonal shine stroke for prismatic highlights.
- **Borders:** Renders a 2px rainbow gradient outline by interpolating magenta, cyan, yellow, and magenta again across the path.
- **Shadow:** Projects a purple-tinted glow shadow (blur 16, offset 8) to echo the holographic lighting.
- **Notes:** Background handles all the iridescence; no extra textures are injected.

### GruvBoxFormPainter.cs
- **Background:** Flat warm base augmented with horizontal grain lines every 3px and a warm 40px glow band at the top.
- **Borders:** Draws the rounded shape with a 2px warm alpha pen (`Color.FromArgb(80, 251, 184, 108)`).
- **Shadow:** Generates a warm brown shadow (blur 8, offset 4) for a vintage vibe.
- **Notes:** Line work supplies the entire texture; gradients are used only for the top glow.

### iOSFormPainter.cs
- **Background:** Applies a vertical gradient from a lightened top colour to the base and adds a translucent white overlay across the top third.
- **Borders:** Uses a 1px low-alpha stroke around `owner.BorderShape` for smooth traffic-light chrome.
- **Shadow:** Provides a gentle blur shadow (blur 14, offset 6) with no horizontal offset.
- **Notes:** Background and border both honour the rounded path; caption frosting introduces dotted texture separately.

### KDEFormPainter.cs
- **Background:** Draws a vertical gradient that is slightly lighter at the top and overlays a subtle white tint on the upper third.
- **Borders:** Renders the rounded shape with a 1px pen using a dimmed `metrics.BorderColor` for a Breeze-style hairline.
- **Shadow:** Emits a cool blue-tinted shadow (blur 12, offset 5) mirroring KDE’s layered surfaces.
- **Notes:** Background work stays restrained so the plasma wave buttons remain the focal point.

### MacOSFormPainter.cs
- **Background:** Solid fill followed by a top highlight band and bottom shade; `PaintBackgroundEffects` adds a translucent overlay for extra depth.
- **Borders:** Draws the rounded border with `metrics.BorderColor` and provides non-client inset painting with an additional bright top line.
- **Shadow:** Supplies a fairly wide shadow (blur 18, offset 10) to emulate macOS drop shadows.
- **Notes:** Depth relies on layered linear gradients rather than textures or patterns.

### MaterialFormPainter.cs
- **Background:** Uses a flat fill and then a subtle top-down gradient (`PaintBackgroundEffects`) to simulate Material Design elevation.
- **Borders:** Draws `owner.BorderShape` with `metrics.BorderColor`/`metrics.BorderWidth`; the non-client border continuation honours the vertical accent bar.
- **Shadow:** Returns a mid-strength shadow (blur 12, offset 4) aligned with Material surface layering.
- **Notes:** Accent colour bars and state layers live in the caption; the background itself stays monochrome.

### Metro2FormPainter.cs
- **Background:** Flat colour with diagonal accent lines drawn every 40px at low alpha to introduce motion.
- **Borders:** Uses a straightforward `metrics.BorderColor` stroke around `owner.BorderShape`, keeping the edges crisp.
- **Shadow:** Provides a light shadow (blur 10, offset 3) to preserve the modern Metro feel.
- **Notes:** Background avoids gradients; diagonal lines supply the only patterning.

### MetroFormPainter.cs
- **Background:** Pure solid fill with anti-aliasing disabled for a classic Metro flat aesthetic.
- **Borders:** Draws the border shape using `metrics.BorderColor` and width, typically with square corners.
- **Shadow:** Keeps a tight drop shadow (blur 5, offset 2) so the sharp geometry stays defined.
- **Notes:** All 3D cues originate from the caption buttons; the background is intentionally austere.

### MinimalFormPainter.cs
- **Background:** Flat fill with a faint vertical highlight (alpha ≈12) to add slight depth without breaking minimalism.
- **Borders:** Draws the rounded border path with `metrics.BorderColor`/`metrics.BorderWidth`, anti-aliased for clean lines.
- **Shadow:** Applies a light shadow (blur 8, offset 2) just enough to separate the form.
- **Notes:** `PaintBackgroundEffects` is intentionally empty; the caption underline acts as the main accent.

### ModernFormPainter.cs
- **Background:** Fills with the base colour then adds a low-alpha vertical gradient overlay to introduce gentle depth.
- **Borders:** Draws the anti-aliased `owner.BorderShape` using `metrics.BorderColor` and width.
- **Shadow:** Returns a medium-weight shadow (blur 10, offset 4) for contemporary elevation.
- **Notes:** Relies on `owner.BorderShape` for clipping; no additional background-effects routine beyond the gradient.

### NeoMorphismFormPainter.cs
- **Background:** Fills the rounded path with a gentle gradient and applies an inner top-left shadow to mimic embossed surfaces.
- **Borders:** Draws a 1px border with a very low-alpha colour so the edge is barely perceptible.
- **Shadow:** Constructs dual lighting—an external soft shadow (blur 20, offset 8) plus inverse highlights—to achieve the soft 3D cushion.
- **Notes:** All lighting cues are built directly into `PaintBackground`; no separate background-effects method is used.

### NeonFormPainter.cs
- **Background:** Uses a dark fill, overlays pink and cyan glow gradients from top and left, and draws neon outline lines along the top and left edges.
- **Borders:** Paints a 3px gradient stroke composed of magenta, cyan, and green for an RGB perimeter glow.
- **Shadow:** Provides a neon-styled shadow (blur 18, offset 9) tinted purple/pink.
- **Notes:** Glow intensity comes from both background gradients and multi-layer border strokes.

### NordFormPainter.cs
- **Background:** Base fill combined with a frost gradient over the top third and a translucent icy top line.
- **Borders:** Draws the border shape with a dimmed `metrics.BorderColor` 1px pen to keep the edge understated.
- **Shadow:** Issues a cool-toned shadow (blur 6, offset 3) for subtle elevation.
- **Notes:** Background keeps to minimal gradients; the rounded triangle caption buttons provide additional motif.

### NordicFormPainter.cs
- **Background:** Applies a gentle vertical gradient (lightened top) using a rounded path for crisp edges.
- **Borders:** Uses a 1px `metrics.BorderColor` stroke on the rounded border shape.
- **Shadow:** Produces a light shadow (blur 10, offset 4) to keep the Scandinavian aesthetic airy.
- **Notes:** Wood-grain and rune motifs are reserved for the caption buttons; the main surface stays clean.

### OneDarkFormPainter.cs
- **Background:** Solid One Dark tone with a dotted 40px grid rendered using low-alpha rectangles to echo code editor minimaps.
- **Borders:** Draws the rounded border with a low-alpha `metrics.BorderColor` 1px pen for a restrained edge.
- **Shadow:** Provides a modest shadow (blur 10, offset 4) with no horizontal offset.
- **Notes:** Background effects depend solely on the grid overlay; no additional gradients are applied.

### PaperFormPainter.cs
- **Background:** Solid base colour with sparse white noise speckles and a 1px top highlight line to suggest a paper edge.
- **Borders:** Draws a 1px low-alpha border following the rounded shape to keep the “paper” edge light.
- **Shadow:** Supplies a Material-style shadow (blur 12, offset 6) for floating-card elevation.
- **Notes:** Additional fibre and torn-edge detailing is confined to the caption buttons.

### RetroFormPainter.cs
- **Background:** Flat 90s-style fill with scanlines every 3px and a 50% hatch to mimic CRT dithering.
- **Borders:** Builds a multi-line bevel: light top/left, dark bottom/right, plus an inner 1px rectangle for the Win95 double frame.
- **Shadow:** Keeps a tight blur (blur 3, offset 4) to echo hard-edged desktop chrome.
- **Notes:** Anti-aliasing stays off throughout to preserve pixel-perfect lines.

### SolarizedFormPainter.cs
- **Background:** Simple solid fill in the Solarized palette—no gradients—to respect the flat design.
- **Borders:** Uses a 1px stroke with lowered alpha based on `metrics.BorderColor` for light edging.
- **Shadow:** Provides a subtle shadow (blur 7, offset 3) so the flat theme still separates from the backdrop.
- **Notes:** All motif work happens in the diamond caption buttons.

### TerminalFormPainter.cs
- **Background:** Solid dark fill with optional scanlines (every 2px) and a low-alpha green grid to evoke terminal phosphor.
- **Borders:** Draws a square 2px border plus carved corner segments using the terminal accent colour.
- **Shadow:** No shadow (blur 0, offsets 0) to keep the console aesthetic firmly flat.
- **Notes:** Anti-aliasing remains off to maintain pixel-perfect lines.

### TokyoFormPainter.cs
- **Background:** Deep fill enriched with a cyan glow over the top 100px, a neon line at the top edge, and subtle scanlines.
- **Borders:** Draws a 2px cyan border over the rounded path for consistent neon framing.
- **Shadow:** Emits a cyan-tinted shadow (blur 14, offset 7) to extend the night-city glow.
- **Notes:** Additional neon intensity is concentrated in the cross-shaped caption buttons.

### UbuntuFormPainter.cs
- **Background:** Applies a warm vertical gradient with a 4px orange accent band along the left edge, inspired by the Unity launcher.
- **Borders:** Renders a 1px border tinted toward orange by adjusting `metrics.BorderColor`.
- **Shadow:** Provides a warm-toned shadow (blur 12, offset 5) matching Canonical’s palette.
- **Notes:** Caption gradients and circular buttons supply the rest of the Ubuntu personality.
