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
    /// Bubble-style menu bar: rounded item backgrounds with active badge-like accent.
    /// </summary>
    public sealed class BubbleMenuBarPainter : MenuBarPainterBase
    {
        private List<Rectangle> _itemRects = new List<Rectangle>();

        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (ctx == null) ctx = new MenuBarContext();
            UpdateContextColors(ctx);
            ctx.CornerRadius = ScaleValue(14);
            ctx.DrawingRect = drawingRect;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -ScaleValue(6), -ScaleValue(4));
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.ItemHeight = Math.Max(ScaleValue(38), ctx.ItemHeight);
            ctx.ItemSpacing = ScaleValue(8);
            ctx.ItemPadding = ScaleValue(16);
            CalculateItemRects(ctx);
            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            using var bg = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bg, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var rect = _itemRects[i];
                var item = ctx.MenuItems[i];
                bool isSelected = ctx.SelectedIndex == i;
                bool isHovered = IsAreaHovered($"MenuItem_{i}");
                bool enabled = ctx.IsItemEnabled(item);

                var baseFore = enabled ? GetItemForegroundColor() : ctx.ItemDisabledForeColor;
                var accent = GetAccentColor();

                // Bubble background
                if (isSelected || isHovered)
                {
                    int alpha = isSelected ? 220 : 40;
                    using var sel = new SolidBrush(Color.FromArgb(alpha, accent));
                    using var path = CreateRoundedPath(rect, ctx.CornerRadius);
                    g.FillPath(sel, path);
                }

                // Icon + Text
                var layout = MenuBarRenderingHelpers.CalculateMenuItemLayout(
                    rect, item, ctx.TextFont, ctx.IconSize, ctx.ShowIcons, ctx.ShowDropdownIndicators);

                var fore = isSelected ? Color.White : baseFore;

                if (ctx.ShowIcons && !string.IsNullOrEmpty(item.ImagePath) && !layout.IconRect.IsEmpty)
                {
                    try
                    {
                        // Use ImagePainter for proper image rendering
                        using var imagePainter = new TheTechIdea.Beep.Winform.Controls.BaseImage.ImagePainter(item.ImagePath);
                        imagePainter.ApplyThemeOnImage = false; // Don't apply theme color by default
                        imagePainter.DrawImage(g, layout.IconRect);
                    }
                    catch
                    {
                        // Fallback to Bubble placeholder
                        using var b = new SolidBrush(Color.FromArgb(isSelected ? 255 : 160, fore));
                        var inner = Rectangle.Inflate(layout.IconRect, -2, -2);
                        g.FillEllipse(b, inner);
                    }
                }
                if (!string.IsNullOrEmpty(item.Text) && !layout.TextRect.IsEmpty)
                {
                    using var tb = new SolidBrush(fore);
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.DrawString(item.Text, ctx.TextFont, tb, layout.TextRect, sf);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            // Optional: small bottom pill when selected
            if (ctx.SelectedIndex >= 0 && ctx.SelectedIndex < _itemRects.Count)
            {
                var r = _itemRects[ctx.SelectedIndex];
                var pill = new Rectangle(r.X + r.Width/4, r.Bottom - ScaleValue(6), r.Width/2, ScaleValue(4));
                using var br = new SolidBrush(Color.FromArgb(200, GetAccentColor()));
                using var path = CreateRoundedPath(pill, ScaleValue(2));
                g.FillPath(br, path);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            ClearOwnerHitAreas();
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var iLocal = i; var rect = _itemRects[i];
                AddHitAreaToOwner($"MenuItem_{i}", rect, () => { ctx.SelectedIndex = iLocal; notifyAreaHit?.Invoke($"MenuItem_{iLocal}", rect); SafeInvalidate(); });
            }
        }

        public override Size CalculatePreferredSize(MenuBarContext ctx, Size proposedSize) => base.CalculatePreferredSize(ctx, proposedSize);

        private int CalculateBubbleItemWidth(SimpleItem item, Font font, Size iconSize, bool showIcons, bool showDropdown)
        {
            if (item == null || font == null) return ScaleValue(100);

            int width = 0;
            int padding = ScaleValue(24); // Bubble design padding
            int iconSpacing = ScaleValue(8);
            int dropdownSpacing = ScaleValue(6);
            int dropdownWidth = ScaleValue(12);

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
            return Math.Max(width, ScaleValue(85)); // Bubble minimum width
        }

        private void CalculateItemRects(MenuBarContext ctx)
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
                int itemWidth = CalculateBubbleItemWidth(item, ctx.TextFont, ctx.IconSize, 
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
                    itemWidths[i] = Math.Max(itemWidths[i] - reductionPerItem, ScaleValue(70));
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
                ctx.ItemRects.Add((_itemRects[i], i, ctx.MenuItems[i]));
        }
    }
}
