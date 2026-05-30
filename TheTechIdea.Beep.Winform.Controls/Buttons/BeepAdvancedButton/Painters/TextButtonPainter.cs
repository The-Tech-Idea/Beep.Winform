using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Text-only button - minimal styling, accent-colored text, subtle underline on hover.
    /// Matches the "Link" text style from reference images.
    /// </summary>
    public class TextButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle bounds = context.Bounds;

            // Subtle background on hover/pressed
            if (context.State == Enums.AdvancedButtonState.Hover || context.State == Enums.AdvancedButtonState.Pressed)
            {
                int alpha = context.State == Enums.AdvancedButtonState.Pressed ? 30 : 12;
                using (var path = GetShapePath(bounds, context))
                using (var bgBrush = new SolidBrush(Color.FromArgb(alpha, context.SolidBackground)))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            DrawRippleEffect(g, context);

            // Accent-colored text
            Color textColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.SolidBackground;

            if (!context.IsLoading && !string.IsNullOrEmpty(context.Text))
            {
                DrawText(g, context, bounds, textColor);
            }
            else if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, bounds, textColor);
            }

            // Underline on hover
            if (context.State == Enums.AdvancedButtonState.Hover && !string.IsNullOrEmpty(context.Text))
            {
                int textWidth = MeasureContextTextWidth(context);
                int underlineY = bounds.Y + bounds.Height / 2 + 10;
                int underlineX = bounds.X + (bounds.Width - textWidth) / 2;
                using (var underlinePen = new Pen(textColor, 1.5f))
                {
                    g.DrawLine(underlinePen, underlineX, underlineY, underlineX + textWidth, underlineY);
                }
            }

            DrawFocusRingPrimitive(g, context);
        }
    }
}
