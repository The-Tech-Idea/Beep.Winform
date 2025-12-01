using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Thumb rating painter using thumbs-up/thumbs-down icons
    /// </summary>
    public class ThumbRatingPainter : RatingPainterBase
    {
        public override string Name => "Thumb";

        public override void Paint(RatingPainterContext context)
        {
            if (context.Graphics == null || context.Bounds.Width <= 0 || context.Bounds.Height <= 0)
                return;

            SetupGraphics(context.Graphics);

            var (startX, startY, starSize) = CalculateStarLayout(context);
            var iconSize = RatingIconHelpers.GetIconSize(starSize, RatingStyle.Thumb);

            for (int i = 0; i < context.StarCount; i++)
            {
                bool isFilled = context.SelectedRating > i;
                bool isHovered = i == context.HoveredStar && !context.ReadOnly;

                var iconPath = RatingIconHelpers.GetRatingIconPath(RatingStyle.Thumb, isFilled, i);
                var iconColor = RatingIconHelpers.GetIconColor(
                    context.Theme,
                    context.UseThemeColors,
                    RatingStyle.Thumb,
                    isFilled,
                    isHovered,
                    context.FilledStarColor,
                    context.EmptyStarColor,
                    context.HoverStarColor);

                var iconBounds = RatingIconHelpers.CalculateIconBounds(
                    new Rectangle(startX + i * (starSize + context.Spacing), startY, starSize, starSize),
                    iconSize, 0, 1, 0, starSize);

                RatingIconHelpers.PaintIcon(
                    context.Graphics,
                    iconBounds,
                    iconPath,
                    context.Theme,
                    context.UseThemeColors,
                    RatingStyle.Thumb,
                    isFilled,
                    isHovered,
                    iconColor);
            }

            DrawLabels(context);
            DrawRatingInfo(context);
        }
    }
}

