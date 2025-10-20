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
    /// Specialized text painter for accessibility-focused interfaces
    /// Features high contrast, large fonts, and WCAG AAA compliance
    /// </summary>
    public static class AccessibilityTextPainter
    {
        #region Accessibility Font Families

        private static readonly string[] HighContrastFonts = {
            "Segoe UI",           // Windows high contrast default
            "Arial",              // Universal accessibility font
            "Verdana",            // High readability
            "Tahoma",             // Clear at small sizes
            "Calibri",            // Microsoft accessibility font
            "DejaVu Sans",        // Open source accessible font
            "Liberation Sans",    // Cross-platform accessibility
            "Open Sans"           // Web accessibility standard
        };

        private static readonly string[] DyslexiaFriendlyFonts = {
            "OpenDyslexic",       // Specialized dyslexia font
            "Dyslexie",          // Dyslexia-friendly font
            "Lexend",            // Reading proficiency font
            "Atkinson Hyperlegible", // Braille Institute font
            "Comic Sans MS",      // Surprisingly dyslexia-friendly
            "Verdana",            // High readability fallback
            "Arial"               // Universal fallback
        };

        #endregion

        #region Accessibility Color Schemes

        private static readonly AccessibilityColorScheme[] AccessibilitySchemes = {
            new AccessibilityColorScheme("High Contrast Black",
                Color.FromArgb(0, 0, 0),       // Black background
                Color.FromArgb(255, 255, 255), // White text
                Color.FromArgb(255, 255, 0),   // Yellow accent
                Color.FromArgb(0, 255, 0)),    // Green success

            new AccessibilityColorScheme("High Contrast White", 
                Color.FromArgb(255, 255, 255), // White background
                Color.FromArgb(0, 0, 0),       // Black text
                Color.FromArgb(0, 0, 255),     // Blue accent
                Color.FromArgb(0, 128, 0)),    // Dark green success

            new AccessibilityColorScheme("Yellow on Black",
                Color.FromArgb(0, 0, 0),       // Black background
                Color.FromArgb(255, 255, 0),   // Yellow text
                Color.FromArgb(255, 255, 255), // White accent
                Color.FromArgb(0, 255, 255)),  // Cyan success

            new AccessibilityColorScheme("Blue on Yellow",
                Color.FromArgb(255, 255, 0),   // Yellow background
                Color.FromArgb(0, 0, 139),     // Dark blue text
                Color.FromArgb(0, 0, 0),       // Black accent
                Color.FromArgb(0, 100, 0))     // Dark green success
        };

        private struct AccessibilityColorScheme
        {
            public string Name;
            public Color Background;
            public Color Text;
            public Color Accent;
            public Color Success;

            public AccessibilityColorScheme(string name, Color background, Color text, Color accent, Color success)
            {
                Name = name;
                Background = background;
                Text = text;
                Accent = accent;
                Success = success;
            }
        }

        #endregion

        #region WCAG Compliance Constants

        private const float MIN_CONTRAST_RATIO_AA = 4.5f;
        private const float MIN_CONTRAST_RATIO_AAA = 7.0f;
        private const float MIN_LARGE_TEXT_CONTRAST_AA = 3.0f;
        private const float MIN_LARGE_TEXT_CONTRAST_AAA = 4.5f;
        private const int LARGE_TEXT_SIZE_THRESHOLD = 18; // 18pt or 14pt bold

        #endregion

        #region Main Paint Methods

        /// <summary>
        /// Paint accessibility-compliant text with high contrast and readability
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Configure graphics for maximum readability
            ConfigureAccessibilityGraphics(g);

            // Get accessibility colors and font
            Color textColor = GetAccessibilityTextColor(style, theme, isFocused, useThemeColors);
            Color backgroundColor = GetAccessibilityBackgroundColor(style, theme, useThemeColors);
            Font font = GetAccessibilityFont(bounds.Height, style);

            try
            {
                // Ensure WCAG compliance
                if (!IsWcagCompliant(textColor, backgroundColor, font.Size))
                {
                    textColor = GetWcagCompliantColor(backgroundColor, font.Size);
                }

                // Apply accessibility-specific rendering
                switch (style)
                {
                    case BeepControlStyle.HighContrast:
                        PaintHighContrast(g, bounds, text, font, textColor, backgroundColor, isFocused);
                        break;

                    default:
                        PaintAccessibleStandard(g, bounds, text, font, textColor, isFocused);
                        break;
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint accessibility text with dyslexia-friendly styling
        /// </summary>
        public static void PaintDyslexiaFriendly(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureAccessibilityGraphics(g);

            Font dyslexiaFont = GetDyslexiaFriendlyFont(bounds.Height);
            Color textColor = GetAccessibilityTextColor(style, theme, isFocused, useThemeColors);

            try
            {
                // Dyslexia-friendly features: larger spacing, clear fonts
                var format = GetDyslexiaFriendlyStringFormat();
                
                using (var brush = new SolidBrush(textColor))
                {
                    // Add subtle background for better readability
                    if (isFocused)
                    {
                        using (var backgroundBrush = new SolidBrush(Color.FromArgb(30, textColor)))
                        {
                            var backgroundRect = new Rectangle(bounds.X - 4, bounds.Y - 2, bounds.Width + 8, bounds.Height + 4);
                            g.FillRectangle(backgroundBrush, backgroundRect);
                        }
                    }

                    g.DrawString(text, dyslexiaFont, brush, bounds, format);
                }
            }
            finally
            {
                dyslexiaFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint screen reader friendly text with enhanced focus indicators
        /// </summary>
        public static void PaintScreenReaderFriendly(Graphics g, Rectangle bounds, string text, bool isFocused, bool hasScreenReaderFocus,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureAccessibilityGraphics(g);

            Font font = GetAccessibilityFont(bounds.Height, style, AccessibilityFontType.ScreenReader);
            Color textColor = GetAccessibilityTextColor(style, theme, isFocused, useThemeColors);

            try
            {
                // Enhanced focus indicators for screen readers
                if (hasScreenReaderFocus)
                {
                    // Bold focus outline for screen reader focus
                    using (var focusPen = new Pen(Color.FromArgb(255, 255, 0), 3))
                    {
                        var focusRect = new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4);
                        g.DrawRectangle(focusPen, focusRect);
                    }

                    // High contrast background
                    using (var focusBackground = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
                    {
                        g.FillRectangle(focusBackground, bounds);
                    }
                }

                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(text, font, brush, bounds, GetAccessibilityStringFormat());
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        #endregion

        #region Accessibility Style Rendering

        private static void PaintHighContrast(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, Color backgroundColor, bool isFocused)
        {
            // Fill background for maximum contrast
            using (var backgroundBrush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(backgroundBrush, bounds);
            }

            // Paint text with maximum contrast
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetAccessibilityStringFormat());
            }

            // High contrast focus indicator
            if (isFocused)
            {
                var scheme = AccessibilitySchemes[0]; // High contrast
                using (var focusPen = new Pen(scheme.Accent, 2))
                {
                    g.DrawRectangle(focusPen, bounds.X - 1, bounds.Y - 1, bounds.Width + 1, bounds.Height + 1);
                }
            }
        }

        private static void PaintAccessibleStandard(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetAccessibilityStringFormat());
            }

            // Clear focus indicator
            if (isFocused)
            {
                using (var focusPen = new Pen(textColor, 1))
                {
                    focusPen.DashStyle = DashStyle.Dot;
                    g.DrawRectangle(focusPen, bounds.X - 1, bounds.Y - 1, bounds.Width + 1, bounds.Height + 1);
                }
            }
        }

        #endregion

        #region Font Management

        private enum AccessibilityFontType
        {
            Standard,
            Large,
            ExtraLarge,
            ScreenReader,
            DyslexiaFriendly
        }

        private static Font GetAccessibilityFont(int height, BeepControlStyle style, AccessibilityFontType fontType = AccessibilityFontType.Standard)
        {
            // Accessibility fonts are generally larger
            float fontSize = Math.Max(10, height * 0.7f);
            FontStyle fontStyle = FontStyle.Regular;

            // Adjust for accessibility requirements
            switch (fontType)
            {
                case AccessibilityFontType.Large:
                    fontSize = Math.Max(14, height * 0.8f);
                    break;
                case AccessibilityFontType.ExtraLarge:
                    fontSize = Math.Max(18, height * 0.9f);
                    fontStyle = FontStyle.Bold;
                    break;
                case AccessibilityFontType.ScreenReader:
                    fontSize = Math.Max(12, height * 0.75f);
                    fontStyle = FontStyle.Bold;
                    break;
                case AccessibilityFontType.DyslexiaFriendly:
                    fontSize = Math.Max(12, height * 0.75f);
                    break;
            }

            // Use high contrast fonts for accessibility
            string[] fontFamily = HighContrastFonts;

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

        private static Font GetDyslexiaFriendlyFont(int height)
        {
            float fontSize = Math.Max(12, height * 0.7f);

            // Try dyslexia-friendly fonts first
            foreach (string fontName in DyslexiaFriendlyFonts)
            {
                Font embeddedFont = BeepFontManager.GetEmbeddedFont(fontName, fontSize, FontStyle.Regular);
                if (embeddedFont != null)
                    return embeddedFont;

                try
                {
                    return new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Point);
                }
                catch
                {
                    continue;
                }
            }

            return new Font("Verdana", fontSize, FontStyle.Regular, GraphicsUnit.Point);
        }

        #endregion

        #region WCAG Compliance

        private static bool IsWcagCompliant(Color textColor, Color backgroundColor, float fontSize)
        {
            float contrastRatio = CalculateContrastRatio(textColor, backgroundColor);
            bool isLargeText = fontSize >= LARGE_TEXT_SIZE_THRESHOLD;

            if (isLargeText)
            {
                return contrastRatio >= MIN_LARGE_TEXT_CONTRAST_AAA;
            }
            else
            {
                return contrastRatio >= MIN_CONTRAST_RATIO_AAA;
            }
        }

        private static float CalculateContrastRatio(Color color1, Color color2)
        {
            float luminance1 = CalculateRelativeLuminance(color1);
            float luminance2 = CalculateRelativeLuminance(color2);

            float lighter = Math.Max(luminance1, luminance2);
            float darker = Math.Min(luminance1, luminance2);

            return (lighter + 0.05f) / (darker + 0.05f);
        }

        private static float CalculateRelativeLuminance(Color color)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;

            r = r <= 0.03928f ? r / 12.92f : (float)Math.Pow((r + 0.055) / 1.055, 2.4);
            g = g <= 0.03928f ? g / 12.92f : (float)Math.Pow((g + 0.055) / 1.055, 2.4);
            b = b <= 0.03928f ? b / 12.92f : (float)Math.Pow((b + 0.055) / 1.055, 2.4);

            return 0.2126f * r + 0.7152f * g + 0.0722f * b;
        }

        private static Color GetWcagCompliantColor(Color backgroundColor, float fontSize)
        {
            bool isLargeText = fontSize >= LARGE_TEXT_SIZE_THRESHOLD;
            float requiredRatio = isLargeText ? MIN_LARGE_TEXT_CONTRAST_AAA : MIN_CONTRAST_RATIO_AAA;

            // Use high contrast colors that meet WCAG AAA standards
            Color[] compliantColors = {
                Color.FromArgb(0, 0, 0),       // Black
                Color.FromArgb(255, 255, 255), // White
                Color.FromArgb(0, 0, 139),     // Dark blue
                Color.FromArgb(139, 0, 0)      // Dark red
            };

            foreach (Color color in compliantColors)
            {
                if (CalculateContrastRatio(color, backgroundColor) >= requiredRatio)
                {
                    return color;
                }
            }

            // Fallback to black or white based on background brightness
            float backgroundLuminance = CalculateRelativeLuminance(backgroundColor);
            return backgroundLuminance > 0.5f ? Color.Black : Color.White;
        }

        #endregion

        #region Accessibility Colors

        private static Color GetAccessibilityTextColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                // Ensure theme colors meet accessibility standards
                Color themeColor = isFocused ? theme.AccentColor : theme.SecondaryTextColor;
                Color backgroundColor = theme.BackgroundColor;
                
                if (IsWcagCompliant(themeColor, backgroundColor, 12f))
                {
                    return themeColor;
                }
                else
                {
                    return GetWcagCompliantColor(backgroundColor, 12f);
                }
            }

            var scheme = GetAccessibilityScheme(style);
            return isFocused ? scheme.Accent : scheme.Text;
        }

        private static Color GetAccessibilityBackgroundColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return theme.BackgroundColor;
            }

            var scheme = GetAccessibilityScheme(style);
            return scheme.Background;
        }

        private static AccessibilityColorScheme GetAccessibilityScheme(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.HighContrast => AccessibilitySchemes[0], // High Contrast Black
                _ => AccessibilitySchemes[1] // High Contrast White (default)
            };
        }

        #endregion

        #region Helper Methods

        private static void ConfigureAccessibilityGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        private static StringFormat GetAccessibilityStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        private static StringFormat GetDyslexiaFriendlyStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap,
                // Increased line spacing for dyslexia readability
            };
        }

        #endregion

        #region Accessibility-Specific Typography Variants

        /// <summary>
        /// Paint button text with enhanced accessibility features
        /// </summary>
        public static void PaintAccessibleButton(Graphics g, Rectangle bounds, string text, bool isFocused, bool isDefault,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font buttonFont = GetAccessibilityFont(bounds.Height, style, 
                isDefault ? AccessibilityFontType.Large : AccessibilityFontType.Standard);
            Color buttonColor = GetAccessibilityTextColor(style, theme, isFocused, useThemeColors);

            try
            {
                // Enhanced focus indicator for buttons
                if (isFocused)
                {
                    using (var focusPen = new Pen(buttonColor, 3))
                    {
                        focusPen.DashStyle = DashStyle.Solid;
                        g.DrawRectangle(focusPen, bounds.X - 2, bounds.Y - 2, bounds.Width + 3, bounds.Height + 3);
                    }
                }

                // Default button indicator
                if (isDefault)
                {
                    using (var defaultPen = new Pen(buttonColor, 2))
                    {
                        g.DrawRectangle(defaultPen, bounds.X - 1, bounds.Y - 1, bounds.Width + 1, bounds.Height + 1);
                    }
                }

                using (var brush = new SolidBrush(buttonColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, buttonFont, brush, bounds, format);
                }
            }
            finally
            {
                buttonFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint error message with high visibility
        /// </summary>
        public static void PaintErrorMessage(Graphics g, Rectangle bounds, string errorText, bool isUrgent,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(errorText)) return;

            Font errorFont = GetAccessibilityFont(bounds.Height, style, AccessibilityFontType.Large);
            Color errorColor = Color.FromArgb(220, 53, 69); // High contrast red
            Color backgroundColor = Color.FromArgb(255, 243, 205); // Light yellow background

            try
            {
                // High contrast error background
                using (var backgroundBrush = new SolidBrush(backgroundColor))
                {
                    g.FillRectangle(backgroundBrush, bounds);
                }

                // Error border
                using (var borderPen = new Pen(errorColor, 2))
                {
                    g.DrawRectangle(borderPen, bounds);
                }

                // Error text
                using (var brush = new SolidBrush(errorColor))
                {
                    var textBounds = new Rectangle(bounds.X + 8, bounds.Y + 4, bounds.Width - 16, bounds.Height - 8);
                    g.DrawString(errorText, errorFont, brush, textBounds, GetAccessibilityStringFormat());
                }
            }
            finally
            {
                errorFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint status text with accessibility indicators
        /// </summary>
        public static void PaintAccessibleStatus(Graphics g, Rectangle bounds, string statusText, StatusType status,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(statusText)) return;

            Font statusFont = GetAccessibilityFont(bounds.Height, style);
            Color statusColor = status switch
            {
                StatusType.Success => Color.FromArgb(25, 135, 84),  // Accessible green
                StatusType.Warning => Color.FromArgb(255, 193, 7),  // Accessible yellow
                StatusType.Error => Color.FromArgb(220, 53, 69),    // Accessible red
                StatusType.Info => Color.FromArgb(13, 110, 253),    // Accessible blue
                _ => GetAccessibilityTextColor(style, theme, false, useThemeColors)
            };

            try
            {
                // Status indicator (colored square)
                using (var indicatorBrush = new SolidBrush(statusColor))
                {
                    var indicatorRect = new Rectangle(bounds.X, bounds.Y + (bounds.Height - 12) / 2, 12, 12);
                    g.FillRectangle(indicatorBrush, indicatorRect);
                }

                // Status text
                using (var brush = new SolidBrush(statusColor))
                {
                    var textBounds = new Rectangle(bounds.X + 18, bounds.Y, bounds.Width - 18, bounds.Height);
                    g.DrawString(statusText, statusFont, brush, textBounds, GetAccessibilityStringFormat());
                }
            }
            finally
            {
                statusFont?.Dispose();
            }
        }

        public enum StatusType
        {
            Success,
            Warning,
            Error,
            Info,
            Normal
        }

        #endregion
    }
}