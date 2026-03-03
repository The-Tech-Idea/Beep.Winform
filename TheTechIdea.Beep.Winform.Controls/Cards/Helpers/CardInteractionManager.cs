using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal sealed class CardInteractionManager : IDisposable
    {
        private readonly Control _owner;
        private readonly Action _requestRepaint;
        private readonly Timer _timer;

        private bool _hovered;
        private bool _pressed;
        private bool _focused;
        private bool _rippleActive;

        public float HoverProgress { get; private set; }
        public float PressProgress { get; private set; }
        public Point RippleCenter { get; private set; }
        public float RippleRadius { get; private set; }
        public int RippleAlpha { get; private set; }

        public CardInteractionManager(Control owner, Action requestRepaint)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _requestRepaint = requestRepaint ?? throw new ArgumentNullException(nameof(requestRepaint));
            _timer = new Timer { Interval = 16 };
            _timer.Tick += OnTimerTick;
        }

        public void NotifyMouseEnter()
        {
            _hovered = true;
            StartTimer();
        }

        public void NotifyMouseLeave()
        {
            _hovered = false;
            _pressed = false;
            StartTimer();
        }

        public void NotifyMouseDown(MouseButtons button, Point location)
        {
            if (button != MouseButtons.Left)
            {
                return;
            }

            _pressed = true;
            StartRipple(location);
            StartTimer();
        }

        public void NotifyMouseUp(MouseButtons button, Point location)
        {
            if (button != MouseButtons.Left)
            {
                return;
            }

            _pressed = false;
            RippleCenter = location;
            StartTimer();
        }

        public void NotifyMouseMove(Point location)
        {
            if (_pressed)
            {
                RippleCenter = location;
                _requestRepaint();
            }
        }

        public void NotifyFocusChanged(bool focused)
        {
            _focused = focused;
            StartTimer();
        }

        private void StartRipple(Point center)
        {
            RippleCenter = center;
            RippleRadius = 0f;
            RippleAlpha = 96;
            _rippleActive = true;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            bool changed = false;

            float hoverTarget = (_hovered || _focused) ? 1f : 0f;
            float pressTarget = _pressed ? 1f : 0f;

            float newHover = Lerp(HoverProgress, hoverTarget, 0.2f);
            float newPress = Lerp(PressProgress, pressTarget, 0.28f);

            if (Math.Abs(newHover - HoverProgress) > 0.001f)
            {
                HoverProgress = newHover;
                changed = true;
            }

            if (Math.Abs(newPress - PressProgress) > 0.001f)
            {
                PressProgress = newPress;
                changed = true;
            }

            if (_rippleActive)
            {
                float rippleStep = DpiScalingHelper.ScaleValue(14f, _owner);
                RippleRadius += rippleStep;
                RippleAlpha = Math.Max(0, RippleAlpha - 9);
                changed = true;

                if (RippleAlpha <= 0)
                {
                    _rippleActive = false;
                    RippleRadius = 0f;
                }
            }

            if (changed)
            {
                _requestRepaint();
            }

            bool idle = Math.Abs(HoverProgress - hoverTarget) < 0.01f &&
                        Math.Abs(PressProgress - pressTarget) < 0.01f &&
                        !_rippleActive;
            if (idle)
            {
                _timer.Stop();
            }
        }

        private static float Lerp(float current, float target, float speed)
        {
            return current + ((target - current) * speed);
        }

        private void StartTimer()
        {
            if (!_timer.Enabled)
            {
                _timer.Start();
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer.Dispose();
        }
    }
}
