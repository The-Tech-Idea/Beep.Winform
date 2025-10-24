using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Specialized text painter for terminal and console interfaces
    /// Features monospace fonts, cursor effects, and terminal-specific typography
    /// </summary>
    public static class TerminalTextPainter
    {
        #region Terminal Font Families

        private static readonly string[] TerminalFonts = {
            "JetBrains Mono",     // Modern programming font
            "Fira Code",          // Popular code font with ligatures
            "Cascadia Code",      // Microsoft's modern terminal font
            "Cascadia Mono",      // Non-ligature version
            "Consolas",           // Classic Windows terminal font
            "DejaVu Sans Mono",   // Cross-platform monospace
            "Liberation Mono",    // Open source monospace
            "Courier New",        // Classic monospace fallback
            "monospace"           // Generic monospace
        };

        private static readonly string[] ClassicTerminalFonts = {
            "Perfect DOS VGA 437", // Retro DOS font
            "MS Gothic",          // Classic console font
            "Terminal",           // Windows terminal font
            "Fixedsys",          // Old Windows fixed font
            "Courier New"         // Fallback
        };

        #endregion

        #region Terminal Colors

        private static readonly Color[] TerminalGreenVariants = {
            Color.FromArgb(0, 255, 0),    // Bright green
            Color.FromArgb(0, 255, 100),  // Green-cyan
            Color.FromArgb(50, 255, 50),  // Soft green
            Color.FromArgb(0, 200, 0),    // Medium green
            Color.FromArgb(0, 150, 0)     // Dark green
        };

        private static readonly Color[] TerminalAmberVariants = {
            Color.FromArgb(255, 191, 0),  // Classic amber
            Color.FromArgb(255, 215, 0),  // Gold amber
            Color.FromArgb(255, 165, 0),  // Orange amber
            Color.FromArgb(255, 140, 0)   // Dark amber
        };

        #endregion

        #region Main Paint Methods

        /// <summary>
        /// Paint terminal text with monospace font and terminal styling
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Configure graphics for crisp monospace rendering
            ConfigureTerminalGraphics(g);

            // Get terminal colors and font
            Color textColor = GetTerminalTextColor(style, theme, isFocused, useThemeColors);
            Color backgroundColor = GetTerminalBackgroundColor(style, theme, useThemeColors);
            Font font = GetTerminalFont(bounds.Height, style);

            try
            {
                // Apply terminal-specific rendering
                switch (style)
                {
                    case BeepControlStyle.Terminal:
                        PaintClassicTerminal(g, bounds, text, font, textColor, backgroundColor, isFocused);
                        break;

                    case BeepControlStyle.DarkGlow:
                        PaintModernTerminal(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Retro:
                        PaintRetroTerminal(g, bounds, text, font, textColor, backgroundColor, isFocused);
                        break;

                    default:
                        PaintStandardTerminal(g, bounds, text, font, textColor, isFocused);
                        break;
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint terminal text with typing animation effect
        /// </summary>
        public static void PaintWithTypewriter(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, int visibleChars = -1)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureTerminalGraphics(g);

            Color textColor = GetTerminalTextColor(style, theme, isFocused, useThemeColors);
            Font font = GetTerminalFont(bounds.Height, style);

            try
            {
                // Show only specified number of characters (for typewriter effect)
                string displayText = visibleChars >= 0 && visibleChars < text.Length ? 
                    text.Substring(0, visibleChars) : text;

                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(displayText, font, brush, bounds, GetTerminalStringFormat());
                }

                // Add blinking cursor after last visible character
                if (visibleChars >= 0 && visibleChars <= text.Length && isFocused)
                {
                    PaintTerminalCursor(g, bounds, displayText, font, textColor);
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint terminal prompt with command styling
        /// </summary>
        public static void PaintPrompt(Graphics g, Rectangle bounds, string prompt, string command, 
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureTerminalGraphics(g);

            Color promptColor = GetTerminalPromptColor(style, theme, useThemeColors);
            Color commandColor = GetTerminalTextColor(style, theme, isFocused, useThemeColors);
            Font font = GetTerminalFont(bounds.Height, style);

            try
            {
                using (var promptBrush = new SolidBrush(promptColor))
                using (var commandBrush = new SolidBrush(commandColor))
                {
                    var format = GetTerminalStringFormat();
                    
                    // Measure prompt width
                    var promptSize = g.MeasureString(prompt, font, bounds.Width, format);
                    
                    // Draw prompt
                    var promptBounds = new Rectangle(bounds.X, bounds.Y, (int)promptSize.Width, bounds.Height);
                    g.DrawString(prompt, font, promptBrush, promptBounds, format);
                    
                    // Draw command after prompt
                    if (!string.IsNullOrEmpty(command))
                    {
                        var commandBounds = new Rectangle(
                            bounds.X + (int)promptSize.Width, 
                            bounds.Y, 
                            bounds.Width - (int)promptSize.Width, 
                            bounds.Height);
                        g.DrawString(command, font, commandBrush, commandBounds, format);
                        
                        // Add cursor at end of command if focused
                        if (isFocused)
                        {
                            PaintTerminalCursor(g, commandBounds, command, font, commandColor);
                        }
                    }
                    else if (isFocused)
                    {
                        // Show cursor after prompt if no command
                        PaintTerminalCursor(g, promptBounds, prompt, font, promptColor);
                    }
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        #endregion

        #region Terminal Style Rendering

        private static void PaintClassicTerminal(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, Color backgroundColor, bool isFocused)
        {
            // Fill background for classic terminal look
            using (var backBrush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Draw text
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetTerminalStringFormat());
            }

            // Add scanlines effect for authenticity
            if (isFocused)
            {
                PaintScanlines(g, bounds, textColor);
            }
        }

        private static void PaintModernTerminal(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Modern terminal with subtle glow
            if (isFocused)
            {
                // Subtle glow effect
                using (var glowBrush = new SolidBrush(Color.FromArgb(30, textColor)))
                {
                    for (int i = 1; i < 3; i++)
                    {
                        var glowBounds = new Rectangle(bounds.X - i, bounds.Y - i, bounds.Width + (i * 2), bounds.Height + (i * 2));
                        g.DrawString(text, font, glowBrush, glowBounds, GetTerminalStringFormat());
                    }
                }
            }

            // Main text
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetTerminalStringFormat());
            }
        }

        private static void PaintRetroTerminal(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, Color backgroundColor, bool isFocused)
        {
            // Retro terminal with pixelated effect
            using (var backBrush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Use pixelated rendering for retro feel
            var oldMode = g.SmoothingMode;
            var oldHint = g.TextRenderingHint;
            
            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            try
            {
                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(text, font, brush, bounds, GetTerminalStringFormat());
                }

                // Add CRT flicker effect
                if (isFocused)
                {
                    using (var flickerBrush = new SolidBrush(Color.FromArgb(20, Color.White)))
                    {
                        g.FillRectangle(flickerBrush, bounds);
                    }
                }
            }
            finally
            {
                g.SmoothingMode = oldMode;
                g.TextRenderingHint = oldHint;
            }
        }

        private static void PaintStandardTerminal(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetTerminalStringFormat());
            }
        }

        #endregion

        #region Terminal Effects

        private static void PaintTerminalCursor(Graphics g, Rectangle bounds, string text, Font font, Color textColor)
        {
            // Calculate cursor position at end of text
            var textSize = g.MeasureString(text, font, bounds.Width, GetTerminalStringFormat());
            var cursorX = bounds.X + textSize.Width;
            var cursorY = bounds.Y + 2;
            var cursorHeight = bounds.Height - 4;

            // Draw blinking cursor (solid block Style)
            using (var cursorBrush = new SolidBrush(Color.FromArgb(180, textColor)))
            {
                g.FillRectangle(cursorBrush, cursorX, cursorY, font.Height / 2, cursorHeight);
            }
        }

        private static void PaintScanlines(Graphics g, Rectangle bounds, Color baseColor)
        {
            using (var scanPen = new Pen(Color.FromArgb(20, baseColor), 1))
            {
                for (int y = bounds.Top + 1; y < bounds.Bottom; y += 2)
                {
                    g.DrawLine(scanPen, bounds.Left, y, bounds.Right, y);
                }
            }
        }

        #endregion

        #region Font Management

        private static Font GetTerminalFont(int height, BeepControlStyle style)
        {
            float fontSize = Math.Max(8, height * 0.55f);
            
            string[] fontFamily = style switch
            {
                BeepControlStyle.Retro => ClassicTerminalFonts,
                _ => TerminalFonts
            };

            // Try embedded fonts first
            foreach (string fontName in fontFamily)
            {
                Font embeddedFont = BeepFontManager.GetEmbeddedFont(fontName, fontSize, FontStyle.Regular);
                if (embeddedFont != null)
                    return embeddedFont;
            }

            // Try system fonts
            foreach (string fontName in fontFamily)
            {
                try
                {
                    return new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Point);
                }
                catch
                {
                    continue;
                }
            }

            return new Font("Consolas", fontSize, FontStyle.Regular, GraphicsUnit.Point);
        }

        #endregion

        #region Terminal Colors

        private static Color GetTerminalTextColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return isFocused ? theme.AccentColor : theme.SecondaryTextColor;
            }

            return style switch
            {
                BeepControlStyle.Terminal => isFocused ? TerminalGreenVariants[0] : TerminalGreenVariants[3],
                BeepControlStyle.Retro => isFocused ? TerminalAmberVariants[0] : TerminalAmberVariants[2],
                BeepControlStyle.DarkGlow => isFocused ? Color.FromArgb(0, 255, 150) : Color.FromArgb(0, 200, 120),
                _ => isFocused ? Color.FromArgb(220, 220, 220) : Color.FromArgb(180, 180, 180)
            };
        }

        private static Color GetTerminalBackgroundColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return theme.BackgroundColor;
            }

            return style switch
            {
                BeepControlStyle.Terminal => Color.FromArgb(0, 0, 0),      // Pure black
                BeepControlStyle.Retro => Color.FromArgb(20, 20, 0), // Dark amber tint
                BeepControlStyle.DarkGlow => Color.FromArgb(5, 5, 10),     // Very dark blue
                _ => Color.FromArgb(12, 12, 12)                            // Very dark gray
            };
        }

        private static Color GetTerminalPromptColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return theme.AccentColor;
            }

            return style switch
            {
                BeepControlStyle.Terminal => Color.FromArgb(0, 200, 255),  // Cyan
                BeepControlStyle.Retro => Color.FromArgb(255, 215, 0), // Gold
                BeepControlStyle.DarkGlow => Color.FromArgb(255, 100, 255),   // Magenta
                _ => Color.FromArgb(100, 200, 100)                            // Light green
            };
        }

        #endregion

        #region Helper Methods

        private static void ConfigureTerminalGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.None;  // Crisp monospace rendering
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        private static StringFormat GetTerminalStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces
            };
        }

        #endregion

        #region Terminal-Specific Typography Variants

        /// <summary>
        /// Paint command line text with syntax highlighting
        /// </summary>
        public static void PaintCommandLine(Graphics g, Rectangle bounds, string command, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(command)) return;

            Font font = GetTerminalFont(bounds.Height, style);
            
            try
            {
                // Simple syntax highlighting for common commands
                if (command.StartsWith("cd ") || command.StartsWith("ls") || command.StartsWith("dir"))
                {
                    // Command keywords in different color
                    Color commandColor = Color.FromArgb(100, 150, 255);
                    Color argColor = GetTerminalTextColor(style, theme, isFocused, useThemeColors);
                    
                    PaintCommandWithArgs(g, bounds, command, font, commandColor, argColor);
                }
                else
                {
                    // Regular terminal text
                    Paint(g, bounds, command, isFocused, style, theme, useThemeColors);
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint terminal output with different styling than input
        /// </summary>
        public static void PaintOutput(Graphics g, Rectangle bounds, string output, bool isError,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(output)) return;

            Color outputColor = isError ? 
                Color.FromArgb(255, 100, 100) :  // Red for errors
                Color.FromArgb(150, 150, 150);   // Gray for normal output

            Font font = GetTerminalFont(bounds.Height, style);

            try
            {
                using (var brush = new SolidBrush(outputColor))
                {
                    g.DrawString(output, font, brush, bounds, GetTerminalStringFormat());
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        private static void PaintCommandWithArgs(Graphics g, Rectangle bounds, string command, Font font,
            Color commandColor, Color argColor)
        {
            var parts = command.Split(' ', 2);
            if (parts.Length == 1)
            {
                using (var brush = new SolidBrush(commandColor))
                {
                    g.DrawString(command, font, brush, bounds, GetTerminalStringFormat());
                }
                return;
            }

            // Paint command part
            var commandSize = g.MeasureString(parts[0] + " ", font);
            var commandBounds = new Rectangle(bounds.X, bounds.Y, (int)commandSize.Width, bounds.Height);
            
            using (var brush = new SolidBrush(commandColor))
            {
                g.DrawString(parts[0] + " ", font, brush, commandBounds, GetTerminalStringFormat());
            }

            // Paint arguments part
            var argBounds = new Rectangle(
                bounds.X + (int)commandSize.Width,
                bounds.Y,
                bounds.Width - (int)commandSize.Width,
                bounds.Height);

            using (var brush = new SolidBrush(argColor))
            {
                g.DrawString(parts[1], font, brush, argBounds, GetTerminalStringFormat());
            }
        }

        #endregion
    }
}