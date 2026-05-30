using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Gradient badge button - gradient background, white icon circle on left, dark text, chevron.
    /// Matches the "SEARCH / BUY NOW" gradient badge style from reference images.
    /// </summary>
    public class GradientButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Shadow
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawShadow(g, bounds, context.BorderRadius, 8, Color.FromArgb(35, 0, 0, 0));
            }

            // Gradient background
            Color startColor = GetBackgroundColor(context);
            Color endColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledBackground
                : Blend(startColor, context.HoverBackground, 0.4f);

            using (var path = GetShapePath(bounds, context))
            using (var gradientBrush = new LinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Horizontal))
            {
                g.FillPath(gradientBrush, path);
            }

            DrawRippleEffect(g, context);

            // White icon circle on left
            int circleSize = Math.Min(bounds.Height - 6, metrics.IconSize + 10);
            Rectangle circleBounds = new Rectangle(
                bounds.X + 6,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize, circleSize);

            if (HasPrimaryIcon(context) && !context.IsLoading)
            {
                // White circle background
                using (var circlePath = new GraphicsPath())
                {
                    circlePath.AddEllipse(circleBounds);
                    using (var whiteBrush = new SolidBrush(Color.White))
                        g.FillPath(whiteBrush, circlePath);
                }

                // Icon inside
                DrawIcon(g, context,
                    new Rectangle(circleBounds.X + (circleSize - metrics.IconSize) / 2,
                        circleBounds.Y + (circleSize - metrics.IconSize) / 2,
                        metrics.IconSize, metrics.IconSize),
                    GetPrimaryIconPath(context));
            }
            else if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, circleBounds, Color.DimGray);
            }

            // Dark text to right of circle
            int textStartX = circleBounds.Right + 8;
            Rectangle textBounds = new Rectangle(textStartX, bounds.Y,
                bounds.Width - textStartX - metrics.IconSize, bounds.Height);

            Color textColor = Color.FromArgb(50, 50, 50);
            if (!context.IsLoading && !string.IsNullOrEmpty(context.Text))
            {
                DrawText(g, context, textBounds, textColor);
            }

            // White chevron on right
            Rectangle chevronBounds = new Rectangle(
                bounds.Right - metrics.IconSize - 6,
                bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                metrics.IconSize, metrics.IconSize);
            if (!context.IsLoading)
            {
                using (var pen = new Pen(Color.White, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    int midY = chevronBounds.Y + chevronBounds.Height / 2;
                    g.DrawLine(pen, chevronBounds.X + 2, chevronBounds.Y + 2,
                        chevronBounds.Right - 4, midY);
                    g.DrawLine(pen, chevronBounds.Right - 4, midY,
                        chevronBounds.X + 2, chevronBounds.Bottom - 2);
                }
            }

            DrawFocusRingPrimitive(g, context);
        }
    }
}
