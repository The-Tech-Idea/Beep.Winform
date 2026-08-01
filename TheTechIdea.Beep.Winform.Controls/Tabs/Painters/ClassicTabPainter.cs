using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    /// <summary>
    /// The notebook tab: the selected tab is a raised sheet whose bottom edge is open so it merges
    /// into the content area below, and unselected tabs are unfilled with a hairline divider
    /// between them — the VS / browser classic look.
    /// </summary>
    /// <remarks>
    /// Classic, Capsule and Segmented were previously the same method with a different corner
    /// radius. The contact sheet measured them at 0.2–1.4% pixel difference: the same tab three
    /// times. What separates this style is the open bottom edge and the divider, not a radius.
    /// </remarks>
    public class ClassicTabPainter : BaseTabPainter
    {
        public ClassicTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTabItem(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            BeepTabItem item = itemLayout.Item;
            Rectangle bounds = itemLayout.Bounds;
            if (bounds.IsEmpty) return;

            int a = (int)(Math.Clamp(alpha, 0f, 1f) * 255f);
            Color border = Color.FromArgb(a, TabThemeHelpers.GetTabBorderColor(Theme, Theme != null, item.IsSelected));

            if (item.IsSelected)
            {
                Color fill = Color.FromArgb(a,
                    TabThemeHelpers.GetTabBackgroundColor(Theme, Theme != null, true, false));

                // A sheet taller than the strip, so its bottom edge falls outside the clip and the
                // tab reads as continuous with the content area below it.
                var sheet = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height + Scale(4));
                using (GraphicsPath path = GetRoundedRect(sheet, Scale(5)))
                {
                    var brush = PaintersFactory.GetSolidBrush(fill);
                    g.FillPath(brush, path);
                    var pen = PaintersFactory.GetPen(border);
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                if (item.IsHovered)
                {
                    Color hover = Color.FromArgb((int)(a * 0.35f),
                        TabThemeHelpers.GetTabBackgroundColor(Theme, Theme != null, false, true));
                    var brush = PaintersFactory.GetSolidBrush(hover);
                    g.FillRectangle(brush, bounds);
                }

                // Hairline divider on the trailing edge: the run reads as labels, not buttons.
                var dividerPen = PaintersFactory.GetPen(Color.FromArgb((int)(a * 0.45f), border));
                g.DrawLine(dividerPen,
                    bounds.Right - 1, bounds.Y + Scale(6),
                    bounds.Right - 1, bounds.Bottom - Scale(6));
            }

            DrawTabItemContent(g, itemLayout, alpha);
        }
    }
}
