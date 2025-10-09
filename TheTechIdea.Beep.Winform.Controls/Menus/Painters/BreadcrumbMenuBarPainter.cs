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
    /// Breadcrumb menu bar painter - displays navigation as a breadcrumb trail with chevron separators
    /// </summary>
    public sealed class BreadcrumbMenuBarPainter : MenuBarPainterBase
    {
        #region Fields
        private List<Rectangle> _itemRects = new List<Rectangle>();
        private Rectangle _homeRect;
        #endregion

        #region MenuBarPainterBase Implementation
        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (drawingRect.IsEmpty || ctx == null)
                return ctx ?? new MenuBarContext();

            UpdateContextColors(ctx);
            
            ctx.DrawingRect = drawingRect;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -ScaleValue(8), -ScaleValue(4));
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.CornerRadius = ScaleValue(4);
            ctx.ItemSpacing = 0; // No spacing for breadcrumbs
            ctx.ItemPadding = ScaleValue(12);

            CalculateBreadcrumbRects(ctx);
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

            // Draw home icon
            if (!_homeRect.IsEmpty)
            {
                DrawHomeIcon(g, _homeRect, ctx.ItemForeColor);
            }

            // Draw breadcrumb items
            for (int i = 0; i < ctx.MenuItems.Count && i < _itemRects.Count; i++)
            {
                DrawBreadcrumbItem(g, _itemRects[i], ctx.MenuItems[i], ctx, i);
                
                // Draw separator (except for last item)
                if (i < ctx.MenuItems.Count - 1)
                {
                    DrawBreadcrumbSeparator(g, _itemRects[i], ctx.ItemForeColor);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Hover effects for breadcrumb items
            if (IsAreaHovered("Home") && !_homeRect.IsEmpty)
            {
                DrawHoverEffect(g, _homeRect, GetHoverBackgroundColor(), 4);
            }

            for (int i = 0; i < _itemRects.Count; i++)
            {
                if (IsAreaHovered($"MenuItem_{i}"))
                {
                    DrawHoverEffect(g, _itemRects[i], GetHoverBackgroundColor(), 4);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null || ctx == null) return;

            ClearOwnerHitAreas();

            // Home button
            if (!_homeRect.IsEmpty)
            {
                AddHitAreaToOwner("Home", _homeRect, () =>
                {
                    ctx.SelectedIndex = -1; // Home selection
                    notifyAreaHit?.Invoke("Home", _homeRect);
                    SafeInvalidate();
                });
            }

            // Breadcrumb items
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
        private void CalculateBreadcrumbRects(MenuBarContext ctx)
        {
            _itemRects.Clear();
            _homeRect = Rectangle.Empty;

            if (ctx?.MenuItems == null || ctx.MenuItemsRect.IsEmpty) return;

            int currentX = ctx.MenuItemsRect.X;
            int itemY = ctx.MenuItemsRect.Y + (ctx.MenuItemsRect.Height - ctx.ItemHeight) / 2;

            // Home icon area
            _homeRect = new Rectangle(currentX, itemY, ScaleValue(24), ctx.ItemHeight);
            currentX += ScaleValue(32);

            // Calculate breadcrumb item rectangles
            foreach (var item in ctx.MenuItems)
            {
                int itemWidth = MenuBarRenderingHelpers.CalculateMenuItemWidth(item, ctx.ItemHeight, ctx.TextFont);
                itemWidth += ScaleValue(20); // Extra space for chevron

                var itemRect = new Rectangle(currentX, itemY, itemWidth, ctx.ItemHeight);
                _itemRects.Add(itemRect);
                currentX += itemWidth + ScaleValue(4);

                if (currentX >= ctx.MenuItemsRect.Right) break;
            }

            // Update context
            ctx.ItemRects.Clear();
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                ctx.ItemRects.Add((_itemRects[i], i, ctx.MenuItems[i]));
            }
        }

        private void DrawHomeIcon(Graphics g, Rectangle rect, Color color)
        {
            // Simple home icon
            using var brush = new SolidBrush(color);
            using var pen = new Pen(color, 1.5f);

            var iconRect = new Rectangle(rect.X + ScaleValue(4), rect.Y + rect.Height / 4, ScaleValue(16), ScaleValue(16));
            
            // Draw house shape
            var points = new Point[]
            {
                new Point(iconRect.X + ScaleValue(8), iconRect.Y + ScaleValue(2)),
                new Point(iconRect.X + ScaleValue(2), iconRect.Y + ScaleValue(8)),
                new Point(iconRect.X + ScaleValue(14), iconRect.Y + ScaleValue(8)),
                new Point(iconRect.X + ScaleValue(14), iconRect.Bottom - ScaleValue(2)),
                new Point(iconRect.X + ScaleValue(2), iconRect.Bottom - ScaleValue(2)),
                new Point(iconRect.X + ScaleValue(2), iconRect.Y + ScaleValue(8))
            };
            g.DrawLines(pen, points);
        }

        private void DrawBreadcrumbItem(Graphics g, Rectangle rect, SimpleItem item, MenuBarContext ctx, int index)
        {
            if (g == null || rect.IsEmpty || item == null) return;

            bool isLast = index == ctx.MenuItems.Count - 1;
            bool isSelected = ctx.SelectedIndex == index;

            // Highlight current/last item
            Color textColor = isLast || isSelected ? GetAccentColor() : ctx.ItemForeColor;

            // Draw text only for breadcrumbs
            using var brush = new SolidBrush(textColor);
            var format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };

            var textRect = new Rectangle(rect.X + ScaleValue(8), rect.Y, rect.Width - ScaleValue(28), rect.Height);
            g.DrawString(item.Text, ctx.TextFont, brush, textRect, format);
        }

        private void DrawBreadcrumbSeparator(Graphics g, Rectangle itemRect, Color color)
        {
            // Draw chevron separator
            MenuBarRenderingHelpers.DrawBreadcrumbSeparator(g, 
                new Rectangle(itemRect.Right - ScaleValue(16), itemRect.Y + itemRect.Height / 2 - ScaleValue(6), ScaleValue(12), ScaleValue(12)), 
                Color.FromArgb(150, color));
        }
        #endregion
    }
}