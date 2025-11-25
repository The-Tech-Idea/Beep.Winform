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
    public sealed class VercelCleanSideBarPainter : BaseSideBarPainter
    {
        public override string Name => "VercelClean";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Vercel ultra-clean white
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.White;
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Vercel hairline border
            Color borderColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(234, 234, 234)
                : Color.FromArgb(234, 234, 234);
            
            using (var pen = new Pen(borderColor, 1f)) 
            { 
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); 
            }
            
            int padding = 16;
            int currentY = bounds.Top + padding;
            
            if (context.ShowToggleButton) 
            { 
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); 
                PaintToggleButton(g, toggleRect, context); 
                currentY += context.ItemHeight + 12; 
            }
            
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // Vercel black button
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.Black;
            
            using (var path = CreateRoundedPath(toggleRect, 6)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            // Use shared DrawHamburgerIcon helper
            Color iconColor = Color.White;
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Vercel subtle selection
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(250, 250, 250)
                : Color.FromArgb(250, 250, 250);
            
            using (var path = CreateRoundedPath(itemRect, 6)) 
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(252, 252, 252)
                : Color.FromArgb(252, 252, 252);
            
            using (var path = CreateRoundedPath(itemRect, 6)) 
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            
            int padding = 16;
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = 12;
            
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
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.Black : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.Black : Color.FromArgb(102, 102, 102));
                    
                    var font = BeepFontManager.GetCachedFont("Inter", 13f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular); 
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSize - 12), itemRect.Height);
                        StringFormat format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                        g.DrawString(item.Text, font, brush, textRect, format);
                    }
                }
                
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
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
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2, cy = expandRect.Y + expandRect.Height / 2;
                                if (isExpanded) { g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2); g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1); }
                                else { g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy); g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3); }
                            }
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
                
                currentY += context.ItemHeight + 4;
                
                if (item.Children != null && item.Children.Count > 0 && context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) 
                { 
                    PaintChildItems(g, bounds, context, item, ref currentY, 1); 
                }
            }
        }
        }
        

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            int childExpandIconSize = GetChildExpandIconSize(context);
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            
            int padding = 8, indent = context.IndentationWidth * indentLevel, iconSize = 16, iconPadding = 8;
            
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X + 8;
                
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, child, iconRect, context);
                    x += iconSize + iconPadding;
                }
                
                Color textColor = context.UseThemeColors && context.Theme != null ? (child == context.SelectedItem ? Color.Black : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)) : (child == context.SelectedItem ? Color.Black : Color.FromArgb(153, 153, 153));
                
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
                    
                    if (context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) { PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); }
                }
                
                currentY += context.ChildItemHeight + 2;
            }
        }
    }
}
