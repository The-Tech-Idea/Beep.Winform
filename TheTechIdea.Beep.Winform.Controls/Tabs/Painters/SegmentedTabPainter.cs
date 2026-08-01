using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    /// <summary>
    /// A segmented control: the whole run sits inside one recessed, bordered track; segments are
    /// separated by thin dividers; only the selected segment is raised as a filled tile.
    /// </summary>
    /// <remarks>
    /// The track is drawn in <see cref="PaintHeaderBackground"/> because it spans the entire run
    /// rather than any single tab — this style is the reason that member is on the painter contract
    /// at all. Previously this painter was <see cref="ClassicTabPainter"/> with a radius of 6, which
    /// the contact sheet measured at 0.2–0.4% pixel difference.
    /// </remarks>
    public class SegmentedTabPainter : BaseTabPainter
    {
        // Design-time pixels; scaled per display via BaseTabPainter.Scale.
        private const int TrackInset = 3;
        private const int SegmentInset = 2;

        public SegmentedTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintHeaderBackground(Graphics g, Rectangle headerBounds)
        {
            base.PaintHeaderBackground(g, headerBounds);
            if (headerBounds.Width <= 0 || headerBounds.Height <= 0) return;

            var track = Rectangle.Inflate(headerBounds, -Scale(TrackInset), -Scale(TrackInset));
            if (track.Width <= 0 || track.Height <= 0) return;

            // A visibly recessed track: shifted away from the header so the whole run reads as one
            // inset control. Taking the plain tab background made the strip almost identical to
            // Capsule's, which draws no track at all.
            Color header = TabThemeHelpers.GetHeaderBackgroundColor(Theme, Theme != null);
            Color trackFill = ShiftToward(header, header.GetBrightness() > 0.5f ? -0.10f : 0.12f);
            Color trackBorder = TabThemeHelpers.GetTabBorderColor(Theme, Theme != null, false, false);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = GetRoundedRect(track, Scale(8)))
            {
                var brush = PaintersFactory.GetSolidBrush(trackFill);
                g.FillPath(brush, path);
                var pen = PaintersFactory.GetPen(trackBorder);
                g.DrawPath(pen, path);
            }
        }

        public override void PaintTabItem(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            BeepTabItem item = itemLayout.Item;
            Rectangle bounds = itemLayout.Bounds;
            if (bounds.IsEmpty) return;

            int a = (int)(Math.Clamp(alpha, 0f, 1f) * 255f);

            if (item.IsSelected)
            {
                var tile = Rectangle.Inflate(bounds, -Scale(SegmentInset), -Scale(TrackInset + SegmentInset));
                if (tile.Width > 0 && tile.Height > 0)
                {
                    Color fill = Color.FromArgb(a,
                        TabThemeHelpers.GetTabBackgroundColor(Theme, Theme != null, true, false));
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using GraphicsPath path = GetRoundedRect(tile, Scale(6));
                    var brush = PaintersFactory.GetSolidBrush(fill);
                    g.FillPath(brush, path);
                }
            }
            else
            {
                // Divider between segments, inset from the track edges.
                Color divider = TabThemeHelpers.GetTabBorderColor(Theme, Theme != null, false, false);
                var pen = PaintersFactory.GetPen(Color.FromArgb((int)(a * 0.55f), divider));
                g.DrawLine(pen,
                    bounds.Right - 1, bounds.Y + Scale(TrackInset + 4),
                    bounds.Right - 1, bounds.Bottom - Scale(TrackInset + 4));
            }

            DrawTabItemContent(g, itemLayout, alpha);
        }

        private static Color ShiftToward(Color color, float shift)
        {
            return Color.FromArgb(
                color.A,
                (int)(Math.Clamp(color.R / 255f + shift, 0f, 1f) * 255),
                (int)(Math.Clamp(color.G / 255f + shift, 0f, 1f) * 255),
                (int)(Math.Clamp(color.B / 255f + shift, 0f, 1f) * 255));
        }

        /// <summary>Unselected labels sit on the recessed track, not on a tab fill.</summary>
        protected override Color GetTabSurfaceColor(BeepTabItem item)
        {
            return TabThemeHelpers.GetTabBackgroundColor(Theme, Theme != null, item.IsSelected, false);
        }
    }
}
