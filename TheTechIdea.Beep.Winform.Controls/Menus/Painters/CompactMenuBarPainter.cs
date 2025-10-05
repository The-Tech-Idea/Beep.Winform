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
    /// Compact menu bar painter - minimalist design with reduced spacing and optional icon-only mode
    /// </summary>
    public sealed class CompactMenuBarPainter : MenuBarPainterBase
    {
        #region Fields
        private List<Rectangle> _itemRects = new List<Rectangle>();
        #endregion

        #region MenuBarPainterBase Implementation
        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (drawingRect.IsEmpty || ctx == null)
                return ctx ?? new MenuBarContext();

            UpdateContextColors(ctx);
            
            // Compact spacing
            ctx.DrawingRect = drawingRect;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -2, -2);
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.CornerRadius = 2; // Minimal rounding
            ctx.ItemSpacing = 2; // Minimal spacing
            ctx.ItemPadding = 6; // Reduced padding
            ctx.ItemHeight = Math.Min(ctx.ItemHeight, 24); // Compact height

            CalculateMenuItemRects(ctx);
            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx.DrawingRect.IsEmpty) return;

            using var bgBrush = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx?.MenuItems == null) return;

            for (int i = 0; i < ctx.MenuItems.Count && i < _itemRects.Count; i++)
            {
                DrawCompactMenuItem(g, _itemRects[i], ctx.MenuItems[i], ctx, i);
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            for (int i = 0; i < _itemRects.Count; i++)
            {
                if (IsAreaHovered($"MenuItem_{i}"))
                {
                    var hoverColor = GetHoverBackgroundColor();
                    using var brush = new SolidBrush(Color.FromArgb(40, hoverColor));
                    g.FillRectangle(brush, _itemRects[i]);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null || ctx == null) return;

            ClearOwnerHitAreas();
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                int itemIndex = i;
                AddHitAreaToOwner($"MenuItem_{i}", _itemRects[i], () =>
                {
                    if (ctx.IsItemEnabled(ctx.MenuItems[itemIndex]))
                    {
                        ctx.SelectedIndex = itemIndex;
                        notifyAreaHit?.Invoke($"MenuItem_{itemIndex}", _itemRects[itemIndex]);
                        SafeInvalidate();
                    }
                });
            }
        }
        #endregion

        #region Private Methods
        private int CalculateCompactItemWidth(SimpleItem item, Font font, Size iconSize, bool showIcons)
        {
            if (item == null || font == null) return 50;

            int width = 0;
            const int padding = 16; // Compact design padding
            const int iconSpacing = 6;

            // For compact mode, prefer icons over text
            if (showIcons && !string.IsNullOrEmpty(item.ImagePath))
            {
                width += iconSize.Width + iconSpacing;
            }
            else if (!string.IsNullOrEmpty(item.Text))
            {
                // Only show text if no icon available
                using var tempBitmap = new Bitmap(1, 1);
                using var tempGraphics = Graphics.FromImage(tempBitmap);
                var textSize = tempGraphics.MeasureString(item.Text, font);
                width += (int)Math.Ceiling(textSize.Width);
            }

            // Apply padding and ensure minimum width
            width += padding;
            return Math.Max(width, 40); // Compact minimum width
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
                int itemWidth = CalculateCompactItemWidth(item, ctx.TextFont, 
                    new Size(16, 16), ctx.ShowIcons); // Small icons for compact mode
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
                    itemWidths[i] = Math.Max(itemWidths[i] - reductionPerItem, 30);
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

            ctx.ItemRects.Clear();
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                ctx.ItemRects.Add((_itemRects[i], i, ctx.MenuItems[i]));
            }
        }

        private void DrawCompactMenuItem(Graphics g, Rectangle rect, SimpleItem item, MenuBarContext ctx, int index)
        {
            if (g == null || rect.IsEmpty || item == null) return;

            bool isSelected = ctx.SelectedIndex == index;
            Color foreColor = isSelected ? GetSelectedForegroundColor() : ctx.ItemForeColor;

            // Compact drawing - prefer icons over text in small spaces
            MenuBarRenderingHelpers.DrawMenuItem(g, rect, item, ctx.TextFont,
                foreColor, Color.Transparent, Color.Transparent,
                new Size(16, 16), true, false, 0); // Small icons, no dropdown indicators
        }
        #endregion
    }
}