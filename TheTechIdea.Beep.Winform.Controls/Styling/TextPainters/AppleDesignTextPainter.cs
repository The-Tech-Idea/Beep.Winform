using System.Drawing;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Apple Design text painter using FontManagement system
    /// Enhanced version with full SF Pro family support
    /// Supports: iOS15, MacOSBigSur
    /// </summary>
    public class AppleDesignTextPainter : DesignSystemTextPainter
    {
        #region Design System Properties
        protected override string PrimaryFontFamily => "SF Pro Display";
        protected override string FallbackFontFamily => "Segoe UI";

        protected override float GetFontSize(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.iOS15 => 14f,
                BeepControlStyle.MacOSBigSur => 13f,    // Slightly smaller for macOS
                _ => 14f
            };
        }

        protected override float GetLetterSpacing(BeepControlStyle style)
        {
            // Apple uses negative letter spacing (tighter tracking)
            return style switch
            {
                BeepControlStyle.iOS15 => -0.2f,        // iOS negative tracking
                BeepControlStyle.MacOSBigSur => -0.3f,  // macOS tighter tracking
                _ => -0.2f
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

        #region Apple Design Font Weights
        /// <summary>
        /// Get Apple Design font with appropriate weight
        /// Supports: Light, Regular, Medium, Semibold, Bold (Apple typography scale)
        /// </summary>
        protected override Font GetFont(BeepControlStyle style, bool isFocused)
        {
            float fontSize = GetFontSize(style);
            string fontWeight = GetAppleWeight(style, isFocused);
            
            // Try SF Pro family first (system font on macOS/iOS)
            Font systemFont = FontListHelper.GetFont("SF Pro Display", fontSize, GetSystemFontStyle(fontWeight));
            if (systemFont != null)
                return systemFont;
            
            // Try SF Pro Text as alternative
            systemFont = FontListHelper.GetFont("SF Pro Text", fontSize, GetSystemFontStyle(fontWeight));
            if (systemFont != null)
                return systemFont;
            
            // Fallback to system fonts with similar characteristics
            FontStyle systemStyle = GetSystemFontStyle(fontWeight);
            return FontListHelper.GetFontWithFallback("Helvetica Neue", FallbackFontFamily, fontSize, systemStyle);
        }

        /// <summary>
        /// Get Apple Design appropriate font weight
        /// </summary>
        private string GetAppleWeight(BeepControlStyle style, bool isFocused)
        {
            if (style == BeepControlStyle.MacOSBigSur)
            {
                // macOS Big Sur uses Medium and Semibold
                return isFocused ? "Semibold" : "Medium";
            }
            else
            {
                // iOS 15 uses Regular and Semibold
                return isFocused ? "Semibold" : "Regular";
            }
        }

        /// <summary>
        /// Convert Apple weight to system FontStyle
        /// </summary>
        private FontStyle GetSystemFontStyle(string appleWeight)
        {
            return appleWeight switch
            {
                "Light" => FontStyle.Regular,
                "Regular" => FontStyle.Regular,
                "Medium" => FontStyle.Regular,      // System doesn't have Medium
                "Semibold" => FontStyle.Bold,       // Map Semibold to Bold
                "Bold" => FontStyle.Bold,
                "Heavy" => FontStyle.Bold,
                _ => FontStyle.Regular
            };
        }
        #endregion

        #region Apple Design Typography Variants
        /// <summary>
        /// Paint Apple Design large title (iOS navigation bars, macOS window titles)
        /// </summary>
        public static void PaintLargeTitle(Graphics g, Rectangle bounds, string text,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new AppleDesignTextPainter();
            
            using (Font titleFont = painter.GetLargeTitleFont(style))
            using (SolidBrush brush = new SolidBrush(painter.GetTextColor(style, theme, useThemeColors)))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                
                // Apply negative letter spacing for large titles
                painter.DrawTextWithLetterSpacing(g, text, titleFont, brush, bounds, -0.4f);
            }
        }

        /// <summary>
        /// Paint Apple Design body text with system font
        /// </summary>
        public static void PaintBody(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new AppleDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }

        /// <summary>
        /// Paint Apple Design footnote text (smaller, secondary)
        /// </summary>
        public static void PaintFootnote(Graphics g, Rectangle bounds, string text,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new AppleDesignTextPainter();
            
            using (Font footnoteFont = painter.GetFootnoteFont(style))
            using (SolidBrush brush = new SolidBrush(painter.GetSecondaryColor(style, theme, useThemeColors)))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                
                g.DrawString(text, footnoteFont, brush, bounds, format);
            }
        }

        private Font GetLargeTitleFont(BeepControlStyle style)
        {
            float titleSize = GetFontSize(style) + 10f;  // Much larger for titles
            return FontListHelper.GetFontWithFallback("SF Pro Display", FallbackFontFamily, titleSize, FontStyle.Bold);
        }

        private Font GetFootnoteFont(BeepControlStyle style)
        {
            float footnoteSize = GetFontSize(style) - 1f;  // Smaller for footnotes
            return FontListHelper.GetFontWithFallback("SF Pro Text", FallbackFontFamily, footnoteSize, FontStyle.Regular);
        }

        private Color GetSecondaryColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color baseColor = GetTextColor(style, theme, useThemeColors);
            // Make secondary text more subtle (60% opacity)
            return Color.FromArgb(153, baseColor.R, baseColor.G, baseColor.B);
        }
        #endregion

        #region Apple Design Letter Spacing
        /// <summary>
        /// Override letter spacing to use Apple's precise negative tracking
        /// </summary>
        protected override void DrawTextWithLetterSpacing(Graphics g, string text, Font font, 
            Brush brush, Rectangle bounds, float letterSpacing)
        {
            if (letterSpacing >= 0)
            {
                // No negative spacing needed, use base implementation
                base.DrawTextWithLetterSpacing(g, text, font, brush, bounds, letterSpacing);
                return;
            }

            // Apple negative tracking implementation
            float y = bounds.Y + (bounds.Height - font.Height) / 2f;
            float x = bounds.X;
            float maxWidth = bounds.Width;
            float currentWidth = 0;

            // For negative spacing, we need to be more precise
            for (int i = 0; i < text.Length; i++)
            {
                string charStr = text[i].ToString();
                SizeF charSize = g.MeasureString(charStr, font);
                
                // Check if character fits
                float charWidth = charSize.Width + (i < text.Length - 1 ? letterSpacing : 0);
                if (currentWidth + charWidth > maxWidth)
                    break;
                
                g.DrawString(charStr, font, brush, x, y);
                x += charSize.Width + letterSpacing;  // Negative spacing makes text tighter
                currentWidth += charWidth;
            }
        }

        /// <summary>
        /// Apple design benefits from precise negative letter spacing
        /// </summary>
        protected override bool ShouldUseLetterSpacing(BeepControlStyle style)
        {
            return true; // Apple always uses letter spacing
        }
        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Paint Apple Design text with enhanced FontManagement
        /// Replaces the original AppleTextPainter.Paint method
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new AppleDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }
        #endregion
    }
}