using System.Drawing;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Base class for design system-specific text painters
    /// Provides FontManagement integration and common typography features
    /// </summary>
    public abstract class DesignSystemTextPainter
    {
        #region Abstract Properties
        protected abstract string PrimaryFontFamily { get; }
        protected abstract string FallbackFontFamily { get; }
        protected abstract float GetFontSize(BeepControlStyle style);
        protected abstract float GetLetterSpacing(BeepControlStyle style);
        protected abstract TextRenderingHint GetRenderingHint();
        protected abstract Color GetTextColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors);
        #endregion

        #region Font Management Integration
        /// <summary>
        /// Get font using FontManagement system with embedded font priority
        /// </summary>
        protected virtual Font GetFont(BeepControlStyle style, bool isFocused)
        {
            FontStyle fontStyle = isFocused ? GetActiveFontStyle(style) : GetFontStyle(style);
            float fontSize = GetFontSize(style);
            
            // Try embedded font first for consistency
            string embeddedPath = BeepFontPaths.GetFontPath(PrimaryFontFamily, 
                fontStyle.HasFlag(FontStyle.Bold) ? "Bold" : "Regular");
            
            if (!string.IsNullOrEmpty(embeddedPath))
            {
                Font embedded = BeepFontPathsExtensions.CreateFontFromResource(
                    embeddedPath, fontSize, fontStyle);
                if (embedded != null) 
                    return embedded;
            }
            
            // Fallback to system fonts via FontListHelper
            return FontListHelper.GetFontWithFallback(PrimaryFontFamily, 
                FallbackFontFamily, fontSize, fontStyle);
        }

        /// <summary>
        /// Get font style for regular state - can be overridden per design system
        /// </summary>
        protected virtual FontStyle GetFontStyle(BeepControlStyle style)
        {
            return StyleTypography.GetFontStyle(style);
        }

        /// <summary>
        /// Get font style for focused/active state - can be overridden per design system
        /// </summary>
        protected virtual FontStyle GetActiveFontStyle(BeepControlStyle style)
        {
            return StyleTypography.GetActiveFontStyle(style);
        }
        #endregion

        #region Text Rendering
        /// <summary>
        /// Main paint method - handles all text rendering with advanced features
        /// </summary>
        public virtual void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            Color textColor = GetTextColor(style, theme, useThemeColors);
            
            using (Font font = GetFont(style, isFocused))
            using (SolidBrush brush = new SolidBrush(textColor))
            {
                g.TextRenderingHint = GetRenderingHint();
                
                // Apply letter spacing if supported
                float letterSpacing = GetLetterSpacing(style);
                if (letterSpacing != 0 && ShouldUseLetterSpacing(style))
                {
                    DrawTextWithLetterSpacing(g, text, font, brush, bounds, letterSpacing);
                }
                else
                {
                    g.DrawString(text, font, brush, bounds, GetStringFormat());
                }
            }
        }

        /// <summary>
        /// Draw text with custom letter spacing
        /// </summary>
        protected virtual void DrawTextWithLetterSpacing(Graphics g, string text, Font font, 
            Brush brush, Rectangle bounds, float letterSpacing)
        {
            if (letterSpacing == 0)
            {
                g.DrawString(text, font, brush, bounds, GetStringFormat());
                return;
            }

            // Calculate baseline position
            float y = bounds.Y + (bounds.Height - font.Height) / 2f;
            float x = bounds.X;
            float maxWidth = bounds.Width;
            float currentWidth = 0;

            // Draw each character with spacing
            foreach (char c in text)
            {
                string charStr = c.ToString();
                SizeF charSize = g.MeasureString(charStr, font, bounds.Size, GetStringFormat());
                
                // Check if character fits in bounds
                if (currentWidth + charSize.Width > maxWidth)
                    break;
                
                g.DrawString(charStr, font, brush, x, y);
                x += charSize.Width + letterSpacing;
                currentWidth += charSize.Width + letterSpacing;
            }
        }

        /// <summary>
        /// Determine if letter spacing should be applied for this style
        /// </summary>
        protected virtual bool ShouldUseLetterSpacing(BeepControlStyle style)
        {
            // Most design systems benefit from letter spacing
            return true;
        }

        /// <summary>
        /// Get string format for text alignment and trimming
        /// </summary>
        protected virtual StringFormat GetStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }
        #endregion

        #region Color Management
        /// <summary>
        /// Get text color with theme override support
        /// </summary>
        protected Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, 
            string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
        #endregion

        #region Static Factory Method
        /// <summary>
        /// Create appropriate design system text painter for the given style
        /// </summary>
        public static DesignSystemTextPainter CreatePainter(BeepControlStyle style)
        {
            return style switch
            {
                // Material Design family
                BeepControlStyle.Material3 or BeepControlStyle.MaterialYou 
                    => new MaterialDesignTextPainter(),
                
                // Apple Design family  
                BeepControlStyle.iOS15 or BeepControlStyle.MacOSBigSur 
                    => new AppleDesignTextPainter(),
                
                // Microsoft Design family
                BeepControlStyle.Fluent2 or BeepControlStyle.Windows11Mica 
                    => new MicrosoftDesignTextPainter(),
                
                // Web Framework family
                BeepControlStyle.TailwindCard or BeepControlStyle.Bootstrap or 
                BeepControlStyle.ChakraUI or BeepControlStyle.NotionMinimal or 
                BeepControlStyle.VercelClean or BeepControlStyle.FigmaCard 
                    => new WebFrameworkTextPainter(),
                
                // Monospace family
                BeepControlStyle.DarkGlow or BeepControlStyle.Terminal 
                    => new MonospaceDesignTextPainter(),
                
                // Default to standard painter
                _ => new StandardDesignTextPainter()
            };
        }
        #endregion
    }
}