using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Containers.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Containers.Painters
{
    internal static class BeepPanelPainter
    {
        public static void DrawTitleOverlay(Graphics g, BeepPanelState state, BeepPanelLayoutContext context, Font titleFont, Color titleColor, Color backColor, Color titleLineColor)
        {
            if (!context.HasTitle || titleFont == null || string.IsNullOrWhiteSpace(state.TitleText))
            {
                return;
            }

            bool solidFill = state.TitleStyle == PanelTitleStyle.Badge || state.TitleStyle == PanelTitleStyle.TopHeader;
            Color effectiveTitleColor = solidFill ? GetContrastingTextColor(titleLineColor) : titleColor;

            if (state.TitleStyle == PanelTitleStyle.GroupBox && !context.TitleGapBounds.IsEmpty)
            {
                using var gapBrush = new SolidBrush(backColor);
                g.FillRectangle(gapBrush, context.TitleGapBounds);
            }

            // TopHeader: solid tab-style header band across the full width.
            if (state.TitleStyle == PanelTitleStyle.TopHeader && !context.HeaderBounds.IsEmpty)
            {
                using var headerBrush = new SolidBrush(titleLineColor);
                g.FillRectangle(headerBrush, context.HeaderBounds);
            }

            // Badge: fill the pill behind the title before drawing text.
            if (state.TitleStyle == PanelTitleStyle.Badge && !context.BadgeBounds.IsEmpty)
            {
                using var badgePath = GraphicsExtensions.CreatePill(context.BadgeBounds.X, context.BadgeBounds.Y, context.BadgeBounds.Width, context.BadgeBounds.Height);
                using var badgeBrush = new SolidBrush(titleLineColor);
                g.FillPath(badgeBrush, badgePath);
            }

            // Divider: horizontal rules on both sides of the title, vertically centered.
            if (state.TitleStyle == PanelTitleStyle.Divider)
            {
                using var pen = new Pen(titleLineColor, Math.Max(1, context.TitleLineBounds.Height));
                if (!context.TitleLineBounds.IsEmpty)
                {
                    int y = context.TitleLineBounds.Y + context.TitleLineBounds.Height / 2;
                    g.DrawLine(pen, context.TitleLineBounds.Left, y, context.TitleLineBounds.Right, y);
                }
                if (!context.TitleLineBoundsRight.IsEmpty)
                {
                    int y = context.TitleLineBoundsRight.Y + context.TitleLineBoundsRight.Height / 2;
                    g.DrawLine(pen, context.TitleLineBoundsRight.Left, y, context.TitleLineBoundsRight.Right, y);
                }
            }

            if (context.HasIcon && !context.IconBounds.IsEmpty && !string.IsNullOrWhiteSpace(state.ResolvedIconPath))
            {
                if (state.TitleIconTintWithForeColor)
                {
                    StyledImagePainter.PaintWithTint(g, context.IconBounds, state.ResolvedIconPath, effectiveTitleColor, 1f, 0);
                }
                else
                {
                    StyledImagePainter.Paint(g, context.IconBounds, state.ResolvedIconPath);
                }
            }

            TextRenderer.DrawText(g, state.TitleText, titleFont, context.TitleBounds.Location, effectiveTitleColor);

            if (context.HasTitleLine && state.TitleStyle != PanelTitleStyle.Divider && !context.TitleLineBounds.IsEmpty)
            {
                using var pen = new Pen(titleLineColor, context.TitleLineBounds.Height);
                int y = context.TitleLineBounds.Y + (context.TitleLineBounds.Height / 2);
                g.DrawLine(pen, context.TitleLineBounds.Left, y, context.TitleLineBounds.Right, y);
            }
        }

        private static Color GetContrastingTextColor(Color background)
        {
            double luminance = (0.299 * background.R + 0.587 * background.G + 0.114 * background.B);
            return luminance < 140 ? Color.White : Color.Black;
        }
    }
}
