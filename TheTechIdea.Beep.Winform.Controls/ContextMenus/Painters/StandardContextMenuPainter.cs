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
    /// Standard Windows-style context menu painter
    /// </summary>
    public class StandardContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Modern; // Standard style maps to Modern

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public int GetPreferredItemHeight()
        {
            return 28;
        }

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Draw background
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }
        
        public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (items == null || items.Count == 0) return;
            
            int y = metrics.Padding;
            
            foreach (var item in items)
            {
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, metrics, theme);
                    y += 8;
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
            // Draw border
            using (var pen = new Pen(metrics.BorderColor, metrics.BorderWidth))
            {
                var borderRect = bounds;
                borderRect.Width -= (int)metrics.BorderWidth;
                borderRect.Height -= (int)metrics.BorderWidth;
                g.DrawRectangle(pen, borderRect);
            }
            
            // Draw shadow effect
            DrawShadow(g, bounds);
        }
        
        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Draw background
            if (isHovered && item.IsEnabled)
            {
                using (var brush = new SolidBrush(metrics.ItemHoverBackColor))
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
            
            int x = 8;
            
            // Draw checkbox
            if (owner.ShowCheckBox)
            {
                var checkRect = owner.LayoutHelper.GetCheckBoxRect(item);
                DrawCheckBox(g, checkRect, item.IsChecked, !item.IsEnabled, theme);
                x += 20;
            }
            
            // Draw icon
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = owner.LayoutHelper.GetIconRect(item);
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += owner.ImageSize + 4;
            }
            
            // Draw text with proper Menu colors
            var textRect = owner.LayoutHelper.GetTextRect(item);
            var textColor = !item.IsEnabled ? theme.DisabledForeColor : 
                           isHovered ? theme.MenuItemHoverForeColor :
                           isSelected ? theme.MenuItemSelectedForeColor :
                           theme.MenuItemForeColor;
            
            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            
            // Draw shortcut using KeyCombination property
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(item.KeyCombination))
            {
                var shortcutRect = owner.LayoutHelper.GetShortcutRect(item);
                var shortcutColor = !item.IsEnabled ? theme.DisabledForeColor : 
                                   Color.FromArgb(128, theme.MenuItemForeColor);
                
                TextRenderer.DrawText(g, item.KeyCombination, owner.ShortcutFont, shortcutRect, shortcutColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
            
            // Draw submenu arrow
            if (item.Children != null && item.Children.Count > 0)
            {
                var arrowRect = owner.LayoutHelper.GetArrowRect(item);
                DrawSubmenuArrow(g, arrowRect, !item.IsEnabled, theme);
            }
        }
        
        private void DrawCheckBox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, IBeepTheme theme)
        {
            // Draw checkbox border using Menu border color
            var borderColor = isDisabled ? theme.DisabledForeColor : theme.MenuBorderColor;
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
            
            // Draw check mark
            if (isChecked)
            {
                var checkColor = isDisabled ? theme.DisabledForeColor : theme.MenuItemSelectedBackColor;
                using (var pen = new Pen(checkColor, 2))
                {
                    var points = new Point[]
                    {
                        new Point(rect.X + 3, rect.Y + rect.Height / 2),
                        new Point(rect.X + rect.Width / 2 - 1, rect.Y + rect.Height - 4),
                        new Point(rect.X + rect.Width - 3, rect.Y + 3)
                    };
                    g.DrawLines(pen, points);
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
                    // Use StyledImagePainter directly
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(g, rect, imagePath);
                }
            }
            catch
            {
                // Image not found or invalid path
            }
        }
        
        private void DrawSubmenuArrow(Graphics g, Rectangle rect, bool isDisabled, IBeepTheme theme)
        {
            var arrowColor = isDisabled ? theme.DisabledForeColor : theme.MenuItemForeColor;
            using (var pen = new Pen(arrowColor, 2))
            {
                int midY = rect.Y + rect.Height / 2;
                var points = new Point[]
                {
                    new Point(rect.X + 4, rect.Y + 4),
                    new Point(rect.X + rect.Width - 4, midY),
                    new Point(rect.X + 4, rect.Y + rect.Height - 4)
                };
                g.DrawLines(pen, points);
            }
        }
        
        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (!owner.ShowSeparators) return;
            
            int lineY = y + 4;
            int x = 8;
            int width = owner.Width - 16;
            
            using (var pen = new Pen(metrics.SeparatorColor, 1))
            {
                g.DrawLine(pen, x, lineY, x + width, lineY);
            }
        }
        
        private void DrawShadow(Graphics g, Rectangle bounds)
        {
            // Draw subtle drop shadow
            int shadowSize = 5;
            int shadowAlpha = 30;
            
            // Right shadow
            for (int i = 0; i < shadowSize; i++)
            {
                int alpha = shadowAlpha - (i * shadowAlpha / shadowSize);
                using (var pen = new Pen(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.DrawLine(pen, bounds.Right + i, bounds.Top + i, bounds.Right + i, bounds.Bottom + i);
                }
            }
            
            // Bottom shadow
            for (int i = 0; i < shadowSize; i++)
            {
                int alpha = shadowAlpha - (i * shadowAlpha / shadowSize);
                using (var pen = new Pen(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.DrawLine(pen, bounds.Left + i, bounds.Bottom + i, bounds.Right + i, bounds.Bottom + i);
                }
            }
        }
        
        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
    }
}
