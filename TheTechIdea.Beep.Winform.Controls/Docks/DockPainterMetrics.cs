using System.Collections.Generic;
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

        // Dimensions the control lays out with. These lived in DockStyleHelpers as a separate set of
        // switch tables that disagreed with this one for 13 of 17 styles; they are here now so there
        // is one answer to "how big is a Material 3 dock item".
        public int DockHeight { get; set; } = 72;
        public int Padding { get; set; } = 12;
        public float IconSizeRatio { get; set; } = 0.8f;

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
        /// <summary>
        /// The one per-style defaults table for the folder.
        /// </summary>
        /// <remarks>
        /// <para>
        /// There used to be three. <c>DockStyleHelpers</c> held eight switch tables that the control
        /// wrote into <c>DockConfig</c>; this class held a second set in a switch; individual painters
        /// held a third by overwriting metrics after <c>DefaultFor</c> returned. They disagreed on item
        /// size for 13 of 17 styles and on spacing for 12, and the first two were live at the same time -
        /// the layout used one, the painters that asked for metrics used the other.
        /// </para>
        /// <para>
        /// Where the two disagreed, the <c>DockStyleHelpers</c> value won, because that is the one the
        /// control actually laid out with (<c>DockLayoutHelper</c> reads <c>config.ItemSize</c>,
        /// <c>config.Spacing</c> and <c>config.Padding</c>). Taking the other set would have resized
        /// most of the styles as a side effect of a consolidation.
        /// </para>
        /// </remarks>
        private sealed record StyleDefaults(
            int ItemSize, int DockHeight, int Spacing, int Padding,
            float MaxScale, bool ShowShadow, float BackgroundOpacity, float IconSizeRatio,
            int CornerRadius, int ItemCornerRadius,
            Color Background, Color Border,
            bool ShowReflection = false, bool ShowGlow = false,
            int ShadowBlur = 8, int GlowBlur = 12, float BackgroundBlur = 10f,
            Color? ItemBackground = null, Color? Glow = null, Color? Shadow = null, Color? Accent = null);

        private static readonly Dictionary<DockStyle, StyleDefaults> Table = new()
        {
            [DockStyle.AppleDock] = new(56, 72, 8, 12, 1.5f, true, 0.85f, 0.80f, 16, 8,
                Color.FromArgb(220, 240, 240, 245), Color.FromArgb(180, 200, 200, 200),
                ShowReflection: true, ItemBackground: Color.FromArgb(200, 255, 255, 255)),

            [DockStyle.Windows11Dock] = new(48, 64, 6, 10, 1.3f, true, 0.90f, 0.75f, 12, 6,
                Color.FromArgb(230, 248, 248, 248), Color.FromArgb(200, 220, 220, 220)),

            [DockStyle.Material3Dock] = new(56, 72, 8, 12, 1.4f, true, 0.80f, 0.80f, 16, 12,
                Color.FromArgb(240, 245, 245, 250), Color.FromArgb(200, 210, 210, 220),
                ShadowBlur: 12),

            [DockStyle.MinimalDock] = new(44, 56, 4, 8, 1.2f, false, 1.00f, 0.85f, 8, 6,
                Color.FromArgb(245, 250, 250, 250), Color.FromArgb(220, 230, 230, 230)),

            [DockStyle.GlassmorphismDock] = new(56, 72, 8, 12, 1.5f, true, 0.70f, 0.80f, 20, 12,
                Color.FromArgb(150, 255, 255, 255), Color.FromArgb(100, 255, 255, 255),
                ShowGlow: true, BackgroundBlur: 20f, Glow: Color.FromArgb(100, 150, 200, 255)),

            [DockStyle.NeumorphismDock] = new(56, 72, 8, 12, 1.4f, true, 0.90f, 0.80f, 16, 12,
                Color.FromArgb(255, 230, 230, 235), Color.FromArgb(255, 230, 230, 235),
                Shadow: Color.FromArgb(80, 150, 150, 160)),

            // PillDock is absent from DockStyleHelpers entirely, so it fell to that table's `_ =>`
            // defaults. Those defaults are what the control has been laying out with, so they are
            // what this row records - not the 56/16 the old metrics switch carried.
            [DockStyle.PillDock] = new(56, 72, 8, 12, 1.5f, true, 0.85f, 0.80f, 28, 28,
                Color.FromArgb(230, 245, 245, 250), Color.FromArgb(200, 220, 220, 230),
                ShadowBlur: 16),

            [DockStyle.iOSDock] = new(60, 80, 10, 14, 1.6f, true, 0.85f, 0.85f, 24, 12,
                Color.FromArgb(200, 250, 250, 255), Color.FromArgb(80, 255, 255, 255)),

            [DockStyle.GNOMEDock] = new(48, 64, 6, 10, 1.3f, false, 0.90f, 0.75f, 12, 8,
                Color.FromArgb(40, 40, 45), Color.FromArgb(60, 255, 255, 255)),

            [DockStyle.PlasmaPanel] = new(48, 64, 6, 10, 1.3f, false, 0.90f, 0.75f, 8, 6,
                Color.FromArgb(42, 46, 50), Color.FromArgb(80, 255, 255, 255)),

            [DockStyle.PlankDock] = new(40, 48, 4, 8, 1.2f, false, 1.00f, 0.90f, 12, 8,
                Color.FromArgb(60, 60, 65), Color.FromArgb(30, 0, 0, 0)),

            [DockStyle.NeonDock] = new(56, 72, 8, 12, 1.5f, true, 0.80f, 0.80f, 12, 8,
                Color.FromArgb(200, 20, 20, 30), Color.FromArgb(255, 0, 255, 255),
                ShowGlow: true, GlowBlur: 20, Glow: Color.FromArgb(200, 0, 255, 255)),

            [DockStyle.NordDock] = new(56, 72, 8, 12, 1.4f, false, 0.85f, 0.80f, 12, 10,
                Color.FromArgb(59, 66, 82), Color.FromArgb(76, 86, 106),
                Accent: Color.FromArgb(136, 192, 208)),

            // These five had no row in the old metrics switch and fell through `default:` to
            // AppleDock's numbers, while DockStyleHelpers gave the layout entirely different ones.
            // Their colours come from the painters' own palettes.
            [DockStyle.CyberpunkDock] = new(54, 74, 10, 12, 1.45f, true, 0.90f, 0.78f, 12, 8,
                Color.FromArgb(28, 14, 45), Color.FromArgb(0, 255, 222)),

            [DockStyle.TerminalDock] = new(48, 64, 6, 10, 1.25f, false, 0.95f, 0.82f, 4, 4,
                Color.FromArgb(16, 22, 16), Color.FromArgb(80, 220, 160)),

            [DockStyle.BubbleDock] = new(58, 76, 12, 14, 1.55f, true, 0.88f, 0.86f, 28, 28,
                Color.FromArgb(230, 246, 251, 255), Color.FromArgb(180, 206, 228, 244)),

            [DockStyle.ArcDock] = new(48, 60, 8, 10, 1.25f, false, 0.95f, 0.80f, 8, 6,
                Color.FromArgb(244, 245, 247), Color.FromArgb(220, 225, 230)),

            [DockStyle.DraculaDock] = new(52, 70, 8, 12, 1.35f, true, 0.90f, 0.80f, 12, 8,
                Color.FromArgb(40, 42, 54), Color.FromArgb(98, 114, 164)),

            // Custom carries neutral values on purpose: the point of the style is that DockConfig
            // drives it. It is also the row an unregistered style lands on, which is why it must not
            // look like any particular style.
            [DockStyle.Custom] = new(56, 72, 8, 12, 1.5f, true, 0.85f, 0.80f, 16, 8,
                Color.FromArgb(255, 240, 240, 240), Color.FromArgb(255, 200, 200, 200)),
        };

        /// <summary>The style's own accent, for callers that must not fall back to a global default.</summary>
        internal static Color AccentFor(DockStyle style)
        {
            var d = Table.TryGetValue(style, out var found) ? found : Table[DockStyle.Custom];
            return d.Accent ?? Color.FromArgb(255, 0, 122, 255);
        }

        /// <summary>Per-style dimension defaults, without building a full metrics object.</summary>
        internal static (int ItemSize, int DockHeight, int Spacing, int Padding, int CornerRadius,
                         float MaxScale, bool ShowShadow, float BackgroundOpacity)
            DimensionsFor(DockStyle style)
        {
            var d = Table.TryGetValue(style, out var found) ? found : Table[DockStyle.Custom];
            return (d.ItemSize, d.DockHeight, d.Spacing, d.Padding, d.CornerRadius,
                    d.MaxScale, d.ShowShadow, d.BackgroundOpacity);
        }

        /// <summary>
        /// Per-style defaults, optionally mapped onto the current theme.
        /// </summary>
        public static DockPainterMetrics DefaultFor(DockStyle style, IBeepTheme? theme = null, bool useThemeColors = false)
        {
            var d = Table.TryGetValue(style, out var found) ? found : Table[DockStyle.Custom];

            var m = new DockPainterMetrics
            {
                ItemSize = d.ItemSize,
                DockHeight = d.DockHeight,
                ItemSpacing = d.Spacing,
                Padding = d.Padding,
                MaxScale = d.MaxScale,
                ShowShadow = d.ShowShadow,
                BackgroundOpacity = d.BackgroundOpacity,
                IconSizeRatio = d.IconSizeRatio,
                CornerRadius = d.CornerRadius,
                ItemCornerRadius = d.ItemCornerRadius,
                BackgroundColor = d.Background,
                BorderColor = d.Border,
                ShowReflection = d.ShowReflection,
                ShowGlow = d.ShowGlow,
                ShadowBlur = d.ShadowBlur,
                GlowBlur = d.GlowBlur,
                BackgroundBlur = d.BackgroundBlur,
            };

            if (d.ItemBackground.HasValue) m.ItemBackgroundColor = d.ItemBackground.Value;
            if (d.Glow.HasValue) m.GlowColor = d.Glow.Value;
            if (d.Shadow.HasValue) m.ShadowColor = d.Shadow.Value;
            if (d.Accent.HasValue) m.AccentColor = d.Accent.Value;


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

        /// <summary>
        /// Gets default metrics for a specific style and applies DPI scaling.
        /// </summary>
        public static DockPainterMetrics DefaultFor(DockStyle style, IBeepTheme? theme, bool useThemeColors, float dpiScale)
        {
            var metrics = DefaultFor(style, theme, useThemeColors);
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
    }
}
