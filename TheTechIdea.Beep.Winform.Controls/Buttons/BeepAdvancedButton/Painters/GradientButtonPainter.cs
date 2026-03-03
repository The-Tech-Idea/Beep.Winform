using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for gradient filled buttons
    /// </summary>
    public class GradientButtonPainter : BaseButtonPainter
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

            // Draw gradient background
            Color startColor = GetBackgroundColor(context);
            Color endColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledBackground
                : Color.FromArgb(
                    Math.Max(0, startColor.R - 30),
                    Math.Max(0, startColor.G - 30),
                    Math.Max(0, startColor.B - 30)
                );

            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                buttonBounds,
                startColor,
                endColor,
                LinearGradientMode.Vertical))
            {
                FillRoundedRectangle(g, gradientBrush, buttonBounds, context.BorderRadius);
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);

            // Draw content
            Color contentColor = GetForegroundColor(context);

            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, buttonBounds, contentColor);
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
            bool hasIcon = HasPrimaryIcon(context);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            if (hasIcon && hasText)
            {
                int totalWidth = metrics.IconSize + metrics.IconTextGap + MeasureContextTextWidth(context);
                int startX = bounds.X + (bounds.Width - totalWidth) / 2;

                Rectangle iconBounds = new Rectangle(
                    startX,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));

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
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
            }
            else
            {
                DrawText(g, context, bounds, color);
            }
        }

    }
}
