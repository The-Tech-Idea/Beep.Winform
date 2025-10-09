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
    /// Minimal flat style context menu painter
    /// </summary>
    public class MinimalContextMenuPainter : IContextMenuPainter
    {
        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, IBeepTheme theme)
        {
            // Clean flat background using Menu colors
            using (var brush = new SolidBrush(theme.MenuBackColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }
        
        public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, IBeepTheme theme)
        {
            if (items == null || items.Count == 0) return;
            
            int y = owner.ScaleDpi(2); // Minimal padding
            
            foreach (var item in items)
            {
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, theme);
                    y += owner.ScaleDpi(8);
                    continue;
                }
                
                int itemHeight = GetPreferredItemHeight();
                var itemRect = new Rectangle(0, y, owner.Width, itemHeight);
                
                DrawItem(g, owner, item, itemRect, item == selectedItem, item == hoveredItem, theme);
                
                y += itemHeight;
            }
        }
        
        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, IBeepTheme theme)
        {
            // Thin border only using Menu border color
            using (var pen = new Pen(theme.MenuBorderColor, 1))
            {
                var borderRect = bounds;
                borderRect.Width -= 1;
                borderRect.Height -= 1;
                g.DrawRectangle(pen, borderRect);
            }
        }
        
        public int GetPreferredItemHeight()
        {
            return 26; // Compact items
        }
        
        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, IBeepTheme theme)
        {
            // Draw minimal hover effect using Menu colors
            if (isHovered && item.IsEnabled)
            {
                using (var brush = new SolidBrush(theme.MenuItemHoverBackColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
                
                // Left accent line
                using (var pen = new Pen(theme.MenuItemSelectedBackColor, 2))
                {
                    g.DrawLine(pen, itemRect.Left, itemRect.Top, itemRect.Left, itemRect.Bottom);
                }
            }
            
            int x = owner.ScaleDpi(8);
            
            // Draw icon
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = owner.LayoutHelper.GetIconRect(item);
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += owner.ImageSize + owner.ScaleDpi(8);
            }
            
            // Draw text with Menu colors
            var textRect = owner.LayoutHelper.GetTextRect(item);
            var textColor = !item.IsEnabled ? theme.DisabledForeColor : 
                           isHovered ? theme.MenuItemHoverForeColor :
                           theme.MenuItemForeColor;
            
            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            
            // Draw shortcut using KeyCombination
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(item.KeyCombination))
            {
                var shortcutRect = owner.LayoutHelper.GetShortcutRect(item);
                var shortcutColor = !item.IsEnabled ? theme.DisabledForeColor : 
                                   Color.FromArgb(150, theme.MenuItemForeColor);
                
                TextRenderer.DrawText(g, item.KeyCombination, owner.ShortcutFont, shortcutRect, shortcutColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
            
            // Draw submenu arrow
            if (item.Children != null && item.Children.Count > 0)
            {
                var arrowRect = owner.LayoutHelper.GetArrowRect(item);
                DrawSimpleArrow(g, arrowRect, !item.IsEnabled, theme);
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
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(
                        g, rect, imagePath, BeepControlStyle.Minimal);
                }
            }
            catch { }
        }
        
        private void DrawSimpleArrow(Graphics g, Rectangle rect, bool isDisabled, IBeepTheme theme)
        {
            var arrowColor = isDisabled ? theme.DisabledForeColor : theme.MenuItemForeColor;
            using (var pen = new Pen(arrowColor, 1))
            {
                int midY = rect.Y + rect.Height / 2;
                g.DrawLine(pen, rect.X + 4, midY - 3, rect.X + rect.Width - 4, midY);
                g.DrawLine(pen, rect.X + rect.Width - 4, midY, rect.X + 4, midY + 3);
            }
        }
        
        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, IBeepTheme theme)
        {
            if (!owner.ShowSeparators) return;
            
            int lineY = y + owner.ScaleDpi(4);
            int x = owner.ScaleDpi(8);
            int width = owner.Width - owner.ScaleDpi(16);
            
            using (var pen = new Pen(Color.FromArgb(50, theme.MenuBorderColor), 1))
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
