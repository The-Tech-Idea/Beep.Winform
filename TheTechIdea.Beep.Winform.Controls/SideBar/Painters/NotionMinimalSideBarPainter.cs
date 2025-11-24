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
    public sealed class NotionMinimalSideBarPainter : BaseSideBarPainter
    {
        private static readonly ImagePainter _imagePainter = new ImagePainter();
        public override string Name => "NotionMinimal";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Notion warm white background
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(251, 251, 250);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // No visible border - Notion Style
            
            int padding = 4;
            int currentY = bounds.Top + padding + 8;
            
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
            // Notion subtle gray button
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(55, 53, 47);
            
            using (var path = CreateRoundedPath(toggleRect, 3)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            Color iconColor = Color.White;
            using (var pen = new Pen(iconColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
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
            // Notion very subtle selection
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(239, 239, 238)
                : Color.FromArgb(239, 239, 238);
            
            using (var path = CreateRoundedPath(itemRect, 3)) 
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Notion barely visible hover
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(245, 245, 244)
                : Color.FromArgb(247, 246, 243);
            
            using (var path = CreateRoundedPath(itemRect, 3)) 
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            
            int padding = 4;
            int iconSize = GetTopLevelIconSize(context);
            int childIconSize = GetChildIconSize(context);
            int expandIconSize = GetExpandIconSize(context);
            int childExpandIconSize = GetChildExpandIconSize(context);
            int iconPadding = GetIconPadding(context) - 4;
            
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                
                int x = itemRect.X + 6;
                
                // Draw expand/collapse first (Notion Style - left side)
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(x, itemRect.Y + (itemRect.Height - expandIconSize) / 2, expandIconSize, expandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(145, 145, 142);
                    
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
                                    // Down triangle
                                    g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2);
                                    g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1);
                                }
                                else 
                                { 
                                    // Right triangle
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
                                // Down triangle
                                g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2);
                                g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1);
                            }
                            else 
                            { 
                                // Right triangle
                                g.DrawLine(pen, cx - 1, cy - 3, cx + 2, cy);
                                g.DrawLine(pen, cx + 2, cy, cx - 1, cy + 3);
                            }
                        }
                    }
                    
                    x += expandIconSize + 4;
                }
                
                // Draw icon using StyledImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = GetIconPath(item, context);
                    if (context.Theme != null && context.UseThemeColors) { _imagePainter.CurrentTheme = context.Theme; _imagePainter.ApplyThemeOnImage = true; _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; } else _imagePainter.ApplyThemeOnImage = false;
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                
                // Draw text with Notion font
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor
                        : Color.FromArgb(55, 53, 47);
                    
                    using (var font = new Font("ui-sans-serif", 14f, FontStyle.Regular)) 
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
                
                currentY += context.ItemHeight + 1;
                
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
            
            int padding = 2;
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
                
                int x = childRect.X + 6;
                
                // No connector lines in Notion - clean hierarchical
                
                // Draw expand/collapse first if has children
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(x, childRect.Y + (childRect.Height - childExpandIconSize) / 2, childExpandIconSize, childExpandIconSize);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(145, 145, 142);
                    
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
                                    g.DrawLine(pen, cx - 2, cy - 1, cx, cy + 1);
                                    g.DrawLine(pen, cx, cy + 1, cx + 2, cy - 1);
                                }
                                else 
                                { 
                                    g.DrawLine(pen, cx - 1, cy - 2, cx + 1, cy);
                                    g.DrawLine(pen, cx + 1, cy, cx - 1, cy + 2);
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
                                g.DrawLine(pen, cx - 2, cy - 1, cx, cy + 1);
                                g.DrawLine(pen, cx, cy + 1, cx + 2, cy - 1);
                            }
                            else 
                            { 
                                g.DrawLine(pen, cx - 1, cy - 2, cx + 1, cy);
                                g.DrawLine(pen, cx + 1, cy, cx - 1, cy + 2);
                            }
                        }
                    }
                    
                    x += 16;
                }
                
                // Draw icon using StyledImagePainter
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
                    ? Color.FromArgb(200, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B)
                    : Color.FromArgb(120, 119, 116);
                
                using (var font = new Font("ui-sans-serif", 13f, FontStyle.Regular)) 
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
                
                currentY += context.ChildItemHeight + 1;
                
                if (child.Children != null && child.Children.Count > 0 && 
                    context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) 
                { 
                    PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); 
                }
            }
        }
    }
}
