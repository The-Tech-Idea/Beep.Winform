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
    /// Microsoft Fluent Design System context menu painter
    /// Features acrylic-like translucency, reveal effects, and smooth animations
    /// </summary>
    public class FluentContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Fluent;

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public int GetPreferredItemHeight()
        {
            return 34;
        }

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Fluent acrylic-like background
            using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
            {
                // Base acrylic background with subtle noise texture
                using (var brush = new SolidBrush(Color.FromArgb(250, metrics.BackgroundColor)))
                {
                    g.FillPath(brush, path);
                }

                // Add subtle light gradient for depth
                var innerBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, 
                    bounds.Width - 2, bounds.Height / 3);
                using (var gradBrush = new LinearGradientBrush(innerBounds,
                    Color.FromArgb(15, Color.White),
                    Color.FromArgb(0, Color.White), 90f))
                {
                    g.FillRectangle(gradBrush, innerBounds);
                }

                // Fluent reveal border highlight
                if (metrics.ShowElevation)
                {
                    DrawFluentGlow(g, bounds, metrics);
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
            using (var pen = new Pen(Color.FromArgb(180, metrics.BorderColor), metrics.BorderWidth))
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

            // Fluent reveal effect on hover with proper state handling
            if (item.IsEnabled)
            {
                if (visuallySelected)
                {
                    // Selected state with Fluent gradient
                    using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                    {
                        using (var brush = new LinearGradientBrush(itemRect,
                            Color.FromArgb(245, metrics.ItemSelectedBackColor),
                            Color.FromArgb(225, metrics.ItemSelectedBackColor), 90f))
                        {
                            g.FillPath(brush, path);
                        }

                        // Stronger reveal border on selection
                        using (var pen = new Pen(Color.FromArgb(80, metrics.AccentColor), 1))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
                else if (isHovered)
                {
                    // Hover state with Fluent smooth gradient
                    using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                    {
                        using (var brush = new LinearGradientBrush(itemRect,
                            Color.FromArgb(240, metrics.ItemHoverBackColor),
                            Color.FromArgb(220, metrics.ItemHoverBackColor), 90f))
                        {
                            g.FillPath(brush, path);
                        }

                        // Subtle reveal border on hover
                        using (var pen = new Pen(Color.FromArgb(40, metrics.AccentColor), 1))
                        {
                            g.DrawPath(pen, path);
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

            // Draw icon with Fluent styling
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

            // Draw text with Fluent typography
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

            // Draw submenu chevron
            if (item.Children != null && item.Children.Count > 0)
            {
                DrawFluentChevron(g, itemRect, !item.IsEnabled, metrics, textColor);
            }
        }

        private void DrawCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, ContextMenuMetrics metrics)
        {
            // Fluent-style checkbox
            var checkColor = isDisabled ? metrics.ItemDisabledForeColor : metrics.AccentColor;
            
            using (var path = GetRoundedRect(rect, 3))
            {
                if (isChecked)
                {
                    // Fill background when checked
                    using (var brush = new SolidBrush(Color.FromArgb(isDisabled ? 100 : 255, checkColor)))
                    {
                        g.FillPath(brush, path);
                    }
                }
                
                // Draw border
                using (var pen = new Pen(checkColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Draw check mark if checked
            if (isChecked)
            {
                var checkmarkColor = isDisabled ? metrics.ItemDisabledForeColor : Color.White;
                using (var pen = new Pen(checkmarkColor, 2))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

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
                        g, rect, imagePath);
                }
            }
            catch
            {
                // Image not found or invalid path
            }
        }

        private void DrawFluentChevron(Graphics g, Rectangle itemRect, bool isDisabled,
            ContextMenuMetrics metrics, Color arrowColor)
        {
            var chevronRect = new Rectangle(
                itemRect.Right - metrics.ItemPaddingRight - metrics.SubmenuArrowSize,
                itemRect.Y + (itemRect.Height - metrics.SubmenuArrowSize) / 2,
                metrics.SubmenuArrowSize, metrics.SubmenuArrowSize);

            using (var pen = new Pen(arrowColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int midY = chevronRect.Y + chevronRect.Height / 2;
                int leftX = chevronRect.X + 5;
                int rightX = chevronRect.Right - 5;

                // Fluent-style chevron
                g.DrawLine(pen, leftX, midY - 5, rightX, midY);
                g.DrawLine(pen, rightX, midY, leftX, midY + 5);
            }
        }

        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y,
            ContextMenuMetrics metrics)
        {
            if (!owner.ShowSeparators) return;

            int lineY = y + metrics.SeparatorHeight / 2;
            int x = metrics.ItemPaddingLeft;
            int width = owner.Width - metrics.ItemPaddingLeft - metrics.ItemPaddingRight;

            // Fluent separator with gradient
            using (var pen = new Pen(Color.FromArgb(40, metrics.SeparatorColor), 1))
            {
                g.DrawLine(pen, x, lineY, x + width, lineY);
            }
        }

        private void DrawFluentGlow(Graphics g, Rectangle bounds, ContextMenuMetrics metrics)
        {
            // Subtle outer glow for Fluent depth
            int glowSize = metrics.ShadowBlur;
            int baseAlpha = 25;

            for (int i = 0; i < glowSize; i++)
            {
                int alpha = baseAlpha - (i * baseAlpha / glowSize);
                using (var pen = new Pen(Color.FromArgb(alpha, metrics.AccentColor)))
                {
                    var glowBounds = bounds;
                    glowBounds.Inflate(i, i);
                    using (var path = GetRoundedRect(glowBounds, metrics.BorderRadius + i))
                    {
                        g.DrawPath(pen, path);
                    }
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
