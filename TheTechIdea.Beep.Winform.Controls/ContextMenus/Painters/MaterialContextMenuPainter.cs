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
    /// Material Design style context menu painter
    /// </summary>
    public class MaterialContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.Material;

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public int GetPreferredItemHeight() => 36;

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            using (var brush = new LinearGradientBrush(bounds, metrics.BackgroundColor, 
                ControlPaint.Light(metrics.BackgroundColor, 0.05f), 90f))
            {
                g.FillRectangle(brush, bounds);
            }

            if (metrics.AccentBarWidth > 0)
            {
                var accentRect = new Rectangle(bounds.X, bounds.Y, metrics.AccentBarWidth, bounds.Height);
                using (var brush = new SolidBrush(metrics.AccentColor))
                {
                    g.FillRectangle(brush, accentRect);
                }
            }
        }
        
        public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (items == null || items.Count == 0) return;
            
            int y = metrics.Padding + 4;
            
            foreach (var item in items)
            {
                if (!item.IsVisible) continue;
                
                if (IsSeparator(item))
                {
                    DrawSeparator(g, owner, item, y, metrics);
                    y += metrics.SeparatorHeight;
                    continue;
                }
                
                var itemRect = new Rectangle(metrics.Padding + metrics.AccentBarWidth, y, 
                    owner.Width - metrics.Padding - metrics.AccentBarWidth - metrics.Padding, 
                    metrics.ItemHeight);
                
                DrawItem(g, owner, item, itemRect, item == selectedItem, item == hoveredItem, metrics, theme);
                
                y += metrics.ItemHeight;
            }
        }
        
        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            if (metrics.ShowElevation)
            {
                DrawElevationShadow(g, bounds, metrics.ShadowDepth);
            }
            
            using (var pen = new Pen(metrics.BorderColor, metrics.BorderWidth))
            {
                g.DrawRectangle(pen, bounds);
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
                    using (var brush = new SolidBrush(backColor))
                    using (var path = GetRoundedRect(itemRect, metrics.ItemBorderRadius))
                    {
                        g.FillPath(brush, path);
                    }
                    
                    if (metrics.ShowRippleEffect && isHovered && !visuallySelected)
                    {
                        using (var rippleBrush = new SolidBrush(Color.FromArgb(20, metrics.AccentColor)))
                        {
                            g.FillEllipse(rippleBrush, itemRect);
                        }
                    }
                }
            }
            
            int x = itemRect.X + metrics.ItemPaddingLeft;
            
            if (owner.ShowCheckBox && item.IsCheckable)
            {
                var checkRect = new Rectangle(x, itemRect.Y + (itemRect.Height - metrics.CheckboxSize) / 2,
                    metrics.CheckboxSize, metrics.CheckboxSize);
                DrawMaterialCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
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
                DrawMaterialArrow(g, itemRect, !item.IsEnabled, metrics, textColor);
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
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(
                        g, rect, imagePath, BeepControlStyle.Material3);
                }
            }
            catch { }
        }
        
        private void DrawMaterialCheckbox(Graphics g, Rectangle rect, bool isChecked, bool isDisabled, ContextMenuMetrics metrics)
        {
            var checkColor = isDisabled ? metrics.ItemDisabledForeColor : metrics.AccentColor;
            using (var path = GetRoundedRect(rect, 2))
            {
                if (isChecked)
                {
                    using (var brush = new SolidBrush(checkColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
                
                using (var pen = new Pen(checkColor, 2))
                {
                    g.DrawPath(pen, path);
                }
            }

            if (isChecked)
            {
                using (var pen = new Pen(Color.White, 2))
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

        private void DrawMaterialArrow(Graphics g, Rectangle itemRect, bool isDisabled, 
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
            int x = metrics.ItemPaddingLeft + metrics.AccentBarWidth;
            int width = owner.Width - metrics.ItemPaddingLeft - metrics.ItemPaddingRight - metrics.AccentBarWidth;
            
            using (var pen = new Pen(metrics.SeparatorColor, 1))
            {
                g.DrawLine(pen, x, lineY, x + width, lineY);
            }
        }
        
        private void DrawElevationShadow(Graphics g, Rectangle bounds, int elevation)
        {
            int shadowSize = elevation * 2;
            int shadowAlpha = 40;
            
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
    }
}
