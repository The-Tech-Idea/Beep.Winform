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
    public sealed class Windows11MicaSideBarPainter : BaseSideBarPainter
    {
        public override string Name => "Windows11Mica";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // Windows 11 Mica background - slightly translucent
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(252, 252, 252);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // Add subtle noise texture effect (Mica characteristic)
            Random rand = new Random(12345); // Fixed seed for consistency
            for (int i = 0; i < 200; i++)
            {
                int x = rand.Next(bounds.Left, bounds.Right);
                int y = rand.Next(bounds.Top, bounds.Bottom);
                Color noiseColor = Color.FromArgb(3, 0, 0, 0);
                using (var brush = new SolidBrush(noiseColor))
                {
                    g.FillRectangle(brush, x, y, 1, 1);
                }
            }
            
            // Windows 11 divider - very subtle
            Color dividerColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(20, context.Theme.BorderColor)
                : Color.FromArgb(20, 0, 0, 0);
            
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
            // Windows 11 accent color
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(0, 120, 212);
            
            using (var path = CreateRoundedPath(toggleRect, 4)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
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
            // Windows 11 subtle selection with slight background
            Color selectionColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(243, context.Theme.PrimaryColor)
                : Color.FromArgb(249, 249, 249);
            
            using (var path = CreateRoundedPath(itemRect, 4)) 
            using (var brush = new SolidBrush(selectionColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            // Add subtle left accent
            Color accentColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(0, 120, 212);
            
            Rectangle accentBar = new Rectangle(itemRect.X + 2, itemRect.Y + 4, 3, itemRect.Height - 8);
            using (var path = CreateRoundedPath(accentBar, 1))
            using (var brush = new SolidBrush(accentColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Windows 11 hover state - very subtle
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(8, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(246, 246, 246);
            
            using (var path = CreateRoundedPath(itemRect, 4)) 
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillPath(brush, path); 
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
                
                int x = itemRect.X + 8;
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    Color defaultTint = Color.FromArgb(32, 32, 32);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && item == context.SelectedItem && context.UseThemeColors) iconTint = Color.FromArgb(0, 120, 212);
                    if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, iconRect, item.ImagePath, iconTint);
                    else StyledImagePainter.Paint(g, iconRect, item.ImagePath);
                    x += iconSize + iconPadding;
                }
                
                // Draw text with Segoe UI Variable
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.FromArgb(0, 120, 212) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(0, 120, 212) : Color.FromArgb(32, 32, 32));
                    
                    using (var font = new Font("Segoe UI Variable", 13f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular)) 
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
                        : Color.FromArgb(100, 100, 100);
                    
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
                
                int x = childRect.X + 8;
                
                // Draw connector line
                Color lineColor = context.UseThemeColors && context.Theme != null 
                    ? Color.FromArgb(40, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(40, 100, 100, 100);
                
                using (var pen = new Pen(lineColor, 1f)) 
                { 
                    int lineX = bounds.Left + (indent / 2);
                    g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
                }
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    Color defaultTint = Color.FromArgb(100, 100, 100);
                    Color iconTint = GetEffectiveColor(context, context.Theme?.SideMenuForeColor ?? defaultTint, defaultTint);
                    if (context.Theme != null && child == context.SelectedItem && context.UseThemeColors) iconTint = Color.FromArgb(0, 120, 212);
                    if (context.Theme != null && context.UseThemeColors) StyledImagePainter.PaintWithTint(g, iconRect, child.ImagePath, iconTint);
                    else StyledImagePainter.Paint(g, iconRect, child.ImagePath);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.FromArgb(0, 120, 212) : Color.FromArgb(200, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.FromArgb(0, 120, 212) : Color.FromArgb(100, 100, 100));
                
                using (var font = new Font("Segoe UI Variable", 12f, FontStyle.Regular)) 
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
                        : Color.FromArgb(100, 100, 100);
                    
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
