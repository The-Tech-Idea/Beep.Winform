using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Ghost button - fully transparent, accent-colored text/icon, subtle fill on hover.
    /// Matches the minimal text/icon style from reference images.
    /// </summary>
    public class GhostButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Subtle background on hover/pressed
            if (context.State == Enums.AdvancedButtonState.Hover ||
                context.State == Enums.AdvancedButtonState.Pressed)
            {
                int alpha = context.State == Enums.AdvancedButtonState.Pressed ? 40 : 15;
                using (var path = GetShapePath(bounds, context))
                using (var bgBrush = new SolidBrush(Color.FromArgb(alpha, context.SolidBackground)))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            DrawRippleEffect(g, context);

            // Content colored with accent
            Color contentColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.SolidBackground;

            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, bounds, contentColor);
            }
            else
            {
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
                int totalWidth = metrics.IconSize + metrics.IconTextGap + MeasureContextTextWidth(context);
                int startX = bounds.X + (bounds.Width - totalWidth) / 2;

                DrawIcon(g, context,
                    new Rectangle(startX, bounds.Y + (bounds.Height - metrics.IconSize) / 2, metrics.IconSize, metrics.IconSize),
                    GetPrimaryIconPath(context));

                DrawText(g, context,
                    new Rectangle(startX + metrics.IconSize + metrics.IconTextGap, bounds.Y,
                        totalWidth - metrics.IconSize - metrics.IconTextGap, bounds.Height), color);
            }
            else if (hasIcon)
            {
                DrawIcon(g, context,
                    new Rectangle(bounds.X + (bounds.Width - metrics.IconSize) / 2,
                        bounds.Y + (bounds.Height - metrics.IconSize) / 2, metrics.IconSize, metrics.IconSize),
                    GetPrimaryIconPath(context));
            }
            else
            {
                DrawText(g, context, bounds, color);
            }
        }
    }
}
