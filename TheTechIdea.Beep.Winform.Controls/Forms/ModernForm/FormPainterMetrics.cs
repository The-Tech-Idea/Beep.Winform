using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public enum SystemButtonsSide
    {
        Right,
        Left
    }
    /// <summary>
    /// Metrics used by BeepiFormProLayoutManager to size and place caption elements.
    /// Painters can provide custom metrics via IFormPainterMetricsProvider.
    /// </summary>
    public sealed class FormPainterMetrics
    {
        #region Static Metrics Cache
        // Key: (FormStyle, themeHashCode, useThemeColors)
        private static readonly Dictionary<(FormStyle, int, bool), FormPainterMetrics> _metricsCache
            = new Dictionary<(FormStyle, int, bool), FormPainterMetrics>();
        private static readonly object _cacheLock = new object();

        /// <summary>
        /// Returns cached metrics for the given style+theme combination.
        /// Creates and caches a new instance on first call.
        /// Call <see cref="InvalidateCache"/> when the global theme changes.
        /// </summary>
        public static FormPainterMetrics DefaultForCached(FormStyle style, IBeepTheme theme, bool useThemeColors = false)
        {
            int themeKey = theme?.GetHashCode() ?? 0;
            var key = (style, themeKey, useThemeColors);
            lock (_cacheLock)
            {
                if (!_metricsCache.TryGetValue(key, out var cached))
                {
                    cached = DefaultFor(style, theme, useThemeColors);
                    _metricsCache[key] = cached;
                }
                return cached;
            }
        }

        /// <summary>
        /// Returns DPI-aware cached metrics. Includes DPI scale in the cache key via rounded int.
        /// </summary>
        public static FormPainterMetrics DefaultForCached(FormStyle style, IBeepTheme theme, float dpiScaleFactor, bool useThemeColors = false)
        {
            // For non-standard DPI, fall through to uncached DPI-scaled version to keep cache small
            if (DpiScalingHelper.AreScaleFactorsEqual(dpiScaleFactor, 1.0f))
                return DefaultForCached(style, theme, useThemeColors);

            int themeKey = theme?.GetHashCode() ?? 0;
            // Encode DPI as 1/100th increments for the key
            int dpiKey = (int)(dpiScaleFactor * 100);
            var key = (style, themeKey ^ (dpiKey << 16), useThemeColors);
            lock (_cacheLock)
            {
                if (!_metricsCache.TryGetValue(key, out var cached))
                {
                    cached = DefaultFor(style, theme, dpiScaleFactor, useThemeColors);
                    _metricsCache[key] = cached;
                }
                return cached;
            }
        }

        /// <summary>
        /// Returns cached metrics using the full DPI-aware owner overload, with caching.
        /// This is the recommended method for painters to call from GetMetrics().
        /// </summary>
        public static FormPainterMetrics DefaultForCached(FormStyle style, BeepiFormPro owner)
        {
            if (owner == null || !owner.IsHandleCreated)
                return DefaultForCached(style, (IBeepTheme)null);

            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(owner);
            return DefaultForCached(style, owner.CurrentTheme, dpiScale, owner.UseThemeColors);
        }

        /// <summary>
        /// Clears the metrics cache. Call when a theme is changed globally.
        /// </summary>
        public static void InvalidateCache()
        {
            lock (_cacheLock)
            {
                _metricsCache.Clear();
            }
        }
        #endregion

        #region Corner Safe-Area Helper
        /// <summary>
        /// For a rounded-rectangle corner with the given <paramref name="radius"/>, returns
        /// the minimum safe X offset at the horizontal centre of an element whose centre Y
        /// is <paramref name="elementCenterY"/> and whose half-height is
        /// <paramref name="elementHalfHeight"/>.
        /// <para>
        /// Use this in <c>CalculateLayoutAndHitAreas</c> to ensure caption elements do not
        /// overlap the visually clipped corner arc.
        /// </para>
        /// </summary>
        /// <param name="radius">Corner arc radius in pixels.</param>
        /// <param name="elementCenterY">Y coordinate of the element's centre, relative to the top of the form.</param>
        /// <param name="elementHalfHeight">Half the element's height in pixels.</param>
        /// <returns>Minimum left X such that the element is fully inside the rounded corner.</returns>
        public static int GetCaptionLeftSafeX(int radius, int elementCenterY, int elementHalfHeight)
        {
            if (radius <= 0) return 0;

            // The arc occupies y in [0, radius] from the top-left corner.
            // For a row at y, the clip edge is at x = radius - sqrt(radius^2 - (radius-y)^2).
            // We take the worst-case row within the element's vertical span.
            int topRow    = Math.Max(0, elementCenterY - elementHalfHeight);
            int bottomRow = elementCenterY + elementHalfHeight;

            int maxClipX = 0;
            for (int row = topRow; row <= Math.Min(bottomRow, radius); row++)
            {
                int dy = radius - row;
                double clipX = radius - Math.Sqrt((double)radius * radius - (double)dy * dy);
                maxClipX = Math.Max(maxClipX, (int)Math.Ceiling(clipX));
            }
            return maxClipX;
        }
        #endregion

        public bool UseThemeColors { get; set; } = false;
        public IBeepTheme beepTheme { get; set; }
        // Helpers to ensure readable backgrounds per Style even with dark themes
        private static Color Blend(Color a, Color b, double t)
        {
            // t in [0,1]; returns a*(1-t)+b*t
            int r = (int)(a.R * (1 - t) + b.R * t);
            int g = (int)(a.G * (1 - t) + b.G * t);
            int bch = (int)(a.B * (1 - t) + b.B * t);
            return Color.FromArgb(255, r, g, bch);
        }

        private static double Luma(Color c)
        {
            return 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B; // [0,255]
        }

        private static Color EnsureMinLuma(Color c, int minLuma)
        {
            if (Luma(c) >= minLuma) return Color.FromArgb(255, c.R, c.G, c.B);
            // Blend towards white to reach target luma
            double t = 0.0;
            Color cur = c;
            // Simple incremental approach (few steps suffice for our ranges)
            for (int i = 0; i < 6 && Luma(cur) < minLuma; i++)
            {
                t += 0.2;
                cur = Blend(c, Color.White, Math.Min(1.0, t));
            }
            return cur;
        }
    
        // Colors for various form elements in caption and borders
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public Color ForegroundColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        // System Buttons --    
          public Color MinimizeButtonHoverColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color MaximizeButtonHoverColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color MaximizeButtonColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color MinimizeButtonColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CloseButtonColor { get; set; } = Color.FromArgb(255, 232, 17, 35);
        public Color CloseButtonHoverColor { get; set; } = Color.FromArgb(255, 232, 17, 35);
        //--------------------------

        // Caption and border colors
        public Color BorderColor { get; set; } = Color.FromArgb(255, 200, 200, 200);
        public Color CaptionColor { get; set; } = Color.FromArgb(255, 240, 240, 240);
        public Color CaptionTextColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionTextColorInactive { get; set; } = Color.FromArgb(255, 100, 100, 100);
        public Color CaptionTextColorMaximized { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionButtonColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionButtonHoverColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionButtonPressedColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionButtonInactiveColor { get; set; } = Color.FromArgb(255, 100, 100, 100);
        public Color CaptionButtonMaximizedColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionTextColorActive { get; set; } = Color.FromArgb(255, 240, 240, 240);

        public Color CaptionColorMinimized { get; set; } = Color.FromArgb(255, 180, 180, 180);
        public int CaptionMargin { get; set; } = 0;
           public int CaptionHeight { get; set; } = 32;
        //--------------------------------

        public int BorderWidth { get; set; } = 1;
        public int ResizeBorderWidth { get; set; } = 6;
        public int OuterMargin { get; set; } = 8;
        public int OuterMarginWhenMaximized { get; set; } = 0;
        public int InnerMargin { get; set; } = 0;
        public int InnerMarginWhenMaximized { get; set; } = 0;
       
        public int BorderRadius { get; set; } = 16;
        public int BorderRadiusWhenMaximized { get; set; } = 0;
        public int CaptionButtonRadius { get; set; } = 6;
    // Optional accent bar width for styles that use a left vertical accent (e.g., Material)
        public int AccentBarWidth { get; set; } = 0;
        public float FontHeightMultiplier { get; set; } = 2.5f;
        public int ButtonWidth { get; set; } = 32;
        public int ButtonSpacing { get; set; } = 4;
        public int TitleLeftPadding { get; set; } = 8;
        public int IconLeftPadding { get; set; } = 8;
        public int IconSize { get; set; } = 24;
        public SystemButtonsSide ButtonsPlacement { get; set; } = SystemButtonsSide.Right;

        // Extra buttons
        public bool ShowThemeButton { get; set; }
        public bool ShowStyleButton { get; set; }
        public bool ShowSearchButton { get; set; }
        public bool ShowProfileButton { get; set; }
        public bool ShowMailButton { get; set; }
        public bool ShowCustomActionButton { get; set; }

        /// <summary>
        /// Gets DPI-aware metrics for the specified owner form.
        /// Automatically detects the current DPI and scales all metrics accordingly.
        /// This is the recommended method for painters to call from GetMetrics().
        /// </summary>
        public static FormPainterMetrics DefaultFor(FormStyle style, BeepiFormPro owner)
        {
            if (owner == null || !owner.IsHandleCreated)
            {
                // Fallback to non-DPI aware if owner not available
                return DefaultFor(style, null, false);
            }
            
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(owner);
            return DefaultFor(style, owner.CurrentTheme, dpiScale, owner.UseThemeColors);
        }

        public static FormPainterMetrics DefaultFor(FormStyle style, IBeepTheme theme,bool UseThemeColors=false)
        {
            var m = new FormPainterMetrics();
            switch (style)
            {
                case FormStyle.Minimal:
                    m.CaptionHeight = 28;
                    m.ButtonWidth = 28;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Modern:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 22;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 8;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Material:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 32;
                    m.IconSize = 24;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 2;
                    m.BorderRadius = 8;
                    m.AccentBarWidth = 6;
                    break;
                case FormStyle.Fluent:
                    m.CaptionHeight = 40;
                    m.ButtonWidth = 36;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 6;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.MacOS:
                    m.CaptionHeight = 40;
                    m.ButtonWidth = 28;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Left;
                    m.BorderWidth = 1;
                    m.BorderRadius = 12;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Glass:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 8;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Cartoon:
                    m.CaptionHeight = 38;
                    m.ButtonWidth = 34;
                    m.IconSize = 22;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 3;
                    m.BorderRadius = 16;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.ChatBubble:
                    m.CaptionHeight = 34;
                    m.ButtonWidth = 30;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 2;
                    m.BorderRadius = 12;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Metro:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 48;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 0;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Metro2:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 48;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 0;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.GNOME:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 32;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 8;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.NeoMorphism:
                    m.CaptionHeight = 38;
                    m.ButtonWidth = 34;
                    m.IconSize = 22;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 12;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Glassmorphism:
                    m.CaptionHeight = 34;
                    m.ButtonWidth = 32;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 10;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.iOS:
                    m.CaptionHeight = 44;
                    m.ButtonWidth = 30;
                    m.IconSize = 22;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 12;
                    m.AccentBarWidth = 0;
                    break;
                // Windows11 REMOVED
                case FormStyle.Nordic:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 36;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 6;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Paper:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 32;
                    m.IconSize = 22;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Ubuntu:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 28;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Left;
                    m.BorderWidth = 1;
                    m.BorderRadius = 6;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.KDE:
                    m.CaptionHeight = 34;
                    m.ButtonWidth = 32;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 6;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.ArcLinux:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 30;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 2;
                    break;
                case FormStyle.Dracula:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 6;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Solarized:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 2;
                    break;
                case FormStyle.OneDark:
                    m.CaptionHeight = 30;
                    m.ButtonWidth = 32;
                    m.IconSize = 18;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.GruvBox:
                    m.CaptionHeight = 30;
                    m.ButtonWidth = 32;
                    m.IconSize = 18;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Nord:
                    m.CaptionHeight = 30;
                    m.ButtonWidth = 32;
                    m.IconSize = 18;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Tokyo:
                    m.CaptionHeight = 30;
                    m.ButtonWidth = 32;
                    m.IconSize = 18;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Brutalist:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 50;
                    m.IconSize = 22;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 3;
                    m.BorderRadius = 0;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Retro:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 44;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 2;
                    m.BorderRadius = 0;
                    m.AccentBarWidth = 2;
                    break;
                case FormStyle.Cyberpunk:
                    m.CaptionHeight = 34;
                    m.ButtonWidth = 42;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 2;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 3;
                    break;
                case FormStyle.Neon:
                    m.CaptionHeight = 34;
                    m.ButtonWidth = 42;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 2;
                    m.BorderRadius = 6;
                    m.AccentBarWidth = 4;
                    break;
                case FormStyle.Holographic:
                    m.CaptionHeight = 34;
                    m.ButtonWidth = 42;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 8;
                    m.AccentBarWidth = 3;
                    break;
                case FormStyle.Terminal:
                    m.CaptionHeight = 28;
                    m.ButtonWidth = 36;
                    m.IconSize = 16;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 2;
                    m.BorderRadius = 0;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Custom:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 22;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Shadcn:
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 8;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.RadixUI:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 36;
                    m.IconSize = 18;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 2;
                    m.BorderRadius = 6;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.NextJS:
                    m.CaptionHeight = 34;
                    m.ButtonWidth = 32;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 10;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.Linear:
                    m.CaptionHeight = 28;
                    m.ButtonWidth = 28;
                    m.IconSize = 14;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    m.AccentBarWidth = 0;
                    break;
                case FormStyle.MaterialYou:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 40;
                    m.IconSize = 24;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 0;
                    m.BorderRadius = 12;
                    m.AccentBarWidth = 6;
                    break;
              
                default:
                    // Fallback to Modern Style metrics
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 24;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 8;
                    m.AccentBarWidth = 0;
                    break;
            }
            if ((theme != null) && (theme != m.beepTheme) && UseThemeColors)
            {
                // Fill colors from the provided theme
                m.BorderColor = theme.BorderColor;
                m.CaptionColor = theme.AppBarBackColor;
                m.CaptionTextColor = theme.AppBarTitleForeColor;
                m.CaptionTextColorInactive = theme.InactiveBorderColor;
                m.CaptionTextColorMaximized = theme.AppBarTitleForeColor;
                m.CaptionButtonColor = theme.AppBarButtonForeColor;
                m.CaptionButtonHoverColor = theme.ButtonHoverForeColor;
                m.CaptionButtonPressedColor = theme.ButtonPressedForeColor;
                m.CaptionButtonInactiveColor = theme.DisabledForeColor;
                m.CaptionButtonMaximizedColor = theme.AppBarButtonForeColor;
                m.ForegroundColor = theme.ForeColor;
                m.BackgroundColor = theme.BackColor;
                
                // System buttons from theme
                m.MinimizeButtonColor = theme.AppBarMinButtonColor;
                m.MaximizeButtonColor = theme.AppBarMaxButtonColor;
                m.CloseButtonColor = theme.AppBarCloseButtonColor;
                m.MinimizeButtonHoverColor = theme.ButtonHoverForeColor;
                m.MaximizeButtonHoverColor = theme.ButtonHoverForeColor;
                m.CloseButtonHoverColor = theme.ButtonErrorForeColor;
                
                m.beepTheme = theme;

                // Style-specific adjustments to ensure backgrounds are not too dark
                switch (style)
                {
                    case FormStyle.Minimal:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 230);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 235);
                        break;
                    case FormStyle.Modern:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 235);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 238);
                        break;
                    case FormStyle.Material:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 235);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 240);
                        break;
                    case FormStyle.MacOS:
                        m.BackgroundColor = Color.FromArgb(255, 245, 245, 245);
                        m.CaptionColor = Color.FromArgb(255, 250, 250, 250);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        break;
                    case FormStyle.Cartoon:
                        m.BackgroundColor = Color.FromArgb(255, 255, 240, 255);
                        m.CaptionColor = Color.FromArgb(255, 255, 240, 255);
                        m.ForegroundColor = Color.FromArgb(255, 80, 0, 120);
                        break;
                    case FormStyle.ChatBubble:
                        m.BackgroundColor = Color.FromArgb(255, 230, 250, 255);
                        m.CaptionColor = Color.FromArgb(255, 230, 250, 255);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        break;
                    case FormStyle.Glass:
                        // Keep theme-driven but ensure readable caption
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 235);
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 235);
                        break;
                    case FormStyle.Fluent:
                        // Painter uses its own acrylic base; keep defaults
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 240);
                        break;
                    case FormStyle.Metro:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 245);
                        m.CaptionColor = theme.PrimaryColor;
                        m.CaptionTextColor = theme.OnPrimaryColor;
                        break;
                    case FormStyle.Metro2:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 245);
                        m.CaptionColor = EnsureMinLuma(theme.AppBarBackColor, 240);
                        m.BorderColor = theme.AccentColor;
                        break;
                    case FormStyle.GNOME:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 240);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 243);
                        break;
                    case FormStyle.NeoMorphism:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 238);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 240);
                        break;
                    case FormStyle.Glassmorphism:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 230);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 235);
                        break;
                    case FormStyle.iOS:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 242);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 245);
                        break;
                    // Windows11 REMOVED
                    case FormStyle.Nordic:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 245);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 248);
                        break;
                    case FormStyle.Paper:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 248);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 250);
                        break;
                    case FormStyle.Ubuntu:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 235);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 238);
                        break;
                    case FormStyle.KDE:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 238);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 240);
                        break;
                    case FormStyle.ArcLinux:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Dracula:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Solarized:
                        // Can work with theme colors
                        break;
                    case FormStyle.OneDark:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.GruvBox:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Nord:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Tokyo:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Brutalist:
                        m.BackgroundColor = EnsureMinLuma(m.BackgroundColor, 245);
                        m.CaptionColor = EnsureMinLuma(m.CaptionColor, 248);
                        break;
                    case FormStyle.Retro:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Cyberpunk:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Neon:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Holographic:
                        // Dark theme, keep as-is
                        break;
                    case FormStyle.Terminal:
                        // Dark theme, keep as-is (terminal aesthetic)
                        break;
                    case FormStyle.Custom:
                        // Custom uses theme as-is without forcing luminance
                        break;
                 
                }
            }else
            {
                // do another switch on Style to set some default colors
                switch (style)
                {
                    case FormStyle.Minimal:
                        m.BorderColor = Color.FromArgb(255, 200, 200, 200);
                        m.CaptionColor = Color.FromArgb(255, 250, 250, 250);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 150, 150, 150);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 30, 30, 30);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 60, 60, 60);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 150, 150, 150);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 0, 0);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.Material:
                        m.BorderColor = Color.FromArgb(255, 180, 180, 180);
                        m.CaptionColor = Color.FromArgb(255, 245, 245, 245);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 120, 120, 120);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 50, 50, 50);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 80, 80, 80);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 110, 110, 110);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 120, 120, 120);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 50, 50, 50);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.AccentBarWidth = 6;
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.Fluent:
                        m.BorderColor = Color.FromArgb(255, 160, 160, 160);
                        m.CaptionColor = Color.FromArgb(255, 240, 240, 240);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 30, 30, 30);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 60, 60, 60);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 90, 90, 90);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 30, 30, 30);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    
                    case FormStyle.MacOS:
                        m.BorderColor = Color.FromArgb(255, 180, 180, 180);
                        m.CaptionColor = Color.FromArgb(255, 250, 250, 250);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 130, 130, 130);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 80, 80, 80);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 110, 110, 110);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 140, 140, 140);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 130, 130, 130);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 80, 80, 80);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.Glass:
                        m.BorderColor = Color.FromArgb(255, 200, 200, 200);
                        m.CaptionColor = Color.FromArgb(255, 230, 230, 230);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 30, 30, 30);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 60, 60, 60);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 0, 0);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.Metro:
                        m.BorderColor = Color.FromArgb(255, 180, 180, 180);
                        m.CaptionColor = Color.FromArgb(255, 0, 120, 215);
                        m.CaptionTextColor = Color.FromArgb(255, 255, 255, 255);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 255, 255, 255);
                        m.CaptionButtonColor = Color.FromArgb(255, 255, 255, 255);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 240, 240, 240);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 200, 200, 200);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 255, 255, 255);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 255, 255, 255);
                        m.MaximizeButtonColor = Color.FromArgb(255, 255, 255, 255);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 255, 255);
                        break;
                    case FormStyle.Metro2:
                        m.BorderColor = Color.FromArgb(255, 0, 120, 215);
                        m.CaptionColor = Color.FromArgb(255, 240, 240, 240);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 120, 215);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 0, 150, 255);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 0, 90, 180);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 120, 215);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.GNOME:
                        m.BorderColor = Color.FromArgb(255, 200, 200, 200);
                        m.CaptionColor = Color.FromArgb(255, 245, 245, 245);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 120, 120, 120);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 50, 50, 50);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 80, 80, 80);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 110, 110, 110);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 120, 120, 120);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 50, 50, 50);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.Cartoon:
                        m.BorderColor = Color.FromArgb(255, 150, 100, 200);
                        m.CaptionColor = Color.FromArgb(255, 255, 240, 255);
                        m.CaptionTextColor = Color.FromArgb(255, 80, 0, 120);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 150, 100, 200);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 80, 0, 120);
                        m.CaptionButtonColor = Color.FromArgb(255, 80, 0, 120);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 120, 0, 180);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 50, 0, 100);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 150, 100, 200);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 80, 0, 120);
                        m.ForegroundColor = Color.FromArgb(255, 80, 0, 120);
                        m.BackgroundColor = Color.FromArgb(255, 255, 240, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 255, 215, 0);
                        m.MaximizeButtonColor = Color.FromArgb(255, 255, 105, 180);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 69, 180);
                        break;
                    case FormStyle.ChatBubble:
                        m.BorderColor = Color.FromArgb(255, 200, 200, 200);
                        m.CaptionColor = Color.FromArgb(255, 230, 250, 255);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 30, 30, 30);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 60, 60, 60);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 0, 0);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 230, 250, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.Modern:
                        m.BorderColor = Color.FromArgb(255, 200, 200, 200);
                        m.CaptionColor = Color.FromArgb(255, 240, 240, 240);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 30, 30, 30);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 60, 60, 60);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 0, 0);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.NeoMorphism:
                        m.BorderColor = Color.FromArgb(255, 220, 220, 225);
                        m.CaptionColor = Color.FromArgb(255, 240, 240, 245);
                        m.CaptionTextColor = Color.FromArgb(255, 50, 50, 60);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 130, 130, 140);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 50, 50, 60);
                        m.CaptionButtonColor = Color.FromArgb(255, 80, 80, 90);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 100, 100, 120);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 60, 60, 80);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 130, 130, 140);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 80, 80, 90);
                        m.ForegroundColor = Color.FromArgb(255, 50, 50, 60);
                        m.BackgroundColor = Color.FromArgb(255, 240, 240, 245);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;
                    case FormStyle.Glassmorphism:
                        m.BorderColor = Color.FromArgb(255, 200, 210, 220);
                        m.CaptionColor = Color.FromArgb(255, 235, 240, 245);
                        m.CaptionTextColor = Color.FromArgb(255, 40, 50, 60);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 120, 130, 140);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 40, 50, 60);
                        m.CaptionButtonColor = Color.FromArgb(255, 70, 80, 90);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 100, 110, 120);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 50, 60, 70);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 120, 130, 140);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 70, 80, 90);
                        m.ForegroundColor = Color.FromArgb(255, 40, 50, 60);
                        m.BackgroundColor = Color.FromArgb(255, 240, 245, 250);
                        m.MinimizeButtonColor = Color.FromArgb(255, 100, 200, 255);
                        m.MaximizeButtonColor = Color.FromArgb(255, 150, 220, 255);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 120, 150);
                        break;
                    case FormStyle.iOS:
                        m.BorderColor = Color.FromArgb(255, 200, 200, 205);
                        m.CaptionColor = Color.FromArgb(255, 248, 248, 252);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 120, 120, 128);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 122, 255);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 10, 132, 255);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 0, 100, 220);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 120, 120, 128);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 122, 255);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 255, 204, 0);
                        m.MaximizeButtonColor = Color.FromArgb(255, 52, 199, 89);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 59, 48);
                        break;
                    // Windows11 REMOVED
                    case FormStyle.Nordic:
                        m.BorderColor = Color.FromArgb(255, 220, 220, 220);
                        m.CaptionColor = Color.FromArgb(255, 250, 250, 250);
                        m.CaptionTextColor = Color.FromArgb(255, 60, 60, 60);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 140, 140, 140);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 60, 60, 60);
                        m.CaptionButtonColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 120, 120, 120);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 80, 80, 80);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 140, 140, 140);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 100, 100, 100);
                        m.ForegroundColor = Color.FromArgb(255, 60, 60, 60);
                        m.BackgroundColor = Color.FromArgb(255, 252, 252, 252);
                        m.MinimizeButtonColor = Color.FromArgb(255, 200, 200, 200);
                        m.MaximizeButtonColor = Color.FromArgb(255, 200, 200, 200);
                        m.CloseButtonColor = Color.FromArgb(255, 200, 200, 200);
                        break;
                    case FormStyle.Paper:
                        m.BorderColor = Color.FromArgb(255, 230, 230, 230);
                        m.CaptionColor = Color.FromArgb(255, 255, 255, 255);
                        m.CaptionTextColor = Color.FromArgb(255, 33, 33, 33);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 117, 117, 117);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 33, 33, 33);
                        m.CaptionButtonColor = Color.FromArgb(255, 66, 66, 66);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 97, 97, 97);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 33, 33, 33);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 117, 117, 117);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 66, 66, 66);
                        m.ForegroundColor = Color.FromArgb(255, 33, 33, 33);
                        m.BackgroundColor = Color.FromArgb(255, 250, 250, 250);
                        m.MinimizeButtonColor = Color.FromArgb(255, 255, 193, 7);
                        m.MaximizeButtonColor = Color.FromArgb(255, 76, 175, 80);
                        m.CloseButtonColor = Color.FromArgb(255, 244, 67, 54);
                        break;
                    case FormStyle.Ubuntu:
                        m.BorderColor = Color.FromArgb(255, 180, 180, 180);
                        m.CaptionColor = Color.FromArgb(255, 240, 240, 240);
                        m.CaptionTextColor = Color.FromArgb(255, 50, 50, 50);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 130, 130, 130);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 50, 50, 50);
                        m.CaptionButtonColor = Color.FromArgb(255, 233, 84, 32);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 243, 104, 52);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 213, 64, 12);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 130, 130, 130);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 233, 84, 32);
                        m.ForegroundColor = Color.FromArgb(255, 50, 50, 50);
                        m.BackgroundColor = Color.FromArgb(255, 245, 245, 245);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 233, 84, 32);
                        break;
                    case FormStyle.KDE:
                        m.BorderColor = Color.FromArgb(255, 190, 190, 195);
                        m.CaptionColor = Color.FromArgb(255, 239, 240, 241);
                        m.CaptionTextColor = Color.FromArgb(255, 35, 38, 41);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 127, 140, 141);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 35, 38, 41);
                        m.CaptionButtonColor = Color.FromArgb(255, 61, 174, 233);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 81, 194, 253);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 41, 154, 213);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 127, 140, 141);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 61, 174, 233);
                        m.ForegroundColor = Color.FromArgb(255, 35, 38, 41);
                        m.BackgroundColor = Color.FromArgb(255, 252, 252, 252);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 237, 21, 21);
                        break;
                    case FormStyle.ArcLinux:
                        m.BorderColor = Color.FromArgb(255, 64, 69, 82);
                        m.CaptionColor = Color.FromArgb(255, 56, 60, 74);
                        m.CaptionTextColor = Color.FromArgb(255, 211, 218, 227);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 140, 150, 160);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 211, 218, 227);
                        m.CaptionButtonColor = Color.FromArgb(255, 82, 148, 226);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 102, 168, 246);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 62, 128, 206);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 140, 150, 160);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 82, 148, 226);
                        m.ForegroundColor = Color.FromArgb(255, 211, 218, 227);
                        m.BackgroundColor = Color.FromArgb(255, 64, 69, 82);
                        m.MinimizeButtonColor = Color.FromArgb(255, 211, 218, 227);
                        m.MaximizeButtonColor = Color.FromArgb(255, 211, 218, 227);
                        m.CloseButtonColor = Color.FromArgb(255, 211, 218, 227);
                        break;
                    case FormStyle.Dracula:
                        m.BorderColor = Color.FromArgb(255, 68, 71, 90);
                        m.CaptionColor = Color.FromArgb(255, 40, 42, 54);
                        m.CaptionTextColor = Color.FromArgb(255, 248, 248, 242);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 98, 114, 164);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 248, 248, 242);
                        m.CaptionButtonColor = Color.FromArgb(255, 189, 147, 249);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 209, 167, 255);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 169, 127, 229);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 98, 114, 164);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 189, 147, 249);
                        m.ForegroundColor = Color.FromArgb(255, 248, 248, 242);
                        m.BackgroundColor = Color.FromArgb(255, 40, 42, 54);
                        m.MinimizeButtonColor = Color.FromArgb(255, 241, 250, 140);
                        m.MaximizeButtonColor = Color.FromArgb(255, 80, 250, 123);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 85, 85);
                        break;
                    case FormStyle.Solarized:
                        m.BorderColor = Color.FromArgb(255, 147, 161, 161);
                        m.CaptionColor = Color.FromArgb(255, 253, 246, 227);
                        m.CaptionTextColor = Color.FromArgb(255, 88, 110, 117);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 147, 161, 161);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 88, 110, 117);
                        m.CaptionButtonColor = Color.FromArgb(255, 38, 139, 210);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 58, 159, 230);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 18, 119, 190);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 147, 161, 161);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 38, 139, 210);
                        m.ForegroundColor = Color.FromArgb(255, 88, 110, 117);
                        m.BackgroundColor = Color.FromArgb(255, 253, 246, 227);
                        m.MinimizeButtonColor = Color.FromArgb(255, 181, 137, 0);
                        m.MaximizeButtonColor = Color.FromArgb(255, 133, 153, 0);
                        m.CloseButtonColor = Color.FromArgb(255, 220, 50, 47);
                        break;
                    case FormStyle.OneDark:
                        m.BorderColor = Color.FromArgb(255, 60, 66, 82);
                        m.CaptionColor = Color.FromArgb(255, 40, 44, 52);
                        m.CaptionTextColor = Color.FromArgb(255, 171, 178, 191);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 92, 99, 112);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 171, 178, 191);
                        m.CaptionButtonColor = Color.FromArgb(255, 97, 175, 239);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 117, 195, 255);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 77, 155, 219);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 92, 99, 112);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 97, 175, 239);
                        m.ForegroundColor = Color.FromArgb(255, 171, 178, 191);
                        m.BackgroundColor = Color.FromArgb(255, 40, 44, 52);
                        m.MinimizeButtonColor = Color.FromArgb(255, 229, 192, 123);
                        m.MaximizeButtonColor = Color.FromArgb(255, 152, 195, 121);
                        m.CloseButtonColor = Color.FromArgb(255, 224, 108, 117);
                        break;
                    case FormStyle.GruvBox:
                        m.BorderColor = Color.FromArgb(255, 80, 73, 69);
                        m.CaptionColor = Color.FromArgb(255, 40, 40, 40);
                        m.CaptionTextColor = Color.FromArgb(255, 235, 219, 178);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 146, 131, 116);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 235, 219, 178);
                        m.CaptionButtonColor = Color.FromArgb(255, 254, 128, 25);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 255, 158, 65);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 214, 93, 14);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 146, 131, 116);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 254, 128, 25);
                        m.ForegroundColor = Color.FromArgb(255, 235, 219, 178);
                        m.BackgroundColor = Color.FromArgb(255, 40, 40, 40);
                        m.MinimizeButtonColor = Color.FromArgb(255, 250, 189, 47);
                        m.MaximizeButtonColor = Color.FromArgb(255, 184, 187, 38);
                        m.CloseButtonColor = Color.FromArgb(255, 251, 73, 52);
                        break;
                    case FormStyle.Nord:
                        m.BorderColor = Color.FromArgb(255, 67, 76, 94);
                        m.CaptionColor = Color.FromArgb(255, 46, 52, 64);
                        m.CaptionTextColor = Color.FromArgb(255, 216, 222, 233);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 129, 161, 193);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 216, 222, 233);
                        m.CaptionButtonColor = Color.FromArgb(255, 136, 192, 208);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 156, 212, 228);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 116, 172, 188);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 129, 161, 193);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 136, 192, 208);
                        m.ForegroundColor = Color.FromArgb(255, 216, 222, 233);
                        m.BackgroundColor = Color.FromArgb(255, 46, 52, 64);
                        m.MinimizeButtonColor = Color.FromArgb(255, 235, 203, 139);
                        m.MaximizeButtonColor = Color.FromArgb(255, 163, 190, 140);
                        m.CloseButtonColor = Color.FromArgb(255, 191, 97, 106);
                        break;
                    case FormStyle.Tokyo:
                        m.BorderColor = Color.FromArgb(255, 41, 46, 73);
                        m.CaptionColor = Color.FromArgb(255, 26, 27, 38);
                        m.CaptionTextColor = Color.FromArgb(255, 169, 177, 214);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 86, 95, 137);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 169, 177, 214);
                        m.CaptionButtonColor = Color.FromArgb(255, 125, 207, 255);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 145, 227, 255);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 105, 187, 235);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 86, 95, 137);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 125, 207, 255);
                        m.ForegroundColor = Color.FromArgb(255, 169, 177, 214);
                        m.BackgroundColor = Color.FromArgb(255, 26, 27, 38);
                        m.MinimizeButtonColor = Color.FromArgb(255, 224, 175, 104);
                        m.MaximizeButtonColor = Color.FromArgb(255, 158, 206, 106);
                        m.CloseButtonColor = Color.FromArgb(255, 247, 118, 142);
                        break;
                    case FormStyle.Brutalist:
                        m.BorderColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionColor = Color.FromArgb(255, 255, 255, 255);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 50, 50, 50);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 0, 0);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 0, 0, 0);
                        m.MaximizeButtonColor = Color.FromArgb(255, 0, 0, 0);
                        m.CloseButtonColor = Color.FromArgb(255, 0, 0, 0);
                        break;
                    case FormStyle.Retro:
                        m.BorderColor = Color.FromArgb(255, 255, 0, 255);
                        m.CaptionColor = Color.FromArgb(255, 60, 50, 100);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 150);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionButtonColor = Color.FromArgb(255, 255, 255, 0);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 255, 255, 100);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 200, 200, 0);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 150);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 255, 255, 0);
                        m.ForegroundColor = Color.FromArgb(255, 0, 255, 255);
                        m.BackgroundColor = Color.FromArgb(255, 30, 25, 50);
                        m.MinimizeButtonColor = Color.FromArgb(255, 255, 255, 0);
                        m.MaximizeButtonColor = Color.FromArgb(255, 0, 255, 255);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 0, 255);
                        break;
                    case FormStyle.Cyberpunk:
                        m.BorderColor = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionColor = Color.FromArgb(255, 10, 10, 20);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 80, 100, 120);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 100, 255, 255);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 0, 200, 200);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 80, 100, 120);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 255, 255);
                        m.ForegroundColor = Color.FromArgb(255, 0, 255, 255);
                        m.BackgroundColor = Color.FromArgb(255, 10, 10, 20);
                        m.MinimizeButtonColor = Color.FromArgb(255, 0, 255, 255);
                        m.MaximizeButtonColor = Color.FromArgb(255, 255, 0, 255);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 0, 128);
                        break;
                    case FormStyle.Neon:
                        m.BorderColor = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionColor = Color.FromArgb(255, 20, 20, 35);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 80, 180, 200);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 255, 255);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 255, 200);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 100, 255, 255);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 0, 200, 180);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 80, 180, 200);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 255, 200);
                        m.ForegroundColor = Color.FromArgb(255, 0, 255, 255);
                        m.BackgroundColor = Color.FromArgb(255, 15, 15, 25);
                        m.MinimizeButtonColor = Color.FromArgb(255, 0, 255, 200);
                        m.MaximizeButtonColor = Color.FromArgb(255, 100, 200, 255);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 50, 150);
                        break;
                    case FormStyle.Holographic:
                        m.BorderColor = Color.FromArgb(255, 150, 200, 255);
                        m.CaptionColor = Color.FromArgb(255, 40, 30, 60);
                        m.CaptionTextColor = Color.FromArgb(255, 200, 150, 255);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 140);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 200, 150, 255);
                        m.CaptionButtonColor = Color.FromArgb(255, 150, 200, 255);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 180, 220, 255);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 120, 170, 230);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 140);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 150, 200, 255);
                        m.ForegroundColor = Color.FromArgb(255, 200, 150, 255);
                        m.BackgroundColor = Color.FromArgb(255, 25, 20, 35);
                        m.MinimizeButtonColor = Color.FromArgb(255, 200, 255, 200);
                        m.MaximizeButtonColor = Color.FromArgb(255, 255, 200, 255);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 150, 200);
                        break;
                    case FormStyle.Terminal:
                        m.BorderColor = Color.FromArgb(255, 0, 255, 0);
                        m.CaptionColor = Color.FromArgb(255, 10, 10, 10);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 255, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 0, 128, 0);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 255, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 255, 0);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 100, 255, 100);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 0, 200, 0);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 0, 128, 0);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 255, 0);
                        m.ForegroundColor = Color.FromArgb(255, 0, 255, 0);
                        m.BackgroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.MinimizeButtonColor = Color.FromArgb(255, 255, 255, 80);
                        m.MaximizeButtonColor = Color.FromArgb(255, 80, 255, 80);
                        m.CloseButtonColor = Color.FromArgb(255, 255, 80, 80);
                        break;
                    case FormStyle.Custom:
                        m.BorderColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionColor = Color.FromArgb(255, 220, 220, 220);
                        m.CaptionTextColor = Color.FromArgb(255, 50, 50, 50);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 50, 50, 50);
                        m.CaptionButtonColor = Color.FromArgb(255, 50, 50, 50);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 80, 80, 80);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 110, 110, 110);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 50, 50, 50);
                        m.ForegroundColor = Color.FromArgb(255, 50, 50, 50);
                        m.BackgroundColor = Color.FromArgb(255, 240, 240, 240);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;

                    default:
                        // Modern / Classic fallbacks
                        m.BorderColor = Color.FromArgb(255, 200, 200, 200);
                        m.CaptionColor = Color.FromArgb(255, 240, 240, 240);
                        m.CaptionTextColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionTextColorInactive = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionTextColorMaximized = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonColor = Color.FromArgb(255, 0, 0, 0);
                        m.CaptionButtonHoverColor = Color.FromArgb(255, 30, 30, 30);
                        m.CaptionButtonPressedColor = Color.FromArgb(255, 60, 60, 60);
                        m.CaptionButtonInactiveColor = Color.FromArgb(255, 100, 100, 100);
                        m.CaptionButtonMaximizedColor = Color.FromArgb(255, 0, 0, 0);
                        m.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                        m.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
                        m.MinimizeButtonColor = Color.FromArgb(255, 253, 224, 71);
                        m.MaximizeButtonColor = Color.FromArgb(255, 134, 239, 172);
                        m.CloseButtonColor = Color.FromArgb(255, 248, 113, 113);
                        break;


                }

            }
            
            // CRITICAL: Ensure caption text is ALWAYS visible against caption background
            // This applies whether using theme colors or default colors
            double captionTextLuma = Luma(m.CaptionTextColor);
            double captionLuma = Luma(m.CaptionColor);
            double captionContrastRatio = Math.Abs(captionTextLuma - captionLuma);
            
            // If contrast is too low (less than 100 luma units), force readable colors
            if (captionContrastRatio < 100)
            {
                // If caption is dark, use light text; if caption is light, use dark text
                if (captionLuma < 128)
                {
                    // Dark caption, use white or very light text
                    m.CaptionTextColor = Color.FromArgb(255, 240, 240, 240);
                    m.CaptionTextColorMaximized = Color.FromArgb(255, 240, 240, 240);
                }
                else
                {
                    // Light caption, use black or very dark text
                    m.CaptionTextColor = Color.FromArgb(255, 30, 30, 30);
                    m.CaptionTextColorMaximized = Color.FromArgb(255, 30, 30, 30);
                }
            }
            
            // CRITICAL: Ensure foreground text is ALWAYS visible against background
            // This applies to text content painted on the main form background
            double foregroundLuma = Luma(m.ForegroundColor);
            double backgroundLuma = Luma(m.BackgroundColor);
            double backgroundContrastRatio = Math.Abs(foregroundLuma - backgroundLuma);
            
            // If contrast is too low (less than 100 luma units), force readable colors
            if (backgroundContrastRatio < 100)
            {
                // If background is dark, use light foreground; if background is light, use dark foreground
                if (backgroundLuma < 128)
                {
                    // Dark background, use white or very light foreground
                    m.ForegroundColor = Color.FromArgb(255, 240, 240, 240);
                }
                else
                {
                    // Light background, use black or very dark foreground
                    m.ForegroundColor = Color.FromArgb(255, 30, 30, 30);
                }
            }
            
            // CRITICAL: Ensure caption text is ALSO visible against main background
            // Some layouts may render caption text against the background color
            double captionTextVsBackgroundLuma = Math.Abs(captionTextLuma - backgroundLuma);
            
            // If contrast is too low (less than 100 luma units), force readable colors
            if (captionTextVsBackgroundLuma < 100)
            {
                // Recalculate caption text luma after potential previous adjustments
                captionTextLuma = Luma(m.CaptionTextColor);
                
                // If background is dark, use light caption text; if background is light, use dark caption text
                if (backgroundLuma < 128)
                {
                    // Dark background, use white or very light caption text
                    m.CaptionTextColor = Color.FromArgb(255, 240, 240, 240);
                    m.CaptionTextColorMaximized = Color.FromArgb(255, 240, 240, 240);
                }
                else
                {
                    // Light background, use black or very dark caption text
                    m.CaptionTextColor = Color.FromArgb(255, 30, 30, 30);
                    m.CaptionTextColorMaximized = Color.FromArgb(255, 30, 30, 30);
                }
            }
            
                return m;
        }

        /// <summary>
        /// Gets DPI-aware metrics for the specified style, theme, and DPI scale factor.
        /// All size metrics will be scaled according to the DPI.
        /// </summary>
        public static FormPainterMetrics DefaultFor(FormStyle style, IBeepTheme theme, float dpiScaleFactor, bool useThemeColors = false)
        {
            // Get base metrics at 96 DPI (scale factor 1.0)
            var m = DefaultFor(style, theme, useThemeColors);
            
            // If DPI scale is 1.0, return as-is (no scaling needed)
            if (DpiScalingHelper.AreScaleFactorsEqual(dpiScaleFactor, 1.0f))
                return m;
            
            // Scale all size-based metrics using DpiScalingHelper
            m.CaptionHeight = DpiScalingHelper.ScaleValue(m.CaptionHeight, dpiScaleFactor);
            m.ButtonWidth = DpiScalingHelper.ScaleValue(m.ButtonWidth, dpiScaleFactor);
            m.IconSize = DpiScalingHelper.ScaleValue(m.IconSize, dpiScaleFactor);
            m.ButtonSpacing = DpiScalingHelper.ScaleValue(m.ButtonSpacing, dpiScaleFactor);
            m.TitleLeftPadding = DpiScalingHelper.ScaleValue(m.TitleLeftPadding, dpiScaleFactor);
            m.IconLeftPadding = DpiScalingHelper.ScaleValue(m.IconLeftPadding, dpiScaleFactor);
            
            m.BorderWidth = DpiScalingHelper.ScaleValue(m.BorderWidth, dpiScaleFactor);
            m.ResizeBorderWidth = DpiScalingHelper.ScaleValue(m.ResizeBorderWidth, dpiScaleFactor);
            m.OuterMargin = DpiScalingHelper.ScaleValue(m.OuterMargin, dpiScaleFactor);
            m.OuterMarginWhenMaximized = DpiScalingHelper.ScaleValue(m.OuterMarginWhenMaximized, dpiScaleFactor);
            m.InnerMargin = DpiScalingHelper.ScaleValue(m.InnerMargin, dpiScaleFactor);
            m.InnerMarginWhenMaximized = DpiScalingHelper.ScaleValue(m.InnerMarginWhenMaximized, dpiScaleFactor);
            
            m.BorderRadius = DpiScalingHelper.ScaleValue(m.BorderRadius, dpiScaleFactor);
            m.BorderRadiusWhenMaximized = DpiScalingHelper.ScaleValue(m.BorderRadiusWhenMaximized, dpiScaleFactor);
            m.CaptionButtonRadius = DpiScalingHelper.ScaleValue(m.CaptionButtonRadius, dpiScaleFactor);
            m.AccentBarWidth = DpiScalingHelper.ScaleValue(m.AccentBarWidth, dpiScaleFactor);
            m.CaptionMargin = DpiScalingHelper.ScaleValue(m.CaptionMargin, dpiScaleFactor);
            
            // FontHeightMultiplier is a ratio, not a size, so don't scale it
            
            return m;
        }

       
    }
}
