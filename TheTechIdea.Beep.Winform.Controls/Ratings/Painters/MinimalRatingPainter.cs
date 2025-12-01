using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Minimal rating painter with simple, clean design
    /// </summary>
    public class MinimalRatingPainter : RatingPainterBase
    {
        public override string Name => "Minimal";

        public override void Paint(RatingPainterContext context)
        {
            if (context.Graphics == null || context.Bounds.Width <= 0 || context.Bounds.Height <= 0)
                return;

            SetupGraphics(context.Graphics);

            var (startX, startY, starSize) = CalculateStarLayout(context);
            var iconSize = RatingIconHelpers.GetIconSize(starSize, RatingStyle.Minimal);

            for (int i = 0; i < context.StarCount; i++)
            {
                bool isFilled = context.SelectedRating > i || (context.AllowHalfStars && context.PreciseRating > i);
                bool isHovered = i == context.HoveredStar && !context.ReadOnly;

                var iconPath = RatingIconHelpers.GetRatingIconPath(RatingStyle.Minimal, isFilled, i);
                var iconColor = RatingIconHelpers.GetIconColor(
                    context.Theme,
                    context.UseThemeColors,
                    RatingStyle.Minimal,
                    isFilled,
                    isHovered,
                    context.FilledStarColor,
                    context.EmptyStarColor,
                    context.HoverStarColor);

                var iconBounds = RatingIconHelpers.CalculateIconBounds(
                    new Rectangle(startX + i * (starSize + context.Spacing), startY, starSize, starSize),
                    iconSize, 0, 1, 0, starSize);

                // Minimal style: just outline or simple fill
                if (isFilled)
                {
                    using (SolidBrush brush = new SolidBrush(iconColor))
                    {
                        context.Graphics.FillEllipse(brush, iconBounds);
                    }
                }
                else
                {
                    using (Pen pen = new Pen(iconColor, 1.5f))
                    {
                        context.Graphics.DrawEllipse(pen, iconBounds);
                    }
                }
            }

            DrawLabels(context);
            DrawRatingInfo(context);
        }
    }
}

