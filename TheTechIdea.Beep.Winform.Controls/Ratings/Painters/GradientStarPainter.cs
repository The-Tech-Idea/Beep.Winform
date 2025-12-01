using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Gradient star rating painter with gradient fills
    /// </summary>
    public class GradientStarPainter : RatingPainterBase
    {
        public override string Name => "Gradient Star";

        public override void Paint(RatingPainterContext context)
        {
            if (context.Graphics == null || context.Bounds.Width <= 0 || context.Bounds.Height <= 0)
                return;

            SetupGraphics(context.Graphics);

            var (startX, startY, starSize) = CalculateStarLayout(context);

            for (int i = 0; i < context.StarCount; i++)
            {
                Color fillColor;
                float scale = context.EnableAnimations && context.StarScale != null && i < context.StarScale.Length
                    ? context.StarScale[i]
                    : 1.0f;
                bool isPartiallyFilled = context.AllowHalfStars && context.PreciseRating > i && context.PreciseRating < i + 1;
                bool isFilled = context.SelectedRating > i || (context.AllowHalfStars && context.PreciseRating > i);

                if (isFilled)
                    fillColor = context.FilledStarColor;
                else if (i == context.HoveredStar && !context.ReadOnly)
                    fillColor = context.HoverStarColor;
                else
                    fillColor = context.EmptyStarColor;

                int scaledSize = (int)(starSize * scale);
                int offsetX = (int)((starSize - scaledSize) / 2);
                int offsetY = (int)((starSize - scaledSize) / 2);

                DrawGradientStar(
                    context.Graphics,
                    startX + i * (starSize + context.Spacing) + offsetX,
                    startY + offsetY,
                    scaledSize,
                    fillColor,
                    isFilled || i == context.HoveredStar,
                    isPartiallyFilled ? (context.PreciseRating - i) : 1.0f,
                    context
                );
            }

            DrawLabels(context);
            DrawRatingInfo(context);
        }

        private void DrawGradientStar(Graphics graphics, int x, int y, int size, Color fillColor, bool isActive, float fillRatio, RatingPainterContext context)
        {
            PointF[] starPoints = CalculateStarPoints(x + size / 2, y + size / 2, size / 2, size / 4, 5);

            if (context.UseGlowEffect && isActive && !context.ReadOnly)
            {
                PointF[] glowPoints = CalculateStarPoints(x + size / 2, y + size / 2, size / 2 + 2, size / 4 + 2, 5);
                DrawGlowEffectForStar(graphics, glowPoints, fillColor, context.GlowIntensity);
            }

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(starPoints);

                if (fillRatio < 1.0f && fillRatio > 0f)
                {
                    int clipWidth = (int)(size * fillRatio);
                    graphics.SetClip(new Rectangle(x, y, clipWidth, size));
                }

                // Create multi-color gradient
                using (PathGradientBrush gradientBrush = new PathGradientBrush(path))
                {
                    gradientBrush.CenterColor = fillColor;
                    gradientBrush.SurroundColors = new Color[]
                    {
                        Color.FromArgb(255, Math.Min(255, fillColor.R + 50), Math.Min(255, fillColor.G + 50), Math.Min(255, fillColor.B + 50)),
                        Color.FromArgb(200, fillColor),
                        Color.FromArgb(150, Math.Max(0, fillColor.R - 30), Math.Max(0, fillColor.G - 30), Math.Max(0, fillColor.B - 30)),
                        Color.FromArgb(200, fillColor),
                        Color.FromArgb(255, Math.Min(255, fillColor.R + 50), Math.Min(255, fillColor.G + 50), Math.Min(255, fillColor.B + 50))
                    };
                    graphics.FillPath(gradientBrush, path);
                }

                if (fillRatio < 1.0f && fillRatio > 0f)
                {
                    graphics.ResetClip();
                    graphics.SetClip(new Rectangle(x + (int)(size * fillRatio), y, size - (int)(size * fillRatio), size));
                    using (SolidBrush emptyBrush = new SolidBrush(context.EmptyStarColor))
                    {
                        graphics.FillPath(emptyBrush, path);
                    }
                    graphics.ResetClip();
                }

                using (Pen pen = new Pen(context.StarBorderColor, context.StarBorderThickness))
                {
                    pen.LineJoin = LineJoin.Round;
                    graphics.DrawPath(pen, path);
                }
            }

            if (isActive && !context.ReadOnly)
            {
                DrawHighlightSpotForStar(graphics, x, y, size);
            }
        }

        private PointF[] CalculateStarPoints(float centerX, float centerY, float outerRadius, float innerRadius, int numPoints)
        {
            PointF[] points = new PointF[numPoints * 2];
            double angleStep = Math.PI / numPoints;
            double angle = -Math.PI / 2;
            for (int i = 0; i < points.Length; i++)
            {
                float radius = i % 2 == 0 ? outerRadius : innerRadius;
                points[i] = new PointF(
                    centerX + (float)(Math.Cos(angle) * radius),
                    centerY + (float)(Math.Sin(angle) * radius));
                angle += angleStep;
            }
            return points;
        }

        private void DrawGlowEffectForStar(Graphics graphics, PointF[] starPoints, Color fillColor, float glowIntensity)
        {
            using (GraphicsPath glowPath = new GraphicsPath())
            {
                glowPath.AddPolygon(starPoints);
                using (PathGradientBrush glowBrush = new PathGradientBrush(glowPath))
                {
                    Color glowColor = Color.FromArgb(
                        (int)(100 * glowIntensity),
                        Math.Min(255, fillColor.R + 40),
                        Math.Min(255, fillColor.G + 40),
                        Math.Min(255, fillColor.B + 40));
                    glowBrush.CenterColor = glowColor;
                    glowBrush.SurroundColors = new Color[] { Color.FromArgb(0, fillColor) };
                    graphics.FillPath(glowBrush, glowPath);
                }
            }
        }

        private void DrawHighlightSpotForStar(Graphics graphics, int x, int y, int size)
        {
            int spotSize = size / 6;
            int spotX = x + size / 4;
            int spotY = y + size / 4;
            using (GraphicsPath spotPath = new GraphicsPath())
            {
                spotPath.AddEllipse(spotX, spotY, spotSize, spotSize);
                using (PathGradientBrush spotBrush = new PathGradientBrush(spotPath))
                {
                    spotBrush.CenterColor = Color.FromArgb(70, 255, 255, 255);
                    spotBrush.SurroundColors = new Color[] { Color.FromArgb(0, 255, 255, 255) };
                    graphics.FillPath(spotBrush, spotPath);
                }
            }
        }
    }
}

