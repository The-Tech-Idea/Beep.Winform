using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Toolstrips.Painters
{
    internal sealed class UnderlineTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsUnderline);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            var (iconRect, textRect) = GetIconTextRects(rect, item, iconPlacement, iconSize, iconGap, padH, padV);
            DrawIconAndText(g, theme, owner, item, selected, string.IsNullOrEmpty(item.Text) ? item.Name : item.Text, font, iconRect, textRect);

            if (selected)
            {
                int underline = GetUnderline(owner.Parameters, "Underline", 2);
                using var pen = new Pen(theme.MenuMainItemSelectedForeColor, underline);
                g.DrawLine(pen, rect.Left + 8, rect.Bottom - underline, rect.Right - 8, rect.Bottom - underline);
            }
        }
    }

    internal sealed class PillTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsPill);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            using var path = RoundedRect(rect, GetCorner(owner.Parameters, "CornerRadius", 999));
            using var fill = new SolidBrush(selected ? theme.MenuMainItemSelectedBackColor : theme.MenuMainItemHoverBackColor);
            using var brd = new Pen(selected ? theme.MenuItemSelectedBackColor : theme.MenuItemHoverBackColor);
            g.FillPath(fill, path); g.DrawPath(brd, path);

            var (iconRect, textRect) = GetIconTextRects(rect, item, iconPlacement, iconSize, iconGap, padH, padV);
            DrawIconAndText(g, theme, owner, item, selected, string.IsNullOrEmpty(item.Text) ? item.Name : item.Text, font, iconRect, textRect);
        }
    }

    internal sealed class OutlineTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsOutline);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            using var path = RoundedRect(rect, GetCorner(owner.Parameters, "CornerRadius", 8));
            using var brd = new Pen(theme.BorderColor, 1);
            g.DrawPath(brd, path);
            bool selected = index == owner.SelectedIndex;

            var (iconRect, textRect) = GetIconTextRects(rect, item, iconPlacement, iconSize, iconGap, padH, padV);
            DrawIconAndText(g, theme, owner, item, selected, string.IsNullOrEmpty(item.Text) ? item.Name : item.Text, font, iconRect, textRect);
        }
    }

    internal sealed class SegmentedTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsSegmented);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            using var brd = new Pen(theme.BorderColor, 1);
            g.DrawRectangle(brd, rect);
            bool selected = index == owner.SelectedIndex;
            if (selected)
            {
                using var fill = new SolidBrush(theme.MenuItemSelectedBackColor);
                g.FillRectangle(fill, Rectangle.Inflate(rect, -1, -1));
            }
            var (iconRect, textRect) = GetIconTextRects(rect, item, iconPlacement, iconSize, iconGap, padH, padV);
            DrawIconAndText(g, theme, owner, item, selected, string.IsNullOrEmpty(item.Text) ? item.Name : item.Text, font, iconRect, textRect);
        }
    }

    internal sealed class FilledTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsFilled);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            using var path = RoundedRect(rect, GetCorner(owner.Parameters, "CornerRadius", 8));
            using var fill = new SolidBrush(selected ? theme.MenuItemSelectedBackColor : theme.MenuItemHoverBackColor);
            g.FillPath(fill, path);
            var (iconRect, textRect) = GetIconTextRects(rect, item, iconPlacement, iconSize, iconGap, padH, padV);
            DrawIconAndText(g, theme, owner, item, selected, string.IsNullOrEmpty(item.Text) ? item.Name : item.Text, font, iconRect, textRect);
        }
    }

    internal sealed class MinimalUnderlineTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.MinimalUnderline);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            var (iconRect, textRect) = GetIconTextRects(rect, item, iconPlacement, iconSize, iconGap, padH, padV);
            DrawIconAndText(g, theme, owner, item, selected, string.IsNullOrEmpty(item.Text) ? item.Name : item.Text, font, iconRect, textRect);

            if (selected)
            {
                int underline = GetUnderline(owner.Parameters, "Underline", 2);
                using var fill = new SolidBrush(theme.MenuMainItemSelectedForeColor);
                var underlineRect = new Rectangle(rect.Left + 10, rect.Bottom - underline - 1, rect.Width - 20, underline);
                g.FillRectangle(fill, underlineRect);
            }
        }
    }
}
