using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle
{
    /// <summary>
    /// Animation effects and easing functions for BeepToggle
    /// </summary>
    public partial class BeepToggle
    {
        #region Animation Easing Functions

        /// <summary>
        /// Ease out cubic (default) - starts fast, ends slow
        /// </summary>
        private float EaseOutCubic(float t)
        {
            return 1f - (float)Math.Pow(1f - t, 3);
        }

        /// <summary>
        /// Ease in out cubic - slow at start and end, fast in middle
        /// </summary>
        private float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f;
        }

        /// <summary>
        /// Ease out elastic - bouncy spring effect
        /// </summary>
        private float EaseOutElastic(float t)
        {
            const float c4 = (2f * (float)Math.PI) / 3f;
            
            return t == 0f ? 0f
                : t == 1f ? 1f
                : (float)Math.Pow(2f, -10f * t) * (float)Math.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        /// <summary>
        /// Ease out bounce - bouncing ball effect
        /// </summary>
        private float EaseOutBounce(float t)
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

        /// <summary>
        /// Ease out back - overshoots then settles
        /// </summary>
        private float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return 1f + c3 * (float)Math.Pow(t - 1f, 3) + c1 * (float)Math.Pow(t - 1f, 2);
        }

        #endregion

        #region Animation Effect Types

        /// <summary>
        /// Animation easing type
        /// </summary>
        public enum AnimationEasing
        {
            Linear,
            EaseIn,
            EaseOut,
            EaseInOut,
            EaseOutCubic,
            EaseInOutCubic,
            EaseOutElastic,
            EaseOutBounce,
            EaseOutBack,
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInOutExpo,
            EaseInOutSine
        }

        private AnimationEasing _animationEasing = AnimationEasing.EaseOutCubic;

        /// <summary>
        /// Gets or sets the animation easing function
        /// </summary>
        public AnimationEasing Easing
        {
            get => _animationEasing;
            set => _animationEasing = value;
        }

        /// <summary>
        /// Apply the selected easing function
        /// Uses ToggleAnimationHelpers for consistent easing calculations
        /// </summary>
        private float ApplyEasing(float linearProgress)
        {
            return ToggleAnimationHelpers.GetEasingFunction(_animationEasing)(linearProgress);
        }

        /// <summary>
        /// Get easing function delegate (for backward compatibility)
        /// </summary>
        private Func<float, float> GetEasingFunction()
        {
            return ToggleAnimationHelpers.GetEasingFunction(_animationEasing);
        }

        #endregion

        #region Color Animation

        private Color _currentTrackColor;
        private Color _targetTrackColor;
        private bool _animateColors = true;

        /// <summary>
        /// Gets or sets whether colors should animate during transitions
        /// Automatically disabled when reduced motion is enabled
        /// </summary>
        public bool AnimateColors
        {
            get => _animateColors && !ToggleAccessibilityHelpers.IsReducedMotionEnabled();
            set => _animateColors = value;
        }

        /// <summary>
        /// Interpolate between two colors
        /// Uses ToggleAnimationHelpers for consistent color transitions
        /// </summary>
        private Color InterpolateColor(Color from, Color to, float progress)
        {
            return ToggleAnimationHelpers.CalculateColorTransition(from, to, progress);
        }

        #endregion

        #region Glow/Pulse Effects

        private bool _enableFocusGlow = true;
        private float _glowIntensity = 0f;
        private bool _glowIncreasing = true;

        /// <summary>
        /// Gets or sets whether focus glow effect is enabled
        /// Automatically disabled when reduced motion is enabled
        /// </summary>
        public bool EnableFocusGlow
        {
            get => _enableFocusGlow && !ToggleAccessibilityHelpers.IsReducedMotionEnabled();
            set => _enableFocusGlow = value;
        }

        /// <summary>
        /// Update glow animation
        /// </summary>
        private void UpdateGlowEffect()
        {
            if (!_enableFocusGlow || !Focused)
            {
                _glowIntensity = 0f;
                return;
            }

            // Pulse effect
            const float glowSpeed = 0.05f;
            
            if (_glowIncreasing)
            {
                _glowIntensity += glowSpeed;
                if (_glowIntensity >= 1f)
                {
                    _glowIntensity = 1f;
                    _glowIncreasing = false;
                }
            }
            else
            {
                _glowIntensity -= glowSpeed;
                if (_glowIntensity <= 0.3f)
                {
                    _glowIntensity = 0.3f;
                    _glowIncreasing = true;
                }
            }
        }

        /// <summary>
        /// Get current glow color with intensity
        /// </summary>
        public Color GetGlowColor()
        {
            if (!_enableFocusGlow || _glowIntensity == 0f)
                return Color.Transparent;

            Color baseGlowColor = _isOn ? _onColor : _offColor;
            int alpha = (int)(100 * _glowIntensity);
            
            return Color.FromArgb(alpha, baseGlowColor);
        }

        #endregion
    }
}
