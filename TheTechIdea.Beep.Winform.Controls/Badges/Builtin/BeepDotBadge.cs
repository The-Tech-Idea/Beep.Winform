namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepDotBadge : BeepFloatingBadge
    {
        public BeepDotBadge()
        {
            Shape = BadgeShape.Circle;
            BadgeDiameter = 10;
            ShowBorder = true;
            Anchor = BadgeAnchor.TopRight;

            // Colours come from the theme's badge slots. Assigning literals here would also have
            // marked them caller-chosen, which would have pinned them against every later theme change.
            Role = BadgeRole.Default;
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
