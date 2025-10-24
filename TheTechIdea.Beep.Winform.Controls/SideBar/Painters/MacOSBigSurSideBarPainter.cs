using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    public sealed class MacOSBigSurSideBarPainter : BaseSideBarPainter
    {
        private static readonly ImagePainter _imagePainter = new ImagePainter();
        public override string Name => "MacOSBigSur";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;
            
            // macOS Big Sur translucent sidebar
            Color backColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.SideMenuBackColor 
                : Color.FromArgb(246, 246, 246);
            
            using (var brush = new SolidBrush(backColor)) 
            { 
                g.FillRectangle(brush, bounds); 
            }
            
            // macOS Big Sur divider - very subtle
            Color dividerColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(30, context.Theme.BorderColor)
                : Color.FromArgb(30, 0, 0, 0);
            
            using (var pen = new Pen(dividerColor, 1f)) 
            { 
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom); 
            }
            
            int padding = 12;
            int currentY = bounds.Top + padding + 8;
            
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
            // macOS Big Sur blue accent
            Color buttonColor = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(10, 132, 255);
            
            // Big Sur Style rounded rect
            using (var path = CreateRoundedPath(toggleRect, 8)) 
            using (var brush = new SolidBrush(buttonColor)) 
            { 
                g.FillPath(brush, path); 
            }
            
            // Add subtle inner glow
            using (var path = CreateRoundedPath(new Rectangle(toggleRect.X + 1, toggleRect.Y + 1, toggleRect.Width - 2, toggleRect.Height / 2), 7))
            using (var brush = new SolidBrush(Color.FromArgb(30, 255, 255, 255)))
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
            // macOS Big Sur selection - vibrant blue with gradient
            Color selectionColor1 = context.UseThemeColors && context.Theme != null 
                ? context.Theme.PrimaryColor 
                : Color.FromArgb(10, 132, 255);
            
            Color selectionColor2 = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(240, context.Theme.PrimaryColor)
                : Color.FromArgb(0, 122, 255);
            
            using (var path = CreateRoundedPath(itemRect, 8))
            using (var brush = new LinearGradientBrush(itemRect, selectionColor1, selectionColor2, LinearGradientMode.Vertical))
            { 
                g.FillPath(brush, path); 
            }
            
            // Add top highlight
            Rectangle highlightRect = new Rectangle(itemRect.X + 2, itemRect.Y + 1, itemRect.Width - 4, itemRect.Height / 3);
            using (var path = CreateRoundedPath(highlightRect, 6))
            using (var brush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
            {
                g.FillPath(brush, path);
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // macOS Big Sur hover - very subtle
            Color hoverColor = context.UseThemeColors && context.Theme != null 
                ? Color.FromArgb(6, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(240, 240, 240);
            
            using (var path = CreateRoundedPath(itemRect, 8)) 
            using (var brush = new SolidBrush(hoverColor)) 
            { 
                g.FillPath(brush, path); 
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;
            
            int padding = 12;
            int iconSize = 22;
            int iconPadding = 12;
            
            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                
                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);
                
                int x = itemRect.X + 10;
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = item.ImagePath;
                    
                    if (context.Theme != null && context.UseThemeColors) 
                    { 
                        _imagePainter.CurrentTheme = context.Theme; 
                        _imagePainter.ApplyThemeOnImage = true; 
                        _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; 
                    }
                    
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                
                // Draw text with SF Pro Display
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.White : Color.FromArgb(51, 51, 51));
                    
                    using (var font = new Font("SF Pro Display", 13f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular)) 
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
                
                // Draw expand/collapse disclosure triangle
                if (item.Children != null && item.Children.Count > 0 && !context.IsCollapsed)
                {
                    Rectangle expandRect = new Rectangle(itemRect.Right - 20, itemRect.Y + (itemRect.Height - 12) / 2, 12, 12);
                    bool isExpanded = context.ExpandedState.ContainsKey(item) && context.ExpandedState[item];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? (item == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.White : Color.FromArgb(142, 142, 147));
                    
                    using (var pen = new Pen(chevronColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    {
                        int cx = expandRect.X + expandRect.Width / 2;
                        int cy = expandRect.Y + expandRect.Height / 2;
                        
                        if (isExpanded) 
                        { 
                            // Pointing down
                            g.DrawLine(pen, cx - 4, cy - 1, cx, cy + 3);
                            g.DrawLine(pen, cx, cy + 3, cx + 4, cy - 1);
                        }
                        else 
                        { 
                            // Pointing right
                            g.DrawLine(pen, cx - 1, cy - 4, cx + 3, cy);
                            g.DrawLine(pen, cx + 3, cy, cx - 1, cy + 4);
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
            
            int padding = 6;
            int indent = context.IndentationWidth * indentLevel;
            int iconSize = 18;
            int iconPadding = 10;
            
            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                Rectangle childRect = new Rectangle(bounds.Left + indent + padding, currentY, bounds.Width - indent - padding * 2, context.ChildItemHeight);
                
                if (child == context.HoveredItem) PaintHover(g, childRect, context);
                if (child == context.SelectedItem) PaintSelection(g, childRect, context);
                
                int x = childRect.X + 10;
                
                // Draw connector line - macOS Style
                Color lineColor = context.UseThemeColors && context.Theme != null 
                    ? Color.FromArgb(30, context.Theme.SideMenuForeColor)
                    : Color.FromArgb(30, 142, 142, 147);
                
                using (var pen = new Pen(lineColor, 1f)) 
                { 
                    int lineX = bounds.Left + (indent / 2);
                    g.DrawLine(pen, lineX, childRect.Y + childRect.Height / 2, childRect.X, childRect.Y + childRect.Height / 2);
                }
                
                // Draw icon using ImagePainter
                if (!string.IsNullOrEmpty(child.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = child.ImagePath;
                    
                    if (context.Theme != null && context.UseThemeColors) 
                    { 
                        _imagePainter.CurrentTheme = context.Theme; 
                        _imagePainter.ApplyThemeOnImage = true; 
                        _imagePainter.ImageEmbededin = ImageEmbededin.SideBar; 
                    }
                    
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }
                
                // Draw text
                Color textColor = context.UseThemeColors && context.Theme != null 
                    ? (child == context.SelectedItem ? Color.White : Color.FromArgb(190, context.Theme.SideMenuForeColor.R, context.Theme.SideMenuForeColor.G, context.Theme.SideMenuForeColor.B))
                    : (child == context.SelectedItem ? Color.White : Color.FromArgb(99, 99, 102));
                
                using (var font = new Font("SF Pro Text", 11.5f, FontStyle.Regular)) 
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
                
                // Draw expand/collapse disclosure triangle for nested children
                if (child.Children != null && child.Children.Count > 0)
                {
                    Rectangle expandRect = new Rectangle(childRect.Right - 18, childRect.Y + (childRect.Height - 10) / 2, 10, 10);
                    bool isExpanded = context.ExpandedState.ContainsKey(child) && context.ExpandedState[child];
                    
                    Color chevronColor = context.UseThemeColors && context.Theme != null 
                        ? (child == context.SelectedItem ? Color.White : context.Theme.SideMenuForeColor)
                        : (child == context.SelectedItem ? Color.White : Color.FromArgb(142, 142, 147));
                    
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
                
                currentY += context.ChildItemHeight + 3;
                
                if (child.Children != null && child.Children.Count > 0 && 
                    context.ExpandedState.ContainsKey(child) && context.ExpandedState[child]) 
                { 
                    PaintChildItems(g, bounds, context, child, ref currentY, indentLevel + 1); 
                }
            }
        }
    }
}
