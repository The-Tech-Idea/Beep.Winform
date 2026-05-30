using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Split Icon Left buttons (Image 1 - Column 3, Top)
    /// Small colored icon on left, white text area on right, with arrow tail
    /// </summary>
    public class SplitIconLeftButtonPainter : BaseButtonPainter
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

            // Split point
            int iconAreaWidth = bounds.Height + 4;
            int splitX = bounds.X + iconAreaWidth;

            // Left area (Colored with icon)
            Rectangle leftArea = new Rectangle(bounds.X, bounds.Y, iconAreaWidth, bounds.Height);
            Color accentColor = context.State == Enums.AdvancedButtonState.Disabled 
                ? context.DisabledBackground 
                : context.SolidBackground;
            if (context.State == Enums.AdvancedButtonState.Hover)
                accentColor = context.HoverBackground;
            else if (context.State == Enums.AdvancedButtonState.Pressed)
                accentColor = context.PressedBackground;

            using (GraphicsPath leftPath = ButtonShapeHelper.CreatePartialRoundedRectangle(
                leftArea, tokens.BorderRadius,
                roundTopLeft: true, roundBottomLeft: true, roundTopRight: false, roundBottomRight: false))
            {
                using (SolidBrush brush = new SolidBrush(accentColor))
                {
                    g.FillPath(brush, leftPath);
                }
            }

            // Icon in left area (white)
            if (HasPrimaryIcon(context) && !context.IsLoading)
            {
                Rectangle iconBounds = new Rectangle(
                    leftArea.X + (leftArea.Width - metrics.IconSize) / 2,
                    leftArea.Y + (leftArea.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                // Draw white icon
                using (GraphicsPath iconPath = iconBounds.ToGraphicsPath())
                {
                    // We'd need a white icon here - using simple arrow for now
                    DrawSmallArrow(g, iconBounds, Color.White);
                }
            }

            // Right area (White with text)
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

            // Little arrow tail pointing to right area (small triangle)
            Point[] tailPoints = new Point[]
            {
                new Point(splitX - 2, bounds.Y + bounds.Height / 2 - 4),
                new Point(splitX + 4, bounds.Y + bounds.Height / 2),
                new Point(splitX - 2, bounds.Y + bounds.Height / 2 + 4)
            };
            using (GraphicsPath tailPath = new GraphicsPath())
            {
                tailPath.AddPolygon(tailPoints);
                using (SolidBrush tailBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(tailBrush, tailPath);
                }
            }

            // Border
            using (GraphicsPath borderPath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, bounds, tokens.BorderRadius))
            {
                using (Pen borderPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, borderPath);
                }
            }

            // Text in right area (dark)
            Rectangle textBounds = new Rectangle(
                rightArea.X + metrics.PaddingHorizontal,
                rightArea.Y,
                rightArea.Width - metrics.PaddingHorizontal * 2,
                rightArea.Height
            );
            Color textColor = Color.FromArgb(50, 50, 50);
            DrawText(g, context, textBounds, textColor);

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }

        private void DrawSmallArrow(Graphics g, Rectangle bounds, Color color)
        {
            using (Pen pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int midY = bounds.Y + bounds.Height / 2;
                g.DrawLine(pen, bounds.X + 4, bounds.Y + 4, bounds.Right - 6, midY);
                g.DrawLine(pen, bounds.Right - 6, midY, bounds.X + 4, bounds.Bottom - 4);
            }
        }
    }
}
