using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Helper class for managing tab animations
    /// </summary>
    internal class TabAnimationHelper
    {
        private readonly System.Windows.Forms.Timer _animationTimer;
        private readonly Action _invalidateCallback;

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

        public void UpdateAnimations(System.Collections.Generic.List<AddinTab> tabs, AnimationSpeed speed)
        {
            bool needsUpdate = false;
            float animSpeed = GetAnimationSpeed(speed);

            foreach (var tab in tabs)
            {
                if (Math.Abs(tab.AnimationProgress - tab.TargetAnimationProgress) > 0.01f)
                {
                    if (tab.AnimationProgress < tab.TargetAnimationProgress)
                    {
                        tab.AnimationProgress = Math.Min(tab.TargetAnimationProgress, 
                            tab.AnimationProgress + animSpeed);
                    }
                    else
                    {
                        tab.AnimationProgress = Math.Max(tab.TargetAnimationProgress, 
                            tab.AnimationProgress - animSpeed);
                    }
                    needsUpdate = true;
                }
            }

            if (needsUpdate)
            {
                _invalidateCallback?.Invoke();
            }
            else
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