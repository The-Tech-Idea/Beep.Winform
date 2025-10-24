using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    /// <summary>
    /// Microsoft Office Style context menu painter
    /// </summary>
    public class OfficeContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Modern; // Office Style maps to Modern

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public int GetPreferredItemHeight()
        {
            return 32;
        }

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Office-Style background with icon column
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }
            
            // Draw icon column background
            int iconColumnWidth = 32;
            var iconColumnRect = new Rectangle(0, 0, iconColumnWidth, bounds.Height);
            using (var brush = new SolidBrush(Color.FromArgb(240, metrics.BackgroundColor)))
            {
                g.FillRectangle(brush, iconColumnRect);
            }
        }
        
        public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (items == null || items.Count == 0) return;
            
            int y = 2;
            
            foreach (var item in items)
            {
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, metrics, theme);
                    y += 6;
                    continue;
                }
                
                int itemHeight = metrics.ItemHeight;
                var itemRect = new Rectangle(0, y, owner.Width, itemHeight);
                
                DrawItem(g, owner, item, itemRect, item == selectedItem, item == hoveredItem, metrics, theme);
                
                y += itemHeight;
            }
        }
        
        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Office-Style border
            using (var pen = new Pen(metrics.BorderColor, metrics.BorderWidth))
            {
                var borderRect = bounds;
                borderRect.Width -= (int)metrics.BorderWidth;
                borderRect.Height -= (int)metrics.BorderWidth;
                g.DrawRectangle(pen, borderRect);
            }
        }
        
        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
        {
            int iconColumnWidth = 32;
            
            // Draw hover/selection background (only in text area, not icon column)
            var textAreaRect = new Rectangle(iconColumnWidth, itemRect.Top, 
                itemRect.Width - iconColumnWidth, itemRect.Height);
            
            if (isHovered && item.IsEnabled)
            {
                // Office-Style hover with gradient using Menu colors
                using (var brush = new LinearGradientBrush(textAreaRect, 
                    Color.FromArgb(60, theme.MenuItemHoverBackColor),
                    Color.FromArgb(30, theme.MenuItemHoverBackColor), 90f))
                {
                    g.FillRectangle(brush, textAreaRect);
                }
                
                // Border around hover
                using (var pen = new Pen(Color.FromArgb(100, theme.MenuItemHoverBackColor), 1))
                {
                    var borderRect = textAreaRect;
                    borderRect.Width -= 1;
                    borderRect.Height -= 1;
                    g.DrawRectangle(pen, borderRect);
                }
            }
            else if (isSelected && item.IsEnabled)
            {
                using (var brush = new SolidBrush(Color.FromArgb(40, theme.MenuItemSelectedBackColor)))
                {
                    g.FillRectangle(brush, textAreaRect);
                }
            }
            
            // Draw checkbox in icon column
            if (owner.ShowCheckBox)
            {
                var checkRect = new Rectangle(8, 
                    itemRect.Y + (itemRect.Height - 16) / 2, 
                    16, 16);
                DrawOfficeCheckBox(g, checkRect, item.IsChecked, !item.IsEnabled, theme);
            }
            
            // Draw icon in icon column
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                int iconSize = 16;
                var iconRect = new Rectangle(8, 
                    itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
            }
            
            // Draw text with proper color states
            int textX = iconColumnWidth + 6;
            var textRect = new Rectangle(textX, itemRect.Y, 
                itemRect.Width - textX - 6, itemRect.Height);
            
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
            
            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            
            // Draw shortcut using KeyCombination
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(item.KeyCombination))
            {
                var shortcutRect = new Rectangle(itemRect.Right - 80, itemRect.Y, 
                        70, itemRect.Height);
                var shortcutColor = !item.IsEnabled ? theme.DisabledForeColor : 
                    Color.FromArgb(120, theme.MenuItemForeColor);
                
                TextRenderer.DrawText(g, item.KeyCombination, owner.ShortcutFont, shortcutRect, shortcutColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
            
            // Draw submenu arrow
            if (item.Children != null && item.Children.Count > 0)
            {
                var arrowRect = new Rectangle(itemRect.Right - 16, 
                    itemRect.Y + (itemRect.Height - 8) / 2, 8, 8);
                DrawOfficeArrow(g, arrowRect, !item.IsEnabled, theme);
            }
        }
        
        private void DrawOfficeCheckBox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, IBeepTheme theme)
        {
            if (isChecked)
            {
                // Draw check mark background using Menu colors
                using (var brush = new SolidBrush(isDisabled ? 
                    theme.DisabledForeColor : theme.MenuItemSelectedBackColor))
                {
                    g.FillRectangle(brush, rect);
                }
                
                // Draw check mark
                using (var pen = new Pen(theme.MenuItemSelectedForeColor, 2))
                {
                    g.DrawLine(pen, rect.X + 3, rect.Y + rect.Height / 2, 
                        rect.X + rect.Width / 2, rect.Y + rect.Height - 3);
                    g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + rect.Height - 3, 
                        rect.X + rect.Width - 2, rect.Y + 2);
                }
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
        
        private void DrawOfficeArrow(Graphics g, Rectangle rect, bool isDisabled, IBeepTheme theme)
        {
            var arrowColor = isDisabled ? theme.DisabledForeColor : theme.MenuItemForeColor;
            using (var brush = new SolidBrush(arrowColor))
            {
                Point[] triangle = new Point[]
                {
                    new Point(rect.X, rect.Y),
                    new Point(rect.X, rect.Bottom),
                    new Point(rect.Right, rect.Y + rect.Height / 2)
                };
                g.FillPolygon(brush, triangle);
            }
        }
        
        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (!owner.ShowSeparators) return;
            
            int lineY = y + 3;
            int iconColumnWidth = 32;
            int x = iconColumnWidth + 2;
            int width = owner.Width - x - 4;
            
            using (var pen = new Pen(metrics.SeparatorColor, 1))
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
