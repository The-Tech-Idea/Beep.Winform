using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters
{
    /// <summary>
    /// Provides style-specific colors derived from IBeepTheme for WebHeader painters.
    /// Each style can customize how it interprets theme colors.
    /// </summary>
    public class WebHeaderColors
    {
        // Core colors
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Color AccentColor { get; set; }
        public Color AccentHoverColor { get; set; }
        
        // Tab colors
        public Color TabTextColor { get; set; }
        public Color TabActiveTextColor { get; set; }
        public Color TabHoverTextColor { get; set; }
        public Color TabActiveBackgroundColor { get; set; }
        public Color TabHoverBackgroundColor { get; set; }
        public Color TabIndicatorColor { get; set; }
        
        // Button colors
        public Color ButtonBackgroundColor { get; set; }
        public Color ButtonTextColor { get; set; }
        public Color ButtonHoverBackgroundColor { get; set; }
        public Color ButtonHoverTextColor { get; set; }
        public Color ButtonBorderColor { get; set; }
        public Color CtaBackgroundColor { get; set; }
        public Color CtaTextColor { get; set; }
        public Color CtaHoverBackgroundColor { get; set; }
        
        // UI element colors
        public Color BorderColor { get; set; }
        public Color DividerColor { get; set; }
        public Color SearchBackgroundColor { get; set; }
        public Color SearchTextColor { get; set; }
        public Color SearchPlaceholderColor { get; set; }
        public Color ShadowColor { get; set; }
        
        /// <summary>
        /// Creates default colors from theme
        /// </summary>
        public static WebHeaderColors FromTheme(IBeepTheme theme)
        {
            if (theme == null)
                return CreateDefaultLight();
                
            return new WebHeaderColors
            {
                BackgroundColor = theme.BackColor,
                ForegroundColor = theme.ForeColor,
                AccentColor = theme.ButtonBackColor,
                AccentHoverColor = Lighten(theme.ButtonBackColor, 0.1f),
                
                TabTextColor = theme.ForeColor,
                TabActiveTextColor = theme.ButtonBackColor,
                TabHoverTextColor = Lighten(theme.ForeColor, 0.2f),
                TabActiveBackgroundColor = Color.FromArgb(30, theme.ButtonBackColor),
                TabHoverBackgroundColor = Color.FromArgb(20, theme.ForeColor),
                TabIndicatorColor = theme.ButtonBackColor,
                
                ButtonBackgroundColor = Color.Transparent,
                ButtonTextColor = theme.ForeColor,
                ButtonHoverBackgroundColor = Color.FromArgb(30, theme.ForeColor),
                ButtonHoverTextColor = theme.ForeColor,
                ButtonBorderColor = Color.FromArgb(80, theme.ForeColor),
                CtaBackgroundColor = theme.ButtonBackColor,
                CtaTextColor = theme.ButtonForeColor,
                CtaHoverBackgroundColor = Darken(theme.ButtonBackColor, 0.1f),
                
                BorderColor = Color.FromArgb(40, theme.ForeColor),
                DividerColor = Color.FromArgb(20, theme.ForeColor),
                SearchBackgroundColor = Color.FromArgb(10, theme.ForeColor),
                SearchTextColor = theme.ForeColor,
                SearchPlaceholderColor = Color.FromArgb(120, theme.ForeColor),
                ShadowColor = Color.FromArgb(30, Color.Black)
            };
        }
        
        /// <summary>
        /// Creates default light theme colors
        /// </summary>
        public static WebHeaderColors CreateDefaultLight()
        {
            return new WebHeaderColors
            {
                BackgroundColor = Color.White,
                ForegroundColor = Color.FromArgb(33, 33, 33),
                AccentColor = Color.FromArgb(66, 133, 244),
                AccentHoverColor = Color.FromArgb(86, 153, 255),
                
                TabTextColor = Color.FromArgb(100, 100, 100),
                TabActiveTextColor = Color.FromArgb(33, 33, 33),
                TabHoverTextColor = Color.FromArgb(66, 66, 66),
                TabActiveBackgroundColor = Color.FromArgb(245, 245, 245),
                TabHoverBackgroundColor = Color.FromArgb(250, 250, 250),
                TabIndicatorColor = Color.FromArgb(66, 133, 244),
                
                ButtonBackgroundColor = Color.Transparent,
                ButtonTextColor = Color.FromArgb(66, 66, 66),
                ButtonHoverBackgroundColor = Color.FromArgb(245, 245, 245),
                ButtonHoverTextColor = Color.FromArgb(33, 33, 33),
                ButtonBorderColor = Color.FromArgb(220, 220, 220),
                CtaBackgroundColor = Color.FromArgb(66, 133, 244),
                CtaTextColor = Color.White,
                CtaHoverBackgroundColor = Color.FromArgb(56, 123, 234),
                
                BorderColor = Color.FromArgb(230, 230, 230),
                DividerColor = Color.FromArgb(240, 240, 240),
                SearchBackgroundColor = Color.FromArgb(245, 245, 248),
                SearchTextColor = Color.FromArgb(66, 66, 66),
                SearchPlaceholderColor = Color.FromArgb(160, 160, 160),
                ShadowColor = Color.FromArgb(20, 0, 0, 0)
            };
        }
        
        /// <summary>
        /// Creates dark theme colors
        /// </summary>
        public static WebHeaderColors CreateDefaultDark()
        {
            return new WebHeaderColors
            {
                BackgroundColor = Color.FromArgb(24, 24, 32),
                ForegroundColor = Color.FromArgb(240, 240, 245),
                AccentColor = Color.FromArgb(255, 140, 0),
                AccentHoverColor = Color.FromArgb(255, 160, 40),
                
                TabTextColor = Color.FromArgb(180, 180, 190),
                TabActiveTextColor = Color.White,
                TabHoverTextColor = Color.FromArgb(220, 220, 225),
                TabActiveBackgroundColor = Color.FromArgb(255, 140, 0),
                TabHoverBackgroundColor = Color.FromArgb(50, 50, 65),
                TabIndicatorColor = Color.FromArgb(255, 140, 0),
                
                ButtonBackgroundColor = Color.FromArgb(45, 45, 60),
                ButtonTextColor = Color.FromArgb(200, 200, 210),
                ButtonHoverBackgroundColor = Color.FromArgb(255, 140, 0),
                ButtonHoverTextColor = Color.White,
                ButtonBorderColor = Color.FromArgb(80, 80, 100),
                CtaBackgroundColor = Color.FromArgb(255, 140, 0),
                CtaTextColor = Color.White,
                CtaHoverBackgroundColor = Color.FromArgb(255, 160, 40),
                
                BorderColor = Color.FromArgb(60, 60, 75),
                DividerColor = Color.FromArgb(50, 50, 65),
                SearchBackgroundColor = Color.FromArgb(40, 40, 55),
                SearchTextColor = Color.FromArgb(220, 220, 225),
                SearchPlaceholderColor = Color.FromArgb(140, 140, 150),
                ShadowColor = Color.FromArgb(40, 0, 0, 0)
            };
        }
        
        #region Color Utilities
        
        public static Color Lighten(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }
        
        public static Color Darken(Color color, float amount)
        {
            int r = Math.Max(0, (int)(color.R * (1 - amount)));
            int g = Math.Max(0, (int)(color.G * (1 - amount)));
            int b = Math.Max(0, (int)(color.B * (1 - amount)));
            return Color.FromArgb(color.A, r, g, b);
        }
        
        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(Math.Clamp(alpha, 0, 255), color.R, color.G, color.B);
        }
        
        public static Color Blend(Color color1, Color color2, float amount)
        {
            int r = (int)(color1.R + (color2.R - color1.R) * amount);
            int g = (int)(color1.G + (color2.G - color1.G) * amount);
            int b = (int)(color1.B + (color2.B - color1.B) * amount);
            return Color.FromArgb(255, Math.Clamp(r, 0, 255), Math.Clamp(g, 0, 255), Math.Clamp(b, 0, 255));
        }
        
        #endregion
    }
}

