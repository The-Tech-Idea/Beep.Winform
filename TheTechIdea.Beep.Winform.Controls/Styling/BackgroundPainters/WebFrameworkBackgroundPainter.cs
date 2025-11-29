using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painters for web framework styles
    /// Delegates to individual painters for each framework
    /// </summary>
    public static class WebFrameworkBackgroundPainter
    {
        /// <summary>
        /// Paint Bootstrap card background - clean and simple
        /// </summary>
        public static void PaintBootstrap(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            Color bgColor = BackgroundPainterHelpers.GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            BackgroundPainterHelpers.PaintSolidBackground(g, path, bgColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);
        }
        
        /// <summary>
        /// Paint Tailwind card background with subtle ring effect
        /// </summary>
        public static void PaintTailwind(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            Color bgColor = BackgroundPainterHelpers.GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            BackgroundPainterHelpers.PaintSolidBackground(g, path, bgColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);

            // Tailwind subtle ring/outline effect
            var ringPen = PaintersFactory.GetPen(Color.FromArgb(8, 0, 0, 0), 1f);
            g.DrawPath(ringPen, path);
        }
        
        /// <summary>
        /// Paint Discord background - dark theme
        /// </summary>
        public static void PaintDiscord(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            Color bgColor = BackgroundPainterHelpers.GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            BackgroundPainterHelpers.PaintSolidBackground(g, path, bgColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);
        }
        
        /// <summary>
        /// Paint Stripe dashboard background with subtle gradient
        /// </summary>
        public static void PaintStripe(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            Color bgColor = BackgroundPainterHelpers.GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            BackgroundPainterHelpers.PaintSolidBackground(g, path, bgColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // Stripe polish: very subtle vertical gradient overlay
            BackgroundPainterHelpers.PaintSubtleGradientBackground(g, path,
                bgColor, 0.03f, state, BackgroundPainterHelpers.StateIntensity.Subtle);
        }
        
        /// <summary>
        /// Paint Figma card background - clean with subtle border
        /// </summary>
        public static void PaintFigma(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            Color bgColor = BackgroundPainterHelpers.GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            BackgroundPainterHelpers.PaintSolidBackground(g, path, bgColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);

            // Figma subtle border for definition
            Color borderColor = BackgroundPainterHelpers.GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            var borderPen = PaintersFactory.GetPen(borderColor, 0.5f);
            g.DrawPath(borderPen, path);
        }
        
        /// <summary>
        /// Paints the background based on the control style.
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            switch (style)
            {
                case BeepControlStyle.Bootstrap:
                    PaintBootstrap(g, path, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.TailwindCard:
                    PaintTailwind(g, path, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.DiscordStyle:
                    PaintDiscord(g, path, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.StripeDashboard:
                    PaintStripe(g, path, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.FigmaCard:
                    PaintFigma(g, path, style, theme, useThemeColors, state);
                    break;
                case BeepControlStyle.WebFramework:
                    // Default to Bootstrap for generic WebFramework style
                    PaintBootstrap(g, path, style, theme, useThemeColors, state);
                    break;
                default:
                    // Fallback: solid background
                    var bgColor = StyleColors.GetBackground(style);
                    BackgroundPainterHelpers.PaintSolidBackground(g, path, bgColor, state,
                        BackgroundPainterHelpers.StateIntensity.Normal);
                    break;
            }
        }
    }
}
