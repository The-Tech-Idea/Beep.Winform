using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Bar rating painter using horizontal bars
    /// </summary>
    public class BarRatingPainter : RatingPainterBase
    {
        public override string Name => "Bar";

        public override void Paint(RatingPainterContext context)
        {
            if (context.Graphics == null || context.Bounds.Width <= 0 || context.Bounds.Height <= 0)
                return;

            SetupGraphics(context.Graphics);

            var (startX, startY, starSize) = CalculateStarLayout(context);
            int barHeight = Math.Max(8, starSize / 3);
            int barSpacing = context.Spacing;

            for (int i = 0; i < context.StarCount; i++)
            {
                bool isFilled = context.SelectedRating > i || (context.AllowHalfStars && context.PreciseRating > i);
                bool isHovered = i == context.HoveredStar && !context.ReadOnly;
                bool isPartiallyFilled = context.AllowHalfStars && context.PreciseRating > i && context.PreciseRating < i + 1;

                Color fillColor;
                if (isFilled)
                    fillColor = context.FilledStarColor;
                else if (isHovered)
                    fillColor = context.HoverStarColor;
                else
                    fillColor = context.EmptyStarColor;

                int barX = startX + i * (starSize + barSpacing);
                int barY = startY + (starSize - barHeight) / 2;
                int barWidth = starSize;

                if (isPartiallyFilled)
                {
                    int filledWidth = (int)(barWidth * (context.PreciseRating - i));
                    Rectangle filledRect = new Rectangle(barX, barY, filledWidth, barHeight);
                    Rectangle emptyRect = new Rectangle(barX + filledWidth, barY, barWidth - filledWidth, barHeight);

                    using (SolidBrush filledBrush = new SolidBrush(fillColor))
                    {
                        context.Graphics.FillRectangle(filledBrush, filledRect);
                    }
                    using (SolidBrush emptyBrush = new SolidBrush(context.EmptyStarColor))
                    {
                        context.Graphics.FillRectangle(emptyBrush, emptyRect);
                    }
                }
                else
                {
                    Rectangle barRect = new Rectangle(barX, barY, barWidth, barHeight);
                    using (SolidBrush brush = new SolidBrush(fillColor))
                    {
                        context.Graphics.FillRectangle(brush, barRect);
                    }
                }

                // Draw border
                Rectangle borderRect = new Rectangle(barX, barY, barWidth, barHeight);
                using (Pen pen = new Pen(context.StarBorderColor, context.StarBorderThickness))
                {
                    context.Graphics.DrawRectangle(pen, borderRect);
                }
            }

            DrawLabels(context);
            DrawRatingInfo(context);
        }
    }
}

