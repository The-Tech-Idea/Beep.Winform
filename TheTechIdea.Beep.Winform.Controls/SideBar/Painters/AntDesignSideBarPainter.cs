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
    public sealed class AntDesignSideBarPainter : BaseSideBarPainter
    {
        public override string Name => "AntDesign";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Ant Design dark sidebar background
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(0, 21, 41);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Ant Design border
            Color borderColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.BorderColor 
                : Color.FromArgb(255, 255, 255, 8);
            
            using (var pen = new Pen(borderColor, 1f)) 
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
            // Ant Design primary blue
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(24, 144, 255);
            
            using (var path = CreateRoundedPath(toggleRect, 2)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            // White icon
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
            // Ant Design primary color with light background
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(230, 247, 255)
                : Color.FromArgb(230, 247, 255);
            
            // For dark theme, use darker blue
            if (context.Theme != null && context.UseThemeColors)
            {
                selectionColor = Color.FromArgb(24, 144, 255, 20);
            }
            else
            {
                selectionColor = Color.FromArgb(24, 144, 255, 15);
            }
            
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillRectangle(brush, itemRect); 
            }
            
            // Ant Design left accent bar
            Color accentColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(24, 144, 255);
            
            Rectangle accentBar = new Rectangle(itemRect.X, itemRect.Y, 3, itemRect.Height);
            using (var brush = new SolidBrush(accentColor)) 
            { 
                g.FillRectangle(brush, accentBar); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Ant Design hover state
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(15, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(255, 255, 255, 8);
            
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillRectangle(brush, itemRect); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            
            int padding = 8;
            int iconSize = 20;
            int iconPadding = 12;
            
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                
                int x = itemRect.X + (item == context.SelectedItem ? 8 : 4);
                
                // Draw icon using StyledImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    Color defaultTint = Color.FromArgb(255, 255, 255, 175);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && item == context.SelectedItem && context.UseThemeColors)
                        iconTint = context.Theme.PrimaryColor;

                    if (context.Theme != null && context.UseThemeColors)
                        StyledImagePainter.PaintWithTint(g, iconRect, item.ImagePath, iconTint);
                    else
                        StyledImagePainter.Paint(g, iconRect, item.ImagePath);

                    x += iconSize + iconPadding;
                }
                
                // Draw text with Ant Design typography
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.FromArgb(24, 144, 255) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(24, 144, 255) : Color.FromArgb(255, 255, 255, 175));
                    
                    using (var font = new Font("Microsoft YaHei UI", 14f, FontStyle.Regular)) 
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - 8, itemRect.Height);
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
                    Rectangle expandRect = new Rectangle(itemRect.Right - 20, itemRect.Y + (itemRect.Height - 12) / 2, 12, 12);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(255, 255, 255, 135);
                    
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
            
            int padding = 4;
            int indent = context.IndentationWidth * indentLevel;
            int iconSize = 16;
            int iconPadding = 8;
            
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X + (child == context.SelectedItem ? 8 : 4);
                
                // Draw connector line
                Color lineColor = context.UseThemeColors && context.Theme != null 
                    ? Color.FromArgb(40, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(255, 255, 255, 30);
                
                using (var pen = new Pen(lineColor, 1f)) 
                { 
                    int lineX = bounds.Left + (indent / 2);
                    g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
                }
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    Color defaultTint = Color.FromArgb(255, 255, 255, 175);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && child == context.SelectedItem && context.UseThemeColors)
                        iconTint = context.Theme.PrimaryColor;

                    if (context.Theme != null && context.UseThemeColors)
                        StyledImagePainter.PaintWithTint(g, iconRect, child.ImagePath, iconTint);
                    else
                        StyledImagePainter.Paint(g, iconRect, child.ImagePath);

                    x += iconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.FromArgb(24, 144, 255) : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.FromArgb(24, 144, 255) : Color.FromArgb(255, 255, 255, 145));
                
                using (var font = new Font("Microsoft YaHei UI", 12f, FontStyle.Regular)) 
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, childRect.Right - x - 6, childRect.Height);
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
                    Rectangle expandRect = new Rectangle(childRect.Right - 16, childRect.Y + (childRect.Height - 10) / 2, 10, 10);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(255, 255, 255, 135);
                    
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
