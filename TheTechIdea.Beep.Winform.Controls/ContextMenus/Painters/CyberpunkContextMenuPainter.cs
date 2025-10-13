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
    /// Cyberpunk theme context menu painter with neon glows and futuristic aesthetics
    /// Features electric blue/pink neon colors, glowing effects, and cyberpunk styling
    /// </summary>
    public class CyberpunkContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Cyberpunk;

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
            // Cyberpunk dark background with neon glow
            using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
            {
                // Dark base
                using (var brush = new SolidBrush(Color.FromArgb(255, 10, 10, 20)))
                {
                    g.FillPath(brush, path);
                }

                // Neon glow overlay
                using (var glowBrush = new LinearGradientBrush(bounds,
                    Color.FromArgb(30, 0, 255, 255), // Electric blue
                    Color.FromArgb(20, 255, 0, 255), // Neon pink
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(glowBrush, path);
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
            {
                // Neon blue border
                using (var pen = new Pen(Color.FromArgb(255, 0, 255, 255), 2))
                {
                    g.DrawPath(pen, path);
                }

                // Inner pink glow
                var innerRect = new Rectangle(bounds.X + 1, bounds.Y + 1,
                    bounds.Width - 2, bounds.Height - 2);
                using (var innerPath = GetRoundedRect(innerRect, metrics.BorderRadius - 1))
                using (var glowPen = new Pen(Color.FromArgb(100, 255, 0, 255), 1))
                {
                    g.DrawPath(glowPen, innerPath);
                }
            }
        }

        #region Private Helper Methods

        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect,
            bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (!item.IsVisible) return;

            bool visuallySelected = isSelected || item.IsSelected;

            // Cyberpunk neon selection
            if (item.IsEnabled)
            {
                if (visuallySelected)
                {
                    using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                    {
                        // Electric blue selection with glow
                        using (var brush = new SolidBrush(Color.FromArgb(255, 0, 50, 100)))
                        {
                            g.FillPath(brush, path);
                        }

                        // Neon border glow
                        using (var pen = new Pen(Color.FromArgb(255, 0, 255, 255), 2))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
                else if (isHovered)
                {
                    using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(30, 0, 255, 255)))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                }
            }

            int x = itemRect.X + metrics.ItemPaddingLeft;

            // Cyberpunk checkbox with neon theme
            if (owner.ShowCheckBox && item.IsCheckable)
            {
                var checkRect = new Rectangle(x,
                    itemRect.Y + (itemRect.Height - metrics.CheckboxSize) / 2,
                    metrics.CheckboxSize, metrics.CheckboxSize);

                DrawCyberpunkCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
                x += metrics.CheckboxSize + metrics.IconTextSpacing;
            }

            // Icon
            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x,
                    itemRect.Y + (itemRect.Height - metrics.IconSize) / 2,
                    metrics.IconSize, metrics.IconSize);

                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += metrics.IconSize + metrics.IconTextSpacing;
            }

            // Calculate text area
            int rightReserved = metrics.ItemPaddingRight;
            if (item.Children != null && item.Children.Count > 0)
            {
                rightReserved += metrics.SubmenuArrowSize + 8;
            }

            string shortcutDisplay = !string.IsNullOrEmpty(item.ShortcutText) ? item.ShortcutText : item.Shortcut;
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcutDisplay))
            {
                rightReserved += 60;
            }

            // Cyberpunk neon text colors
            var textRect = new Rectangle(x, itemRect.Y,
                itemRect.Width - x - rightReserved,
                itemRect.Height);

            var textColor = !item.IsEnabled ? Color.FromArgb(255, 100, 100, 100) :
                           visuallySelected ? Color.FromArgb(255, 0, 255, 255) :
                           isHovered ? Color.FromArgb(255, 255, 0, 255) :
                           Color.FromArgb(255, 200, 200, 200);

            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            // Shortcut text
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcutDisplay))
            {
                var shortcutColor = item.IsEnabled ? Color.FromArgb(255, 100, 100, 100) :
                                   visuallySelected ? Color.FromArgb(255, 0, 255, 255) :
                                   isHovered ? Color.FromArgb(255, 255, 0, 255) :
                                   Color.FromArgb(255, 150, 150, 150);

                var shortcutRect = new Rectangle(
                    itemRect.Right - rightReserved,
                    itemRect.Y, 60, itemRect.Height);

                TextRenderer.DrawText(g, shortcutDisplay, owner.ShortcutFont, shortcutRect, shortcutColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // Submenu arrow
            if (item.Children != null && item.Children.Count > 0)
            {
                var arrowColor = item.IsEnabled ? Color.FromArgb(255, 100, 100, 100) :
                               visuallySelected ? Color.FromArgb(255, 0, 255, 255) :
                               isHovered ? Color.FromArgb(255, 255, 0, 255) :
                               Color.FromArgb(255, 150, 150, 150);

                var arrowRect = new Rectangle(
                    itemRect.Right - metrics.SubmenuArrowSize - 8,
                    itemRect.Y + (itemRect.Height - metrics.SubmenuArrowSize) / 2,
                    metrics.SubmenuArrowSize, metrics.SubmenuArrowSize);

                DrawSubmenuArrow(g, arrowRect, arrowColor);
            }
        }

        private void DrawCyberpunkCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, ContextMenuMetrics metrics)
        {
            using (var path = GetRoundedRect(rect, 2))
            {
                // Background
                var bgColor = isDisabled ? Color.FromArgb(255, 30, 30, 40) :
                           Color.FromArgb(255, 0, 20, 40);
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Neon border
                var borderColor = isDisabled ? Color.FromArgb(255, 100, 100, 100) :
                              Color.FromArgb(255, 0, 255, 255);
                using (var pen = new Pen(borderColor, 2))
                {
                    g.DrawPath(pen, path);
                }

                // Check mark
                if (isChecked)
                {
                    var checkColor = isDisabled ? Color.FromArgb(255, 150, 150, 150) :
                                 Color.FromArgb(255, 255, 0, 255);
                    using (var checkPen = new Pen(checkColor, 2))
                    {
                        var checkPoints = new Point[] {
                            new Point(rect.X + 3, rect.Y + rect.Height / 2),
                            new Point(rect.X + rect.Width / 2 - 1, rect.Y + rect.Height - 3),
                            new Point(rect.X + rect.Width - 3, rect.Y + 3)
                        };
                        g.DrawLines(checkPen, checkPoints);
                    }
                }
            }
        }

        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, ContextMenuMetrics metrics)
        {
            int separatorY = y + metrics.SeparatorHeight / 2;
            using (var pen = new Pen(Color.FromArgb(100, 0, 255, 255), 1))
            {
                g.DrawLine(pen, metrics.Padding + 8, separatorY,
                         owner.Width - metrics.Padding - 8, separatorY);
            }
        }

        private void DrawIcon(Graphics g, Rectangle rect, string imagePath, bool disabled)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            
            try
            {
                if (disabled)
                {
                    using (var temp = new Bitmap(rect.Width, rect.Height))
                    {
                        using (var tempG = Graphics.FromImage(temp))
                        {
                            TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(
                                tempG, new Rectangle(0, 0, rect.Width, rect.Height), imagePath);
                        }
                        ControlPaint.DrawImageDisabled(g, temp, rect.X, rect.Y, Color.Transparent);
                    }
                }
                else
                {
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(g, rect, imagePath);
                }
            }
            catch { }
        }

        private void DrawSubmenuArrow(Graphics g, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 2))
            {
                var centerY = rect.Y + rect.Height / 2;
                var points = new Point[] {
                    new Point(rect.X + 2, centerY - 3),
                    new Point(rect.X + rect.Width - 2, centerY),
                    new Point(rect.X + 2, centerY + 3)
                };
                g.DrawLines(pen, points);
            }
        }

        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || string.IsNullOrEmpty(item.DisplayField));
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}