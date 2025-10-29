using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Vercel Clean background painter - Pure white solid
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class VercelCleanBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Vercel Clean: Pure clean white with subtle feedback
            Color backgroundColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.VercelClean);

            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.03f)), Math.Min(255, backgroundColor.G + (int)(255 *0.03f)), Math.Min(255, backgroundColor.B + (int)(255 *0.03f))),
                ControlState.Pressed => Color.FromArgb(backgroundColor.A, Math.Max(0, backgroundColor.R - (int)(backgroundColor.R *0.04f)), Math.Max(0, backgroundColor.G - (int)(backgroundColor.G *0.04f)), Math.Max(0, backgroundColor.B - (int)(backgroundColor.B *0.04f))),
                ControlState.Selected => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.05f)), Math.Min(255, backgroundColor.G + (int)(255 *0.05f)), Math.Min(255, backgroundColor.B + (int)(255 *0.05f))),
                ControlState.Focused => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.02f)), Math.Min(255, backgroundColor.G + (int)(255 *0.02f)), Math.Min(255, backgroundColor.B + (int)(255 *0.02f))),
                ControlState.Disabled => Color.FromArgb(110, backgroundColor),
                _ => backgroundColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
