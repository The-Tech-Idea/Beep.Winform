using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Toolstrips.Painters
{
    // Connected segmented group: draws one rounded container and internal dividers.
    internal sealed class ConnectedSegmentedTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsConnectedSegmented);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepToolStrip owner, IReadOnlyDictionary<string, object> parameters)
        {
            base.Paint(g, bounds, theme, owner, parameters); // background

            if (owner.Buttons == null || owner.Buttons.Count == 0) return;
            int padH = GetInt(parameters, "ItemPaddingH", DefaultItemPaddingH);
            int padV = GetInt(parameters, "ItemPaddingV", DefaultItemPaddingV);
            int spacing = GetInt(parameters, "Spacing", DefaultSpacing);
            int iconSize = GetInt(parameters, "IconSize", DefaultIconSize);
            int iconGap = GetInt(parameters, "IconGap", DefaultIconGap);
            string iconPlacement = GetString(parameters, "IconPlacement", DefaultIconPlacement);
            int corner = GetCorner(parameters, "CornerRadius", DefaultCornerRadius);

            using var font = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.ComboBoxItemFont);
            var itemRects = owner.GetItemRects(bounds, font, padH, padV, spacing, iconSize, iconGap, iconPlacement);
            if (itemRects.Count == 0) return;

            // Container rect spans all items without external spacing between
            var groupRect = Rectangle.Union(itemRects[0], itemRects[itemRects.Count - 1]);
            using var groupPath = RoundedRect(groupRect, corner);
            using var groupBack = new SolidBrush(theme.MenuItemHoverBackColor);
            using var groupBorder = new Pen(theme.BorderColor, 1);
            g.FillPath(groupBack, groupPath);
            g.DrawPath(groupBorder, groupPath);

            // Internal separators
            for (int i = 0; i < itemRects.Count - 1; i++)
            {
                var r = itemRects[i];
                if (owner.Orientation == ToolStripOrientation.Vertical)
                {
                    int y = r.Bottom;
                    using var p = new Pen(theme.BorderColor, 1);
                    g.DrawLine(p, groupRect.Left, y, groupRect.Right, y);
                }
                else
                {
                    int x = r.Right;
                    using var p = new Pen(theme.BorderColor, 1);
                    g.DrawLine(p, x, groupRect.Top, x, groupRect.Bottom);
                }
            }

            // Draw each item content with selection fill
            for (int i = 0; i < itemRects.Count; i++)
            {
                var rect = itemRects[i];
                var item = owner.Buttons[i];
                bool selected = i == owner.SelectedIndex;

                if (selected)
                {
                    using var sel = new SolidBrush(theme.MenuItemSelectedBackColor);
                    g.FillRectangle(sel, Rectangle.Inflate(rect, -1, -1));
                }

                string text = string.IsNullOrEmpty(item.Text) ? item.Name : item.Text;
                var tc = selected ? theme.MenuItemSelectedForeColor : theme.MenuItemForeColor;

                // position icon+text
                Rectangle iconRect = Rectangle.Empty; Rectangle textRect = rect;
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    if (iconPlacement.Equals("Left", System.StringComparison.OrdinalIgnoreCase))
                    { iconRect = new Rectangle(rect.Left + padH, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize); textRect = new Rectangle(iconRect.Right + iconGap, rect.Top, rect.Right - (iconRect.Right + iconGap) - padH, rect.Height); }
                    else
                    { iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + padV, iconSize, iconSize); textRect = new Rectangle(rect.Left + padH, iconRect.Bottom + iconGap, rect.Width - padH * 2, rect.Bottom - (iconRect.Bottom + iconGap) - padV); }
                    DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconSize, out _);
                }
                TextRenderer.DrawText(g, text, font, textRect, tc, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
        }

        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, System.Drawing.Font font, int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            // not used; Paint override draws whole group
        }
    }

    // Elevated pill with shadow
    internal sealed class ElevatedPillTabsPainter : TabsBasePainter
    {
        public override string Key => nameof(ToolStripPainterKind.TabsElevatedPill);

        protected override void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            bool selected = index == owner.SelectedIndex;
            int radius = GetCorner(owner.Parameters, "CornerRadius", 18);

            // Shadow (simple drop shadow)
            var shadowRect = new Rectangle(rect.X + 2, rect.Y + 3, rect.Width, rect.Height);
            using (var pathShadow = RoundedRect(shadowRect, radius))
            using (var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            {
                g.FillPath(shadow, pathShadow);
            }

            // Pill background
            using var path = RoundedRect(rect, radius);
            using var fill = new SolidBrush(selected ? theme.MenuMainItemSelectedBackColor : theme.MenuMainItemHoverBackColor);
            using var border = new Pen(selected ? theme.MenuItemSelectedBackColor : theme.BorderColor, 1);
            g.FillPath(fill, path);
            g.DrawPath(border, path);

            // Icon + text
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
        }
    }
}
