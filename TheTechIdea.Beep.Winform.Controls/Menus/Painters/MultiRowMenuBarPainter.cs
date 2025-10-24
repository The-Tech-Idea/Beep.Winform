using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Painters
{
    /// <summary>
    /// Multi-Row menu bar painter - organizes menu items across multiple rows with different visual styles
    /// </summary>
    public sealed class MultiRowMenuBarPainter : MenuBarPainterBase
    {
        #region Fields
        private List<List<Rectangle>> _rowItemRects = new List<List<Rectangle>>();
        private List<SimpleItem> _flatItems = new List<SimpleItem>();
        private const int ROW_HEIGHT = 36;
        private const int ROW_SPACING = 4;
        private const int ITEMS_PER_ROW = 6;
        private const int MIN_ROW_HEIGHT = 36; // Minimum row height for modern design
        #endregion

        #region MenuBarPainterBase Implementation
        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (drawingRect.IsEmpty || ctx == null)
                return ctx ?? new MenuBarContext();

            UpdateContextColors(ctx);
            ctx.DrawingRect = drawingRect;

            int padding = 8;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -padding, -padding);
            ctx.MenuItemsRect = ctx.ContentRect;

            // Ensure item height is calculated dynamically based on font size and padding
            ctx.ItemHeight = Math.Max(ctx.TextFont.Height + 12, MIN_ROW_HEIGHT);

            // Calculate multi-row layout
            CalculateMultiRowLayout(ctx);

            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx.DrawingRect.IsEmpty) return;

            using var bgBrush = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // Draw row separators
            DrawRowSeparators(g, ctx);
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null || ctx.MenuItems == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int itemIndex = 0;

            // Draw items in each row
            for (int rowIndex = 0; rowIndex < _rowItemRects.Count; rowIndex++)
            {
                var rowRects = _rowItemRects[rowIndex];

                for (int i = 0; i < rowRects.Count && itemIndex < _flatItems.Count; i++)
                {
                    var item = _flatItems[itemIndex];
                    var rect = rowRects[i];

                    DrawMultiRowItem(g, rect, item, ctx, itemIndex, rowIndex);

                    itemIndex++;
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            int itemIndex = 0;

            // Draw hover and selection effects
            for (int rowIndex = 0; rowIndex < _rowItemRects.Count; rowIndex++)
            {
                var rowRects = _rowItemRects[rowIndex];

                for (int i = 0; i < rowRects.Count && itemIndex < _flatItems.Count; i++)
                {
                    var rect = rowRects[i];

                    // Hover effect
                    if (IsAreaHovered($"Item_{itemIndex}"))
                    {
                        DrawItemHoverEffect(g, rect, rowIndex);
                    }

                    // Selection effect
                    if (ctx.SelectedIndex == itemIndex)
                    {
                        DrawItemSelectionEffect(g, rect, ctx, rowIndex);
                    }

                    itemIndex++;
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null || ctx == null) return;

            ClearOwnerHitAreas();

            int itemIndex = 0;

            // Add hit areas for all items
            for (int rowIndex = 0; rowIndex < _rowItemRects.Count; rowIndex++)
            {
                var rowRects = _rowItemRects[rowIndex];

                for (int i = 0; i < rowRects.Count && itemIndex < _flatItems.Count; i++)
                {
                    var item = _flatItems[itemIndex];
                    var rect = rowRects[i];
                    int currentIndex = itemIndex;

                    AddHitAreaToOwner($"Item_{itemIndex}", rect, () =>
                    {
                        if (item.IsEnabled)
                        {
                            ctx.SelectedIndex = currentIndex;
                            notifyAreaHit?.Invoke($"Item_{currentIndex}", rect);
                            SafeInvalidate();
                        }
                    });

                    itemIndex++;
                }
            }
        }
        #endregion

        #region Private Methods
        private void CalculateMultiRowLayout(MenuBarContext ctx)
        {
            _rowItemRects.Clear();
            _flatItems.Clear();

            if (ctx.MenuItems == null || ctx.ContentRect.IsEmpty) return;

            _flatItems = ctx.MenuItems.ToList();

            int availableWidth = ctx.ContentRect.Width;
            int itemWidth = (availableWidth - (ITEMS_PER_ROW - 1) * 4) / ITEMS_PER_ROW;

            int y = ctx.ContentRect.Y;
            int itemIndex = 0;

            while (itemIndex < _flatItems.Count)
            {
                var rowRects = new List<Rectangle>();
                int x = ctx.ContentRect.X;

                for (int i = 0; i < ITEMS_PER_ROW && itemIndex < _flatItems.Count; i++)
                {
                    var rect = new Rectangle(x, y, itemWidth, ctx.ItemHeight);
                    rowRects.Add(rect);
                    x += itemWidth + 4;
                    itemIndex++;
                }

                _rowItemRects.Add(rowRects);
                y += ctx.ItemHeight + ROW_SPACING;
            }
        }

        private void DrawRowSeparators(Graphics g, MenuBarContext ctx)
        {
            if (_rowItemRects.Count <= 1) return;

            using var pen = new Pen(GetBorderColor());
            int y = ctx.ContentRect.Y;

            for (int i = 0; i < _rowItemRects.Count - 1; i++)
            {
                y += ROW_HEIGHT + ROW_SPACING / 2;
                g.DrawLine(pen, ctx.ContentRect.X, y, ctx.ContentRect.Right, y);
                y += ROW_SPACING / 2;
            }
        }

        private void DrawMultiRowItem(Graphics g, Rectangle rect, SimpleItem item, MenuBarContext ctx, int itemIndex, int rowIndex)
        {
            if (g == null || item == null) return;

            // Alternate row styling
            var bgColor = GetItemBackgroundColor();
            if (itemIndex == ctx.SelectedIndex)
            {
                bgColor = GetSelectedBackgroundColor();
            }
            else if (rowIndex % 2 == 1)
            {
                bgColor = Color.FromArgb(248, 248, 248);
            }

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Draw icon
            int contentX = rect.X + 8;
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int iconSize = 20;
                var iconRect = new Rectangle(contentX, rect.Y + (rect.Height - iconSize) / 2, iconSize, iconSize);
                if (item.IsEnabled)
                {
                    StyledImagePainter.Paint(g, iconRect, item.ImagePath);

                }
                else
                {
                    StyledImagePainter.PaintDisabled(g, iconRect, item.ImagePath, GetDisabledForegroundColor());
                }

                contentX += iconSize + 8;
            }

            // Draw text
            var textColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
            using (var brush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(contentX, rect.Y, rect.Right - contentX - 4, rect.Height);
                var sf = new StringFormat 
                { 
                    Alignment = StringAlignment.Near, 
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(item.Text ?? "", ctx.TextFont, brush, textRect, sf);
            }

            // Draw subtle border
            using (var pen = new Pen(GetBorderColor(), 1))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawItemHoverEffect(Graphics g, Rectangle rect, int rowIndex)
        {
            // Different hover styles per row
            var hoverColor = GetHoverBackgroundColor();

            using (var brush = new SolidBrush(Color.FromArgb(50, hoverColor))) // Slightly more visible hover effect
            {
                g.FillRectangle(brush, rect);
            }
        }

        private void DrawItemSelectionEffect(Graphics g, Rectangle rect, MenuBarContext ctx, int rowIndex)
        {
            using (var pen = new Pen(GetAccentColor(), 3)) // Slightly thicker border for better visibility
            {
                g.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
            }
        }
        #endregion
    }
}
