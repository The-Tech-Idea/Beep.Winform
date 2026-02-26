using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Ubuntu background painter - Yaru theme design
    /// Clean solid background with optional left accent stripe
    /// Modern Ubuntu design: subtle, professional, orange accent
    /// </summary>
    public static class UbuntuBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Ubuntu Yaru: neutral light background (not orange gradient)
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xFA, 0xFA, 0xFA); // Yaru light gray
            Color accentColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.FromArgb(0xE9, 0x54, 0x20); // Ubuntu orange

            // Solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Ubuntu accent stripe on left edge (Yaru signature)
            BackgroundPainterHelpers.PaintAccentStripe(g, Rectangle.Round(bounds),
                Color.FromArgb(120, accentColor), BackgroundPainterHelpers.StripeSide.Left, 3, path);
        }
    }
}
