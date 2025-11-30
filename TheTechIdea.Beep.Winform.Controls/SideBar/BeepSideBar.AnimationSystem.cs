using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// Enhanced animation system for BeepSideBar
    /// Provides smooth animations for:
    /// - Selection transitions
    /// - Hover effects
    /// - Accordion expand/collapse
    /// </summary>
    public partial class BeepSideBar
    {
        #region Animation System Fields

        // Master animation timer (single timer for all animations)
        private Timer _masterAnimationTimer;
        private const int ANIMATION_FRAME_RATE = 16; // ~60 FPS

        // Selection animation
        private SimpleItem _previousSelectedItem;
        private float _selectionAnimProgress = 1f;
        private DateTime _selectionAnimStartTime;
        private Rectangle _selectionAnimStartRect;
        private Rectangle _selectionAnimEndRect;
        private bool _selectionAnimating = false;

        // Hover animation
        private SimpleItem _previousHoveredItem;
        private float _hoverAnimProgress = 0f;
        private DateTime _hoverAnimStartTime;
        private bool _hoverAnimating = false;
        private bool _hoverFadingIn = true;

        // Accordion animation state per item
        private Dictionary<SimpleItem, AccordionAnimationState> _accordionAnimStates = new Dictionary<SimpleItem, AccordionAnimationState>();

        // Animation durations (in milliseconds)
        private int _selectionAnimDuration = 200;
        private int _hoverAnimDuration = 150;
        private int _accordionAnimDuration = 250;

        // Enable flags - DISABLED by default to prevent design-time issues
        private bool _enableSelectionAnimation = false;
        private bool _enableHoverAnimation = false;
        private bool _enableAccordionAnimation = false;

        #endregion

        #region Animation State Classes

        private class AccordionAnimationState
        {
            public bool IsAnimating { get; set; }
            public bool IsExpanding { get; set; }
            public float Progress { get; set; }
            public DateTime StartTime { get; set; }
            public int StartHeight { get; set; }
            public int TargetHeight { get; set; }
            public int ChildCount { get; set; }
        }

        #endregion

        #region Animation Properties

        [Browsable(true)]
        [Category("Animation")]
        [Description("Duration of selection transition animation in milliseconds.")]
        [DefaultValue(200)]
        public int SelectionAnimationDuration
        {
            get => _selectionAnimDuration;
            set => _selectionAnimDuration = Math.Max(50, Math.Min(1000, value));
        }

        [Browsable(true)]
        [Category("Animation")]
        [Description("Duration of hover animation in milliseconds.")]
        [DefaultValue(150)]
        public int HoverAnimationDuration
        {
            get => _hoverAnimDuration;
            set => _hoverAnimDuration = Math.Max(50, Math.Min(500, value));
        }

        [Browsable(true)]
        [Category("Animation")]
        [Description("Duration of accordion expand/collapse animation in milliseconds.")]
        [DefaultValue(250)]
        public int AccordionAnimationDuration
        {
            get => _accordionAnimDuration;
            set => _accordionAnimDuration = Math.Max(50, Math.Min(1000, value));
        }

        [Browsable(true)]
        [Category("Animation")]
        [Description("Enable selection transition animation.")]
        [DefaultValue(false)]
        public bool EnableSelectionAnimation
        {
            get => _enableSelectionAnimation;
            set => _enableSelectionAnimation = value;
        }

        [Browsable(true)]
        [Category("Animation")]
        [Description("Enable hover animation.")]
        [DefaultValue(false)]
        public bool EnableHoverAnimation
        {
            get => _enableHoverAnimation;
            set => _enableHoverAnimation = value;
        }

        [Browsable(true)]
        [Category("Animation")]
        [Description("Enable accordion expand/collapse animation.")]
        [DefaultValue(false)]
        public bool EnableAccordionAnimation
        {
            get => _enableAccordionAnimation;
            set => _enableAccordionAnimation = value;
        }

        #endregion

        #region Animation Initialization

        /// <summary>
        /// Initializes the master animation timer - ONLY called when needed
        /// </summary>
        private void InitializeAnimationSystem()
        {
            // CRITICAL: Never initialize in design mode
            if (DesignMode) return;
            if (_masterAnimationTimer != null) return;

            _masterAnimationTimer = new Timer { Interval = ANIMATION_FRAME_RATE };
            _masterAnimationTimer.Tick += MasterAnimationTimer_Tick;
        }

        /// <summary>
        /// Ensures the animation timer is running when needed.
        /// ONLY called from actual animation start methods, never automatically.
        /// </summary>
        private void EnsureAnimationTimerRunning()
        {
            // CRITICAL: Never run animations in design mode
            if (DesignMode) return;
            if (!IsHandleCreated) return;

            if (_masterAnimationTimer == null)
            {
                InitializeAnimationSystem();
            }

            if (_masterAnimationTimer != null && !_masterAnimationTimer.Enabled)
            {
                _masterAnimationTimer.Start();
            }
        }

        /// <summary>
        /// Stops the animation timer if no animations are active
        /// </summary>
        private void StopAnimationTimerIfIdle()
        {
            if (_masterAnimationTimer == null) return;

            bool anyAnimating = _selectionAnimating || _hoverAnimating;

            // Check accordion animations
            if (_accordionAnimStates != null)
            {
                foreach (var state in _accordionAnimStates.Values)
                {
                    if (state.IsAnimating)
                    {
                        anyAnimating = true;
                        break;
                    }
                }
            }

            if (!anyAnimating)
            {
                _masterAnimationTimer.Stop();
            }
        }

        #endregion

        #region Master Animation Timer

        private void MasterAnimationTimer_Tick(object sender, EventArgs e)
        {
            // Safety check - stop if we're in design mode or disposed
            if (DesignMode || IsDisposed || !IsHandleCreated)
            {
                _masterAnimationTimer?.Stop();
                return;
            }

            bool needsRepaint = false;

            // Update selection animation
            if (_selectionAnimating)
            {
                needsRepaint |= UpdateSelectionAnimation();
            }

            // Update hover animation
            if (_hoverAnimating)
            {
                needsRepaint |= UpdateHoverAnimation();
            }

            // Update accordion animations
            needsRepaint |= UpdateAccordionAnimations();

            // Repaint if any animation updated
            if (needsRepaint)
            {
                Invalidate();
            }

            // Stop timer if no animations active
            StopAnimationTimerIfIdle();
        }

        #endregion

        #region Selection Animation

        /// <summary>
        /// Starts the selection transition animation.
        /// ONLY called from SelectedItem property setter when user selects an item.
        /// </summary>
        internal void StartSelectionAnimation(SimpleItem newItem, SimpleItem oldItem)
        {
            // CRITICAL: Multiple safety checks - animation only starts from user action
            if (DesignMode || !IsHandleCreated || IsDisposed)
            {
                _selectionAnimProgress = 1f;
                _selectionAnimationProgress = 1f;
                return;
            }

            // Check if animation is enabled
            if (!_enableSelectionAnimation)
            {
                _selectionAnimProgress = 1f;
                _selectionAnimationProgress = 1f;
                return;
            }

            // Don't animate if no previous item or same item
            if (oldItem == null || newItem == null || oldItem == newItem)
            {
                _selectionAnimProgress = 1f;
                _selectionAnimationProgress = 1f;
                return;
            }

            _previousSelectedItem = oldItem;
            _selectionAnimStartTime = DateTime.Now;
            _selectionAnimProgress = 0f;
            _selectionAnimating = true;

            // Calculate start and end rectangles for morphing effect
            _selectionAnimStartRect = GetItemRectangle(oldItem);
            _selectionAnimEndRect = GetItemRectangle(newItem);

            EnsureAnimationTimerRunning();
        }

        private bool UpdateSelectionAnimation()
        {
            double elapsed = (DateTime.Now - _selectionAnimStartTime).TotalMilliseconds;
            double progress = Math.Min(1.0, elapsed / _selectionAnimDuration);

            // Ease out cubic for smooth deceleration
            _selectionAnimProgress = (float)EaseOutCubic(progress);
            _selectionAnimationProgress = _selectionAnimProgress;

            if (progress >= 1.0)
            {
                _selectionAnimating = false;
                _selectionAnimProgress = 1f;
                _selectionAnimationProgress = 1f;
                _previousSelectedItem = null;
            }

            return true;
        }

        /// <summary>
        /// Gets the interpolated selection rectangle during animation
        /// </summary>
        public Rectangle GetAnimatedSelectionRect()
        {
            if (!_selectionAnimating || _selectionAnimProgress >= 1f)
            {
                return _selectionAnimEndRect;
            }

            // Interpolate between start and end rectangles
            int x = (int)Lerp(_selectionAnimStartRect.X, _selectionAnimEndRect.X, _selectionAnimProgress);
            int y = (int)Lerp(_selectionAnimStartRect.Y, _selectionAnimEndRect.Y, _selectionAnimProgress);
            int w = (int)Lerp(_selectionAnimStartRect.Width, _selectionAnimEndRect.Width, _selectionAnimProgress);
            int h = (int)Lerp(_selectionAnimStartRect.Height, _selectionAnimEndRect.Height, _selectionAnimProgress);

            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Gets the current selection animation opacity (for fade effect)
        /// </summary>
        public float GetSelectionAnimationOpacity()
        {
            return _selectionAnimProgress;
        }

        #endregion

        #region Hover Animation

        /// <summary>
        /// Starts the hover animation when mouse enters an item.
        /// ONLY called from OnMouseMove/OnMouseLeave when user moves mouse.
        /// </summary>
        internal void StartHoverAnimation(SimpleItem item, bool fadeIn)
        {
            // CRITICAL: Multiple safety checks - animation only starts from user action
            if (DesignMode || !IsHandleCreated || IsDisposed)
            {
                _hoverAnimProgress = fadeIn ? 1f : 0f;
                _hoverAnimationProgress = _hoverAnimProgress;
                return;
            }

            // Check if animation is enabled
            if (!_enableHoverAnimation)
            {
                _hoverAnimProgress = fadeIn ? 1f : 0f;
                _hoverAnimationProgress = _hoverAnimProgress;
                return;
            }

            // Don't animate if no item
            if (item == null)
            {
                _hoverAnimProgress = 0f;
                _hoverAnimationProgress = 0f;
                return;
            }

            _hoverAnimStartTime = DateTime.Now;
            _hoverFadingIn = fadeIn;

            // If already animating in same direction, continue from current progress
            if (_hoverAnimating && _hoverFadingIn == fadeIn)
            {
                return;
            }

            // If reversing direction, start from current progress
            if (_hoverAnimating)
            {
                // Reverse the animation from current position
                _hoverAnimProgress = fadeIn ? (1f - _hoverAnimProgress) : _hoverAnimProgress;
            }
            else
            {
                _hoverAnimProgress = fadeIn ? 0f : 1f;
            }

            _hoverAnimating = true;
            EnsureAnimationTimerRunning();
        }

        private bool UpdateHoverAnimation()
        {
            double elapsed = (DateTime.Now - _hoverAnimStartTime).TotalMilliseconds;
            double progress = Math.Min(1.0, elapsed / _hoverAnimDuration);

            if (_hoverFadingIn)
            {
                // Ease out for fade in (quick start, slow end)
                _hoverAnimProgress = (float)EaseOutQuad(progress);
            }
            else
            {
                // Ease in for fade out (slow start, quick end)
                _hoverAnimProgress = 1f - (float)EaseOutQuad(progress);
            }

            _hoverAnimationProgress = _hoverAnimProgress;

            if (progress >= 1.0)
            {
                _hoverAnimating = false;
                _hoverAnimProgress = _hoverFadingIn ? 1f : 0f;
                _hoverAnimationProgress = _hoverAnimProgress;
            }

            return true;
        }

        /// <summary>
        /// Gets the current hover animation progress (0-1)
        /// </summary>
        public float GetHoverAnimationProgress()
        {
            return _hoverAnimProgress;
        }

        #endregion

        #region Accordion Animation

        /// <summary>
        /// Starts the accordion expand/collapse animation for an item.
        /// ONLY called from ToggleItemExpansion/ExpandItem/CollapseItem when user clicks.
        /// </summary>
        internal void StartAccordionAnimation(SimpleItem item, bool expanding)
        {
            // CRITICAL: Multiple safety checks - animation only starts from user action
            if (DesignMode || !IsHandleCreated || IsDisposed)
            {
                // No animation - set final state immediately
                if (item != null && _accordionAnimationProgress != null)
                {
                    _accordionAnimationProgress[item] = expanding ? 1f : 0f;
                }
                return;
            }

            // Check if item is valid
            if (item == null)
            {
                return;
            }

            // Check if animation is enabled
            if (!_enableAccordionAnimation)
            {
                // No animation - set final state immediately
                _accordionAnimationProgress[item] = expanding ? 1f : 0f;
                return;
            }

            int childCount = item.Children?.Count ?? 0;
            if (childCount == 0) return;

            // Calculate target height based on children
            int targetHeight = childCount * (_childItemHeight + 2);

            // Get or create animation state
            if (!_accordionAnimStates.TryGetValue(item, out var state))
            {
                state = new AccordionAnimationState();
                _accordionAnimStates[item] = state;
            }

            // If already animating, continue from current position
            float currentProgress = _accordionAnimationProgress.ContainsKey(item) ? _accordionAnimationProgress[item] : (expanding ? 0f : 1f);

            state.IsAnimating = true;
            state.IsExpanding = expanding;
            state.StartTime = DateTime.Now;
            state.ChildCount = childCount;
            state.StartHeight = (int)(targetHeight * currentProgress);
            state.TargetHeight = expanding ? targetHeight : 0;
            state.Progress = currentProgress;

            // Initialize progress tracking
            _accordionAnimationProgress[item] = currentProgress;

            EnsureAnimationTimerRunning();
        }

        private bool UpdateAccordionAnimations()
        {
            bool anyUpdated = false;
            var itemsToRemove = new List<SimpleItem>();

            foreach (var kvp in _accordionAnimStates)
            {
                var item = kvp.Key;
                var state = kvp.Value;

                if (!state.IsAnimating) continue;

                double elapsed = (DateTime.Now - state.StartTime).TotalMilliseconds;
                double progress = Math.Min(1.0, elapsed / _accordionAnimDuration);

                // Use ease out cubic for smooth animation
                float easedProgress = (float)EaseOutCubic(progress);

                if (state.IsExpanding)
                {
                    state.Progress = easedProgress;
                }
                else
                {
                    state.Progress = 1f - easedProgress;
                }

                // Update the progress dictionary
                _accordionAnimationProgress[item] = state.Progress;

                anyUpdated = true;

                if (progress >= 1.0)
                {
                    state.IsAnimating = false;
                    state.Progress = state.IsExpanding ? 1f : 0f;
                    _accordionAnimationProgress[item] = state.Progress;

                    // Clean up if collapsed
                    if (!state.IsExpanding)
                    {
                        itemsToRemove.Add(item);
                    }
                }
            }

            // Remove completed collapse animations
            foreach (var item in itemsToRemove)
            {
                _accordionAnimStates.Remove(item);
            }

            return anyUpdated;
        }

        /// <summary>
        /// Gets the accordion animation progress for an item (0-1)
        /// </summary>
        public float GetAccordionAnimationProgress(SimpleItem item)
        {
            if (item == null) return 1f;

            if (_accordionAnimationProgress.TryGetValue(item, out float progress))
            {
                return progress;
            }

            // Default: expanded = 1, collapsed = 0
            if (_expandedState.TryGetValue(item, out bool isExpanded))
            {
                return isExpanded ? 1f : 0f;
            }

            return 0f;
        }

        /// <summary>
        /// Gets the animated height for accordion children
        /// </summary>
        public int GetAnimatedAccordionHeight(SimpleItem item)
        {
            if (item?.Children == null || item.Children.Count == 0) return 0;

            float progress = GetAccordionAnimationProgress(item);
            int fullHeight = item.Children.Count * (_childItemHeight + 2);

            return (int)(fullHeight * progress);
        }

        /// <summary>
        /// Checks if an item's accordion is currently animating
        /// </summary>
        public bool IsAccordionAnimating(SimpleItem item)
        {
            if (item == null) return false;

            if (_accordionAnimStates.TryGetValue(item, out var state))
            {
                return state.IsAnimating;
            }

            return false;
        }

        #endregion

        #region Easing Functions

        /// <summary>
        /// Ease out cubic: decelerating to zero velocity
        /// </summary>
        private static double EaseOutCubic(double t)
        {
            return 1 - Math.Pow(1 - t, 3);
        }

        /// <summary>
        /// Ease out quadratic: decelerating to zero velocity
        /// </summary>
        private static double EaseOutQuad(double t)
        {
            return 1 - (1 - t) * (1 - t);
        }

        /// <summary>
        /// Ease in out cubic: acceleration until halfway, then deceleration
        /// </summary>
        private static double EaseInOutCubic(double t)
        {
            return t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        /// <summary>
        /// Ease out elastic: exponentially decaying sine wave
        /// </summary>
        private static double EaseOutElastic(double t)
        {
            const double c4 = (2 * Math.PI) / 3;
            return t == 0 ? 0 : t == 1 ? 1 : Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1;
        }

        /// <summary>
        /// Ease out back: overshooting cubic easing
        /// </summary>
        private static double EaseOutBack(double t)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1;
            return 1 + c3 * Math.Pow(t - 1, 3) + c1 * Math.Pow(t - 1, 2);
        }

        /// <summary>
        /// Linear interpolation
        /// </summary>
        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the rectangle for a menu item (for animation calculations)
        /// </summary>
        private Rectangle GetItemRectangle(SimpleItem item)
        {
            if (item == null || _items == null)
            {
                return Rectangle.Empty;
            }

            int padding = 16;
            int currentY = DrawingRect.Top + padding;

            // Skip toggle button
            if (_showToggleButton)
            {
                currentY += _itemHeight + 12;
            }

            // Find the item in the hierarchy
            return FindItemRectangle(item, _items, ref currentY, 0);
        }

        private Rectangle FindItemRectangle(SimpleItem target, System.Collections.Generic.IEnumerable<SimpleItem> items, ref int currentY, int indentLevel)
        {
            int padding = indentLevel == 0 ? 16 : 8;
            int indent = _indentationWidth * indentLevel;
            int itemHeight = indentLevel == 0 ? _itemHeight : _childItemHeight;

            foreach (var item in items)
            {
                Rectangle itemRect = new Rectangle(
                    DrawingRect.Left + indent + padding,
                    currentY,
                    DrawingRect.Width - indent - padding * 2,
                    itemHeight);

                if (item == target)
                {
                    return itemRect;
                }

                currentY += itemHeight + (indentLevel == 0 ? 4 : 2);

                // Check children if expanded
                if (item.Children != null && item.Children.Count > 0 &&
                    _expandedState.TryGetValue(item, out bool isExpanded) && isExpanded)
                {
                    var childItems = new List<SimpleItem>();
                    foreach (var child in item.Children)
                    {
                        if (child is SimpleItem si)
                            childItems.Add(si);
                    }

                    var result = FindItemRectangle(target, childItems, ref currentY, indentLevel + 1);
                    if (!result.IsEmpty)
                    {
                        return result;
                    }
                }
            }

            return Rectangle.Empty;
        }

        #endregion

        #region Animation System Cleanup

        /// <summary>
        /// Disposes animation system resources
        /// </summary>
        private void DisposeAnimationSystem()
        {
            if (_masterAnimationTimer != null)
            {
                _masterAnimationTimer.Stop();
                _masterAnimationTimer.Tick -= MasterAnimationTimer_Tick;
                _masterAnimationTimer.Dispose();
                _masterAnimationTimer = null;
            }

            _accordionAnimStates?.Clear();
        }

        #endregion
    }
}

