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
    public class NeoMorphismContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.NeoMorphism;

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public int GetPreferredItemHeight() => 36;

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
            {
                using (var brush = new SolidBrush(metrics.BackgroundColor))
                {
                    g.FillPath(brush, path);
                }
            }

            if (metrics.ShowElevation)
            {
                DrawNeoMorphismShadows(g, bounds, metrics);
            }
        }

        public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (items == null || items.Count == 0) return;

            int y = metrics.Padding + 6;
            foreach (var item in items)
            {
                if (!item.IsVisible) continue;
                
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, metrics);
                    y += metrics.SeparatorHeight;
                    continue;
                }
                
                var itemRect = new Rectangle(metrics.Padding + 4, y, 
                    owner.Width - (metrics.Padding * 2) - 8, metrics.ItemHeight);
                
                DrawItem(g, owner, item, itemRect, 
                    item == selectedItem, item == hoveredItem, metrics, theme);
                
                y += metrics.ItemHeight + 4;
            }
        }

        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // NeoMorphism uses shadows instead of traditional borders
        }

        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
        {
            bool visuallySelected = isSelected || item.IsSelected;
            
            if (item.IsEnabled && (visuallySelected || isHovered))
            {
                using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                {
                    using (var brush = new SolidBrush(metrics.BackgroundColor))
                    {
                        g.FillPath(brush, path);
                    }

                    if (visuallySelected)
                    {
                        DrawNeoMorphismInset(g, itemRect, metrics);
                    }
                    else
                    {
                        DrawNeoMorphismRaised(g, itemRect, metrics);
                    }
                }
            }

            int x = itemRect.X + metrics.ItemPaddingLeft;

            if (owner.ShowCheckBox && item.IsCheckable)
            {
                var checkRect = new Rectangle(x, itemRect.Y + (itemRect.Height - metrics.CheckboxSize) / 2,
                    metrics.CheckboxSize, metrics.CheckboxSize);
                DrawNeoMorphismCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
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
                DrawSubmenuArrow(g, itemRect, !item.IsEnabled, metrics, textColor);
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

        private void DrawNeoMorphismCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, ContextMenuMetrics metrics)
        {
            using (var path = GetRoundedRect(rect, rect.Height / 2))
            {
                using (var brush = new SolidBrush(metrics.BackgroundColor))
                {
                    g.FillPath(brush, path);
                }

                if (isChecked)
                {
                    DrawNeoMorphismInset(g, rect, metrics);
                    var innerRect = new Rectangle(rect.X + 4, rect.Y + 4, rect.Width - 8, rect.Height - 8);
                    using (var innerPath = GetRoundedRect(innerRect, innerRect.Height / 2))
                    using (var innerBrush = new SolidBrush(metrics.AccentColor))
                    {
                        g.FillPath(innerBrush, innerPath);
                    }
                }
                else
                {
                    DrawNeoMorphismRaised(g, rect, metrics);
                }
            }
        }

        private void DrawNeoMorphismShadows(Graphics g, Rectangle bounds, ContextMenuMetrics metrics)
        {
            // Light shadow (top-left)
            using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
            using (var pen = new Pen(Color.FromArgb(40, Color.White), metrics.ShadowDepth))
            {
                var lightBounds = bounds;
                lightBounds.Offset(-2, -2);
                using (var lightPath = GetRoundedRect(lightBounds, metrics.BorderRadius))
                {
                    g.DrawPath(pen, lightPath);
                }
            }

            // Dark shadow (bottom-right)
            using (var pen = new Pen(Color.FromArgb(40, Color.Black), metrics.ShadowDepth))
            {
                var darkBounds = bounds;
                darkBounds.Offset(2, 2);
                using (var darkPath = GetRoundedRect(darkBounds, metrics.BorderRadius))
                {
                    g.DrawPath(pen, darkPath);
                }
            }
        }

        private void DrawNeoMorphismRaised(Graphics g, Rectangle rect, ContextMenuMetrics metrics)
        {
            using (var path = GetRoundedRect(rect, metrics.ItemBorderRadius))
            {
                using (var lightPen = new Pen(Color.FromArgb(60, Color.White), 2))
                {
                    g.DrawLine(lightPen, rect.X + 2, rect.Y + 2, rect.Right - 2, rect.Y + 2);
                    g.DrawLine(lightPen, rect.X + 2, rect.Y + 2, rect.X + 2, rect.Bottom - 2);
                }

                using (var darkPen = new Pen(Color.FromArgb(40, Color.Black), 2))
                {
                    g.DrawLine(darkPen, rect.Right - 2, rect.Y + 2, rect.Right - 2, rect.Bottom - 2);
                    g.DrawLine(darkPen, rect.X + 2, rect.Bottom - 2, rect.Right - 2, rect.Bottom - 2);
                }
            }
        }

        private void DrawNeoMorphismInset(Graphics g, Rectangle rect, ContextMenuMetrics metrics)
        {
            using (var path = GetRoundedRect(rect, metrics.ItemBorderRadius))
            {
                using (var darkPen = new Pen(Color.FromArgb(40, Color.Black), 2))
                {
                    g.DrawLine(darkPen, rect.X + 2, rect.Y + 2, rect.Right - 2, rect.Y + 2);
                    g.DrawLine(darkPen, rect.X + 2, rect.Y + 2, rect.X + 2, rect.Bottom - 2);
                }

                using (var lightPen = new Pen(Color.FromArgb(60, Color.White), 2))
                {
                    g.DrawLine(lightPen, rect.Right - 2, rect.Y + 2, rect.Right - 2, rect.Bottom - 2);
                    g.DrawLine(lightPen, rect.X + 2, rect.Bottom - 2, rect.Right - 2, rect.Bottom - 2);
                }
            }
        }

        private void DrawSubmenuArrow(Graphics g, Rectangle itemRect, bool isDisabled, 
            ContextMenuMetrics metrics, Color arrowColor)
        {
            var arrowRect = new Rectangle(itemRect.Right - metrics.ItemPaddingRight - metrics.SubmenuArrowSize,
                itemRect.Y + (itemRect.Height - metrics.SubmenuArrowSize) / 2,
                metrics.SubmenuArrowSize, metrics.SubmenuArrowSize);

            using (var pen = new Pen(arrowColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int midY = arrowRect.Y + arrowRect.Height / 2;
                g.DrawLine(pen, arrowRect.X + 4, midY - 4, arrowRect.Right - 4, midY);
                g.DrawLine(pen, arrowRect.Right - 4, midY, arrowRect.X + 4, midY + 4);
            }
        }

        private void DrawSeparator(Graphics g, BeepContextMenu owner, SimpleItem item, int y, ContextMenuMetrics metrics)
        {
            if (!owner.ShowSeparators) return;
            int lineY = y + metrics.SeparatorHeight / 2;
            int x = metrics.ItemPaddingLeft + 8;
            int width = owner.Width - x - metrics.ItemPaddingRight - 8;

            using (var darkPen = new Pen(Color.FromArgb(30, Color.Black), 1))
            using (var lightPen = new Pen(Color.FromArgb(30, Color.White), 1))
            {
                g.DrawLine(darkPen, x, lineY, x + width, lineY);
                g.DrawLine(lightPen, x, lineY + 1, x + width, lineY + 1);
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
    }
}
