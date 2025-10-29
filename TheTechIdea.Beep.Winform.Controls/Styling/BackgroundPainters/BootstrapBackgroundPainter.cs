using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Bootstrap background painter - Solid white background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class BootstrapBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Bootstrap: Utilitarian clean with noticeable feedback
            Color backgroundColor = StyleColors.GetBackground(BeepControlStyle.Bootstrap);

            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.06f)), Math.Min(255, backgroundColor.G + (int)(255 *0.06f)), Math.Min(255, backgroundColor.B + (int)(255 *0.06f))),
                ControlState.Pressed => Color.FromArgb(backgroundColor.A, Math.Max(0, backgroundColor.R - (int)(backgroundColor.R *0.07f)), Math.Max(0, backgroundColor.G - (int)(backgroundColor.G *0.07f)), Math.Max(0, backgroundColor.B - (int)(backgroundColor.B *0.07f))),
                ControlState.Selected => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.09f)), Math.Min(255, backgroundColor.G + (int)(255 *0.09f)), Math.Min(255, backgroundColor.B + (int)(255 *0.09f))),
                ControlState.Focused => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.04f)), Math.Min(255, backgroundColor.G + (int)(255 *0.04f)), Math.Min(255, backgroundColor.B + (int)(255 *0.04f))),
                ControlState.Disabled => Color.FromArgb(95, backgroundColor),
                _ => backgroundColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
