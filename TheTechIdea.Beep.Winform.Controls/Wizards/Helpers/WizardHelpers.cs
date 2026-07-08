using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Helpers
{
    /// <summary>
    /// Helper utilities for wizard animations and transitions.
    /// Bitmap-based animation eliminates per-frame control hierarchy repaints.
    /// </summary>
    public static class WizardHelpers
    {
        /// <summary>
        /// Resolve a font from theme typography, with theme-consistent fallback.
        /// Uses BeepThemesManager.ToFont exclusively — no direct BeepFontManager usage.
        /// </summary>
        public static Font GetFont(IBeepTheme? theme, TypographyStyle? style, float fallbackSize, FontStyle fallbackStyle)
        {
            if (style != null)
            {
                var font = BeepThemesManager.ToFont(style);
                if (font != null) return font;
            }

            if (theme?.BodyStyle != null && fallbackStyle == FontStyle.Regular)
            {
                var font = BeepThemesManager.ToFont(theme.BodyStyle);
                if (font != null) return font;
            }

            // Theme-consistent fallback using BodyMedium as the base typography
            var fallbackTypo = theme?.BodyStyle
                ?? theme?.BodyMedium
                ?? BeepThemesManager.CurrentTheme?.BodyMedium;
            return BeepThemesManager.ToFont(fallbackTypo)
                ?? SystemFonts.DefaultFont;
        }

        /// <summary>
        /// Gets error color from theme token, with a sensible default fallback.
        /// </summary>
        public static Color GetErrorColor(IBeepTheme? theme)
        {
            if (theme?.ErrorColor != null && theme.ErrorColor != Color.Empty)
                return theme.ErrorColor;
            if (BeepThemesManager.CurrentTheme?.ErrorColor != null
                && BeepThemesManager.CurrentTheme.ErrorColor != Color.Empty)
                return BeepThemesManager.CurrentTheme.ErrorColor;
            return Color.FromArgb(200, 50, 50); // Fallback red
        }

        /// <summary>
        /// Gets warning background color derived from theme error token.
        /// </summary>
        public static Color GetWarningBackColor(IBeepTheme? theme)
        {
            var errorColor = GetErrorColor(theme);
            return Color.FromArgb(40, errorColor);
        }

        /// <summary>
        /// Returns true if the OS is in high contrast mode.
        /// </summary>
        public static bool IsHighContrast => SystemInformation.HighContrast;

        /// <summary>
        /// Get a high-contrast-safe border width (minimum 2px).
        /// </summary>
        public static int GetAccessibleBorderWidth(int baseWidth)
            => IsHighContrast ? Math.Max(3, baseWidth) : baseWidth;

        /// <summary>
        /// Get a high-contrast-safe version of a color. Uses system colors when in HC mode.
        /// </summary>
        public static Color GetHighContrastSafeColor(Color themeColor, Color systemFallback)
            => IsHighContrast ? systemFallback : themeColor;

        /// <summary>
        /// Animate a step transition between controls.
        /// Delegates to WizardTransitionEngine with configurable type, easing, and duration.
        /// </summary>
        public static void AnimateStepTransition(
            Control fromControl,
            Control toControl,
            bool forward,
            Action onComplete,
            List<Timer> timerRegistry = null)
        {
            // Legacy default: slide with ease-out-cubic, 300ms
            WizardTransitionEngine.AnimateTransition(
                fromControl, toControl,
                TransitionType.Slide, TransitionEasing.EaseOutCubic,
                300, forward, onComplete, timerRegistry);
        }

        /// <summary>
        /// Animate a fade transition between controls.
        /// Now fully implemented via WizardTransitionEngine.
        /// </summary>
        public static void AnimateFadeTransition(
            Control fromControl,
            Control toControl,
            Action onComplete,
            List<Timer> timerRegistry = null)
        {
            WizardTransitionEngine.AnimateTransition(
                fromControl, toControl,
                TransitionType.Fade, TransitionEasing.EaseOutCubic,
                300, true, onComplete, timerRegistry);
        }

        /// <summary>Smoothly animate a progress bar to a target value.</summary>
        public static void AnimateProgressBar(ProgressBar bar, int targetValue, int durationMs = 300)
        {
            if (bar == null || bar.IsDisposed || WizardManager.ReducedMotion || durationMs <= 0)
            {
                if (bar != null && !bar.IsDisposed)
                    bar.Value = Math.Max(bar.Minimum, Math.Min(bar.Maximum, targetValue));
                return;
            }
            int startValue = bar.Value, delta = targetValue - startValue;
            if (delta == 0) return;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var timer = new Timer { Interval = 16 };
            timer.Tick += (s, e) =>
            {
                float p = Math.Min(1f, (float)sw.ElapsedMilliseconds / durationMs);
                float eased = WizardAnimationEngine.EaseOutCubic(p);
                try { if (!bar.IsDisposed) bar.Value = Math.Max(bar.Minimum, Math.Min(bar.Maximum, startValue + (int)(delta * eased))); }
                catch { timer.Stop(); timer.Dispose(); return; }
                if (p >= 1f) { timer.Stop(); timer.Dispose(); sw.Stop(); }
            };
            timer.Start();
        }
    }
}
