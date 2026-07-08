using System;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Helpers
{
    /// <summary>
    /// Easing functions for wizard step transitions.
    /// All functions accept a normalized time t in [0, 1] and return an eased value in [0, 1].
    /// </summary>
    public static class WizardAnimationEngine
    {
        // ── Linear ──────────────────────────────────────────────────────────

        public static float Linear(float t) => t;

        // ── Quad ────────────────────────────────────────────────────────────

        public static float EaseInQuad(float t) => t * t;
        public static float EaseOutQuad(float t) => t * (2f - t);
        public static float EaseInOutQuad(float t)
        {
            t *= 2f;
            return t < 1f ? 0.5f * t * t : -0.5f * (--t * (t - 2f) - 1f);
        }

        // ── Cubic ───────────────────────────────────────────────────────────

        public static float EaseInCubic(float t) => t * t * t;
        public static float EaseOutCubic(float t) => 1f - (float)Math.Pow(1f - t, 3);
        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f;
        }

        // ── Quart ────────────────────────────────────────────────────────────

        public static float EaseInQuart(float t) => t * t * t * t;
        public static float EaseOutQuart(float t) => 1f - (float)Math.Pow(1f - t, 4);
        public static float EaseInOutQuart(float t)
        {
            t *= 2f;
            return t < 1f ? 0.5f * t * t * t * t : -0.5f * ((t -= 2f) * t * t * t - 2f);
        }

        // ── Quint ────────────────────────────────────────────────────────────

        public static float EaseInQuint(float t) => t * t * t * t * t;
        public static float EaseOutQuint(float t) => 1f - (float)Math.Pow(1f - t, 5);
        public static float EaseInOutQuint(float t)
        {
            t *= 2f;
            return t < 1f ? 0.5f * t * t * t * t * t : 0.5f * ((t -= 2f) * t * t * t * t + 2f);
        }

        // ── Back (overshoot) ─────────────────────────────────────────────────

        private const float C1 = 1.70158f;
        private const float C2 = C1 * 1.525f;
        private const float C3 = C1 + 1f;

        public static float EaseInBack(float t) => C3 * t * t * t - C1 * t * t;
        public static float EaseOutBack(float t) => 1f + C3 * (float)Math.Pow(t - 1f, 3) + C1 * (float)Math.Pow(t - 1f, 2);
        public static float EaseInOutBack(float t)
        {
            t *= 2f;
            return t < 1f
                ? 0.5f * (t * t * ((C2 + 1f) * t - C2))
                : 0.5f * ((t -= 2f) * t * ((C2 + 1f) * t + C2) + 2f);
        }

        // ── Bounce ───────────────────────────────────────────────────────────

        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            if (t < 1f / d1)
                return n1 * t * t;
            if (t < 2f / d1)
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            if (t < 2.5f / d1)
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }

        public static float EaseInBounce(float t) => 1f - EaseOutBounce(1f - t);

        public static float EaseInOutBounce(float t)
        {
            return t < 0.5f
                ? (1f - EaseOutBounce(1f - 2f * t)) / 2f
                : (1f + EaseOutBounce(2f * t - 1f)) / 2f;
        }

        // ── Elastic ──────────────────────────────────────────────────────────

        public static float EaseOutElastic(float t)
        {
            const float c4 = 2f * (float)Math.PI / 3f;
            return t == 0f ? 0f
                 : Math.Abs(t - 1f) < float.Epsilon ? 1f
                 : (float)(Math.Pow(2f, -10f * t) * Math.Sin((t * 10f - 0.75f) * c4) + 1f);
        }

        public static float EaseInElastic(float t)
        {
            const float c4 = 2f * (float)Math.PI / 3f;
            return t == 0f ? 0f
                 : Math.Abs(t - 1f) < float.Epsilon ? 1f
                 : -(float)(Math.Pow(2f, 10f * t - 10f) * Math.Sin((t * 10f - 10.75f) * c4));
        }

        // ── Spring ───────────────────────────────────────────────────────────

        /// <summary>Configurable spring easing with damping.</summary>
        /// <param name="t">Normalized time [0, 1].</param>
        /// <param name="damping">Damping ratio (0.3 = bouncy, 0.8 = stiff).</param>
        public static float Spring(float t, float damping = 0.6f)
        {
            float c = Math.Max(0.1f, Math.Min(1f, damping));
            float decay = (float)Math.Exp(-t * 6f * c);
            float oscillation = (float)Math.Cos(t * 12f * (1f - c));
            return 1f - decay * oscillation;
        }

        // ── Resolver ─────────────────────────────────────────────────────────

        /// <summary>Resolve a TransitionEasing enum to its function delegate.</summary>
        public static Func<float, float> Resolve(TransitionEasing easing)
        {
            return easing switch
            {
                TransitionEasing.Linear => Linear,
                TransitionEasing.EaseInCubic => EaseInCubic,
                TransitionEasing.EaseOutCubic => EaseOutCubic,
                TransitionEasing.EaseInOutCubic => EaseInOutCubic,
                TransitionEasing.EaseInQuad => EaseInQuad,
                TransitionEasing.EaseOutQuad => EaseOutQuad,
                TransitionEasing.EaseInOutQuad => EaseInOutQuad,
                TransitionEasing.EaseInQuart => EaseInQuart,
                TransitionEasing.EaseOutQuart => EaseOutQuart,
                TransitionEasing.EaseOutQuint => EaseOutQuint,
                TransitionEasing.EaseOutBack => EaseOutBack,
                TransitionEasing.EaseInOutBack => EaseInOutBack,
                TransitionEasing.EaseOutBounce => EaseOutBounce,
                TransitionEasing.EaseOutElastic => EaseOutElastic,
                TransitionEasing.Spring => t => Spring(t),
                _ => EaseOutCubic,
            };
        }
    }
}
