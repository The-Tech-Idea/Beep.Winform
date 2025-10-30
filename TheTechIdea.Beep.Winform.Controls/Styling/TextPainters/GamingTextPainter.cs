using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Specialized text painter for gaming interfaces
    /// Features RGB glow effects, tech fonts, and gaming-specific typography
    /// </summary>
    public static class GamingTextPainter
    {
        #region Gaming Font Families

        private static readonly string[] GamingFonts = {
            "Orbitron",           // Futuristic sci-fi font
            "Exo 2",              // Modern tech font
            "Rajdhani",           // Clean tech font
            "JetBrains Mono",     // Code/terminal font
            "Consolas",           // Fallback monospace
            "Segoe UI",           // System fallback
            "Arial"               // Final fallback
        };

        private static readonly string[] CodeFonts = {
            "JetBrains Mono",
            "Fira Code",
            "Cascadia Code",
            "Consolas",
            "Courier New"
        };

        #endregion

        #region Main Paint Methods

        /// <summary>
        /// Paint gaming text with RGB glow effects and tech styling
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Configure graphics for high quality
            ConfigureGraphicsQuality(g);

            // Get gaming colors
            Color textColor = GetGamingTextColor(style, theme, isFocused, useThemeColors);
            Color glowColor = GetGamingGlowColor(style, theme);

            // Get gaming font
            Font font = GetGamingFont(bounds.Height, style);

            try
            {
                // Apply gaming-specific text rendering
                switch (style)
                {
                    case BeepControlStyle.DarkGlow:
                        PaintDarkGlowText(g, bounds, text, font, textColor, glowColor, isFocused);
                        break;

                    case BeepControlStyle.NeonGlow:
                        PaintNeonGlowText(g, bounds, text, font, textColor, glowColor, isFocused);
                        break;

                    case BeepControlStyle.Terminal:
                        PaintTerminalStyleText(g, bounds, text, font, textColor, isFocused);
                        break;

                    default:
                        PaintTechStyleText(g, bounds, text, font, textColor, glowColor, isFocused);
                        break;
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint gaming text with RGB rainbow glow effect
        /// </summary>
        public static void PaintRGBGlow(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <=0 || bounds.Height <=0)
                return;

            ConfigureGraphicsQuality(g);

            Font font = GetGamingFont(bounds.Height, style);
            Color baseColor = GetGamingTextColor(style, theme, isFocused, useThemeColors);

            try
            {
                // Create RGB gradient glow
                using (var path = new GraphicsPath())
                {
                    path.AddString(text, font.FontFamily, (int)font.Style, font.Size, bounds, StringFormat.GenericTypographic);

                    // Multiple glow layers with RGB colors
                    Color[] rgbColors = { Color.Red, Color.Lime, Color.Blue, Color.Magenta, Color.Cyan, Color.Yellow };

                    for (int i =0; i < rgbColors.Length; i++)
                    {
                        var pen = PaintersFactory.GetPen(Color.FromArgb(30, rgbColors[i]),8 - i);
                        g.DrawPath(pen, path);
                    }

                    // Main text
                    var brush = PaintersFactory.GetSolidBrush(baseColor);
                    g.FillPath(brush, path);
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint gaming HUD-Style text with scanlines effect
        /// </summary>
        public static void PaintHUDStyle(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <=0 || bounds.Height <=0)
                return;

            ConfigureGraphicsQuality(g);

            Font font = GetCodeFont(bounds.Height);
            Color hudColor = isFocused ? Color.FromArgb(0,255,150) : Color.FromArgb(0,200,100);

            try
            {
                // Paint text with HUD green
                var brush = PaintersFactory.GetSolidBrush(hudColor);
                g.DrawString(text, font, brush, bounds, GetCenteredStringFormat());

                // Add scanlines effect
                if (isFocused)
                {
                    var scanPen = PaintersFactory.GetPen(Color.FromArgb(30, hudColor),1);
                    for (int y = bounds.Top; y < bounds.Bottom; y +=3)
                    {
                        g.DrawLine(scanPen, bounds.Left, y, bounds.Right, y);
                    }
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        #endregion

        #region Gaming Style Text Rendering

        private static void PaintDarkGlowText(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, Color glowColor, bool isFocused)
        {
            // Outer glow
            for (int i =4; i >=1; i--)
            {
                Color glowLayer = Color.FromArgb(20 * i, glowColor);
                var brush = PaintersFactory.GetSolidBrush(glowLayer);
                var glowBounds = new Rectangle(bounds.X - i, bounds.Y - i, bounds.Width + (i *2), bounds.Height + (i *2));
                g.DrawString(text, font, brush, glowBounds, GetCenteredStringFormat());
            }

            // Main text
            var mainBrush = PaintersFactory.GetSolidBrush(textColor);
            g.DrawString(text, font, mainBrush, bounds, GetCenteredStringFormat());

            // Inner highlight if focused
            if (isFocused)
            {
                Color highlight = Color.FromArgb(100, Color.White);
                var highlightBrush = PaintersFactory.GetSolidBrush(highlight);
                var innerBounds = new Rectangle(bounds.X +1, bounds.Y +1, bounds.Width -2, bounds.Height -2);
                g.DrawString(text, font, highlightBrush, innerBounds, GetCenteredStringFormat());
            }
        }

        private static void PaintNeonGlowText(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, Color glowColor, bool isFocused)
        {
            // Create neon effect with multiple glow layers
            using (var path = new GraphicsPath())
            {
                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, bounds, StringFormat.GenericTypographic);

                // Outer neon glow
                for (int i =6; i >=1; i--)
                {
                    int alpha = Math.Max(10,60 - (i *8));
                    var pen = PaintersFactory.GetPen(Color.FromArgb(alpha, glowColor), i *2);
                    g.DrawPath(pen, path);
                }

                // Main neon color
                var brush = PaintersFactory.GetSolidBrush(textColor);
                g.FillPath(brush, path);

                // Inner bright core
                Color coreColor = Color.FromArgb(200, Color.White);
                var coreBrush = PaintersFactory.GetSolidBrush(coreColor);
                using (var corePath = new GraphicsPath())
                {
                    var coreSize = font.Size *0.8f;
                    var coreFont = new Font(font.FontFamily, coreSize, font.Style);
                    corePath.AddString(text, coreFont.FontFamily, (int)coreFont.Style, coreFont.Size, bounds, StringFormat.GenericTypographic);
                    g.FillPath(coreBrush, corePath);
                    coreFont.Dispose();
                }
            }
        }

        private static void PaintTerminalStyleText(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Terminal green with slight flicker effect
            Color terminalGreen = isFocused ? Color.FromArgb(0,255,100) : Color.FromArgb(0,200,80);

            var brush = PaintersFactory.GetSolidBrush(terminalGreen);
            g.DrawString(text, font, brush, bounds, GetMonospaceStringFormat());

            // Add cursor if focused
            if (isFocused)
            {
                var textSize = g.MeasureString(text, font);
                var cursorX = bounds.X + textSize.Width +2;
                var cursorY = bounds.Y;
                var cursorHeight = bounds.Height -4;

                var cursorBrush = PaintersFactory.GetSolidBrush(terminalGreen);
                g.FillRectangle(cursorBrush, cursorX, cursorY +2,2, cursorHeight);
            }
        }

        private static void PaintTechStyleText(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, Color glowColor, bool isFocused)
        {
            // Subtle tech glow
            if (isFocused)
            {
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(50, glowColor));
                var glowBounds = new Rectangle(bounds.X -1, bounds.Y -1, bounds.Width +2, bounds.Height +2);
                g.DrawString(text, font, glowBrush, glowBounds, GetCenteredStringFormat());
            }

            // Main text with letter spacing
            DrawTextWithLetterSpacing(g, text, font, textColor, bounds,0.8f);
        }

        #endregion

        #region Font Management

        private static Font GetGamingFont(int height, BeepControlStyle style)
        {
            float fontSize = Math.Max(8, height * 0.6f);

            // Style-specific font selection
            string[] fontFamily = style switch
            {
                BeepControlStyle.Terminal => CodeFonts,
                BeepControlStyle.DarkGlow => GamingFonts,
                BeepControlStyle.NeonGlow => GamingFonts,
                _ => GamingFonts
            };

            // Try embedded fonts first
            Font embeddedFont = BeepFontManager.GetEmbeddedFont(fontFamily[0], fontSize, FontStyle.Regular);
            if (embeddedFont != null)
                return embeddedFont;

            // Try system fonts
            foreach (string font in fontFamily)
            {
                try
                {
                    return new Font(font, fontSize, FontStyle.Regular, GraphicsUnit.Point);
                }
                catch
                {
                    continue;
                }
            }

            return new Font("Arial", fontSize, FontStyle.Regular, GraphicsUnit.Point);
        }

        private static Font GetCodeFont(int height)
        {
            float fontSize = Math.Max(8, height * 0.55f);

            // Try embedded monospace fonts first
            Font embeddedFont = BeepFontManager.GetEmbeddedFont("JetBrains Mono", fontSize, FontStyle.Regular);
            if (embeddedFont != null)
                return embeddedFont;

            foreach (string font in CodeFonts)
            {
                try
                {
                    return new Font(font, fontSize, FontStyle.Regular, GraphicsUnit.Point);
                }
                catch
                {
                    continue;
                }
            }

            return new Font("Consolas", fontSize, FontStyle.Regular, GraphicsUnit.Point);
        }

        #endregion

        #region Gaming Colors

        private static Color GetGamingTextColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return isFocused ? theme.AccentColor : theme.SecondaryTextColor;
            }

            return style switch
            {
                BeepControlStyle.DarkGlow => isFocused ? Color.FromArgb(255, 100, 255) : Color.FromArgb(200, 80, 200),
                BeepControlStyle.NeonGlow => isFocused ? Color.FromArgb(0, 255, 255) : Color.FromArgb(0, 200, 200),
                BeepControlStyle.Terminal => isFocused ? Color.FromArgb(0, 255, 100) : Color.FromArgb(0, 200, 80),
                _ => isFocused ? Color.FromArgb(100, 200, 255) : Color.FromArgb(80, 160, 200)
            };
        }

        private static Color GetGamingGlowColor(BeepControlStyle style, IBeepTheme theme)
        {
            return style switch
            {
                BeepControlStyle.DarkGlow => Color.FromArgb(255, 0, 255),     // Magenta
                BeepControlStyle.NeonGlow => Color.FromArgb(0, 255, 255),     // Cyan
                BeepControlStyle.Terminal => Color.FromArgb(0, 255, 0),       // Green
                _ => Color.FromArgb(100, 150, 255)                            // Blue
            };
        }

        #endregion

        #region Helper Methods

        private static void ConfigureGraphicsQuality(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        private static StringFormat GetCenteredStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        private static StringFormat GetMonospaceStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces
            };
        }

        private static void DrawTextWithLetterSpacing(Graphics g, string text, Font font, Color color, Rectangle bounds, float letterSpacing)
        {
            if (letterSpacing == 0)
            {
                using (var brush = new SolidBrush(color))
                {
                    g.DrawString(text, font, brush, bounds, GetCenteredStringFormat());
                }
                return;
            }

            // Calculate positions for each character with letter spacing
            using (var brush = new SolidBrush(color))
            {
                float totalWidth = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    var charSize = g.MeasureString(text[i].ToString(), font);
                    totalWidth += charSize.Width + (i < text.Length - 1 ? letterSpacing : 0);
                }

                float startX = bounds.X + (bounds.Width - totalWidth) / 2;
                float y = bounds.Y + (bounds.Height - font.Height) / 2;

                float currentX = startX;
                for (int i = 0; i < text.Length; i++)
                {
                    string character = text[i].ToString();
                    g.DrawString(character, font, brush, currentX, y);

                    var charSize = g.MeasureString(character, font);
                    currentX += charSize.Width + letterSpacing;
                }
            }
        }

        #endregion

        #region Gaming-Specific Typography Variants

        /// <summary>
        /// Paint gaming UI element text (buttons, menus, etc.)
        /// </summary>
        public static void PaintUIElement(Graphics g, Rectangle bounds, string text, bool isActive,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            Color textColor = isActive ? 
                Color.FromArgb(255, 255, 255) : 
                Color.FromArgb(180, 180, 180);
            
            Color glowColor = GetGamingGlowColor(style, theme);
            Font font = GetGamingFont(bounds.Height, style);

            try
            {
                if (isActive)
                {
                    PaintTechStyleText(g, bounds, text, font, textColor, glowColor, true);
                }
                else
                {
                    using (var brush = new SolidBrush(textColor))
                    {
                        g.DrawString(text, font, brush, bounds, GetCenteredStringFormat());
                    }
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint gaming stat or score text with emphasis
        /// </summary>
        public static void PaintStatText(Graphics g, Rectangle bounds, string text, bool isHighlighted,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            Font font = GetCodeFont(bounds.Height);
            Color statColor = isHighlighted ? 
                Color.FromArgb(255, 215, 0) :  // Gold for highlighted stats
                Color.FromArgb(200, 200, 200); // Gray for normal stats

            try
            {
                if (isHighlighted)
                {
                    // Add glow for highlighted stats
                    using (var glowBrush = new SolidBrush(Color.FromArgb(80, statColor)))
                    {
                        var glowBounds = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 2);
                        g.DrawString(text, font, glowBrush, glowBounds, GetMonospaceStringFormat());
                    }
                }

                using (var brush = new SolidBrush(statColor))
                {
                    g.DrawString(text, font, brush, bounds, GetMonospaceStringFormat());
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        #endregion
    }
}