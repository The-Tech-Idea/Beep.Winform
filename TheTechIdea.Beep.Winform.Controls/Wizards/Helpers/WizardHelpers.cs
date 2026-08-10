using TheTechIdea.Beep.Winform.Controls.Diagnostics;
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
        /// Resolves the font FAMILY from the supplied theme typography and returns that family at
        /// the caller's requested <paramref name="size"/> and <paramref name="fontStyle"/>.
        /// </summary>
        /// <remarks>
        /// The typography contributes its FAMILY only. Its own size, weight, underline and strikeout
        /// are deliberately ignored, because callers ask one typography slot (e.g. BodyStyle) for
        /// several distinct sizes and weights — a card title at 10pt bold, its description at 8.5pt
        /// regular, its number at 12pt bold.
        /// <para>
        /// The last two parameters used to be named <c>fallbackSize</c>/<c>fallbackStyle</c> and were
        /// ignored entirely, so those three calls all returned THE SAME cached instance; callers then
        /// disposed it once per field and kept drawing with it, which threw inside OnPaint.
        /// </para>
        /// <para>
        /// The returned Font is owned by the font cache. Callers MUST NOT dispose it and MUST NOT
        /// wrap it in <c>using</c>; re-call this on theme or DPI change instead of holding it forever.
        /// </para>
        /// </remarks>
        public static Font GetFont(IBeepTheme? theme, TypographyStyle? style, float size, FontStyle fontStyle)
        {
            TypographyStyle? source = style
                ?? theme?.BodyStyle
                ?? theme?.BodyMedium
                ?? BeepThemesManager.CurrentTheme?.BodyStyle;

            float resolvedSize = size > 0f
                ? size
                : (source != null && source.FontSize > 0f ? source.FontSize : 9f);

            // Family from the theme; size and style from the caller. FontWeight/IsUnderlined/
            // IsStrikeout are neutralised because ToFont ORs them into the FontStyle — leaving the
            // source values in would re-add Bold to a caller that explicitly asked for Regular.
            var request = new TypographyStyle
            {
                FontFamily = source?.FontFamily,
                FontSize = resolvedSize,
                FontStyle = fontStyle,
                FontWeight = FontWeight.Normal,
                IsUnderlined = false,
                IsStrikeout = false
            };

            // ToFont never returns null (it terminates at SystemFonts.DefaultFont), so a null guard
            // here would be a check that cannot fail.
            return BeepThemesManager.ToFont(request);
        }

        /// <summary>
        /// Gets error color from theme token, with a sensible default fallback.
        /// </summary>
        public static Color GetErrorColor(IBeepTheme? theme)
        {
            // There is always a theme; the slot is the theme's decision (standing directive).
            return (theme ?? BeepThemesManager.CurrentTheme).ErrorColor;
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
                catch (Exception ex)
                {
                    BeepLog.FailureOnce("Wizard.animTick", null, "run wizard animation tick", ex);
                    timer.Stop(); timer.Dispose(); return;
                }
                if (p >= 1f) { timer.Stop(); timer.Dispose(); sw.Stop(); }
            };
            timer.Start();
        }
    }
}
