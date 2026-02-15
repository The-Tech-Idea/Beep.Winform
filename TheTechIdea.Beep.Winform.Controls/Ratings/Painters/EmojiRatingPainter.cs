using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Emoji rating painter using emoji characters
    /// </summary>
    public class EmojiRatingPainter : RatingPainterBase
    {
        public override string Name => "Emoji";

        public override void Paint(RatingPainterContext context)
        {
            if (context.Graphics == null || context.Bounds.Width <= 0 || context.Bounds.Height <= 0)
                return;

            SetupGraphics(context.Graphics);

            var (startX, startY, starSize) = CalculateStarLayout(context);
            var iconSize = RatingIconHelpers.GetIconSize(starSize, RatingStyle.Emoji);

            for (int i = 0; i < context.StarCount; i++)
            {
                bool isFilled = context.SelectedRating > i;
                bool isHovered = i == context.HoveredStar && !context.ReadOnly;
                
                // Determine emoji based on rating value (not just filled/empty)
                int emojiIndex = isFilled ? context.SelectedRating - 1 : -1;
                var iconPath = RatingIconHelpers.GetRatingIconPath(RatingStyle.Emoji, isFilled, emojiIndex);
                
                var iconColor = RatingIconHelpers.GetIconColor(
                    context.Theme,
                    context.UseThemeColors,
                    RatingStyle.Emoji,
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
                    RatingStyle.Emoji,
                    isFilled,
                    isHovered,
                    iconColor,
                    rotation: 0f,
                    textFont: TextFont,
                    ownerControl: context.OwnerControl);
            }

            DrawLabels(context);
            DrawRatingInfo(context);
        }
    }
}

