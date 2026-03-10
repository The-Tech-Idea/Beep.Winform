using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for buttons with icon and text in optimized layout
    /// </summary>
    public class IconTextButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
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
                FillRoundedRectangle(g, bgBrush, buttonBounds, tokens.BorderRadius);
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
                DrawIconAndText(g, context, metrics, contentColor);
            }

            DrawFocusRingPrimitive(g, context);
        }

        private void DrawIconAndText(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Color color)
        {
            Rectangle bounds = context.Bounds;
            bool hasIcon = HasPrimaryIcon(context);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            if (!hasIcon && !hasText) return;

            if (hasIcon && hasText)
            {
                // Calculate total width for centering
                int textWidth = MeasureContextTextWidth(context);
                int totalWidth = metrics.IconSize + metrics.IconTextGap + textWidth;
                int startX = bounds.X + (bounds.Width - totalWidth) / 2;

                // Draw icon on left
                Rectangle iconBounds = new Rectangle(
                    startX,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));

                // Draw text on right
                Rectangle textBounds = new Rectangle(
                    startX + metrics.IconSize + metrics.IconTextGap,
                    bounds.Y,
                    textWidth,
                    bounds.Height
                );
                DrawText(g, context, textBounds, color);

                // Optional: Draw right icon if specified
                if (!string.IsNullOrEmpty(context.IconRight))
                {
                    Rectangle rightIconBounds = new Rectangle(
                        textBounds.Right + metrics.IconTextGap,
                        bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                        metrics.IconSize,
                        metrics.IconSize
                    );
                    DrawIcon(g, context, rightIconBounds, context.IconRight);
                }
            }
            else if (hasIcon)
            {
                // Icon only, centered
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
                // Text only, centered
                DrawText(g, context, bounds, color);
            }
        }

    }
}
