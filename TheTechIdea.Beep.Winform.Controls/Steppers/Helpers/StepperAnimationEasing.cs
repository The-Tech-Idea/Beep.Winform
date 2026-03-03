using System;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    internal static class StepperAnimationEasing
    {
        public static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }

        public static float Linear(float t) => Clamp01(t);

        public static float CubicEaseOut(float t)
        {
            float x = Clamp01(t);
            float inv = 1f - x;
            return 1f - (inv * inv * inv);
        }

        public static float SpringEaseOut(float t)
        {
            float x = Clamp01(t);
            return (float)(1 - Math.Exp(-6 * x) * Math.Cos(9 * x));
        }
    }
}
