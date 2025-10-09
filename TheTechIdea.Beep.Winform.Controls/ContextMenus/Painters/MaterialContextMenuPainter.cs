using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    /// <summary>
    /// Material Design style context menu painter
    /// </summary>
    public class MaterialContextMenuPainter : IContextMenuPainter
    {
        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, IBeepTheme theme)
        {
            // Draw elevated background with subtle gradient using Menu colors
            using (var brush = new LinearGradientBrush(bounds, theme.MenuBackColor, 
                ControlPaint.Light(theme.MenuBackColor, 0.05f), 90f))
            {
                g.FillRectangle(brush, bounds);
            }
        }
        
        public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, IBeepTheme theme)
        {
            if (items == null || items.Count == 0) return;
            
            int y = owner.ScaleDpi(8); // More padding for Material
            
            foreach (var item in items)
            {
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, theme);
                    y += owner.ScaleDpi(8);
                    continue;
                }
                
                int itemHeight = GetPreferredItemHeight();
                var itemRect = new Rectangle(owner.ScaleDpi(4), y, owner.Width - owner.ScaleDpi(8), itemHeight);
                
                DrawItem(g, owner, item, itemRect, item == selectedItem, item == hoveredItem, theme);
                
                y += itemHeight;
            }
        }
        
        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, IBeepTheme theme)
        {
            // Material elevation shadow
            DrawElevationShadow(g, bounds, 4);
        }
        
        public int GetPreferredItemHeight()
        {
            return 36; // Taller items for Material
        }
        
        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, IBeepTheme theme)
        {
            // Draw ripple/hover effect with rounded corners using Menu colors
            if (isHovered && item.IsEnabled)
            {
                using (var brush = new SolidBrush(theme.MenuItemHoverBackColor))
                using (var path = GetRoundedRect(itemRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }
            else if (isSelected && item.IsEnabled)
            {
                using (var brush = new SolidBrush(theme.MenuItemSelectedBackColor))
                using (var path = GetRoundedRect(itemRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }
            
            int x = itemRect.X + owner.ScaleDpi(12);
            
            // Draw icon with Material style
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - owner.ImageSize) / 2, 
                    owner.ImageSize, owner.ImageSize);
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += owner.ImageSize + owner.ScaleDpi(16);
            }
            
            // Draw text with Material typography and Menu colors
            var textRect = new Rectangle(x, itemRect.Y, itemRect.Width - x - owner.ScaleDpi(16), itemRect.Height);
            var textColor = !item.IsEnabled ? theme.DisabledForeColor : 
                           isHovered ? theme.MenuItemHoverForeColor :
                           isSelected ? theme.MenuItemSelectedForeColor :
                           theme.MenuItemForeColor;
            
            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            
            // Draw submenu arrow
            if (item.Children != null && item.Children.Count > 0)
            {
                var arrowRect = new Rectangle(itemRect.Right - owner.ScaleDpi(20), 
                    itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                DrawMaterialArrow(g, arrowRect, !item.IsEnabled, theme);
            }
        }
        
        private void DrawIcon(Graphics g, Rectangle rect, string imagePath, bool isDisabled)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            
            try
            {
                // Use StyledImagePainter for consistent image rendering with caching
                if (isDisabled)
                {
                    // For disabled state, draw with reduced opacity
                    using (var tempBitmap = new Bitmap(rect.Width, rect.Height))
                    {
                        using (var tempG = Graphics.FromImage(tempBitmap))
                        {
                            TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(
                                tempG, new Rectangle(0, 0, rect.Width, rect.Height), imagePath);
                        }
                        
                        // Draw grayed out
                        ControlPaint.DrawImageDisabled(g, tempBitmap, rect.X, rect.Y, Color.Transparent);
                    }
                }
                else
                {
                    // Use StyledImagePainter directly with Material rounded corners
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(
                        g, rect, imagePath, BeepControlStyle.Material3);
                }
            }
            catch
            {
                // Image not found or invalid path
            }
        }
        
        private void DrawMaterialArrow(Graphics g, Rectangle rect, bool isDisabled, IBeepTheme theme)
        {
            var arrowColor = isDisabled ? theme.DisabledForeColor : theme.MenuItemForeColor;
            using (var pen = new Pen(arrowColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                int midY = rect.Y + rect.Height / 2;
                g.DrawLine(pen, rect.X + 4, midY - 4, rect.X + rect.Width - 4, midY);
                g.DrawLine(pen, rect.X + rect.Width - 4, midY, rect.X + 4, midY + 4);
            }
        }
        
        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, IBeepTheme theme)
        {
            if (!owner.ShowSeparators) return;
            
            int lineY = y + owner.ScaleDpi(4);
            int x = owner.ScaleDpi(16);
            int width = owner.Width - owner.ScaleDpi(32);
            
            using (var pen = new Pen(Color.FromArgb(30, theme.MenuItemForeColor), 1))
            {
                g.DrawLine(pen, x, lineY, x + width, lineY);
            }
        }
        
        private void DrawElevationShadow(Graphics g, Rectangle bounds, int elevation)
        {
            int shadowSize = elevation * 2;
            int shadowAlpha = 40;
            
            using (var path = new GraphicsPath())
            {
                path.AddRectangle(bounds);
                
                for (int i = 0; i < shadowSize; i++)
                {
                    int alpha = shadowAlpha - (i * shadowAlpha / shadowSize);
                    using (var pen = new Pen(Color.FromArgb(alpha, 0, 0, 0)))
                    {
                        var shadowBounds = bounds;
                        shadowBounds.Inflate(i, i);
                        g.DrawRectangle(pen, shadowBounds);
                    }
                }
            }
        }
        
        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
        
        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
    }
}
