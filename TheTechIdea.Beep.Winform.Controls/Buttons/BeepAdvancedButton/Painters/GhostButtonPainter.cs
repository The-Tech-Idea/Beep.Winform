using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for ghost buttons with subtle styling
    /// </summary>
    public class GhostButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Very subtle background on hover/pressed
            if (context.State == Enums.AdvancedButtonState.Hover ||
                context.State == Enums.AdvancedButtonState.Pressed)
            {
                int alpha = context.State == Enums.AdvancedButtonState.Pressed ? 30 : 15;
                Color bgColor = Color.FromArgb(alpha, context.SolidBackground);

                using (Brush bgBrush = new SolidBrush(bgColor))
                {
                    FillRoundedRectangle(g, bgBrush, buttonBounds, context.BorderRadius);
                }
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);

            // Draw content
            Color contentColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.SolidBackground;

            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, buttonBounds, contentColor);
            }
            else
            {
                DrawCenteredContent(g, context, metrics, contentColor);
            }
        }

        private void DrawCenteredContent(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Color color)
        {
            Rectangle bounds = context.Bounds;
            bool hasIcon = !string.IsNullOrEmpty(context.ImagePainter?.ImagePath);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            if (hasIcon && hasText)
            {
                int totalWidth = metrics.IconSize + metrics.IconTextGap + MeasureTextWidth(context);
                int startX = bounds.X + (bounds.Width - totalWidth) / 2;

                Rectangle iconBounds = new Rectangle(
                    startX,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.ImagePainter.ImagePath);

                Rectangle textBounds = new Rectangle(
                    startX + metrics.IconSize + metrics.IconTextGap,
                    bounds.Y,
                    totalWidth - metrics.IconSize - metrics.IconTextGap,
                    bounds.Height
                );
                DrawText(g, context, textBounds, color);
            }
            else if (hasIcon)
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (bounds.Width - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.ImagePainter.ImagePath);
            }
            else
            {
                DrawText(g, context, bounds, color);
            }
        }

        private int MeasureTextWidth(AdvancedButtonPaintContext context)
        {
            if (string.IsNullOrEmpty(context.Text)) return 0;
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                return (int)g.MeasureString(context.Text, context.Font).Width;
            }
        }
    }
}
