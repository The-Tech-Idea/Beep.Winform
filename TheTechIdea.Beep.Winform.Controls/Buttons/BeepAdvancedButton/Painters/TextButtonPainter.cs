using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for text-only buttons with minimal styling
    /// </summary>
    public class TextButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle buttonBounds = context.Bounds;

            // Very subtle background on hover
            if (context.State == Enums.AdvancedButtonState.Hover || context.State == Enums.AdvancedButtonState.Pressed)
            {
                int alpha = context.State == Enums.AdvancedButtonState.Pressed ? 24 : 12;
                using (Brush bgBrush = new SolidBrush(Color.FromArgb(alpha, context.SolidBackground)))
                {
                    FillRoundedRectangle(g, bgBrush, buttonBounds, tokens.BorderRadius);
                }
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);

            // Draw text
            Color textColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.SolidBackground;

            if (!context.IsLoading)
            {
                DrawText(g, context, buttonBounds, textColor);
            }
            else
            {
                DrawLoadingSpinner(g, context, buttonBounds, textColor);
            }

            DrawFocusRingPrimitive(g, context);
        }
    }
}
