using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    public sealed class DarkGlowSideBarPainter : BaseSideBarPainter
    {
        private static readonly ImagePainter _imagePainter = new ImagePainter();
        public override string Name => "DarkGlow";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            Color backColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuBackColor : Color.FromArgb(17, 24, 39);
            using (var brush = new SolidBrush(backColor)) { g.FillRectangle(brush, bounds); }
            Color borderColor = context.UseThemeColors && context.Theme != null ? context.Theme.BorderColor : Color.FromArgb(31, 41, 55);
            using (var pen = new Pen(borderColor, 1f)) { g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); }
            int padding = 12, currentY = bounds.Top + padding;
            if (context.ShowToggleButton) { Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); PaintToggleButton(g, toggleRect, context); currentY += context.ItemHeight + 12; }
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color buttonColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(59, 130, 246);
            using (var path = CreateRoundedPath(toggleRect, 8)) using (var brush = new LinearGradientBrush(toggleRect, buttonColor, Color.FromArgb(buttonColor.R - 20, buttonColor.G - 20, buttonColor.B), LinearGradientMode.Vertical)) { g.FillPath(brush, path); }
            using (var path = CreateRoundedPath(new Rectangle(toggleRect.X + 1, toggleRect.Y + 1, toggleRect.Width - 2, toggleRect.Height / 2), 7)) using (var glowBrush = new SolidBrush(Color.FromArgb(60, 255, 255, 255))) { g.FillPath(glowBrush, path); }
            Color iconColor = Color.White;
            using (var pen = new Pen(iconColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                int centerX = toggleRect.X + toggleRect.Width / 2, centerY = toggleRect.Y + toggleRect.Height / 2, lineWidth = 16;
                g.DrawLine(pen, centerX - lineWidth / 2, centerY - 5, centerX + lineWidth / 2, centerY - 5);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY, centerX + lineWidth / 2, centerY);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY + 5, centerX + lineWidth / 2, centerY + 5);
            }
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color selectionColor1 = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(59, 130, 246);
            Color selectionColor2 = context.UseThemeColors && context.Theme != null ? Color.FromArgb(selectionColor1.R - 30, selectionColor1.G - 30, selectionColor1.B) : Color.FromArgb(37, 99, 235);
            using (var path = CreateRoundedPath(itemRect, 8)) using (var brush = new LinearGradientBrush(itemRect, selectionColor1, selectionColor2, LinearGradientMode.Vertical)) { g.FillPath(brush, path); }
            using (var glowPath = CreateRoundedPath(new Rectangle(itemRect.X, itemRect.Y - 2, itemRect.Width, 6), 4)) using (var glowBrush = new SolidBrush(Color.FromArgb(40, selectionColor1))) { g.FillPath(glowBrush, glowPath); }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(31, 41, 55) : Color.FromArgb(31, 41, 55);
            using (var path = CreateRoundedPath(itemRect, 8)) using (var brush = new SolidBrush(hoverColor)) { g.FillPath(brush, path); }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            int padding = 12, iconSize = 20, iconPadding = 12;
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                int x = itemRect.X + 10;
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
                    Color textColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(209, 213, 219));
                    using (var font = new Font("Inter", 13f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular)) using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - 10, itemRect.Height);
                        g.DrawString(item.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                    }
                }
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - 20, itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(156, 163, 184));
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
            int padding = 6, indent = context.IndentationWidth * indentLevel, iconSize = 18, iconPadding = 10;
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                int x = childRect.X + 10;
                Color lineColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(50, context.Theme.SideMenuForeColor) : Color.FromArgb(50, 75, 85, 99);
                using (var pen = new Pen(lineColor, 1f)) { int lineX = bounds.Left + (indent / 2); g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2); }
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = GetIconPath(child, context);
                    if (context.Theme != null && context.UseThemeColors) { _imagePainter.CurrentTheme = context.Theme; _imagePainter.ApplyThemeOnImage = true; _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; }
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                Color textColor = context.UseThemeColors && context.Theme != null ? (child == context.SelectedItem ? Color.White : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)) : (child == context.SelectedItem ? Color.White : Color.FromArgb(156, 163, 175));
                using (var font = new Font("Inter", 12f, FontStyle.Regular)) using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, childRect.Right - x - 8, childRect.Height);
                    g.DrawString(child.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                }
                currentY += context.ChildItemHeight + 3;
                if (child.Children != null && child.Children.Count > 0 && context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) { PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); }
            }
        }
    }
}
