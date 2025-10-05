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
    /// Fluent-style menu bar: pill selection, soft reveal hover, rounded corners.
    /// </summary>
    public sealed class FluentMenuBarPainter : MenuBarPainterBase
    {
        private List<Rectangle> _itemRects = new List<Rectangle>();

        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (ctx == null) ctx = new MenuBarContext();
            UpdateContextColors(ctx);

            ctx.CornerRadius = 10; // softer rounding
            ctx.DrawingRect = drawingRect;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -8, -6);
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.ItemHeight = Math.Max(34, ctx.ItemHeight);
            ctx.ItemSpacing = 6;
            ctx.ItemPadding = 14;

            CalculateItemRects(ctx);
            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null) return;
            using var bg = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bg, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null) return;
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var rect = _itemRects[i];
                var item = ctx.MenuItems[i];

                bool isSelected = ctx.SelectedIndex == i;
                bool isHovered = IsAreaHovered($"MenuItem_{i}");
                bool enabled = ctx.IsItemEnabled(item);

                // Selection pill
                if (isSelected)
                {
                    using var sel = new SolidBrush(Color.FromArgb(28, GetAccentColor()));
                    using var path = CreateRoundedPath(rect, ctx.CornerRadius);
                    g.FillPath(sel, path);
                }
                else if (isHovered)
                {
                    using var hov = new SolidBrush(Color.FromArgb(16, GetAccentColor()));
                    using var path = CreateRoundedPath(rect, ctx.CornerRadius);
                    g.FillPath(hov, path);
                }

                // Content
                var layout = MenuBarRenderingHelpers.CalculateMenuItemLayout(
                    rect, item, ctx.TextFont, ctx.IconSize, ctx.ShowIcons, ctx.ShowDropdownIndicators);

                var fore = enabled ? (isSelected ? GetAccentColor() : GetItemForegroundColor()) : ctx.ItemDisabledForeColor;

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
                        // Fallback to Fluent placeholder
                        using var b = new SolidBrush(Color.FromArgb(isSelected ? 220 : 150, fore));
                        using var p = new Pen(fore, 1.5f);
                        var inner = Rectangle.Inflate(layout.IconRect, -3, -3);
                        g.FillEllipse(b, inner);
                        g.DrawEllipse(p, inner);
                    }
                }
                if (!string.IsNullOrEmpty(item.Text) && !layout.TextRect.IsEmpty)
                {
                    using var tb = new SolidBrush(fore);
                    var sf = new StringFormat{ Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.DrawString(item.Text, ctx.TextFont, tb, layout.TextRect, sf);
                }
                if (ctx.ShowDropdownIndicators && item.Children?.Count > 0 && !layout.DropdownRect.IsEmpty)
                {
                    using var p = new Pen(fore, 1.5f){ StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round };
                    int cx = layout.DropdownRect.X + layout.DropdownRect.Width/2;
                    int cy = layout.DropdownRect.Y + layout.DropdownRect.Height/2;
                    g.DrawLine(p, cx-3, cy-1, cx, cy+2);
                    g.DrawLine(p, cx, cy+2, cx+3, cy-1);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            // No separate accents needed; selection/hover handled in content for Fluent
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

        public override Size CalculatePreferredSize(MenuBarContext ctx, Size proposedSize)
        {
            return base.CalculatePreferredSize(ctx, proposedSize);
        }

        private int CalculateFluentItemWidth(SimpleItem item, Font font, Size iconSize, bool showIcons, bool showDropdown)
        {
            if (item == null || font == null) return 100;

            int width = 0;
            const int padding = 28; // Fluent design padding
            const int iconSpacing = 8;
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
            return Math.Max(width, 90); // Fluent minimum width
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
                int itemWidth = CalculateFluentItemWidth(item, ctx.TextFont, ctx.IconSize, 
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
                    itemWidths[i] = Math.Max(itemWidths[i] - reductionPerItem, 70);
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
