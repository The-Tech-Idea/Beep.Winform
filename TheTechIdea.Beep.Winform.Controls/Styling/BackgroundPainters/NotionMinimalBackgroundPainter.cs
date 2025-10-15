using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

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
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.NotionMinimal);

            // NotionMinimal-specific state handling - NO HELPER FUNCTIONS
            // Unique extremely refined 1-3% changes for Notion's zen minimalism
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Notion hover: Extremely refined 1.5% lighter
                    int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.015f));
                    int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.015f));
                    int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.015f));
                    stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Notion pressed: 2.5% darker (very gentle)
                    int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.025f));
                    int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.025f));
                    int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.025f));
                    stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Notion selected: 3% lighter (zen selection)
                    int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.03f));
                    int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.03f));
                    int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.03f));
                    stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Notion focused: 1% lighter (barely perceptible)
                    int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.01f));
                    int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.01f));
                    int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.01f));
                    stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Notion disabled: 130 alpha (zen translucency)
                    stateColor = Color.FromArgb(130, backgroundColor);
                    break;
                default: // Normal
                    stateColor = backgroundColor;
                    break;
            }

            using (var brush = new SolidBrush(stateColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
