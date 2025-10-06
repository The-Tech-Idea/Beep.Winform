using System;
using System.Windows.Forms;
 

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    public partial class BeepSideBar
    {
        #region Animation Fields
        private Timer _animationTimer;
        private bool _isAnimating = false;
        private int _animationStep = 20;
        private int _animationDelay = 15;
        private int _targetWidth;
        private DateTime _animationStartTime;
        private int _animationDuration = 300; // milliseconds
        #endregion

        #region Animation Properties
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Category("Animation")]
        [System.ComponentModel.Description("Speed of animation in pixels per step.")]
        [System.ComponentModel.DefaultValue(20)]
        public int AnimationStep
        {
            get => _animationStep;
            set => _animationStep = Math.Max(1, value);
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Category("Animation")]
        [System.ComponentModel.Description("Delay between animation steps in milliseconds.")]
        [System.ComponentModel.DefaultValue(15)]
        public int AnimationDelay
        {
            get => _animationDelay;
            set
            {
                _animationDelay = Math.Max(1, value);
                if (_animationTimer != null)
                {
                    _animationTimer.Interval = _animationDelay;
                }
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Category("Animation")]
        [System.ComponentModel.Description("Total duration of animation in milliseconds.")]
        [System.ComponentModel.DefaultValue(300)]
        public int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Math.Max(50, value);
        }
        #endregion

        #region Animation Methods
        /// <summary>
        /// Initializes the animation timer
        /// </summary>
        private void InitializeAnimation()
        {
            _animationTimer = new Timer { Interval = _animationDelay };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        /// <summary>
        /// Partial method implementation for collapse state changes
        /// </summary>
        partial void OnIsCollapsedChanging(bool newValue)
        {
            StartAnimation(newValue);
            CollapseStateChanged?.Invoke(newValue);
        }

        /// <summary>
        /// Starts the collapse/expand animation
        /// </summary>
        private void StartAnimation(bool collapsing)
        {
            if (_animationTimer == null)
            {
                InitializeAnimation();
            }

            _targetWidth = collapsing ? _collapsedWidth : _expandedWidth;
            _animationStartTime = DateTime.Now;
            _isAnimating = true;
            _animationTimer.Start();
        }

        /// <summary>
        /// Animation timer tick handler
        /// </summary>
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!_isAnimating)
            {
                _animationTimer?.Stop();
                return;
            }

            int currentWidth = Width;
            double elapsed = (DateTime.Now - _animationStartTime).TotalMilliseconds;
            double progress = Math.Min(1.0, elapsed / _animationDuration);

            // Ease out cubic for smooth animation
            double easedProgress = 1 - Math.Pow(1 - progress, 3);

            int startWidth = _isCollapsed ? _expandedWidth : _collapsedWidth;
            int newWidth = (int)(startWidth + ((_targetWidth - startWidth) * easedProgress));

            if (progress >= 1.0 || Math.Abs(newWidth - _targetWidth) < 2)
            {
                // Animation complete
                Width = _targetWidth;
                _isAnimating = false;
                _animationTimer.Stop();
                RefreshHitAreas();
            }
            else
            {
                Width = newWidth;
            }

            Invalidate();
        }

        /// <summary>
        /// Disposes animation resources
        /// </summary>
        partial void DisposeAnimation()
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                _animationTimer.Tick -= AnimationTimer_Tick;
                _animationTimer.Dispose();
                _animationTimer = null;
            }
        }
        #endregion
    }
}
