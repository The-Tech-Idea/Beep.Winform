using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Glass Acrylic background painter - 3-layer frosted glass effect
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class GlassAcrylicBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Glass Acrylic: Multi-layer frosted glass
            
            // Layer 1: Base frosted layer (50% alpha) - adjusted by state
            int baseAlpha = state == ControlState.Pressed ? 140 : 127;
            Color baseGlass = BackgroundPainterHelpers.WithAlpha(Color.White, baseAlpha);
            using (var brush = new SolidBrush(baseGlass))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // Layer 2: Top highlight (15% alpha)
            Rectangle highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 3);
            Color highlightGlass = BackgroundPainterHelpers.WithAlpha(Color.White, 38);
            using (var brush = new SolidBrush(highlightGlass))
            {
                if (path != null)
                {
                    using (var highlightPath = BackgroundPainterHelpers.CreateRoundedRectangle(highlightRect, 6))
                    {
                        g.FillPath(brush, highlightPath);
                    }
                }
                else
                {
                    g.FillRectangle(brush, highlightRect);
                }
            }

            // Layer 3: Subtle shine (25% alpha at top)
            Rectangle shineRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 5);
            Color shineGlass = BackgroundPainterHelpers.WithAlpha(Color.White, 64);
            using (var brush = new SolidBrush(shineGlass))
            {
                if (path != null)
                {
                    using (var shinePath = BackgroundPainterHelpers.CreateRoundedRectangle(shineRect, 6))
                    {
                        g.FillPath(brush, shinePath);
                    }
                }
                else
                {
                    g.FillRectangle(brush, shineRect);
                }
            }

            // Apply state overlay
            Color stateOverlay = BackgroundPainterHelpers.GetStateOverlay(state);
            if (stateOverlay != Color.Transparent && state != ControlState.Disabled)
            {
                using (var brush = new SolidBrush(Color.FromArgb(stateOverlay.A / 2, stateOverlay.R, stateOverlay.G, stateOverlay.B)))
                {
                    if (path != null)
                        g.FillPath(brush, path);
                    else
                        g.FillRectangle(brush, bounds);
                }
            }
        }
    }
}
