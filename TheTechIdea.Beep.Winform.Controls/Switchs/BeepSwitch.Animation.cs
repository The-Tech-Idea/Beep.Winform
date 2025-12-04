using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSwitch - Animation system
    /// </summary>
    public partial class BeepSwitch
    {
        /// <summary>
        /// Animates the switch toggle with smooth transition
        /// </summary>
        /// <param name="newState">Target state (true = On, false = Off)</param>
        private void AnimateToggle(bool newState)
        {
            if (_animating)
            {
                // Stop current animation
                _animTimer?.Stop();
                _animTimer?.Dispose();
            }
            
            // Quick toggle if animations disabled or painter not initialized
            if (_painter == null)
            {
                _checked = newState;
                _animProgress = newState ? 1.0f : 0.0f;
                UpdateMetrics();
                Invalidate();
                return;
            }
            
            _animating = true;
            float targetProgress = newState ? 1.0f : 0.0f;
            int duration = _painter.GetAnimationDuration();
            int steps = duration / 16;  // 60 FPS (16ms per frame)
            if (steps == 0) steps = 1;
            
            float stepSize = Math.Abs(targetProgress - _animProgress) / steps;
            
            _animTimer = new Timer { Interval = 16 };
            int currentStep = 0;
            
            _animTimer.Tick += (s, e) =>
            {
                currentStep++;
                
                // Easing function (ease-out cubic for smooth deceleration)
                float t = (float)currentStep / steps;
                float easedT = 1 - (float)Math.Pow(1 - t, 3);
                
                if (newState)
                {
                    _animProgress = easedT;
                }
                else
                {
                    _animProgress = 1.0f - easedT;
                }
                
                _animProgress = Math.Max(0f, Math.Min(1.0f, _animProgress));
                
                UpdateMetrics();
                Invalidate();
                
                if (currentStep >= steps)
                {
                    _animTimer.Stop();
                    _animTimer.Dispose();
                    _animTimer = null;
                    _animating = false;
                    _animProgress = targetProgress;
                    _checked = newState;
                    UpdateMetrics();
                    Invalidate();
                }
            };
            
            _animTimer.Start();
        }
        
        /// <summary>
        /// Updates metrics with current animation progress
        /// </summary>
        private void UpdateMetrics()
        {
            if (_painter == null) return;
            
            _metrics.AnimationProgress = _animProgress;
            _painter.CalculateLayout(this, _metrics);
        }
    }
}

