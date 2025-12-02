using System;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Modern easing functions for dock animations
    /// Implements easing functions from popular animation libraries (GSAP, Framer Motion, etc.)
    /// </summary>
    public static class DockEasingHelper
    {
        #region Standard Easing Functions

        /// <summary>
        /// Linear interpolation (no easing)
        /// </summary>
        public static float Linear(float t)
        {
            return t;
        }

        #endregion

        #region Quadratic Easing

        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        public static float EaseOutQuad(float t)
        {
            return t * (2 - t);
        }

        public static float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }

        #endregion

        #region Cubic Easing

        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }

        public static float EaseOutCubic(float t)
        {
            return (--t) * t * t + 1;
        }

        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
        }

        #endregion

        #region Quartic Easing

        public static float EaseInQuart(float t)
        {
            return t * t * t * t;
        }

        public static float EaseOutQuart(float t)
        {
            return 1 - (--t) * t * t * t;
        }

        public static float EaseInOutQuart(float t)
        {
            return t < 0.5f ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
        }

        #endregion

        #region Quintic Easing

        public static float EaseInQuint(float t)
        {
            return t * t * t * t * t;
        }

        public static float EaseOutQuint(float t)
        {
            return 1 + (--t) * t * t * t * t;
        }

        public static float EaseInOutQuint(float t)
        {
            return t < 0.5f ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;
        }

        #endregion

        #region Exponential Easing

        public static float EaseInExpo(float t)
        {
            return t == 0 ? 0 : (float)Math.Pow(2, 10 * (t - 1));
        }

        public static float EaseOutExpo(float t)
        {
            return t == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * t);
        }

        public static float EaseInOutExpo(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;
            if (t < 0.5f)
                return (float)Math.Pow(2, 20 * t - 10) / 2;
            return (2 - (float)Math.Pow(2, -20 * t + 10)) / 2;
        }

        #endregion

        #region Circular Easing

        public static float EaseInCirc(float t)
        {
            return 1 - (float)Math.Sqrt(1 - t * t);
        }

        public static float EaseOutCirc(float t)
        {
            return (float)Math.Sqrt(1 - (--t) * t);
        }

        public static float EaseInOutCirc(float t)
        {
            if (t < 0.5f)
                return (1 - (float)Math.Sqrt(1 - 4 * t * t)) / 2;
            return ((float)Math.Sqrt(1 - (-2 * t + 2) * (-2 * t + 2)) + 1) / 2;
        }

        #endregion

        #region Back Easing (Overshoot)

        public static float EaseInBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * t * t * t - c1 * t * t;
        }

        public static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * (float)Math.Pow(t - 1, 3) + c1 * (float)Math.Pow(t - 1, 2);
        }

        public static float EaseInOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;

            return t < 0.5f
                ? ((float)Math.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
                : ((float)Math.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        }

        #endregion

        #region Elastic Easing

        public static float EaseInElastic(float t)
        {
            const float c4 = (2 * (float)Math.PI) / 3;

            return t == 0 ? 0 : t == 1 ? 1
                : -(float)Math.Pow(2, 10 * t - 10) * (float)Math.Sin((t * 10 - 10.75f) * c4);
        }

        public static float EaseOutElastic(float t)
        {
            const float c4 = (2 * (float)Math.PI) / 3;

            return t == 0 ? 0 : t == 1 ? 1
                : (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * 10 - 0.75f) * c4) + 1;
        }

        public static float EaseInOutElastic(float t)
        {
            const float c5 = (2 * (float)Math.PI) / 4.5f;

            return t == 0 ? 0 : t == 1 ? 1 : t < 0.5f
                ? -((float)Math.Pow(2, 20 * t - 10) * (float)Math.Sin((20 * t - 11.125f) * c5)) / 2
                : ((float)Math.Pow(2, -20 * t + 10) * (float)Math.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
        }

        #endregion

        #region Bounce Easing

        public static float EaseInBounce(float t)
        {
            return 1 - EaseOutBounce(1 - t);
        }

        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
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

        public static float EaseInOutBounce(float t)
        {
            return t < 0.5f
                ? (1 - EaseOutBounce(1 - 2 * t)) / 2
                : (1 + EaseOutBounce(2 * t - 1)) / 2;
        }

        #endregion

        #region Spring Easing (Physics-based)

        /// <summary>
        /// Spring animation with damping and stiffness
        /// </summary>
        public static float Spring(float t, float stiffness = 100f, float damping = 10f, float mass = 1f)
        {
            // Simplified spring physics
            float w0 = (float)Math.Sqrt(stiffness / mass);
            float dampingRatio = damping / (2 * (float)Math.Sqrt(stiffness * mass));

            if (dampingRatio < 1) // Underdamped
            {
                float wd = w0 * (float)Math.Sqrt(1 - dampingRatio * dampingRatio);
                float A = 1;
                float phi = (float)Math.Atan2(-damping / (2 * mass), wd);
                return 1 - (A * (float)Math.Exp(-dampingRatio * w0 * t) * 
                    (float)Math.Cos(wd * t + phi) / (float)Math.Cos(phi));
            }
            else if (dampingRatio == 1) // Critically damped
            {
                return 1 - (float)Math.Exp(-w0 * t) * (1 + w0 * t);
            }
            else // Overdamped
            {
                float r1 = (-dampingRatio + (float)Math.Sqrt(dampingRatio * dampingRatio - 1)) * w0;
                float r2 = (-dampingRatio - (float)Math.Sqrt(dampingRatio * dampingRatio - 1)) * w0;
                return 1 - ((r2 * (float)Math.Exp(r1 * t) - r1 * (float)Math.Exp(r2 * t)) / (r2 - r1));
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets an easing function by name
        /// </summary>
        public static Func<float, float> GetEasingFunction(DockAnimationStyle style)
        {
            return style switch
            {
                DockAnimationStyle.Spring => (t) => Spring(t, 150f, 15f, 1f),
                DockAnimationStyle.Scale => EaseOutCubic,
                DockAnimationStyle.Bounce => EaseOutBounce,
                DockAnimationStyle.Elastic => EaseOutElastic,
                DockAnimationStyle.Fade => EaseInOutQuad,
                DockAnimationStyle.Slide => EaseOutQuart,
                DockAnimationStyle.Pulse => (t) => EaseInOutQuad(t),
                DockAnimationStyle.Rotate => EaseOutCubic,
                _ => EaseOutCubic
            };
        }

        /// <summary>
        /// Interpolates between two values using an easing function
        /// </summary>
        public static float Lerp(float start, float end, float t, Func<float, float> easingFunc = null)
        {
            t = Math.Max(0, Math.Min(1, t)); // Clamp to [0, 1]
            
            if (easingFunc != null)
                t = easingFunc(t);
            
            return start + (end - start) * t;
        }

        /// <summary>
        /// Smoothly interpolates between current and target value (smooth damping)
        /// </summary>
        public static float SmoothDamp(float current, float target, ref float currentVelocity, 
            float smoothTime, float maxSpeed = float.PositiveInfinity, float deltaTime = 0.016f)
        {
            smoothTime = Math.Max(0.0001f, smoothTime);
            float omega = 2f / smoothTime;
            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
            float change = current - target;
            float originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = Math.Max(-maxChange, Math.Min(change, maxChange));
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            float output = target + (change + temp) * exp;

            // Prevent overshooting
            if (originalTo - current > 0.0f == output > originalTo)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        #endregion
    }
}

