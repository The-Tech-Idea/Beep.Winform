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
    /// Minimal flat style context menu painter
    /// </summary>
    public class MinimalContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Minimal;

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
            // Clean flat background
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
                    y += metrics.SeparatorHeight;
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
            // Thin border
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
            if (!item.IsVisible) return;

            bool visuallySelected = isSelected || item.IsSelected;

            // Draw hover effect
            if (item.IsEnabled && (visuallySelected || isHovered))
            {
                var backColor = visuallySelected ? metrics.ItemSelectedBackColor : metrics.ItemHoverBackColor;
                using (var brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
                
                // Left accent line
                using (var pen = new Pen(metrics.AccentColor, 2))
                {
                    g.DrawLine(pen, itemRect.Left, itemRect.Top, itemRect.Left, itemRect.Bottom);
                }
            }
            
            int x = itemRect.X + metrics.ItemPaddingLeft;
            
            // Draw checkbox
            if (owner.ShowCheckBox && item.IsCheckable)
            {
                var checkRect = new Rectangle(x, itemRect.Y + (itemRect.Height - metrics.CheckboxSize) / 2, 
                    metrics.CheckboxSize, metrics.CheckboxSize);
                DrawCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
                x += metrics.CheckboxSize + metrics.IconTextSpacing;
            }
            
            // Draw icon
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - metrics.IconSize) / 2, 
                    metrics.IconSize, metrics.IconSize);
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += metrics.IconSize + metrics.IconTextSpacing;
            }
            
            // Calculate text area
            int rightReserved = metrics.ItemPaddingRight;
            if (item.Children?.Count > 0) rightReserved += metrics.SubmenuArrowSize + 8;
            string shortcut = item.ShortcutText ?? item.Shortcut;
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcut)) rightReserved += 60;

            var textRect = new Rectangle(x, itemRect.Y, itemRect.Width - x - rightReserved, itemRect.Height);
            var textColor = !item.IsEnabled ? metrics.ItemDisabledForeColor : 
                           visuallySelected ? metrics.ItemForeColor :
                           isHovered ? metrics.ItemHoverForeColor : metrics.ItemHoverForeColor;
            
            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            
            // Draw shortcut
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcut))
            {
                var shortcutRect = new Rectangle(itemRect.Right - rightReserved, itemRect.Y, 60, itemRect.Height);
                var shortcutColor = !item.IsEnabled ? metrics.ItemDisabledForeColor : metrics.ShortcutForeColor;
                
                TextRenderer.DrawText(g, shortcut, owner.ShortcutFont, shortcutRect, shortcutColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }
            
            // Draw submenu arrow
            if (item.Children?.Count > 0)
            {
                var arrowRect = new Rectangle(itemRect.Right - 20, itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                DrawSimpleArrow(g, arrowRect, !item.IsEnabled, textColor);
            }
        }
        
        private void DrawCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, ContextMenuMetrics metrics)
        {
            var borderColor = isDisabled ? metrics.ItemDisabledForeColor : metrics.ItemForeColor;
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, rect);
            }
            
            if (isChecked)
            {
                using (var checkBrush = new SolidBrush(isDisabled ? metrics.ItemDisabledForeColor : metrics.AccentColor))
                using (var pen = new Pen(checkBrush, 2))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    
                    g.DrawLine(pen, rect.X + 3, rect.Y + rect.Height / 2, 
                        rect.X + rect.Width / 2 - 1, rect.Bottom - 4);
                    g.DrawLine(pen, rect.X + rect.Width / 2 - 1, rect.Bottom - 4, 
                        rect.Right - 3, rect.Y + 3);
                }
            }
        }
        
        private void DrawIcon(Graphics g, Rectangle rect, string imagePath, bool isDisabled)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            
            try
            {
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
        
        private void DrawSimpleArrow(Graphics g, Rectangle rect, bool isDisabled, Color color)
        {
            using (var pen = new Pen(color, 1))
            {
                int midY = rect.Y + rect.Height / 2;
                g.DrawLine(pen, rect.X + 4, midY - 3, rect.X + rect.Width - 4, midY);
                g.DrawLine(pen, rect.X + rect.Width - 4, midY, rect.X + 4, midY + 3);
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
        
        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
    }
}
