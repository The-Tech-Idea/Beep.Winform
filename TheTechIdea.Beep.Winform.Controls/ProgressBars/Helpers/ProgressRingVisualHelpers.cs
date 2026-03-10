using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    internal static class ProgressRingVisualHelpers
    {
        public static Rectangle GetSquareRingRect(Rectangle bounds, int inset)
        {
            int size = Math.Max(0, Math.Min(bounds.Width, bounds.Height) - inset * 2);
            int x = bounds.X + (bounds.Width - size) / 2;
            int y = bounds.Y + (bounds.Height - size) / 2;
            return new Rectangle(x, y, size, size);
        }

        public static int GetClampedThickness(BeepProgressBar owner, Rectangle bounds, int requestedThickness)
        {
            int min = ProgressBarDpiHelpers.Scale(owner, 4);
            int max = Math.Max(min, Math.Min(bounds.Width, bounds.Height) / 4);
            return Math.Max(min, Math.Min(requestedThickness, max));
        }

        public static int GetShadowOffset(BeepProgressBar owner)
        {
            return ProgressBarDpiHelpers.Scale(owner, 1);
        }

        public static int GetRingShadowAlpha(IReadOnlyDictionary<string, object> parameters, bool enabled)
        {
            return GetProfileAlpha(parameters, enabled, 22, 12, 34, 20);
        }

        public static int GetDotShadowAlpha(IReadOnlyDictionary<string, object> parameters, bool enabled)
        {
            return GetProfileAlpha(parameters, enabled, 18, 10, 28, 16);
        }

        public static int GetTrackAlpha(
            IReadOnlyDictionary<string, object> parameters,
            bool enabled,
            int lighterEnabled,
            int lighterDisabled,
            int strongerEnabled,
            int strongerDisabled)
        {
            return GetProfileAlpha(parameters, enabled, lighterEnabled, lighterDisabled, strongerEnabled, strongerDisabled);
        }

        public static int GetDisabledAccentAlpha(IReadOnlyDictionary<string, object> parameters)
        {
            return GetProfileAlpha(parameters, false, 118, 118, 145, 145);
        }

        public static int GetDisabledTextAlpha(IReadOnlyDictionary<string, object> parameters)
        {
            return GetProfileAlpha(parameters, false, 136, 136, 160, 160);
        }

        private static int GetProfileAlpha(
            IReadOnlyDictionary<string, object> parameters,
            bool enabled,
            int lighterEnabled,
            int lighterDisabled,
            int strongerEnabled,
            int strongerDisabled)
        {
            float t = GetDepthStrength(parameters);
            int lighter = enabled ? lighterEnabled : lighterDisabled;
            int stronger = enabled ? strongerEnabled : strongerDisabled;
            return ClampAlpha((int)Math.Round(Lerp(lighter, stronger, t)));
        }

        private static float GetDepthStrength(IReadOnlyDictionary<string, object> parameters)
        {
            // RingDepthProfile: "lighter" (default) or "stronger"
            if (parameters != null &&
                parameters.TryGetValue("RingDepthProfile", out var profileValue) &&
                profileValue is string profile)
            {
                profile = profile.Trim().ToLowerInvariant();
                if (profile == "stronger")
                {
                    return 1f;
                }
            }

            // Optional fine tune: RingDepthStrength in range [0..1]
            if (parameters != null &&
                parameters.TryGetValue("RingDepthStrength", out var strengthValue) &&
                strengthValue is IConvertible convertible)
            {
                float raw = Convert.ToSingle(convertible);
                if (raw < 0f) return 0f;
                if (raw > 1f) return 1f;
                return raw;
            }

            return 0f;
        }

        private static float Lerp(float a, float b, float t) => a + (b - a) * t;
        private static int ClampAlpha(int alpha) => Math.Max(0, Math.Min(255, alpha));
    }
}
