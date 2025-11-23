using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    public sealed class DiscordStyleSideBarPainter : BaseSideBarPainter
    {
        public override string Name => "DiscordStyle";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            Color backColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuBackColor : Color.FromArgb(47, 49, 54);
            using (var brush = new SolidBrush(backColor)) { g.FillRectangle(brush, bounds); }
            int padding = 8, currentY = bounds.Top + padding;
            if (context.ShowToggleButton) { Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); PaintToggleButton(g, toggleRect, context); currentY += context.ItemHeight + 8; }
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color buttonColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(88, 101, 242);
            using (var path = CreateRoundedPath(toggleRect, 4)) using (var brush = new SolidBrush(buttonColor)) { g.FillPath(brush, path); }
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
            Color selectionColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(66, 70, 77) : Color.FromArgb(66, 70, 77);
            using (var path = CreateRoundedPath(itemRect, 4)) using (var brush = new SolidBrush(selectionColor)) { g.FillPath(brush, path); }
            Color accentColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(88, 101, 242);
            Rectangle accentBar = new Rectangle(itemRect.X, itemRect.Y, 4, itemRect.Height);
            using (var path = CreateRoundedPath(accentBar, 2)) using (var brush = new SolidBrush(accentColor)) { g.FillPath(brush, path); }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(54, 57, 63) : Color.FromArgb(54, 57, 63);
            using (var path = CreateRoundedPath(itemRect, 4)) using (var brush = new SolidBrush(hoverColor)) { g.FillPath(brush, path); }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            int padding = 8, iconSize = 20, iconPadding = 12;
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                int x = itemRect.X + 8;
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
<<<<<<< HEAD
                    Color defaultTint = Color.FromArgb(185, 187, 190);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && item == context.SelectedItem && context.UseThemeColors) iconTint = Color.White;
                    if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, iconRect, item.ImagePath, iconTint);
                    else StyledImagePainter.Paint(g, iconRect, item.ImagePath);
=======
                    _imagePainter.ImagePath = GetIconPath(item, context);
                    if (context.Theme != null && context.UseThemeColors) { _imagePainter.CurrentTheme = context.Theme; _imagePainter.ApplyThemeOnImage = true; _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; }
                    _imagePainter.DrawImage(g, iconRect);
>>>>>>> bdb7ce0d65c735a56e2837a4b1bdc571b4d72341
                    x += iconSize + iconPadding;
                }
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(185, 187, 190));
                    using (var font = new Font("Whitney", 14f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular)) using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - 8, itemRect.Height);
                        g.DrawString(item.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                    }
                }
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - 20, itemRect.Y + (itemRect.Height - 12) / 2, 12, 12);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuForeColor : Color.FromArgb(185, 187, 190);
                    using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    {
                        int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                        if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3); g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1); }
                        else { g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy); g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4); }
                    }
                }
                currentY += context.ItemHeight + 2;
                if (item.Children != null && item.Children.Count > 0 && context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) { PaintChildItems(g, bounds, context, item, ref currentY, 1); }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            int padding = 4, indent = context.IndentationWidth * indentLevel, iconSize = 16, iconPadding = 8;
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                int x = childRect.X + 8;
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
<<<<<<< HEAD
                    Color defaultTint = Color.FromArgb(142, 146, 151);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && child == context.SelectedItem && context.UseThemeColors) iconTint = Color.White;
                    if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, iconRect, child.ImagePath, iconTint);
                    else StyledImagePainter.Paint(g, iconRect, child.ImagePath);
=======
                    _imagePainter.ImagePath = GetIconPath(child, context);
                    if (context.Theme != null && context.UseThemeColors) { _imagePainter.CurrentTheme = context.Theme; _imagePainter.ApplyThemeOnImage = true; _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; }
                    _imagePainter.DrawImage(g, iconRect);
>>>>>>> bdb7ce0d65c735a56e2837a4b1bdc571b4d72341
                    x += iconSize + iconPadding;
                }
                Color textColor = context.UseThemeColors && context.Theme != null ? (child == context.SelectedItem ? Color.White : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)) : (child == context.SelectedItem ? Color.White : Color.FromArgb(142, 146, 151));
                using (var font = new Font("Whitney", 12f, FontStyle.Regular)) using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, childRect.Right - x - 6, childRect.Height);
                    g.DrawString(child.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                }
                currentY += context.ChildItemHeight + 2;
                if (child.Children != null && child.Children.Count > 0 && context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) { PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); }
            }
        }
    }
}
