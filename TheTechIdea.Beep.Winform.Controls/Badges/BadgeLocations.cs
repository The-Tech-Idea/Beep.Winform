namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public static class BadgeLocations
    {
        public static BadgeLocation TopRight(int offsetX = 0, int offsetY = 0)
            => new BadgeLocation { Anchor = BadgeAnchor.TopRight, Offset = new Point(offsetX, offsetY) };

        public static BadgeLocation TopLeft(int offsetX = 0, int offsetY = 0)
            => new BadgeLocation { Anchor = BadgeAnchor.TopLeft, Offset = new Point(offsetX, offsetY) };

        public static BadgeLocation TopCenter(int offsetX = 0, int offsetY = 0)
            => new BadgeLocation { Anchor = BadgeAnchor.TopCenter, Offset = new Point(offsetX, offsetY) };

        public static BadgeLocation BottomRight(int offsetX = 0, int offsetY = 0)
            => new BadgeLocation { Anchor = BadgeAnchor.BottomRight, Offset = new Point(offsetX, offsetY) };

        public static BadgeLocation BottomLeft(int offsetX = 0, int offsetY = 0)
            => new BadgeLocation { Anchor = BadgeAnchor.BottomLeft, Offset = new Point(offsetX, offsetY) };

        public static BadgeLocation BottomCenter(int offsetX = 0, int offsetY = 0)
            => new BadgeLocation { Anchor = BadgeAnchor.BottomCenter, Offset = new Point(offsetX, offsetY) };

        public static BadgeLocation FloatingCorner(BadgeAnchor corner, int nudgeX = 4, int nudgeY = 4)
            => new BadgeLocation { Anchor = corner, Offset = new Point(nudgeX, nudgeY) };

        public static BadgeLocation Css(BadgeSide side, BadgeAlignment alignment)
            => BadgeLocation.FromSideAndAlignment(side, alignment);

        public static BadgeLocation Relative(float fractionX, float fractionY)
            => new BadgeLocation { Anchor = BadgeAnchor.Custom, RelativePosition = new PointF(fractionX, fractionY) };

        public static BadgeLocation Custom(Func<Rectangle, Size, Rectangle> boundsProvider)
            => new BadgeLocation { BoundsProvider = boundsProvider };

        public static BadgeLocation Default
            => new BadgeLocation { Anchor = BadgeAnchor.TopRight };
    }
}
