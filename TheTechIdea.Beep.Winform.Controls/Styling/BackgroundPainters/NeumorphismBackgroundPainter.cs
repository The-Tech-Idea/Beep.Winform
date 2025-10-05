using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Neumorphism background painter - Soft embossed with inner highlight
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// Pressed state inverts the shadow effect
    /// </summary>
    public static class NeumorphismBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Neumorphism: Soft 3D embossed effect
            Color baseColor = useThemeColors ? theme.BackColor : Color.FromArgb(230, 230, 230);
            baseColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(baseColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // Inner highlight (top half) - inverted when pressed
            Rectangle highlightRect = new Rectangle(
                bounds.X + 2,
                bounds.Y + 2,
                bounds.Width - 4,
                bounds.Height / 2
            );

            Color highlightColor = state == ControlState.Pressed 
                ? BackgroundPainterHelpers.Darken(baseColor, 0.1f)
                : BackgroundPainterHelpers.Lighten(baseColor, 0.1f);
            
            using (var brush = new SolidBrush(BackgroundPainterHelpers.WithAlpha(highlightColor, 60)))
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

            // Apply state overlay
            if (state != ControlState.Pressed) // Skip overlay for pressed (has inverted highlight)
            {
                Color stateOverlay = BackgroundPainterHelpers.GetStateOverlay(state);
                if (stateOverlay != Color.Transparent)
                {
                    using (var brush = new SolidBrush(stateOverlay))
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
}
