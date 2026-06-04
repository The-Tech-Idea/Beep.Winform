using System;

namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public sealed class BadgeLocation
    {
        public BadgeAnchor Anchor { get; set; } = BadgeAnchor.TopRight;

        public Point Offset { get; set; } = Point.Empty;

        public PointF? RelativePosition { get; set; }

        public Func<Rectangle, Size, Rectangle>? BoundsProvider { get; set; }

        public Rectangle ComputeBounds(Rectangle ownerBounds, Size badgeSize)
        {
            if (BoundsProvider is not null)
                return BoundsProvider(ownerBounds, badgeSize);

            if (RelativePosition.HasValue)
            {
                var rp = RelativePosition.Value;
                int x = ownerBounds.Left + (int)(ownerBounds.Width * rp.X) - badgeSize.Width / 2;
                int y = ownerBounds.Top + (int)(ownerBounds.Height * rp.Y) - badgeSize.Height / 2;
                return new Rectangle(x + Offset.X, y + Offset.Y, badgeSize.Width, badgeSize.Height);
            }

            int left, top;

            switch (Anchor)
            {
                case BadgeAnchor.TopLeft:
                    left = ownerBounds.Left;
                    top = ownerBounds.Top;
                    break;
                case BadgeAnchor.TopCenter:
                    left = ownerBounds.Left + (ownerBounds.Width - badgeSize.Width) / 2;
                    top = ownerBounds.Top;
                    break;
                case BadgeAnchor.TopRight:
                    left = ownerBounds.Right - badgeSize.Width;
                    top = ownerBounds.Top;
                    break;
                case BadgeAnchor.BottomLeft:
                    left = ownerBounds.Left;
                    top = ownerBounds.Bottom - badgeSize.Height;
                    break;
                case BadgeAnchor.BottomCenter:
                    left = ownerBounds.Left + (ownerBounds.Width - badgeSize.Width) / 2;
                    top = ownerBounds.Bottom - badgeSize.Height;
                    break;
                case BadgeAnchor.BottomRight:
                    left = ownerBounds.Right - badgeSize.Width;
                    top = ownerBounds.Bottom - badgeSize.Height;
                    break;
                case BadgeAnchor.MiddleLeft:
                    left = ownerBounds.Left;
                    top = ownerBounds.Top + (ownerBounds.Height - badgeSize.Height) / 2;
                    break;
                case BadgeAnchor.MiddleCenter:
                    left = ownerBounds.Left + (ownerBounds.Width - badgeSize.Width) / 2;
                    top = ownerBounds.Top + (ownerBounds.Height - badgeSize.Height) / 2;
                    break;
                case BadgeAnchor.MiddleRight:
                    left = ownerBounds.Right - badgeSize.Width;
                    top = ownerBounds.Top + (ownerBounds.Height - badgeSize.Height) / 2;
                    break;
                default:
                    left = ownerBounds.Right - badgeSize.Width;
                    top = ownerBounds.Top;
                    break;
            }

            return new Rectangle(left + Offset.X, top + Offset.Y, badgeSize.Width, badgeSize.Height);
        }

        public static BadgeLocation FromSideAndAlignment(BadgeSide side, BadgeAlignment alignment)
        {
            var loc = new BadgeLocation();
            loc.Anchor = (side, alignment) switch
            {
                (BadgeSide.Top, BadgeAlignment.Start) => BadgeAnchor.TopLeft,
                (BadgeSide.Top, BadgeAlignment.Center) => BadgeAnchor.TopCenter,
                (BadgeSide.Top, BadgeAlignment.End) => BadgeAnchor.TopRight,
                (BadgeSide.Bottom, BadgeAlignment.Start) => BadgeAnchor.BottomLeft,
                (BadgeSide.Bottom, BadgeAlignment.Center) => BadgeAnchor.BottomCenter,
                (BadgeSide.Bottom, BadgeAlignment.End) => BadgeAnchor.BottomRight,
                (BadgeSide.Left, BadgeAlignment.Start) => BadgeAnchor.TopLeft,
                (BadgeSide.Left, BadgeAlignment.Center) => BadgeAnchor.MiddleLeft,
                (BadgeSide.Left, BadgeAlignment.End) => BadgeAnchor.BottomLeft,
                (BadgeSide.Right, BadgeAlignment.Start) => BadgeAnchor.TopRight,
                (BadgeSide.Right, BadgeAlignment.Center) => BadgeAnchor.MiddleRight,
                (BadgeSide.Right, BadgeAlignment.End) => BadgeAnchor.BottomRight,
                _ => BadgeAnchor.TopRight,
            };
            return loc;
        }
    }
}
