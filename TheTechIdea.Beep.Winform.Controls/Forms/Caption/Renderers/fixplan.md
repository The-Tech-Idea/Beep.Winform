# Caption Renderers Fix Plan

Goals
- Make caption bars visually distinct per style (not only color changes)
- Ensure form border thickness and radius changes are visible and affect layout
- Align system icon, theme/style buttons, and title consistently across all renderers
- Standardize sizing/spacing of caption system buttons and insets

Work Done
1) Standardized border drawing
   - Use PenAlignment.Inset to draw borders within client region
   - Sync Padding with `BorderThickness` (except Maximized)
   - Ensure region updates on style/border changes
2) Standardized caption button metrics across renderers
   - Button size = max(24, CaptionHeight - 8*scale)
   - Cluster right padding = 8*scale
   - Title insets computed consistently via `GetTitleInsets`
3) Title and icon alignment fixes
   - Logo width contributes to left inset
   - Extra theme/style buttons space added to right inset
   - Centered title for `Material` (mac-like) and `Gnome`
4) WindowsCaptionRenderer baseline fixed
   - Computes rects with unified rules (pad, btn, top)
   - Uses hover fill per-button consistently

Next Steps
- Apply the same rect computation to other renderers (Kde, Gnome, Cinnamon, Elementary, Neon, Retro, Gaming, Corporate, Artistic, HighContrast, Soft, Industrial)
- Ensure hover and press visuals are consistent per style
- Verify glyph drawing across DPIs (scale correctness)
- Add tests to validate `GetTitleInsets` and layout stays collision-free with Theme/Style buttons and Logo

Visual Requirements Per Renderer
- Gnome: flat glyphs, no hover fill; centered title
- Kde: subtle hover fills; right-aligned cluster
- Cinnamon: bigger button size and spacing; gradient caption
- Elementary: thin glyphs, generous top spacing; gradient off
- Material (MacLike): left circles cluster, centered title
- Neon: glow outline, vibrant colors
- Gaming: cut corners polygons with green/red accents
- Corporate: minimal gray outline, soft hover
- Industrial: metallic gradient background within buttons
- HighContrast: black/white, high stroke widths
- Soft: rounded rectangles with soft blue fill on hover
- Artistic: circular buttons with rainbow palette

Validation Checklist
- [ ] Borders visible in Normal state and hidden in Maximized
- [ ] BorderThickness affects Padding and DisplayRectangle
- [ ] Caption system buttons never overlap logo or extra buttons
- [ ] Title is centered for Material/Gnome and left-aligned otherwise
- [ ] Hover states consistent per renderer
- [ ] DPI scaling validated at 100%, 150%, 200%

Notes
- CaptionRendererKind remains for backward compatibility but FormStyle is the single source of truth.
- Update README with unified style system and migration guidance (done).
