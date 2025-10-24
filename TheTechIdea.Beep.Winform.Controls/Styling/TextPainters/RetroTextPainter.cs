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
    /// Specialized text painter for retro and legacy design interfaces
    /// Features vintage fonts, retro colors, and legacy design system typography
    /// </summary>
    public static class RetroTextPainter
    {
        #region Retro Font Families

        private static readonly string[] MetroFonts = {
            "Segoe UI Light",     // Windows Phone/Metro signature
            "Segoe UI",           // Windows 8/10 Metro
            "Segoe WP",           // Windows Phone specific
            "Frutiger",           // Metro inspiration
            "Helvetica Neue",     // Clean, minimal
            "Arial",              // Fallback
        };

        private static readonly string[] OfficeFonts = {
            "Calibri",            // Office 2007+ default
            "Cambria",            // Office serif
            "Consolas",           // Office code font
            "Corbel",             // Office sans-serif
            "Segoe UI",           // Windows system
            "Arial"               // Fallback
        };

        private static readonly string[] LegacyMaterialFonts = {
            "Roboto",             // Material Design classic
            "Noto Sans",          // Google's font
            "Open Sans",          // Early Material alternative
            "Lato",               // Material-era web font
            "Source Sans Pro",    // Adobe's Material-era font
            "Arial"               // Fallback
        };

        private static readonly string[] LegacyFluentFonts = {
            "Segoe UI",           // Original Fluent
            "Segoe UI Historic",  // Fluent Design era
            "Segoe MDL2 Assets",  // Fluent icons font
            "Calibri",            // Office integration
            "Arial"               // Fallback
        };

        private static readonly string[] VintageSystemFonts = {
            "MS Sans Serif",      // Windows 95/98
            "System",             // Classic system font
            "Terminal",           // DOS-era
            "Fixedsys",          // Early Windows
            "Courier New",        // Monospace fallback
            "Arial"               // Modern fallback
        };

        #endregion

        #region Retro Color Schemes

        private static readonly RetroColorScheme[] RetroSchemes = {
            new RetroColorScheme("Windows Phone Metro",
                Color.FromArgb(0, 114, 188),   // Metro blue
                Color.FromArgb(255, 255, 255), // White background
                Color.FromArgb(0, 0, 0),       // Black text
                Color.FromArgb(140, 140, 140)), // Gray secondary

            new RetroColorScheme("Office Ribbon",
                Color.FromArgb(68, 114, 196),  // Office blue
                Color.FromArgb(243, 243, 243), // Light gray background
                Color.FromArgb(68, 68, 68),    // Dark gray text
                Color.FromArgb(149, 149, 149)), // Medium gray

            new RetroColorScheme("Material Design Classic",
                Color.FromArgb(33, 150, 243),  // Material blue
                Color.FromArgb(250, 250, 250), // Almost white
                Color.FromArgb(33, 33, 33),    // Dark text
                Color.FromArgb(158, 158, 158)), // Material gray

            new RetroColorScheme("Original Fluent",
                Color.FromArgb(0, 120, 212),   // Fluent blue
                Color.FromArgb(255, 255, 255), // White
                Color.FromArgb(50, 49, 48),    // Fluent dark
                Color.FromArgb(96, 94, 92)),   // Fluent gray

            new RetroColorScheme("Vintage Windows",
                Color.FromArgb(128, 128, 128), // Classic gray
                Color.FromArgb(192, 192, 192), // Light gray background
                Color.FromArgb(0, 0, 0),       // Black text
                Color.FromArgb(64, 64, 64))    // Dark gray
        };

        private struct RetroColorScheme
        {
            public string Name;
            public Color Primary;
            public Color Background;
            public Color Text;
            public Color Secondary;

            public RetroColorScheme(string name, Color primary, Color background, Color text, Color secondary)
            {
                Name = name;
                Primary = primary;
                Background = background;
                Text = text;
                Secondary = secondary;
            }
        }

        #endregion

        #region Main Paint Methods

        /// <summary>
        /// Paint retro text with vintage styling
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Configure graphics for retro aesthetics
            ConfigureRetroGraphics(g, style);

            // Get retro colors and font
            Color textColor = GetRetroTextColor(style, theme, isFocused, useThemeColors);
            Font font = GetRetroFont(bounds.Height, style);

            try
            {
                // Apply retro-specific rendering
                switch (style)
                {
                    case BeepControlStyle.Metro:
                        PaintMetroStyle(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Office:
                        PaintOfficeRibbon(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Material:
                        PaintLegacyMaterial(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Fluent:
                        PaintOriginalFluent(g, bounds, text, font, textColor, isFocused);
                        break;

                    default:
                        PaintVintageSystem(g, bounds, text, font, textColor, isFocused);
                        break;
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint retro tile text (Windows Phone Style)
        /// </summary>
        public static void PaintTile(Graphics g, Rectangle bounds, string text, string subtitle, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text)) return;

            ConfigureRetroGraphics(g, style);

            Font titleFont = GetRetroFont(bounds.Height, style, RetroFontType.Title);
            Font subtitleFont = GetRetroFont((int)(bounds.Height * 0.6f), style, RetroFontType.Subtitle);
            
            Color titleColor = GetRetroTextColor(style, theme, isFocused, useThemeColors);
            Color subtitleColor = GetRetroSubtitleColor(style, theme, useThemeColors);

            try
            {
                // Main title
                var titleBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 2);
                using (var brush = new SolidBrush(titleColor))
                {
                    g.DrawString(text, titleFont, brush, titleBounds, GetRetroStringFormat());
                }

                // Subtitle if provided
                if (!string.IsNullOrEmpty(subtitle))
                {
                    var subtitleBounds = new Rectangle(bounds.X, bounds.Y + bounds.Height / 2, bounds.Width, bounds.Height / 2);
                    using (var brush = new SolidBrush(subtitleColor))
                    {
                        g.DrawString(subtitle, subtitleFont, brush, subtitleBounds, GetRetroStringFormat());
                    }
                }
            }
            finally
            {
                titleFont?.Dispose();
                subtitleFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint retro ribbon tab text (Office Style)
        /// </summary>
        public static void PaintRibbonTab(Graphics g, Rectangle bounds, string text, bool isSelected,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text)) return;

            ConfigureRetroGraphics(g, style);

            Font ribbonFont = GetRetroFont(bounds.Height, style, RetroFontType.RibbonTab);
            Color ribbonColor = isSelected ? 
                GetRetroScheme(style).Primary : 
                GetRetroScheme(style).Text;

            try
            {
                // Office-Style ribbon tab with underline when selected
                using (var brush = new SolidBrush(ribbonColor))
                {
                    g.DrawString(text, ribbonFont, brush, bounds, GetCenteredRetroStringFormat());
                }

                if (isSelected)
                {
                    // Ribbon-Style bottom border
                    using (var pen = new Pen(ribbonColor, 2))
                    {
                        g.DrawLine(pen, bounds.X + 4, bounds.Bottom - 1, bounds.Right - 4, bounds.Bottom - 1);
                    }
                }
            }
            finally
            {
                ribbonFont?.Dispose();
            }
        }

        #endregion

        #region Retro Style Rendering

        private static void PaintMetroStyle(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Metro: Ultra-minimal, flat, bold typography
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetRetroStringFormat());
            }

            // Metro-Style accent line for focus
            if (isFocused)
            {
                var scheme = RetroSchemes[0]; // Metro
                using (var accentPen = new Pen(scheme.Primary, 3))
                {
                    g.DrawLine(accentPen, bounds.X, bounds.Bottom - 3, bounds.X + 40, bounds.Bottom - 3);
                }
            }
        }

        private static void PaintOfficeRibbon(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Office Ribbon: Professional with subtle gradients
            if (isFocused)
            {
                // Office-Style subtle highlight
                var scheme = RetroSchemes[1]; // Office
                using (var highlightBrush = new LinearGradientBrush(bounds, 
                    Color.FromArgb(30, scheme.Primary), 
                    Color.FromArgb(10, scheme.Primary), 
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(highlightBrush, bounds);
                }
            }

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetRetroStringFormat());
            }
        }

        private static void PaintLegacyMaterial(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Legacy Material: Roboto with subtle shadows
            if (isFocused)
            {
                // Material Design-Style subtle shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    var shadowBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width, bounds.Height);
                    g.DrawString(text, font, shadowBrush, shadowBounds, GetRetroStringFormat());
                }
            }

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetRetroStringFormat());
            }
        }

        private static void PaintOriginalFluent(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Original Fluent: Subtle depth and acrylic hints
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetRetroStringFormat());
            }

            // Fluent-Style subtle glow for focus
            if (isFocused)
            {
                var scheme = RetroSchemes[3]; // Fluent
                using (var glowBrush = new SolidBrush(Color.FromArgb(20, scheme.Primary)))
                {
                    var glowBounds = new Rectangle(bounds.X - 2, bounds.Y - 1, bounds.Width + 4, bounds.Height + 2);
                    g.FillRectangle(glowBrush, glowBounds);
                }
            }
        }

        private static void PaintVintageSystem(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Vintage Windows: Pixelated, bitmap-Style rendering
            var oldMode = g.SmoothingMode;
            var oldHint = g.TextRenderingHint;

            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            try
            {
                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(text, font, brush, bounds, GetRetroStringFormat());
                }

                // Vintage-Style raised effect for focus
                if (isFocused)
                {
                    using (var highlightBrush = new SolidBrush(Color.FromArgb(255, 255, 255)))
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(128, 128, 128)))
                    {
                        // Top-left highlight
                        var highlightBounds = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width, bounds.Height);
                        g.DrawString(text, font, highlightBrush, highlightBounds, GetRetroStringFormat());
                        
                        // Bottom-right shadow
                        var shadowBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width, bounds.Height);
                        g.DrawString(text, font, shadowBrush, shadowBounds, GetRetroStringFormat());
                    }
                }
            }
            finally
            {
                g.SmoothingMode = oldMode;
                g.TextRenderingHint = oldHint;
            }
        }

        #endregion

        #region Font Management

        private enum RetroFontType
        {
            Body,
            Title,
            Subtitle,
            RibbonTab,
            Button
        }

        private static Font GetRetroFont(int height, BeepControlStyle style, RetroFontType fontType = RetroFontType.Body)
        {
            float fontSize = Math.Max(8, height * 0.6f);
            FontStyle fontStyle = FontStyle.Regular;

            // Adjust for font type
            switch (fontType)
            {
                case RetroFontType.Title:
                    fontSize = Math.Max(12, height * 0.8f);
                    fontStyle = FontStyle.Bold;
                    break;
                case RetroFontType.Subtitle:
                    fontSize = Math.Max(8, height * 0.5f);
                    fontStyle = FontStyle.Regular;
                    break;
                case RetroFontType.RibbonTab:
                    fontSize = Math.Max(8, height * 0.55f);
                    break;
                case RetroFontType.Button:
                    fontSize = Math.Max(8, height * 0.6f);
                    fontStyle = FontStyle.Bold;
                    break;
            }

            // Get appropriate font family for retro Style
            string[] fontFamily = style switch
            {
                BeepControlStyle.Metro => MetroFonts,
                BeepControlStyle.Office => OfficeFonts,
                BeepControlStyle.Material => LegacyMaterialFonts,
                BeepControlStyle.Fluent => LegacyFluentFonts,
                _ => VintageSystemFonts
            };

            // Try embedded fonts first
            foreach (string fontName in fontFamily)
            {
                Font embeddedFont = BeepFontManager.GetEmbeddedFont(fontName, fontSize, fontStyle);
                if (embeddedFont != null)
                    return embeddedFont;
            }

            // Try system fonts
            foreach (string fontName in fontFamily)
            {
                try
                {
                    return new Font(fontName, fontSize, fontStyle, GraphicsUnit.Point);
                }
                catch
                {
                    continue;
                }
            }

            return new Font("Arial", fontSize, fontStyle, GraphicsUnit.Point);
        }

        #endregion

        #region Retro Colors

        private static Color GetRetroTextColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return isFocused ? theme.AccentColor : theme.SecondaryTextColor;
            }

            var scheme = GetRetroScheme(style);
            return isFocused ? scheme.Primary : scheme.Text;
        }

        private static Color GetRetroSubtitleColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return theme.SecondaryTextColor;
            }

            var scheme = GetRetroScheme(style);
            return scheme.Secondary;
        }

        private static RetroColorScheme GetRetroScheme(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Metro => RetroSchemes[0],    // Windows Phone Metro
                BeepControlStyle.Office => RetroSchemes[1],   // Office Ribbon
                BeepControlStyle.Material => RetroSchemes[2], // Material Classic
                BeepControlStyle.Fluent => RetroSchemes[3],   // Original Fluent
                _ => RetroSchemes[4]                          // Vintage Windows
            };
        }

        #endregion

        #region Helper Methods

        private static void ConfigureRetroGraphics(Graphics g, BeepControlStyle style)
        {
            // Different quality settings for different retro styles
            switch (style)
            {
                case BeepControlStyle.Metro:
                case BeepControlStyle.Office:
                case BeepControlStyle.Material:
                case BeepControlStyle.Fluent:
                    // Modern retro styles - high quality
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    break;

                default:
                    // Vintage styles - pixelated
                    g.SmoothingMode = SmoothingMode.None;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    break;
            }

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        private static StringFormat GetRetroStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        private static StringFormat GetCenteredRetroStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        #endregion

        #region Retro-Specific Typography Variants

        /// <summary>
        /// Paint Windows Phone-Style live tile text
        /// </summary>
        public static void PaintLiveTile(Graphics g, Rectangle bounds, string primaryText, string secondaryText, 
            bool isFlipped, BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(primaryText)) return;

            Font primaryFont = GetRetroFont(bounds.Height, style, RetroFontType.Title);
            Font secondaryFont = GetRetroFont((int)(bounds.Height * 0.4f), style, RetroFontType.Body);

            var scheme = GetRetroScheme(style);
            Color primaryColor = scheme.Background == Color.White ? scheme.Text : Color.White;
            Color secondaryColor = Color.FromArgb(180, primaryColor);

            try
            {
                // Primary text (large)
                var primaryBounds = new Rectangle(bounds.X + 10, bounds.Y + 10, bounds.Width - 20, bounds.Height / 2);
                using (var brush = new SolidBrush(primaryColor))
                {
                    g.DrawString(primaryText, primaryFont, brush, primaryBounds, GetRetroStringFormat());
                }

                // Secondary text (smaller, bottom)
                if (!string.IsNullOrEmpty(secondaryText))
                {
                    var secondaryBounds = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 2, bounds.Width - 20, bounds.Height / 2);
                    using (var brush = new SolidBrush(secondaryColor))
                    {
                        g.DrawString(secondaryText, secondaryFont, brush, secondaryBounds, GetRetroStringFormat());
                    }
                }
            }
            finally
            {
                primaryFont?.Dispose();
                secondaryFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint Office ribbon button text with icon space
        /// </summary>
        public static void PaintRibbonButton(Graphics g, Rectangle bounds, string text, bool hasIcon, bool isPressed,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font buttonFont = GetRetroFont(bounds.Height, style, RetroFontType.Button);
            Color buttonColor = GetRetroTextColor(style, theme, isPressed, useThemeColors);

            try
            {
                // Adjust text bounds for icon space
                var textBounds = hasIcon ? 
                    new Rectangle(bounds.X + 24, bounds.Y, bounds.Width - 24, bounds.Height) :
                    bounds;

                using (var brush = new SolidBrush(buttonColor))
                {
                    g.DrawString(text, buttonFont, brush, textBounds, GetCenteredRetroStringFormat());
                }
            }
            finally
            {
                buttonFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint vintage dialog text with raised effect
        /// </summary>
        public static void PaintVintageDialog(Graphics g, Rectangle bounds, string text, bool isTitle,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font dialogFont = GetRetroFont(bounds.Height, style, isTitle ? RetroFontType.Title : RetroFontType.Body);
            var scheme = GetRetroScheme(style);

            try
            {
                // Vintage raised text effect
                using (var shadowBrush = new SolidBrush(Color.FromArgb(128, 128, 128)))
                using (var highlightBrush = new SolidBrush(Color.White))
                using (var textBrush = new SolidBrush(scheme.Text))
                {
                    // Shadow (bottom-right)
                    var shadowBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width, bounds.Height);
                    g.DrawString(text, dialogFont, shadowBrush, shadowBounds, GetRetroStringFormat());

                    // Highlight (top-left)
                    var highlightBounds = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width, bounds.Height);
                    g.DrawString(text, dialogFont, highlightBrush, highlightBounds, GetRetroStringFormat());

                    // Main text
                    g.DrawString(text, dialogFont, textBrush, bounds, GetRetroStringFormat());
                }
            }
            finally
            {
                dialogFont?.Dispose();
            }
        }

        #endregion
    }
}