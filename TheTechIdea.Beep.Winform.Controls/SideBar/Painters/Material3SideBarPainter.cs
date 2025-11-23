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
    public sealed class Material3SideBarPainter : BaseSideBarPainter
    {
        public override string Name => "Material3";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Material 3 surface color
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(255, 251, 254);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Material 3 outline color
            Color outlineColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.BorderColor 
                : Color.FromArgb(121, 116, 126);
            
            using (var pen = new Pen(outlineColor, 1f)) 
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
            // Material 3 primary color (purple)
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(103, 80, 164);
            
            using (var path = CreateRoundedPath(toggleRect, 28)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            // Material 3 on-primary color
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
            // Material 3 primary container
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(234, 221, 255);
            
            using (var path = CreateRoundedPath(itemRect, 28)) 
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Material 3 hover state overlay
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(8, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(8, 103, 80, 164);
            
            using (var path = CreateRoundedPath(itemRect, 28)) 
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            
            int padding = 16;
            int iconSize = 24;
            int iconPadding = 12;
            
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                
                int x = itemRect.X + iconPadding;
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    Color defaultTint = Color.FromArgb(28, 27, 31);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && item == context.SelectedItem && context.UseThemeColors) iconTint = Color.FromArgb(73, 69, 79);
                    if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, iconRect, item.ImagePath, iconTint);
                    else StyledImagePainter.Paint(g, iconRect, item.ImagePath);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                if (!context.IsCollapsed)
                {
                    // Material 3 on-surface or on-primary-container
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.FromArgb(73, 69, 79) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(73, 69, 79) : Color.FromArgb(28, 27, 31));
                    
                    using (var font = new Font("Roboto", 14f, FontStyle.Bold)) 
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - 10, itemRect.Height);
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
                    Rectangle expandRect = new Rectangle(itemRect.Right - 24, itemRect.Y + (itemRect.Height - 24) / 2, 24, 24);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? context.Theme.SideMenuForeColor 
                        : Color.FromArgb(73, 69, 79);
                    
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
            int iconSize = 20;
            int iconPadding = 8;
            
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X + iconPadding;
                
                // Draw connector line
                Color lineColor = context.UseThemeColors && context.Theme != null 
                    ? Color.FromArgb(50, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(50, 121, 116, 126);
                
                using (var pen = new Pen(lineColor, 1f)) 
                { 
                    int lineX = bounds.Left + (indent / 2);
                    g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
                }
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    Color defaultTint = Color.FromArgb(99, 91, 103);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && child == context.SelectedItem && context.UseThemeColors) iconTint = Color.FromArgb(73, 69, 79);
                    if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, iconRect, child.ImagePath, iconTint);
                    else StyledImagePainter.Paint(g, iconRect, child.ImagePath);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.FromArgb(73, 69, 79) : Color.FromArgb(180, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.FromArgb(73, 69, 79) : Color.FromArgb(99, 91, 103));
                
                using (var font = new Font("Roboto", 12f, FontStyle.Regular)) 
                using (var brush = new SolidBrush(textColor))
                {
                    Rectangle textRect = new Rectangle(x, childRect.Y, childRect.Right - x - 8, childRect.Height);
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
                    Rectangle expandRect = new Rectangle(childRect.Right - 20, childRect.Y + (childRect.Height - 20) / 2, 20, 20);
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
