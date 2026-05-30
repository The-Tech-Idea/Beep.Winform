using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Outline Pill buttons (Image 1 - Column 1, Top)
    /// Thick colored border, white interior, dark text, small arrow/chevron
    /// </summary>
    public class OutlinePillButtonPainter : BaseButtonPainter
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

            // Border color
            Color borderColor = context.State == Enums.AdvancedButtonState.Disabled 
                ? context.DisabledBackground 
                : context.SolidBackground;
            
            if (context.State == Enums.AdvancedButtonState.Hover)
                borderColor = context.HoverBackground;
            else if (context.State == Enums.AdvancedButtonState.Pressed)
                borderColor = context.PressedBackground;

            // Thick border (6-8px)
            int borderThickness = Math.Max(6, metrics.PaddingVertical);
            
            // Draw outer border path
            using (GraphicsPath outerPath = ButtonShapeHelper.CreateShapePath(context.Shape, bounds, tokens.BorderRadius))
            {
                // Fill white interior
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(whiteBrush, outerPath);
                }

                // Draw thick border
                using (Pen borderPen = new Pen(borderColor, borderThickness))
                {
                    borderPen.Alignment = PenAlignment.Inset;
                    g.DrawPath(borderPen, outerPath);
                }
            }

            // Content: Text + Chevron
            int contentPadding = borderThickness + 8;
            Rectangle contentBounds = new Rectangle(
                bounds.X + contentPadding,
                bounds.Y,
                bounds.Width - contentPadding * 2 - metrics.IconSize,
                bounds.Height
            );

            Color textColor = context.State == Enums.AdvancedButtonState.Disabled ? Color.Gray : Color.FromArgb(50, 50, 50);
            DrawText(g, context, contentBounds, textColor);

            // Chevron on right
            Rectangle chevronBounds = new Rectangle(
                bounds.Right - contentPadding - metrics.IconSize + 4,
                bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                metrics.IconSize - 4,
                metrics.IconSize - 4
            );
            DrawChevron(g, chevronBounds, borderColor);

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
