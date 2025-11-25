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
    public sealed class DarkGlowSideBarPainter : BaseSideBarPainter
    {
        // Use the BeepImage cache via context.GetCachedIcon
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
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
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
            int padding = 12;
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                int x = itemRect.X + 10;
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(209, 213, 219));
                    var font = BeepFontManager.GetCachedFont("Inter", 13f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular);
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSize - 12), itemRect.Height);
                        g.DrawString(item.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                    }
                }
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : Color.FromArgb(156, 163, 184));
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, expandRect, iconPath, chevronColor);
                            else StyledImagePainter.Paint(g, expandRect, iconPath);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                                if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2); }
                                else { g.DrawLine(pen, cx - 2, cy - 4, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 2, cy + 4); }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2; 
                            if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2); }
                            else { g.DrawLine(pen, cx - 2, cy - 4, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 2, cy + 4); }
                        }
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
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += iconSize + iconPadding;
                }
                Color textColor = context.UseThemeColors && context.Theme != null ? (child == context.SelectedItem ? Color.White : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)) : (child == context.SelectedItem ? Color.White : Color.FromArgb(156, 163, 175));
                var font = BeepFontManager.GetCachedFont("Inter", 12f, FontStyle.Regular);
                using (var brush = new SolidBrush(textColor))
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
