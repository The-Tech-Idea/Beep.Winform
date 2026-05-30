using System;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.Animation
{
    /// <summary>
    /// Easing functions used by docking animations. All take and return a normalized
    /// progress value <c>t</c> in the range [0,1].
    /// </summary>
    public static class Easing
    {
        public static float Linear(float t) => Clamp01(t);

        public static float EaseOutQuad(float t)
        {
            t = Clamp01(t);
            return 1f - (1f - t) * (1f - t);
        }

        public static float EaseInQuad(float t)
        {
            t = Clamp01(t);
            return t * t;
        }

        public static float EaseInOutQuad(float t)
        {
            t = Clamp01(t);
            return t < 0.5f ? 2f * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 2) / 2f;
        }

        public static float EaseOutCubic(float t)
        {
            t = Clamp01(t);
            return 1f - (float)Math.Pow(1f - t, 3);
        }

        private static float Clamp01(float t) => t < 0f ? 0f : (t > 1f ? 1f : t);
    }
}
