using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepSwitch
    {
        private void AnimateToggle(bool newState)
        {
            if (_animating)
            {
                _animTimer?.Stop();
                _animTimer?.Dispose();
            }
            
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
            int steps = Math.Max(1, duration / 16);
            
            _animTimer = new Timer { Interval = 16 };
            int currentStep = 0;
            
            bool useSpring = ControlStyle == BeepControlStyle.iOS15 || 
                             ControlStyle == BeepControlStyle.MacOSBigSur ||
                             ControlStyle == BeepControlStyle.Apple;
            
            _animTimer.Tick += (s, e) =>
            {
                currentStep++;
                
                float t = (float)currentStep / steps;
                float easedT = useSpring 
                    ? EaseOutSpring(t) 
                    : EaseOutCubic(t);
                
                _animProgress = newState ? easedT : 1.0f - easedT;
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
        
        private static float EaseOutCubic(float t)
        {
            return 1 - (float)Math.Pow(1 - t, 3);
        }
        
        private static float EaseOutSpring(float t)
        {
            const float p = 0.4f;
            const float s = p / 4;
            
            if (t == 0 || t == 1) return t;
            
            return (float)(Math.Pow(2, -10 * t) * Math.Sin((t - s) * (2 * Math.PI) / p) + 1);
        }
        
        private void UpdateMetrics()
        {
            if (_painter == null) return;
            
            _metrics.AnimationProgress = _animProgress;
            _painter.CalculateLayout(this, _metrics);
        }
    }
}

