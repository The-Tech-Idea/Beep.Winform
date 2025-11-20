using System;
namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Simple easing helper to evaluate easing functions expressed as strings
    /// Supports some named functions and CubicBezier(x1,y1,x2,y2).
    /// </summary>
    public static class AnimationEasingHelper
    {
        public static float Evaluate(string easing, float progress)
        {
            if (string.IsNullOrEmpty(easing)) return progress;
            easing = easing.Trim().ToLowerInvariant();
            if (easing == "linear") return progress;
            if (easing == "ease-in") return EaseIn(progress);
            if (easing == "ease-out") return EaseOut(progress);
            if (easing == "ease-in-out") return EaseInOut(progress);
            if (easing.StartsWith("cubicbezier("))
            {
                try
                {
                    var inner = easing.Substring("cubicbezier(".Length).TrimEnd(')');
                    var parts = inner.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        float x1 = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                        float y1 = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                        float x2 = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                        float y2 = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                        return CubicBezierY(progress, y1, y2, x1, x2);
                    }
                }
                catch
                {
                    return progress;
                }
            }
            return progress;
        }

        private static float EaseIn(float t) => t * t;
        private static float EaseOut(float t) => 1 - (1 - t) * (1 - t);
        private static float EaseInOut(float t) => t < 0.5f ? 2 * t * t : 1 - 2 * (1 - t) * (1 - t);

        // Simplified cubic bezier evaluation: returns y for given t (not solving for x)
        private static float CubicBezierY(float t, float y1, float y2, float x1, float x2)
        {
            // Polynomial form: B(t) = (1 - t)^3*0 + 3(1 - t)^2 t y1 + 3(1 - t) t^2 y2 + t^3*1
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float a = 3 * uu * t * y1;
            float b = 3 * u * tt * y2;
            float c = tt * t;
            return a + b + c;
        }
    }
}
