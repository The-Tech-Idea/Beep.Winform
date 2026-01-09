using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Toggle;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Helpers
{
    /// <summary>
    /// Helper class for animation calculations and easing functions
    /// Centralizes animation logic for toggle transitions
    /// </summary>
    public static class ToggleAnimationHelpers
    {
        /// <summary>
        /// Calculate thumb position based on animation progress and easing
        /// </summary>
        public static float CalculateThumbPosition(
            float progress,
            BeepToggle.AnimationEasing easing,
            bool isOn)
        {
            // Clamp progress to 0-1
            progress = Math.Max(0f, Math.Min(1f, progress));

            // Apply easing function
            float easedProgress = GetEasingFunction(easing)(progress);

            // Return position (0 = OFF, 1 = ON)
            return isOn ? easedProgress : 1f - easedProgress;
        }

        /// <summary>
        /// Calculate color transition between two colors
        /// </summary>
        public static Color CalculateColorTransition(
            Color from,
            Color to,
            float progress)
        {
            progress = Math.Max(0f, Math.Min(1f, progress));

            int r = (int)(from.R + (to.R - from.R) * progress);
            int g = (int)(from.G + (to.G - from.G) * progress);
            int b = (int)(from.B + (to.B - from.B) * progress);
            int a = (int)(from.A + (to.A - from.A) * progress);

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Calculate scale factor for animation
        /// </summary>
        public static float CalculateScale(
            float progress,
            BeepToggle.AnimationEasing easing,
            float fromScale = 1f,
            float toScale = 1f)
        {
            progress = Math.Max(0f, Math.Min(1f, progress));
            float easedProgress = GetEasingFunction(easing)(progress);

            return fromScale + (toScale - fromScale) * easedProgress;
        }

        /// <summary>
        /// Calculate glow intensity for animation
        /// </summary>
        public static float CalculateGlowIntensity(
            float progress,
            bool isOn,
            float maxIntensity = 0.3f)
        {
            progress = Math.Max(0f, Math.Min(1f, progress));

            // Glow is strongest at the start of transition
            float intensity = isOn
                ? (1f - progress) * maxIntensity
                : progress * maxIntensity;

            return intensity;
        }

        /// <summary>
        /// Get easing function delegate for animation easing type
        /// </summary>
        public static Func<float, float> GetEasingFunction(BeepToggle.AnimationEasing easing)
        {
            return easing switch
            {
                BeepToggle.AnimationEasing.Linear => EaseLinear,
                BeepToggle.AnimationEasing.EaseIn => EaseInQuad,
                BeepToggle.AnimationEasing.EaseOut => EaseOutQuad,
                BeepToggle.AnimationEasing.EaseInOut => EaseInOutQuad,
                BeepToggle.AnimationEasing.EaseOutCubic => EaseOutCubic,
                BeepToggle.AnimationEasing.EaseInOutCubic => EaseInOutCubic,
                BeepToggle.AnimationEasing.EaseOutElastic => EaseOutElastic,
                BeepToggle.AnimationEasing.EaseOutBounce => EaseOutBounce,
                BeepToggle.AnimationEasing.EaseOutBack => EaseOutBack,
                BeepToggle.AnimationEasing.EaseInQuad => EaseInQuad,
                BeepToggle.AnimationEasing.EaseOutQuad => EaseOutQuad,
                BeepToggle.AnimationEasing.EaseInOutQuad => EaseInOutQuad,
                BeepToggle.AnimationEasing.EaseInOutExpo => EaseInOutExpo,
                BeepToggle.AnimationEasing.EaseInOutSine => EaseInOutSine,
                _ => EaseOutQuad // Default
            };
        }

        #region Easing Functions

        private static float EaseLinear(float t) => t;

        private static float EaseInQuad(float t) => t * t;

        private static float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);

        private static float EaseInOutQuad(float t)
        {
            return t < 0.5f
                ? 2f * t * t
                : 1f - (float)Math.Pow(-2f * t + 2f, 2) / 2f;
        }

        private static float EaseOutCubic(float t)
        {
            return 1f - (float)Math.Pow(1f - t, 3);
        }

        private static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f;
        }

        private static float EaseOutElastic(float t)
        {
            const float c4 = (2f * (float)Math.PI) / 3f;
            return t == 0f ? 0f :
                   t == 1f ? 1f :
                   (float)Math.Pow(2f, -10f * t) * (float)Math.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        private static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1f / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2f / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5f / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }

        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * (float)Math.Pow(t - 1f, 3) + c1 * (float)Math.Pow(t - 1f, 2);
        }

        private static float EaseInOutExpo(float t)
        {
            return t == 0f ? 0f :
                   t == 1f ? 1f :
                   t < 0.5f
                       ? (float)Math.Pow(2f, 20f * t - 10f) / 2f
                       : (2f - (float)Math.Pow(2f, -20f * t + 10f)) / 2f;
        }

        private static float EaseInOutSine(float t)
        {
            return -(float)(Math.Cos(Math.PI * t) - 1f) / 2f;
        }

        #endregion
    }
}
