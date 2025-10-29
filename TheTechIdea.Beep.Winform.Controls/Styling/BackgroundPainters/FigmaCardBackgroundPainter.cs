using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Figma Card background painter - Solid white background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class FigmaCardBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Figma Card: Designer-friendly with clear hover feedback
            Color backgroundColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.FigmaCard);

            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.10f)), Math.Min(255, backgroundColor.G + (int)(255 *0.10f)), Math.Min(255, backgroundColor.B + (int)(255 *0.10f))),
                ControlState.Pressed => Color.FromArgb(backgroundColor.A, Math.Max(0, backgroundColor.R - (int)(backgroundColor.R *0.08f)), Math.Max(0, backgroundColor.G - (int)(backgroundColor.G *0.08f)), Math.Max(0, backgroundColor.B - (int)(backgroundColor.B *0.08f))),
                ControlState.Selected => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.15f)), Math.Min(255, backgroundColor.G + (int)(255 *0.15f)), Math.Min(255, backgroundColor.B + (int)(255 *0.15f))),
                ControlState.Focused => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.07f)), Math.Min(255, backgroundColor.G + (int)(255 *0.07f)), Math.Min(255, backgroundColor.B + (int)(255 *0.07f))),
                ControlState.Disabled => Color.FromArgb(100, backgroundColor),
                _ => backgroundColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
