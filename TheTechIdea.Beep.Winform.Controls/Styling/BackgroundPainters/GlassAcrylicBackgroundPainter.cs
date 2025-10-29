using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
 /// <summary>
 /// Glass Acrylic background painter -3-layer frosted glass effect
 /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
 /// </summary>
 public static class GlassAcrylicBackgroundPainter
 {
 public static void Paint(Graphics g, GraphicsPath path,
 BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
 ControlState state = ControlState.Normal)
 {
 // Glass Acrylic: Multi-layer frosted glass
 Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.GlassAcrylic);

 // INLINE STATE HANDLING - GlassAcrylic: Base alpha modulation for frosted glass translucency (pressed140, hover +20, selected +30, focus +15, disabled70)
 int baseAlpha = state switch
 {
 ControlState.Pressed =>140,
 ControlState.Hovered => Math.Min(255, baseColor.A +20),
 ControlState.Selected => Math.Min(255, baseColor.A +30),
 ControlState.Focused => Math.Min(255, baseColor.A +15),
 ControlState.Disabled =>70,
 _ => baseColor.A
 };

 Color baseGlass = Color.FromArgb(baseAlpha, baseColor);
 var baseBrush = PaintersFactory.GetSolidBrush(baseGlass);
 g.FillPath(baseBrush, path);

 // Get bounds for clip region calculations
 RectangleF bounds = path.GetBounds();

 // Layer2: Top highlight (15% alpha =38) - Top third region
 Color highlightGlass = Color.FromArgb(38, Color.White);
 var highlightBrush = PaintersFactory.GetSolidBrush(highlightGlass);
 using (var highlightRegion = new Region(path))
 {
 // Clip to top third
 using (var clipRect = new GraphicsPath())
 {
 clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height /3));
 highlightRegion.Intersect(clipRect);
 g.SetClip(highlightRegion, CombineMode.Replace);
 g.FillPath(highlightBrush, path);
 g.ResetClip();
 }
 }

 // Layer3: Subtle shine (25% alpha =64) - Top fifth region
 Color shineGlass = Color.FromArgb(64, Color.White);
 var shineBrush = PaintersFactory.GetSolidBrush(shineGlass);
 using (var shineRegion = new Region(path))
 {
 using (var clipRect = new GraphicsPath())
 {
 clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height /5));
 shineRegion.Intersect(clipRect);
 g.SetClip(shineRegion, CombineMode.Replace);
 g.FillPath(shineBrush, path);
 g.ResetClip();
 }
 }
 }
 }
}
