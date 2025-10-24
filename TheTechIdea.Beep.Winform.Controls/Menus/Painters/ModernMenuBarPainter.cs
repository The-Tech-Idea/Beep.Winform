using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Painters
{
    /// <summary>
    /// Modern menu bar painter - flat design with subtle hover effects and clean typography
    /// </summary>
    public sealed class ModernMenuBarPainter : MenuBarPainterBase
    {
        #region Fields
        private List<Rectangle> _itemRects = new List<Rectangle>();
        #endregion

        #region MenuBarPainterBase Implementation
        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (drawingRect.IsEmpty || ctx == null)
                return ctx ?? new MenuBarContext();

            // Update context colors from theme
            UpdateContextColors(ctx);

            // Override some colors for modern Style
            ctx.ItemBackColor = Color.Transparent; // No background by default
            ctx.ItemBorderColor = Color.Transparent; // No borders
            ctx.CornerRadius = 6; // Slightly rounded corners

            // Main drawing rectangle
            ctx.DrawingRect = drawingRect;

            // Content rectangle with minimal padding
            int padding = 2;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -padding, -padding);

            // For modern Style, use full content area for menu items
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.TitleRect = Rectangle.Empty;
            ctx.ActionsRect = Rectangle.Empty;

            // Adjust spacing for modern look
            ctx.ItemSpacing = 10; // Slightly more spacing for air
            ctx.ItemPadding = 14; // Slightly more padding inside items

            // Calculate menu item rectangles
            CalculateMenuItemRects(ctx);

            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx.DrawingRect.IsEmpty) return;

            // Modern Style: clean flat background
            using var bgBrush = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // No borders for modern Style - clean and minimal
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null || ctx.MenuItems == null || ctx.MenuItems.Count == 0) 
                return;

            // Draw each menu item with modern styling
            for (int i = 0; i < ctx.MenuItems.Count && i < _itemRects.Count; i++)
            {
                var item = ctx.MenuItems[i];
                var itemRect = _itemRects[i];
                
                DrawModernMenuItem(g, itemRect, item, ctx, i);
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Draw modern hover effects - subtle and smooth
            for (int i = 0; i < _itemRects.Count; i++)
            {
                if (IsAreaHovered($"MenuItem_{i}"))
                {
                    var hoverColor = GetHoverBackgroundColor();
                    DrawModernHoverEffect(g, _itemRects[i], hoverColor, ctx.CornerRadius);
                }
            }

            // Draw modern selection: pill background + subtle accent line
            if (ctx.SelectedIndex >= 0 && ctx.SelectedIndex < _itemRects.Count)
            {
                var selectedRect = _itemRects[ctx.SelectedIndex];
                var selectionColor = GetAccentColor();
                using (var pill = new SolidBrush(Color.FromArgb(18, selectionColor)))
                using (var path = CreateRoundedPath(selectedRect, 8))
                {
                    g.FillPath(pill, path);
                }
                DrawModernSelectionIndicator(g, selectedRect, selectionColor);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null || ctx == null) return;

            ClearOwnerHitAreas();

            // Add hit areas for each menu item
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var itemRect = _itemRects[i];
                var item = ctx.MenuItems[i];
                int itemIndex = i; // Capture for closure

                AddHitAreaToOwner($"MenuItem_{i}", itemRect, () =>
                {
                    if (ctx.IsItemEnabled(item))
                    {
                        ctx.SelectedIndex = itemIndex;
                        notifyAreaHit?.Invoke($"MenuItem_{itemIndex}", itemRect);
                        SafeInvalidate();
                    }
                });
            }
        }
        #endregion

        #region Private Methods
        private void CalculateMenuItemRects(MenuBarContext ctx)
        {
            _itemRects.Clear();
            if (ctx?.MenuItems == null || ctx.MenuItems.Count == 0 || ctx.MenuItemsRect.IsEmpty)
                return;

            int currentX = ctx.MenuItemsRect.X;
            int availableWidth = ctx.MenuItemsRect.Width;
            int totalItemsWidth = 0;

            // First pass: calculate individual widths using font measurements
            var itemWidths = new List<int>();
            foreach (var item in ctx.MenuItems)
            {
                int itemWidth = CalculateModernItemWidth(item, ctx.TextFont, ctx.IconSize, 
                    ctx.ShowIcons, ctx.ShowDropdownIndicators);
                itemWidths.Add(itemWidth);
                totalItemsWidth += itemWidth;
            }

            // Add spacing between items
            int totalSpacing = (ctx.MenuItems.Count - 1) * ctx.ItemSpacing;
            int totalRequiredWidth = totalItemsWidth + totalSpacing;

            // Second pass: adjust for available width if needed
            if (totalRequiredWidth > availableWidth && ctx.MenuItems.Count > 0)
            {
                int excessWidth = totalRequiredWidth - availableWidth;
                int reductionPerItem = excessWidth / ctx.MenuItems.Count;
                
                for (int i = 0; i < itemWidths.Count; i++)
                {
                    itemWidths[i] = Math.Max(itemWidths[i] - reductionPerItem, 80); // Modern minimum
                }
            }

            // Third pass: create rectangles
            for (int i = 0; i < ctx.MenuItems.Count; i++)
            {
                var itemRect = new Rectangle(
                    currentX,
                    ctx.MenuItemsRect.Y,
                    itemWidths[i],
                    ctx.ItemHeight
                );
                
                _itemRects.Add(itemRect);
                currentX += itemWidths[i] + ctx.ItemSpacing;
            }

            // Update context with calculated rectangles
            ctx.ItemRects.Clear();
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                ctx.ItemRects.Add((_itemRects[i], i, ctx.MenuItems[i]));
            }
        }

        private void DrawModernMenuItem(Graphics g, Rectangle itemRect, SimpleItem item, MenuBarContext ctx, int index)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Determine item state
            bool isHovered = IsAreaHovered($"MenuItem_{index}");
            bool isSelected = ctx.SelectedIndex == index;
            bool isEnabled = ctx.IsItemEnabled(item);

            // Modern color scheme - more subtle
            Color backgroundColor = Color.Transparent; // Always transparent background
            Color foregroundColor = ctx.ItemForeColor;

            if (!isEnabled)
            {
                foregroundColor = ctx.ItemDisabledForeColor;
            }
            else if (isSelected)
            {
                foregroundColor = GetAccentColor(); // Use accent color for selected items
            }
            else if (isHovered)
            {
                foregroundColor = ctx.ItemHoverForeColor;
            }

            // Draw the menu item with modern styling
            DrawModernMenuItemContent(g, itemRect, item, ctx, foregroundColor, isEnabled);
        }

        private void DrawModernMenuItemContent(Graphics g, Rectangle itemRect, SimpleItem item, 
            MenuBarContext ctx, Color foregroundColor, bool isEnabled)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Calculate layout with modern spacing
            var layout = MenuBarRenderingHelpers.CalculateMenuItemLayout(
                itemRect, item, ctx.TextFont, ctx.IconSize, ctx.ShowIcons, ctx.ShowDropdownIndicators);

            // Modern icon rendering - subtle and clean
            if (ctx.ShowIcons && !string.IsNullOrEmpty(item.ImagePath) && !layout.IconRect.IsEmpty)
            {
                var iconColor = isEnabled ? foregroundColor : ctx.ItemDisabledForeColor;
                DrawModernIcon(g, layout.IconRect, item.ImagePath, iconColor);
            }

            // Modern text rendering - clean typography
            if (!string.IsNullOrEmpty(item.Text) && !layout.TextRect.IsEmpty)
            {
                DrawModernText(g, layout.TextRect, item.Text, ctx.TextFont, foregroundColor);
            }

            // Modern dropdown indicator - minimalist design
            if (ctx.ShowDropdownIndicators && item.Children?.Count > 0 && !layout.DropdownRect.IsEmpty)
            {
                DrawModernDropdownIndicator(g, layout.DropdownRect, foregroundColor);
            }
        }

        private void DrawModernIcon(Graphics g, Rectangle iconRect, string iconPath, Color color)
        {
            try
            {
                // Use ImagePainter for proper image rendering
                using var imagePainter = new TheTechIdea.Beep.Winform.Controls.BaseImage.ImagePainter(iconPath);
                imagePainter.ApplyThemeOnImage = false; // Don't apply theme color by default
                imagePainter.DrawImage(g, iconRect);
            }
            catch
            {
                // Fallback to modern placeholder if image loading fails
                using var brush = new SolidBrush(Color.FromArgb(120, color));
                using var pen = new Pen(color, 1.5f);
                
                var padding = 2;
                var innerRect = Rectangle.Inflate(iconRect, -padding, -padding);
                
                g.FillEllipse(brush, innerRect);
                g.DrawEllipse(pen, innerRect);
            }
        }

        private int CalculateModernItemWidth(SimpleItem item, Font font, Size iconSize, bool showIcons, bool showDropdown)
        {
            if (item == null || font == null) return 100;

            int width = 0;
            const int padding = 32; // Modern design padding (16px each side)
            const int iconSpacing = 8;
            const int dropdownSpacing = 6;
            const int dropdownWidth = 12;

            // Icon width
            if (showIcons && !string.IsNullOrEmpty(item.ImagePath))
            {
                width += iconSize.Width + iconSpacing;
            }

            // Text width - measure actual text with font
            // Use TextRenderer.MeasureText for safer measurement without Graphics context
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextRenderer.MeasureText(item.Text, font);
                width += textSize.Width;
            }

            // Dropdown indicator width
            if (showDropdown && item.Children?.Count > 0)
            {
                width += dropdownSpacing + dropdownWidth;
            }

            // Apply padding and ensure minimum width
            width += padding;
            return Math.Max(width, 100); // Modern minimum width
        }

        private void DrawModernText(Graphics g, Rectangle textRect, string text, Font font, Color color)
        {
            using var brush = new SolidBrush(color);
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            // Apply high-quality text rendering
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Use anti-aliasing for smoother text rendering
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.DrawString(text, font, brush, textRect, format);
        }

        private void DrawModernDropdownIndicator(Graphics g, Rectangle rect, Color color)
        {
            // Modern dropdown indicator - subtle chevron down
            using var pen = new Pen(color, 1.5f);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            var centerX = rect.X + rect.Width / 2;
            var centerY = rect.Y + rect.Height / 2;
            var size = 3;

            // Draw chevron down with rounded line caps
            g.DrawLine(pen, centerX - size, centerY - 1, centerX, centerY + 2);
            g.DrawLine(pen, centerX, centerY + 2, centerX + size, centerY - 1);
        }

        private void DrawModernHoverEffect(Graphics g, Rectangle rect, Color hoverColor, int cornerRadius)
        {
            // Modern hover effect - subtle background with rounded corners
            using var brush = new SolidBrush(Color.FromArgb(25, hoverColor));
            
            if (cornerRadius > 0)
            {
                using var path = CreateRoundedPath(rect, cornerRadius);
                g.FillPath(brush, path);
            }
            else
            {
                g.FillRectangle(brush, rect);
            }
        }

        private void DrawModernSelectionIndicator(Graphics g, Rectangle rect, Color selectionColor)
        {
            // Modern selection - subtle bottom accent line
            using var pen = new Pen(selectionColor, 2);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            var lineY = rect.Bottom - 2;
            var padding = 8;
            g.DrawLine(pen, rect.Left + padding, lineY, rect.Right - padding, lineY);
        }
        #endregion

        #region Protected Overrides
        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();
            SafeInvalidate();
        }
        #endregion
    }
}