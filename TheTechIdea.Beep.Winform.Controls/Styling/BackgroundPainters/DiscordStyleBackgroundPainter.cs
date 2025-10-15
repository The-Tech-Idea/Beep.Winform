using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Discord ProgressBarStyle background painter - Dark solid background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class DiscordStyleBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Discord: Dark theme with gaming-style glow/brightness feedback
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.DiscordStyle);
            Color primaryColor = useThemeColors ? theme.PrimaryColor : StyleColors.GetPrimary(BeepControlStyle.DiscordStyle);

            // DiscordStyle-specific state handling - NO HELPER FUNCTIONS
            // Unique gaming-style glow and brightness for Discord dark theme
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Discord hover: 12% brighter (noticeable gaming feedback)
                    int hR = Math.Min(255, baseColor.R + (int)(255 * 0.12f));
                    int hG = Math.Min(255, baseColor.G + (int)(255 * 0.12f));
                    int hB = Math.Min(255, baseColor.B + (int)(255 * 0.12f));
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Discord pressed: 8% darker
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.08f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.08f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.08f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Discord selected: 18% brighter (bold gaming selection)
                    int sR = Math.Min(255, baseColor.R + (int)(255 * 0.18f));
                    int sG = Math.Min(255, baseColor.G + (int)(255 * 0.18f));
                    int sB = Math.Min(255, baseColor.B + (int)(255 * 0.18f));
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Discord focused: 10% brighter (clear focus)
                    int fR = Math.Min(255, baseColor.R + (int)(255 * 0.10f));
                    int fG = Math.Min(255, baseColor.G + (int)(255 * 0.10f));
                    int fB = Math.Min(255, baseColor.B + (int)(255 * 0.10f));
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Discord disabled: 80 alpha (very dim)
                    stateColor = Color.FromArgb(80, baseColor);
                    break;
                default: // Normal
                    stateColor = baseColor;
                    break;
            }

            using (var brush = new SolidBrush(stateColor))
            {
                g.FillPath(brush, path);
            }

            // Add subtle gaming glow overlay on hover (Discord-specific)
            if (state == ControlState.Hovered)
            {
                Color glowOverlay = Color.FromArgb(20, primaryColor);
                using (var brush = new SolidBrush(glowOverlay))
                {
                    g.FillPath(brush, path);
                }
            }
        }
    }
}
