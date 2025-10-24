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
    /// Flat bold color Style context menu painter (Paper Style)
    /// </summary>
    public class FlatContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Paper;

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Flat background using Menu colors
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
            
            int y = 4;
            
            foreach (var item in items)
            {
                if (IsSeparator(item))
                {
                DrawSeparator(g, owner, item, y, metrics, theme);
                y += 8;
                continue;
            }
            
            int itemHeight = metrics.ItemHeight;
            var itemRect = new Rectangle(2, y, owner.Width - 4, itemHeight);
            
            DrawItem(g, owner, item, itemRect, item == selectedItem, item == hoveredItem, metrics, theme);                y += itemHeight;
            }
        }
        
        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Bold border with menu border color
            using (var pen = new Pen(metrics.BorderColor, metrics.BorderWidth))
            {
                var borderRect = bounds;
                borderRect.Width -= (int)metrics.BorderWidth;
                borderRect.Height -= (int)metrics.BorderWidth;
                borderRect.Inflate(1, 1);
                g.DrawRectangle(pen, borderRect);
            }
        }
        
        public int GetPreferredItemHeight()
        {
            return 30;
        }
        
        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Draw bold flat hover effect using Menu colors
            if (isHovered && item.IsEnabled)
            {
                using (var brush = new SolidBrush(metrics.ItemHoverBackColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
            else if (isSelected && item.IsEnabled)
            {
                using (var brush = new SolidBrush(metrics.ItemSelectedBackColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
            
            // Determine text color based on state
            Color textColor;
            if (!item.IsEnabled)
            {
                textColor = metrics.ItemDisabledForeColor;
            }
            else if (isHovered)
            {
                textColor = metrics.ItemHoverForeColor;
            }
            else if (isSelected)
            {
                textColor = metrics.ItemSelectedForeColor;
            }
            else
            {
                textColor = metrics.ItemForeColor;
            }
            
            int x = itemRect.X + 12;
            
            // Draw checkbox if checkable
            if (owner.ShowCheckBox && item.IsCheckable)
            {
                var checkRect = new Rectangle(x, itemRect.Y + (itemRect.Height - 16) / 2, 
                    16, 16);
                DrawCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics, theme);
                x += 24;
            }
            
            // Draw icon
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - owner.ImageSize) / 2, 
                    owner.ImageSize, owner.ImageSize);
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += owner.ImageSize + 12;
            }
            
            // Draw text
            var textRect = new Rectangle(x, itemRect.Y, itemRect.Width - x - 12, itemRect.Height);
            
            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            
            // Draw submenu arrow
            if (item.Children != null && item.Children.Count > 0)
            {
                var arrowRect = new Rectangle(itemRect.Right - 20, 
                    itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                DrawBoldArrow(g, arrowRect, !item.IsEnabled, isHovered || isSelected, metrics, theme);
            }
        }
        
        private void DrawCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Draw checkbox border
            var borderColor = isDisabled ? metrics.ItemDisabledForeColor : metrics.ItemForeColor;
            using (var pen = new Pen(borderColor, 2))
            {
                g.DrawRectangle(pen, rect);
            }
            
            // Draw check mark if checked
            if (isChecked)
            {
                using (var checkBrush = new SolidBrush(isDisabled ? metrics.ItemDisabledForeColor : metrics.AccentColor))
                {
                    // Draw checkmark
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
        
        private void DrawBoldArrow(Graphics g, Rectangle rect, bool isDisabled, bool isHighlighted, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            var arrowColor = isHighlighted ? metrics.ItemHoverBackColor : 
                (isDisabled ? metrics.ItemDisabledBackColor : metrics.ItemBackColor);
            using (var pen = new Pen(arrowColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                int midY = rect.Y + rect.Height / 2;
                g.DrawLine(pen, rect.X + 4, midY - 4, rect.X + rect.Width - 4, midY);
                g.DrawLine(pen, rect.X + rect.Width - 4, midY, rect.X + 4, midY + 4);
            }
        }
        
        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (!owner.ShowSeparators) return;
            
            int lineY = y + 4;
            int x = 12;
            int width = owner.Width - 24;
            
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
