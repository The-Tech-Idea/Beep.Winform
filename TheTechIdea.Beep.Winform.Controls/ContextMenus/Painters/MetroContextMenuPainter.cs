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
    public class MetroContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Metro;

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public int GetPreferredItemHeight() => 32;

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
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

            int y = 0;
            foreach (var item in items)
            {
                if (!item.IsVisible) continue;
                
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, metrics);
                    y += metrics.SeparatorHeight;
                    continue;
                }
                
                var itemRect = new Rectangle(0, y, owner.Width, metrics.ItemHeight);
                
                DrawItem(g, owner, item, itemRect, 
                    item == selectedItem, item == hoveredItem, metrics, theme);
                
                y += metrics.ItemHeight;
            }
        }

        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            using (var pen = new Pen(metrics.BorderColor, metrics.BorderWidth))
            {
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }

        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
        {
            bool visuallySelected = isSelected || item.IsSelected;
            
            if (item.IsEnabled && (visuallySelected || isHovered))
            {
                var backColor = visuallySelected ? metrics.ItemSelectedBackColor : metrics.ItemHoverBackColor;
                using (var brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }

            int x = itemRect.X + metrics.ItemPaddingLeft;

            if (owner.ShowCheckBox && item.IsCheckable)
            {
                var checkRect = new Rectangle(x, itemRect.Y + (itemRect.Height - metrics.CheckboxSize) / 2,
                    metrics.CheckboxSize, metrics.CheckboxSize);
                DrawMetroCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
                x += metrics.CheckboxSize + metrics.IconTextSpacing;
            }

            if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - metrics.IconSize) / 2,
                    metrics.IconSize, metrics.IconSize);
                DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
                x += metrics.IconSize + metrics.IconTextSpacing;
            }

            int rightReserved = metrics.ItemPaddingRight;
            if (item.Children?.Count > 0) rightReserved += metrics.SubmenuArrowSize + 8;
            string shortcut = item.ShortcutText ?? item.Shortcut;
            if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcut)) rightReserved += 60;

            var textRect = new Rectangle(x, itemRect.Y, itemRect.Width - x - rightReserved, itemRect.Height);
            var textColor = !item.IsEnabled ? metrics.ItemDisabledForeColor :
                           visuallySelected ? metrics.ItemSelectedForeColor :
                           isHovered ? metrics.ItemHoverForeColor : metrics.ItemForeColor;

            TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcut))
            {
                var shortcutRect = new Rectangle(itemRect.Right - rightReserved, itemRect.Y, 60, itemRect.Height);
                var shortcutColor = !item.IsEnabled ? metrics.ItemDisabledForeColor :
                                   visuallySelected ? metrics.ItemSelectedForeColor : metrics.ShortcutForeColor;
                TextRenderer.DrawText(g, shortcut, owner.ShortcutFont, shortcutRect, shortcutColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }

            if (item.Children?.Count > 0)
            {
                DrawMetroArrow(g, itemRect, !item.IsEnabled, metrics, textColor);
            }
        }

        private void DrawIcon(Graphics g, Rectangle rect, string imagePath, bool isDisabled)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            try
            {
                if (isDisabled)
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

        private void DrawMetroCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, ContextMenuMetrics metrics)
        {
            var checkColor = isDisabled ? metrics.ItemDisabledForeColor : metrics.ItemForeColor;
            using (var pen = new Pen(checkColor, 2))
            {
                g.DrawRectangle(pen, rect);
            }

            if (isChecked)
            {
                using (var brush = new SolidBrush(checkColor))
                {
                    g.FillRectangle(brush, rect.X + 3, rect.Y + 3, rect.Width - 6, rect.Height - 6);
                }
            }
        }

        private void DrawMetroArrow(Graphics g, Rectangle itemRect, bool isDisabled, 
            ContextMenuMetrics metrics, Color arrowColor)
        {
            var arrowRect = new Rectangle(itemRect.Right - metrics.ItemPaddingRight - metrics.SubmenuArrowSize,
                itemRect.Y + (itemRect.Height - metrics.SubmenuArrowSize) / 2,
                metrics.SubmenuArrowSize, metrics.SubmenuArrowSize);

            using (var brush = new SolidBrush(arrowColor))
            {
                Point[] points = new Point[]
                {
                    new Point(arrowRect.X + 4, arrowRect.Y + 4),
                    new Point(arrowRect.Right - 4, arrowRect.Y + arrowRect.Height / 2),
                    new Point(arrowRect.X + 4, arrowRect.Bottom - 4)
                };
                g.FillPolygon(brush, points);
            }
        }

        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, ContextMenuMetrics metrics)
        {
            if (!owner.ShowSeparators) return;
            int lineY = y + metrics.SeparatorHeight / 2;
            using (var pen = new Pen(metrics.SeparatorColor, 1))
            {
                g.DrawLine(pen, 0, lineY, owner.Width, lineY);
            }
        }

        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
    }
}
