using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for link-style buttons
    /// </summary>
    public class LinkButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle buttonBounds = context.Bounds;

            // No background - just text with underline

            // Determine text color
            Color textColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : (context.State == Enums.AdvancedButtonState.Hover
                    ? context.HoverBackground
                    : context.SolidBackground);

            if (!context.IsLoading)
            {
                // Draw text
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;

                    using (Brush textBrush = new SolidBrush(textColor))
                    {
                        g.DrawString(context.Text, context.TextFont, textBrush, buttonBounds, sf);
                    }
                }

                if (context.State == Enums.AdvancedButtonState.Hover || context.State == Enums.AdvancedButtonState.Focused)
                {
                    int textWidth = MeasureContextTextWidth(context);
                    int y = buttonBounds.Y + (buttonBounds.Height / 2) + (int)(context.TextFont.Size * 0.45f);
                    int x = buttonBounds.X + (buttonBounds.Width - textWidth) / 2;
                    using Pen underlinePen = new Pen(textColor, 1);
                    g.DrawLine(underlinePen, x, y, x + textWidth, y);
                }
            }
            else
            {
                DrawLoadingSpinner(g, context, buttonBounds, textColor);
            }
        }
    }
}
