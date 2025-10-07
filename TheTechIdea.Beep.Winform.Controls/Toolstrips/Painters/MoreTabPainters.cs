using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Toolstrips.Painters
{
    // 1) Segmented with equal widths (fills available space evenly)
    internal sealed class SegmentedEqualTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsSegmentedEqual);
        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepToolStrip owner, IReadOnlyDictionary<string, object> parameters)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var bg = new SolidBrush(owner.BackColor);
            g.FillRectangle(bg, bounds);
            if (owner.Buttons == null || owner.Buttons.Count == 0) return;

            int spacing = GetInt(parameters, "Spacing", DefaultSpacing);
            int padH = GetInt(parameters, "ItemPaddingH", DefaultItemPaddingH);
            int padV = GetInt(parameters, "ItemPaddingV", DefaultItemPaddingV);
            int iconSize = GetInt(parameters, "IconSize", DefaultIconSize);
            int iconGap = GetInt(parameters, "IconGap", DefaultIconGap);
            string iconPlacement = GetString(parameters, "IconPlacement", DefaultIconPlacement);

            using var font =  BeepThemesManager.ToFont(theme.ComboBoxItemFont);

            if (owner.Orientation == ToolStripOrientation.Vertical)
            {
                int available = bounds.Height - spacing * (owner.Buttons.Count + 1);
                int piece = available / owner.Buttons.Count;
                int x = bounds.Left + spacing;
                int y = bounds.Top + spacing;
                for (int i = 0; i < owner.Buttons.Count; i++)
                {
                    var rect = new Rectangle(x, y, bounds.Width - spacing * 2, piece);
                    DrawOne(g, rect, theme, owner, owner.Buttons[i], i, font, padH, padV, spacing, iconSize, iconGap, iconPlacement);
                    y += piece + spacing;
                }
            }
            else
            {
                int available = bounds.Width - spacing * (owner.Buttons.Count + 1);
                int piece = System.Math.Max(20, available / owner.Buttons.Count);
                int x = bounds.Left + spacing;
                for (int i = 0; i < owner.Buttons.Count; i++)
                {
                    var rect = new Rectangle(x, bounds.Top + spacing, piece, bounds.Height - spacing * 2);
                    DrawOne(g, rect, theme, owner, owner.Buttons[i], i, font, padH, padV, spacing, iconSize, iconGap, iconPlacement);
                    x += piece + spacing;
                }
            }
        }

        private void DrawOne(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, System.Drawing.Font font, int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            using var brd = new Pen(theme.BorderColor, 1);
            g.DrawRectangle(brd, rect);
            bool selected = index == owner.SelectedIndex;
            if (selected)
            {
                using var fill = new SolidBrush(theme.MenuItemSelectedBackColor);
                g.FillRectangle(fill, Rectangle.Inflate(rect, -1, -1));
            }
            var tc = selected ? theme.MenuItemSelectedForeColor : theme.MenuItemForeColor;
            Rectangle iconRect = Rectangle.Empty; Rectangle textRect = rect;
            string text = string.IsNullOrEmpty(item?.Text) ? item?.Name ?? string.Empty : item.Text;
            if (item != null && !string.IsNullOrEmpty(item.ImagePath))
            {
                if (iconPlacement.Equals("Left", System.StringComparison.OrdinalIgnoreCase))
                { iconRect = new Rectangle(rect.Left + padH, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize); textRect = new Rectangle(iconRect.Right + iconGap, rect.Top, rect.Right - (iconRect.Right + iconGap) - padH, rect.Height); }
                else
                { iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + padV, iconSize, iconSize); textRect = new Rectangle(rect.Left + padH, iconRect.Bottom + iconGap, rect.Width - padH * 2, rect.Bottom - (iconRect.Bottom + iconGap) - padV); }
                DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconSize, out _);
            }
            TextRenderer.DrawText(g, text, font, textRect, tc, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, System.Drawing.Font font, int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement) { }
    }

    // 2) Shadow only on selected tabs
    internal sealed class SelectedShadowTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsSelectedShadow);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            if (selected)
            {
                var shadowRect = new Rectangle(rect.X + 2, rect.Y + 3, rect.Width, rect.Height);
                using var pathShadow = RoundedRect(shadowRect, GetCorner(owner.Parameters, "CornerRadius", 8));
                using var shadow = new SolidBrush(Color.FromArgb(60, theme.ShadowColor));
                g.FillPath(shadow, pathShadow);
            }

            using var path = RoundedRect(rect, GetCorner(owner.Parameters, "CornerRadius", 8));
            using var fill = new SolidBrush(selected ? theme.MenuItemSelectedBackColor : theme.MenuItemHoverBackColor);
            using var border = new Pen(theme.BorderColor, 1);
            g.FillPath(fill, path);
            g.DrawPath(border, path);

            Rectangle iconRect = Rectangle.Empty; Rectangle textRect = rect;
            if (item != null && !string.IsNullOrEmpty(item.ImagePath))
            {
                if (iconPlacement.Equals("Left", System.StringComparison.OrdinalIgnoreCase))
                { iconRect = new Rectangle(rect.Left + padH, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize); textRect = new Rectangle(iconRect.Right + iconGap, rect.Top, rect.Right - (iconRect.Right + iconGap) - padH, rect.Height); }
                else
                { iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + padV, iconSize, iconSize); textRect = new Rectangle(rect.Left + padH, iconRect.Bottom + iconGap, rect.Width - padH * 2, rect.Bottom - (iconRect.Bottom + iconGap) - padV); }
                DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconSize, out _);
            }
            var tc = selected ? theme.MenuItemSelectedForeColor : theme.MenuItemForeColor;
            TextRenderer.DrawText(g, text, font, textRect, tc, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
    }

    // 3) Rounded underline for selected
    internal sealed class RoundedUnderlineTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsRoundedUnderline);
        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;

            Rectangle iconRect = Rectangle.Empty; Rectangle textRect = rect;
            if (item != null && !string.IsNullOrEmpty(item.ImagePath))
            {
                if (iconPlacement.Equals("Left", System.StringComparison.OrdinalIgnoreCase))
                { iconRect = new Rectangle(rect.Left + padH, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize); textRect = new Rectangle(iconRect.Right + iconGap, rect.Top, rect.Right - (iconRect.Right + iconGap) - padH, rect.Height); }
                else
                { iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + padV, iconSize, iconSize); textRect = new Rectangle(rect.Left + padH, iconRect.Bottom + iconGap, rect.Width - padH * 2, rect.Bottom - (iconRect.Bottom + iconGap) - padV); }
                DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconSize, out _);
            }
            var tc = selected ? theme.MenuMainItemSelectedForeColor : theme.MenuMainItemForeColor;
            TextRenderer.DrawText(g, text, font, textRect, tc, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            if (selected)
            {
                int underline = GetUnderline(owner.Parameters, "Underline", 4);
                int radius = underline; // pill-like
                var ul = new Rectangle(rect.Left + 12, rect.Bottom - underline - 1, rect.Width - 24, underline);
                using var path = new GraphicsPath();
                int d = radius * 2;
                path.AddArc(ul.Left, ul.Top, d, d, 90, 180);
                path.AddArc(ul.Right - d, ul.Top, d, d, 270, 180);
                path.CloseAllFigures();
                using var fill = new SolidBrush(tc);
                g.FillPath(fill, path);
            }
        }
    }
}
