using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepIconBadge : BeepFloatingBadge
    {
        private string _svgPath = string.Empty;

        public BeepIconBadge() : this(string.Empty) { }

        public BeepIconBadge(string svgPath)
        {
            _svgPath = svgPath;
            Shape = BadgeShape.Circle;
            BadgeBackColor = Color.White;
            BadgeForeColor = Color.Black;
            Anchor = BadgeAnchor.TopRight;
        }

        public string SvgPath
        {
            get => _svgPath;
            set
            {
                _svgPath = value ?? string.Empty;
                Invalidate();
            }
        }

        public BeepIconBadge SetIcon(string svgPath)
        {
            SvgPath = svgPath;
            return this;
        }

        public BeepIconBadge SetTint(Color tint)
        {
            BadgeForeColor = tint;
            return this;
        }

        public BeepIconBadge At(float fractionX, float fractionY)
        {
            Location = BadgeLocations.Relative(fractionX, fractionY);
            return this;
        }

        protected override void DrawBadgeContent(Graphics g, Rectangle contentBounds)
        {
            if (string.IsNullOrEmpty(_svgPath))
                return;

            int iconSize = (int)(contentBounds.Width * 0.6f);
            int offsetX = contentBounds.X + (contentBounds.Width - iconSize) / 2;
            int offsetY = contentBounds.Y + (contentBounds.Height - iconSize) / 2;
            var iconRect = new Rectangle(offsetX, offsetY, iconSize, iconSize);

            try
            {
                StyledImagePainter.Paint(g, iconRect, _svgPath);
            }
            catch
            {
            }
        }
    }
}
