using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Speech Bubble buttons (Image 1 - Column 1, Comments)
    /// White rounded rect with colored triangular tail at bottom
    /// </summary>
    public class SpeechBubbleButtonPainter : BaseButtonPainter
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

            // Bubble color (white)
            Color bubbleColor = Color.White;
            if (context.State == Enums.AdvancedButtonState.Disabled)
                bubbleColor = Color.FromArgb(240, 240, 240);

            // Tail color (accent)
            Color tailColor = context.State == Enums.AdvancedButtonState.Disabled 
                ? context.DisabledBackground 
                : context.SolidBackground;
            
            if (context.State == Enums.AdvancedButtonState.Hover)
                tailColor = context.HoverBackground;
            else if (context.State == Enums.AdvancedButtonState.Pressed)
                tailColor = context.PressedBackground;

            // Draw tail (triangle at bottom)
            int tailWidth = 20;
            int tailHeight = 12;
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
                using (SolidBrush tailBrush = new SolidBrush(tailColor))
                {
                    g.FillPath(tailBrush, tailPath);
                }
            }

            // Draw main bubble (rounded rect)
            Rectangle bubbleBounds = new Rectangle(
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height - tailHeight + 2 // Overlap slightly
            );

            using (GraphicsPath bubblePath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, bubbleBounds, tokens.BorderRadius))
            {
                using (SolidBrush bubbleBrush = new SolidBrush(bubbleColor))
                {
                    g.FillPath(bubbleBrush, bubblePath);
                }

                // Border
                using (Pen borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, bubblePath);
                }
            }

            // Content: Text centered
            Color textColor = context.State == Enums.AdvancedButtonState.Disabled ? Color.Gray : Color.FromArgb(50, 50, 50);
            DrawText(g, context, bubbleBounds, textColor);

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }
    }
}
