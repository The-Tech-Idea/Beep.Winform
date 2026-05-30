using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Split Gradient buttons (Image 1 - Column 2, Bottom)
    /// Left half gradient, right half white, with arrow
    /// </summary>
    public class SplitGradientButtonPainter : BaseButtonPainter
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
                DrawShadow(g, bounds, tokens.BorderRadius, 6, Color.FromArgb(30, 0, 0, 0));
            }

            int splitX = bounds.X + (int)(bounds.Width * 0.55);

            // Left area (Gradient)
            Rectangle leftArea = new Rectangle(bounds.X, bounds.Y, splitX - bounds.X, bounds.Height);
            Color startColor = context.SolidBackground;
            Color endColor = context.HoverBackground;
            if (context.State == Enums.AdvancedButtonState.Disabled)
            {
                startColor = context.DisabledBackground;
                endColor = Color.Gray;
            }

            using (GraphicsPath leftPath = ButtonShapeHelper.CreatePartialRoundedRectangle(
                leftArea, tokens.BorderRadius,
                roundTopLeft: true, roundBottomLeft: true, roundTopRight: false, roundBottomRight: false))
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(leftArea, startColor, endColor, LinearGradientMode.Horizontal))
            {
                g.FillPath(gradientBrush, leftPath);
            }

            // Right area (White)
            Rectangle rightArea = new Rectangle(splitX, bounds.Y, bounds.Right - splitX, bounds.Height);
            using (GraphicsPath rightPath = ButtonShapeHelper.CreatePartialRoundedRectangle(
                rightArea, tokens.BorderRadius,
                roundTopLeft: false, roundBottomLeft: false, roundTopRight: true, roundBottomRight: true))
            {
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(whiteBrush, rightPath);
                }
            }

            // Border around whole button
            using (GraphicsPath borderPath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, bounds, tokens.BorderRadius))
            {
                using (Pen borderPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, borderPath);
                }
            }

            // Left text (white)
            Rectangle leftTextBounds = new Rectangle(
                leftArea.X + metrics.PaddingHorizontal,
                leftArea.Y,
                leftArea.Width - metrics.PaddingHorizontal * 2,
                leftArea.Height
            );
            DrawText(g, context, leftTextBounds, Color.White);

            // Right chevron (colored)
            Rectangle chevronBounds = new Rectangle(
                rightArea.X + (rightArea.Width - metrics.IconSize) / 2,
                rightArea.Y + (rightArea.Height - metrics.IconSize) / 2,
                metrics.IconSize,
                metrics.IconSize
            );
            DrawChevron(g, chevronBounds, startColor);

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }

        private void DrawChevron(Graphics g, Rectangle bounds, Color color)
        {
            using (Pen pen = new Pen(color, 2f))
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
