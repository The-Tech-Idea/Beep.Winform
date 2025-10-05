using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material You background painter - Solid with 8% tonal primary highlight
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MaterialYouBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Material You: Dynamic color with tonal highlight
            Color backgroundColor = useThemeColors ? theme.BackColor : Color.White;
            Color primaryColor = useThemeColors ? theme.PrimaryColor : Color.FromArgb(103, 80, 164);

            // Apply state modification
            backgroundColor = BackgroundPainterHelpers.ApplyState(backgroundColor, state);

            using (var brush = new SolidBrush(backgroundColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // Add tonal primary highlight (8% alpha)
            Color tonalColor = BackgroundPainterHelpers.WithAlpha(primaryColor, 20);
            using (var brush = new SolidBrush(tonalColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // Apply state overlay
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
