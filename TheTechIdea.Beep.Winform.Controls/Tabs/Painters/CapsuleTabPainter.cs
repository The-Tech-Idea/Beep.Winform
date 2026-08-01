using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    /// <summary>
    /// A floating pill: only the selected tab is filled, as a fully-rounded capsule inset on all
    /// sides so it never touches its neighbours or the strip edges — the Material / Ant Design pill
    /// pattern. Unselected tabs carry no chrome at all.
    /// </summary>
    /// <remarks>
    /// The inset and the absence of any unselected fill are what distinguish this from
    /// <see cref="ClassicTabPainter"/>. Previously the only difference was a corner radius, which
    /// the contact sheet measured at ~1% of pixels — indistinguishable in use.
    /// </remarks>
    public class CapsuleTabPainter : BaseTabPainter
    {
        // Design-time pixels; scaled per display via BaseTabPainter.Scale.
        private const int InsetX = 3;
        private const int InsetY = 4;

        public CapsuleTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTabItem(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            BeepTabItem item = itemLayout.Item;
            Rectangle bounds = itemLayout.Bounds;
            if (bounds.IsEmpty) return;

            var pill = Rectangle.Inflate(bounds, -Scale(InsetX), -Scale(InsetY));
            bool drawsPill = (item.IsSelected || item.IsHovered) && pill.Width > 0 && pill.Height > 0;

            if (drawsPill)
            {
                int a = (int)(Math.Clamp(alpha, 0f, 1f) * 255f);
                Color fill = TabThemeHelpers.GetTabBackgroundColor(
                    Theme, Theme != null, item.IsSelected, item.IsHovered);

                // Hover is a hint, not a selection: same shape, much lighter.
                int fillAlpha = item.IsSelected ? a : (int)(a * 0.30f);

                int radius = (int)Math.Min(pill.Height / 2f, Scale(20));
                using (GraphicsPath path = GetRoundedRect(pill, radius))
                {
                    var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(fillAlpha, fill));
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillPath(brush, path);
                }
            }

            DrawTabItemContent(g, itemLayout, alpha);
        }

        /// <summary>The pill is what the label sits on when selected; otherwise it is the header.</summary>
        protected override Color GetTabSurfaceColor(BeepTabItem item)
        {
            return item.IsSelected
                ? TabThemeHelpers.GetTabBackgroundColor(Theme, Theme != null, true, false)
                : TabThemeHelpers.GetHeaderBackgroundColor(Theme, Theme != null);
        }
    }
}
