using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Discord background painter - dark gaming-inspired design
    /// Dark solid background with strong state feedback and hover glow
    /// </summary>
    public static class DiscordStyleBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Discord: dark gray background
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.DiscordStyle);
            Color primaryColor = useThemeColors && theme != null 
                ? theme.PrimaryColor 
                : StyleColors.GetPrimary(BeepControlStyle.DiscordStyle);

            // Discord uses strong state feedback (gaming-style)
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);

            // Discord signature: subtle primary glow overlay on hover
            if (state == ControlState.Hovered)
            {
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(18, primaryColor));
                g.FillPath(glowBrush, path);
            }
        }
    }
}
