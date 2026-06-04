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
            BorderColor = Color.White;
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

        private void ApplyState(ValidationState state)
        {
            switch (state)
            {
                case ValidationState.Error:
                    BadgeBackColor = Color.FromArgb(220, 60, 60);
                    SvgPathOverride = SvgsUIcons.Common.Error;
                    break;
                case ValidationState.Success:
                    BadgeBackColor = Color.FromArgb(40, 167, 69);
                    SvgPathOverride = SvgsUIcons.Common.Success;
                    break;
                case ValidationState.Warning:
                    BadgeBackColor = Color.FromArgb(255, 152, 0);
                    SvgPathOverride = SvgsUIcons.Common.Warning;
                    break;
                case ValidationState.Info:
                    BadgeBackColor = Color.FromArgb(33, 150, 243);
                    SvgPathOverride = SvgsUIcons.Common.Info;
                    break;
                default:
                    BadgeBackColor = Color.Gray;
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

            try
            {
                StyledImagePainter.Paint(g, iconRect, SvgPathOverride);
            }
            catch
            {
            }
        }
    }
}
