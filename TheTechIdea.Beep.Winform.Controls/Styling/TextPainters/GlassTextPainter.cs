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
    /// Specialized text painter for glass, acrylic, and transparency effects
    /// Features glassmorphism, neumorphism, gradient backgrounds, and advanced transparency
    /// </summary>
    public static class GlassTextPainter
    {
        #region Glass Font Families

        private static readonly string[] GlassFonts = {
            "Segoe UI Variable",  // Modern variable font with glass effect
            "SF Pro Display",     // Apple's glass-friendly font
            "Inter",              // Clean, modern glass font
            "Helvetica Neue",     // Clean sans-serif for glass
            "Roboto",             // Material glass variant
            "Open Sans",          // Web glass standard
            "Segoe UI",           // Windows glass
            "Arial"               // Fallback
        };

        private static readonly string[] NeumorphismFonts = {
            "SF Pro Text",        // Apple neumorphism
            "Inter",              // Modern neumorphism
            "Circular",           // Soft UI font
            "Poppins",            // Rounded neumorphism
            "Nunito",             // Soft, rounded
            "Segoe UI",           // Windows soft
            "Arial"               // Fallback
        };

        private static readonly string[] GradientFonts = {
            "Montserrat",         // Modern gradient font
            "Lato",               // Clean gradient display
            "Source Sans Pro",    // Adobe gradient font
            "Roboto",             // Material gradient
            "Open Sans",          // Universal gradient
            "Arial"               // Fallback
        };

        #endregion

        #region Glass Color Schemes

        private static readonly GlassColorScheme[] GlassSchemes = {
            new GlassColorScheme("Glass Acrylic",
                Color.FromArgb(255, 255, 255), // White text
                Color.FromArgb(40, 255, 255, 255), // Semi-transparent white
                Color.FromArgb(80, 100, 150, 255), // Blue glass tint
                Color.FromArgb(20, 0, 0, 0)),      // Subtle shadow

            new GlassColorScheme("Neumorphism Light",
                Color.FromArgb(80, 80, 80),    // Dark gray text
                Color.FromArgb(240, 240, 245), // Light background
                Color.FromArgb(255, 255, 255), // White highlight
                Color.FromArgb(200, 200, 210)), // Light shadow

            new GlassColorScheme("Neumorphism Dark",
                Color.FromArgb(220, 220, 220), // Light text
                Color.FromArgb(40, 40, 45),    // Dark background
                Color.FromArgb(60, 60, 65),    // Dark highlight
                Color.FromArgb(20, 20, 25)),   // Darker shadow

            new GlassColorScheme("Gradient Modern",
                Color.FromArgb(255, 255, 255), // White text
                Color.FromArgb(50, 100, 200),  // Blue gradient start
                Color.FromArgb(200, 50, 150),  // Purple gradient end
                Color.FromArgb(30, 0, 0, 0))   // Soft shadow
        };

        private struct GlassColorScheme
        {
            public string Name;
            public Color Text;
            public Color Background;
            public Color Highlight;
            public Color Shadow;

            public GlassColorScheme(string name, Color text, Color background, Color highlight, Color shadow)
            {
                Name = name;
                Text = text;
                Background = background;
                Highlight = highlight;
                Shadow = shadow;
            }
        }

        #endregion

        #region Main Paint Methods

        /// <summary>
        /// Paint glass text with transparency and blur effects
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Configure graphics for glass rendering
            ConfigureGlassGraphics(g);

            // Get glass colors and font
            Color textColor = GetGlassTextColor(style, theme, isFocused, useThemeColors);
            Font font = GetGlassFont(bounds.Height, style);

            try
            {
                // Apply glass-specific rendering
                switch (style)
                {
                    case BeepControlStyle.GlassAcrylic:
                        PaintGlassAcrylicEffect(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Neumorphism:
                        PaintNeumorphismEffect(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.GradientModern:
                        PaintGradientModernEffect(g, bounds, text, font, textColor, isFocused);
                        break;

                    default:
                        PaintStandardGlassEffect(g, bounds, text, font, textColor, isFocused);
                        break;
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint glass text with frosted glass background
        /// </summary>
        public static void PaintFrostedGlass(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, float opacity = 0.8f)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureGlassGraphics(g);

            Font font = GetGlassFont(bounds.Height, style);
            var scheme = GetGlassScheme(style);

            try
            {
                // Frosted glass background
                using (var frostBrush = new SolidBrush(Color.FromArgb((int)(opacity * 255), scheme.Background)))
                {
                    var frostRect = new Rectangle(bounds.X - 4, bounds.Y - 2, bounds.Width + 8, bounds.Height + 4);
                    g.FillRectangle(frostBrush, frostRect);
                }

                // Subtle border for glass effect
                using (var borderPen = new Pen(Color.FromArgb(60, scheme.Highlight), 1))
                {
                    var borderRect = new Rectangle(bounds.X - 4, bounds.Y - 2, bounds.Width + 8, bounds.Height + 4);
                    g.DrawRectangle(borderPen, borderRect);
                }

                // Text with enhanced readability on glass
                Color enhancedTextColor = Color.FromArgb(255, scheme.Text);
                using (var brush = new SolidBrush(enhancedTextColor))
                {
                    g.DrawString(text, font, brush, bounds, GetGlassStringFormat());
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint text with animated glass shimmer effect
        /// </summary>
        public static void PaintGlassShimmer(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, float shimmerPhase = 0f)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureGlassGraphics(g);

            Font font = GetGlassFont(bounds.Height, style);
            Color textColor = GetGlassTextColor(style, theme, isFocused, useThemeColors);

            try
            {
                // Create shimmer effect using a traveling highlight
                using (var path = new GraphicsPath())
                {
                    path.AddString(text, font.FontFamily, (int)font.Style, font.Size, bounds, StringFormat.GenericTypographic);

                    // Moving shimmer highlight
                    var shimmerX = bounds.X + (int)((Math.Sin(shimmerPhase) * 0.5 + 0.5) * bounds.Width);
                    var shimmerRect = new Rectangle(shimmerX - 20, bounds.Y, 40, bounds.Height);

                    using (var shimmerBrush = new LinearGradientBrush(shimmerRect,
                        Color.FromArgb(0, 255, 255, 255),   // Transparent
                        Color.FromArgb(120, 255, 255, 255), // Semi-transparent white
                        LinearGradientMode.Horizontal))
                    {
                        ColorBlend colorBlend = new ColorBlend();
                        colorBlend.Colors = new Color[] {
                            Color.FromArgb(0, 255, 255, 255),
                            Color.FromArgb(120, 255, 255, 255),
                            Color.FromArgb(0, 255, 255, 255)
                        };
                        colorBlend.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                        shimmerBrush.InterpolationColors = colorBlend;

                        // Apply shimmer to text path
                        g.SetClip(path);
                        g.FillRectangle(shimmerBrush, shimmerRect);
                        g.ResetClip();
                    }

                    // Base text
                    using (var brush = new SolidBrush(textColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        #endregion

        #region Glass Effect Rendering

        private static void PaintGlassAcrylicEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var scheme = GlassSchemes[0]; // Glass Acrylic

            // Glass background with acrylic effect
            using (var acrylicBrush = new LinearGradientBrush(bounds,
                Color.FromArgb(60, 255, 255, 255),  // Top: more transparent
                Color.FromArgb(30, 255, 255, 255),  // Bottom: less transparent
                LinearGradientMode.Vertical))
            {
                var glassRect = new Rectangle(bounds.X - 3, bounds.Y - 1, bounds.Width + 6, bounds.Height + 2);
                g.FillRectangle(acrylicBrush, glassRect);
            }

            // Glass border effect
            using (var glassPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1))
            {
                var borderRect = new Rectangle(bounds.X - 3, bounds.Y - 1, bounds.Width + 6, bounds.Height + 2);
                g.DrawRectangle(glassPen, borderRect);
            }

            // Text with subtle shadow for depth
            if (isFocused)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                {
                    var shadowBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width, bounds.Height);
                    g.DrawString(text, font, shadowBrush, shadowBounds, GetGlassStringFormat());
                }
            }

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetGlassStringFormat());
            }
        }

        private static void PaintNeumorphismEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var scheme = GlassSchemes[1]; // Neumorphism Light

            // Neumorphism soft shadow and highlight
            if (isFocused)
            {
                // Soft outer shadow (bottom-right)
                using (var shadowBrush = new SolidBrush(scheme.Shadow))
                {
                    var shadowBounds = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width, bounds.Height);
                    g.DrawString(text, font, shadowBrush, shadowBounds, GetGlassStringFormat());
                }

                // Soft highlight (top-left)
                using (var highlightBrush = new SolidBrush(scheme.Highlight))
                {
                    var highlightBounds = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width, bounds.Height);
                    g.DrawString(text, font, highlightBrush, highlightBounds, GetGlassStringFormat());
                }
            }

            // Main text
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetGlassStringFormat());
            }
        }

        private static void PaintGradientModernEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var scheme = GlassSchemes[3]; // Gradient Modern

            // Create modern gradient text
            using (var path = new GraphicsPath())
            {
                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, bounds, StringFormat.GenericTypographic);

                // Modern gradient fill
                using (var gradientBrush = new LinearGradientBrush(bounds,
                    scheme.Background,  // Blue start
                    scheme.Highlight,   // Purple end
                    LinearGradientMode.Horizontal))
                {
                    // Add color stops for modern gradient
                    ColorBlend colorBlend = new ColorBlend();
                    colorBlend.Colors = new Color[] {
                        scheme.Background,
                        Color.FromArgb(150, 75, 175), // Middle purple
                        scheme.Highlight
                    };
                    colorBlend.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                    gradientBrush.InterpolationColors = colorBlend;

                    g.FillPath(gradientBrush, path);
                }

                // Add subtle white highlight for modern look
                if (isFocused)
                {
                    using (var highlightBrush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                    {
                        using (var highlightPath = new GraphicsPath())
                        {
                            var highlightSize = font.Size * 0.95f;
                            var highlightFont = new Font(font.FontFamily, highlightSize, font.Style);
                            highlightPath.AddString(text, highlightFont.FontFamily, (int)highlightFont.Style, 
                                highlightFont.Size, bounds, StringFormat.GenericTypographic);
                            g.FillPath(highlightBrush, highlightPath);
                            highlightFont.Dispose();
                        }
                    }
                }
            }
        }

        private static void PaintStandardGlassEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Standard glass with subtle transparency
            if (isFocused)
            {
                // Subtle glass background
                using (var glassBrush = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
                {
                    var glassRect = new Rectangle(bounds.X - 2, bounds.Y - 1, bounds.Width + 4, bounds.Height + 2);
                    g.FillRectangle(glassBrush, glassRect);
                }
            }

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetGlassStringFormat());
            }
        }

        #endregion

        #region Font Management

        private static Font GetGlassFont(int height, BeepControlStyle style)
        {
            float fontSize = Math.Max(8, height * 0.6f);
            FontStyle fontStyle = FontStyle.Regular;

            // Style-specific font selection
            string[] fontFamily = style switch
            {
                BeepControlStyle.GlassAcrylic => GlassFonts,
                BeepControlStyle.Neumorphism => NeumorphismFonts,
                BeepControlStyle.GradientModern => GradientFonts,
                _ => GlassFonts
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

        #region Glass Colors

        private static Color GetGlassTextColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                // Ensure good contrast for glass effects
                Color themeColor = isFocused ? theme.AccentColor : theme.SecondaryTextColor;
                return Color.FromArgb(Math.Max(200, (int)themeColor.A), themeColor.R, themeColor.G, themeColor.B);
            }

            var scheme = GetGlassScheme(style);
            return isFocused ? scheme.Highlight : scheme.Text;
        }

        private static GlassColorScheme GetGlassScheme(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.GlassAcrylic => GlassSchemes[0],    // Glass Acrylic
                BeepControlStyle.Neumorphism => GlassSchemes[1],     // Neumorphism Light
                BeepControlStyle.GradientModern => GlassSchemes[3],  // Gradient Modern
                _ => GlassSchemes[0]                                 // Default to Glass Acrylic
            };
        }

        #endregion

        #region Helper Methods

        private static void ConfigureGlassGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingMode = CompositingMode.SourceOver; // Important for transparency
        }

        private static StringFormat GetGlassStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        #endregion

        #region Glass-Specific Typography Variants

        /// <summary>
        /// Paint glass button text with enhanced glass effect
        /// </summary>
        public static void PaintGlassButton(Graphics g, Rectangle bounds, string text, bool isPressed, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font buttonFont = GetGlassFont(bounds.Height, style);
            Color buttonColor = GetGlassTextColor(style, theme, isFocused, useThemeColors);
            var scheme = GetGlassScheme(style);

            try
            {
                // Glass button background
                Color bgColor = isPressed ? 
                    Color.FromArgb(80, scheme.Background) : 
                    Color.FromArgb(40, scheme.Background);

                using (var buttonBrush = new LinearGradientBrush(bounds, bgColor, 
                    Color.FromArgb(20, scheme.Background), LinearGradientMode.Vertical))
                {
                    var buttonRect = new Rectangle(bounds.X - 4, bounds.Y - 2, bounds.Width + 8, bounds.Height + 4);
                    g.FillRectangle(buttonBrush, buttonRect);
                }

                // Glass button border
                using (var borderPen = new Pen(Color.FromArgb(100, scheme.Highlight), 1))
                {
                    var buttonRect = new Rectangle(bounds.X - 4, bounds.Y - 2, bounds.Width + 8, bounds.Height + 4);
                    g.DrawRectangle(borderPen, buttonRect);
                }

                // Button text
                Color textColor = isPressed ? 
                    Color.FromArgb(200, buttonColor) : 
                    buttonColor;

                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(text, buttonFont, brush, bounds, GetGlassStringFormat());
                }
            }
            finally
            {
                buttonFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint glass card title with enhanced readability
        /// </summary>
        public static void PaintGlassCardTitle(Graphics g, Rectangle bounds, string title, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(title)) return;

            Font titleFont = GetGlassFont((int)(bounds.Height * 1.1f), style);
            var scheme = GetGlassScheme(style);

            try
            {
                // Enhanced readability background for titles
                using (var titleBackBrush = new SolidBrush(Color.FromArgb(60, scheme.Background)))
                {
                    var titleRect = new Rectangle(bounds.X - 6, bounds.Y - 3, bounds.Width + 12, bounds.Height + 6);
                    g.FillRectangle(titleBackBrush, titleRect);
                }

                // Title text with enhanced contrast
                Color titleColor = Color.FromArgb(255, scheme.Text);
                using (var brush = new SolidBrush(titleColor))
                {
                    g.DrawString(title, titleFont, brush, bounds, GetGlassStringFormat());
                }

                // Subtle underline for glass card titles
                using (var underlinePen = new Pen(Color.FromArgb(80, scheme.Highlight), 1))
                {
                    g.DrawLine(underlinePen, bounds.X, bounds.Bottom + 2, bounds.Right, bounds.Bottom + 2);
                }
            }
            finally
            {
                titleFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint glass overlay text with backdrop blur simulation
        /// </summary>
        public static void PaintGlassOverlay(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false, float blurIntensity = 0.5f)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font overlayFont = GetGlassFont(bounds.Height, style);
            var scheme = GetGlassScheme(style);

            try
            {
                // Simulate backdrop blur with layered semi-transparent rectangles
                for (int i = 0; i < 3; i++)
                {
                    int alpha = (int)(blurIntensity * 30 * (3 - i));
                    using (var blurBrush = new SolidBrush(Color.FromArgb(alpha, scheme.Background)))
                    {
                        var blurRect = new Rectangle(bounds.X - i - 2, bounds.Y - i - 1, 
                            bounds.Width + (i + 2) * 2, bounds.Height + (i + 1) * 2);
                        g.FillRectangle(blurBrush, blurRect);
                    }
                }

                // Overlay border
                using (var borderPen = new Pen(Color.FromArgb(60, scheme.Highlight), 1))
                {
                    var borderRect = new Rectangle(bounds.X - 5, bounds.Y - 3, bounds.Width + 10, bounds.Height + 6);
                    g.DrawRectangle(borderPen, borderRect);
                }

                // High contrast overlay text
                Color overlayColor = Color.FromArgb(255, scheme.Text);
                using (var brush = new SolidBrush(overlayColor))
                {
                    g.DrawString(text, overlayFont, brush, bounds, GetGlassStringFormat());
                }
            }
            finally
            {
                overlayFont?.Dispose();
            }
        }

        #endregion
    }
}