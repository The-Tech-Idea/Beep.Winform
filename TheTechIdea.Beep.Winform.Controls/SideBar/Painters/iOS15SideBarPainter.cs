using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    public sealed class iOS15SideBarPainter : BaseSideBarPainter
    {
        private static readonly ImagePainter _imagePainter = new ImagePainter();
        public override string Name => "iOS15";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            Color backColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuBackColor : Color.FromArgb(250, 250, 250);
            using (var brush = new SolidBrush(Color.FromArgb(245, backColor))) { g.FillRectangle(brush, bounds); }
            Color separatorColor = context.UseThemeColors && context.Theme != null ? context.Theme.BorderColor : Color.FromArgb(220, 220, 220);
            using (var pen = new Pen(separatorColor, 1f)) { g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); }
            int padding = 12, currentY = bounds.Top + padding;
            if (context.ShowToggleButton) { Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); PaintToggleButton(g, toggleRect, context); currentY += context.ItemHeight + 8; }
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color buttonColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(0, 122, 255);
            using (var path = CreateRoundedPath(toggleRect, 10)) using (var brush = new SolidBrush(buttonColor)) { g.FillPath(brush, path); }
            Color iconColor = Color.White;
            using (var pen = new Pen(iconColor, 2.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                int centerX = toggleRect.X + toggleRect.Width / 2, centerY = toggleRect.Y + toggleRect.Height / 2, lineWidth = 18;
                g.DrawLine(pen, centerX - lineWidth / 2, centerY - 6, centerX + lineWidth / 2, centerY - 6);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY, centerX + lineWidth / 2, centerY);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY + 6, centerX + lineWidth / 2, centerY + 6);
            }
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color selectionColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(0, 122, 255);
            using (var path = CreateRoundedPath(itemRect, 10)) using (var brush = new SolidBrush(Color.FromArgb(200, selectionColor))) { g.FillPath(brush, path); }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            using (var path = CreateRoundedPath(itemRect, 10)) using (var brush = new SolidBrush(Color.FromArgb(20, 0, 0, 0))) { g.FillPath(brush, path); }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            int padding = 12, iconSize = 24, iconPadding = 8;
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                int x = itemRect.X + iconPadding;
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = GetIconPath(item, context);
                    if (context.Theme != null && context.UseThemeColors) { _imagePainter.CurrentTheme = context.Theme; _imagePainter.ApplyThemeOnImage = true; _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; }
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(60, 60, 67));
                    using (var font = new Font("SF Pro Display", 13f, FontStyle.Regular)) using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - 8, itemRect.Height);
                        StringFormat format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                        g.DrawString(item.Text, font, brush, textRect, format);
                    }
                }
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - 20, itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuForeColor : Color.FromArgb(142, 142, 147);
                    using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    {
                        int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                        if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2); }
                        else { g.DrawLine(pen, cx - 2, cy - 4, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 2, cy + 4); }
                    }
                }
                currentY += context.ItemHeight + 4;
                if (item.Children != null && item.Children.Count > 0 && context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) { PaintChildItems(g, bounds, context, item, ref currentY, 1); }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            int padding = 4, indent = context.IndentationWidth * indentLevel, iconSize = 18, iconPadding = 6;
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                int x = childRect.X + iconPadding;
                Color lineColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(40, context.Theme.SideMenuForeColor) : Color.FromArgb(40, 142, 142, 147);
                using (var pen = new Pen(lineColor, 1f)) { int lineX = bounds.Left + (indent / 2); g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2); }
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = GetIconPath(child, context);
                    if (context.Theme != null && context.UseThemeColors) { _imagePainter.CurrentTheme = context.Theme; _imagePainter.ApplyThemeOnImage = true; _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; }
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                Color textColor = context.UseThemeColors && context.Theme != null ? (child == context.SelectedItem ? Color.White : Color.FromArgb(200, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)) : (child == context.SelectedItem ? Color.White : Color.FromArgb(142, 142, 147));
                using (var font = new Font("SF Pro Text", 11.5f, FontStyle.Regular)) using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, childRect.Right - x - 6, childRect.Height);
                    StringFormat format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.DrawString(child.Text, font, brush, textRect, format);
                }
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - 18, childRect.Y + (childRect.Height - 14) / 2, 14, 14);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuForeColor : Color.FromArgb(142, 142, 147);
                    using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    {
                        int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                        if (isExpanded) { g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1); }
                        else { g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3); }
                    }
                }
                currentY += context.ChildItemHeight + 2;
                if (child.Children != null && child.Children.Count > 0 && context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) { PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); }
            }
        }
    }
}
