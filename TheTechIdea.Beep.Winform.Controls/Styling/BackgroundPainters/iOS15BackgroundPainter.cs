using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// iOS15 background painter - Solid with15% white translucent overlay
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class iOS15BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // iOS15: Clean with translucent blur-inspired overlay
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.iOS15);

            // iOS15-specific state handling - NO HELPER FUNCTIONS
            // Unique blur + translucent overlay intensity for iOS design
            Color stateColor;
            int translucencyBoost =0; // Additional alpha for translucent overlay
            
            switch (state)
            {
                case ControlState.Hovered:
                    // iOS15 hover: Increase blur opacity (30 alpha boost)
                    stateColor = baseColor;
                    translucencyBoost =30;
                    break;
                case ControlState.Pressed:
                    // iOS15 pressed: Strong blur opacity increase (50 alpha boost)
                    stateColor = baseColor;
                    translucencyBoost =50;
                    break;
                case ControlState.Selected:
                    // iOS15 selected: Noticeable blur opacity (40 alpha boost)
                    stateColor = baseColor;
                    translucencyBoost =40;
                    break;
                case ControlState.Focused:
                    // iOS15 focused: Gentle blur opacity (20 alpha boost)
                    stateColor = baseColor;
                    translucencyBoost =20;
                    break;
                case ControlState.Disabled:
                    // iOS15 disabled: Very translucent (60 alpha)
                    stateColor = Color.FromArgb(60, baseColor);
                    translucencyBoost =0; // No additional overlay when disabled
                    break;
                default: // Normal
                    stateColor = baseColor;
                    translucencyBoost =0;
                    break;
            }

            var baseBrush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(baseBrush, path);

            // Add translucent white overlay (iOS blur effect) with state-aware intensity
            if (state != ControlState.Disabled)
            {
                int overlayAlpha = Math.Min(255,38 + translucencyBoost);
                Color overlayColor = Color.FromArgb(overlayAlpha, Color.White);
                var overlayBrush = PaintersFactory.GetSolidBrush(overlayColor);
                g.FillPath(overlayBrush, path);
            }
        }
    }
}
