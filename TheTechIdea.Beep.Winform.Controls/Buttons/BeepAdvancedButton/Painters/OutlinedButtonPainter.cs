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
            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle buttonBounds = context.Bounds;

            // Draw background (transparent or subtle on hover)
            Color bgColor = context.State switch
            {
                Enums.AdvancedButtonState.Hover => Color.FromArgb(24, context.BorderColor),
                Enums.AdvancedButtonState.Pressed => Color.FromArgb(40, context.BorderColor),
                _ => Color.Transparent
            };
            
            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                FillRoundedRectangle(g, bgBrush, buttonBounds, tokens.BorderRadius);
            }

            // Draw border
            Color borderColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.BorderColor;
                
            using (Pen borderPen = new Pen(borderColor, Math.Max(1, context.BorderWidth)))
            {
                int borderInset = Math.Max(1, (int)Math.Round(borderPen.Width)) / 2;
                Rectangle borderBounds = new Rectangle(
                    buttonBounds.X + borderInset,
                    buttonBounds.Y + borderInset,
                    buttonBounds.Width - (borderInset * 2),
                    buttonBounds.Height - (borderInset * 2)
                );
                DrawRoundedRectangle(g, borderPen, borderBounds, tokens.BorderRadius);
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);

            // Draw content
            Color contentColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.BorderColor;

            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, buttonBounds, contentColor);
            }
            else
            {
                // Draw icon and text centered
                DrawCenteredContent(g, context, metrics, contentColor);
            }

            DrawFocusRingPrimitive(g, context);
        }

        private void DrawCenteredContent(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Color color)
        {
            Rectangle bounds = context.Bounds;
            bool hasIcon = HasPrimaryIcon(context);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            if (hasIcon && hasText)
            {
                // Both icon and text
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
