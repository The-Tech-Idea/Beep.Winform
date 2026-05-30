using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Toggle button - split-style toggle with ON/OFF visual states.
    /// Shows filled color when ON, outlined when OFF, with smooth transition.
    /// </summary>
    public class ToggleButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Shadow
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawShadow(g, bounds, context.BorderRadius, 4, Color.FromArgb(30, 0, 0, 0));
            }

            Color bgColor, fgColor;
            if (context.IsToggled)
            {
                bgColor = context.State == Enums.AdvancedButtonState.Pressed
                    ? context.PressedBackground : context.SolidBackground;
                fgColor = context.SolidForeground;
            }
            else
            {
                bgColor = context.State == Enums.AdvancedButtonState.Hover
                    ? Color.FromArgb(20, context.SolidBackground) : Color.Transparent;
                fgColor = context.SolidBackground;
            }

            // Background
            if (bgColor.A > 0)
            {
                using (var path = GetShapePath(bounds, context))
                using (var bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            // Border for OFF state
            if (!context.IsToggled)
            {
                using (var path = GetShapePath(bounds, context))
                using (var borderPen = new Pen(context.BorderColor, Math.Max(1.5f, context.BorderWidth)))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            DrawRippleEffect(g, context);

            // Content centered
            Color contentColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground : fgColor;

            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, bounds, contentColor);
            }
            else if (!string.IsNullOrEmpty(context.Text))
            {
                Rectangle contentRect = new Rectangle(
                    bounds.X + metrics.PaddingHorizontal,
                    bounds.Y,
                    bounds.Width - metrics.PaddingHorizontal * 2 - metrics.IconSize - 4,
                    bounds.Height);
                DrawText(g, context, contentRect, contentColor);

                // Toggle indicator icon on right
                using (var pen = new Pen(contentColor, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    int indicatorX = bounds.Right - metrics.PaddingHorizontal - metrics.IconSize + 4;
                    int midY = bounds.Y + bounds.Height / 2;

                    if (context.IsToggled)
                    {
                        // Checkmark style
                        g.DrawLine(pen, indicatorX, midY + 2, indicatorX + 4, midY + 6);
                        g.DrawLine(pen, indicatorX + 4, midY + 6, indicatorX + 14, midY - 4);
                    }
                    else
                    {
                        // X style
                        g.DrawLine(pen, indicatorX + 2, midY - 4, indicatorX + 12, midY + 6);
                        g.DrawLine(pen, indicatorX + 12, midY - 4, indicatorX + 2, midY + 6);
                    }
                }
            }

            DrawFocusRingPrimitive(g, context);
        }
    }
}
