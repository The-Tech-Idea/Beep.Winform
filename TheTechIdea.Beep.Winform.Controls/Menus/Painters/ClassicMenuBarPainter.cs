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
    /// Classic menu bar painter - replicates the traditional BeepMenuBar appearance and behavior
    /// </summary>
    public sealed class ClassicMenuBarPainter : MenuBarPainterBase
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

            // Main drawing rectangle
            ctx.DrawingRect = drawingRect;

            // Content rectangle with padding
            int padding = 4;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -padding, -padding);

            // For classic style, we don't need separate title/actions areas
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.TitleRect = Rectangle.Empty;
            ctx.ActionsRect = Rectangle.Empty;

            // Calculate menu item rectangles
            CalculateMenuItemRects(ctx);

            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx.DrawingRect.IsEmpty) return;

            // Draw main background
            using var bgBrush = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // Draw subtle border if needed
            var borderColor = GetBorderColor();
            if (borderColor != Color.Transparent)
            {
                using var borderPen = new Pen(borderColor);
                g.DrawRectangle(borderPen, 
                    ctx.DrawingRect.X, ctx.DrawingRect.Y, 
                    ctx.DrawingRect.Width - 1, ctx.DrawingRect.Height - 1);
            }
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null || ctx.MenuItems == null || ctx.MenuItems.Count == 0) 
                return;

            // Draw each menu item
            for (int i = 0; i < ctx.MenuItems.Count && i < _itemRects.Count; i++)
            {
                var item = ctx.MenuItems[i];
                var itemRect = _itemRects[i];
                
                DrawMenuItem(g, itemRect, item, ctx, i);
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Draw hover effects
            for (int i = 0; i < _itemRects.Count; i++)
            {
                if (IsAreaHovered($"MenuItem_{i}"))
                {
                    var hoverColor = GetHoverBackgroundColor();
                    DrawHoverEffect(g, _itemRects[i], hoverColor, ctx.CornerRadius);
                }
            }

            // Draw selection indicator for selected item
            if (ctx.SelectedIndex >= 0 && ctx.SelectedIndex < _itemRects.Count)
            {
                var selectionColor = GetSelectedBackgroundColor();
                DrawSelectionIndicator(g, _itemRects[ctx.SelectedIndex], selectionColor, 
                    SelectionIndicatorStyle.BottomLine);
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
        private int CalculateClassicItemWidth(SimpleItem item, Font font, Size iconSize, bool showIcons, bool showDropdown)
        {
            if (item == null || font == null) return 80;

            int width = 0;
            const int padding = 24; // Classic design padding
            const int iconSpacing = 6;
            const int dropdownSpacing = 6;
            const int dropdownWidth = 12;

            // Icon width
            if (showIcons && !string.IsNullOrEmpty(item.ImagePath))
            {
                width += iconSize.Width + iconSpacing;
            }

            // Text width - measure actual text with font
            if (!string.IsNullOrEmpty(item.Text))
            {
                using var tempBitmap = new Bitmap(1, 1);
                using var tempGraphics = Graphics.FromImage(tempBitmap);
                var textSize = tempGraphics.MeasureString(item.Text, font);
                width += (int)Math.Ceiling(textSize.Width);
            }

            // Dropdown indicator width
            if (showDropdown && item.Children?.Count > 0)
            {
                width += dropdownSpacing + dropdownWidth;
            }

            // Apply padding and ensure minimum width
            width += padding;
            return Math.Max(width, 80); // Classic minimum width
        }

        private void CalculateMenuItemRects(MenuBarContext ctx)
        {
            _itemRects.Clear();

            if (ctx?.MenuItems == null || ctx.MenuItems.Count == 0 || ctx.MenuItemsRect.IsEmpty)
                return;

            int currentX = ctx.MenuItemsRect.X;
            int availableWidth = ctx.MenuItemsRect.Width;
            int totalItemsWidth = 0;

            // Calculate individual widths using font measurements
            var itemWidths = new List<int>();
            foreach (var item in ctx.MenuItems)
            {
                int itemWidth = CalculateClassicItemWidth(item, ctx.TextFont, ctx.IconSize, 
                    ctx.ShowIcons, ctx.ShowDropdownIndicators);
                itemWidths.Add(itemWidth);
                totalItemsWidth += itemWidth;
            }

            // Add spacing and adjust for available width
            int totalSpacing = (ctx.MenuItems.Count - 1) * ctx.ItemSpacing;
            int totalRequiredWidth = totalItemsWidth + totalSpacing;

            if (totalRequiredWidth > availableWidth && ctx.MenuItems.Count > 0)
            {
                int excessWidth = totalRequiredWidth - availableWidth;
                int reductionPerItem = excessWidth / ctx.MenuItems.Count;
                
                for (int i = 0; i < itemWidths.Count; i++)
                {
                    itemWidths[i] = Math.Max(itemWidths[i] - reductionPerItem, 60);
                }
            }

            // Create rectangles
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

        private void DrawMenuItem(Graphics g, Rectangle itemRect, SimpleItem item, MenuBarContext ctx, int index)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Determine item state
            bool isHovered = IsAreaHovered($"MenuItem_{index}");
            bool isSelected = ctx.SelectedIndex == index;
            bool isEnabled = ctx.IsItemEnabled(item);

            // Determine colors based on state
            Color backgroundColor = Color.Transparent;
            Color foregroundColor = ctx.ItemForeColor;
            Color borderColor = Color.Transparent;

            if (!isEnabled)
            {
                backgroundColor = ctx.ItemDisabledBackColor;
                foregroundColor = ctx.ItemDisabledForeColor;
            }
            else if (isSelected)
            {
                backgroundColor = ctx.ItemSelectedBackColor;
                foregroundColor = ctx.ItemSelectedForeColor;
                borderColor = ctx.ItemSelectedBackColor;
            }
            else if (isHovered)
            {
                backgroundColor = ctx.ItemHoverBackColor;
                foregroundColor = ctx.ItemHoverForeColor;
                borderColor = ctx.ItemBorderColor;
            }
            else
            {
                backgroundColor = ctx.ItemBackColor;
                foregroundColor = ctx.ItemForeColor;
            }

            // Draw the menu item
            MenuBarRenderingHelpers.DrawMenuItem(
                g, itemRect, item, ctx.TextFont,
                foregroundColor, backgroundColor, borderColor,
                ctx.IconSize, ctx.ShowIcons, ctx.ShowDropdownIndicators,
                ctx.CornerRadius
            );
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