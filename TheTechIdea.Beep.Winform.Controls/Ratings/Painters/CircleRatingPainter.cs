using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Circle rating painter using filled/outline circle icons
    /// </summary>
    public class CircleRatingPainter : RatingPainterBase
    {
        public override string Name => "Circle";

        public override void Paint(RatingPainterContext context)
        {
            if (context.Graphics == null || context.Bounds.Width <= 0 || context.Bounds.Height <= 0)
                return;

            SetupGraphics(context.Graphics);

            var (startX, startY, starSize) = CalculateStarLayout(context);
            var iconSize = RatingIconHelpers.GetIconSize(starSize, RatingStyle.Circle);

            for (int i = 0; i < context.StarCount; i++)
            {
                bool isFilled = context.SelectedRating > i || (context.AllowHalfStars && context.PreciseRating > i);
                bool isHovered = i == context.HoveredStar && !context.ReadOnly;
                bool isPartiallyFilled = context.AllowHalfStars && context.PreciseRating > i && context.PreciseRating < i + 1;

                var iconPath = RatingIconHelpers.GetRatingIconPath(RatingStyle.Circle, isFilled, i);
                var iconColor = RatingIconHelpers.GetIconColor(
                    context.Theme,
                    context.UseThemeColors,
                    RatingStyle.Circle,
                    isFilled,
                    isHovered,
                    context.FilledStarColor,
                    context.EmptyStarColor,
                    context.HoverStarColor);

                var iconBounds = RatingIconHelpers.CalculateIconBounds(
                    new Rectangle(startX + i * (starSize + context.Spacing), startY, starSize, starSize),
                    iconSize, 0, 1, 0, starSize);

                if (isPartiallyFilled)
                {
                    int clipWidth = (int)(iconBounds.Width * (context.PreciseRating - i));
                    Rectangle clipRect = new Rectangle(iconBounds.X, iconBounds.Y, clipWidth, iconBounds.Height);
                    context.Graphics.SetClip(clipRect);
                }

                RatingIconHelpers.PaintIcon(
                    context.Graphics,
                    iconBounds,
                    iconPath,
                    context.Theme,
                    context.UseThemeColors,
                    RatingStyle.Circle,
                    isFilled,
                    isHovered,
                    iconColor);

                if (isPartiallyFilled)
                {
                    context.Graphics.ResetClip();
                }
            }

            DrawLabels(context);
            DrawRatingInfo(context);
        }
    }
}

