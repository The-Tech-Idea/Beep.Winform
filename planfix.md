# Plan to Fix BorderPainters - Return GraphicsPath

This plan outlines the changes needed for all BorderPainter classes in the folder `C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Styling\BorderPainters\`.

The Paint methods have been changed to return `GraphicsPath`, but they must return a new `GraphicsPath` representing the area inside the border (the internal area), not the original path. Each class should paint its own distinct border style, and not all should use `BorderPainterHelpers.PaintSimpleBorder`. The returned path should be the area available for content after the border is drawn.

## Classes to Update

- [x] AntDesignBorderPainter.cs
- [x] AppleBorderPainter.cs
- [x] BootstrapBorderPainter.cs
- [x] ChakraUIBorderPainter.cs
- [ ] DarkGlowBorderPainter.cs
- [ ] DiscordStyleBorderPainter.cs
- [ ] EffectBorderPainter.cs
- [ ] FigmaCardBorderPainter.cs
- [ ] Fluent2BorderPainter.cs
- [ ] FluentBorderPainter.cs
- [ ] GlassAcrylicBorderPainter.cs
- [ ] GradientModernBorderPainter.cs
- [ ] iOS15BorderPainter.cs
- [ ] MacOSBigSurBorderPainter.cs
- [ ] Material3BorderPainter.cs
- [ ] MaterialBorderPainter.cs
- [ ] MaterialYouBorderPainter.cs
- [ ] MinimalBorderPainter.cs
- [ ] NeumorphismBorderPainter.cs
- [ ] NotionMinimalBorderPainter.cs
- [ ] PillRailBorderPainter.cs
- [ ] StripeDashboardBorderPainter.cs
- [ ] TailwindCardBorderPainter.cs
- [ ] VercelCleanBorderPainter.cs
- [ ] WebFrameworkBorderPainter.cs
- [ ] Windows11MicaBorderPainter.cs

## Implementation Steps

1. For each class, after painting the border, create and return a new `GraphicsPath` representing the area inside the border (subtracting the border thickness from the original path).
2. Ensure each class paints its own distinct border style (not all use `BorderPainterHelpers.PaintSimpleBorder`).
3. Update this document by marking the class as completed.
4. Proceed to the next class.

## Notes

- Each class paints a distinct border style, not all use BorderPainterHelpers.PaintSimpleBorder.
- The `path` parameter represents the shape of the control.
- After painting the border, the returned GraphicsPath should represent the area inside the border (subtract border thickness from the original path bounds).
- Each class should implement its own border logic and return the correct internal area for content.