using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for solid filled buttons
    /// </summary>
    public class SolidButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Draw shadow
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawShadow(g, buttonBounds, context.BorderRadius, context.ShadowBlur, context.ShadowColor);
            }

            // Draw background
            Color bgColor = GetBackgroundColor(context);
            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                FillRoundedRectangle(g, bgBrush, buttonBounds, context.BorderRadius);
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);

            // Calculate content layout
            CalculateLayout(context, metrics, out Rectangle iconBounds, out Rectangle textBounds);

            // Draw loading spinner or icon and text
            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, buttonBounds, GetForegroundColor(context));
            }
            else
            {
                // Draw icon
                if (!string.IsNullOrEmpty(context.ImagePainter?.ImagePath))
                {
                    DrawIcon(g, context, iconBounds, context.ImagePainter.ImagePath);
                }

                // Draw text
                Color textColor = GetForegroundColor(context);
                DrawText(g, context, textBounds, textColor);
            }
        }

        private void CalculateLayout(AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics,
            out Rectangle iconBounds, out Rectangle textBounds)
        {
            Rectangle bounds = context.Bounds;
            bool hasIcon = !string.IsNullOrEmpty(context.ImagePainter?.ImagePath);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            if (hasIcon && hasText)
            {
                // Both icon and text
                int totalWidth = metrics.IconSize + metrics.IconTextGap + MeasureTextWidth(context);
                int startX = bounds.X + (bounds.Width - totalWidth) / 2;

                iconBounds = new Rectangle(
                    startX,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );

                textBounds = new Rectangle(
                    startX + metrics.IconSize + metrics.IconTextGap,
                    bounds.Y,
                    totalWidth - metrics.IconSize - metrics.IconTextGap,
                    bounds.Height
                );
            }
            else if (hasIcon)
            {
                // Icon only
                iconBounds = new Rectangle(
                    bounds.X + (bounds.Width - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                textBounds = Rectangle.Empty;
            }
            else
            {
                // Text only
                iconBounds = Rectangle.Empty;
                textBounds = new Rectangle(
                    bounds.X + metrics.PaddingHorizontal,
                    bounds.Y,
                    bounds.Width - metrics.PaddingHorizontal * 2,
                    bounds.Height
                );
            }
        }

        private int MeasureTextWidth(AdvancedButtonPaintContext context)
        {
            if (string.IsNullOrEmpty(context.Text)) return 0;

            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                SizeF textSize = g.MeasureString(context.Text, context.Font);
                return (int)textSize.Width;
            }
        }
    }
}
