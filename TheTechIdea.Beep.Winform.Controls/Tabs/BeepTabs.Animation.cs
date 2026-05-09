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

            using Graphics graphics = CreateGraphics();
            var tabRects = GetCurrentHeaderTabRects(graphics);
            if (selectedIndex >= tabRects.Count)
            {
                return;
            }

            RectangleF selectedRect = tabRects[selectedIndex];
            _underlineTargetRect = new RectangleF(
                selectedRect.X + padding,
                selectedRect.Bottom - thickness,
                Math.Max(0, selectedRect.Width - (padding * 2)),
                thickness);

            if (_underlineCurrentRect == RectangleF.Empty)
            {
                _underlineCurrentRect = _underlineTargetRect;
                Invalidate();
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

            if (_styleTransitionProgress > 0f && _transitionFrom != _transitionTo)
            {
                if (_transitionFrom == TabStyle.Underline || _transitionFrom == TabStyle.Minimal)
                {
                    // PaintersFactory returns pooled brushes — do NOT wrap in using
                    SolidBrush brush = PaintersFactory.GetSolidBrush(Color.FromArgb((int)((1 - _styleTransitionProgress) * 255), _currentTheme?.PrimaryColor ?? Color.Blue));
                    graphics.FillRectangle(brush, _underlineCurrentRect);
                }

                if (_transitionTo == TabStyle.Underline || _transitionTo == TabStyle.Minimal)
                {
                    SolidBrush brush = PaintersFactory.GetSolidBrush(Color.FromArgb((int)(_styleTransitionProgress * 255), _currentTheme?.PrimaryColor ?? Color.Blue));
                    graphics.FillRectangle(brush, _underlineCurrentRect);
                }

                return;
            }

            if ((_tabStyle == TabStyle.Underline || _tabStyle == TabStyle.Minimal) && _underlineCurrentRect != RectangleF.Empty)
            {
                SolidBrush brush = PaintersFactory.GetSolidBrush(_currentTheme?.PrimaryColor ?? Color.Blue);
                graphics.FillRectangle(brush, _underlineCurrentRect);
            }
        }
    }
}