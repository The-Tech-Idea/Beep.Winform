using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Containers.Models;
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

            if (state.TitleStyle == PanelTitleStyle.GroupBox && !context.TitleGapBounds.IsEmpty)
            {
                using var gapBrush = new SolidBrush(backColor);
                g.FillRectangle(gapBrush, context.TitleGapBounds);
            }

            if (context.HasIcon && !context.IconBounds.IsEmpty && !string.IsNullOrWhiteSpace(state.ResolvedIconPath))
            {
                if (state.TitleIconTintWithForeColor)
                {
                    StyledImagePainter.PaintWithTint(g, context.IconBounds, state.ResolvedIconPath, titleColor, 1f, 0);
                }
                else
                {
                    StyledImagePainter.Paint(g, context.IconBounds, state.ResolvedIconPath);
                }
            }

            TextRenderer.DrawText(g, state.TitleText, titleFont, context.TitleBounds.Location, titleColor);

            if (context.HasTitleLine && !context.TitleLineBounds.IsEmpty)
            {
                using var pen = new Pen(titleLineColor, context.TitleLineBounds.Height);
                int y = context.TitleLineBounds.Y + (context.TitleLineBounds.Height / 2);
                g.DrawLine(pen, context.TitleLineBounds.Left, y, context.TitleLineBounds.Right, y);
            }
        }

        public static GraphicsPath BuildGroupBoxBorderPath(BeepPanelState state, BeepPanelLayoutContext context)
        {
            Rectangle bounds = context.BorderBounds;
            GraphicsPath path = new GraphicsPath();
            int radius = System.Math.Max(0, state.BorderRadius);
            int diameter = radius * 2;
            Rectangle gap = context.TitleGapBounds;

            if (radius <= 1)
            {
                if (gap.Left > bounds.Left)
                {
                    path.AddLine(bounds.Left, bounds.Top, gap.Left, bounds.Top);
                }
                if (gap.Right < bounds.Right)
                {
                    path.AddLine(gap.Right, bounds.Top, bounds.Right, bounds.Top);
                }
                path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
                return path;
            }

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            if (gap.Left > bounds.Left + radius)
            {
                path.AddLine(bounds.Left + radius, bounds.Top, gap.Left, bounds.Top);
            }
            if (gap.Right < bounds.Right - radius)
            {
                path.AddLine(gap.Right, bounds.Top, bounds.Right - radius, bounds.Top);
            }
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
