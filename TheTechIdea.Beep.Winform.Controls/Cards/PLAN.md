BeepCard Refactor Plan

Goals
- Align BeepCard with BaseControl/Material pipeline
- Introduce painter strategy (helper) to render card backgrounds/accents for multiple UI styles inspired by modern JS frameworks
- Keep CardViewMode (content layout) and add CardStyle (visual style)
- Make layout painter-adjustable for accents (header stripe, avatar ring, etc.)

Key Concepts
- CardViewMode: FullImage, Compact, ImageLeft (existing)
- CardStyle (new): Classic, MaterialElevated, SoftShadow, Outline, AccentHeader, ListTile, Glass

Helpers
- ICardPainter
  - Initialize(BaseControl owner, IBeepTheme theme)
  - AdjustLayout(Rectangle drawingRect, LayoutContext ctx) -> LayoutContext
  - DrawBackground(Graphics g, LayoutContext ctx)
  - DrawForegroundAccents(Graphics g, LayoutContext ctx)
- CardPainterBase: common utilities, rounded path, subtle shadows
- Concrete painters:
  - MaterialElevatedCardPainter (MD3 card with elevation)
  - SoftShadowCardPainter (neumorphism-like soft shadow)
  - OutlineCardPainter (bordered, flat background)
  - AccentHeaderCardPainter (top accent strip and optional footer)
  - ListTileCardPainter (avatar emphasis, small badge)
  - GlassCardPainter (glassmorphism)

BeepCard changes
- Add CardStyle enum + property
- Add AccentColor property
- Create LayoutContext class (imageRect, headerRect, paragraphRect, buttonRect, flags)
- RefreshLayout computes base rectangles by view mode, then lets painter tweak via AdjustLayout
- DrawContent uses painter to draw background/accents, then draws children and hit areas
- ApplyTheme updates subcomponents and reinitializes painter

Testing
- Verify each style renders with/without image and button
- Check Material-enabled parent draws outline; painters only handle inner background and accents
