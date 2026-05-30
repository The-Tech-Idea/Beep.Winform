using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Mini Gradient Pill buttons (Image 1 - Bottom left)
    /// Small rounded pill with gradient, icon inside
    /// </summary>
    public class MiniGradientPillButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle bounds = context.Bounds;

            // Shadow
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawShadow(g, bounds, tokens.BorderRadius, 6, Color.FromArgb(40, 0, 0, 0));
            }

            // Gradient colors
            Color startColor = context.SolidBackground;
            Color endColor = context.HoverBackground;
            if (context.State == Enums.AdvancedButtonState.Disabled)
            {
                startColor = context.DisabledBackground;
                endColor = Color.Gray;
            }

            // Draw gradient pill
            using (GraphicsPath pillPath = ButtonShapeHelper.CreateShapePath(ButtonShape.Pill, bounds, tokens.BorderRadius))
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Horizontal))
            {
                g.FillPath(gradientBrush, pillPath);
            }

            // Content: Small text + icon
            int contentWidth = metrics.IconSize + 4 + MeasureContextTextWidth(context);
            int startX = bounds.X + (bounds.Width - contentWidth) / 2;

            // Text
            Rectangle textBounds = new Rectangle(
                startX,
                bounds.Y,
                contentWidth - metrics.IconSize - 4,
                bounds.Height
            );
            Color textColor = Color.White;
            DrawText(g, context, textBounds, textColor);

            // Small icon/chevron
            Rectangle iconBounds = new Rectangle(
                startX + contentWidth - metrics.IconSize,
                bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                metrics.IconSize,
                metrics.IconSize
            );
            DrawSmallChevron(g, iconBounds, Color.White);

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }

        private void DrawSmallChevron(Graphics g, Rectangle bounds, Color color)
        {
            using (Pen pen = new Pen(color, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int midY = bounds.Y + bounds.Height / 2;
                g.DrawLine(pen, bounds.X + 2, bounds.Y + 4, bounds.Right - 4, midY);
                g.DrawLine(pen, bounds.Right - 4, midY, bounds.X + 2, bounds.Bottom - 4);
            }
        }
    }
}
