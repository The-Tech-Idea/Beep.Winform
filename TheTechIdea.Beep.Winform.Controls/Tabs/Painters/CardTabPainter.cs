using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class CardTabPainter : BaseTabPainter
    {
        public CardTabPainter(BeepTabs tabControl) : base(tabControl) { }

        private GraphicsPath GetRoundedTopRect(RectangleF rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            RectangleF arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));
            
            // Top Left
            path.AddArc(arc, 180, 90);
            
            // Top Right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom Right
            path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom);
            
            // Bottom Left
            path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + radius);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>Gap between cards, so they read as separate sheets rather than one strip.</summary>
        // Design-time pixels; scaled per display via BaseTabPainter.Scale.
        private const int CardGap = 3;

        public override void PaintTabItem(Graphics g, Tabs.Models.BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            Rectangle bounds = itemLayout.Bounds;
            if (bounds.IsEmpty) return;

            int a = (int)(Math.Clamp(alpha, 0f, 1f) * 255f);
            Color borderColor = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabThemeHelpers
                .GetTabBorderColor(Theme, itemLayout.Item.IsSelected, false);

            // Separated cards, and the selected one lifted a little higher than its neighbours.
            int lift = itemLayout.Item.IsSelected ? 0 : Scale(3);
            var drawRect = new RectangleF(
                bounds.X + Scale(CardGap),
                bounds.Y + lift,
                Math.Max(0, bounds.Width - Scale(CardGap) * 2),
                Math.Max(0, bounds.Height - lift));
            if (drawRect.Width <= 0 || drawRect.Height <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = GetRoundedTopRect(drawRect, Scale(6)))
            {
                // Resolved through the colour seam. Reading Theme.ButtonBackColor directly rendered
                // *unselected* cards in the primary colour under MaterialDesignTheme, so the tabs
                // looked inverted — every unselected tab appeared selected.
                Color resolved = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabThemeHelpers
                    .GetTabBackgroundColor(Theme, itemLayout.Item.IsSelected, itemLayout.Item.IsHovered);

                // Every tab is a card, not just the selected one — that is what separates this style
                // from Classic, where only the selected tab has a body at all. An unselected card
                // whose fill matches the strip is invisible, so nudge it away from the header.
                if (!itemLayout.Item.IsSelected)
                {
                    Color header = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabThemeHelpers
                        .GetHeaderBackgroundColor(Theme);
                    if (Math.Abs(resolved.R - header.R) + Math.Abs(resolved.G - header.G)
                        + Math.Abs(resolved.B - header.B) <= 24)
                    {
                        float shift = header.GetBrightness() > 0.5f ? -0.07f : 0.10f;
                        resolved = Color.FromArgb(
                            header.A,
                            (int)(Math.Clamp(header.R / 255f + shift, 0f, 1f) * 255),
                            (int)(Math.Clamp(header.G / 255f + shift, 0f, 1f) * 255),
                            (int)(Math.Clamp(header.B / 255f + shift, 0f, 1f) * 255));
                    }
                }

                Color fillColor = Color.FromArgb(a, resolved);

                var brush = PaintersFactory.GetSolidBrush(fillColor);
                g.FillPath(brush, path);

                var pen = PaintersFactory.GetPen(borderColor);
                g.DrawPath(pen, path);

                if (itemLayout.Item.IsSelected)
                {
                    // Accent stripe along the card's top edge, and a merge line along the bottom so
                    // the selected card joins the content area.
                    Color accent = Color.FromArgb(a, TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
                        .TabThemeHelpers.GetTabIndicatorColor(Theme));
                    var accentBrush = PaintersFactory.GetSolidBrush(accent);
                    g.FillRectangle(accentBrush, drawRect.X + Scale(2), drawRect.Y,
                        drawRect.Width - Scale(4), Scale(3));

                    var mergePen = PaintersFactory.GetPen(fillColor);
                    g.DrawLine(mergePen, drawRect.Left + 1, drawRect.Bottom, drawRect.Right - 1, drawRect.Bottom);
                }
            }

            DrawTabItemContent(g, itemLayout, alpha);
        }
    }
}
