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
    /// Tab menu bar painter - displays menu items as tabs with active tab highlighting
    /// </summary>
    public sealed class TabMenuBarPainter : MenuBarPainterBase
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
            
            ctx.DrawingRect = drawingRect;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -4, -2);
            ctx.MenuItemsRect = ctx.ContentRect;
            ctx.CornerRadius = 6; // Rounded tab corners
            ctx.ItemSpacing = 2; // Small gap between tabs
            ctx.ItemPadding = 16;

            // Ensure item height is calculated dynamically based on font size and padding
            const int MIN_TAB_HEIGHT = 36; // Minimum tab height for modern design
            ctx.ItemHeight = Math.Max(ctx.TextFont.Height + 12, MIN_TAB_HEIGHT);

            CalculateTabRects(ctx);
            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx.DrawingRect.IsEmpty) return;

            // Tab container background
            using var bgBrush = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // Tab bar bottom border
            using var borderPen = new Pen(GetBorderColor());
            g.DrawLine(borderPen, ctx.DrawingRect.X, ctx.DrawingRect.Bottom - 1, 
                       ctx.DrawingRect.Right, ctx.DrawingRect.Bottom - 1);
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx?.MenuItems == null) return;

            // Draw inactive tabs first, then active tab on top
            for (int i = 0; i < ctx.MenuItems.Count && i < _itemRects.Count; i++)
            {
                if (ctx.SelectedIndex != i)
                {
                    DrawTab(g, _itemRects[i], ctx.MenuItems[i], ctx, i, false);
                }
            }

            // Draw active tab last (on top)
            if (ctx.SelectedIndex >= 0 && ctx.SelectedIndex < _itemRects.Count)
            {
                DrawTab(g, _itemRects[ctx.SelectedIndex], ctx.MenuItems[ctx.SelectedIndex], 
                       ctx, ctx.SelectedIndex, true);
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Tab hover effects (only for inactive tabs)
            for (int i = 0; i < _itemRects.Count; i++)
            {
                if (i != ctx.SelectedIndex && IsAreaHovered($"MenuItem_{i}"))
                {
                    DrawTabHoverEffect(g, _itemRects[i], GetHoverBackgroundColor());
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
        private void CalculateTabRects(MenuBarContext ctx)
        {
            _itemRects.Clear();
            if (ctx?.MenuItems == null || ctx.MenuItemsRect.IsEmpty) return;

            int currentX = ctx.MenuItemsRect.X;
            int tabHeight = ctx.MenuItemsRect.Height - 4; // Leave space for bottom border
            int tabY = ctx.MenuItemsRect.Y + 2;

            foreach (var item in ctx.MenuItems)
            {
                int tabWidth = MenuBarRenderingHelpers.CalculateMenuItemWidth(item, tabHeight, ctx.TextFont);
                tabWidth = Math.Max(tabWidth, 80); // Minimum tab width

                if (currentX + tabWidth > ctx.MenuItemsRect.Right)
                {
                    tabWidth = ctx.MenuItemsRect.Right - currentX;
                    if (tabWidth < 40) break; // Not enough space
                }

                var tabRect = new Rectangle(currentX, tabY, tabWidth, tabHeight);
                _itemRects.Add(tabRect);
                currentX += tabWidth + ctx.ItemSpacing;

                if (currentX >= ctx.MenuItemsRect.Right) break;
            }

            // Update context
            ctx.ItemRects.Clear();
            for (int i = 0; i < _itemRects.Count && i < ctx.MenuItems.Count; i++)
            {
                ctx.ItemRects.Add((_itemRects[i], i, ctx.MenuItems[i]));
            }
        }

        private void DrawTab(Graphics g, Rectangle tabRect, SimpleItem item, MenuBarContext ctx, int index, bool isActive)
        {
            if (g == null || tabRect.IsEmpty || item == null) return;

            // Create tab path
            using var tabPath = MenuBarRenderingHelpers.CreateTabPath(tabRect, ctx.CornerRadius, isActive);

            // Tab background
            Color backgroundColor = isActive ? GetItemBackgroundColor() : Color.FromArgb(245, GetBackgroundColor()); // Slightly lighter inactive background
            using var bgBrush = new SolidBrush(backgroundColor);
            g.FillPath(bgBrush, tabPath);

            // Tab border
            if (!isActive)
            {
                using var borderPen = new Pen(GetBorderColor(), 1.5f); // Slightly thicker border for better visibility
                g.DrawPath(borderPen, tabPath);
            }

            // Tab content
            Color textColor = isActive ? GetAccentColor() : ctx.ItemForeColor;
            DrawTabContent(g, tabRect, item, ctx, textColor);

            // Active tab indicator (bottom border removal)
            if (isActive)
            {
                using var activeBrush = new SolidBrush(backgroundColor);
                g.FillRectangle(activeBrush, tabRect.X, tabRect.Bottom - 3, tabRect.Width, 3); // Slightly thicker indicator
            }
        }

        private void DrawTabContent(Graphics g, Rectangle tabRect, SimpleItem item, MenuBarContext ctx, Color textColor)
        {
            // Calculate content layout
            var layout = MenuBarRenderingHelpers.CalculateMenuItemLayout(
                tabRect, item, ctx.TextFont, ctx.IconSize, ctx.ShowIcons, false); // No dropdown for tabs

            // Draw icon if available
            if (ctx.ShowIcons && !string.IsNullOrEmpty(item.ImagePath) && !layout.IconRect.IsEmpty)
            {
                StyledImagePainter.PaintWithTint(g, layout.IconRect, item.ImagePath, textColor);
            }

            // Draw text
            if (!string.IsNullOrEmpty(item.Text) && !layout.TextRect.IsEmpty)
            {
                MenuBarRenderingHelpers.DrawMenuItemText(g, layout.TextRect, item.Text, ctx.TextFont, textColor);
            }
        }

        private void DrawTabHoverEffect(Graphics g, Rectangle tabRect, Color hoverColor)
        {
            using var tabPath = MenuBarRenderingHelpers.CreateTabPath(tabRect, 6, false);
            using var hoverBrush = new SolidBrush(Color.FromArgb(50, hoverColor)); // Slightly more visible hover effect
            g.FillPath(hoverBrush, tabPath);
        }
        #endregion
    }
}