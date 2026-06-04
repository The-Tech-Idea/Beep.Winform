namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepDotBadge : BeepFloatingBadge
    {
        public BeepDotBadge()
        {
            Shape = BadgeShape.Circle;
            BadgeBackColor = Color.Red;
            BadgeDiameter = 10;
            ShowBorder = true;
            BorderColor = Color.White;
            Anchor = BadgeAnchor.TopRight;
        }

        public BeepDotBadge SetColor(Color color)
        {
            BadgeBackColor = color;
            return this;
        }

        public BeepDotBadge At(float fractionX, float fractionY)
        {
            Location = BadgeLocations.Relative(fractionX, fractionY);
            return this;
        }

        protected override void DrawBadgeContent(Graphics g, Rectangle contentBounds)
        {
        }
    }
}
