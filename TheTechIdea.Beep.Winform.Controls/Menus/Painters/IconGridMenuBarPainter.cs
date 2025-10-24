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
    /// Icon Grid menu bar painter - displays menu items in a grid organized by categories
    /// </summary>
    public sealed class IconGridMenuBarPainter : MenuBarPainterBase
    {
        #region Fields
        private Dictionary<string, List<SimpleItem>> _categorizedItems = new Dictionary<string, List<SimpleItem>>();
        private List<Rectangle> _iconRects = new List<Rectangle>();
        private List<SimpleItem> _flatItems = new List<SimpleItem>();
        #endregion

        #region MenuBarPainterBase Implementation
        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (drawingRect.IsEmpty || ctx == null)
                return ctx ?? new MenuBarContext();

            UpdateContextColors(ctx);
            ctx.DrawingRect = drawingRect;

            int padding = ScaleValue(16);
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -padding, -padding);
            ctx.MenuItemsRect = ctx.ContentRect;

            // Organize items by category
            OrganizeByCategory(ctx);

            // Calculate icon grid layout
            CalculateIconGridRects(ctx);

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
            if (g == null || ctx == null || ctx.MenuItems == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int y = ctx.ContentRect.Y;

            // Draw each category section
            foreach (var category in _categorizedItems)
            {
                // Draw category header
                y = DrawCategoryHeader(g, category.Key, y, ctx);

                // Draw icons in this category
                y = DrawCategoryIcons(g, category.Value, y, ctx);

                y += ScaleValue(16); // Space between categories
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Draw hover effects
            for (int i = 0; i < _iconRects.Count; i++)
            {
                if (IsAreaHovered($"Icon_{i}"))
                {
                    DrawIconHoverEffect(g, _iconRects[i]);
                }
            }

            // Draw selection effect
            if (ctx.SelectedIndex >= 0 && ctx.SelectedIndex < _flatItems.Count)
            {
                int visualIndex = _flatItems.FindIndex(item => 
                    ctx.MenuItems.IndexOf(item) == ctx.SelectedIndex);
                
                if (visualIndex >= 0 && visualIndex < _iconRects.Count)
                {
                    DrawIconSelectionEffect(g, _iconRects[visualIndex], ctx);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null || ctx == null) return;

            ClearOwnerHitAreas();

            // Add hit areas for each icon
            for (int i = 0; i < _iconRects.Count && i < _flatItems.Count; i++)
            {
                var item = _flatItems[i];
                int originalIndex = ctx.MenuItems.IndexOf(item);
                int iconIndex = i;

                AddHitAreaToOwner($"Icon_{i}", _iconRects[i], () =>
                {
                    if (item.IsEnabled && originalIndex >= 0)
                    {
                        ctx.SelectedIndex = originalIndex;
                        notifyAreaHit?.Invoke($"Icon_{iconIndex}", _iconRects[iconIndex]);
                        SafeInvalidate();
                    }
                });
            }
        }
        #endregion

        #region Private Methods
        private void OrganizeByCategory(MenuBarContext ctx)
        {
            _categorizedItems.Clear();

            if (ctx.MenuItems == null) return;

            // Group items by their Category property
            foreach (var item in ctx.MenuItems)
            {
                string categoryName = item.Category.ToString() != "NONE" 
                    ? item.Category.ToString() 
                    : "General";

                if (!_categorizedItems.ContainsKey(categoryName))
                {
                    _categorizedItems[categoryName] = new List<SimpleItem>();
                }

                _categorizedItems[categoryName].Add(item);
            }
        }

        private void CalculateIconGridRects(MenuBarContext ctx)
        {
            _iconRects.Clear();
            _flatItems.Clear();

            if (ctx.ContentRect.IsEmpty) return;

            int availableWidth = ctx.ContentRect.Width;
            int iconSize = ScaleValue(48);
            int gridSpacing = ScaleValue(16);
            int categoryHeaderHeight = ScaleValue(32);
            int iconCellWidth = iconSize + gridSpacing;
            int iconsPerRow = Math.Max(1, availableWidth / iconCellWidth);

            int x = ctx.ContentRect.X;
            int y = ctx.ContentRect.Y;

            // Ensure item height is calculated dynamically based on font size and padding
            const int MIN_ICON_SIZE = 48; // Minimum icon size for modern design
            ctx.ItemHeight = Math.Max(ctx.TextFont.Height + MIN_ICON_SIZE + 12, MIN_ICON_SIZE + 24);

            foreach (var category in _categorizedItems)
            {
                // Account for category header
                y += categoryHeaderHeight + ScaleValue(8);

                int col = 0;
                foreach (var item in category.Value)
                {
                    int iconX = x + col * iconCellWidth + (iconCellWidth - iconSize) / 2;
                    var iconRect = new Rectangle(iconX, y, iconSize, iconSize + ScaleValue(20)); // Extra space for text

                    _iconRects.Add(iconRect);
                    _flatItems.Add(item);

                    col++;
                    if (col >= iconsPerRow)
                    {
                        col = 0;
                        y += iconSize + ScaleValue(24) + gridSpacing; // Icon + text + spacing
                    }
                }

                // Move to next row if not at start of row
                if (col > 0)
                {
                    y += iconSize + ScaleValue(24) + gridSpacing;
                }

                y += gridSpacing; // Extra space between categories
            }
        }

        private int DrawCategoryHeader(Graphics g, string categoryName, int y, MenuBarContext ctx)
        {
            int categoryHeaderHeight = ScaleValue(32);
            var headerRect = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, categoryHeaderHeight);

            // Draw category name
            using (var font = new Font(ctx.TextFont.FontFamily, ctx.TextFont.Size + 1, FontStyle.Bold))
            using (var brush = new SolidBrush(GetItemForegroundColor()))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(categoryName, font, brush, headerRect, sf);
            }

            // Draw separator line
            int lineY = headerRect.Bottom - 1;
            using (var pen = new Pen(GetBorderColor()))
            {
                g.DrawLine(pen, headerRect.X, lineY, headerRect.Right, lineY);
            }

            return headerRect.Bottom + 8;
        }

        private int DrawCategoryIcons(Graphics g, List<SimpleItem> items, int startY, MenuBarContext ctx)
        {
            if (items == null || items.Count == 0) return startY;

            int availableWidth = ctx.ContentRect.Width;
            int iconSize = ScaleValue(48);
            int gridSpacing = ScaleValue(16);
            int iconCellWidth = iconSize + gridSpacing;
            int iconsPerRow = Math.Max(1, availableWidth / iconCellWidth);

            int x = ctx.ContentRect.X;
            int y = startY;
            int col = 0;

            foreach (var item in items)
            {
                int iconX = x + col * iconCellWidth + (iconCellWidth - iconSize) / 2;
                var iconRect = new Rectangle(iconX, y, iconSize, iconSize);

                // Draw icon
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                   // var iconColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
                    if (item.IsEnabled)
                    {
                        StyledImagePainter.Paint(g, iconRect, item.ImagePath);

                    }
                    else
                    {
                        StyledImagePainter.PaintDisabled(g, iconRect, item.ImagePath, GetDisabledForegroundColor());
                    }
                    ;
                }
                else
                {
                    // Draw placeholder
                    using (var brush = new SolidBrush(GetBorderColor()))
                    {
                        g.FillRectangle(brush, iconRect);
                    }
                }

                // Draw text below icon
                var textRect = new Rectangle(iconX - ScaleValue(10), y + iconSize + ScaleValue(4), iconSize + ScaleValue(20), ScaleValue(20));
                var textColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
                using (var brush = new SolidBrush(textColor))
                using (var font = new Font(ctx.TextFont.FontFamily, ctx.TextFont.Size - 1))
                {
                    var sf = new StringFormat 
                    { 
                        Alignment = StringAlignment.Center, 
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    g.DrawString(item.Text ?? "", font, brush, textRect, sf);
                }

                col++;
                if (col >= iconsPerRow)
                {
                    col = 0;
                    y += iconSize + ScaleValue(24) + gridSpacing;
                }
            }

            // Move to next row if not at start of row
            if (col > 0)
            {
                y += iconSize + ScaleValue(24) + gridSpacing;
            }

            return y;
        }

        private void DrawIconHoverEffect(Graphics g, Rectangle rect)
        {
            var expandedRect = Rectangle.Inflate(rect, 6, 6); // Slightly larger hover effect
            using (var brush = new SolidBrush(Color.FromArgb(50, GetAccentColor()))) // More visible hover effect
            {
                g.FillEllipse(brush, expandedRect);
            }
        }

        private void DrawIconSelectionEffect(Graphics g, Rectangle rect, MenuBarContext ctx)
        {
            var expandedRect = Rectangle.Inflate(rect, 4, 4);
            using (var pen = new Pen(GetAccentColor(), 4)) // Slightly thicker border for better visibility
            {
                g.DrawRectangle(pen, expandedRect);
            }
        }
        #endregion
    }
}
