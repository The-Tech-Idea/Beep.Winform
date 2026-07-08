using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Navigation-rail style list painter.
    /// Renders each item as a large icon centred above a compact label — like a mobile bottom-nav
    /// or a left-rail icon menu.
    /// Items are stacked vertically; width should be ≤ 72 px for typical usage.
    /// </summary>
    internal class NavigationRailListBoxPainter : BaseListBoxPainter
    {
        private const int RailItemHeight = 72; // px logical

        public override int GetPreferredItemHeight()
            => DpiScalingHelper.ScaleValue(RailItemHeight, _owner ?? new System.Windows.Forms.Control());

        public override void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            base.Paint(g, owner, drawingRect);
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int iconSz = DpiScalingHelper.ScaleValue(32, _owner);
            int labelH  = DpiScalingHelper.ScaleValue(14, _owner);
            int gap     = DpiScalingHelper.ScaleValue(4, _owner);
            int total   = iconSz + gap + labelH;

            int startY = itemRect.Top + (itemRect.Height - total) / 2;

            // Selected: filled pill behind icon
            if (isSelected)
            {
                int pillW = DpiScalingHelper.ScaleValue(56, _owner);
                int pillH = DpiScalingHelper.ScaleValue(32, _owner);
                var pillRect = new Rectangle(
                    itemRect.Left + (itemRect.Width - pillW) / 2,
                    startY,
                    pillW, pillH);
                using var path = RoundedRect(pillRect, pillH / 2);
                g.FillPath(GetBrush(
                    Color.FromArgb(40, _theme?.PrimaryColor ?? Color.DodgerBlue)), path);
            }

            // Icon
            var iconRect = new Rectangle(
                itemRect.Left + (itemRect.Width - iconSz) / 2,
                startY,
                iconSz, iconSz);

            if (!string.IsNullOrEmpty(item.ImagePath))
                DrawItemImage(g, iconRect, item.ImagePath);
            else
            {
                // Fallback: initials circle
                g.FillEllipse(GetBrush(
                    Color.FromArgb(120, _theme?.PrimaryColor ?? Color.DodgerBlue)), iconRect);
                string initials = item.Text?.Length > 0 ? item.Text.Substring(0, 1).ToUpper() : "?";
                var iFnt = GetCachedFont(14f, FontStyle.Bold);
                TextRenderer.DrawText(g, initials, iFnt, iconRect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // Label
            var labelRect = new Rectangle(itemRect.Left, iconRect.Bottom + gap, itemRect.Width, labelH);
            Color textColor = isSelected
                ? (_theme?.PrimaryColor ?? Color.DodgerBlue)
                : Color.FromArgb(ListBoxTokens.SubTextAlpha, _theme?.ListForeColor ?? Color.Gray);
            var lFont = GetCachedFont(Math.Max(7.5f, _owner.TextFont.Size - 1.5f),
                isSelected ? FontStyle.Bold : FontStyle.Regular);
            TextRenderer.DrawText(g, item.Text ?? "", lFont, labelRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

            // Active indicator: 3 px bar on left edge
            if (isSelected)
            {
                int barH = DpiScalingHelper.ScaleValue(20, _owner);
                var barRect = new Rectangle(itemRect.Left,
                    itemRect.Top + (itemRect.Height - barH) / 2,
                    ListBoxTokens.PinnedAccentBarWidth, barH);
                using var barPath = RoundedRect(barRect, barRect.Width / 2);
                g.FillPath(GetBrush(_theme?.PrimaryColor ?? Color.DodgerBlue), barPath);
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        public override bool SupportsSearch()     => false;
        public override bool SupportsCheckboxes() => false;
    }
}
