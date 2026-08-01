using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private RectangleF _underlineCurrentRect = RectangleF.Empty;
        private RectangleF _underlineStartRect = RectangleF.Empty;
        private RectangleF _underlineTargetRect = RectangleF.Empty;
        private Timer _underlineTimer;
        private int _underlineElapsed;
        private int _underlineDuration = 220;
        private TabStyle _transitionFrom = TabStyle.Classic;
        private TabStyle _transitionTo = TabStyle.Classic;
        private float _styleTransitionProgress;
        private Timer _styleTransitionTimer;
        private int _styleTransitionElapsed;
        private int _styleTransitionDuration = 220;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Duration in milliseconds for tab animations (underline, style transitions)")]
        [DefaultValue(220)]
        public int TabAnimationDuration
        {
            get => _styleTransitionDuration;
            set
            {
                _styleTransitionDuration = Math.Max(50, value);
                _underlineDuration = _styleTransitionDuration;
            }
        }

        private void InitializeStyleTransitionTimer()
        {
            _styleTransitionTimer = new Timer { Interval = 16 };
            _styleTransitionTimer.Tick += (sender, e) =>
            {
                _styleTransitionElapsed += _styleTransitionTimer.Interval;
                float progress = Math.Min(1f, (float)_styleTransitionElapsed / _styleTransitionDuration);
                string easing = _currentTheme?.AnimationEasingFunction;
                _styleTransitionProgress = AnimationEasingHelper.Evaluate(easing, progress);
                Invalidate();
                if (progress >= 1f)
                {
                    _styleTransitionTimer.Stop();

                    // Clear the transition state, not just the timer. Leaving progress at 1f with
                    // _transitionFrom != _transitionTo left the control permanently "mid-transition":
                    //   * BeepTabHeaderRenderRequest.HasTransition stayed true forever, so every tab
                    //     was painted twice by two painters on every paint — one of those passes at
                    //     alpha 0, doing all of its GDI work to produce nothing — and PrimaryPainter
                    //     was never used again;
                    //   * DrawHeaderSelectionIndicator took its transition branch forever, so its
                    //     settled-state code below was unreachable and the Minimal style kept
                    //     drawing the Underline accent bar, making the two styles pixel-identical.
                    _styleTransitionProgress = 0f;
                    _transitionFrom = _transitionTo;
                    Invalidate();
                }
            };
        }

        private void InitializeUnderlineTimer()
        {
            _underlineTimer = new Timer { Interval = 16 };
            _underlineTimer.Tick += (sender, e) =>
            {
                _underlineElapsed += _underlineTimer.Interval;
                float progress = Math.Min(1f, (float)_underlineElapsed / _underlineDuration);
                string easing = _currentTheme?.AnimationEasingFunction;
                progress = AnimationEasingHelper.Evaluate(easing, progress);
                _underlineCurrentRect = LerpRect(_underlineStartRect, _underlineTargetRect, progress);
                if (progress >= 1f)
                {
                    _underlineTimer.Stop();
                }

                Invalidate();
            };
        }

        private void BeepTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartUnderlineAnimation();
        }

        private void StartUnderlineAnimation()
        {
            int itemCount = GetHostedSourceItemCount();
            int selectedIndex = GetHostedSourceSelectedIndex();
            if (itemCount == 0 || selectedIndex < 0 || HeaderPosition == TabHeaderPosition.Left || HeaderPosition == TabHeaderPosition.Right)
            {
                return;
            }

            if (!IsHandleCreated)
            {
                return;
            }

            int padding = DpiScalingHelper.ScaleValue(6, this);
            int thickness = DpiScalingHelper.ScaleValue(3, this);

            // Use cached tab rectangles to avoid expensive CreateGraphics() + layout recalculation
            RectangleF selectedRect;
            if (selectedIndex < _cachedHeaderTabRects.Count)
            {
                selectedRect = _cachedHeaderTabRects[selectedIndex];
            }
            else
            {
                // Fallback: recalculate if cache is stale
                using Graphics graphics = CreateGraphics();
                var tabRects = GetCurrentHeaderTabRects(graphics);
                if (selectedIndex >= tabRects.Count)
                {
                    return;
                }
                selectedRect = tabRects[selectedIndex];
            }

            _underlineTargetRect = new RectangleF(
                selectedRect.X + padding,
                selectedRect.Bottom - thickness,
                Math.Max(0, selectedRect.Width - (padding * 2)),
                thickness);

            if (_underlineCurrentRect == RectangleF.Empty)
            {
                _underlineCurrentRect = _underlineTargetRect;
                InvalidateHeader();
                return;
            }

            _underlineStartRect = _underlineCurrentRect;
            _underlineElapsed = 0;
            _underlineTimer?.Start();
        }

        private void StartStyleTransition(TabStyle from, TabStyle to)
        {
            _transitionFrom = from;
            _transitionTo = to;
            _styleTransitionElapsed = 0;
            _styleTransitionProgress = 0f;
            _styleTransitionTimer?.Start();
        }

        private RectangleF LerpRect(RectangleF start, RectangleF end, float progress)
        {
            return new RectangleF(
                start.X + (end.X - start.X) * progress,
                start.Y + (end.Y - start.Y) * progress,
                start.Width + (end.Width - start.Width) * progress,
                start.Height + (end.Height - start.Height) * progress);
        }

        private void DrawHeaderSelectionIndicator(Graphics graphics)
        {
            if (HeaderPosition != TabHeaderPosition.Top && HeaderPosition != TabHeaderPosition.Bottom)
            {
                return;
            }

            // Nothing to draw when there are no tabs — avoids crashes on style change with empty control
            if (GetHostedSourceItemCount() == 0)
            {
                return;
            }

            // Each painter decides whether its style has a selection accent and what it looks like.
            // This used to be `_tabStyle == TabStyle.Underline || _tabStyle == TabStyle.Minimal`
            // here in the control — a switch on style outside the painters, which is what painters
            // exist to avoid, and which made Minimal draw Underline's accent.
            if (_styleTransitionProgress > 0f && _transitionFrom != _transitionTo)
            {
                AccentPainter(_transitionFrom).PaintSelectionAccent(
                    graphics, _underlineCurrentRect, 1f - _styleTransitionProgress);
                AccentPainter(_transitionTo).PaintSelectionAccent(
                    graphics, _underlineCurrentRect, _styleTransitionProgress);
                return;
            }

            AccentPainter(_tabStyle).PaintSelectionAccent(graphics, _underlineCurrentRect);
        }

        /// <summary>
        /// Resolves a painter with its theme applied. <see cref="GetPainter"/> alone can hand back an
        /// instance whose <c>Theme</c> was never set, which would silently fall back to the built-in
        /// default accent colour instead of the active theme's.
        /// </summary>
        private Tabs.Painters.ITabPainter AccentPainter(TabStyle style)
        {
            Tabs.Painters.ITabPainter painter = (style == _tabStyle ? _painter : null) ?? GetPainter(style);
            painter.Theme = _currentTheme;
            return painter;
        }
    }
}