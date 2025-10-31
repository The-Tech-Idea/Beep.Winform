using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for Material Design styles (Material3, MaterialYou)
    /// </summary>
    public static class MaterialBackgroundPainter
    {
        /// <summary>
        /// Paint Material Design background with subtle elevation
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);

            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillPath(bgBrush, path);

            // Subtle elevation tint from top (Material surface depth)
            RectangleF bounds = path.GetBounds();
            Color highlight = Color.FromArgb(14,0,0,0);
            var highlightBrush = PaintersFactory.GetSolidBrush(highlight);
            using (var highlightRegion = new Region(path))
            {
                using (var clipRect = new GraphicsPath())
                {
                    clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width,2));
                    highlightRegion.Intersect(clipRect);
                    g.SetClip(highlightRegion, CombineMode.Replace);
                    g.FillPath(highlightBrush, path);
                    g.ResetClip();
                }
            }
        }

        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}

