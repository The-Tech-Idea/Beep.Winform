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
    /// Modern context menu painter with clean, contemporary design
    /// Features rounded corners, subtle shadows, and smooth animations
    /// </summary>
    public class ModernContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Modern;

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
            // Draw modern background with subtle gradient
            using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
            {
                // Main background with subtle gradient
                using (var brush = new LinearGradientBrush(bounds, 
                    metrics.BackgroundColor, 
                    ControlPaint.Light(metrics.BackgroundColor, 0.02f), 
                    90f))
                {
                    g.FillPath(brush, path);
                }
                
                // Draw shadow if enabled
                if (metrics.ShowElevation)
                {
                    DrawModernShadow(g, bounds, metrics);
                }
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
                    DrawSeparator(g, owner, item, y, metrics);
                    y += metrics.SeparatorHeight;
                    continue;
                }
                
                int itemHeight = metrics.ItemHeight;
                var itemRect = new Rectangle(metrics.Padding, y, 
                    owner.Width - (metrics.Padding * 2), itemHeight);
                
                DrawItem(g, owner, item, itemRect, 
                    item == selectedItem, item == hoveredItem, metrics, theme);
                
                y += itemHeight;
            }
        }

        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
            using (var pen = new Pen(metrics.BorderColor, metrics.BorderWidth))
            {
                g.DrawPath(pen, path);
            }
        }

        #region Private Helper Methods

        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Skip if item is not visible
            if (!item.IsVisible) return;

            // Priority: Selected > Hovered > Normal
            // Note: isSelected parameter is for visual selection, item.IsSelected is the data model state
            bool visuallySelected = isSelected || item.IsSelected;
            
            // Draw hover/selection background with proper state handling
            if (item.IsEnabled)
            {
                if (visuallySelected)
                {
                    // Selected state (highest priority)
                    using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                    {
                        using (var brush = new SolidBrush(metrics.ItemSelectedBackColor))
                        {
                            g.FillPath(brush, path);
                        }
                        
                        // Optional: subtle border for selected items
                        using (var pen = new Pen(Color.FromArgb(40, metrics.AccentColor), 1))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
                else if (isHovered)
                {
                    // Hover state
                    using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                    {
                        using (var brush = new SolidBrush(metrics.ItemHoverBackColor))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                }
            }

            int x = itemRect.X + metrics.ItemPaddingLeft;

            // Draw checkbox if item is checkable (using IsCheckable property)
            if (owner.ShowCheckBox && item.IsCheckable)
            {
                var checkRect = new Rectangle(x,
                    itemRect.Y + (itemRect.Height - metrics.CheckboxSize) / 2,
                    metrics.CheckboxSize, metrics.CheckboxSize);
                
                DrawCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
                x += metrics.CheckboxSize + metrics.IconTextSpacing;
            }

            // Draw icon
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x, 
                    itemRect.Y + (itemRect.Height - metrics.IconSize) / 2, 
                    metrics.IconSize, metrics.IconSize);
                
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += metrics.IconSize + metrics.IconTextSpacing;
            }

            // Calculate available width for text (account for submenu arrow and shortcut)
            int rightReserved = metrics.ItemPaddingRight;
            if (item.Children != null && item.Children.Count > 0)
            {
                rightReserved += metrics.SubmenuArrowSize + 8;
            }
            
            // Use ShortcutText property (falls back to Shortcut if ShortcutText is empty)
            string shortcutDisplay = !string.IsNullOrEmpty(item.ShortcutText) ? item.ShortcutText : item.Shortcut;
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcutDisplay))
            {
                rightReserved += 60; // Reserve space for shortcut
            }

            // Draw text with proper state colors
            var textRect = new Rectangle(x, itemRect.Y, 
                itemRect.Width - x - rightReserved, 
                itemRect.Height);
            
            var textColor = !item.IsEnabled ? metrics.ItemDisabledForeColor :
                           visuallySelected ? metrics.ItemSelectedForeColor :
                           isHovered ? metrics.ItemHoverForeColor :
                           metrics.ItemForeColor;

            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            // Draw shortcut text if available (use ShortcutText or fall back to Shortcut)
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcutDisplay))
            {
                var shortcutColor = !item.IsEnabled ? metrics.ItemDisabledForeColor :
                                   visuallySelected ? metrics.ItemSelectedForeColor :
                                   isHovered ? metrics.ShortcutHoverForeColor :
                                   metrics.ShortcutForeColor;

                var shortcutRect = new Rectangle(
                    itemRect.Right - rightReserved,
                    itemRect.Y, 60, itemRect.Height);

                TextRenderer.DrawText(g, shortcutDisplay, owner.ShortcutFont, shortcutRect, shortcutColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // Draw submenu arrow
            if (item.Children != null && item.Children.Count > 0)
            {
                DrawSubmenuArrow(g, itemRect, !item.IsEnabled, metrics, textColor);
            }
        }

        private void DrawCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, ContextMenuMetrics metrics)
        {
            // Draw checkbox border
            var checkColor = isDisabled ? metrics.ItemDisabledForeColor : metrics.ItemForeColor;
            using (var pen = new Pen(checkColor, 1))
            using (var path = GetRoundedRect(rect, 2))
            {
                g.DrawPath(pen, path);
            }

            // Draw check mark if checked
            if (isChecked)
            {
                using (var pen = new Pen(checkColor, 2))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    // Draw check mark
                    int centerX = rect.X + rect.Width / 2;
                    int centerY = rect.Y + rect.Height / 2;
                    
                    g.DrawLine(pen, 
                        rect.X + 3, centerY,
                        centerX - 1, rect.Bottom - 4);
                    g.DrawLine(pen,
                        centerX - 1, rect.Bottom - 4,
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
                    // For disabled state, create a grayscale/dimmed version
                    using (var tempBitmap = new Bitmap(rect.Width, rect.Height))
                    {
                        using (var tempG = Graphics.FromImage(tempBitmap))
                        {
                            // Draw using StyledImagePainter
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
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(
                        g, rect, imagePath);
                }
            }
            catch
            {
                // Image not found or invalid path - fail silently
            }
        }

        private void DrawSubmenuArrow(Graphics g, Rectangle itemRect, bool isDisabled, 
            ContextMenuMetrics metrics, Color arrowColor)
        {
            var arrowRect = new Rectangle(
                itemRect.Right - metrics.ItemPaddingRight - metrics.SubmenuArrowSize,
                itemRect.Y + (itemRect.Height - metrics.SubmenuArrowSize) / 2,
                metrics.SubmenuArrowSize, metrics.SubmenuArrowSize);

            using (var pen = new Pen(arrowColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int midY = arrowRect.Y + arrowRect.Height / 2;
                int leftX = arrowRect.X + 4;
                int rightX = arrowRect.Right - 4;

                g.DrawLine(pen, leftX, midY - 4, rightX, midY);
                g.DrawLine(pen, rightX, midY, leftX, midY + 4);
            }
        }

        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, 
            ContextMenuMetrics metrics)
        {
            if (!owner.ShowSeparators) return;

            int lineY = y + metrics.SeparatorHeight / 2;
            int x = metrics.ItemPaddingLeft;
            int width = owner.Width - metrics.ItemPaddingLeft - metrics.ItemPaddingRight;

            using (var pen = new Pen(metrics.SeparatorColor, 1))
            {
                g.DrawLine(pen, x, lineY, x + width, lineY);
            }
        }

        private void DrawModernShadow(Graphics g, Rectangle bounds, ContextMenuMetrics metrics)
        {
            int shadowSize = metrics.ShadowDepth;
            int shadowAlpha = metrics.ShadowAlpha;

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

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

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

        #endregion
    }
}
