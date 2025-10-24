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
    /// Specialized text painter for visual effects and animated interfaces
    /// Features neon effects, neo-brutalist styling, and custom visual animations
    /// </summary>
    public static class EffectTextPainter
    {
        #region Effect Font Families

        private static readonly string[] NeonFonts = {
            "Orbitron",           // Sci-fi neon font
            "Exo 2",              // Modern tech font
            "Rajdhani",           // Clean futuristic
            "Space Mono",         // Monospace tech
            "JetBrains Mono",     // Code/tech font
            "Consolas",           // Monospace fallback
            "Arial Black",        // Bold fallback
            "Arial"               // Final fallback
        };

        private static readonly string[] NeoBrutalistFonts = {
            "Impact",             // Bold, aggressive
            "Arial Black",        // Heavy weight
            "Helvetica Bold",     // Strong sans-serif
            "Bebas Neue",         // Condensed bold
            "Oswald",             // Strong display font
            "Roboto Black",       // Heavy Material font
            "Segoe UI Black",     // Windows bold
            "Arial"               // Fallback
        };

        private static readonly string[] EffectFonts = {
            "Courier New",        // Classic monospace for effects
            "Lucida Console",     // Clean monospace
            "Consolas",           // Modern monospace
            "Monaco",             // Mac monospace
            "DejaVu Sans Mono",   // Cross-platform
            "Arial"               // Fallback
        };

        #endregion

        #region Effect Color Schemes

        private static readonly EffectColorScheme[] EffectSchemes = {
            new EffectColorScheme("Vibrant Neon",
                Color.FromArgb(255, 0, 255),   // Magenta primary
                Color.FromArgb(0, 255, 255),   // Cyan secondary
                Color.FromArgb(255, 255, 0),   // Yellow accent
                Color.FromArgb(0, 0, 0)),      // Black background

            new EffectColorScheme("Neo-Brutalist",
                Color.FromArgb(0, 0, 0),       // Black primary
                Color.FromArgb(255, 255, 255), // White secondary
                Color.FromArgb(255, 0, 0),     // Red accent
                Color.FromArgb(255, 255, 0)),  // Yellow background

            new EffectColorScheme("Electric Blue",
                Color.FromArgb(0, 150, 255),   // Electric blue
                Color.FromArgb(100, 200, 255), // Light blue
                Color.FromArgb(255, 255, 255), // White accent
                Color.FromArgb(0, 0, 30)),     // Dark blue background

            new EffectColorScheme("Gaming RGB",
                Color.FromArgb(255, 100, 255), // Purple-pink
                Color.FromArgb(100, 255, 255), // Cyan
                Color.FromArgb(255, 255, 100), // Yellow
                Color.FromArgb(20, 20, 20))    // Dark background
        };

        private struct EffectColorScheme
        {
            public string Name;
            public Color Primary;
            public Color Secondary;
            public Color Accent;
            public Color Background;

            public EffectColorScheme(string name, Color primary, Color secondary, Color accent, Color background)
            {
                Name = name;
                Primary = primary;
                Secondary = secondary;
                Accent = accent;
                Background = background;
            }
        }

        #endregion

        #region Main Paint Methods

        /// <summary>
        /// Paint effect text with visual effects and animations
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            // Configure graphics for effect rendering
            ConfigureEffectGraphics(g);

            // Get effect colors and font
            Color textColor = GetEffectTextColor(style, theme, isFocused, useThemeColors);
            Font font = GetEffectFont(bounds.Height, style);

            try
            {
                // Apply effect-specific rendering
                switch (style)
                {
                    case BeepControlStyle.Neon:
                        PaintNeonEffect(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.NeoBrutalist:
                        PaintNeoBrutalistEffect(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Effect:
                        PaintCustomEffect(g, bounds, text, font, textColor, isFocused);
                        break;

                    case BeepControlStyle.Gaming:
                        PaintGamingEffect(g, bounds, text, font, textColor, isFocused);
                        break;

                    default:
                        PaintStandardEffect(g, bounds, text, font, textColor, isFocused);
                        break;
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint text with animated rainbow effect
        /// </summary>
        public static void PaintRainbowEffect(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, float animationPhase = 0f)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureEffectGraphics(g);

            Font font = GetEffectFont(bounds.Height, style);

            try
            {
                // Create rainbow gradient path
                using (var path = new GraphicsPath())
                {
                    path.AddString(text, font.FontFamily, (int)font.Style, font.Size, bounds, StringFormat.GenericTypographic);

                    // Animated rainbow gradient
                    Color[] rainbowColors = GetRainbowColors(animationPhase);
                    
                    using (var brush = new LinearGradientBrush(bounds, rainbowColors[0], rainbowColors[1], LinearGradientMode.Horizontal))
                    {
                        ColorBlend colorBlend = new ColorBlend();
                        colorBlend.Colors = rainbowColors;
                        colorBlend.Positions = new float[] { 0.0f, 0.16f, 0.33f, 0.5f, 0.66f, 0.83f, 1.0f };
                        brush.InterpolationColors = colorBlend;

                        g.FillPath(brush, path);
                    }

                    // Add glow effect
                    if (isFocused)
                    {
                        using (var glowPen = new Pen(Color.FromArgb(100, Color.White), 3))
                        {
                            g.DrawPath(glowPen, path);
                        }
                    }
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        /// <summary>
        /// Paint text with pulsing glow effect
        /// </summary>
        public static void PaintPulsingGlow(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, float pulsePhase = 0f)
        {
            if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            ConfigureEffectGraphics(g);

            Font font = GetEffectFont(bounds.Height, style);
            Color baseColor = GetEffectTextColor(style, theme, isFocused, useThemeColors);

            try
            {
                // Calculate pulse intensity
                float pulseIntensity = (float)(Math.Sin(pulsePhase) * 0.5 + 0.5); // 0.0 to 1.0
                int glowAlpha = (int)(pulseIntensity * 150 + 50); // 50 to 200
                int glowRadius = (int)(pulseIntensity * 8 + 2);   // 2 to 10

                // Multi-layer pulsing glow
                for (int i = glowRadius; i >= 1; i--)
                {
                    int layerAlpha = glowAlpha / (i + 1);
                    using (var glowBrush = new SolidBrush(Color.FromArgb(layerAlpha, baseColor)))
                    {
                        var glowBounds = new Rectangle(bounds.X - i, bounds.Y - i, bounds.Width + (i * 2), bounds.Height + (i * 2));
                        g.DrawString(text, font, glowBrush, glowBounds, GetEffectStringFormat());
                    }
                }

                // Main text
                using (var brush = new SolidBrush(baseColor))
                {
                    g.DrawString(text, font, brush, bounds, GetEffectStringFormat());
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        #endregion

        #region Effect Style Rendering

        private static void PaintNeonEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var scheme = EffectSchemes[0]; // Vibrant Neon

            // Create neon tube effect with multiple glow layers
            using (var path = new GraphicsPath())
            {
                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, bounds, StringFormat.GenericTypographic);

                // Outer glow layers (neon tube effect)
                Color[] neonColors = { scheme.Primary, scheme.Secondary, scheme.Accent };
                for (int layer = 0; layer < neonColors.Length; layer++)
                {
                    for (int i = 8 - layer * 2; i >= 1; i--)
                    {
                        int alpha = Math.Max(10, 80 - (i * 8) - (layer * 20));
                        using (var pen = new Pen(Color.FromArgb(alpha, neonColors[layer]), i))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }

                // Bright neon core
                using (var brush = new SolidBrush(Color.FromArgb(255, 255, 255)))
                {
                    using (var corePath = new GraphicsPath())
                    {
                        var coreSize = font.Size * 0.9f;
                        var coreFont = new Font(font.FontFamily, coreSize, font.Style);
                        corePath.AddString(text, coreFont.FontFamily, (int)coreFont.Style, coreFont.Size, bounds, StringFormat.GenericTypographic);
                        g.FillPath(brush, corePath);
                        coreFont.Dispose();
                    }
                }
            }
        }

        private static void PaintNeoBrutalistEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var scheme = EffectSchemes[1]; // Neo-Brutalist

            // Neo-brutalist: Thick black outlines, bold colors, no anti-aliasing
            var oldSmoothing = g.SmoothingMode;
            var oldHint = g.TextRenderingHint;
            
            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            try
            {
                // Thick black outline (brutalist shadow)
                using (var outlineBrush = new SolidBrush(scheme.Primary))
                {
                    for (int x = -3; x <= 3; x++)
                    {
                        for (int y = -3; y <= 3; y++)
                        {
                            if (x == 0 && y == 0) continue;
                            var outlineBounds = new Rectangle(bounds.X + x, bounds.Y + y, bounds.Width, bounds.Height);
                            g.DrawString(text, font, outlineBrush, outlineBounds, GetEffectStringFormat());
                        }
                    }
                }

                // Main text in contrasting color
                Color mainColor = isFocused ? scheme.Accent : scheme.Secondary;
                using (var brush = new SolidBrush(mainColor))
                {
                    g.DrawString(text, font, brush, bounds, GetEffectStringFormat());
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldHint;
            }
        }

        private static void PaintCustomEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Custom matrix-Style digital rain effect
            var scheme = EffectSchemes[2]; // Electric Blue

            // Digital glitch effect
            Random random = new Random(text.GetHashCode());
            
            for (int i = 0; i < 3; i++)
            {
                int offsetX = random.Next(-2, 3);
                int offsetY = random.Next(-1, 2);
                int alpha = 60 - (i * 20);
                
                Color glitchColor = i switch
                {
                    0 => Color.FromArgb(alpha, 255, 0, 0),   // Red channel
                    1 => Color.FromArgb(alpha, 0, 255, 0),   // Green channel
                    2 => Color.FromArgb(alpha, 0, 0, 255),   // Blue channel
                    _ => textColor
                };

                using (var brush = new SolidBrush(glitchColor))
                {
                    var glitchBounds = new Rectangle(bounds.X + offsetX, bounds.Y + offsetY, bounds.Width, bounds.Height);
                    g.DrawString(text, font, brush, glitchBounds, GetEffectStringFormat());
                }
            }

            // Main text
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetEffectStringFormat());
            }
        }

        private static void PaintGamingEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            var scheme = EffectSchemes[3]; // Gaming RGB

            // Gaming-Style RGB split effect
            if (isFocused)
            {
                // RGB chromatic aberration
                using (var redBrush = new SolidBrush(Color.FromArgb(80, 255, 0, 0)))
                {
                    var redBounds = new Rectangle(bounds.X - 1, bounds.Y, bounds.Width, bounds.Height);
                    g.DrawString(text, font, redBrush, redBounds, GetEffectStringFormat());
                }

                using (var blueBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 255)))
                {
                    var blueBounds = new Rectangle(bounds.X + 1, bounds.Y, bounds.Width, bounds.Height);
                    g.DrawString(text, font, blueBrush, blueBounds, GetEffectStringFormat());
                }
            }

            // Main gaming text
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetEffectStringFormat());
            }
        }

        private static void PaintStandardEffect(Graphics g, Rectangle bounds, string text, Font font,
            Color textColor, bool isFocused)
        {
            // Standard effect with subtle glow
            if (isFocused)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(50, textColor)))
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        var glowBounds = new Rectangle(bounds.X - i, bounds.Y - i, bounds.Width + (i * 2), bounds.Height + (i * 2));
                        g.DrawString(text, font, glowBrush, glowBounds, GetEffectStringFormat());
                    }
                }
            }

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, brush, bounds, GetEffectStringFormat());
            }
        }

        #endregion

        #region Font Management

        private static Font GetEffectFont(int height, BeepControlStyle style)
        {
            float fontSize = Math.Max(8, height * 0.6f);
            FontStyle fontStyle = FontStyle.Regular;

            // Style-specific font selection and styling
            string[] fontFamily = style switch
            {
                BeepControlStyle.Neon => NeonFonts,
                BeepControlStyle.NeoBrutalist => NeoBrutalistFonts,
                BeepControlStyle.Gaming => NeonFonts,
                _ => EffectFonts
            };

            // Neo-brutalist uses bold fonts
            if (style == BeepControlStyle.NeoBrutalist)
            {
                fontStyle = FontStyle.Bold;
                fontSize = Math.Max(10, height * 0.65f);
            }

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

        #region Effect Colors

        private static Color GetEffectTextColor(BeepControlStyle style, IBeepTheme theme, bool isFocused, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                return isFocused ? theme.AccentColor : theme.SecondaryTextColor;
            }

            var scheme = GetEffectScheme(style);
            return isFocused ? scheme.Accent : scheme.Primary;
        }

        private static EffectColorScheme GetEffectScheme(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Neon => EffectSchemes[0],        // Vibrant Neon
                BeepControlStyle.NeoBrutalist => EffectSchemes[1], // Neo-Brutalist
                BeepControlStyle.Effect => EffectSchemes[2],      // Electric Blue
                BeepControlStyle.Gaming => EffectSchemes[3],      // Gaming RGB
                _ => EffectSchemes[0]                             // Default to Neon
            };
        }

        private static Color[] GetRainbowColors(float phase)
        {
            // Generate rainbow colors with phase offset for animation
            return new Color[]
            {
                Color.FromArgb(255, (int)(Math.Sin(phase) * 127 + 128), 0),                    // Red
                Color.FromArgb(255, 165, (int)(Math.Sin(phase + 1) * 127 + 128)),             // Orange  
                Color.FromArgb(255, 255, (int)(Math.Sin(phase + 2) * 127 + 128)),             // Yellow
                Color.FromArgb((int)(Math.Sin(phase + 3) * 127 + 128), 255, 0),               // Green
                Color.FromArgb(0, (int)(Math.Sin(phase + 4) * 127 + 128), 255),               // Blue
                Color.FromArgb(75, 0, (int)(Math.Sin(phase + 5) * 127 + 128)),                // Indigo
                Color.FromArgb(148, 0, (int)(Math.Sin(phase + 6) * 127 + 128))                // Violet
            };
        }

        #endregion

        #region Helper Methods

        private static void ConfigureEffectGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        private static StringFormat GetEffectStringFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        #endregion

        #region Effect-Specific Typography Variants

        /// <summary>
        /// Paint text with matrix digital rain effect
        /// </summary>
        public static void PaintMatrixEffect(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font matrixFont = GetEffectFont(bounds.Height, style);
            Color matrixGreen = Color.FromArgb(0, 255, 100);

            try
            {
                // Matrix-Style trailing effect
                for (int i = 5; i >= 0; i--)
                {
                    int alpha = 255 - (i * 40);
                    using (var brush = new SolidBrush(Color.FromArgb(alpha, matrixGreen)))
                    {
                        var trailBounds = new Rectangle(bounds.X, bounds.Y + i, bounds.Width, bounds.Height);
                        g.DrawString(text, matrixFont, brush, trailBounds, GetEffectStringFormat());
                    }
                }
            }
            finally
            {
                matrixFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint text with electric arc effect
        /// </summary>
        public static void PaintElectricArc(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font arcFont = GetEffectFont(bounds.Height, style);
            Color electricBlue = Color.FromArgb(100, 150, 255);

            try
            {
                // Electric arc effect with jagged lines
                Random random = new Random(text.GetHashCode());
                
                using (var arcPen = new Pen(Color.FromArgb(150, electricBlue), 2))
                {
                    // Draw random electric arcs around text
                    for (int i = 0; i < 3; i++)
                    {
                        Point start = new Point(
                            bounds.X + random.Next(bounds.Width),
                            bounds.Y + random.Next(bounds.Height)
                        );
                        Point end = new Point(
                            bounds.X + random.Next(bounds.Width),
                            bounds.Y + random.Next(bounds.Height)
                        );
                        
                        g.DrawLine(arcPen, start, end);
                    }
                }

                // Main electric text
                using (var brush = new SolidBrush(electricBlue))
                {
                    g.DrawString(text, arcFont, brush, bounds, GetEffectStringFormat());
                }
            }
            finally
            {
                arcFont?.Dispose();
            }
        }

        /// <summary>
        /// Paint text with holographic shimmer effect
        /// </summary>
        public static void PaintHolographicShimmer(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors = false, float shimmerPhase = 0f)
        {
            if (string.IsNullOrEmpty(text)) return;

            Font holoFont = GetEffectFont(bounds.Height, style);

            try
            {
                // Holographic shimmer with shifting colors
                using (var path = new GraphicsPath())
                {
                    path.AddString(text, holoFont.FontFamily, (int)holoFont.Style, holoFont.Size, bounds, StringFormat.GenericTypographic);

                    // Create shifting gradient for holographic effect
                    var shimmerRect = new Rectangle(
                        bounds.X - (int)(Math.Sin(shimmerPhase) * 50),
                        bounds.Y,
                        bounds.Width + 100,
                        bounds.Height
                    );

                    using (var shimmerBrush = new LinearGradientBrush(shimmerRect,
                        Color.FromArgb(255, 100, 255), Color.FromArgb(100, 255, 255), LinearGradientMode.Horizontal))
                    {
                        ColorBlend colorBlend = new ColorBlend();
                        colorBlend.Colors = new Color[] {
                            Color.FromArgb(255, 100, 255), // Magenta
                            Color.FromArgb(100, 255, 255), // Cyan
                            Color.FromArgb(255, 255, 100), // Yellow
                            Color.FromArgb(255, 100, 255)  // Magenta
                        };
                        colorBlend.Positions = new float[] { 0.0f, 0.33f, 0.66f, 1.0f };
                        shimmerBrush.InterpolationColors = colorBlend;

                        g.FillPath(shimmerBrush, path);
                    }
                }
            }
            finally
            {
                holoFont?.Dispose();
            }
        }

        #endregion
    }
}