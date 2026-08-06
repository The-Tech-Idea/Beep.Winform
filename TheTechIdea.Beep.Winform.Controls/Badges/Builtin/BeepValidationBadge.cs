using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepValidationBadge : BeepFloatingBadge
    {
        private ValidationState _state = ValidationState.None;
        private string _message = string.Empty;

        public BeepValidationBadge() : this(ValidationState.None) { }

        public BeepValidationBadge(ValidationState state)
        {
            _state = state;
            Shape = BadgeShape.Circle;
            BadgeDiameter = 20;
            ShowBorder = true;
            Anchor = BadgeAnchor.TopRight;
            ApplyState(_state);
        }

        public ValidationState State
        {
            get => _state;
            set
            {
                _state = value;
                ApplyState(_state);
                Invalidate();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value ?? string.Empty;
            }
        }

        public BeepValidationBadge SetState(ValidationState state)
        {
            State = state;
            return this;
        }

        public BeepValidationBadge SetMessage(string message)
        {
            Message = message;
            return this;
        }

        public BeepValidationBadge At(float fractionX, float fractionY)
        {
            Location = BadgeLocations.Relative(fractionX, fractionY);
            return this;
        }

        /// <summary>
        /// Maps the state to a theme role and a glyph.
        /// </summary>
        /// <remarks>
        /// These four colours are the exception this library allows to "nothing assigns colours": an
        /// error badge is red because red means error, not because red is the accent. Taking them from
        /// the theme's semantic slots rather than from literal ARGB keeps the meaning while letting a
        /// dark or high-contrast palette pick its own red.
        /// </remarks>
        private void ApplyState(ValidationState state)
        {
            switch (state)
            {
                case ValidationState.Error:
                    Role = BadgeRole.Error;
                    SvgPathOverride = SvgsUIcons.Common.Error;
                    break;
                case ValidationState.Success:
                    Role = BadgeRole.Success;
                    SvgPathOverride = SvgsUIcons.Common.Success;
                    break;
                case ValidationState.Warning:
                    Role = BadgeRole.Warning;
                    SvgPathOverride = SvgsUIcons.Common.Warning;
                    break;
                case ValidationState.Info:
                    Role = BadgeRole.Info;
                    SvgPathOverride = SvgsUIcons.Common.Info;
                    break;
                default:
                    Role = BadgeRole.Surface;
                    SvgPathOverride = string.Empty;
                    break;
            }
        }

        private string? SvgPathOverride { get; set; }

        protected override void DrawBadgeContent(Graphics g, Rectangle contentBounds)
        {
            if (_state == ValidationState.None || string.IsNullOrEmpty(SvgPathOverride))
                return;

            int iconSize = (int)(contentBounds.Width * 0.65f);
            int offsetX = contentBounds.X + (contentBounds.Width - iconSize) / 2;
            int offsetY = contentBounds.Y + (contentBounds.Height - iconSize) / 2;
            var iconRect = new Rectangle(offsetX, offsetY, iconSize, iconSize);

            DrawIconOrFallback(g, iconRect, SvgPathOverride);
        }
    }
}
