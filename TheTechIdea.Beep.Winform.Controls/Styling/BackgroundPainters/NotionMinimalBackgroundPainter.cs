using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Notion Minimal background painter - Light solid background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class NotionMinimalBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Notion Minimal: Extremely refined, barely noticeable changes
            Color backgroundColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.NotionMinimal);

            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.015f)), Math.Min(255, backgroundColor.G + (int)(255 *0.015f)), Math.Min(255, backgroundColor.B + (int)(255 *0.015f))),
                ControlState.Pressed => Color.FromArgb(backgroundColor.A, Math.Max(0, backgroundColor.R - (int)(backgroundColor.R *0.025f)), Math.Max(0, backgroundColor.G - (int)(backgroundColor.G *0.025f)), Math.Max(0, backgroundColor.B - (int)(backgroundColor.B *0.025f))),
                ControlState.Selected => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.03f)), Math.Min(255, backgroundColor.G + (int)(255 *0.03f)), Math.Min(255, backgroundColor.B + (int)(255 *0.03f))),
                ControlState.Focused => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.01f)), Math.Min(255, backgroundColor.G + (int)(255 *0.01f)), Math.Min(255, backgroundColor.B + (int)(255 *0.01f))),
                ControlState.Disabled => Color.FromArgb(130, backgroundColor),
                _ => backgroundColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
