using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks
{
    /// <summary>
    /// Metrics used by dock painters to configure colors, sizes, and visual properties.
    /// Similar to FormPainterMetrics, this centralizes all dock styling configuration.
    /// </summary>
    public sealed class DockPainterMetrics
    {
        // Theme integration
        public bool UseThemeColors { get; set; } = false;
        public IBeepTheme? BeepTheme { get; set; }

        // Background colors
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 240, 240, 240);
        public Color BorderColor { get; set; } = Color.FromArgb(255, 200, 200, 200);
        public float BackgroundOpacity { get; set; } = 0.85f;
        public float BackgroundBlur { get; set; } = 10f;

        // Item colors
        public Color ItemBackgroundColor { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public Color ItemHoverColor { get; set; } = Color.FromArgb(255, 230, 230, 230);
        public Color ItemSelectedColor { get; set; } = Color.FromArgb(255, 200, 200, 200);
        public Color ItemPressedColor { get; set; } = Color.FromArgb(255, 180, 180, 180);

        // Icon and text colors
        public Color IconColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color IconHoverColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color TextColor { get; set; } = Color.FromArgb(255, 0, 0, 0);

        // Badge colors
        public Color BadgeBackgroundColor { get; set; } = Color.FromArgb(255, 255, 76, 76);
        public Color BadgeForegroundColor { get; set; } = Color.White;

        // Indicator colors
        public Color IndicatorColor { get; set; } = Color.FromArgb(255, 0, 122, 255);
        public Color RunningIndicatorColor { get; set; } = Color.FromArgb(255, 0, 200, 100);
        public Color AttentionIndicatorColor { get; set; } = Color.FromArgb(255, 255, 150, 0);

        // Shadow and glow
        public Color ShadowColor { get; set; } = Color.FromArgb(100, 0, 0, 0);
        public Color GlowColor { get; set; } = Color.FromArgb(150, 0, 122, 255);
        public int ShadowBlur { get; set; } = 8;
        public int GlowBlur { get; set; } = 12;

        // Accent colors (for styles with accent bars/borders)
        public Color AccentColor { get; set; } = Color.FromArgb(255, 0, 122, 255);
        public Color AccentHoverColor { get; set; } = Color.FromArgb(255, 0, 150, 255);

        // Dimensions
        public int ItemSize { get; set; } = 56;
        public int ItemSpacing { get; set; } = 8;
        public int ItemPadding { get; set; } = 4;
        public int CornerRadius { get; set; } = 12;
        public int ItemCornerRadius { get; set; } = 8;
        public int BorderWidth { get; set; } = 1;
        public int IndicatorSize { get; set; } = 4;
        public int IndicatorOffset { get; set; } = 6;

        // Animation and effects
        public float MaxScale { get; set; } = 1.5f;
        public float HoverScale { get; set; } = 1.2f;
        public float AnimationSpeed { get; set; } = 0.2f;
        public bool ShowReflection { get; set; } = false;
        public bool ShowShadow { get; set; } = true;
        public bool ShowGlow { get; set; } = false;
        public bool ShowBorder { get; set; } = true;

        // Separator
        public Color SeparatorColor { get; set; } = Color.FromArgb(100, 200, 200, 200);
        public int SeparatorWidth { get; set; } = 1;

        /// <summary>
        /// Helper to blend two colors
        /// </summary>
        private static Color Blend(Color a, Color b, double t)
        {
            int r = (int)(a.R * (1 - t) + b.R * t);
            int g = (int)(a.G * (1 - t) + b.G * t);
            int bch = (int)(a.B * (1 - t) + b.B * t);
            int alpha = (int)(a.A * (1 - t) + b.A * t);
            return Color.FromArgb(alpha, r, g, bch);
        }

        /// <summary>
        /// Helper to calculate luminance
        /// </summary>
        private static double Luma(Color c)
        {
            return 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;
        }

        /// <summary>
        /// Gets default metrics for a specific dock style
        /// </summary>
        public static DockPainterMetrics DefaultFor(DockStyle style, IBeepTheme? theme = null, bool useThemeColors = false)
        {
            var m = new DockPainterMetrics();

            // Set base metrics based on style
            switch (style)
            {
                case DockStyle.AppleDock:
                    m.ItemSize = 56;
                    m.ItemSpacing = 8;
                    m.CornerRadius = 16;
                    m.ItemCornerRadius = 8;
                    m.BackgroundOpacity = 0.85f;
                    m.ShowReflection = true;
                    m.ShowShadow = true;
                    m.ShowGlow = false;
                    m.MaxScale = 1.8f;
                    m.BackgroundColor = Color.FromArgb(220, 240, 240, 245);
                    m.BorderColor = Color.FromArgb(180, 200, 200, 200);
                    m.ItemBackgroundColor = Color.FromArgb(200, 255, 255, 255);
                    break;

                case DockStyle.Windows11Dock:
                    m.ItemSize = 48;
                    m.ItemSpacing = 4;
                    m.CornerRadius = 12;
                    m.ItemCornerRadius = 6;
                    m.BackgroundOpacity = 0.9f;
                    m.ShowReflection = false;
                    m.ShowShadow = true;
                    m.ShowGlow = false;
                    m.MaxScale = 1.3f;
                    m.BackgroundColor = Color.FromArgb(230, 248, 248, 248);
                    m.BorderColor = Color.FromArgb(200, 220, 220, 220);
                    break;

                case DockStyle.Material3Dock:
                    m.ItemSize = 52;
                    m.ItemSpacing = 6;
                    m.CornerRadius = 16;
                    m.ItemCornerRadius = 12;
                    m.BackgroundOpacity = 0.95f;
                    m.ShowReflection = false;
                    m.ShowShadow = true;
                    m.ShowGlow = false;
                    m.MaxScale = 1.4f;
                    m.BackgroundColor = Color.FromArgb(240, 245, 245, 250);
                    m.BorderColor = Color.FromArgb(200, 210, 210, 220);
                    m.ShadowBlur = 12;
                    break;

                case DockStyle.MinimalDock:
                    m.ItemSize = 48;
                    m.ItemSpacing = 12;
                    m.CornerRadius = 8;
                    m.ItemCornerRadius = 6;
                    m.BackgroundOpacity = 0.95f;
                    m.ShowReflection = false;
                    m.ShowShadow = false;
                    m.ShowGlow = false;
                    m.MaxScale = 1.2f;
                    m.BackgroundColor = Color.FromArgb(245, 250, 250, 250);
                    m.BorderColor = Color.FromArgb(220, 230, 230, 230);
                    break;

                case DockStyle.GlassmorphismDock:
                    m.ItemSize = 54;
                    m.ItemSpacing = 8;
                    m.CornerRadius = 20;
                    m.ItemCornerRadius = 12;
                    m.BackgroundOpacity = 0.6f;
                    m.BackgroundBlur = 20f;
                    m.ShowReflection = false;
                    m.ShowShadow = true;
                    m.ShowGlow = true;
                    m.MaxScale = 1.5f;
                    m.BackgroundColor = Color.FromArgb(150, 255, 255, 255);
                    m.BorderColor = Color.FromArgb(100, 255, 255, 255);
                    m.GlowColor = Color.FromArgb(100, 150, 200, 255);
                    break;

                case DockStyle.PillDock:
                    m.ItemSize = 56;
                    m.ItemSpacing = 16;
                    m.CornerRadius = 28;
                    m.ItemCornerRadius = 28;
                    m.BackgroundOpacity = 0.9f;
                    m.ShowReflection = false;
                    m.ShowShadow = true;
                    m.ShowGlow = false;
                    m.MaxScale = 1.3f;
                    m.BackgroundColor = Color.FromArgb(230, 245, 245, 250);
                    m.BorderColor = Color.FromArgb(200, 220, 220, 230);
                    m.ShadowBlur = 16;
                    break;

                case DockStyle.NeumorphismDock:
                    m.ItemSize = 52;
                    m.ItemSpacing = 10;
                    m.CornerRadius = 16;
                    m.ItemCornerRadius = 12;
                    m.BackgroundOpacity = 1.0f;
                    m.ShowReflection = false;
                    m.ShowShadow = true;
                    m.ShowGlow = false;
                    m.MaxScale = 1.2f;
                    m.BackgroundColor = Color.FromArgb(255, 230, 230, 235);
                    m.BorderColor = Color.FromArgb(255, 230, 230, 235);
                    m.ShadowColor = Color.FromArgb(80, 150, 150, 160);
                    break;

                case DockStyle.NeonDock:
                    m.ItemSize = 50;
                    m.ItemSpacing = 12;
                    m.CornerRadius = 12;
                    m.ItemCornerRadius = 8;
                    m.BackgroundOpacity = 0.8f;
                    m.ShowReflection = false;
                    m.ShowShadow = false;
                    m.ShowGlow = true;
                    m.MaxScale = 1.4f;
                    m.BackgroundColor = Color.FromArgb(200, 20, 20, 30);
                    m.BorderColor = Color.FromArgb(255, 0, 255, 255);
                    m.GlowColor = Color.FromArgb(200, 0, 255, 255);
                    m.GlowBlur = 20;
                    break;

                default:
                    // Use AppleDock as default
                    return DefaultFor(DockStyle.AppleDock, theme, useThemeColors);
            }

            // Apply theme colors if requested
            if (useThemeColors && theme != null)
            {
                m.UseThemeColors = true;
                m.BeepTheme = theme;

                // Map theme colors to dock colors
                m.BackgroundColor = theme.BackColor;
                m.BorderColor = theme.BorderColor;
                m.ItemBackgroundColor = theme.ButtonBackColor;
                m.ItemHoverColor = theme.ButtonHoverBackColor;
                m.ItemSelectedColor = theme.ButtonSelectedBackColor;
                m.ItemPressedColor = theme.ButtonPressedBackColor;
                
                m.IconColor = theme.ForeColor;
                m.IconHoverColor = theme.ButtonHoverForeColor;
                m.TextColor = theme.ForeColor;
                
                m.IndicatorColor = theme.AccentColor;
                m.AccentColor = theme.AccentColor;
                m.AccentHoverColor = Blend(theme.AccentColor, Color.White, 0.2);
                
                m.ShadowColor = Color.FromArgb(100, theme.ShadowColor);
                
                // Adjust opacity for certain styles
                switch (style)
                {
                    case DockStyle.GlassmorphismDock:
                        m.BackgroundColor = Color.FromArgb(150, theme.BackColor);
                        m.BorderColor = Color.FromArgb(100, theme.BorderColor);
                        break;
                    case DockStyle.NeonDock:
                        m.GlowColor = Color.FromArgb(200, theme.AccentColor);
                        m.BorderColor = theme.AccentColor;
                        break;
                }
            }

            return m;
        }

        // Note: DPI-aware metrics method commented out until BeepDock has CurrentTheme and UseThemeColors properties
        /*
        /// <summary>
        /// Gets DPI-aware metrics for the specified dock style
        /// </summary>
        public static DockPainterMetrics DefaultFor(DockStyle style, BeepDock owner)
        {
            if (owner == null || !owner.IsHandleCreated)
            {
                return DefaultFor(style, null, false);
            }

            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(owner);
            var metrics = DefaultFor(style, owner.CurrentTheme, owner.UseThemeColors);

            // Scale size-based properties
            if (!DpiScalingHelper.AreScaleFactorsEqual(dpiScale, 1.0f))
            {
                metrics.ItemSize = DpiScalingHelper.ScaleValue(metrics.ItemSize, dpiScale);
                metrics.ItemSpacing = DpiScalingHelper.ScaleValue(metrics.ItemSpacing, dpiScale);
                metrics.ItemPadding = DpiScalingHelper.ScaleValue(metrics.ItemPadding, dpiScale);
                metrics.CornerRadius = DpiScalingHelper.ScaleValue(metrics.CornerRadius, dpiScale);
                metrics.ItemCornerRadius = DpiScalingHelper.ScaleValue(metrics.ItemCornerRadius, dpiScale);
                metrics.BorderWidth = DpiScalingHelper.ScaleValue(metrics.BorderWidth, dpiScale);
                metrics.IndicatorSize = DpiScalingHelper.ScaleValue(metrics.IndicatorSize, dpiScale);
                metrics.IndicatorOffset = DpiScalingHelper.ScaleValue(metrics.IndicatorOffset, dpiScale);
                metrics.ShadowBlur = DpiScalingHelper.ScaleValue(metrics.ShadowBlur, dpiScale);
                metrics.GlowBlur = DpiScalingHelper.ScaleValue(metrics.GlowBlur, dpiScale);
                metrics.SeparatorWidth = DpiScalingHelper.ScaleValue(metrics.SeparatorWidth, dpiScale);
            }

            return metrics;
        }
        */
    }
}
