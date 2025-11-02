using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for outlined buttons with border and no fill
    /// </summary>
    public class OutlinedButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Draw background (transparent or subtle on hover)
            Color bgColor = context.State == Enums.AdvancedButtonState.Hover
                ? Color.FromArgb(20, context.BorderColor)
                : Color.Transparent;
            
            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                FillRoundedRectangle(g, bgBrush, buttonBounds, context.BorderRadius);
            }

            // Draw border
            Color borderColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.BorderColor;
                
            using (Pen borderPen = new Pen(borderColor, context.BorderWidth))
            {
                Rectangle borderBounds = new Rectangle(
                    buttonBounds.X + context.BorderWidth / 2,
                    buttonBounds.Y + context.BorderWidth / 2,
                    buttonBounds.Width - context.BorderWidth,
                    buttonBounds.Height - context.BorderWidth
                );
                DrawRoundedRectangle(g, borderPen, borderBounds, context.BorderRadius);
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);

            // Draw content
            Color contentColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.BorderColor;

            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, buttonBounds, contentColor);
            }
            else
            {
                // Draw icon and text centered
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
                // Both icon and text
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
