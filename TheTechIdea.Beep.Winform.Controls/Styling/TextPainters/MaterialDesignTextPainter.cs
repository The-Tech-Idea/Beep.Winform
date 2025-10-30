using System.Drawing;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Material Design text painter using FontManagement system
    /// Enhanced version of MaterialTextPainter with full Roboto family support
    /// Supports: Material3, MaterialYou
    /// </summary>
    public class MaterialDesignTextPainter : DesignSystemTextPainter
    {
        #region Design System Properties
        protected override string PrimaryFontFamily => "Roboto";
        protected override string FallbackFontFamily => "Segoe UI";

        protected override float GetFontSize(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 =>14f,
                BeepControlStyle.MaterialYou =>14f,
                _ =>14f
            };
        }

        protected override float GetLetterSpacing(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 =>0.1f, // Subtle tracking
                BeepControlStyle.MaterialYou =>0.15f, // Slightly wider for Material You
                _ =>0.1f
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

        #region Material Design Font Weights
        /// <summary>
        /// Get Material Design font with appropriate weight
        /// Supports: Regular, Medium, Bold (Material Design typography scale)
        /// </summary>
        protected override Font GetFont(BeepControlStyle style, bool isFocused)
        {
            float fontSize = GetFontSize(style);
            string fontWeight = GetMaterialWeight(style, isFocused);
            
            // Try embedded Roboto with specific weight
            string embeddedPath = BeepFontPaths.GetFontPath("Roboto", fontWeight);
            if (!string.IsNullOrEmpty(embeddedPath))
            {
                Font embedded = BeepFontPathsExtensions.CreateFontFromResource(
                    embeddedPath, fontSize, GetSystemFontStyle(fontWeight));
                if (embedded != null)
                    return embedded;
            }
            
            // Fallback to system fonts
            FontStyle systemStyle = GetSystemFontStyle(fontWeight);
            return FontListHelper.GetFontWithFallback(PrimaryFontFamily, 
                FallbackFontFamily, fontSize, systemStyle);
        }

        /// <summary>
        /// Get Material Design appropriate font weight
        /// </summary>
        private string GetMaterialWeight(BeepControlStyle style, bool isFocused)
        {
            if (style == BeepControlStyle.MaterialYou)
            {
                // Material You uses Medium weight more prominently
                return isFocused ? "Bold" : "Medium";
            }
            else
            {
                // Material3 uses Regular/Bold
                return isFocused ? "Bold" : "Regular";
            }
        }

        /// <summary>
        /// Convert Material weight to system FontStyle
        /// </summary>
        private FontStyle GetSystemFontStyle(string materialWeight)
        {
            return materialWeight switch
            {
                "Light" => FontStyle.Regular,
                "Regular" => FontStyle.Regular,
                "Medium" => FontStyle.Regular, // System doesn't have Medium, use Regular
                "Bold" => FontStyle.Bold,
                "Black" => FontStyle.Bold, // System doesn't have Black, use Bold
                _ => FontStyle.Regular
            };
        }
        #endregion

        #region Material Design Typography Variants
        /// <summary>
        /// Paint Material Design headline text (larger, bold)
        /// </summary>
        public static void PaintHeadline(Graphics g, Rectangle bounds, string text, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MaterialDesignTextPainter();
            
            // Use larger font for headlines
            using (Font headlineFont = painter.GetHeadlineFont(style))
            {
                var brush = PaintersFactory.GetSolidBrush(painter.GetTextColor(style, theme, useThemeColors));
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                
                g.DrawString(text, headlineFont, brush, bounds, format);
            }
        }

        /// <summary>
        /// Paint Material Design body text (regular size)
        /// </summary>
        public static void PaintBody(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MaterialDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }

        /// <summary>
        /// Paint Material Design caption text (smaller, subtle)
        /// </summary>
        public static void PaintCaption(Graphics g, Rectangle bounds, string text,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MaterialDesignTextPainter();
            
            using (Font captionFont = painter.GetCaptionFont(style))
            {
                var brush = PaintersFactory.GetSolidBrush(painter.GetCaptionColor(style, theme, useThemeColors));
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.DrawString(text, captionFont, brush, bounds, painter.GetStringFormat());
            }
        }

        private Font GetHeadlineFont(BeepControlStyle style)
        {
            float headlineSize = GetFontSize(style) +6f; //20pt for headlines
            string embeddedPath = BeepFontPaths.GetFontPath("Roboto", "Bold");
            
            if (!string.IsNullOrEmpty(embeddedPath))
            {
                Font embedded = BeepFontPathsExtensions.CreateFontFromResource(
                    embeddedPath, headlineSize, FontStyle.Bold);
                if (embedded != null)
                    return embedded;
            }
            
            return FontListHelper.GetFontWithFallback("Roboto", "Segoe UI", headlineSize, FontStyle.Bold);
        }

        private Font GetCaptionFont(BeepControlStyle style)
        {
            float captionSize = GetFontSize(style) -2f; //12pt for captions
            string embeddedPath = BeepFontPaths.GetFontPath("Roboto", "Regular");
            
            if (!string.IsNullOrEmpty(embeddedPath))
            {
                Font embedded = BeepFontPathsExtensions.CreateFontFromResource(
                    embeddedPath, captionSize, FontStyle.Regular);
                if (embedded != null)
                    return embedded;
            }
            
            return FontListHelper.GetFontWithFallback("Roboto", "Segoe UI", captionSize, FontStyle.Regular);
        }

        private Color GetCaptionColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color baseColor = GetTextColor(style, theme, useThemeColors);
            // Make caption text more subtle (70% opacity)
            return Color.FromArgb(179, baseColor.R, baseColor.G, baseColor.B);
        }
        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Paint Material Design text with enhanced FontManagement
        /// Replaces the original MaterialTextPainter.Paint method
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MaterialDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }
        #endregion
    }
}