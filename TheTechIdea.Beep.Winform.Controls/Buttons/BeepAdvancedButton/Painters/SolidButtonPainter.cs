using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Modern solid pill button with shadow, slight gradient, and centered content.
    /// Matches the solid filled button style from reference images.
    /// </summary>
    public class SolidButtonPainter : BaseButtonPainter
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
                int shadowBlur = context.State == Enums.AdvancedButtonState.Hover ? 8 : 4;
                DrawShadow(g, bounds, context.BorderRadius, shadowBlur, Color.FromArgb(40, 0, 0, 0));
            }

            // Background with subtle gradient
            Color bgColor = GetBackgroundColor(context);
            Color bgLight = Blend(bgColor, Color.White, 0.15f);
            
            using (var path = GetShapePath(bounds, context))
            using (var gradientBrush = new LinearGradientBrush(bounds, bgLight, bgColor, LinearGradientMode.Vertical))
            {
                g.FillPath(gradientBrush, path);
            }

            // Top highlight for 3D effect
            using (var path = GetShapePath(bounds, context))
            using (var highlightPen = new Pen(Color.FromArgb(40, Color.White), 1f))
            {
                g.DrawPath(highlightPen, path);
            }

            DrawRippleEffect(g, context);

            // Centered content
            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, bounds, GetForegroundColor(context));
            }
            else
            {
                DrawCenteredContent(g, context, metrics);
            }

            DrawFocusRingPrimitive(g, context);
        }

        private void DrawCenteredContent(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics)
        {
            Rectangle bounds = context.Bounds;
            bool hasIcon = HasPrimaryIcon(context);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            if (hasIcon && hasText)
            {
                int totalWidth = metrics.IconSize + metrics.IconTextGap + MeasureContextTextWidth(context);
                int startX = bounds.X + (bounds.Width - totalWidth) / 2;

                DrawIcon(g, context, new Rectangle(startX, bounds.Y + (bounds.Height - metrics.IconSize) / 2, metrics.IconSize, metrics.IconSize),
                    GetPrimaryIconPath(context));

                DrawText(g, context, new Rectangle(startX + metrics.IconSize + metrics.IconTextGap, bounds.Y,
                    totalWidth - metrics.IconSize - metrics.IconTextGap, bounds.Height), GetForegroundColor(context));
            }
            else if (hasIcon)
            {
                DrawIcon(g, context, new Rectangle(bounds.X + (bounds.Width - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2, metrics.IconSize, metrics.IconSize),
                    GetPrimaryIconPath(context));
            }
            else
            {
                DrawText(g, context, bounds, GetForegroundColor(context));
            }
        }
    }
}
