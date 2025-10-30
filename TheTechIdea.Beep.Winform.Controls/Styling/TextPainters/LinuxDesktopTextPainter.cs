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
    /// Specialized text painter for Linux desktop environment interfaces
    /// Features authentic Linux DE typography (GNOME, KDE, Elementary, Cinnamon)
    /// </summary>
    public static class LinuxDesktopTextPainter
    {
        #region Linux Desktop Font Families

        private static readonly string[] GnomeFonts = {
            "Cantarell",          // GNOME's default font
            "Ubuntu",             // Ubuntu system font
            "DejaVu Sans",        // Common Linux font
            "Liberation Sans",    // Open source alternative
            "Noto Sans",          // Google's comprehensive font
            "Source Sans Pro",    // Adobe's open source font
            "Open Sans",          // Web-safe fallback
            "Arial"               // Final fallback
        };

        private static readonly string[] KdeFonts = {
            "Noto Sans",          // KDE Plasma 5+ default
            "Oxygen",             // Classic KDE font
            "DejaVu Sans",        // KDE fallback
            "Liberation Sans",    // Open source
            "Ubuntu",             // Popular Linux font
            "Cantarell",          // GNOME font (cross-compatibility)
            "Arial"               // Final fallback
        };

        private static readonly string[] ElementaryFonts = {
            "Inter",              // elementary OS 6+ default
            "Open Sans",          // elementary OS legacy
            "Ubuntu",             // Ubuntu-based
            "Lato",               // Professional sans-serif
            "Source Sans Pro",    // Clean and readable
            "Roboto",             // Android/Google
            "Arial"               // Final fallback
        };

        private static readonly string[] CinnamonFonts = {
            "Noto Sans",          // Cinnamon default
            "Ubuntu",             // Linux Mint base
            "DejaVu Sans",        // Common Linux
            "Liberation Sans",    // Office compatibility
            "Open Sans",          // Web-friendly
            "Arial"               // Final fallback
        };

        #endregion

        #region Linux Desktop Colors

        private static readonly LinuxDesktopColorScheme[] LinuxSchemes = {
            new LinuxDesktopColorScheme("GNOME Adwaita", 
                Color.FromArgb(53, 132, 228),   // Blue primary
                Color.FromArgb(99, 142, 155),   // Blue-gray secondary  
                Color.FromArgb(46, 52, 64),     // Dark text
                Color.FromArgb(255, 255, 255)), // White background

            new LinuxDesktopColorScheme("KDE Breeze",
                Color.FromArgb(61, 174, 233),   // Breeze blue
                Color.FromArgb(35, 38, 41),     // Dark secondary
                Color.FromArgb(49, 54, 59),     // Text color
                Color.FromArgb(252, 252, 252)), // Light background

            new LinuxDesktopColorScheme("elementary OS",
                Color.FromArgb(102, 109, 238),  // elementary purple
                Color.FromArgb(153, 193, 241),  // Light blue
                Color.FromArgb(51, 51, 51),     // Dark text
                Color.FromArgb(248, 248, 248)), // Off-white background

            new LinuxDesktopColorScheme("Cinnamon Mint",
                Color.FromArgb(139, 195, 74),   // Mint green
                Color.FromArgb(76, 175, 80),    // Material green
                Color.FromArgb(33, 33, 33),     // Dark text
                Color.FromArgb(250, 250, 250))  // Light background
        };

        private struct LinuxDesktopColorScheme
        {
            public string Name;
            public Color Primary;
            public Color Secondary;
            public Color Text;
            public Color Background;

            public LinuxDesktopColorScheme(string name, Color primary, Color secondary, Color text, Color background)
            {
                Name = name;
                Primary = primary;
                Secondary = secondary;
                Text = text;
                Background = background;
            }
        }

        #endregion

        #region Main Paint Methods

        /// <summary>
        /// Paint Linux desktop text with authentic DE styling
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Configure graphics for Linux DE aesthetics
            ConfigureLinuxGraphics(g);

            // Get Linux DE colors and font
            Color textColor = GetLinuxTextColor(style, theme, isFocused, useThemeColors);
            Font font = GetLinuxDesktopFont(bounds.Height, style);

            try
            {
                // Apply Linux DE-specific rendering
                switch (style)
                {
                    case BeepControlStyle.Gnome:
                        PaintGnomeAdwaita(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Kde:
                        PaintKdeBreeze(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Elementary:
                        PaintElementaryOS(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Cinnamon:
                        PaintCinnamon(g, bounds, text, font, textColor, isFocused);
                        break;

                    default:
                        PaintGenericLinux(g, bounds, text, font, textColor, isFocused);
                        break;
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint Linux application title bar text
        /// </summary>
        public static void PaintTitleBar(Graphics g, Rectangle bounds, string title, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(title)) return;

            ConfigureLinuxGraphics(g);

            Font titleFont = GetLinuxDesktopFont(bounds.Height, style, LinuxFontType.Title);
            Color titleColor = GetLinuxTitleColor(style, theme, isFocused, useThemeColors);

            try
            {
                var brush = PaintersFactory.GetSolidBrush(titleColor);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap
                };

                g.DrawString(title, titleFont, brush, bounds, format);
            }
            finally
            {
                titleFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint Linux button text with DE-specific styling
        /// </summary>
        public static void PaintButton(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text)) return;

            ConfigureLinuxGraphics(g);

            Font buttonFont = GetLinuxDesktopFont(bounds.Height, style, LinuxFontType.Button);
            Color buttonColor = GetLinuxButtonColor(style, theme, isFocused, useThemeColors);

            try
            {
                // Linux DE buttons often have subtle text effects
                if (isFocused)
                {
                    // Subtle highlight for focused state
                    var highlightBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(30, Color.White));
                    g.FillRectangle(highlightBrush, bounds);
                }

                var brush = PaintersFactory.GetSolidBrush(buttonColor);
                g.DrawString(text, buttonFont, brush, bounds, GetLinuxStringFormat());
            }
            finally
            {
                buttonFont?.Dispose();
            }
        }

        #endregion

        #region Linux Desktop Style Rendering

        private static void PaintGnomeAdwaita(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var brush = PaintersFactory.GetSolidBrush(textColor);
            g.DrawString(text, font, brush, bounds, GetLinuxStringFormat());

            // Subtle bottom border for focused state (GNOME Style)
            if (isFocused)
            {
                var scheme = LinuxSchemes[0]; // GNOME
                var borderPen = PaintersFactory.GetPen(scheme.Primary,2);
                var borderY = bounds.Bottom -1;
                g.DrawLine(borderPen, bounds.X +4, borderY, bounds.Right -4, borderY);
            }
        }

        private static void PaintKdeBreeze(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // KDE Breeze: Subtle glow and gradients
            if (isFocused)
            {
                // KDE-Style subtle glow
                var scheme = LinuxSchemes[1]; // KDE
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, scheme.Primary));
                var glowBounds = new Rectangle(bounds.X -2, bounds.Y -1, bounds.Width +4, bounds.Height +2);
                g.FillRectangle(glowBrush, glowBounds);
            }

            var brush = PaintersFactory.GetSolidBrush(textColor);
            g.DrawString(text, font, brush, bounds, GetLinuxStringFormat());
        }

        private static void PaintElementaryOS(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var brush = PaintersFactory.GetSolidBrush(textColor);
            g.DrawString(text, font, brush, bounds, GetLinuxStringFormat());

            // elementary-Style accent line
            if (isFocused)
            {
                var scheme = LinuxSchemes[2]; // elementary
                var accentPen = PaintersFactory.GetPen(scheme.Primary,1);
                g.DrawLine(accentPen, bounds.X, bounds.Bottom -1, bounds.Right, bounds.Bottom -1);
            }
        }

        private static void PaintCinnamon(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var brush = PaintersFactory.GetSolidBrush(textColor);
            g.DrawString(text, font, brush, bounds, GetLinuxStringFormat());

            // Mint green highlight for focus
            if (isFocused)
            {
                var scheme = LinuxSchemes[3]; // Cinnamon
                var highlightBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(25, scheme.Primary));
                g.FillRectangle(highlightBrush, bounds);
            }
        }

        private static void PaintGenericLinux(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var brush = PaintersFactory.GetSolidBrush(textColor);
            g.DrawString(text, font, brush, bounds, GetLinuxStringFormat());
        }

        #endregion

        #region Font Management

        private enum LinuxFontType
        {
            Body,
            Title,
            Button,
            Menu
        }

        private static Font GetLinuxDesktopFont(int height, BeepControlStyle style, LinuxFontType fontType = LinuxFontType.Body)
        {
            float fontSize = Math.Max(8, height * 0.6f);
            FontStyle fontStyle = FontStyle.Regular;

            // Adjust for font type
            switch (fontType)
            {
                case LinuxFontType.Title:
                    fontSize = Math.Max(10, height * 0.7f);
                    fontStyle = FontStyle.Bold;
                    break;
                case LinuxFontType.Button:
                    fontSize = Math.Max(8, height * 0.55f);
                    break;
                case LinuxFontType.Menu:
                    fontSize = Math.Max(8, height * 0.5f);
                    break;
            }

            // Get appropriate font family for Linux DE
            string[] fontFamily = style switch
            {
                BeepControlStyle.Gnome => GnomeFonts,
                BeepControlStyle.Kde => KdeFonts,
                BeepControlStyle.Elementary => ElementaryFonts,
                BeepControlStyle.Cinnamon => CinnamonFonts,
                _ => GnomeFonts // Default to GNOME fonts
            };

            // Try embedded fonts first (FontManagement integration)
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

        #region Linux Desktop Colors

        private static Color GetLinuxTextColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return isFocused ? theme.AccentColor : theme.SecondaryTextColor;
            }

            var scheme = GetLinuxScheme(style);
            return isFocused ? scheme.Primary : scheme.Text;
        }

        private static Color GetLinuxTitleColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return theme.PrimaryTextColor;
            }

            var scheme = GetLinuxScheme(style);
            return scheme.Text;
        }

        private static Color GetLinuxButtonColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return isFocused ? theme.AccentColor : theme.ButtonForeColor;
            }

            var scheme = GetLinuxScheme(style);
            return isFocused ? Color.White : scheme.Text;
        }

        private static LinuxDesktopColorScheme GetLinuxScheme(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Gnome => LinuxSchemes[0],       // GNOME Adwaita
                BeepControlStyle.Kde => LinuxSchemes[1],         // KDE Breeze  
                BeepControlStyle.Elementary => LinuxSchemes[2],  // elementary OS
                BeepControlStyle.Cinnamon => LinuxSchemes[3],    // Cinnamon Mint
                _ => LinuxSchemes[0]                             // Default to GNOME
            };
        }

        #endregion

        #region Helper Methods

        private static void ConfigureLinuxGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        private static StringFormat GetLinuxStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        #endregion

        #region Linux-Specific Typography Variants

        /// <summary>
        /// Paint Linux notification text
        /// </summary>
        public static void PaintNotification(Graphics g, Rectangle bounds, string text, bool isUrgent,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font notificationFont = GetLinuxDesktopFont(bounds.Height, style, LinuxFontType.Body);
            Color notificationColor = isUrgent ? 
                Color.FromArgb(237, 51, 59) :  // Red for urgent
                GetLinuxTextColor(style, theme, false, useThemeColors);

            try
            {
                using (var brush = new SolidBrush(notificationColor))
                {
                    g.DrawString(text, notificationFont, brush, bounds, GetLinuxStringFormat());
                }
            }
            finally
            {
                notificationFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint Linux menu item text with keyboard shortcuts
        /// </summary>
        public static void PaintMenuItem(Graphics g, Rectangle bounds, string text, string shortcut, bool isSelected,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font menuFont = GetLinuxDesktopFont(bounds.Height, style, LinuxFontType.Menu);
            Color menuColor = GetLinuxTextColor(style, theme, isSelected, useThemeColors);
            Color shortcutColor = Color.FromArgb(150, menuColor);

            try
            {
                // Paint main text
                using (var brush = new SolidBrush(menuColor))
                {
                    g.DrawString(text, menuFont, brush, bounds, GetLinuxStringFormat());
                }

                // Paint keyboard shortcut on the right
                if (!string.IsNullOrEmpty(shortcut))
                {
                    var textSize = g.MeasureString(text, menuFont);
                    var shortcutBounds = new Rectangle(
                        bounds.Right - 80, bounds.Y, 80, bounds.Height);

                    var shortcutFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Center
                    };

                    using (var shortcutBrush = new SolidBrush(shortcutColor))
                    {
                        g.DrawString(shortcut, menuFont, shortcutBrush, shortcutBounds, shortcutFormat);
                    }
                }
            }
            finally
            {
                menuFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint Linux status bar text with system information
        /// </summary>
        public static void PaintStatusBar(Graphics g, Rectangle bounds, string text, bool isImportant,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font statusFont = GetLinuxDesktopFont((int)(bounds.Height * 0.8f), style, LinuxFontType.Body);
            Color statusColor = isImportant ?
                GetLinuxScheme(style).Primary :
                Color.FromArgb(120, GetLinuxScheme(style).Text);

            try
            {
                using (var brush = new SolidBrush(statusColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(text, statusFont, brush, bounds, format);
                }
            }
            finally
            {
                statusFont?.Dispose();
            }
        }

        #endregion
    }
}