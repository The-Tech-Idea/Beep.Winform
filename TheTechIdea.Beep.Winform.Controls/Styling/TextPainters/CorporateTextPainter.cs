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
    /// Specialized text painter for corporate and professional business interfaces
    /// Features professional fonts, corporate branding colors, and business-appropriate typography
    /// </summary>
    public static class CorporateTextPainter
    {
        #region Corporate Font Families

        private static readonly string[] CorporateHeadingFonts = {
            "Calibri",            // Microsoft Office standard
            "Arial",              // Universal business font
            "Helvetica Neue",     // Modern corporate
            "Open Sans",          // Web-friendly corporate
            "Lato",               // Professional sans-serif
            "Source Sans Pro",    // Adobe corporate font
            "Roboto",             // Google corporate font
            "Segoe UI",           // Windows corporate
            "Tahoma"              // Fallback
        };

        private static readonly string[] CorporateBodyFonts = {
            "Segoe UI",           // Modern Windows corporate
            "Arial",              // Classic business
            "Calibri",            // Office standard
            "Verdana",            // Readable corporate
            "Tahoma",             // Clean corporate
            "Open Sans",          // Web corporate
            "Lato",               // Professional
            "Source Sans Pro"     // Adobe corporate
        };

        private static readonly string[] FinancialFonts = {
            "Consolas",           // For numbers/data
            "Monaco",             // Professional monospace
            "Menlo",              // Apple corporate monospace
            "DejaVu Sans Mono",   // Cross-platform
            "Courier New"         // Classic monospace
        };

        #endregion

        #region Corporate Color Schemes

        private static readonly CorporateColorScheme[] CorporateSchemes = {
            new CorporateColorScheme("Professional Blue", 
                Color.FromArgb(0, 78, 146),    // Primary
                Color.FromArgb(108, 117, 125), // Secondary
                Color.FromArgb(52, 58, 64),    // Text
                Color.FromArgb(248, 249, 250)), // Background

            new CorporateColorScheme("Corporate Gray",
                Color.FromArgb(52, 58, 64),    // Primary
                Color.FromArgb(108, 117, 125), // Secondary
                Color.FromArgb(33, 37, 41),    // Text
                Color.FromArgb(255, 255, 255)), // Background

            new CorporateColorScheme("Executive Navy",
                Color.FromArgb(13, 27, 42),    // Primary
                Color.FromArgb(27, 38, 59),    // Secondary
                Color.FromArgb(52, 58, 64),    // Text
                Color.FromArgb(248, 249, 250)), // Background

            new CorporateColorScheme("Financial Green",
                Color.FromArgb(25, 135, 84),   // Primary
                Color.FromArgb(108, 117, 125), // Secondary
                Color.FromArgb(33, 37, 41),    // Text
                Color.FromArgb(255, 255, 255))  // Background
        };

        private struct CorporateColorScheme
        {
            public string Name;
            public Color Primary;
            public Color Secondary;
            public Color Text;
            public Color Background;

            public CorporateColorScheme(string name, Color primary, Color secondary, Color text, Color background)
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
        /// Paint corporate text with professional styling
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Configure graphics for professional quality
            ConfigureCorporateGraphics(g);

            // Get corporate colors and font
            Color textColor = GetCorporateTextColor(style, theme, isFocused, useThemeColors);
            Font font = GetCorporateFont(bounds.Height, style, CorporateFontType.Body);

            try
            {
                // Apply corporate-specific rendering
                switch (style)
                {
                    case BeepControlStyle.StripeDashboard:
                        PaintStripeDashboard(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.AntDesign:
                        PaintAntDesign(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Minimal:
                        PaintMinimalCorporate(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.ChakraUI:
                        PaintChakraCorporate(g, bounds, text, font, textColor, isFocused);
                        break;

                    default:
                        PaintStandardCorporate(g, bounds, text, font, textColor, isFocused);
                        break;
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint corporate heading text
        /// </summary>
        public static void PaintHeading(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureCorporateGraphics(g);

            Color textColor = GetCorporateHeadingColor(style, theme, isFocused, useThemeColors);
            Font font = GetCorporateFont(bounds.Height, style, CorporateFontType.Heading);

            try
            {
                // Corporate headings with subtle emphasis
                if (isFocused)
                {
                    // Subtle underline for focused headings
                    using (var underlinePen = new Pen(textColor, 1))
                    {
                        var textSize = g.MeasureString(text, font);
                        var underlineY = bounds.Y + bounds.Height - 2;
                        var underlineWidth = Math.Min(textSize.Width, bounds.Width);
                        g.DrawLine(underlinePen, bounds.X, underlineY, bounds.X + underlineWidth, underlineY);
                    }
                }

                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(text, font, brush, bounds, GetCorporateStringFormat());
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint financial/data text with monospace font
        /// </summary>
        public static void PaintFinancialData(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, bool isPositive = true)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureCorporateGraphics(g);

            Color textColor = GetFinancialDataColor(style, theme, isPositive, isFocused, useThemeColors);
            Font font = GetCorporateFont(bounds.Height, style, CorporateFontType.Financial);

            try
            {
                // Financial data with right alignment and tabular numbers
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap
                };

                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(text, font, brush, bounds, format);
                }

                // Add subtle background for important financial data
                if (isFocused)
                {
                    using (var backBrush = new SolidBrush(Color.FromArgb(20, textColor)))
                    {
                        g.FillRectangle(backBrush, bounds);
                    }
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        #endregion

        #region Corporate Style Rendering

        private static void PaintStripeDashboard(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Clean, minimal styling like Stripe dashboard
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetCorporateStringFormat());
            }

            // Subtle left border for focused elements
            if (isFocused)
            {
                using (var borderPen = new Pen(Color.FromArgb(99, 102, 241), 3))
                {
                    g.DrawLine(borderPen, bounds.X, bounds.Y, bounds.X, bounds.Bottom);
                }
            }
        }

        private static void PaintAntDesign(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Ant Design corporate styling
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetCorporateStringFormat());
            }

            // Blue accent for focused state
            if (isFocused)
            {
                Color antBlue = Color.FromArgb(24, 144, 255);
                using (var accentBrush = new SolidBrush(Color.FromArgb(40, antBlue)))
                {
                    g.FillRectangle(accentBrush, bounds);
                }
            }
        }

        private static void PaintMinimalCorporate(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Ultra-minimal corporate styling
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetCorporateStringFormat());
            }
        }

        private static void PaintChakraCorporate(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Chakra UI corporate styling with subtle shadows
            if (isFocused)
            {
                // Subtle text shadow for depth
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    var shadowBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width, bounds.Height);
                    g.DrawString(text, font, shadowBrush, shadowBounds, GetCorporateStringFormat());
                }
            }

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetCorporateStringFormat());
            }
        }

        private static void PaintStandardCorporate(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Standard professional text rendering
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetCorporateStringFormat());
            }
        }

        #endregion

        #region Font Management

        private enum CorporateFontType
        {
            Heading,
            Body,
            Financial,
            Label
        }

        private static Font GetCorporateFont(int height, BeepControlStyle style, CorporateFontType fontType)
        {
            float fontSize = Math.Max(8, height * 0.6f);
            FontStyle fontStyle = FontStyle.Regular;

            // Adjust for font type
            switch (fontType)
            {
                case CorporateFontType.Heading:
                    fontSize = Math.Max(10, height * 0.65f);
                    fontStyle = FontStyle.Bold;
                    break;
                case CorporateFontType.Financial:
                    fontSize = Math.Max(8, height * 0.55f);
                    break;
                case CorporateFontType.Label:
                    fontSize = Math.Max(8, height * 0.5f);
                    break;
            }

            // Get appropriate font family
            string[] fontFamily = fontType switch
            {
                CorporateFontType.Heading => CorporateHeadingFonts,
                CorporateFontType.Financial => FinancialFonts,
                _ => CorporateBodyFonts
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

        #region Corporate Colors

        private static Color GetCorporateTextColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return isFocused ? theme.AccentColor : theme.SecondaryTextColor;
            }

            var scheme = GetCorporateScheme(style);
            return isFocused ? scheme.Primary : scheme.Text;
        }

        private static Color GetCorporateHeadingColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return theme.PrimaryTextColor;
            }

            var scheme = GetCorporateScheme(style);
            return isFocused ? scheme.Primary : scheme.Text;
        }

        private static Color GetFinancialDataColor(BeepControlStyle style, IBeepTheme theme, bool isPositive, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return isFocused ? theme.AccentColor : theme.SecondaryTextColor;
            }

            // Financial data colors: green for positive, red for negative
            if (isPositive)
            {
                return isFocused ? Color.FromArgb(25, 135, 84) : Color.FromArgb(40, 167, 69);
            }
            else
            {
                return isFocused ? Color.FromArgb(220, 53, 69) : Color.FromArgb(220, 53, 69);
            }
        }

        private static CorporateColorScheme GetCorporateScheme(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.StripeDashboard => CorporateSchemes[0], // Professional Blue
                BeepControlStyle.AntDesign => CorporateSchemes[1],       // Corporate Gray
                BeepControlStyle.Minimal => CorporateSchemes[2],         // Executive Navy
                BeepControlStyle.ChakraUI => CorporateSchemes[3],        // Financial Green
                _ => CorporateSchemes[0]                                 // Default to Professional Blue
            };
        }

        #endregion

        #region Helper Methods

        private static void ConfigureCorporateGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        private static StringFormat GetCorporateStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        #endregion

        #region Corporate-Specific Typography Variants

        /// <summary>
        /// Paint corporate title text for reports and documents
        /// </summary>
        public static void PaintTitle(Graphics g, Rectangle bounds, string title,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(title)) return;

            ConfigureCorporateGraphics(g);

            Font titleFont = GetCorporateFont(bounds.Height, style, CorporateFontType.Heading);
            Color titleColor = GetCorporateHeadingColor(style, theme, true, useThemeColors);

            try
            {
                using (var brush = new SolidBrush(titleColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap
                    };

                    g.DrawString(title, titleFont, brush, bounds, format);
                }

                // Add subtle underline for corporate titles
                var scheme = GetCorporateScheme(style);
                using (var underlinePen = new Pen(scheme.Secondary, 1))
                {
                    var underlineY = bounds.Bottom - 3;
                    g.DrawLine(underlinePen, bounds.X + 20, underlineY, bounds.Right - 20, underlineY);
                }
            }
            finally
            {
                titleFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint corporate label text for forms
        /// </summary>
        public static void PaintLabel(Graphics g, Rectangle bounds, string label, bool isRequired,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(label)) return;

            Font labelFont = GetCorporateFont(bounds.Height, style, CorporateFontType.Label);
            Color labelColor = GetCorporateTextColor(style, theme, false, useThemeColors);

            try
            {
                using (var brush = new SolidBrush(labelColor))
                {
                    g.DrawString(label, labelFont, brush, bounds, GetCorporateStringFormat());
                }

                // Add red asterisk for required fields
                if (isRequired)
                {
                    var labelSize = g.MeasureString(label, labelFont);
                    var asteriskX = bounds.X + labelSize.Width + 2;
                    var asteriskBounds = new Rectangle((int)asteriskX, bounds.Y, 10, bounds.Height);

                    using (var asteriskBrush = new SolidBrush(Color.FromArgb(220, 53, 69)))
                    {
                        g.DrawString("*", labelFont, asteriskBrush, asteriskBounds, GetCorporateStringFormat());
                    }
                }
            }
            finally
            {
                labelFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint corporate metric text with emphasis
        /// </summary>
        public static void PaintMetric(Graphics g, Rectangle bounds, string value, string unit, bool isImportant,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(value)) return;

            ConfigureCorporateGraphics(g);

            Font valueFont = GetCorporateFont(bounds.Height, style, CorporateFontType.Financial);
            Font unitFont = GetCorporateFont((int)(bounds.Height * 0.7f), style, CorporateFontType.Label);
            
            Color valueColor = isImportant ? 
                GetCorporateHeadingColor(style, theme, true, useThemeColors) :
                GetCorporateTextColor(style, theme, false, useThemeColors);
            
            Color unitColor = GetCorporateTextColor(style, theme, false, useThemeColors);

            try
            {
                // Measure value text
                var valueSize = g.MeasureString(value, valueFont);
                var valueBounds = new Rectangle(bounds.X, bounds.Y, (int)valueSize.Width, bounds.Height);

                // Paint value
                using (var brush = new SolidBrush(valueColor))
                {
                    g.DrawString(value, valueFont, brush, valueBounds, GetCorporateStringFormat());
                }

                // Paint unit if provided
                if (!string.IsNullOrEmpty(unit))
                {
                    var unitBounds = new Rectangle(
                        bounds.X + (int)valueSize.Width + 4, 
                        bounds.Y,
                        bounds.Width - (int)valueSize.Width - 4, 
                        bounds.Height);

                    using (var brush = new SolidBrush(unitColor))
                    {
                        g.DrawString(unit, unitFont, brush, unitBounds, GetCorporateStringFormat());
                    }
                }
            }
            finally
            {
                valueFont?.Dispose();
                unitFont?.Dispose();
            }
        }

        #endregion
    }
}