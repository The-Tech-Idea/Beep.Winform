using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painters for web framework styles (Bootstrap, Tailwind, Stripe, Figma, Discord)
    /// </summary>
    public static class WebFrameworkBackgroundPainter
    {
        /// <summary>
        /// Paint Bootstrap card background
        /// </summary>
        public static void PaintBootstrap(Graphics g, Rectangle bounds, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Simple solid background - Bootstrap is clean and simple
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
        }
        
        /// <summary>
        /// Paint Tailwind card background with subtle border glow
        /// </summary>
        public static void PaintTailwind(Graphics g, Rectangle bounds, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Base background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // Tailwind often has a subtle ring/outline effect
            Color ringColor = Color.FromArgb(8, 0, 0, 0);
            using (var ringPen = new Pen(ringColor, 1f))
            {
                g.DrawPath(ringPen, path);
            }
        }
        
        /// <summary>
        /// Paint Discord background with dark theme
        /// </summary>
        public static void PaintDiscord(Graphics g, Rectangle bounds, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Solid dark background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
        }
        
        /// <summary>
        /// Paint Stripe dashboard background with subtle gradient
        /// </summary>
        public static void PaintStripe(Graphics g, Rectangle bounds, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Base background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // Stripe has very subtle gradients for polish
            Color topTint = Color.FromArgb(3, 255, 255, 255);
            Color bottomTint = Color.FromArgb(3, 0, 0, 0);
            using (var gradientBrush = new LinearGradientBrush(
                bounds,
                topTint,
                bottomTint,
                LinearGradientMode.Vertical))
            {
                g.FillPath(gradientBrush, path);
            }
        }
        
        /// <summary>
        /// Paint Figma card background with clean appearance
        /// </summary>
        public static void PaintFigma(Graphics g, Rectangle bounds, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Clean solid background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // Figma uses subtle border for definition
            Color borderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            using (var borderPen = new Pen(borderColor, 0.5f))
            {
                g.DrawPath(borderPen, path);
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
