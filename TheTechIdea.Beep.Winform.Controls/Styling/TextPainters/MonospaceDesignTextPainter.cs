using System.Drawing;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Monospace Design text painter using FontManagement system
    /// Enhanced version with full monospace family support
    /// Supports: DarkGlow, Terminal
    /// </summary>
    public class MonospaceDesignTextPainter : DesignSystemTextPainter
    {
        #region Design System Properties
        protected override string PrimaryFontFamily => "JetBrains Mono";
        protected override string FallbackFontFamily => "Consolas";

        protected override float GetFontSize(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.DarkGlow => 13f,       // Slightly smaller for glow effect
                BeepControlStyle.Terminal => 12f,       // Terminal-appropriate size
                _ => 13f
            };
        }

        protected override float GetLetterSpacing(BeepControlStyle style)
        {
            // Monospace fonts benefit from wider letter spacing for readability
            return style switch
            {
                BeepControlStyle.DarkGlow => 0.5f,      // Wide spacing for glow effect
                BeepControlStyle.Terminal => 0.3f,      // Moderate spacing for terminal
                _ => 0.5f
            };
        }

        protected override TextRenderingHint GetRenderingHint()
        {
            return TextRenderingHint.ClearTypeGridFit;
        }

        protected override Color GetTextColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            // Monospace styles often use special colors
            if (style == BeepControlStyle.DarkGlow)
            {
                // DarkGlow uses bright/neon colors
                if (useThemeColors && theme != null)
                {
                    var accentColor = BeepStyling.GetThemeColor("Accent");
                    if (accentColor != Color.Empty)
                        return accentColor;
                }
                return Color.FromArgb(0, 255, 127); // Bright green glow
            }
            
            return GetColor(style, StyleColors.GetForeground, "Foreground", theme, useThemeColors);
        }
        #endregion

        #region Monospace Font Management
        /// <summary>
        /// Get monospace font with embedded font priority
        /// </summary>
        protected override Font GetFont(BeepControlStyle style, bool isFocused)
        {
            float fontSize = GetFontSize(style);
            FontStyle fontStyle = isFocused ? FontStyle.Bold : FontStyle.Regular;
            
            // Try embedded Consolas first
            string consolasPath = BeepFontPaths.Consolas;
            if (!string.IsNullOrEmpty(consolasPath))
            {
                Font embedded = BeepFontPathsExtensions.CreateFontFromResource(
                    consolasPath, fontSize, fontStyle);
                if (embedded != null)
                    return embedded;
            }
            
            // Try JetBrains Mono (system install)
            Font jetbrainsFont = FontListHelper.GetFont("JetBrains Mono", fontSize, fontStyle);
            if (jetbrainsFont != null)
                return jetbrainsFont;
            
            // Try other monospace fonts
            Font consolasFont = FontListHelper.GetFont("Consolas", fontSize, fontStyle);
            if (consolasFont != null)
                return consolasFont;
            
            // Ultimate fallback
            return FontListHelper.GetFontWithFallback("Courier New", "Generic Monospace", fontSize, fontStyle);
        }
        #endregion

        #region Terminal-Specific Typography
        /// <summary>
        /// Paint terminal prompt text (with special formatting)
        /// </summary>
        public static void PaintPrompt(Graphics g, Rectangle bounds, string prompt, string command,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MonospaceDesignTextPainter();
            
            using (Font promptFont = painter.GetFont(style, false))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                
                // Draw prompt in different color
                using (SolidBrush promptBrush = new SolidBrush(painter.GetPromptColor(style, theme, useThemeColors)))
                {
                    SizeF promptSize = g.MeasureString(prompt, promptFont);
                    Rectangle promptRect = new Rectangle(bounds.X, bounds.Y, (int)promptSize.Width, bounds.Height);
                    g.DrawString(prompt, promptFont, promptBrush, promptRect);
                    
                    // Draw command in regular color
                    if (!string.IsNullOrEmpty(command))
                    {
                        using (SolidBrush commandBrush = new SolidBrush(painter.GetTextColor(style, theme, useThemeColors)))
                        {
                            Rectangle commandRect = new Rectangle(
                                bounds.X + (int)promptSize.Width + 5, bounds.Y, 
                                bounds.Width - (int)promptSize.Width - 5, bounds.Height);
                            
                            painter.DrawTextWithLetterSpacing(g, command, promptFont, commandBrush, 
                                commandRect, painter.GetLetterSpacing(style));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Paint code/programming text with syntax highlighting colors
        /// </summary>
        public static void PaintCode(Graphics g, Rectangle bounds, string code,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MonospaceDesignTextPainter();
            
            using (Font codeFont = painter.GetFont(style, false))
            using (SolidBrush codeBrush = new SolidBrush(painter.GetCodeColor(style, theme, useThemeColors)))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                
                // Apply wide letter spacing for code readability
                painter.DrawTextWithLetterSpacing(g, code, codeFont, codeBrush, bounds, 
                    painter.GetLetterSpacing(style));
            }
        }

        /// <summary>
        /// Paint monospace text with glow effect (for DarkGlow Style)
        /// </summary>
        public static void PaintGlow(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (style != BeepControlStyle.DarkGlow)
            {
                // Not a glow Style, use regular painting
                Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
                return;
            }

            var painter = new MonospaceDesignTextPainter();
            
            using (Font glowFont = painter.GetFont(style, isFocused))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Color glowColor = painter.GetTextColor(style, theme, useThemeColors);
                
                // Draw glow effect (multiple passes with alpha)
                for (int i = 3; i >= 1; i--)
                {
                    Color shadowColor = Color.FromArgb(40 / i, glowColor.R, glowColor.G, glowColor.B);
                    using (SolidBrush shadowBrush = new SolidBrush(shadowColor))
                    {
                        Rectangle shadowBounds = new Rectangle(bounds.X + i, bounds.Y + i, bounds.Width, bounds.Height);
                        painter.DrawTextWithLetterSpacing(g, text, glowFont, shadowBrush, 
                            shadowBounds, painter.GetLetterSpacing(style));
                    }
                }
                
                // Draw main text
                using (SolidBrush mainBrush = new SolidBrush(glowColor))
                {
                    painter.DrawTextWithLetterSpacing(g, text, glowFont, mainBrush, bounds, 
                        painter.GetLetterSpacing(style));
                }
            }
        }

        private Color GetPromptColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var accentColor = BeepStyling.GetThemeColor("Accent");
                if (accentColor != Color.Empty)
                    return accentColor;
            }
            
            return style switch
            {
                BeepControlStyle.DarkGlow => Color.FromArgb(255, 165, 0),  // Orange
                BeepControlStyle.Terminal => Color.FromArgb(0, 255, 0),    // Green
                _ => Color.FromArgb(128, 128, 128) // Gray
            };
        }

        private Color GetCodeColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var primaryColor = BeepStyling.GetThemeColor("Primary");
                if (primaryColor != Color.Empty)
                    return primaryColor;
            }
            
            return style switch
            {
                BeepControlStyle.DarkGlow => Color.FromArgb(173, 216, 230),  // Light blue
                BeepControlStyle.Terminal => Color.FromArgb(220, 220, 220),  // Light gray
                _ => Color.FromArgb(200, 200, 200)
            };
        }
        #endregion

        #region Enhanced Letter Spacing for Monospace
        /// <summary>
        /// Enhanced letter spacing for monospace readability
        /// </summary>
        protected override void DrawTextWithLetterSpacing(Graphics g, string text, Font font, 
            Brush brush, Rectangle bounds, float letterSpacing)
        {
            if (letterSpacing == 0)
            {
                g.DrawString(text, font, brush, bounds, GetStringFormat());
                return;
            }

            // Monospace-optimized letter spacing
            float y = bounds.Y + (bounds.Height - font.Height) / 2f;
            float x = bounds.X;
            float maxWidth = bounds.Width;
            float currentWidth = 0;

            // Calculate character width (should be consistent for monospace)
            SizeF singleCharSize = g.MeasureString("W", font); // Use wide character for measurement
            float monoCharWidth = singleCharSize.Width;

            foreach (char c in text)
            {
                string charStr = c.ToString();
                
                // Check if character fits
                float totalCharWidth = monoCharWidth + letterSpacing;
                if (currentWidth + totalCharWidth > maxWidth)
                    break;
                
                g.DrawString(charStr, font, brush, x, y);
                x += monoCharWidth + letterSpacing;
                currentWidth += totalCharWidth;
            }
        }

        /// <summary>
        /// Monospace always benefits from letter spacing
        /// </summary>
        protected override bool ShouldUseLetterSpacing(BeepControlStyle style)
        {
            return true; // Monospace always uses letter spacing for readability
        }
        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Paint monospace text with enhanced FontManagement
        /// Replaces the original MonospaceTextPainter.Paint method
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MonospaceDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }

        /// <summary>
        /// Paint terminal-Style text
        /// </summary>
        public static void PaintTerminal(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, bounds, text, isFocused, BeepControlStyle.Terminal, theme, useThemeColors);
        }

        /// <summary>
        /// Paint dark glow-Style text
        /// </summary>
        public static void PaintDarkGlow(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            PaintGlow(g, bounds, text, isFocused, BeepControlStyle.DarkGlow, theme, useThemeColors);
        }
        #endregion
    }
}