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
    public sealed class MaterialYouSideBarPainter : BaseSideBarPainter
    {
        private static readonly ImagePainter _imagePainter = new ImagePainter();
        public override string Name => "MaterialYou";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Material You dynamic surface
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(254, 247, 255);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Material You outline variant
            Color outlineColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.BorderColor 
                : Color.FromArgb(124, 117, 126);
            
            using (var pen = new Pen(Color.FromArgb(30, outlineColor), 1f)) 
            { 
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); 
            }
            
            int padding = 12;
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
            // Material You dynamic primary
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(103, 80, 164);
            
            // Full rounded pill
            using (var path = CreateRoundedPath(toggleRect, toggleRect.Height / 2)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            Color iconColor = Color.White;
            using (var pen = new Pen(iconColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                int centerX = toggleRect.X + toggleRect.Width / 2;
                int centerY = toggleRect.Y + toggleRect.Height / 2;
                int lineWidth = 14;
                
                g.DrawLine(pen, centerX - lineWidth / 2, centerY - 4, centerX + lineWidth / 2, centerY - 4);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY, centerX + lineWidth / 2, centerY);
                g.DrawLine(pen, centerX - lineWidth / 2, centerY + 4, centerX + lineWidth / 2, centerY + 4);
            }
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Material You secondary container (dynamic)
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(220, context.Theme.PrimaryColor)
                : Color.FromArgb(232, 222, 248);
            
            using (var path = CreateRoundedPath(itemRect, itemRect.Height / 2)) 
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Material You state layer
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(12, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(12, 103, 80, 164);
            
            using (var path = CreateRoundedPath(itemRect, itemRect.Height / 2)) 
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillPath(brush, path); 
            }
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
                
                int x = itemRect.X + 12;
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = GetIconPath(item, context);
                    if (context.Theme != null && context.UseThemeColors) { _imagePainter.CurrentTheme = context.Theme; _imagePainter.ApplyThemeOnImage = true; _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; } else _imagePainter.ApplyThemeOnImage = false;
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                if (!context.IsCollapsed)
                {
                    // Material You on-surface or on-secondary-container
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.FromArgb(31, 27, 51) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(31, 27, 51) : Color.FromArgb(28, 27, 31));
                    
                    using (var font = new Font("Roboto", 14f, FontStyle.Bold)) 
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
                
                // Draw expand/collapse icon
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - expandIconSize - 8, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(73, 69, 79);
                    
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
                                int cx = expandRect.X + expandRect.Width / 2;
                                int cy = expandRect.Y + expandRect.Height / 2;
                                
                                if (isExpanded) 
                                { 
                                    g.DrawLine(pen, cx - 5, cy - 2, cx, cy + 3);
                                    g.DrawLine(pen, cx, cy + 3, cx + 5, cy - 2);
                                }
                                else 
                                { 
                                    g.DrawLine(pen, cx - 2, cy - 5, cx + 3, cy);
                                    g.DrawLine(pen, cx + 3, cy, cx - 2, cy + 5);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            int cx = expandRect.X + expandRect.Width / 2;
                            int cy = expandRect.Y + expandRect.Height / 2;
                            
                            if (isExpanded) 
                            { 
                                g.DrawLine(pen, cx - 5, cy - 2, cx, cy + 3);
                                g.DrawLine(pen, cx, cy + 3, cx + 5, cy - 2);
                            }
                            else 
                            { 
                                g.DrawLine(pen, cx - 2, cy - 5, cx + 3, cy);
                                g.DrawLine(pen, cx + 3, cy, cx - 2, cy + 5);
                            }
                        }
                    }
                }
                
                currentY += context.ItemHeight + 4;
                
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
            
            int padding = 8;
            int indent = context.IndentationWidth * indentLevel;
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context);

            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X + 8;
                
                // Draw connector line with Material You Style
                Color lineColor = context.UseThemeColors && context.Theme != null 
                    ? Color.FromArgb(40, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(40, 124, 117, 126);
                
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
                    if (context.Theme != null && context.UseThemeColors) { _imagePainter.CurrentTheme = context.Theme; _imagePainter.ApplyThemeOnImage = true; _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; } else _imagePainter.ApplyThemeOnImage = false;
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.FromArgb(31, 27, 51) : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.FromArgb(31, 27, 51) : Color.FromArgb(99, 91, 103));
                
                using (var font = new Font("Roboto", 13f, FontStyle.Regular)) 
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
                
                // Draw expand/collapse icon for nested children
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - childExpandIconSize - 8, childRect.Y + (childRect.Height - childExpandIconSize) / 2, childExpandIconSize, childExpandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(99, 91, 103);
                    
                    using (var pen = new Pen(chevronColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    {
                        int cx = expandRect.X + expandRect.Width / 2;
                        int cy = expandRect.Y + expandRect.Height / 2;
                        
                        if (isExpanded) 
                        { 
                            g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2);
                            g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2);
                        }
                        else 
                        { 
                            g.DrawLine(pen, cx - 2, cy - 4, cx + 2, cy);
                            g.DrawLine(pen, cx + 2, cy, cx - 2, cy + 4);
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
