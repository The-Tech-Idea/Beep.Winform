namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepNotificationBadge : BeepCounterBadge
    {
        private bool _pulseEnabled;
        private Timer? _pulseTimer;
        private float _pulsePhase;
        private int _pulseDirection = 1;

        /// <summary>How far the fill lightens at the peak of a pulse, 0..1.</summary>
        private const float PulseDepth = 0.35f;

        public BeepNotificationBadge() : this("") { }

        public BeepNotificationBadge(string text) : base(text)
        {
            Shape = BadgeShape.Circle;
            Role = BadgeRole.Default;
            ShowDropShadow = true;
            PulseEnabled = true;
        }

        /// <summary>Whether the caller wants this badge to pulse.</summary>
        /// <remarks>
        /// Distinct from whether the timer is currently ticking: a hidden or detached badge stops
        /// animating without forgetting that it was asked to. The two used to share one field, so
        /// stopping the timer for visibility would have read back as "the caller turned it off".
        /// </remarks>
        public bool PulseEnabled
        {
            get => _pulseEnabled;
            set
            {
                if (_pulseEnabled == value) return;
                _pulseEnabled = value;
                UpdatePulseTimer();
            }
        }

        /// <summary>Whether the pulse timer is actually ticking right now.</summary>
        public bool IsPulsing => _pulseTimer is not null;

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

        /// <summary>
        /// The fill, lightened by the current pulse phase.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The pulse used to scale the <i>drawing</i>: a <c>ScaleTransform</c> of up to 1.2x with a
        /// negative translate to keep it centred. A WinForms control cannot paint outside its client
        /// rectangle, so everything the transform pushed past the edge was clipped — measured, the
        /// control was still 20px wide at both rest and peak. The visible result was not a
        /// grow-and-shrink but the badge's edges squaring off at each peak and rounding out again.
        /// </para>
        /// <para>
        /// Two fixes were available: grow the control to its peak extent and draw the resting badge
        /// inset within it, or animate something that is not size. <b>This is the second.</b> It needs
        /// no extra layout footprint, cannot clip by construction, and does not have to interact with
        /// <c>CornerOverlap</c>, which centres the badge on the target's corner from the control's own
        /// bounds — growing the control would have moved that anchor point on every frame.
        /// </para>
        /// </remarks>
        protected override Color EffectiveBackColor
        {
            get
            {
                var baseColour = base.EffectiveBackColor;
                if (!_pulseEnabled || _pulsePhase <= 0.001f) return baseColour;

                float t = _pulsePhase * PulseDepth;
                return Color.FromArgb(
                    baseColour.A,
                    Blend(baseColour.R, t),
                    Blend(baseColour.G, t),
                    Blend(baseColour.B, t));
            }
        }

        private static int Blend(byte channel, float towardsWhite)
            => (int)Math.Round(channel + (255 - channel) * towardsWhite);

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            UpdatePulseTimer();
        }

        public override void Detach()
        {
            base.Detach();
            UpdatePulseTimer();
        }

        /// <summary>Runs the timer only while the badge can be seen.</summary>
        /// <remarks>
        /// A hidden badge invalidating every 40ms for the life of the form is pure cost. The timer used
        /// to start in the constructor and run until disposal regardless of visibility.
        /// </remarks>
        private void UpdatePulseTimer()
        {
            bool shouldRun = _pulseEnabled && Visible && !IsDisposed;

            if (shouldRun && _pulseTimer is null)
            {
                _pulseTimer = new Timer { Interval = 40 };
                _pulseTimer.Tick += OnPulseTick;
                _pulseTimer.Start();
            }
            else if (!shouldRun && _pulseTimer is not null)
            {
                _pulseTimer.Stop();
                _pulseTimer.Tick -= OnPulseTick;
                _pulseTimer.Dispose();
                _pulseTimer = null;
                _pulsePhase = 0f;
                _pulseDirection = 1;
                if (!IsDisposed) Invalidate();
            }
        }

        private void OnPulseTick(object? sender, EventArgs e)
        {
            _pulsePhase += _pulseDirection * 0.08f;

            if (_pulsePhase >= 1f) { _pulsePhase = 1f; _pulseDirection = -1; }
            else if (_pulsePhase <= 0f) { _pulsePhase = 0f; _pulseDirection = 1; }

            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pulseEnabled = false;
                UpdatePulseTimer();
            }
            base.Dispose(disposing);
        }
    }
}
