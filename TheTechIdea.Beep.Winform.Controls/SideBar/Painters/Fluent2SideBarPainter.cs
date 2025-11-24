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
    public sealed class Fluent2SideBarPainter : BaseSideBarPainter
    {
        private static readonly ImagePainter _imagePainter = new ImagePainter();
        public override string Name => "Fluent2";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Fluent 2 layer color with acrylic effect
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(243, 242, 241);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Fluent 2 divider color
            Color dividerColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.BorderColor 
                : Color.FromArgb(237, 235, 233);
            
            using (var pen = new Pen(dividerColor, 1f)) 
            { 
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); 
            }
            
            int padding = 8;
            int currentY = bounds.Top + padding;
            
            if (context.ShowToggleButton) 
            { 
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight); 
                PaintToggleButton(g, toggleRect, context); 
                currentY += context.ItemHeight + 8; 
            }
            
            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // Fluent 2 accent color (blue)
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(0, 120, 212);
            
            using (var path = CreateRoundedPath(toggleRect, 4)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            // Fluent 2 white on accent
            Color iconColor = Color.White;
            using (var pen = new Pen(iconColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                int centerX = toggleRect.X + toggleRect.Width / 2;
                int centerY = toggleRect.Y + toggleRect.Height / 2;
                int lineWidth = 16;
                
                g.DrawLine(pen, centerX - lineWidth / 2, centerY - 5, centerX + lineWidth / 2, centerY - 5);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY, centerX + lineWidth / 2, centerY);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY + 5, centerX + lineWidth / 2, centerY + 5);
            }
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Fluent 2 subtle selection with left accent bar
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(243, context.Theme.PrimaryColor)
                : Color.FromArgb(243, 242, 241);
            
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillRectangle(brush, itemRect); 
            }
            
            // Left accent bar (Fluent signature)
            Color accentColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(0, 120, 212);
            
            Rectangle accentBar = new Rectangle(itemRect.X, itemRect.Y, 3, itemRect.Height);
            using (var brush = new SolidBrush(accentColor)) 
            { 
                g.FillRectangle(brush, accentBar); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Fluent 2 hover state
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(10, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(10, 0, 120, 212);
            
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillRectangle(brush, itemRect); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            
            int padding = 8;
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
                
                int x = itemRect.X + (item == context.SelectedItem ? 8 : 6); // Offset for accent bar
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = GetIconPath(item, context);
                    if (context.Theme != null && context.UseThemeColors)
                    {
                        _imagePainter.CurrentTheme = context.Theme;
                        _imagePainter.ApplyThemeOnImage = true;
                        _imagePainter.ImageEmbededin = ImageEmbededin.SideBar;
                    }
                    else _imagePainter.ApplyThemeOnImage = false;
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                if (!context.IsCollapsed)
                {
                    // Fluent 2 text color
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.FromArgb(0, 120, 212) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(0, 120, 212) : Color.FromArgb(50, 49, 48));
                    
                    using (var font = new Font("Segoe UI", 14f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular)) 
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - expandIconSize - 12), itemRect.Height);
                        StringFormat format = new StringFormat 
                        { 
                            Alignment = StringAlignment.Near, 
                            LineAlignment = StringAlignment.Center, 
                            Trimming = StringTrimming.EllipsisCharacter 
                        };
                        
                        g.DrawString(item.Text, font, brush, textRect, format);
                    }
                }
                
                // Draw expand/collapse chevron
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(96, 94, 92);
                    
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
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;
                                if (isExpanded)
                                {
                                    g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3);
                                    g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1);
                                }
                                else
                                {
                                    g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy);
                                    g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2;
                            int cy = expandRect.Y + expandRect.Height / 2;
                            
                            if (isExpanded) 
                            { 
                                g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3);
                                g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1);
                            }
                            else 
                            { 
                                g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy);
                                g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4);
                            }
                        }
                    }
                }
                
                currentY += context.ItemHeight + 2;
                
                if (item.Children != null && item.Children.Count > 0 && 
                    context.ExpandedState.ContainsKey(item) && context.ExpandedState[item]) 
                { 
                    PaintChildItems(g, bounds, context, item, ref currentY, 1); 
                }
            }
        }

        private void PaintChildItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, SimpleItem parentItem, ref int currentY, int indentLevel)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0 || context.IsCollapsed) return;
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);
            int padding = 4;
            int indent = context.IndentationWidth * indentLevel;
            
            
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X + (child == context.SelectedItem ? 8 : 4);
                
                // Draw connector line
                Color lineColor = context.UseThemeColors && context.Theme != null 
                    ? Color.FromArgb(60, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(60, 237, 235, 233);
                
                using (var pen = new Pen(lineColor, 1f)) 
                { 
                    int lineX = bounds.Left + (indent / 2);
                    g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
                }
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = GetIconPath(child, context);
                    if (context.Theme != null && context.UseThemeColors)
                    {
                        _imagePainter.CurrentTheme = context.Theme;
                        _imagePainter.ApplyThemeOnImage = true;
                        _imagePainter.ImageEmbededin = ImageEmbededin.SideBar;
                    }
                    else _imagePainter.ApplyThemeOnImage = false;
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.FromArgb(0, 120, 212) : Color.FromArgb(200, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.FromArgb(0, 120, 212) : Color.FromArgb(96, 94, 92));
                
                using (var font = new Font("Segoe UI", 12f, FontStyle.Regular)) 
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, Math.Max(0, childRect.Right - x - childExpandIconSize - 12), childRect.Height);
                    StringFormat format = new StringFormat 
                    { 
                        Alignment = StringAlignment.Near, 
                        LineAlignment = StringAlignment.Center, 
                        Trimming = StringTrimming.EllipsisCharacter 
                    };
                    
                    g.DrawString(child.Text, font, brush, textRect, format);
                }
                
                // Draw expand/collapse chevron for nested children
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - childExpandIconSize - 8, childRect.Y + (childRect.Height - childExpandIconSize) / 2, childExpandIconSize, childExpandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(96, 94, 92);
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
                            using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            {
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;
                                
                                if (isExpanded) 
                                { 
                                    g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2);
                                    g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1);
                                }
                                else 
                                { 
                                    g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy);
                                    g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2;
                            int cy = expandRect.Y + expandRect.Height / 2;
                            
                            if (isExpanded) 
                            { 
                                g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2);
                                g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1);
                            }
                            else 
                            { 
                                g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy);
                                g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3);
                            }
                        }
                    }
                }
                
                currentY += context.ChildItemHeight + 2;
                
                if (child.Children != null && child.Children.Count > 0 && 
                    context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) 
                { 
                    PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); 
                }
            }
        }
    }
}
