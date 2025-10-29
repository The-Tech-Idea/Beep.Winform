using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

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
        public static void PaintBootstrap(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);

            // Simple solid background - Bootstrap is clean and simple
            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillPath(bgBrush, path);
        }
        
        /// <summary>
        /// Paint Tailwind card background with subtle border glow
        /// </summary>
        public static void PaintTailwind(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);

            // Base background
            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillPath(bgBrush, path);

            // Tailwind often has a subtle ring/outline effect
            Color ringColor = Color.FromArgb(8,0,0,0);
            var ringPen = PaintersFactory.GetPen(ringColor,1f);
            g.DrawPath(ringPen, path);
        }
        
        /// <summary>
        /// Paint Discord background with dark theme
        /// </summary>
        public static void PaintDiscord(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);

            // Solid dark background
            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillPath(bgBrush, path);
        }
        
        /// <summary>
        /// Paint Stripe dashboard background with subtle gradient
        /// </summary>
        public static void PaintStripe(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);

            // Base background
            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillPath(bgBrush, path);

            // Stripe has very subtle gradients for polish
            RectangleF bounds = path.GetBounds();
            var gradientBrush = PaintersFactory.GetLinearGradientBrush(bounds, Color.FromArgb(3,255,255,255), Color.FromArgb(3,0,0,0), LinearGradientMode.Vertical);
            g.FillPath(gradientBrush, path);
        }
        
        /// <summary>
        /// Paint Figma card background with clean appearance
        /// </summary>
        public static void PaintFigma(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);

            // Clean solid background
            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillPath(bgBrush, path);

            // Figma uses subtle border for definition
            Color borderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            var borderPen = PaintersFactory.GetPen(borderColor,0.5f);
            g.DrawPath(borderPen, path);
        }
        
        /// <summary>
        /// Paints the background based on the control Style.
        /// </summary>
        /// <param name="g">The graphics object to paint on.</param>
        /// <param name="path">The graphics path defining the shape to paint.</param>
        /// <param name="style">The control Style.</param>
        /// <param name="theme">The theme information.</param>
        /// <param name="useThemeColors">Whether to use theme colors.</param>
        /// <param name="state">The control state.</param>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            switch (style)
            {
                case BeepControlStyle.Bootstrap:
                    PaintBootstrap(g, path, style, theme, useThemeColors);
                    break;
                case BeepControlStyle.TailwindCard:
                    PaintTailwind(g, path, style, theme, useThemeColors);
                    break;
                case BeepControlStyle.DiscordStyle:
                    PaintDiscord(g, path, style, theme, useThemeColors);
                    break;
                case BeepControlStyle.StripeDashboard:
                    PaintStripe(g, path, style, theme, useThemeColors);
                    break;
                case BeepControlStyle.FigmaCard:
                    PaintFigma(g, path, style, theme, useThemeColors);
                    break;
                case BeepControlStyle.WebFramework:
                    // Default to Bootstrap for generic WebFramework Style
                    PaintBootstrap(g, path, style, theme, useThemeColors);
                    break;
                default:
                    throw new NotSupportedException($"Style {style} is not supported by WebFrameworkBackgroundPainter.");
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
