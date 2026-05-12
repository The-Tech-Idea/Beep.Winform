using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Provides animation support for BeepTree including expand/collapse
    /// transitions, selection fades, and scroll animations.
    /// </summary>
    public class BeepTreeAnimationHelper : IDisposable
    {
        private readonly BeepTree _owner;
        private Timer _animationTimer;
        private float _animationProgress;
        private float _animationDuration = 200; // ms
        private DateTime _animationStartTime;
        private AnimationType _currentAnimation;
        private Rectangle _animationBounds;
        private float _startValue;
        private float _targetValue;

        public bool IsAnimating => _animationTimer?.Enabled ?? false;

        public BeepTreeAnimationHelper(BeepTree owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _animationTimer = new Timer { Interval = 16 }; // ~60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        /// <summary>
        /// Animates the expand/collapse of a node.
        /// </summary>
        public void AnimateExpandCollapse(Rectangle bounds, bool expanding)
        {
            _currentAnimation = AnimationType.ExpandCollapse;
            _animationBounds = bounds;
            _startValue = expanding ? 0 : 1;
            _targetValue = expanding ? 1 : 0;
            _animationStartTime = DateTime.Now;
            _animationProgress = 0;
            _animationTimer.Start();
        }

        /// <summary>
        /// Animates the selection of a node with a fade effect.
        /// </summary>
        public void AnimateSelection(Rectangle bounds)
        {
            _currentAnimation = AnimationType.Selection;
            _animationBounds = bounds;
            _startValue = 0;
            _targetValue = 1;
            _animationStartTime = DateTime.Now;
            _animationProgress = 0;
            _animationTimer.Start();
        }

        /// <summary>
        /// Animates a scroll to a specific position.
        /// </summary>
        public void AnimateScroll(int fromY, int toY)
        {
            _currentAnimation = AnimationType.Scroll;
            _startValue = fromY;
            _targetValue = toY;
            _animationStartTime = DateTime.Now;
            _animationProgress = 0;
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - _animationStartTime).TotalMilliseconds;
            _animationProgress = Math.Min(1f, (float)(elapsed / _animationDuration));

            // Ease out cubic
            float easedProgress = 1 - (float)Math.Pow(1 - _animationProgress, 3);

            switch (_currentAnimation)
            {
                case AnimationType.ExpandCollapse:
                    // During expand/collapse animation, we just invalidate the tree
                    // The actual animation is handled by the painter
                    _owner.Invalidate(_animationBounds);
                    break;

                case AnimationType.Selection:
                    _owner.Invalidate(_animationBounds);
                    break;

                case AnimationType.Scroll:
                    int currentY = (int)(_startValue + (_targetValue - _startValue) * easedProgress);
                    _owner.ScrollBy(0, currentY - _owner.YOffset);
                    break;
            }

            if (_animationProgress >= 1f)
            {
                _animationTimer.Stop();
            }
        }

        /// <summary>
        /// Gets the current animation progress (0-1).
        /// </summary>
        public float GetAnimationProgress()
        {
            return _animationProgress;
        }

        /// <summary>
        /// Gets the current animation type.
        /// </summary>
        public AnimationType CurrentAnimation => _currentAnimation;

        /// <summary>
        /// Stops any active animation.
        /// </summary>
        public void StopAnimation()
        {
            _animationTimer?.Stop();
            _animationProgress = 1f;
        }

        public void Dispose()
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                _animationTimer.Dispose();
                _animationTimer = null;
            }
        }
    }

    /// <summary>
    /// Defines the types of animations available.
    /// </summary>
    public enum AnimationType
    {
        None,
        ExpandCollapse,
        Selection,
        Scroll
    }
}
