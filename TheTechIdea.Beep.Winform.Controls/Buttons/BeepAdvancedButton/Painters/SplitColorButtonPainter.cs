using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for two-tone split buttons (Image 1 style).
    /// Features a colored section and a white section with icons and text.
    /// </summary>
    public class SplitColorButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle bounds = context.Bounds;

            // 1. Draw Shadow (Subtle)
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawShadow(g, bounds, tokens.BorderRadius, Math.Max(4, context.ShadowBlur), Color.FromArgb(30, 0, 0, 0));
            }

            // 2. Define Split Areas
            // Split roughly 50/50 or based on content. 
            // For this style, a fixed split or dynamic split works. We'll use a dynamic split based on icon position.
            int splitX = bounds.X + (bounds.Width / 2);

            // Left Area (Colored)
            Rectangle leftArea = new Rectangle(bounds.X, bounds.Y, bounds.Width / 2, bounds.Height);
            // Right Area (White/Light)
            Rectangle rightArea = new Rectangle(bounds.X + bounds.Width / 2, bounds.Y, bounds.Width / 2, bounds.Height);

            // 3. Draw Backgrounds
            Color accentColor = context.SolidBackground;
            Color secondaryColor = Color.White;

            // Determine state colors
            if (context.State == Enums.AdvancedButtonState.Hover)
            {
                accentColor = context.HoverBackground;
            }
            else if (context.State == Enums.AdvancedButtonState.Pressed)
            {
                accentColor = context.PressedBackground;
            }
            else if (context.State == Enums.AdvancedButtonState.Disabled)
            {
                accentColor = context.DisabledBackground;
                secondaryColor = Color.FromArgb(240, 240, 240);
            }

            // Draw Left Path (Colored)
            using (GraphicsPath leftPath = ButtonShapeHelper.CreatePartialRoundedRectangle(
                leftArea, tokens.BorderRadius,
                roundTopLeft: true, roundBottomLeft: true, roundTopRight: false, roundBottomRight: false))
            {
                using (SolidBrush brush = new SolidBrush(accentColor))
                {
                    g.FillPath(brush, leftPath);
                }
            }

            // Draw Right Path (White)
            using (GraphicsPath rightPath = ButtonShapeHelper.CreatePartialRoundedRectangle(
                rightArea, tokens.BorderRadius,
                roundTopLeft: false, roundBottomLeft: false, roundTopRight: true, roundBottomRight: true))
            {
                using (SolidBrush brush = new SolidBrush(secondaryColor))
                {
                    g.FillPath(brush, rightPath);
                }
            }

            // 4. Draw Separator Line (Optional, adds definition)
            using (Pen separatorPen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
            {
                g.DrawLine(separatorPen, splitX, bounds.Y + 4, splitX, bounds.Bottom - 4);
            }

            // 5. Draw Content
            // Layout: Text in Colored Area (Left), Icon in White Area (Right)
            // OR: Icon in Colored, Text in White.
            // Based on Image 1 "DOWNLOAD", "UPLOAD": Text is Left (Colored), Icon is Right (White).
            
            DrawContent(context, g, metrics, leftArea, rightArea, accentColor, secondaryColor);

            // 6. Ripple
            DrawRippleEffect(g, context);

            // 7. Focus Ring
            DrawFocusRingPrimitive(g, context);
        }

        private void DrawContent(AdvancedButtonPaintContext context, Graphics g, AdvancedButtonMetrics metrics, 
            Rectangle leftArea, Rectangle rightArea, Color accentColor, Color secondaryColor)
        {
            bool hasIcon = HasPrimaryIcon(context);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            // Default Layout: Text Left, Icon Right
            if (hasText && hasIcon)
            {
                // Text in Left Area (Colored) -> White Text
                DrawText(g, context, leftArea, Color.White);

                // Icon in Right Area (White) -> Accent Color Icon
                Rectangle iconBounds = new Rectangle(
                    rightArea.X + (rightArea.Width - metrics.IconSize) / 2,
                    rightArea.Y + (rightArea.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
            }
            else if (hasText)
            {
                // Text Centered (Spanning or Left)
                DrawText(g, context, context.Bounds, Color.White);
            }
            else if (hasIcon)
            {
                // Icon Centered
                Rectangle iconBounds = new Rectangle(
                    context.Bounds.X + (context.Bounds.Width - metrics.IconSize) / 2,
                    context.Bounds.Y + (context.Bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
            }
        }
    }
}
