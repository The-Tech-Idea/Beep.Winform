using System.Drawing;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Standard Design text painter using FontManagement system
    /// Enhanced version covering all remaining styles
    /// </summary>
    public class StandardDesignTextPainter : DesignSystemTextPainter
    {
        #region Design System Properties
        protected override string PrimaryFontFamily => "Segoe UI";
        protected override string FallbackFontFamily => "Arial";

        protected override float GetFontSize(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.AntDesign => 14f,
                BeepControlStyle.StripeDashboard => 14f,
                BeepControlStyle.DiscordStyle => 14f,
                BeepControlStyle.GradientModern => 14f,
                BeepControlStyle.GlassAcrylic => 14f,
                BeepControlStyle.Neumorphism => 14f,
                BeepControlStyle.PillRail => 13f,
                _ => 14f
            };
        }

        protected override float GetLetterSpacing(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.AntDesign => 0f,           // No tracking
                BeepControlStyle.StripeDashboard => 0f,     // No tracking
                BeepControlStyle.DiscordStyle => 0f,        // No tracking
                BeepControlStyle.GradientModern => 0.3f,    // Wider for modern look
                BeepControlStyle.GlassAcrylic => 0f,        // No tracking
                BeepControlStyle.Neumorphism => 0.5f,       // Wide for soft look
                BeepControlStyle.PillRail => 0f,            // No tracking
                _ => 0f
            };
        }

        protected override TextRenderingHint GetRenderingHint()
        {
            return TextRenderingHint.ClearTypeGridFit;
        }

        protected override Color GetTextColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            return GetColor(style, StyleColors.GetForeground, "Foreground", theme, useThemeColors);
        }
        #endregion

        #region Standard Design Font Management
        /// <summary>
        /// Get standard font with style-specific family preferences
        /// </summary>
        protected override Font GetFont(BeepControlStyle style, bool isFocused)
        {
            float fontSize = GetFontSize(style);
            FontStyle fontStyle = isFocused ? FontStyle.Bold : FontStyle.Regular;
            
            // Get style-specific font family
            string fontFamily = GetStyleSpecificFontFamily(style);
            
            Font styledFont = FontListHelper.GetFont(fontFamily, fontSize, fontStyle);
            if (styledFont != null)
                return styledFont;
            
            // Fallback to standard fonts
            return FontListHelper.GetFontWithFallback(PrimaryFontFamily, FallbackFontFamily, fontSize, fontStyle);
        }

        /// <summary>
        /// Get font family specific to each style
        /// </summary>
        private string GetStyleSpecificFontFamily(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.AntDesign => "Chinese Quote",         // Ant Design specific
                BeepControlStyle.StripeDashboard => "Inter",           // Stripe uses Inter
                BeepControlStyle.DiscordStyle => "Whitney",            // Discord uses Whitney
                BeepControlStyle.GradientModern => "Inter",            // Modern designs use Inter
                BeepControlStyle.GlassAcrylic => "Segoe UI Variable",  // Windows 11 style
                BeepControlStyle.Neumorphism => "Montserrat",          // Soft, rounded font
                BeepControlStyle.PillRail => "Inter",                  // Clean, minimal
                _ => PrimaryFontFamily
            };
        }
        #endregion

        #region Style-Specific Typography Variants
        /// <summary>
        /// Paint Discord-style text with appropriate styling
        /// </summary>
        public static void PaintDiscord(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            var painter = new StandardDesignTextPainter();
            
            using (Font discordFont = painter.GetFont(BeepControlStyle.DiscordStyle, isFocused))
            using (SolidBrush brush = new SolidBrush(painter.GetDiscordColor(theme, useThemeColors)))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.DrawString(text, discordFont, brush, bounds, painter.GetStringFormat());
            }
        }

        /// <summary>
        /// Paint Ant Design text with Chinese character support
        /// </summary>
        public static void PaintAntDesign(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            var painter = new StandardDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, BeepControlStyle.AntDesign, theme, useThemeColors);
        }

        /// <summary>
        /// Paint Neumorphism text with soft appearance
        /// </summary>
        public static void PaintNeumorphism(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            var painter = new StandardDesignTextPainter();
            
            using (Font neuFont = painter.GetFont(BeepControlStyle.Neumorphism, isFocused))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Color textColor = painter.GetTextColor(BeepControlStyle.Neumorphism, theme, useThemeColors);
                
                // Soft shadow effect for neumorphism
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    Rectangle shadowBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width, bounds.Height);
                    painter.DrawTextWithLetterSpacing(g, text, neuFont, shadowBrush, shadowBounds, 
                        painter.GetLetterSpacing(BeepControlStyle.Neumorphism));
                }
                
                // Main text
                using (SolidBrush mainBrush = new SolidBrush(textColor))
                {
                    painter.DrawTextWithLetterSpacing(g, text, neuFont, mainBrush, bounds, 
                        painter.GetLetterSpacing(BeepControlStyle.Neumorphism));
                }
            }
        }

        /// <summary>
        /// Paint Glass Acrylic text with transparency effects
        /// </summary>
        public static void PaintGlassAcrylic(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            var painter = new StandardDesignTextPainter();
            
            using (Font glassFont = painter.GetFont(BeepControlStyle.GlassAcrylic, isFocused))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Color baseColor = painter.GetTextColor(BeepControlStyle.GlassAcrylic, theme, useThemeColors);
                
                // Slightly transparent for glass effect
                Color glassColor = Color.FromArgb(220, baseColor.R, baseColor.G, baseColor.B);
                
                using (SolidBrush glassBrush = new SolidBrush(glassColor))
                {
                    g.DrawString(text, glassFont, glassBrush, bounds, painter.GetStringFormat());
                }
            }
        }

        /// <summary>
        /// Paint Gradient Modern text with enhanced spacing
        /// </summary>
        public static void PaintGradientModern(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            var painter = new StandardDesignTextPainter();
            
            using (Font modernFont = painter.GetFont(BeepControlStyle.GradientModern, isFocused))
            using (SolidBrush modernBrush = new SolidBrush(painter.GetModernColor(theme, useThemeColors)))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                
                // Use enhanced letter spacing for modern look
                painter.DrawTextWithLetterSpacing(g, text, modernFont, modernBrush, bounds, 
                    painter.GetLetterSpacing(BeepControlStyle.GradientModern));
            }
        }

        private Color GetDiscordColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor("Foreground");
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            
            // Discord's characteristic text color
            return Color.FromArgb(220, 221, 222); // Discord light gray
        }

        private Color GetModernColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var accentColor = BeepStyling.GetThemeColor("Accent");
                if (accentColor != Color.Empty)
                    return accentColor;
            }
            
            // Modern gradient-friendly color
            return Color.FromArgb(64, 64, 64); // Dark gray
        }
        #endregion

        #region Enhanced Letter Spacing Control
        /// <summary>
        /// Some standard styles benefit from letter spacing
        /// </summary>
        protected override bool ShouldUseLetterSpacing(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.GradientModern => true,    // Modern designs use spacing
                BeepControlStyle.Neumorphism => true,       // Soft designs use wide spacing
                _ => false // Most standard styles use default spacing
            };
        }
        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Paint standard design text with enhanced FontManagement
        /// Replaces the original StandardTextPainter.Paint method
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new StandardDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }

        /// <summary>
        /// Paint text for any remaining style not covered by specialized painters
        /// </summary>
        public static void PaintGeneric(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }
        #endregion
    }
}