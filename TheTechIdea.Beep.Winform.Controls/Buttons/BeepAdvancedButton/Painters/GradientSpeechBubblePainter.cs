using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Gradient Speech Bubble buttons (Image 1 - Column 2, Center)
    /// Gradient rounded rect with speech tail and white horizontal line
    /// </summary>
    public class GradientSpeechBubblePainter : BaseButtonPainter
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
                DrawShadow(g, bounds, tokens.BorderRadius, 8, Color.FromArgb(30, 0, 0, 0));
            }

            // Gradient colors
            Color startColor = context.SolidBackground;
            Color endColor = context.HoverBackground;
            if (context.State == Enums.AdvancedButtonState.Disabled)
            {
                startColor = context.DisabledBackground;
                endColor = Color.Gray;
            }

            // Tail (pointing down)
            int tailWidth = 24;
            int tailHeight = 14;
            int tailX = bounds.X + bounds.Width / 2;
            int tailY = bounds.Bottom;

            Point[] tailPoints = new Point[]
            {
                new Point(tailX - tailWidth / 2, tailY - tailHeight),
                new Point(tailX + tailWidth / 2, tailY - tailHeight),
                new Point(tailX, tailY)
            };

            using (GraphicsPath tailPath = new GraphicsPath())
            {
                tailPath.AddPolygon(tailPoints);
                using (SolidBrush tailBrush = new SolidBrush(endColor))
                {
                    g.FillPath(tailBrush, tailPath);
                }
            }

            // Main bubble
            Rectangle bubbleBounds = new Rectangle(
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height - tailHeight + 2
            );

            using (GraphicsPath bubblePath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, bubbleBounds, tokens.BorderRadius))
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(bubbleBounds, startColor, endColor, LinearGradientMode.Horizontal))
            {
                g.FillPath(gradientBrush, bubblePath);
            }

            // White horizontal accent line at bottom of bubble
            int lineY = bubbleBounds.Bottom - 6;
            int lineMargin = metrics.PaddingHorizontal;
            using (Pen linePen = new Pen(Color.White, 2.5f))
            {
                linePen.StartCap = LineCap.Round;
                linePen.EndCap = LineCap.Round;
                g.DrawLine(linePen, bubbleBounds.X + lineMargin, lineY, bubbleBounds.Right - lineMargin, lineY);
            }

            // Content: Text centered
            Color textColor = Color.White;
            DrawText(g, context, bubbleBounds, textColor);

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }
    }
}
