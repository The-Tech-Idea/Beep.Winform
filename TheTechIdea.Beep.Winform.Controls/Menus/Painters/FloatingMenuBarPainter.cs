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
    /// Floating bar: rounded container with subtle shadow; items are simple with underline selection.
    /// </summary>
    public sealed class FloatingMenuBarPainter : MenuBarPainterBase
    {
        private List<Rectangle> _itemRects = new List<Rectangle>();
        private Rectangle _containerRect;

        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (ctx == null) ctx = new MenuBarContext();
            UpdateContextColors(ctx);

            // Floating container inside drawing rect
            _containerRect = Rectangle.Inflate(drawingRect, -ScaleValue(8), -ScaleValue(6));
            ctx.DrawingRect = drawingRect;
            ctx.ContentRect = Rectangle.Inflate(_containerRect, -ScaleValue(12), -ScaleValue(8));
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.CornerRadius = ScaleValue(14);
            ctx.ItemHeight = Math.Max(ScaleValue(34), ctx.ItemHeight);
            ctx.ItemSpacing = ScaleValue(10);
            ctx.ItemPadding = ScaleValue(12);

            CalculateItemRects(ctx);
            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            // Shadow
            using (var shadow = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                var sRect = _containerRect; sRect.Offset(0, ScaleValue(3));
                using var sPath = CreateRoundedPath(sRect, ctx.CornerRadius + ScaleValue(2));
                g.FillPath(shadow, sPath);
            }
            // Container
            using var bg = new SolidBrush(GetBackgroundColor());
            using var path = CreateRoundedPath(_containerRect, ctx.CornerRadius);
            g.FillPath(bg, path);
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var rect = _itemRects[i];
                var item = ctx.MenuItems[i];
                bool isSelected = ctx.SelectedIndex == i;
                bool isHovered = IsAreaHovered($"MenuItem_{i}");

                var fore = isSelected ? GetAccentColor() : (isHovered ? GetHoverForegroundColor() : GetItemForegroundColor());

                var layout = MenuBarRenderingHelpers.CalculateMenuItemLayout(rect, item, ctx.TextFont, ctx.IconSize, ctx.ShowIcons, ctx.ShowDropdownIndicators);
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
                        // Fallback to Floating placeholder
                        using var p = new Pen(fore, (float)ScaleValue(2) * 0.8f);
                        var inner = Rectangle.Inflate(layout.IconRect, -ScaleValue(3), -ScaleValue(3));
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
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (ctx.SelectedIndex >= 0 && ctx.SelectedIndex < _itemRects.Count)
            {
                var r = _itemRects[ctx.SelectedIndex];
                using var pen = new Pen(GetAccentColor(), (float)ScaleValue(2)){ StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round };
                g.DrawLine(pen, r.Left + ScaleValue(8), r.Bottom - ScaleValue(3), r.Right - ScaleValue(8), r.Bottom - ScaleValue(3));
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

        private int CalculateFloatingItemWidth(SimpleItem item, Font font, Size iconSize, bool showIcons, bool showDropdown)
        {
            if (item == null || font == null) return ScaleValue(100);

            int width = 0;
            int padding = ScaleValue(32); // Floating design padding
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
            return Math.Max(width, ScaleValue(95)); // Floating minimum width
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
                int itemWidth = CalculateFloatingItemWidth(item, ctx.TextFont, ctx.IconSize, 
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
                    itemWidths[i] = Math.Max(itemWidths[i] - reductionPerItem, ScaleValue(75));
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
