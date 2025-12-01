using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Base class for rating painters providing common functionality
    /// </summary>
    public abstract class RatingPainterBase : IRatingPainter
    {
        public virtual string Name => GetType().Name;

        public virtual void Dispose() { }

        public abstract void Paint(RatingPainterContext context);

        public virtual Size CalculateSize(RatingPainterContext context)
        {
            int width = (context.StarSize * context.StarCount) + (context.Spacing * (context.StarCount - 1));
            int height = context.StarSize;

            // Add space for labels
            if (context.ShowLabels)
            {
                height += 20;
            }

            // Add space for rating count/average
            if (context.ShowRatingCount || context.ShowAverage)
            {
                height += 15;
            }

            return new Size(width, height);
        }

        public virtual Rectangle GetHitTestRect(RatingPainterContext context, int index)
        {
            if (index < 0 || index >= context.StarCount)
                return Rectangle.Empty;

            // Calculate star positions (same as in Paint)
            int additionalTextHeight = 0;
            if (context.ShowLabels) additionalTextHeight += 20;
            if (context.ShowRatingCount || context.ShowAverage) additionalTextHeight += 15;

            int availableWidth = context.Bounds.Width - (context.Spacing * (context.StarCount - 1));
            int availableHeight = context.Bounds.Height - additionalTextHeight;
            int dynamicStarSize = Math.Min(availableWidth / context.StarCount, availableHeight);
            int starSize = Math.Min(context.StarSize, dynamicStarSize);

            int startX = context.Bounds.Left + (context.Bounds.Width - (starSize * context.StarCount + context.Spacing * (context.StarCount - 1))) / 2;
            int startY = context.Bounds.Top + (context.Bounds.Height - (starSize + additionalTextHeight)) / 2;

            return new Rectangle(
                startX + index * (starSize + context.Spacing),
                startY,
                starSize,
                starSize);
        }

        /// <summary>
        /// Setup graphics for high-quality rendering
        /// </summary>
        protected void SetupGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        /// <summary>
        /// Draw rating labels if enabled
        /// </summary>
        protected void DrawLabels(RatingPainterContext context)
        {
            if (!context.ShowLabels || context.SelectedRating <= 0 || context.SelectedRating > context.RatingLabels.Length)
                return;

            string label = context.RatingLabels[context.SelectedRating - 1];
            if (string.IsNullOrEmpty(label))
                return;

            // Get font from font helpers if LabelFont is not set
            Font labelFont = context.LabelFont;
            if (labelFont == null)
            {
                // Use font helpers with context properties
                labelFont = RatingFontHelpers.GetLabelFont(
                    context.ControlStyle,
                    context.RatingStyle,
                    context.StarSize,
                    null);
            }

            using (SolidBrush brush = new SolidBrush(context.LabelColor))
            {
                SizeF textSize = TextUtils.MeasureText(context.Graphics, label, labelFont);
                PointF textPos = new PointF(
                    context.Bounds.Left + (context.Bounds.Width - textSize.Width) / 2,
                    GetStarsBottom(context) + 5);

                context.Graphics.DrawString(label, labelFont, brush, textPos);
            }

            // Dispose font if we created it
            if (labelFont != context.LabelFont)
            {
                labelFont?.Dispose();
            }
        }

        /// <summary>
        /// Draw rating count and average if enabled
        /// </summary>
        protected void DrawRatingInfo(RatingPainterContext context)
        {
            if (!context.ShowRatingCount && !context.ShowAverage)
                return;

            string info = "";
            if (context.ShowRatingCount && context.RatingCount > 0)
            {
                info = $"({context.RatingCount} {(context.RatingCount == 1 ? "rating" : "ratings")})";
            }

            if (context.ShowAverage && context.AverageRating > 0)
            {
                if (!string.IsNullOrEmpty(info)) info += " ";
                info += $"Avg: {context.AverageRating:F1}";
            }

            if (!string.IsNullOrEmpty(info))
            {
                // Get font from font helpers
                Font infoFont;
                
                // Use count font for rating count, average font for average
                if (context.ShowRatingCount && context.ShowAverage)
                {
                    infoFont = RatingFontHelpers.GetCountFont(
                        context.ControlStyle,
                        context.RatingStyle,
                        context.LabelFont);
                }
                else if (context.ShowRatingCount)
                {
                    infoFont = RatingFontHelpers.GetCountFont(
                        context.ControlStyle,
                        context.RatingStyle,
                        context.LabelFont);
                }
                else
                {
                    infoFont = RatingFontHelpers.GetAverageFont(
                        context.ControlStyle,
                        context.RatingStyle,
                        context.LabelFont);
                }

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, context.LabelColor)))
                {
                    SizeF textSize = TextUtils.MeasureText(context.Graphics, info, infoFont);
                    PointF textPos = new PointF(
                        context.Bounds.Left + (context.Bounds.Width - textSize.Width) / 2,
                        context.Bounds.Bottom - textSize.Height - 2);

                    context.Graphics.DrawString(info, infoFont, brush, textPos);
                }

                // Dispose font if we created it (only if it's not the LabelFont from context)
                if (infoFont != context.LabelFont)
                {
                    infoFont?.Dispose();
                }
            }
        }

        /// <summary>
        /// Get the bottom Y coordinate of the stars area
        /// </summary>
        protected int GetStarsBottom(RatingPainterContext context)
        {
            int additionalTextHeight = 0;
            if (context.ShowLabels) additionalTextHeight += 20;
            if (context.ShowRatingCount || context.ShowAverage) additionalTextHeight += 15;

            int availableWidth = context.Bounds.Width - (context.Spacing * (context.StarCount - 1));
            int availableHeight = context.Bounds.Height - additionalTextHeight;
            int dynamicStarSize = Math.Min(availableWidth / context.StarCount, availableHeight);
            int starSize = Math.Min(context.StarSize, dynamicStarSize);

            int startY = context.Bounds.Top + (context.Bounds.Height - (starSize + additionalTextHeight)) / 2;
            return startY + starSize;
        }

        /// <summary>
        /// Calculate star positions and size
        /// </summary>
        protected (int startX, int startY, int starSize) CalculateStarLayout(RatingPainterContext context)
        {
            int additionalTextHeight = 0;
            if (context.ShowLabels) additionalTextHeight += 20;
            if (context.ShowRatingCount || context.ShowAverage) additionalTextHeight += 15;

            int availableWidth = context.Bounds.Width - (context.Spacing * (context.StarCount - 1));
            int availableHeight = context.Bounds.Height - additionalTextHeight;
            int dynamicStarSize = Math.Min(availableWidth / context.StarCount, availableHeight);
            int starSize = Math.Min(context.StarSize, dynamicStarSize);

            int startX = context.Bounds.Left + (context.Bounds.Width - (starSize * context.StarCount + context.Spacing * (context.StarCount - 1))) / 2;
            int startY = context.Bounds.Top + (context.Bounds.Height - (starSize + additionalTextHeight)) / 2;

            return (startX, startY, starSize);
        }
    }
}

