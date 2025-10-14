using System;
using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Painters
{
    /// <summary>
    /// Material Design menu bar painter - implements Material Design 3.0 principles
    /// </summary>
    public sealed class MaterialMenuBarPainter : MenuBarPainterBase
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
            
            // Material Design spacing and sizing
            ctx.DrawingRect = drawingRect;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -8, -4);
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.CornerRadius = 12; // Material Design rounded corners
            ctx.ItemSpacing = 8; // Increased spacing for Material Design
            ctx.ItemPadding = 16;
            
            // Ensure adequate item height for Material Design
            if (ctx.ItemHeight < 40)
            {
                ctx.ItemHeight = 40; // Material Design minimum touch target
            }

            CalculateMenuItemRects(ctx);
            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx.DrawingRect.IsEmpty) return;

            // Material Design surface
            using var bgBrush = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // Material elevation shadow (simplified)
            DrawMaterialElevation(g, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx?.MenuItems == null) return;

            for (int i = 0; i < ctx.MenuItems.Count && i < _itemRects.Count; i++)
            {
                DrawMaterialMenuItem(g, _itemRects[i], ctx.MenuItems[i], ctx, i);
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Material ripple effects and state overlays
            for (int i = 0; i < _itemRects.Count; i++)
            {
                if (IsAreaHovered($"MenuItem_{i}"))
                {
                    DrawMaterialRipple(g, _itemRects[i], GetAccentColor());
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
        private void CalculateMenuItemRects(MenuBarContext ctx)
        {
            _itemRects.Clear();
            if (ctx?.MenuItems == null || ctx.MenuItemsRect.IsEmpty) return;

            // Calculate item rectangles with proper text sizing for Material Design
            _itemRects = CalculateMaterialMenuItemRects(ctx);

            ctx.ItemRects.Clear();
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                ctx.ItemRects.Add((_itemRects[i], i, ctx.MenuItems[i]));
            }
        }

        private List<Rectangle> CalculateMaterialMenuItemRects(MenuBarContext ctx)
        {
            var rects = new List<Rectangle>();
            if (ctx.MenuItemsRect.IsEmpty || ctx.MenuItems == null || ctx.MenuItems.Count == 0) 
                return rects;

            int currentX = ctx.MenuItemsRect.X;
            int itemY = ctx.MenuItemsRect.Y + (ctx.MenuItemsRect.Height - ctx.ItemHeight) / 2;
            int minItemWidth = 100; // Larger minimum width for Material Design

            foreach (var item in ctx.MenuItems)
            {
                // Calculate item width with proper font measurement for Material Design
                int itemWidth = CalculateMaterialItemWidth(item, ctx);
                
                // Ensure item fits in available space
                if (currentX + itemWidth > ctx.MenuItemsRect.Right)
                {
                    itemWidth = ctx.MenuItemsRect.Right - currentX;
                    if (itemWidth < minItemWidth / 2) break; // Not enough space
                }

                var itemRect = new Rectangle(currentX, itemY, itemWidth, ctx.ItemHeight);
                rects.Add(itemRect);

                currentX += itemWidth + ctx.ItemSpacing;
                if (currentX >= ctx.MenuItemsRect.Right) break;
            }

            return rects;
        }

        private int CalculateMaterialItemWidth(SimpleItem item, MenuBarContext ctx)
        {
            if (item == null) return 100;

            int width = ctx.ItemPadding * 2; // Material Design padding (32px total)
            
            // Add icon width with Material spacing
            if (ctx.ShowIcons && !string.IsNullOrEmpty(item.ImagePath))
            {
                width += ctx.IconSize.Width + 8; // Icon + Material spacing
            }

            // Add text width with proper font measurement
            if (!string.IsNullOrEmpty(item.Text) && ctx.TextFont != null)
            {
                using var tempBitmap = new Bitmap(1, 1);
                using var g = Graphics.FromImage(tempBitmap);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                
                var textSize = TextUtils.MeasureText(g,item.Text, ctx.TextFont);
                width += (int)Math.Ceiling(textSize.Width) + 8; // Text + Material spacing
            }

            // Add dropdown indicator width with Material spacing
            if (ctx.ShowDropdownIndicators && item.Children?.Count > 0)
            {
                width += 16 + 8; // Material dropdown arrow + spacing
            }

            return Math.Max(width, 100); // Material Design minimum width
        }

        private void DrawMaterialMenuItem(Graphics g, Rectangle rect, SimpleItem item, MenuBarContext ctx, int index)
        {
            if (g == null || rect.IsEmpty || item == null) return;

            bool isSelected = ctx.SelectedIndex == index;
            bool isHovered = IsAreaHovered($"MenuItem_{index}");
            bool isEnabled = ctx.IsItemEnabled(item);

            // Material Design container
            if (isSelected || isHovered)
            {
                var containerColor = isSelected ? GetSelectedBackgroundColor() : GetHoverBackgroundColor();
                using var brush = new SolidBrush(Color.FromArgb(100, containerColor));
                using var path = CreateRoundedPath(rect, ctx.CornerRadius);
                g.FillPath(brush, path);
            }

            // Draw content using Material typography with proper contrast
            DrawMaterialMenuItemContent(g, rect, item, ctx, isSelected, isHovered, isEnabled);
        }

        private void DrawMaterialMenuItemContent(Graphics g, Rectangle itemRect, SimpleItem item, 
            MenuBarContext ctx, bool isSelected, bool isHovered, bool isEnabled)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Calculate layout with Material spacing
            var layout = MenuBarRenderingHelpers.CalculateMenuItemLayout(
                itemRect, item, ctx.TextFont, ctx.IconSize, ctx.ShowIcons, ctx.ShowDropdownIndicators);

            // Determine text color based on Material Design principles
            Color textColor = ctx.ItemForeColor;
            if (!isEnabled)
            {
                textColor = ctx.ItemDisabledForeColor;
            }
            else if (isSelected)
            {
                // Use high contrast color for selected items
                textColor = Color.White;
            }
            else if (isHovered)
            {
                textColor = ctx.ItemHoverForeColor;
            }

            // Material icon rendering
            if (ctx.ShowIcons && !string.IsNullOrEmpty(item.ImagePath) && !layout.IconRect.IsEmpty)
            {
                DrawMaterialIcon(g, layout.IconRect, item.ImagePath, textColor, isEnabled);
            }

            // Material text rendering with proper contrast
            if (!string.IsNullOrEmpty(item.Text) && !layout.TextRect.IsEmpty)
            {
                DrawMaterialText(g, layout.TextRect, item.Text, ctx.TextFont, textColor);
            }

            // Material dropdown indicator
            if (ctx.ShowDropdownIndicators && item.Children?.Count > 0 && !layout.DropdownRect.IsEmpty)
            {
                DrawMaterialDropdownIndicator(g, layout.DropdownRect, textColor);
            }
        }

        private void DrawMaterialIcon(Graphics g, Rectangle iconRect, string iconPath, Color color, bool isEnabled)
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
                // Fallback to Material placeholder if image loading fails
                var alpha = isEnabled ? 255 : 100;
                using var brush = new SolidBrush(Color.FromArgb(alpha, color));
                
                var padding = 2;
                var innerRect = Rectangle.Inflate(iconRect, -padding, -padding);
                
                g.FillRectangle(brush, innerRect);
            }
        }

        private void DrawMaterialText(Graphics g, Rectangle textRect, string text, Font font, Color color)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            using var brush = new SolidBrush(color);
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            // Use anti-aliasing for crisp Material typography
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.DrawString(text, font, brush, textRect, format);
        }

        private void DrawMaterialDropdownIndicator(Graphics g, Rectangle rect, Color color)
        {
            // Material dropdown indicator - filled triangle
            using var brush = new SolidBrush(color);
            
            var size = Math.Min(rect.Width, rect.Height) / 3;
            var centerX = rect.X + rect.Width / 2;
            var centerY = rect.Y + rect.Height / 2;
            
            var points = new Point[]
            {
                new Point(centerX - size / 2, centerY - size / 4),
                new Point(centerX + size / 2, centerY - size / 4),
                new Point(centerX, centerY + size / 4)
            };
            
            g.FillPolygon(brush, points);
        }

        private void DrawMaterialElevation(Graphics g, Rectangle rect)
        {
            // Simplified Material elevation shadow
            using var shadowBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
            var shadowRect = new Rectangle(rect.X, rect.Y + 2, rect.Width, rect.Height);
            g.FillRectangle(shadowBrush, shadowRect);
        }

        private void DrawMaterialRipple(Graphics g, Rectangle rect, Color rippleColor)
        {
            // Simplified Material ripple effect
            using var brush = new SolidBrush(Color.FromArgb(30, rippleColor));
            using var path = CreateRoundedPath(rect, 12);
            g.FillPath(brush, path);
        }
        #endregion
    }
}