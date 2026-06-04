namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepNotificationBadge : BeepCounterBadge
    {
        private bool _pulseActive;
        private Timer? _pulseTimer;
        private float _pulseScale = 1f;
        private int _pulseDirection = 1;

        public BeepNotificationBadge() : this("") { }

        public BeepNotificationBadge(string text) : base(text)
        {
            Shape = BadgeShape.Circle;
            BadgeBackColor = Color.Red;
            ShowDropShadow = true;
            PulseEnabled = true;
        }

        public bool PulseEnabled
        {
            get => _pulseActive;
            set
            {
                _pulseActive = value;
                if (_pulseActive)
                    StartPulse();
                else
                    StopPulse();
            }
        }

        public new BeepNotificationBadge SetText(string text)
        {
            DisplayText = text;
            return this;
        }

        public new BeepNotificationBadge At(float fractionX, float fractionY)
        {
            Location = BadgeLocations.Relative(fractionX, fractionY);
            return this;
        }

        private void StartPulse()
        {
            StopPulse();
            _pulseTimer = new Timer();
            _pulseTimer.Interval = 40;
            _pulseTimer.Tick += OnPulseTick;
            _pulseTimer.Start();
        }

        private void StopPulse()
        {
            if (_pulseTimer is not null)
            {
                _pulseTimer.Stop();
                _pulseTimer.Tick -= OnPulseTick;
                _pulseTimer.Dispose();
                _pulseTimer = null;
            }
            _pulseScale = 1f;
        }

        private void OnPulseTick(object? sender, EventArgs e)
        {
            _pulseScale += _pulseDirection * 0.04f;

            if (_pulseScale >= 1.2f)
            {
                _pulseScale = 1.2f;
                _pulseDirection = -1;
            }
            else if (_pulseScale <= 1f)
            {
                _pulseScale = 1f;
                _pulseDirection = 1;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_pulseActive && Math.Abs(_pulseScale - 1f) > 0.001f)
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int scaledW = (int)(Width * _pulseScale);
                int scaledH = (int)(Height * _pulseScale);
                int offsetX = (Width - scaledW) / 2;
                int offsetY = (Height - scaledH) / 2;

                var savedState = g.Save();
                g.TranslateTransform(offsetX, offsetY);
                g.ScaleTransform(_pulseScale, _pulseScale);
                base.OnPaint(e);
                g.Restore(savedState);
            }
            else
            {
                base.OnPaint(e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopPulse();
            }
            base.Dispose(disposing);
        }
    }
}
