using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Generic solid background painter for styles that need simple fill
    /// Used by: various simple styles as fallback or base
    /// </summary>
    public static class SolidBackgroundPainter
    {
        /// <summary>
        /// Paint solid color background with state awareness
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            Color bgColor = GetBackgroundColor(style, theme, useThemeColors);
            BackgroundPainterHelpers.PaintSolidBackground(g, path, bgColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);
        }

        /// <summary>
        /// Legacy overload without state (defaults to Normal)
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, path, style, theme, useThemeColors, ControlState.Normal);
        }
        
        private static Color GetBackgroundColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor("Background");
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return StyleColors.GetBackground(style);
        }
    }
}
