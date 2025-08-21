using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls.Advanced.Helpers
{
    internal class ControlEffectHelper
    {
        private readonly BeepControlAdvanced _owner;
        private Timer _animationTimer;
        private Timer _rippleTimer;
        private float _opacity = 0f;
        private int _animationElapsedTime;
        private Rectangle _slideStartRect;
        private Rectangle _slideEndRect;
        private bool _isAnimating;

        // Ripple effect properties
        private Point _rippleCenter;
        private float _rippleSize = 0;
        private bool _showRipple = false;
        private float _rippleOpacity = 1.0f;
        private bool _showMaterialRipple = false;
        private Point _rippleOrigin = Point.Empty;
        private float _rippleRadius = 0;

        public ControlEffectHelper(BeepControlAdvanced owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Effect Properties
        [Browsable(true)] public bool ShowFocusIndicator { get; set; } = false;
        [Browsable(true)] public Color FocusIndicatorColor { get; set; } = Color.RoyalBlue;
        [Browsable(true)] public bool EnableRippleEffect { get; set; } = false;

        // Animation Properties
        [Browsable(true)] public DisplayAnimationType AnimationType { get; set; } = DisplayAnimationType.None;
        [Browsable(true)] public int AnimationDuration { get; set; } = 500;
        [Browsable(true)] public EasingType Easing { get; set; } = EasingType.Linear;
        [Browsable(true)] public SlideDirection SlideFrom { get; set; } = SlideDirection.Left;
        #endregion

        public void DrawOverlays(Graphics g)
        {
            // Focus indicator
            if (ShowFocusIndicator && _owner.Focused)
            {
                DrawFocusIndicator(g);
            }

            // Ripple effects
            if (_showRipple && EnableRippleEffect)
            {
                DrawRippleEffect(g);
            }

            if (_showMaterialRipple && EnableRippleEffect)
            {
                DrawMaterialRipple(g);
            }
        }

        #region Focus Indicator
        private void DrawFocusIndicator(Graphics g)
        {
            // Glow effect
            var glowRect = new Rectangle(-3, -3, _owner.Width + 6, _owner.Height + 6);
            using (var glowBrush = new SolidBrush(Color.FromArgb(128, FocusIndicatorColor)))
            {
                if (_owner is BeepControlAdvanced ownerAdv && ownerAdv.IsRounded)
                {
                    using (var path = ControlPaintHelper.GetRoundedRectPath(glowRect, 10)) // Slightly larger radius
                    {
                        g.FillPath(glowBrush, path);
                    }
                }
                else
                {
                    g.FillRectangle(glowBrush, glowRect);
                }
            }

            // Alternative: Color overlay
            using (var overlayBrush = new SolidBrush(Color.FromArgb(50, FocusIndicatorColor)))
            {
                g.FillRectangle(overlayBrush, _owner.ClientRectangle);
            }
        }
        #endregion

        #region Ripple Effects
        public void StartRippleEffect(Point center)
        {
            if (!EnableRippleEffect) return;

            _rippleCenter = center;
            _rippleSize = 0;
            _showRipple = true;
            _rippleOpacity = 1.0f;

            if (_rippleTimer == null)
            {
                _rippleTimer = new Timer();
                _rippleTimer.Interval = 20; // 50fps
                _rippleTimer.Tick += OnRippleTick;
            }

            _rippleTimer.Start();
        }

        private void OnRippleTick(object sender, EventArgs e)
        {
            _rippleSize += _owner.Width / 20.0f;
            _rippleOpacity -= 0.05f;

            if (_rippleOpacity <= 0 || _rippleSize > Math.Max(_owner.Width, _owner.Height) * 2)
            {
                _showRipple = false;
                _rippleTimer.Stop();
            }

            _owner.Invalidate();
        }

        private void DrawRippleEffect(Graphics g)
        {
            if (!_showRipple) return;

            using (var brush = new SolidBrush(Color.FromArgb(
                (int)(_rippleOpacity * 64),
                _owner.ForeColor)))
            {
                float diameter = _rippleSize * 2;
                g.FillEllipse(brush,
                    _rippleCenter.X - _rippleSize,
                    _rippleCenter.Y - _rippleSize,
                    diameter,
                    diameter);
            }
        }

        public void StartMaterialRipple(Point clickPosition)
        {
            if (!EnableRippleEffect) return;

            _showMaterialRipple = true;
            _rippleOrigin = clickPosition;
            _rippleRadius = 0;
            _rippleOpacity = 0.5f;

            if (_rippleTimer == null)
            {
                _rippleTimer = new Timer();
                _rippleTimer.Interval = 20;
                _rippleTimer.Tick += OnMaterialRippleTick;
            }

            _rippleTimer.Start();
        }

        private void OnMaterialRippleTick(object sender, EventArgs e)
        {
            _rippleRadius += Math.Max(_owner.Width, _owner.Height) / 10f;
            _rippleOpacity -= 0.05f;

            if (_rippleOpacity <= 0)
            {
                _rippleTimer.Stop();
                _showMaterialRipple = false;
            }

            _owner.Invalidate();
        }

        private void DrawMaterialRipple(Graphics g)
        {
            if (!_showMaterialRipple) return;

            using (var rippleBrush = new SolidBrush(Color.FromArgb(
                (int)(_rippleOpacity * 64),
                _owner.Focused ? Color.RoyalBlue : _owner.ForeColor)))
            {
                float diameter = _rippleRadius * 2;
                g.FillEllipse(rippleBrush,
                    _rippleOrigin.X - _rippleRadius,
                    _rippleOrigin.Y - _rippleRadius,
                    diameter,
                    diameter);
            }
        }
        #endregion

        #region Animations
        public void ShowWithAnimation(DisplayAnimationType animationType, Control parentControl = null)
        {
            AnimationType = animationType;
            if (AnimationType == DisplayAnimationType.None)
            {
                _owner.Visible = true;
                return;
            }

            if (!_owner.Visible)
            {
                _owner.Visible = true;
            }

            InitializeAnimation(parentControl);
        }

        private void InitializeAnimation(Control parentControl)
        {
            _animationTimer?.Stop();
            _owner.Visible = true;
            _opacity = 0f;
            _animationElapsedTime = 0;

            _slideStartRect = GetSlideStartRect(parentControl);
            _slideEndRect = _owner.Bounds;

            _animationTimer = new Timer { Interval = 15 }; // ~60 FPS
            _animationTimer.Tick += OnAnimationTick;
            _isAnimating = true;

            if (AnimationType == DisplayAnimationType.Fade || AnimationType == DisplayAnimationType.SlideAndFade)
            {
                _owner.BackColor = Color.FromArgb((int)(_opacity * 255), _owner.BackColor);
            }

            if (AnimationType == DisplayAnimationType.Slide || AnimationType == DisplayAnimationType.SlideAndFade)
            {
                _owner.Bounds = _slideStartRect;
            }

            _animationTimer.Start();
        }

        private void OnAnimationTick(object sender, EventArgs e)
        {
            _animationElapsedTime += _animationTimer.Interval;
            float progress = Math.Min(1.0f, (float)_animationElapsedTime / AnimationDuration);
            progress = ApplyEasing(progress);

            // Fade effect
            if (AnimationType == DisplayAnimationType.Fade || AnimationType == DisplayAnimationType.SlideAndFade)
            {
                _opacity = progress;
                _owner.BackColor = Color.FromArgb((int)(_opacity * 255), _owner.BackColor);
            }

            // Slide effect
            if (AnimationType == DisplayAnimationType.Slide || AnimationType == DisplayAnimationType.SlideAndFade)
            {
                int x = (int)(_slideStartRect.X + (_slideEndRect.X - _slideStartRect.X) * progress);
                int y = (int)(_slideStartRect.Y + (_slideEndRect.Y - _slideStartRect.Y) * progress);
                int width = (int)(_slideStartRect.Width + (_slideEndRect.Width - _slideStartRect.Width) * progress);
                int height = (int)(_slideStartRect.Height + (_slideEndRect.Height - _slideStartRect.Height) * progress);
                _owner.Bounds = new Rectangle(x, y, width, height);
            }

            if (progress >= 1.0f)
            {
                _animationTimer.Stop();
                _isAnimating = false;
            }
        }

        private Rectangle GetSlideStartRect(Control parentControl)
        {
            var startRect = _owner.Bounds;

            if (parentControl != null)
            {
                var parentLocation = parentControl.PointToScreen(Point.Empty);

                switch (SlideFrom)
                {
                    case SlideDirection.Bottom:
                        startRect = new Rectangle(parentLocation.X, parentLocation.Y + parentControl.Height, _owner.Width, _owner.Height);
                        break;
                    case SlideDirection.Top:
                        startRect = new Rectangle(parentLocation.X, parentLocation.Y - _owner.Height, _owner.Width, _owner.Height);
                        break;
                    case SlideDirection.Left:
                        startRect = new Rectangle(parentLocation.X - _owner.Width, parentLocation.Y, _owner.Width, _owner.Height);
                        break;
                    case SlideDirection.Right:
                        startRect = new Rectangle(parentLocation.X + parentControl.Width, parentLocation.Y, _owner.Width, _owner.Height);
                        break;
                }
            }

            return startRect;
        }

        public void ShowWithDropdownAnimation(Control parentControl = null)
        {
            if (AnimationType == DisplayAnimationType.None)
            {
                _owner.Visible = true;
                return;
            }

            _animationTimer = new Timer { Interval = 15 };
            int elapsedTime = 0;
            float startOpacity = 0f;
            float endOpacity = 1f;

            int startX = _owner.Location.X, startY = _owner.Location.Y;
            int finalX = _owner.Location.X, finalY = _owner.Location.Y;

            if (parentControl != null)
            {
                switch (SlideFrom)
                {
                    case SlideDirection.Bottom:
                        startX = parentControl.Left;
                        startY = parentControl.Bottom;
                        finalY = startY + _owner.Height;
                        break;
                    case SlideDirection.Top:
                        startX = parentControl.Left;
                        startY = parentControl.Top - _owner.Height;
                        finalY = parentControl.Top - _owner.Height;
                        break;
                    case SlideDirection.Left:
                        startX = parentControl.Left - _owner.Width;
                        finalX = parentControl.Left - _owner.Width;
                        startY = parentControl.Top;
                        finalY = startY;
                        break;
                    case SlideDirection.Right:
                        startX = parentControl.Right;
                        finalX = startX + _owner.Width;
                        startY = parentControl.Top;
                        finalY = startY;
                        break;
                }
            }

            _owner.Location = new Point(startX, startY);
            _owner.Visible = true;

            _animationTimer.Tick += (sender, args) =>
            {
                elapsedTime += _animationTimer.Interval;
                float progress = Math.Min(1.0f, (float)elapsedTime / AnimationDuration);
                progress = ApplyEasing(progress);

                if (AnimationType == DisplayAnimationType.Fade || AnimationType == DisplayAnimationType.SlideAndFade)
                {
                    _opacity = startOpacity + (endOpacity - startOpacity) * progress;
                    _owner.BackColor = Color.FromArgb((int)(_opacity * 255), _owner.BackColor);
                }

                if (AnimationType == DisplayAnimationType.Slide || AnimationType == DisplayAnimationType.SlideAndFade)
                {
                    _owner.Location = new Point(
                        (int)(startX + (finalX - startX) * progress),
                        (int)(startY + (finalY - startY) * progress));
                }

                if (progress >= 1.0f)
                {
                    _animationTimer.Stop();
                    _animationTimer.Dispose();
                }
            };

            _animationTimer.Start();
        }

        private float ApplyEasing(float progress)
        {
            return Easing switch
            {
                EasingType.Linear => progress,
                EasingType.EaseIn => progress * progress,
                EasingType.EaseOut => 1 - (1 - progress) * (1 - progress),
                EasingType.EaseInOut => progress < 0.5f ? 2 * progress * progress : 1 - 2 * (1 - progress) * (1 - progress),
                _ => progress
            };
        }

        public void StopAnimation()
        {
            _animationTimer?.Stop();
            _isAnimating = false;
        }
        #endregion

        #region Cleanup
        public void Dispose()
        {
            _animationTimer?.Stop();
            _animationTimer?.Dispose();
            _rippleTimer?.Stop();
            _rippleTimer?.Dispose();
        }
        #endregion
    }
}
