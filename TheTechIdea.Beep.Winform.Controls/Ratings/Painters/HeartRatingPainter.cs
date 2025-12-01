using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Heart-shaped rating painter using heart icons
    /// </summary>
    public class HeartRatingPainter : RatingPainterBase
    {
        public override string Name => "Heart";

        public override void Paint(RatingPainterContext context)
        {
            if (context.Graphics == null || context.Bounds.Width <= 0 || context.Bounds.Height <= 0)
                return;

            SetupGraphics(context.Graphics);

            var (startX, startY, starSize) = CalculateStarLayout(context);
            var iconSize = RatingIconHelpers.GetIconSize(starSize, RatingStyle.Heart);

            for (int i = 0; i < context.StarCount; i++)
            {
                bool isFilled = context.SelectedRating > i || (context.AllowHalfStars && context.PreciseRating > i);
                bool isHovered = i == context.HoveredStar && !context.ReadOnly;
                bool isPartiallyFilled = context.AllowHalfStars && context.PreciseRating > i && context.PreciseRating < i + 1;

                var iconPath = RatingIconHelpers.GetRatingIconPath(RatingStyle.Heart, isFilled, i);
                var iconColor = RatingIconHelpers.GetIconColor(
                    context.Theme,
                    context.UseThemeColors,
                    RatingStyle.Heart,
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
                    RatingStyle.Heart,
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

