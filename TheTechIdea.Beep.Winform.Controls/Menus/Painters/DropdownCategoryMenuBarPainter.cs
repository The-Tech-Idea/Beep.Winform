using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Painters
{
    /// <summary>
    /// Dropdown Category menu bar painter - hierarchical navigation with dropdown categories
    /// </summary>
    public sealed class DropdownCategoryMenuBarPainter : MenuBarPainterBase
    {
        #region Fields
        private List<Rectangle> _categoryRects = new List<Rectangle>();
        private Dictionary<int, List<SimpleItem>> _categorizedItems = new Dictionary<int, List<SimpleItem>>();
        private int _openCategoryIndex = -1;
        private List<Rectangle> _dropdownItemRects = new List<Rectangle>();
        #endregion

        #region MenuBarPainterBase Implementation
        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (drawingRect.IsEmpty || ctx == null)
                return ctx ?? new MenuBarContext();

            UpdateContextColors(ctx);
            ctx.DrawingRect = drawingRect;

            int padding = ScaleValue(8);
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -padding, -padding);
            ctx.MenuItemsRect = ctx.ContentRect;

            // Dynamically calculate item height
            ctx.ItemHeight = CalculateDynamicItemHeight(ctx);

            // Organize items into categories (top-level items are categories)
            OrganizeItemsByCategory(ctx);

            // Calculate category button rectangles
            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx.DrawingRect.IsEmpty) return;

            using var bgBrush = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // Draw border
            using var borderPen = new Pen(GetBorderColor());
            g.DrawRectangle(borderPen, ctx.DrawingRect.X, ctx.DrawingRect.Y,
                ctx.DrawingRect.Width - ScaleValue(1), ctx.DrawingRect.Height - ScaleValue(1));
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null || ctx.MenuItems == null) return;

            // Draw category buttons
            for (int i = 0; i < _categoryRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var item = ctx.MenuItems[i];
                var rect = _categoryRects[i];
                bool isOpen = (_openCategoryIndex == i);

                DrawCategoryButton(g, rect, item, ctx, i, isOpen);
            }

            // Draw open dropdown menu
            if (_openCategoryIndex >= 0 && _openCategoryIndex < ctx.MenuItems.Count)
            {
                DrawDropdownMenu(g, _openCategoryIndex, ctx);
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Draw hover effects on category buttons
            for (int i = 0; i < _categoryRects.Count; i++)
            {
                if (IsAreaHovered($"Category_{i}"))
                {
                    DrawHoverEffect(g, _categoryRects[i], GetHoverBackgroundColor(), ScaleValue(4));
                }
            }

            // Draw hover effects on dropdown items
            for (int i = 0; i < _dropdownItemRects.Count; i++)
            {
                if (IsAreaHovered($"DropdownItem_{i}"))
                {
                    DrawHoverEffect(g, _dropdownItemRects[i], GetHoverBackgroundColor(), ScaleValue(2));
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null || ctx == null) return;

            ClearOwnerHitAreas();

            // Add hit areas for category buttons
            for (int i = 0; i < _categoryRects.Count && i < ctx.MenuItems.Count; i++)
            {
                int catIndex = i;
                AddHitAreaToOwner($"Category_{i}", _categoryRects[i], () =>
                {
                    _openCategoryIndex = (_openCategoryIndex == catIndex) ? -1 : catIndex;
                    notifyAreaHit?.Invoke($"Category_{catIndex}", _categoryRects[catIndex]);
                    SafeInvalidate();
                });
            }

            // Add hit areas for dropdown items if dropdown is open
            if (_openCategoryIndex >= 0 && _categorizedItems.ContainsKey(_openCategoryIndex))
            {
                var children = _categorizedItems[_openCategoryIndex];
                for (int i = 0; i < _dropdownItemRects.Count && i < children.Count; i++)
                {
                    int itemIndex = i;
                    AddHitAreaToOwner($"DropdownItem_{i}", _dropdownItemRects[i], () =>
                    {
                        _openCategoryIndex = -1; // Close dropdown
                        notifyAreaHit?.Invoke($"DropdownItem_{itemIndex}", _dropdownItemRects[itemIndex]);
                        SafeInvalidate();
                    });
                }
            }
        }
        #endregion

        #region Private Methods
        private void OrganizeItemsByCategory(MenuBarContext ctx)
        {
            _categorizedItems.Clear();

            if (ctx.MenuItems == null) return;

            // Top-level items are categories, their children are dropdown items
            for (int i = 0; i < ctx.MenuItems.Count; i++)
            {
                var item = ctx.MenuItems[i];
                if (item.Children != null && item.Children.Any())
                {
                    _categorizedItems[i] = item.Children.ToList();
                }
                else
                {
                    _categorizedItems[i] = new List<SimpleItem>();
                }
            }
        }

        private void CalculateCategoryRects(MenuBarContext ctx)
        {
            _categoryRects.Clear();

            if (ctx.MenuItems == null || ctx.ContentRect.IsEmpty) return;

            int x = ctx.ContentRect.X;
            int y = ctx.ContentRect.Y;
            int height = ctx.ItemHeight;
            int spacing = ScaleValue(4);

            foreach (var item in ctx.MenuItems)
            {
                int width = MeasureCategoryWidth(item, ctx);
                var rect = new Rectangle(x, y, width, height);
                _categoryRects.Add(rect);
                x += width + spacing;
            }
        }

        private int MeasureCategoryWidth(SimpleItem item, MenuBarContext ctx)
        {
            if (item == null) return ScaleValue(80);

            using var bmp = new Bitmap(1, 1);
            using var g = Graphics.FromImage(bmp);
            var textSize = TextUtils.MeasureText(g,item.Text ?? "", ctx.TextFont);

            int width = (int)Math.Ceiling(textSize.Width) + ScaleValue(40); // Padding + arrow space
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += ctx.IconSize.Width + ScaleValue(8);
            }

            return Math.Max(width, ScaleValue(80));
        }

        private void DrawCategoryButton(Graphics g, Rectangle rect, SimpleItem item, MenuBarContext ctx, int index, bool isOpen)
        {
            if (g == null || item == null) return;

            // Background
            var bgColor = isOpen ? GetSelectedBackgroundColor() : GetItemBackgroundColor();
            if (index == ctx.SelectedIndex)
            {
                bgColor = GetSelectedBackgroundColor();
            }

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Icon
            int iconX = rect.X + ScaleValue(8);
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(iconX, rect.Y + (rect.Height - ctx.IconSize.Height) / 2,
                    ctx.IconSize.Width, ctx.IconSize.Height);
                if (item.IsEnabled)
                {
                    StyledImagePainter.Paint(g, iconRect, item.ImagePath);

                }
                else
                {
                    StyledImagePainter.PaintDisabled(g, iconRect, item.ImagePath,  GetDisabledForegroundColor());
                }
                iconX += ctx.IconSize.Width + ScaleValue(8);
            }

            // Text
            var textColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
            using (var brush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(iconX, rect.Y, rect.Width - (iconX - rect.X) - ScaleValue(20), rect.Height);
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text ?? "", ctx.TextFont, brush, textRect, sf);
            }

            // Dropdown arrow if has children
            if (_categorizedItems.ContainsKey(index) && _categorizedItems[index].Count > 0)
            {
                DrawDropdownArrow(g, rect, isOpen);
            }
        }

        private void DrawDropdownArrow(Graphics g, Rectangle rect, bool isOpen)
        {
            int arrowSize = ScaleValue(6);
            int arrowX = rect.Right - ScaleValue(12);
            int arrowY = rect.Y + rect.Height / 2;

            using var pen = new Pen(GetItemForegroundColor(), ScaleValue(2));
            if (isOpen)
            {
                // Up arrow
                g.DrawLine(pen, arrowX, arrowY, arrowX + arrowSize / 2, arrowY - arrowSize / 2);
                g.DrawLine(pen, arrowX + arrowSize / 2, arrowY - arrowSize / 2, arrowX + arrowSize, arrowY);
            }
            else
            {
                // Down arrow
                g.DrawLine(pen, arrowX, arrowY - arrowSize / 2, arrowX + arrowSize / 2, arrowY);
                g.DrawLine(pen, arrowX + arrowSize / 2, arrowY, arrowX + arrowSize, arrowY - arrowSize / 2);
            }
        }

        private void DrawDropdownMenu(Graphics g, int categoryIndex, MenuBarContext ctx)
        {
            if (!_categorizedItems.ContainsKey(categoryIndex)) return;

            var children = _categorizedItems[categoryIndex];
            if (children.Count == 0 || categoryIndex >= _categoryRects.Count) return;

            var categoryRect = _categoryRects[categoryIndex];
            
            // Calculate dropdown position and size
            int dropdownX = categoryRect.X;
            int dropdownY = categoryRect.Bottom + ScaleValue(2);
            int dropdownWidth = ScaleValue(200);
            int itemHeight = ctx.ItemHeight;
            int dropdownHeight = children.Count * itemHeight + ScaleValue(8);

            var dropdownRect = new Rectangle(dropdownX, dropdownY, dropdownWidth, dropdownHeight);

            // Draw dropdown background with shadow
            DrawDropdownShadow(g, dropdownRect);
            using (var brush = new SolidBrush(GetBackgroundColor()))
            {
                g.FillRectangle(brush, dropdownRect);
            }
            using (var pen = new Pen(GetBorderColor()))
            {
                g.DrawRectangle(pen, dropdownRect);
            }

            // Draw dropdown items
            _dropdownItemRects.Clear();
            int y = dropdownRect.Y + ScaleValue(4);

            foreach (var child in children)
            {
                var itemRect = new Rectangle(dropdownRect.X + ScaleValue(4), y, dropdownRect.Width - ScaleValue(8), itemHeight);
                _dropdownItemRects.Add(itemRect);
                DrawDropdownItem(g, itemRect, child, ctx);
                y += itemHeight;
            }
        }

        private void DrawDropdownShadow(Graphics g, Rectangle rect)
        {
            // Simple shadow effect
            var shadowRect = new Rectangle(rect.X + ScaleValue(4), rect.Y + ScaleValue(4), rect.Width, rect.Height);
            using var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
            g.FillRectangle(shadowBrush, shadowRect);
        }

        private void DrawDropdownItem(Graphics g, Rectangle rect, SimpleItem item, MenuBarContext ctx)
        {
            if (g == null || item == null) return;

            // Background
            var bgColor = GetItemBackgroundColor();
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Icon
            int iconSize = ScaleValue(16);
            int iconX = rect.X + ScaleValue(4);
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(iconX, rect.Y + (rect.Height - iconSize) / 2, iconSize, iconSize);
                if (item.IsEnabled)
                {
                    StyledImagePainter.Paint(g, iconRect, item.ImagePath);

                }
                else
                {
                    StyledImagePainter.PaintDisabled(g, iconRect, item.ImagePath, GetDisabledForegroundColor());
                }
                iconX += ScaleValue(24);
            }

            // Text
            var textColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
            using (var brush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(iconX, rect.Y, rect.Width - (iconX - rect.X), rect.Height);
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text ?? "", ctx.TextFont, brush, textRect, sf);
            }
        }

        private int CalculateDynamicItemHeight(MenuBarContext ctx)
        {
            if (ctx == null || ctx.TextFont == null)
                return ScaleValue(28); // Default dropdown item height

            // Calculate height based on font size and padding
            int textHeight = (int)ctx.TextFont.GetHeight() + ScaleValue(8);
            return Math.Max(textHeight, ScaleValue(28));
        }
        #endregion
    }
}
