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
    public class GlassContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Glass;

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public int GetPreferredItemHeight() => 32;

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
            {
                using (var brush = new SolidBrush(Color.FromArgb(245, metrics.BackgroundColor)))
                {
                    g.FillPath(brush, path);
                }

                var topRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 2);
                using (var glossBrush = new LinearGradientBrush(topRect,
                    Color.FromArgb(60, Color.White),
                    Color.FromArgb(10, Color.White), 90f))
                {
                    using (var glossPath = GetRoundedRect(topRect, metrics.BorderRadius))
                    {
                        g.FillPath(glossBrush, glossPath);
                    }
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
                if (!item.IsVisible) continue;
                
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, metrics);
                    y += metrics.SeparatorHeight;
                    continue;
                }
                
                var itemRect = new Rectangle(metrics.Padding, y, 
                    owner.Width - (metrics.Padding * 2), metrics.ItemHeight);
                
                DrawItem(g, owner, item, itemRect, 
                    item == selectedItem, item == hoveredItem, metrics, theme);
                
                y += metrics.ItemHeight;
            }
        }

        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
            {
                using (var pen = new Pen(Color.FromArgb(150, metrics.BorderColor), metrics.BorderWidth))
                {
                    g.DrawPath(pen, path);
                }

                var innerBounds = bounds;
                innerBounds.Inflate(-1, -1);
                using (var innerPath = GetRoundedRect(innerBounds, metrics.BorderRadius - 1))
                using (var innerPen = new Pen(Color.FromArgb(80, Color.White), 1))
                {
                    g.DrawPath(innerPen, innerPath);
                }
            }

            if (metrics.ShowElevation)
            {
                DrawGlassShadow(g, bounds, metrics);
            }
        }

        private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect, 
            bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
        {
            bool visuallySelected = isSelected || item.IsSelected;
            
            if (item.IsEnabled)
            {
                if (visuallySelected || isHovered)
                {
                    var backColor = visuallySelected ? metrics.ItemSelectedBackColor : metrics.ItemHoverBackColor;
                    using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(180, backColor)))
                        {
                            g.FillPath(brush, path);
                        }

                        var glossRect = new Rectangle(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 2);
                        using (var glossBrush = new LinearGradientBrush(glossRect,
                            Color.FromArgb(40, Color.White),
                            Color.FromArgb(5, Color.White), 90f))
                        {
                            g.FillRectangle(glossBrush, glossRect);
                        }
                    }
                }
            }

            int x = itemRect.X + metrics.ItemPaddingLeft;

            if (owner.ShowCheckBox && item.IsCheckable)
            {
                var checkRect = new Rectangle(x, itemRect.Y + (itemRect.Height - metrics.CheckboxSize) / 2,
                    metrics.CheckboxSize, metrics.CheckboxSize);
                DrawGlassCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
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

        private void DrawGlassCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, ContextMenuMetrics metrics)
        {
            var checkColor = isDisabled ? metrics.ItemDisabledForeColor : metrics.ItemForeColor;
            using (var path = GetRoundedRect(rect, 3))
            {
                using (var brush = new SolidBrush(Color.FromArgb(100, Color.White)))
                {
                    g.FillPath(brush, path);
                }
                
                using (var pen = new Pen(checkColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            if (isChecked)
            {
                using (var pen = new Pen(checkColor, 2))
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
            using (var pen = new Pen(Color.FromArgb(100, metrics.SeparatorColor), 1))
            {
                g.DrawLine(pen, metrics.ItemPaddingLeft, lineY, 
                    owner.Width - metrics.ItemPaddingRight, lineY);
            }
        }

        private void DrawGlassShadow(Graphics g, Rectangle bounds, ContextMenuMetrics metrics)
        {
            for (int i = 0; i < metrics.ShadowDepth; i++)
            {
                int alpha = metrics.ShadowAlpha - (i * metrics.ShadowAlpha / metrics.ShadowDepth);
                using (var pen = new Pen(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    var shadowBounds = bounds;
                    shadowBounds.Inflate(i, i);
                    using (var path = GetRoundedRect(shadowBounds, metrics.BorderRadius + i))
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
    }
}
