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
    /// Flat bold color style context menu painter
    /// </summary>
    public class FlatContextMenuPainter : IContextMenuPainter
    {
        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, IBeepTheme theme)
        {
            // Flat background using Menu colors
            using (var brush = new SolidBrush(theme.MenuBackColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }
        
        public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, IBeepTheme theme)
        {
            if (items == null || items.Count == 0) return;
            
            int y = owner.ScaleDpi(4);
            
            foreach (var item in items)
            {
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, theme);
                    y += owner.ScaleDpi(8);
                    continue;
                }
                
                int itemHeight = GetPreferredItemHeight();
                var itemRect = new Rectangle(owner.ScaleDpi(2), y, owner.Width - owner.ScaleDpi(4), itemHeight);
                
                DrawItem(g, owner, item, itemRect, item == selectedItem, item == hoveredItem, theme);
                
                y += itemHeight;
            }
        }
        
        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, IBeepTheme theme)
        {
            // Bold border with menu border color
            using (var pen = new Pen(theme.MenuBorderColor, 2))
            {
                var borderRect = bounds;
                borderRect.Width -= 2;
                borderRect.Height -= 2;
                borderRect.Inflate(1, 1);
                g.DrawRectangle(pen, borderRect);
            }
        }
        
        public int GetPreferredItemHeight()
        {
            return 30;
        }
        
        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, IBeepTheme theme)
        {
            // Draw bold flat hover effect using Menu colors
            if (isHovered && item.IsEnabled)
            {
                using (var brush = new SolidBrush(theme.MenuItemHoverBackColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
            else if (isSelected && item.IsEnabled)
            {
                using (var brush = new SolidBrush(theme.MenuItemSelectedBackColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
            
            // Determine text color based on state
            Color textColor;
            if (!item.IsEnabled)
            {
                textColor = theme.DisabledForeColor;
            }
            else if (isHovered)
            {
                textColor = theme.MenuItemHoverForeColor;
            }
            else if (isSelected)
            {
                textColor = theme.MenuItemSelectedForeColor;
            }
            else
            {
                textColor = theme.MenuItemForeColor;
            }
            
            int x = itemRect.X + owner.ScaleDpi(12);
            
            // Draw icon
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - owner.ImageSize) / 2, 
                    owner.ImageSize, owner.ImageSize);
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += owner.ImageSize + owner.ScaleDpi(12);
            }
            
            // Draw text
            var textRect = new Rectangle(x, itemRect.Y, itemRect.Width - x - owner.ScaleDpi(12), itemRect.Height);
            
            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            
            // Draw submenu arrow
            if (item.Children != null && item.Children.Count > 0)
            {
                var arrowRect = new Rectangle(itemRect.Right - owner.ScaleDpi(20), 
                    itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                DrawBoldArrow(g, arrowRect, !item.IsEnabled, isHovered || isSelected, theme);
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
                    using (var tempBitmap = new Bitmap(rect.Width, rect.Height))
                    {
                        using (var tempG = Graphics.FromImage(tempBitmap))
                        {
                            TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(
                                tempG, new Rectangle(0, 0, rect.Width, rect.Height), imagePath);
                        }
                        ControlPaint.DrawImageDisabled(g, tempBitmap, rect.X, rect.Y, Color.Transparent);
                    }
                }
                else
                {
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(g, rect, imagePath);
                }
            }
            catch { }
        }
        
        private void DrawBoldArrow(Graphics g, Rectangle rect, bool isDisabled, bool isHighlighted, IBeepTheme theme)
        {
            var arrowColor = isHighlighted ? theme.MenuItemHoverForeColor : 
                (isDisabled ? theme.DisabledForeColor : theme.MenuItemForeColor);
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
            int x = owner.ScaleDpi(12);
            int width = owner.Width - owner.ScaleDpi(24);
            
            using (var pen = new Pen(theme.MenuBorderColor, 1))
            {
                g.DrawLine(pen, x, lineY, x + width, lineY);
            }
        }
        
        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
    }
}
