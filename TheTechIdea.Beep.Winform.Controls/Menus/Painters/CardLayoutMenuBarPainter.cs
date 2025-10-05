using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Painters
{
    /// <summary>
    /// Card Layout menu bar painter - menu items displayed as cards with icons, titles, and descriptions
    /// </summary>
    public sealed class CardLayoutMenuBarPainter : MenuBarPainterBase
    {
        #region Fields
        private List<Rectangle> _cardRects = new List<Rectangle>();
        private const int CARD_WIDTH = 180;
        private const int CARD_HEIGHT = 120;
        private const int CARD_SPACING = 12;
        private const int CARD_PADDING = 12;
        #endregion

        #region MenuBarPainterBase Implementation
        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (drawingRect.IsEmpty || ctx == null)
                return ctx ?? new MenuBarContext();

            UpdateContextColors(ctx);
            ctx.DrawingRect = drawingRect;

            int padding = 16;
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -padding, -padding);
            ctx.MenuItemsRect = ctx.ContentRect;

            // Calculate card grid layout
            CalculateCardRects(ctx);

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

            // Draw each card
            for (int i = 0; i < _cardRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var item = ctx.MenuItems[i];
                var rect = _cardRects[i];
                DrawCard(g, rect, item, ctx, i);
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Draw hover effects on cards
            for (int i = 0; i < _cardRects.Count; i++)
            {
                if (IsAreaHovered($"Card_{i}"))
                {
                    var rect = _cardRects[i];
                    DrawCardHoverEffect(g, rect);
                }
            }

            // Draw selection indicator
            if (ctx.SelectedIndex >= 0 && ctx.SelectedIndex < _cardRects.Count)
            {
                var rect = _cardRects[ctx.SelectedIndex];
                DrawCardSelectionEffect(g, rect, ctx);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null || ctx == null) return;

            ClearOwnerHitAreas();

            // Add hit areas for each card
            for (int i = 0; i < _cardRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var item = ctx.MenuItems[i];
                int cardIndex = i;

                AddHitAreaToOwner($"Card_{i}", _cardRects[i], () =>
                {
                    if (item.IsEnabled)
                    {
                        ctx.SelectedIndex = cardIndex;
                        notifyAreaHit?.Invoke($"Card_{cardIndex}", _cardRects[cardIndex]);
                        SafeInvalidate();
                    }
                });
            }
        }
        #endregion

        #region Private Methods
        private void CalculateCardRects(MenuBarContext ctx)
        {
            _cardRects.Clear();

            if (ctx.MenuItems == null || ctx.ContentRect.IsEmpty) return;

            int x = ctx.ContentRect.X;
            int y = ctx.ContentRect.Y;
            int availableWidth = ctx.ContentRect.Width;

            // Calculate how many cards fit per row
            int cardsPerRow = Math.Max(1, (availableWidth + CARD_SPACING) / (CARD_WIDTH + CARD_SPACING));

            int currentRow = 0;
            int currentCol = 0;

            foreach (var item in ctx.MenuItems)
            {
                int cardX = x + currentCol * (CARD_WIDTH + CARD_SPACING);
                int cardY = y + currentRow * (CARD_HEIGHT + CARD_SPACING);

                var cardRect = new Rectangle(cardX, cardY, CARD_WIDTH, CARD_HEIGHT);
                _cardRects.Add(cardRect);

                currentCol++;
                if (currentCol >= cardsPerRow)
                {
                    currentCol = 0;
                    currentRow++;
                }
            }
        }

        private void DrawCard(Graphics g, Rectangle rect, SimpleItem item, MenuBarContext ctx, int index)
        {
            if (g == null || item == null) return;

            // Draw card shadow
            DrawCardShadow(g, rect);

            // Draw card background with rounded corners
            var bgColor = (index == ctx.SelectedIndex) ? GetSelectedBackgroundColor() : GetItemBackgroundColor();
            using (var path = CreateRoundedPath(rect, 8))
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }

            // Draw card border
            using (var path = CreateRoundedPath(rect, 8))
            using (var pen = new Pen(GetBorderColor(), 1))
            {
                g.DrawPath(pen, path);
            }

            // Draw icon at top
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int iconSize = 48;
                var iconRect = new Rectangle(
                    rect.X + (rect.Width - iconSize) / 2,
                    rect.Y + CARD_PADDING,
                    iconSize, iconSize);
                var iconColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
                MenuBarRenderingHelpers.DrawMenuItemIcon(g, iconRect, item.ImagePath, iconColor);
            }

            // Draw title text
            var titleColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
            using (var brush = new SolidBrush(titleColor))
            using (var titleFont = new Font(ctx.TextFont.FontFamily, ctx.TextFont.Size + 1, FontStyle.Bold))
            {
                var titleRect = new Rectangle(
                    rect.X + CARD_PADDING,
                    rect.Y + 70,
                    rect.Width - CARD_PADDING * 2,
                    20);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(item.Text ?? "", titleFont, brush, titleRect, sf);
            }

            // Draw description if available (using Category as description placeholder)
            if (item.Category != DatasourceCategory.NONE)
            {
                var descColor = Color.FromArgb(128, titleColor);
                using (var brush = new SolidBrush(descColor))
                using (var descFont = new Font(ctx.TextFont.FontFamily, ctx.TextFont.Size - 1))
                {
                    var descRect = new Rectangle(
                        rect.X + CARD_PADDING,
                        rect.Y + 92,
                        rect.Width - CARD_PADDING * 2,
                        rect.Height - 92 - CARD_PADDING);
                    var sf = new StringFormat 
                    { 
                        Alignment = StringAlignment.Center, 
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    g.DrawString(item.Category.ToString(), descFont, brush, descRect, sf);
                }
            }
        }

        private void DrawCardShadow(Graphics g, Rectangle rect)
        {
            var shadowRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
            using (var path = CreateRoundedPath(shadowRect, 8))
            using (var brush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawCardHoverEffect(Graphics g, Rectangle rect)
        {
            using (var path = CreateRoundedPath(rect, 8))
            using (var brush = new SolidBrush(Color.FromArgb(30, GetAccentColor())))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawCardSelectionEffect(Graphics g, Rectangle rect, MenuBarContext ctx)
        {
            using (var path = CreateRoundedPath(rect, 8))
            using (var pen = new Pen(GetAccentColor(), 3))
            {
                g.DrawPath(pen, path);
            }
        }
        #endregion
    }
}
