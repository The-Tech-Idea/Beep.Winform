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
    public sealed class StripeDashboardSideBarPainter : BaseSideBarPainter
    {
        public override string Name => "StripeDashboard";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Stripe deep indigo sidebar
            Color backColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuBackColor : Color.FromArgb(18, 22, 38);
            using (var brush = new SolidBrush(backColor)) { g.FillRectangle(brush, bounds); }
            
            Color borderColor = context.UseThemeColors && context.Theme != null ? context.Theme.BorderColor : Color.FromArgb(255, 255, 255, 5);
            using (var pen = new Pen(borderColor, 1f)) { g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); }
            
            int padding = 12, currentY = bounds.Top + padding;
            if (context.ShowToggleButton) { Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); PaintToggleButton(g, toggleRect, context); currentY += context.ItemHeight + 12; }
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            Color buttonColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(99, 102, 241);
            using (var path = CreateRoundedPath(toggleRect, 8)) using (var brush = new SolidBrush(buttonColor)) { g.FillPath(brush, path); }
            Color iconColor = Color.White;
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color selectionColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(99, 102, 241, 20) : Color.FromArgb(99, 102, 241, 15);
            using (var path = CreateRoundedPath(itemRect, 6)) using (var brush = new SolidBrush(selectionColor)) { g.FillPath(brush, path); }
            Color accentColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : Color.FromArgb(99, 102, 241);
            Rectangle accentBar = new Rectangle(itemRect.X, itemRect.Y, 3, itemRect.Height);
            using (var brush = new SolidBrush(accentColor)) { g.FillRectangle(brush, accentBar); }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(255, 255, 255, 5) : Color.FromArgb(255, 255, 255, 5);
            using (var path = CreateRoundedPath(itemRect, 6)) using (var brush = new SolidBrush(hoverColor)) { g.FillPath(brush, path); }
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
                int x = itemRect.X + 8;
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }
                if (!context.IsCollapsed)
                {
                    Color fallbackText = context.Theme?.SubLabelForColor ?? Color.FromArgb(75, 85, 99);
                    Color textColor = context.UseThemeColors && context.Theme != null ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor) : (item == context.SelectedItem ? Color.White : fallbackText);
                    var font = BeepFontManager.GetCachedFont("Inter", 13f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular);
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSize - 12), itemRect.Height);
                        g.DrawString(item.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                    }
                }
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                        Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuForeColor : Color.FromArgb(156, 163, 175);
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            PaintSvgWithFallback(g, expandRect, iconPath, context.Theme != null && context.UseThemeColors ? chevronColor : (Color?)null, true, context);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                                if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3); g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1); }
                                else { g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy); g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4); }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                            if (isExpanded) { g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3); g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1); }
                            else { g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy); g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4); }
                        }
                    }
                }
                currentY += context.ItemHeight + 2;
                if (item.Children != null && item.Children.Count > 0 && context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) { PaintChildItems(g, bounds, context, item, ref currentY, 1); }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);

            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            int padding = 4, indent = context.IndentationWidth * indentLevel, iconSize = childIconSize;
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                int x = childRect.X + 8;
                Color lineColor = context.UseThemeColors && context.Theme != null ? Color.FromArgb(40, context.Theme.SideMenuForeColor) : Color.FromArgb(255, 255, 255, 15);
                using (var pen = new Pen(lineColor, 1f)) { int lineX = bounds.Left + (indent / 2); g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2); }
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += iconSize + iconPadding;
                }
                Color fallbackChildText = context.Theme?.SubLabelForColor ?? Color.FromArgb(75, 85, 99);
                Color textColor = context.UseThemeColors && context.Theme != null ? (child == context.SelectedItem ? Color.White : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)) : (child == context.SelectedItem ? Color.White : fallbackChildText);
                var font = BeepFontManager.GetCachedFont("Inter", 12f, FontStyle.Regular);
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, Math.Max(0, childRect.Right - x - childExpandIconSize - 12), childRect.Height);
                    g.DrawString(child.Text, font, brush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                }
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - childExpandIconSize - 8, childRect.Y + (childRect.Height - childExpandIconSize) / 2, childExpandIconSize, childExpandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    Color chevronColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuForeColor : Color.FromArgb(153, 153, 153);
                    if (context.UseExpandCollapseIcon && !string.IsNullOrEmpty(context.ExpandIconPath) && !string.IsNullOrEmpty(context.CollapseIconPath))
                    {
                        var iconPath = isExpanded ? context.CollapseIconPath : context.ExpandIconPath;
                        try
                        {
                            PaintSvgWithFallback(g, expandRect, iconPath, context.Theme != null && context.UseThemeColors ? chevronColor : (Color?)null, true, context);
                        }
                        catch
                        {
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round }) { int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2; if (isExpanded) { g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1); } else { g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3); } }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                            if (isExpanded) { g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1); }
                            else { g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3); }
                        }
                    }
                }

                currentY += context.ChildItemHeight + 2;
                if (child.Children != null && child.Children.Count > 0 && context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) { PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); }
            }
        }
    }
}
