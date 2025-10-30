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
    /// Web Framework text painter using FontManagement system
    /// Supports: TailwindCard, Bootstrap, ChakraUI, NotionMinimal, VercelClean, FigmaCard
    /// </summary>
    public class WebFrameworkTextPainter : DesignSystemTextPainter
    {
        #region Design System Properties
        protected override string PrimaryFontFamily => "Inter";
        protected override string FallbackFontFamily => "Segoe UI";

        protected override float GetFontSize(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.TailwindCard => 14f,
                BeepControlStyle.Bootstrap => 14f,
                BeepControlStyle.ChakraUI => 14f,
                BeepControlStyle.NotionMinimal => 14f,
                BeepControlStyle.VercelClean => 13f,     // Slightly smaller
                BeepControlStyle.FigmaCard => 13f,       // Slightly smaller
                _ => 14f
            };
        }

        protected override float GetLetterSpacing(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.TailwindCard => 0f,     // No tracking
                BeepControlStyle.Bootstrap => 0f,        // No tracking
                BeepControlStyle.ChakraUI => 0f,         // No tracking
                BeepControlStyle.NotionMinimal => 0.2f,  // Wider tracking for readability
                BeepControlStyle.VercelClean => 0.2f,    // Wider tracking for clean look
                BeepControlStyle.FigmaCard => 0.2f,      // Wider tracking for design
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

        #region Web Framework Font Management
        /// <summary>
        /// Get web framework font with Inter priority
        /// </summary>
        protected override Font GetFont(BeepControlStyle style, bool isFocused)
        {
            float fontSize = GetFontSize(style);
            FontStyle fontStyle = GetWebFrameworkFontStyle(style, isFocused);
            
            // Try Inter first (common web framework font)
            Font interFont = FontListHelper.GetFont("Inter", fontSize, fontStyle);
            if (interFont != null)
                return interFont;
            
            // Try system UI fonts
            Font systemUIFont = FontListHelper.GetFont("System UI", fontSize, fontStyle);
            if (systemUIFont != null)
                return systemUIFont;
            
            // Fallback chain for web frameworks
            return FontListHelper.GetFontWithFallback("Helvetica", FallbackFontFamily, fontSize, fontStyle);
        }

        /// <summary>
        /// Get font Style specific to web framework conventions
        /// </summary>
        private FontStyle GetWebFrameworkFontStyle(BeepControlStyle style, bool isFocused)
        {
            // Some web frameworks prefer subtle weight changes
            if (style == BeepControlStyle.NotionMinimal || style == BeepControlStyle.VercelClean)
            {
                // Minimal styles prefer subtle weight changes
                return isFocused ? FontStyle.Regular : FontStyle.Regular; // Same weight
            }
            
            return isFocused ? FontStyle.Bold : FontStyle.Regular;
        }
        #endregion

        #region Web Framework Typography Variants
        /// <summary>
        /// Paint web framework heading text
        /// </summary>
        public static void PaintHeading(Graphics g, Rectangle bounds, string text,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new WebFrameworkTextPainter();

            using (Font headingFont = painter.GetHeadingFont(style))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                
                // Apply letter spacing for headings
                float headingSpacing = painter.GetLetterSpacing(style) +0.1f; // Slightly more for headings
                if (headingSpacing >0)
                {
                    painter.DrawTextWithLetterSpacing(g, text, headingFont, PaintersFactory.GetSolidBrush(painter.GetTextColor(style, theme, useThemeColors)), bounds, headingSpacing);
                }
                else
                {
                    g.DrawString(text, headingFont, PaintersFactory.GetSolidBrush(painter.GetTextColor(style, theme, useThemeColors)), bounds, format);
                }
            }
        }

        /// <summary>
        /// Paint web framework body text
        /// </summary>
        public static void PaintBody(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new WebFrameworkTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }

        /// <summary>
        /// Paint web framework small text (for labels, captions)
        /// </summary>
        public static void PaintSmall(Graphics g, Rectangle bounds, string text,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new WebFrameworkTextPainter();

            using (Font smallFont = painter.GetSmallFont(style))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                g.DrawString(text, smallFont, PaintersFactory.GetSolidBrush(painter.GetMutedColor(style, theme, useThemeColors)), bounds, format);
            }
        }

        /// <summary>
        /// Paint code/monospace text within web framework context
        /// </summary>
        public static void PaintCode(Graphics g, Rectangle bounds, string text,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new WebFrameworkTextPainter();

            using (Font codeFont = painter.GetCodeFont(style))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                g.DrawString(text, codeFont, PaintersFactory.GetSolidBrush(painter.GetCodeColor(style, theme, useThemeColors)), bounds, format);
            }
        }

        private Font GetHeadingFont(BeepControlStyle style)
        {
            float headingSize = GetFontSize(style) + 2f;  // Slightly larger for headings
            return FontListHelper.GetFontWithFallback("Inter", FallbackFontFamily, headingSize, FontStyle.Bold);
        }

        private Font GetSmallFont(BeepControlStyle style)
        {
            float smallSize = GetFontSize(style) - 1f;  // Smaller for labels
            return FontListHelper.GetFontWithFallback("Inter", FallbackFontFamily, smallSize, FontStyle.Regular);
        }

        private Font GetCodeFont(BeepControlStyle style)
        {
            float codeSize = GetFontSize(style) - 1f;  // Smaller for code
            // Use embedded Consolas if available
            string consolasPath = BeepFontPaths.Consolas;
            if (!string.IsNullOrEmpty(consolasPath))
            {
                Font embedded = BeepFontPathsExtensions.CreateFontFromResource(consolasPath, codeSize, FontStyle.Regular);
                if (embedded != null)
                    return embedded;
            }
            
            return FontListHelper.GetFontWithFallback("Consolas", "Courier New", codeSize, FontStyle.Regular);
        }

        private Color GetMutedColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color baseColor = GetTextColor(style, theme, useThemeColors);
            // Make muted text more subtle (65% opacity)
            return Color.FromArgb(166, baseColor.R, baseColor.G, baseColor.B);
        }

        private Color GetCodeColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            // Code text often uses a different color
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor("Accent");
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            
            // Default to slightly purple-ish for code
            return Color.FromArgb(88, 110, 117); // Subtle blue-gray
        }
        #endregion

        #region Web Framework Letter Spacing
        /// <summary>
        /// Web frameworks selectively use letter spacing
        /// </summary>
        protected override bool ShouldUseLetterSpacing(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.NotionMinimal => true,  // Notion uses spacing for readability
                BeepControlStyle.VercelClean => true,    // Vercel uses spacing for clean look
                BeepControlStyle.FigmaCard => true,      // Figma uses spacing for design
                _ => false // Bootstrap, Tailwind, Chakra use default spacing
            };
        }
        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Paint web framework text with enhanced FontManagement
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new WebFrameworkTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }

        /// <summary>
        /// Paint text for Bootstrap-Style controls
        /// </summary>
        public static void PaintBootstrap(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, bounds, text, isFocused, BeepControlStyle.Bootstrap, theme, useThemeColors);
        }

        /// <summary>
        /// Paint text for Tailwind-Style controls
        /// </summary>
        public static void PaintTailwind(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, bounds, text, isFocused, BeepControlStyle.TailwindCard, theme, useThemeColors);
        }

        /// <summary>
        /// Paint text for Notion-Style controls
        /// </summary>
        public static void PaintNotion(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, bounds, text, isFocused, BeepControlStyle.NotionMinimal, theme, useThemeColors);
        }
        #endregion
    }
}