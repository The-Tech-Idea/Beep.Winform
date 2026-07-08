using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Helper class for managing tab animations including hover and transition effects
    /// </summary>
    internal class TabAnimationHelper
    {
        private readonly System.Windows.Forms.Timer _animationTimer;
        private readonly Action _invalidateCallback;

        // Transition animation state
        public float TransitionProgress { get; private set; } = 1.0f;
        public float TargetTransitionProgress { get; private set; } = 1.0f;
        public bool IsTransitioning { get; private set; } = false;

        public TabAnimationHelper(Action invalidateCallback)
        {
            _invalidateCallback = invalidateCallback;
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // 60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        public void StartAnimation(AddinTab tab, float targetProgress)
        {
            if (tab == null) return;
            
            tab.TargetAnimationProgress = Math.Max(0, Math.Min(1, targetProgress));
            
            if (!_animationTimer.Enabled)
            {
                _animationTimer.Start();
            }
        }

        public void StartTransition(float targetProgress = 1.0f)
        {
            TargetTransitionProgress = Math.Max(0, Math.Min(1, targetProgress));
            TransitionProgress = 0.0f;
            IsTransitioning = true;
            
            if (!_animationTimer.Enabled)
            {
                _animationTimer.Start();
            }
        }

        public void StopTransition()
        {
            IsTransitioning = false;
            TransitionProgress = 1.0f;
            TargetTransitionProgress = 1.0f;
        }

        public void UpdateAnimations(System.Collections.Generic.List<AddinTab> tabs, AnimationSpeed speed)
        {
            bool needsUpdate = false;
            float animSpeed = GetAnimationSpeed(speed);

            // DC-08: Tab hover animations with WizardAnimationEngine easing
            float dt = animSpeed; // per-frame delta
            foreach (var tab in tabs)
            {
                if (Math.Abs(tab.AnimationProgress - tab.TargetAnimationProgress) > 0.005f)
                {
                    // Move progress toward target using eased interpolation
                    float target = tab.TargetAnimationProgress;
                    float current = tab.AnimationProgress;
                    float newProgress = current + (target - current) * dt * 4f;
                    // Apply ease-out: decelerate as we approach target
                    float t = Math.Abs(target > 0.01f ? newProgress / target : newProgress);
                    float eased = WizardAnimationEngine.EaseOutCubic(Math.Min(1f, t));
                    tab.AnimationProgress = Math.Max(0f, Math.Min(1f, eased * target));

                    if (Math.Abs(tab.AnimationProgress - tab.TargetAnimationProgress) < 0.01f)
                        tab.AnimationProgress = tab.TargetAnimationProgress;

                    needsUpdate = true;
                }
            }

            // Transition animation with easing
            if (IsTransitioning)
            {
                float raw = TransitionProgress + (TargetTransitionProgress - TransitionProgress) * dt * 6f;
                TransitionProgress = Math.Max(0f, Math.Min(1f, raw));

                if (Math.Abs(TransitionProgress - TargetTransitionProgress) < 0.01f)
                {
                    TransitionProgress = 1.0f;
                    IsTransitioning = false;
                }
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                _invalidateCallback?.Invoke();
            }
            else if (!IsTransitioning)
            {
                _animationTimer.Stop();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // This will be called by the container with the current tabs and speed
        }

        private float GetAnimationSpeed(AnimationSpeed speed)
        {
            return speed switch
            {
                AnimationSpeed.Slow => 0.02f,
                AnimationSpeed.Normal => 0.05f,
                AnimationSpeed.Fast => 0.1f,
                _ => 0.05f
            };
        }

        public void Dispose()
        {
            _animationTimer?.Stop();
            _animationTimer?.Dispose();
        }
    }
}