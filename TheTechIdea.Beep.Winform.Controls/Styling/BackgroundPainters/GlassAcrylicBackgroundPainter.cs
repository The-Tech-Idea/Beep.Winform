using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Glass Acrylic background painter - 3-layer frosted glass effect
    /// Multi-layer glass with top highlight and shine for premium look
    /// </summary>
    public static class GlassAcrylicBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Glass Acrylic: semi-transparent base
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.GlassAcrylic);

            // Use enhanced blur for better glassmorphism effect
            int blurRadius = 16; // Medium blur for glass acrylic
            BackgroundPainterHelpers.PaintAdvancedBlurBackground(g, path, baseColor, blurRadius, state);

            // State-based alpha modulation
            int baseAlpha = state switch
            {
                ControlState.Pressed => 140,
                ControlState.Hovered => Math.Min(255, 200 + 20),
                ControlState.Selected => Math.Min(255, 200 + 30),
                ControlState.Focused => Math.Min(255, 200 + 15),
                ControlState.Disabled => 70,
                _ => 200
            };

            // Also apply frosted glass effect for additional realism
            BackgroundPainterHelpers.PaintFrostedGlassBackground(
                g,
                path,
                baseColor,
                baseAlpha,
                state,
                BackgroundPainterHelpers.ShouldPaintDecorativeEdgeStroke(style));

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Radial gradients enhance glass effect - add subtle radial highlight
            Color centerColor = ColorAccessibilityHelper.LightenColor(baseColor, 0.08f);
            Color edgeColor = baseColor;
            BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);

            // Layer 2: Top highlight (top third)
            using (var highlightRegion = new Region(path))
            using (var clipRect = new GraphicsPath())
            {
                clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height / 3));
                highlightRegion.Intersect(clipRect);
                g.SetClip(highlightRegion, CombineMode.Replace);
                
                var highlightBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(35, Color.White));
                g.FillPath(highlightBrush, path);
                g.ResetClip();
            }

            // Layer 3: Top shine (top fifth)
            using (var shineRegion = new Region(path))
            using (var clipRect = new GraphicsPath())
            {
                clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height / 5));
                shineRegion.Intersect(clipRect);
                g.SetClip(shineRegion, CombineMode.Replace);
                
                var shineBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(55, Color.White));
                g.FillPath(shineBrush, path);
                g.ResetClip();
            }
        }
    }
}
