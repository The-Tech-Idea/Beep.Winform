using System;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Enhanced animation helpers with more easing functions
    /// Provides smooth, natural animations for tooltip show/hide
    /// </summary>
    public static class ToolTipAnimationHelpers
    {
        /// <summary>
        /// Ease in-out quadratic (smooth acceleration and deceleration)
        /// </summary>
        public static double EaseInOutQuad(double t)
        {
            return t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2;
        }

        /// <summary>
        /// Ease out back (bouncy exit effect)
        /// </summary>
        public static double EaseOutBack(double t)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1;

            return 1 + c3 * Math.Pow(t - 1, 3) + c1 * Math.Pow(t - 1, 2);
        }

        /// <summary>
        /// Ease in-out cubic (smooth cubic transition)
        /// </summary>
        public static double EaseInOutCubic(double t)
        {
            return t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        /// <summary>
        /// Ease in-out quart (stronger acceleration)
        /// </summary>
        public static double EaseInOutQuart(double t)
        {
            return t < 0.5 ? 8 * t * t * t * t : 1 - Math.Pow(-2 * t + 2, 4) / 2;
        }

        /// <summary>
        /// Ease in-out quint (very strong acceleration)
        /// </summary>
        public static double EaseInOutQuint(double t)
        {
            return t < 0.5 ? 16 * t * t * t * t * t : 1 - Math.Pow(-2 * t + 2, 5) / 2;
        }

        /// <summary>
        /// Ease in-out sine (smooth sine wave)
        /// </summary>
        public static double EaseInOutSine(double t)
        {
            return -(Math.Cos(Math.PI * t) - 1) / 2;
        }

        /// <summary>
        /// Ease in-out expo (exponential transition)
        /// </summary>
        public static double EaseInOutExpo(double t)
        {
            return t == 0 ? 0 : t == 1 ? 1 : t < 0.5 
                ? Math.Pow(2, 20 * t - 10) / 2 
                : (2 - Math.Pow(2, -20 * t + 10)) / 2;
        }

        /// <summary>
        /// Ease in-out circ (circular transition)
        /// </summary>
        public static double EaseInOutCirc(double t)
        {
            return t < 0.5
                ? (1 - Math.Sqrt(1 - Math.Pow(2 * t, 2))) / 2
                : (Math.Sqrt(1 - Math.Pow(-2 * t + 2, 2)) + 1) / 2;
        }

        /// <summary>
        /// Ease out elastic (elastic bounce effect)
        /// </summary>
        public static double EaseOutElastic(double t)
        {
            const double c4 = (2 * Math.PI) / 3;

            return t == 0 ? 0 : t == 1 ? 1 : 
                Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1;
        }

        /// <summary>
        /// Calculate opacity based on animation progress and type
        /// </summary>
        public static double CalculateOpacity(double progress, ToolTipAnimation animationType)
        {
            if (progress <= 0) return 0;
            if (progress >= 1) return 1;

            return animationType switch
            {
                ToolTipAnimation.Fade => EaseInOutQuad(progress),
                ToolTipAnimation.Scale => EaseOutBack(progress),
                ToolTipAnimation.Slide => EaseInOutCubic(progress),
                ToolTipAnimation.Bounce => EaseOutElastic(progress),
                _ => progress
            };
        }

        /// <summary>
        /// Calculate scale factor based on animation progress and type
        /// </summary>
        public static double CalculateScale(double progress, ToolTipAnimation animationType)
        {
            if (progress <= 0) return 0.8;
            if (progress >= 1) return 1.0;

            return animationType switch
            {
                ToolTipAnimation.Scale => 0.8 + (0.2 * EaseOutBack(progress)),
                ToolTipAnimation.Bounce => 0.8 + (0.2 * EaseOutElastic(progress)),
                _ => 1.0
            };
        }

        /// <summary>
        /// Calculate position offset for slide animations
        /// </summary>
        public static System.Drawing.Point CalculateOffset(
            double progress, 
            ToolTipPlacement placement, 
            ToolTipAnimation animationType)
        {
            if (animationType != ToolTipAnimation.Slide || progress >= 1)
            {
                return System.Drawing.Point.Empty;
            }

            const int slideDistance = 20;
            var easedProgress = 1.0 - EaseInOutCubic(progress); // Reverse for slide out

            return placement switch
            {
                ToolTipPlacement.Top or ToolTipPlacement.TopStart or ToolTipPlacement.TopEnd =>
                    new System.Drawing.Point(0, (int)(slideDistance * easedProgress)),
                
                ToolTipPlacement.Bottom or ToolTipPlacement.BottomStart or ToolTipPlacement.BottomEnd =>
                    new System.Drawing.Point(0, -(int)(slideDistance * easedProgress)),
                
                ToolTipPlacement.Left or ToolTipPlacement.LeftStart or ToolTipPlacement.LeftEnd =>
                    new System.Drawing.Point((int)(slideDistance * easedProgress), 0),
                
                ToolTipPlacement.Right or ToolTipPlacement.RightStart or ToolTipPlacement.RightEnd =>
                    new System.Drawing.Point(-(int)(slideDistance * easedProgress), 0),
                
                _ => System.Drawing.Point.Empty
            };
        }

        /// <summary>
        /// Get recommended easing function for animation type
        /// </summary>
        public static Func<double, double> GetEasingFunction(ToolTipAnimation animationType)
        {
            return animationType switch
            {
                ToolTipAnimation.Fade => EaseInOutQuad,
                ToolTipAnimation.Scale => EaseOutBack,
                ToolTipAnimation.Slide => EaseInOutCubic,
                ToolTipAnimation.Bounce => EaseOutElastic,
                _ => t => t
            };
        }
    }
}

