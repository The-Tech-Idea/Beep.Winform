using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace TheTechIdea.Beep.Winform.Controls.Toolstrips.Painters
{
    // 1) Thick underline variant (Option 5-like)
    internal sealed class ThickUnderlineTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsThickUnderline);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            var tc = selected ? theme.MenuMainItemSelectedForeColor : theme.MenuMainItemForeColor;

            Rectangle iconRect = Rectangle.Empty; Rectangle textRect = rect;
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                if (iconPlacement.Equals("Left", System.StringComparison.OrdinalIgnoreCase)) { iconRect = new Rectangle(rect.Left + padH, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize); textRect = new Rectangle(iconRect.Right + iconGap, rect.Top, rect.Right - (iconRect.Right + iconGap) - padH, rect.Height); }
                else { iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + padV, iconSize, iconSize); textRect = new Rectangle(rect.Left + padH, iconRect.Bottom + iconGap, rect.Width - padH * 2, rect.Bottom - (iconRect.Bottom + iconGap) - padV); }
                DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconSize, out _);
            }
            TextRenderer.DrawText(g, text, font, textRect, tc, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            if (selected)
            {
                int underline = GetUnderline(owner.Parameters, "Underline", 6);
                using var fill = new SolidBrush(tc);
                var underlineRect = new Rectangle(rect.Left + 10, rect.Bottom - underline, rect.Width - 20, underline);
                using var path = new GraphicsPath();
                path.AddRectangle(underlineRect);
                g.FillPath(fill, path);
            }
        }
    }

    // 2) Dot indicator variant (Option 9-like)
    internal sealed class DotIndicatorTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsDotIndicator);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            var tc = selected ? theme.MenuMainItemSelectedForeColor : theme.MenuMainItemForeColor;

            Rectangle iconRect = Rectangle.Empty; Rectangle textRect = rect;
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                if (iconPlacement.Equals("Left", System.StringComparison.OrdinalIgnoreCase)) { iconRect = new Rectangle(rect.Left + padH, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize); textRect = new Rectangle(iconRect.Right + iconGap, rect.Top, rect.Right - (iconRect.Right + iconGap) - padH, rect.Height); }
                else { iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + padV, iconSize, iconSize); textRect = new Rectangle(rect.Left + padH, iconRect.Bottom + iconGap, rect.Width - padH * 2, rect.Bottom - (iconRect.Bottom + iconGap) - padV); }
                DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconSize, out _);
            }
            TextRenderer.DrawText(g, text, font, textRect, tc, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            if (selected)
            {
                int dot = System.Math.Max(4, GetUnderline(owner.Parameters, "DotSize", 6));
                using var fill = new SolidBrush(tc);
                var center = new Rectangle(rect.Left + (rect.Width - dot) / 2, rect.Bottom - dot - 2, dot, dot);
                g.FillEllipse(fill, center);
            }
        }
    }

    // 3) Icon-only variant (for compact toolbars)
    internal sealed class IconOnlyTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsIconOnly);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            int radius = GetCorner(owner.Parameters, "CornerRadius", 8);
            using var path = RoundedRect(rect, radius);
            using var fill = new SolidBrush(selected ? theme.MenuItemSelectedBackColor : Color.Transparent);
            using var brd = new Pen(theme.BorderColor, selected ? 1 : 0.5f);
            if (selected) g.FillPath(fill, path);
            g.DrawPath(brd, path);

            Rectangle iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize);
            DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconSize, out _);
        }
    }

    // 4) Ghost tabs (outlined, hover filled lightly)
    internal sealed class GhostTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsGhost);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            int radius = GetCorner(owner.Parameters, "CornerRadius", 8);
            using var path = RoundedRect(rect, radius);
            using var brd = new Pen(theme.BorderColor, 1);
            g.DrawPath(brd, path);

            var tc = selected ? theme.MenuMainItemSelectedForeColor : theme.MenuMainItemForeColor;
            Rectangle iconRect = Rectangle.Empty; Rectangle textRect = rect;
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                if (iconPlacement.Equals("Left", StringComparison.OrdinalIgnoreCase)) { iconRect = new Rectangle(rect.Left + padH, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize); textRect = new Rectangle(iconRect.Right + iconGap, rect.Top, rect.Right - (iconRect.Right + iconGap) - padH, rect.Height); }
                else { iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + padV, iconSize, iconSize); textRect = new Rectangle(rect.Left + padH, iconRect.Bottom + iconGap, rect.Width - padH * 2, rect.Bottom - (iconRect.Bottom + iconGap) - padV); }
                DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconSize, out _);
            }
            TextRenderer.DrawText(g, text, font, textRect, tc, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
    }
}
